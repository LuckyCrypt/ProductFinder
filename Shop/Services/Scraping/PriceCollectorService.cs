using Microsoft.EntityFrameworkCore;
using ParsElements.Scraping;
using Shop.Domain;

namespace Shop.Services.Scraping
{
    /// <summary>
    /// Оркестрация сбора цен: находит офферы-цели (магазин с кодом адаптера + заданный
    /// URL карточки), вызывает нужный адаптер маркетплейса, апсертит цену/наличие и
    /// пересчитывает диапазон цены товара.
    /// </summary>
    public sealed class PriceCollectorService
    {
        private readonly DBContext _db;
        private readonly IReadOnlyDictionary<string, IMarketplaceParser> _parsers;
        private readonly ILogger<PriceCollectorService> _logger;

        public PriceCollectorService(
            DBContext db,
            IEnumerable<IMarketplaceParser> parsers,
            ILogger<PriceCollectorService> logger)
        {
            _db = db;
            _parsers = parsers.ToDictionary(p => p.Code, StringComparer.OrdinalIgnoreCase);
            _logger = logger;
        }

        public async Task CollectAsync(ScrapeJob job, CancellationToken ct = default)
        {
            var query = _db.Products
                .Include(p => p.Offers).ThenInclude(o => o.Store)
                .Where(p => p.Offers.Any(o => o.Store!.Code != null && o.ProductUrl != null));

            if (job.ProductId is int pid)
                query = query.Where(p => p.Id == pid);

            var products = await query.ToListAsync(ct);
            _logger.LogInformation("Сбор цен: задание {Job}, товаров с целями: {Count}",
                job.ProductId?.ToString() ?? "ВСЕ", products.Count);

            var updated = 0;
            foreach (var product in products)
            {
                var targets = product.Offers
                    .Where(o => o.Store?.Code != null && !string.IsNullOrWhiteSpace(o.ProductUrl))
                    .ToList();

                foreach (var offer in targets)
                {
                    ct.ThrowIfCancellationRequested();

                    var code = offer.Store!.Code!;
                    if (!_parsers.TryGetValue(code, out var parser))
                    {
                        offer.LastStatus = ScrapeStatus.Error;
                        offer.LastError = $"Нет адаптера для магазина с кодом '{code}'";
                        offer.LastCheckedAt = DateTime.UtcNow;
                        continue;
                    }

                    PriceResult result;
                    try
                    {
                        result = await parser.FetchAsync(offer.ProductUrl!, ct);
                    }
                    catch (Exception ex)
                    {
                        // Адаптер по контракту не должен бросать, но подстрахуемся —
                        // сбой одного источника не должен ронять остальные.
                        result = PriceResult.Fail(ScrapeStatus.Error, ex.Message);
                    }

                    offer.LastStatus = result.Status;
                    offer.LastError = result.Error;
                    offer.LastCheckedAt = DateTime.UtcNow;

                    if (result.Status == ScrapeStatus.Ok && result.Price is decimal price)
                    {
                        offer.Price = price;
                        offer.InStock = result.InStock;
                        updated++;
                    }

                    _logger.LogInformation("[{Store}] товар #{ProductId}: {Status} {Price}",
                        offer.Store!.Name, product.Id, result.Status,
                        result.Price?.ToString() ?? "-");
                }

                product.RecomputePriceRange();
            }

            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Сбор цен завершён: обновлено цен — {Updated}", updated);
        }
    }
}
