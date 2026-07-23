using Microsoft.Playwright;

namespace ParsElements.Scraping
{
    /// <summary>
    /// Яндекс.Маркет. Самый сложный источник: агрессивная SmartCaptcha. Best-effort —
    /// при блокировке честно возвращаем статус Captcha/Blocked, не бросая исключений.
    /// </summary>
    public sealed class YandexParser : PlaywrightParserBase
    {
        public override string Code => "yandex";

        public YandexParser(bool headless = false) : base(headless) { }

        protected override async Task<decimal?> ExtractFromDomAsync(IPage page)
        {
            // Маркет часто помечает актуальную цену data-auto="snippet-price-current"/"price-value".
            var selectors = new[]
            {
                "[data-auto='snippet-price-current']",
                "[data-auto='price-value']",
                "[data-tid] span:has-text('₽')",
                "span:has-text('₽')"
            };

            foreach (var sel in selectors)
            {
                try
                {
                    var elements = await page.QuerySelectorAllAsync(sel);
                    foreach (var el in elements)
                    {
                        var text = await el.InnerTextAsync();
                        var price = ParsePriceText(text);
                        if (price is decimal p && p > 0)
                            return p;
                    }
                }
                catch { /* пробуем следующий селектор */ }
            }

            return null;
        }
    }
}
