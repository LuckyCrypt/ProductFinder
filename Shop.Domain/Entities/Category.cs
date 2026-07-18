namespace Shop.Domain.Entities
{
    /// <summary>
    /// Категория каталога. Поддерживает иерархию (напр. «Компьютеры» → «Ноутбуки»).
    /// </summary>
    public class Category : Entity
    {
        public required string Name { get; set; }

        /// <summary>URL-совместимый идентификатор для маршрутов, напр. "noutbuki".</summary>
        public required string Slug { get; set; }

        /// <summary>Эмодзи/иконка или путь к изображению для меню каталога.</summary>
        public string? IconOrImage { get; set; }

        public int? ParentCategoryId { get; set; }
        public Category? Parent { get; set; }

        public ICollection<Category> Children { get; set; } = new List<Category>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
