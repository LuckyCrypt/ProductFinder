namespace Shop.Services
{
    /// <summary>Параметры фильтрации/сортировки списка товаров категории.</summary>
    public class CatalogFilter
    {
        public string? Brand { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }

        /// <summary>"price" | "price_desc" | "name" | "popularity" (по умолчанию).</summary>
        public string? Sort { get; set; }
    }
}
