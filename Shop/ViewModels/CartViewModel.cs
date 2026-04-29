using System.ComponentModel.DataAnnotations;

namespace Shop.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

        [DataType(DataType.Currency)]
        public decimal CartTotal => Items.Sum(item => item.Subtotal);
    }
}
