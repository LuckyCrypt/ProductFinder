using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Areas.Admin.Models;
using Shop.Domain;
using Shop.Services.Scraping;

namespace Shop.Areas.Admin.Controllers
{
    /// <summary>
    /// Импорт цен с маркетплейсов: запуск сбора и просмотр статусов последнего прогона.
    /// </summary>
    public class ImportController : AdminBaseController
    {
        private readonly DBContext _context;
        private readonly IScrapeQueue _scrapeQueue;

        public ImportController(DBContext context, IScrapeQueue scrapeQueue)
        {
            _context = context;
            _scrapeQueue = scrapeQueue;
        }

        public async Task<IActionResult> Index()
        {
            // Офферы-цели парсинга: магазин с кодом адаптера и заданный URL карточки.
            var targets = await _context.Offers
                .Include(o => o.Store)
                .Include(o => o.Product)
                .Where(o => o.Store!.Code != null && o.ProductUrl != null)
                .OrderByDescending(o => o.LastCheckedAt)
                .Select(o => new PriceTargetRow
                {
                    ProductId = o.ProductId,
                    ProductName = o.Product!.Name,
                    StoreName = o.Store!.Name,
                    Price = o.Price,
                    InStock = o.InStock,
                    LastCheckedAt = o.LastCheckedAt,
                    LastStatus = o.LastStatus,
                    LastError = o.LastError
                })
                .ToListAsync();

            return View(targets);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RefreshAll()
        {
            await _scrapeQueue.EnqueueAsync(new ScrapeJob(null));
            TempData["Ok"] = "Сбор всех цен запущен в фоне — обновите страницу через некоторое время";
            return RedirectToAction(nameof(Index));
        }
    }
}
