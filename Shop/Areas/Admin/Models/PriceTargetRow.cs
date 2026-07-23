namespace Shop.Areas.Admin.Models
{
    /// <summary>Строка таблицы статусов сбора цен на странице «Импорт цен».</summary>
    public class PriceTargetRow
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string StoreName { get; set; } = "";
        public decimal Price { get; set; }
        public bool InStock { get; set; }
        public DateTime LastCheckedAt { get; set; }
        public string? LastStatus { get; set; }
        public string? LastError { get; set; }
    }
}
