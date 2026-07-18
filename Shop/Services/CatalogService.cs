using Microsoft.EntityFrameworkCore;
using Shop.Domain;
using Shop.Domain.Entities;

namespace Shop.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly DBContext _context;

        public CatalogService(DBContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetRootCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == null)
                .Include(c => c.Children)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryBySlugAsync(string slug)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Slug == slug);
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(string slug, string? sort = null)
        {
            var category = await _context.Categories
                .Include(c => c.Children)
                .FirstOrDefaultAsync(c => c.Slug == slug);

            if (category is null)
                return new List<Product>();

            // Товары самой категории и её дочерних (напр. «Компьютеры» → «Ноутбуки»).
            var categoryIds = new List<int> { category.Id };
            categoryIds.AddRange(category.Children.Select(c => c.Id));

            var query = _context.Products
                .Include(p => p.Offers).ThenInclude(o => o.Store)
                .Include(p => p.Specifications)
                .Include(p => p.Category)
                .Where(p => categoryIds.Contains(p.CategoryId));

            query = sort switch
            {
                "price" => query.OrderBy(p => p.PriceMin ?? decimal.MaxValue),
                _ => query.OrderByDescending(p => p.Offers.Count).ThenBy(p => p.Name),
            };

            return await query.ToListAsync();
        }

        public async Task<Product?> GetProductWithOffersAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Specifications)
                .Include(p => p.Offers.OrderBy(o => o.Price)).ThenInclude(o => o.Store)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetFeaturedProductsAsync(int count)
        {
            return await _context.Products
                .Include(p => p.Offers).ThenInclude(o => o.Store)
                .Include(p => p.Category)
                .OrderByDescending(p => p.Offers.Count)
                .Take(count)
                .ToListAsync();
        }
    }
}
