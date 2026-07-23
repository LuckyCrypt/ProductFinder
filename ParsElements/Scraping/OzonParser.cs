using Microsoft.Playwright;

namespace ParsElements.Scraping
{
    /// <summary>
    /// Ozon. Сильная анти-бот защита: даже с браузером результат не гарантирован.
    /// Сначала пробуем JSON-LD (база), затем DOM-виджет цены.
    /// </summary>
    public sealed class OzonParser : PlaywrightParserBase
    {
        public override string Code => "ozon";

        public OzonParser(bool headless = false) : base(headless) { }

        protected override async Task<decimal?> ExtractFromDomAsync(IPage page)
        {
            // Ozon рендерит цену в виджете webPrice; берём первый крупный ценник со знаком ₽.
            var selectors = new[]
            {
                "[data-widget='webPrice'] span",
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
