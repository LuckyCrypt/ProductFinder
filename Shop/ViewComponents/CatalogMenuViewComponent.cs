using Microsoft.AspNetCore.Mvc;
using Shop.Services;

namespace Shop.ViewComponents
{
    /// <summary>Горизонтальное меню корневых категорий, строится из БД.</summary>
    public class CatalogMenuViewComponent : ViewComponent
    {
        private readonly ICatalogService _catalog;

        public CatalogMenuViewComponent(ICatalogService catalog)
        {
            _catalog = catalog;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _catalog.GetRootCategoriesAsync();
            return View(categories);
        }
    }
}
