using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class DataRentApart
    {
        public int id { get; set; }
        public required string name { get; set; }
        //public required string Description { get; set; } = string.Empty;
        public required DateTime datescan { get; set; } 
        public required string url { get; set; }
        public required string requirement { get; set; }
        public required bool active { get; set; }
        public required string apartmentsid { get; set; }
        // Для лучшего отображения цены
        [DataType(DataType.Currency)]
        public required string price { get; set; }
        //public string ImageUrl { get; set; }
        
    }
}
