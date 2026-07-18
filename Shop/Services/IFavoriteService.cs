using Shop.Domain.Entities;

namespace Shop.Services
{
    public interface IFavoriteService
    {
        /// <summary>Переключает избранное. Возвращает true, если товар добавлен, false — если удалён.</summary>
        Task<bool> ToggleAsync(string userId, int productId);

        /// <summary>Товары в избранном пользователя (с офферами).</summary>
        Task<List<Product>> GetForUserAsync(string userId);

        /// <summary>Множество id товаров в избранном (для отметки кнопок на витрине).</summary>
        Task<HashSet<int>> GetFavoriteProductIdsAsync(string userId);
    }
}
