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

        public async Task<List<Product>> GetProductsByCategoryAsync(string slug, CatalogFilter? filter = null)
        {
            var categoryIds = await CategoryWithChildrenIdsAsync(slug);
            if (categoryIds.Count == 0)
                return new List<Product>();

            filter ??= new CatalogFilter();

            var query = _context.Products
                .Include(p => p.Offers).ThenInclude(o => o.Store)
                .Include(p => p.Specifications)
                .Include(p => p.Category)
                .Where(p => categoryIds.Contains(p.CategoryId));

            if (!string.IsNullOrWhiteSpace(filter.Brand))
                query = query.Where(p => p.Brand == filter.Brand);
            if (filter.PriceMin.HasValue)
                query = query.Where(p => p.PriceMin >= filter.PriceMin.Value);
            if (filter.PriceMax.HasValue)
                query = query.Where(p => p.PriceMin <= filter.PriceMax.Value);

            query = filter.Sort switch
            {
                "price" => query.OrderBy(p => p.PriceMin ?? decimal.MaxValue),
                "price_desc" => query.OrderByDescending(p => p.PriceMin ?? decimal.MinValue),
                "name" => query.OrderBy(p => p.Name),
                _ => query.OrderByDescending(p => p.Offers.Count).ThenBy(p => p.Name),
            };

            return await query.ToListAsync();
        }

        public async Task<List<string>> GetBrandsForCategoryAsync(string slug)
        {
            var categoryIds = await CategoryWithChildrenIdsAsync(slug);
            if (categoryIds.Count == 0)
                return new List<string>();

            return await _context.Products
                .Where(p => categoryIds.Contains(p.CategoryId) && p.Brand != null && p.Brand != "")
                .Select(p => p.Brand!)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync();
        }

        /// <summary>Id категории по slug плюс id её прямых подкатегорий.</summary>
        private async Task<List<int>> CategoryWithChildrenIdsAsync(string slug)
        {
            var category = await _context.Categories
                .Include(c => c.Children)
                .FirstOrDefaultAsync(c => c.Slug == slug);

            if (category is null)
                return new List<int>();

            var ids = new List<int> { category.Id };
            ids.AddRange(category.Children.Select(c => c.Id));
            return ids;
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
