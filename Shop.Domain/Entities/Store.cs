namespace Shop.Domain.Entities
{
    /// <summary>
    /// Магазин-источник цены (продавец), предложения которого сравниваются.
    /// </summary>
    public class Store : Entity
    {
        public required string Name { get; set; }

        public string? SiteUrl { get; set; }

        public string? LogoUrl { get; set; }

        /// <summary>
        /// Ключ адаптера парсера маркетплейса: "wb" | "ozon" | "yandex".
        /// null — обычный магазин, цены которого парсер не собирает.
        /// </summary>
        public string? Code { get; set; }

        public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    }
}
