using Microsoft.Playwright;
using NLog;
using ParsElements.Models;

namespace AvitoScraper
{
    class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        static async Task Main(string[] args)
        {
            // Настройка логирования
            //Log.Logger = new LoggerConfiguration()
            //.WriteTo.Console()
            //.WriteTo.File("avito_scraper.log")
            //.CreateLogger();

            var db = new DatabaseManager("localhost", "postgres", "2028", "postgres");
            await db.LogHistory();

            //await RunScraper(db);
        }

        static async Task AvitoApartment(DatabaseManager db)
        {
            using var playwright = await Playwright.CreateAsync();
            // Настройка браузера
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false, // Avito часто блокирует headless
                Args = new[] { "--disable-blink-features=AutomationControlled" }
            });

            var context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
            });

            var page = await context.NewPageAsync();

            try
            {
                string url = "https://www.avito.ru/sankt-peterburg/kvartiry/sdam/na_dlitelnyy_srok?metro=171-181&s=104";
                await page.GotoAsync(url);

                // Ожидание и клик по фильтрам (аналог логики на Python)
                try
                {
                    await page.ClickAsync("button[data-marker='search-filters/submit-button']", new PageClickOptions { Timeout = 5000 });
                }
                catch 
                {
                    Log.Error("Не получилось нажать на кнопку поиск");
                }

                for (int p = 1; p <= 5; p++)
                {
                    Log.Info("Смотрим страницу {Page}", p);
                    await Task.Delay(5000); // Пауза для загрузки

                    var listings = await page.QuerySelectorAllAsync("div[data-marker='item']");
                    var items = new List<ApartmentItem>();

                    foreach (var listing in listings)
                    {
                        try
                        {
                            var titleEl = await listing.QuerySelectorAsync("a[data-marker='item-title']");
                            var priceEl = await listing.QuerySelectorAsync("meta[itemprop='price']");
                            var reqEl = await listing.QuerySelectorAsync("p[data-marker='item-specific-params']");
                            if (reqEl != null)
                                items.Add(new ApartmentItem
                                {
                                    Name = await titleEl.InnerTextAsync(),
                                    Url = "https://www.avito.ru" + await titleEl.GetAttributeAsync("href"),
                                    Price = await priceEl.GetAttributeAsync("content"),
                                    Requirement = reqEl != null ? await reqEl.InnerTextAsync() : "",
                                    ApartmentsId = await listing.GetAttributeAsync("data-item-id")
                                });
                        }
                        catch (Exception ex)
                        {
                            Log.Warn("Error parsing element: {Message}", ex.Message);
                        }
                    }

                    if (items.Count > 0)
                        await db.SaveItems(items);

                    var nextBtn = await page.QuerySelectorAsync("a[data-marker='pagination-button/nextPage']");
                    if (nextBtn != null)
                    {
                        await nextBtn.ClickAsync();
                    }
                    else break;
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Scraper crashed");
                await db.LogToDb($"CRITICAL ERROR: {ex.Message}");
            }
        }
        static async Task MarketPhone(DatabaseManager db)
        {
            using var playwright = await Playwright.CreateAsync();
            // Настройка браузера
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false, // Avito часто блокирует headless
                Args = new[] { "--disable-blink-features=AutomationControlled" }
            });

            var context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
            });

            var page = await context.NewPageAsync();

            try
            {
                string url = "https://www.avito.ru/sankt-peterburg/kvartiry/sdam/na_dlitelnyy_srok?metro=171-181&s=104";
                await page.GotoAsync(url);

                // Ожидание и клик по фильтрам (аналог логики на Python)
                try
                {
                    await page.ClickAsync("button[data-marker='search-filters/submit-button']", new PageClickOptions { Timeout = 5000 });
                }
                catch
                {
                    Log.Error("Не получилось нажать на кнопку поиск");
                }

                for (int p = 1; p <= 5; p++)
                {
                    Log.Info("Смотрим страницу {Page}", p);
                    await Task.Delay(5000); // Пауза для загрузки

                    var listings = await page.QuerySelectorAllAsync("div[data-marker='item']");
                    var items = new List<ApartmentItem>();

                    foreach (var listing in listings)
                    {
                        try
                        {
                            var titleEl = await listing.QuerySelectorAsync("a[data-marker='item-title']");
                            var priceEl = await listing.QuerySelectorAsync("meta[itemprop='price']");
                            var reqEl = await listing.QuerySelectorAsync("p[data-marker='item-specific-params']");
                            if (reqEl != null)
                                items.Add(new ApartmentItem
                                {
                                    Name = await titleEl.InnerTextAsync(),
                                    Url = "https://www.avito.ru" + await titleEl.GetAttributeAsync("href"),
                                    Price = await priceEl.GetAttributeAsync("content"),
                                    Requirement = reqEl != null ? await reqEl.InnerTextAsync() : "",
                                    ApartmentsId = await listing.GetAttributeAsync("data-item-id")
                                });
                        }
                        catch (Exception ex)
                        {
                            Log.Warn("Error parsing element: {Message}", ex.Message);
                        }
                    }

                    if (items.Count > 0)
                        await db.SaveItems(items);

                    var nextBtn = await page.QuerySelectorAsync("a[data-marker='pagination-button/nextPage']");
                    if (nextBtn != null)
                    {
                        await nextBtn.ClickAsync();
                    }
                    else break;
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Scraper crashed");
                await db.LogToDb($"CRITICAL ERROR: {ex.Message}");
            }
        }
    }
}
