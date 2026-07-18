using Microsoft.EntityFrameworkCore;
using Shop.Domain;
using Shop.Domain.Entities;

namespace Shop.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly DBContext _context;

        public FavoriteService(DBContext context)
        {
            _context = context;
        }

        public async Task<bool> ToggleAsync(string userId, int productId)
        {
            var existing = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

            if (existing != null)
            {
                _context.Favorites.Remove(existing);
                await _context.SaveChangesAsync();
                return false;
            }

            // Проверяем, что товар существует, прежде чем добавлять.
            if (!await _context.Products.AnyAsync(p => p.Id == productId))
                return false;

            _context.Favorites.Add(new Favorite { UserId = userId, ProductId = productId });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Product>> GetForUserAsync(string userId)
        {
            return await _context.Products
                .Where(p => _context.Favorites.Any(f => f.UserId == userId && f.ProductId == p.Id))
                .Include(p => p.Offers).ThenInclude(o => o.Store)
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<HashSet<int>> GetFavoriteProductIdsAsync(string userId)
        {
            var ids = await _context.Favorites
                .Where(f => f.UserId == userId)
                .Select(f => f.ProductId)
                .ToListAsync();
            return ids.ToHashSet();
        }
    }
}
