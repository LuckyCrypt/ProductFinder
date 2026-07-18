using System.ComponentModel.DataAnnotations;

namespace Shop.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public int Products { get; set; }
        public int Offers { get; set; }
        public int Categories { get; set; }
        public int Stores { get; set; }
    }

    public class CategoryFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите название")]
        [Display(Name = "Название")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Укажите slug")]
        [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Только строчные латинские буквы, цифры и дефис")]
        [Display(Name = "Slug (для URL)")]
        public string Slug { get; set; } = "";

        [Display(Name = "Иконка/эмодзи или путь к картинке")]
        public string? IconOrImage { get; set; }

        [Display(Name = "Родительская категория")]
        public int? ParentCategoryId { get; set; }
    }

    public class StoreFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите название")]
        [Display(Name = "Название")]
        public string Name { get; set; } = "";

        [Display(Name = "Сайт магазина")]
        public string? SiteUrl { get; set; }

        [Display(Name = "Логотип (URL или эмодзи)")]
        public string? LogoUrl { get; set; }
    }

    public class ProductFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите название")]
        [Display(Name = "Название")]
        public string Name { get; set; } = "";

        [Display(Name = "Бренд")]
        public string? Brand { get; set; }

        [Required(ErrorMessage = "Выберите категорию")]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        [Display(Name = "Год")]
        public int? Year { get; set; }

        [Display(Name = "Теги (через запятую)")]
        public string? Tags { get; set; }

        [Display(Name = "URL изображения")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Описание")]
        public string? Description { get; set; }
    }

    public class OfferFormModel
    {
        [Required(ErrorMessage = "Выберите магазин")]
        [Display(Name = "Магазин")]
        public int StoreId { get; set; }

        [Range(0, 100000000, ErrorMessage = "Некорректная цена")]
        [Display(Name = "Цена, грн")]
        public decimal Price { get; set; }

        [Display(Name = "Ссылка на товар в магазине")]
        public string? ProductUrl { get; set; }

        [Display(Name = "В наличии")]
        public bool InStock { get; set; } = true;
    }

    public class SpecFormModel
    {
        [Display(Name = "Группа")]
        public string? Group { get; set; }

        [Required(ErrorMessage = "Укажите название характеристики")]
        [Display(Name = "Характеристика")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Укажите значение")]
        [Display(Name = "Значение")]
        public string Value { get; set; } = "";
    }
}
