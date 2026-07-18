using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Shop.Areas.Admin.Models;
using Shop.Domain;

namespace Shop.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        private readonly DBContext _context;

        public DashboardController(DBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel
            {
                Products = await _context.Products.CountAsync(),
                Offers = await _context.Offers.CountAsync(),
                Categories = await _context.Categories.CountAsync(),
                Stores = await _context.Stores.CountAsync()
            };
            return View(vm);
        }
    }
}
