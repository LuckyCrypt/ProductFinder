using Microsoft.AspNetCore.Mvc.Rendering;
using Shop.Domain.Entities;

namespace Shop.Areas.Admin.Models
{
    /// <summary>Страница редактирования товара: основные поля + офферы + характеристики.</summary>
    public class ProductEditViewModel
    {
        public required Product Product { get; set; }
        public ProductFormModel Form { get; set; } = new();
        public List<SelectListItem> Categories { get; set; } = new();
        public List<SelectListItem> Stores { get; set; } = new();
        public OfferFormModel NewOffer { get; set; } = new();
        public SpecFormModel NewSpec { get; set; } = new();
    }
}
