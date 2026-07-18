using Shop.Domain.Entities;
using Shop.Services;

namespace Shop.ViewModels
{
    /// <summary>Модель страницы категории каталога со списком товаров, офферами и фильтром.</summary>
    public class CatalogPageViewModel
    {
        public required string Title { get; set; }
        public string? Slug { get; set; }
        public CatalogFilter Filter { get; set; } = new();
        public List<string> Brands { get; set; } = new();
        public List<Product> Products { get; set; } = new();

        /// <summary>Id товаров в избранном текущего пользователя (пусто для анонима).</summary>
        public HashSet<int> FavoriteIds { get; set; } = new();
    }
}
