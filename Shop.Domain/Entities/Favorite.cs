namespace Shop.Domain.Entities
{
    /// <summary>
    /// Товар в избранном у пользователя. Задел под кабинет клиента
    /// (UI избранного реализуется в фазе кабинетов).
    /// </summary>
    public class Favorite : Entity
    {
        /// <summary>Идентификатор пользователя Identity (строковый ключ).</summary>
        public required string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
