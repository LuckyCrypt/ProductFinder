using Shop.Domain.Entities;

namespace Shop.Services
{
    public interface ICatalogService
    {
        /// <summary>Корневые категории (для меню каталога).</summary>
        Task<List<Category>> GetRootCategoriesAsync();

        Task<Category?> GetCategoryBySlugAsync(string slug);

        /// <summary>Товары категории (и её подкатегорий) по slug с офферами и фильтром.</summary>
        Task<List<Product>> GetProductsByCategoryAsync(string slug, CatalogFilter? filter = null);

        /// <summary>Уникальные бренды товаров категории (для фильтра).</summary>
        Task<List<string>> GetBrandsForCategoryAsync(string slug);

        /// <summary>Товар с характеристиками и офферами всех магазинов.</summary>
        Task<Product?> GetProductWithOffersAsync(int id);

        /// <summary>Подборка товаров для главной страницы.</summary>
        Task<List<Product>> GetFeaturedProductsAsync(int count);
    }
}
