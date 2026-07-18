namespace Shop.Domain.Entities
{
    /// <summary>
    /// Гибкая характеристика товара (ключ-значение). Позволяет описывать разный набор
    /// спеков для разных категорий (экран/камера у телефона, GPU/ОЗУ у ноутбука)
    /// без раздувания сущности <see cref="Product"/>.
    /// </summary>
    public class ProductSpecification : Entity
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        /// <summary>Группа характеристик для группировки в UI (напр. "Дисплей", "Память").</summary>
        public string? Group { get; set; }

        public required string Name { get; set; }

        public required string Value { get; set; }
    }
}
