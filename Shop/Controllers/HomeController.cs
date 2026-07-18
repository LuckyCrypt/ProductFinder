using Microsoft.AspNetCore.Mvc;
using Shop.Services;
using Shop.ViewModels;
using System.Diagnostics;

namespace Shop.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ICatalogService _catalog;

		public HomeController(ILogger<HomeController> logger, ICatalogService catalog)
		{
			_logger = logger;
			_catalog = catalog;
		}

		public async Task<IActionResult> Index()
		{
			var products = await _catalog.GetFeaturedProductsAsync(8);
			return View(products);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
