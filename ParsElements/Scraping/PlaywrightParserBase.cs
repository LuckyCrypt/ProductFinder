using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace ParsElements.Scraping
{
    /// <summary>
    /// База для парсеров, которым нужен реальный браузер (Ozon, Яндекс.Маркет).
    /// Инкапсулирует запуск Chromium со стелс-настройками, навигацию, детект
    /// анти-бот страниц и извлечение цены из JSON-LD. Наследники добавляют
    /// специфику маркетплейса.
    /// </summary>
    public abstract class PlaywrightParserBase : IMarketplaceParser
    {
        public abstract string Code { get; }

        private readonly bool _headless;

        protected PlaywrightParserBase(bool headless = false)
        {
            // Для Ozon/Яндекса headful заметно повышает шанс обойти анти-бот.
            _headless = headless;
        }

        private const string UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";

        public async Task<PriceResult> FetchAsync(string url, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(url))
                return PriceResult.Fail(ScrapeStatus.Error, "Пустой URL");

            try
            {
                using var playwright = await Playwright.CreateAsync();
                await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = _headless,
                    Args = new[] { "--disable-blink-features=AutomationControlled" }
                });

                var context = await browser.NewContextAsync(new BrowserNewContextOptions
                {
                    UserAgent = UserAgent,
                    Locale = "ru-RU"
                });

                var page = await context.NewPageAsync();

                try
                {
                    await page.GotoAsync(url, new PageGotoOptions
                    {
                        Timeout = 45000,
                        WaitUntil = WaitUntilState.DOMContentLoaded
                    });
                }
                catch (TimeoutException)
                {
                    return PriceResult.Fail(ScrapeStatus.Error, "Таймаут загрузки страницы");
                }

                // Дать JS дорисовать цену.
                await page.WaitForTimeoutAsync(3500);

                // 1) Анти-бот?
                var block = await DetectBlockAsync(page);
                if (block != null)
                    return block;

                // 2) JSON-LD (структурированные данные — самый стабильный источник).
                var ldHtml = await GetJsonLdBlocksAsync(page);
                foreach (var json in ldHtml)
                {
                    var priceFromLd = TryPriceFromJsonLd(json);
                    if (priceFromLd is decimal p && p > 0)
                        return PriceResult.Ok(p, inStock: true);
                }

                // 3) DOM-селекторы конкретного маркетплейса.
                var domPrice = await ExtractFromDomAsync(page);
                if (domPrice is decimal dp && dp > 0)
                    return PriceResult.Ok(dp, inStock: true);

                return PriceResult.Fail(ScrapeStatus.NotFound, "Цена не найдена на странице");
            }
            catch (OperationCanceledException)
            {
                return PriceResult.Fail(ScrapeStatus.Error, "Операция отменена/таймаут");
            }
            catch (Exception ex)
            {
                return PriceResult.Fail(ScrapeStatus.Error, ex.Message);
            }
        }

        /// <summary>
        /// Детект анти-бот/капчи. Возвращает PriceResult со статусом, если страница
        /// заблокирована; иначе null. База проверяет общие маркеры, наследник может
        /// расширить.
        /// </summary>
        protected virtual async Task<PriceResult?> DetectBlockAsync(IPage page)
        {
            var currentUrl = page.Url ?? string.Empty;
            if (currentUrl.Contains("captcha", StringComparison.OrdinalIgnoreCase) ||
                currentUrl.Contains("showcaptcha", StringComparison.OrdinalIgnoreCase))
                return PriceResult.Fail(ScrapeStatus.Captcha, "Редирект на страницу капчи");

            string content;
            try { content = await page.ContentAsync(); }
            catch { return null; }

            var lower = content.ToLowerInvariant();
            if (lower.Contains("smartcaptcha") || lower.Contains("подтвердите, что запросы") ||
                lower.Contains("i'm not a robot") || lower.Contains("confirm you are not a robot"))
                return PriceResult.Fail(ScrapeStatus.Captcha, "Обнаружена капча");

            if (lower.Contains("доступ ограничен") || lower.Contains("access denied") ||
                lower.Contains("access to this page has been denied"))
                return PriceResult.Fail(ScrapeStatus.Blocked, "Доступ ограничен маркетплейсом");

            return null;
        }

        /// <summary>Специфичное для маркетплейса извлечение цены из DOM. По умолчанию — нет.</summary>
        protected virtual Task<decimal?> ExtractFromDomAsync(IPage page) => Task.FromResult<decimal?>(null);

        private static async Task<List<string>> GetJsonLdBlocksAsync(IPage page)
        {
            var result = new List<string>();
            var handles = await page.QuerySelectorAllAsync("script[type='application/ld+json']");
            foreach (var h in handles)
            {
                try
                {
                    var txt = await h.InnerTextAsync();
                    if (!string.IsNullOrWhiteSpace(txt))
                        result.Add(txt);
                }
                catch { /* пропускаем битый блок */ }
            }
            return result;
        }

        /// <summary>Ищет offers.price в JSON-LD (Product/Offer, в т.ч. вложенные массивы).</summary>
        internal static decimal? TryPriceFromJsonLd(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                return FindPrice(doc.RootElement);
            }
            catch
            {
                return null;
            }
        }

        private static decimal? FindPrice(JsonElement el)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Object:
                    if (el.TryGetProperty("offers", out var offers))
                    {
                        var p = FindPrice(offers);
                        if (p != null) return p;
                    }
                    if (el.TryGetProperty("price", out var priceEl))
                    {
                        var parsed = ParsePriceToken(priceEl);
                        if (parsed != null) return parsed;
                    }
                    if (el.TryGetProperty("lowPrice", out var lowEl))
                    {
                        var parsed = ParsePriceToken(lowEl);
                        if (parsed != null) return parsed;
                    }
                    foreach (var prop in el.EnumerateObject())
                    {
                        var p = FindPrice(prop.Value);
                        if (p != null) return p;
                    }
                    return null;

                case JsonValueKind.Array:
                    foreach (var item in el.EnumerateArray())
                    {
                        var p = FindPrice(item);
                        if (p != null) return p;
                    }
                    return null;

                default:
                    return null;
            }
        }

        private static decimal? ParsePriceToken(JsonElement el)
        {
            if (el.ValueKind == JsonValueKind.Number)
                return el.GetDecimal();
            if (el.ValueKind == JsonValueKind.String)
                return ParsePriceText(el.GetString());
            return null;
        }

        /// <summary>Достаёт число из "39 990 ₽", "39990.00", "39 990" и т.п.</summary>
        internal static decimal? ParsePriceText(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            // Оставляем цифры, точки и запятые; убираем разделители тысяч (пробелы/nbsp).
            var cleaned = Regex.Replace(text, @"[^\d.,]", string.Empty);
            if (cleaned.Length == 0) return null;

            // Если и точка, и запятая — считаем запятую десятичной (рус. формат) либо наоборот.
            if (cleaned.Contains('.') && cleaned.Contains(','))
                cleaned = cleaned.Replace(".", string.Empty).Replace(",", ".");
            else
                cleaned = cleaned.Replace(",", ".");

            // Может остаться несколько точек (39.990.00) — оставляем последнюю как десятичную.
            var lastDot = cleaned.LastIndexOf('.');
            if (lastDot >= 0)
            {
                var intPart = cleaned[..lastDot].Replace(".", string.Empty);
                var fracPart = cleaned[(lastDot + 1)..];
                cleaned = intPart + "." + fracPart;
            }

            return decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
                ? Math.Round(value, 2)
                : null;
        }
    }
}
