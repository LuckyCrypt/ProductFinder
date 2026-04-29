using System.ComponentModel.DataAnnotations;

namespace Shop.ViewModels
{
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = "/images/placeholder.jpg";
        public int Quantity { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [DataType(DataType.Currency)]
        public decimal Subtotal => Quantity * Price; // Вычисляемое свойство
    }
}
