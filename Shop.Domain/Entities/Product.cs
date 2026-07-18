namespace Shop.Domain.Entities
{
    /// <summary>
    /// Товар каталога (ноутбук, телефон и т.д.). Универсальная замена прежней сущности Phone.
    /// Конкретные предложения магазинов хранятся в <see cref="Offer"/>.
    /// </summary>
    public class Product : Entity
    {
        public required string Name { get; set; }

        /// <summary>Бренд/производитель (напр. "Apple", "Asus").</summary>
        public string? Brand { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int? Year { get; set; }

        /// <summary>Метки через запятую (напр. "игровой,165 Гц,DDR5").</summary>
        public string? Tags { get; set; }

        public string? ImageUrl { get; set; }

        public string? Description { get; set; }

        /// <summary>Кэш минимальной цены среди офферов. Пересчитывается из <see cref="Offers"/>.</summary>
        public decimal? PriceMin { get; set; }

        /// <summary>Кэш максимальной цены среди офферов. Пересчитывается из <see cref="Offers"/>.</summary>
        public decimal? PriceMax { get; set; }

        public ICollection<ProductSpecification> Specifications { get; set; } = new List<ProductSpecification>();
        public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    }
}
