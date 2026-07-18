using Microsoft.AspNetCore.Mvc;
using Shop.Services;
using Shop.ViewModels;

namespace Shop.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalog;

        public CatalogController(ICatalogService catalog)
        {
            _catalog = catalog;
        }

        // Действия меню отображают общую страницу категории по slug.
        public Task<IActionResult> Gadgets(string? sort) => CategoryPage("gadgets", "Гаджеты", sort);
        public Task<IActionResult> Phones(string? sort) => CategoryPage("phones", "Мобильные телефоны", sort);
        public Task<IActionResult> Computers(string? sort) => CategoryPage("computers", "Компьютеры", sort);
        public Task<IActionResult> Photo(string? sort) => CategoryPage("photo", "Фото", sort);
        public Task<IActionResult> TV(string? sort) => CategoryPage("tv", "TV", sort);
        public Task<IActionResult> Audio(string? sort) => CategoryPage("audio", "Аудио", sort);
        public Task<IActionResult> Appliances(string? sort) => CategoryPage("appliances", "Бытовая техника", sort);
        public Task<IActionResult> Climate(string? sort) => CategoryPage("climate", "Климат", sort);
        public Task<IActionResult> Home(string? sort) => CategoryPage("home", "Дом", sort);

        // Обобщённый маршрут по slug: /Catalog/Category/noutbuki
        public Task<IActionResult> Category(string slug, string? sort) => CategoryPage(slug, slug, sort);

        // Карточка товара со всеми офферами: /Catalog/Product/5
        public async Task<IActionResult> Product(int id)
        {
            var product = await _catalog.GetProductWithOffersAsync(id);
            if (product is null)
                return NotFound();

            ViewData["Title"] = product.Name;
            return View(product);
        }

        private async Task<IActionResult> CategoryPage(string slug, string title, string? sort)
        {
            var products = await _catalog.GetProductsByCategoryAsync(slug, sort);
            var category = await _catalog.GetCategoryBySlugAsync(slug);

            var vm = new CatalogPageViewModel
            {
                Title = category?.Name ?? title,
                Slug = slug,
                Sort = sort,
                Products = products
            };
            ViewData["Title"] = vm.Title;
            return View("Category", vm);
        }
    }
}
