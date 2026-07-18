using Shop.Domain.Entities;

namespace Shop.ViewModels
{
    /// <summary>Модель страницы категории каталога со списком товаров и офферами.</summary>
    public class CatalogPageViewModel
    {
        public required string Title { get; set; }
        public string? Slug { get; set; }
        public string? Sort { get; set; }
        public List<Product> Products { get; set; } = new();
    }
}
