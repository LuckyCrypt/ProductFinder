using System.Text.Json;
using System.Text.RegularExpressions;

namespace ParsElements.Scraping
{
    /// <summary>
    /// Wildberries: самый надёжный источник — цену отдаёт публичный JSON API
    /// (card.wb.ru), браузер не нужен. Из URL карточки достаём артикул (nm) и
    /// запрашиваем детально.
    /// </summary>
    public sealed class WildberriesParser : IMarketplaceParser
    {
        public string Code => "wb";

        private readonly HttpClient _http;

        public WildberriesParser(HttpClient http)
        {
            _http = http;
        }

        public async Task<PriceResult> FetchAsync(string url, CancellationToken ct = default)
        {
            try
            {
                var nm = ExtractArticle(url);
                if (nm is null)
                    return PriceResult.Fail(ScrapeStatus.Error, "Не удалось извлечь артикул (nm) из URL Wildberries");

                var api = $"https://card.wb.ru/cards/v2/detail?appType=1&curr=rub&dest=-1257786&nm={nm}";

                using var req = new HttpRequestMessage(HttpMethod.Get, api);
                req.Headers.TryAddWithoutValidation("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                req.Headers.TryAddWithoutValidation("Accept", "application/json");

                using var resp = await _http.SendAsync(req, ct);
                if (!resp.IsSuccessStatusCode)
                    return PriceResult.Fail(ScrapeStatus.Error, $"WB API вернул {(int)resp.StatusCode}");

                await using var stream = await resp.Content.ReadAsStreamAsync(ct);
                using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

                if (!doc.RootElement.TryGetProperty("data", out var data) ||
                    !data.TryGetProperty("products", out var products) ||
                    products.ValueKind != JsonValueKind.Array ||
                    products.GetArrayLength() == 0)
                {
                    return PriceResult.Fail(ScrapeStatus.NotFound, "Товар не найден в ответе WB");
                }

                var product = products[0];
                var priceKopecks = ExtractPriceKopecks(product);
                if (priceKopecks is null || priceKopecks.Value <= 0)
                    return PriceResult.Fail(ScrapeStatus.NotFound, "Цена отсутствует в ответе WB (возможно, нет в продаже)");

                var price = Math.Round(priceKopecks.Value / 100m, 2);
                var inStock = IsInStock(product);
                return PriceResult.Ok(price, inStock);
            }
            catch (OperationCanceledException)
            {
                return PriceResult.Fail(ScrapeStatus.Error, "Таймаут запроса к WB");
            }
            catch (Exception ex)
            {
                return PriceResult.Fail(ScrapeStatus.Error, ex.Message);
            }
        }

        /// <summary>Достаёт числовой артикул из ссылок вида .../catalog/12345678/detail.aspx.</summary>
        internal static string? ExtractArticle(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            var m = Regex.Match(url, @"/catalog/(\d+)");
            if (m.Success) return m.Groups[1].Value;

            // запасной вариант: ?nm=12345678 или просто длинное число в строке
            m = Regex.Match(url, @"[?&]nm=(\d+)");
            if (m.Success) return m.Groups[1].Value;

            m = Regex.Match(url, @"(\d{6,})");
            return m.Success ? m.Groups[1].Value : null;
        }

        /// <summary>
        /// Цена в копейках. WB в разное время кладёт её в sizes[].price.{total|product|basic}
        /// или в верхнеуровневые salePriceU/priceU. Берём первое доступное.
        /// </summary>
        private static long? ExtractPriceKopecks(JsonElement product)
        {
            if (product.TryGetProperty("sizes", out var sizes) && sizes.ValueKind == JsonValueKind.Array)
            {
                foreach (var size in sizes.EnumerateArray())
                {
                    if (size.TryGetProperty("price", out var price) && price.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var key in new[] { "total", "product", "basic" })
                        {
                            if (price.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.Number)
                                return v.GetInt64();
                        }
                    }
                }
            }

            foreach (var key in new[] { "salePriceU", "priceU" })
            {
                if (product.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.Number)
                    return v.GetInt64();
            }

            return null;
        }

        private static bool IsInStock(JsonElement product)
        {
            if (product.TryGetProperty("totalQuantity", out var tq) && tq.ValueKind == JsonValueKind.Number)
                return tq.GetInt64() > 0;

            if (product.TryGetProperty("sizes", out var sizes) && sizes.ValueKind == JsonValueKind.Array)
            {
                foreach (var size in sizes.EnumerateArray())
                {
                    if (size.TryGetProperty("stocks", out var stocks) &&
                        stocks.ValueKind == JsonValueKind.Array &&
                        stocks.GetArrayLength() > 0)
                        return true;
                }
                return false;
            }

            // Нет данных об остатках, но цена есть — считаем доступным.
            return true;
        }
    }
}
