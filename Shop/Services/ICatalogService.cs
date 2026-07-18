using Shop.Domain.Entities;

namespace Shop.Services
{
    public interface ICatalogService
    {
        /// <summary>Корневые категории (для меню каталога).</summary>
        Task<List<Category>> GetRootCategoriesAsync();

        Task<Category?> GetCategoryBySlugAsync(string slug);

        /// <summary>Товары категории по slug с офферами; sort: "price" | "popularity".</summary>
        Task<List<Product>> GetProductsByCategoryAsync(string slug, string? sort = null);

        /// <summary>Товар с характеристиками и офферами всех магазинов.</summary>
        Task<Product?> GetProductWithOffersAsync(int id);

        /// <summary>Подборка товаров для главной страницы.</summary>
        Task<List<Product>> GetFeaturedProductsAsync(int count);
    }
}
