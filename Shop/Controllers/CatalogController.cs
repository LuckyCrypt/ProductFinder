using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Shop.Domain.Entities;

namespace Shop.Controllers
{
    public class CatalogController : Controller
    {
        private readonly Shop.Repository.DataFromSqlRepository _repo;

        public CatalogController(Shop.Repository.DataFromSqlRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Gadgets()
        {
            ViewData["Title"] = "Гаджеты";
            return View();
        }

        public IActionResult Phones()
        {
            ViewData["Title"] = "Мобильные телефоны";
            var phones = new List<Phone>
            {
                new Phone { Id = 1, Name = "iPhone 13 Pro", Subtitle = "Apple", Storage = "128GB", Ram = "6GB", Year = 2021, Tags = "smartphone,apple,ios", Screen = "6.1 inch", Camera = "12MP", Battery = "3095mAh", PriceMin = 999.99m, PriceMax = 1299.99m, ImageUrl = "https://example.com/iphone13pro.jpg" },
                new Phone { Id = 2, Name = "Samsung Galaxy S21", Subtitle = "Samsung", Storage = "128GB", Ram = "8GB", Year = 2021, Tags = "smartphone,samsung,android", Screen = "6.2 inch", Camera = "64MP", Battery = "4000mAh", PriceMin = 799.99m, PriceMax = 999.99m, ImageUrl = "https://example.com/galaxys21.jpg" },
            };
            return View(phones);
        }

        public IActionResult Computers()
        {
            ViewData["Title"] = "Компьютеры";
            return View();
        }

        public IActionResult Photo()
        {
            ViewData["Title"] = "Фото";
            return View();
        }

        public IActionResult TV()
        {
            ViewData["Title"] = "TV";
            return View();
        }

        public IActionResult Audio()
        {
            ViewData["Title"] = "Аудио";
            return View();
        }

        public IActionResult Appliances()
        {
            ViewData["Title"] = "Бытовая техника";
            return View();
        }

        public IActionResult Climate()
        {
            ViewData["Title"] = "Климат";
            return View();
        }

        public IActionResult Home()
        {
            ViewData["Title"] = "Дом";
            return View();
        }
    }
}
