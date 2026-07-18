using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Domain.Entities;
using Shop.Services;
using Shop.ViewModels;

namespace Shop.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalog;
        private readonly IFavoriteService _favorites;
        private readonly UserManager<ApplicationUser> _userManager;

        public CatalogController(
            ICatalogService catalog,
            IFavoriteService favorites,
            UserManager<ApplicationUser> userManager)
        {
            _catalog = catalog;
            _favorites = favorites;
            _userManager = userManager;
        }

        private async Task<HashSet<int>> CurrentFavoritesAsync()
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
                return new HashSet<int>();
            var userId = _userManager.GetUserId(User);
            return userId is null ? new HashSet<int>() : await _favorites.GetFavoriteProductIdsAsync(userId);
        }

        // Действия меню отображают общую страницу категории по slug.
        public Task<IActionResult> Gadgets(CatalogFilter f) => CategoryPage("gadgets", "Гаджеты", f);
        public Task<IActionResult> Phones(CatalogFilter f) => CategoryPage("phones", "Мобильные телефоны", f);
        public Task<IActionResult> Computers(CatalogFilter f) => CategoryPage("computers", "Компьютеры", f);
        public Task<IActionResult> Photo(CatalogFilter f) => CategoryPage("photo", "Фото", f);
        public Task<IActionResult> TV(CatalogFilter f) => CategoryPage("tv", "TV", f);
        public Task<IActionResult> Audio(CatalogFilter f) => CategoryPage("audio", "Аудио", f);
        public Task<IActionResult> Appliances(CatalogFilter f) => CategoryPage("appliances", "Бытовая техника", f);
        public Task<IActionResult> Climate(CatalogFilter f) => CategoryPage("climate", "Климат", f);
        public Task<IActionResult> Home(CatalogFilter f) => CategoryPage("home", "Дом", f);

        // Обобщённый маршрут по slug: /Catalog/Category/noutbuki
        [Route("Catalog/Category/{slug?}")]
        public Task<IActionResult> Category(string slug, CatalogFilter f) => CategoryPage(slug, slug, f);

        // Карточка товара со всеми офферами: /Catalog/Product/5
        public async Task<IActionResult> Product(int id)
        {
            var product = await _catalog.GetProductWithOffersAsync(id);
            if (product is null)
                return NotFound();

            ViewData["Title"] = product.Name;
            ViewBag.IsFavorite = (await CurrentFavoritesAsync()).Contains(product.Id);
            return View(product);
        }

        private async Task<IActionResult> CategoryPage(string slug, string title, CatalogFilter filter)
        {
            var products = await _catalog.GetProductsByCategoryAsync(slug, filter);
            var category = await _catalog.GetCategoryBySlugAsync(slug);
            var brands = await _catalog.GetBrandsForCategoryAsync(slug);

            var vm = new CatalogPageViewModel
            {
                Title = category?.Name ?? title,
                Slug = slug,
                Filter = filter,
                Brands = brands,
                Products = products,
                FavoriteIds = await CurrentFavoritesAsync()
            };
            ViewData["Title"] = vm.Title;
            return View("Category", vm);
        }
    }
}
