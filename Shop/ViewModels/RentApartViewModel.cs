using System.ComponentModel.DataAnnotations;

namespace Shop.ViewModels
{
    public class RentApartViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public required string URL { get; set; }
        public required string requirement { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } 
    }
}
