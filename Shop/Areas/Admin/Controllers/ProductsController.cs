using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Areas.Admin.Models;
using Shop.Domain;
using Shop.Domain.Entities;
using Shop.Services.Scraping;

namespace Shop.Areas.Admin.Controllers
{
    public class ProductsController : AdminBaseController
    {
        private readonly DBContext _context;
        private readonly IScrapeQueue _scrapeQueue;

        public ProductsController(DBContext context, IScrapeQueue scrapeQueue)
        {
            _context = context;
            _scrapeQueue = scrapeQueue;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Offers)
                .OrderBy(p => p.Name)
                .ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesAsync();
            return View(new ProductFormModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategoriesAsync();
                return View(model);
            }

            var product = new Product
            {
                Name = model.Name,
                Brand = model.Brand,
                CategoryId = model.CategoryId,
                Year = model.Year,
                Tags = model.Tags,
                ImageUrl = model.ImageUrl,
                Description = model.Description
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Товар создан — добавьте офферы и характеристики";
            return RedirectToAction(nameof(Edit), new { id = product.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await LoadProductAsync(id);
            if (product is null) return NotFound();
            return View(await BuildEditVmAsync(product));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductFormModel form)
        {
            var product = await LoadProductAsync(form.Id);
            if (product is null) return NotFound();

            if (!ModelState.IsValid)
            {
                var vm = await BuildEditVmAsync(product);
                vm.Form = form;
                return View(vm);
            }

            product.Name = form.Name;
            product.Brand = form.Brand;
            product.CategoryId = form.CategoryId;
            product.Year = form.Year;
            product.Tags = form.Tags;
            product.ImageUrl = form.ImageUrl;
            product.Description = form.Description;
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Товар обновлён";
            return RedirectToAction(nameof(Edit), new { id = product.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOffer(int id, OfferFormModel offer)
        {
            var product = await LoadProductAsync(id);
            if (product is null) return NotFound();

            if (product.Offers.Any(o => o.StoreId == offer.StoreId))
                TempData["Ok"] = "У этого магазина уже есть оффер для товара — отредактируйте или удалите его";
            else if (ModelState.IsValid)
            {
                product.Offers.Add(new Offer
                {
                    StoreId = offer.StoreId,
                    Price = offer.Price,
                    ProductUrl = offer.ProductUrl,
                    InStock = offer.InStock,
                    LastCheckedAt = DateTime.UtcNow
                });
                product.RecomputePriceRange();
                await _context.SaveChangesAsync();
                TempData["Ok"] = "Оффер добавлен";
            }
            return RedirectToAction(nameof(Edit), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RefreshPrices(int id)
        {
            var exists = await _context.Products.AnyAsync(p => p.Id == id);
            if (!exists) return NotFound();

            await _scrapeQueue.EnqueueAsync(new ScrapeJob(id));
            TempData["Ok"] = "Сбор цен запущен в фоне — обновите страницу через минуту, чтобы увидеть результат";
            return RedirectToAction(nameof(Edit), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOffer(int id, int offerId)
        {
            var product = await LoadProductAsync(id);
            if (product is null) return NotFound();

            var offer = product.Offers.FirstOrDefault(o => o.Id == offerId);
            if (offer != null)
            {
                _context.Offers.Remove(offer);
                product.Offers.Remove(offer);
                product.RecomputePriceRange();
                await _context.SaveChangesAsync();
                TempData["Ok"] = "Оффер удалён";
            }
            return RedirectToAction(nameof(Edit), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSpec(int id, SpecFormModel spec)
        {
            var product = await LoadProductAsync(id);
            if (product is null) return NotFound();

            if (ModelState.IsValid)
            {
                product.Specifications.Add(new ProductSpecification
                {
                    Group = spec.Group,
                    Name = spec.Name,
                    Value = spec.Value
                });
                await _context.SaveChangesAsync();
                TempData["Ok"] = "Характеристика добавлена";
            }
            return RedirectToAction(nameof(Edit), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSpec(int id, int specId)
        {
            var spec = await _context.ProductSpecifications
                .FirstOrDefaultAsync(s => s.Id == specId && s.ProductId == id);
            if (spec != null)
            {
                _context.ProductSpecifications.Remove(spec);
                await _context.SaveChangesAsync();
                TempData["Ok"] = "Характеристика удалена";
            }
            return RedirectToAction(nameof(Edit), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null) return NotFound();

            _context.Products.Remove(product); // офферы и спеки уйдут каскадом
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Товар удалён";
            return RedirectToAction(nameof(Index));
        }

        private async Task<Product?> LoadProductAsync(int id) =>
            await _context.Products
                .Include(p => p.Offers).ThenInclude(o => o.Store)
                .Include(p => p.Specifications)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

        private async Task<ProductEditViewModel> BuildEditVmAsync(Product product)
        {
            return new ProductEditViewModel
            {
                Product = product,
                Form = new ProductFormModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Brand = product.Brand,
                    CategoryId = product.CategoryId,
                    Year = product.Year,
                    Tags = product.Tags,
                    ImageUrl = product.ImageUrl,
                    Description = product.Description
                },
                Categories = await CategoryItemsAsync(),
                Stores = await _context.Stores
                    .OrderBy(s => s.Name)
                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                    .ToListAsync()
            };
        }

        private async Task<List<SelectListItem>> CategoryItemsAsync() =>
            await _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();

        private async Task PopulateCategoriesAsync()
        {
            ViewBag.Categories = await CategoryItemsAsync();
        }
    }
}
