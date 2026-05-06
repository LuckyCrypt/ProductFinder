using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Repository;
using Shop.ViewModels;
using System.Diagnostics;

namespace Shop.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

        public async Task<IActionResult> Index([FromServices] DataFromSqlRepository dataFromSql)
        {
			var _products = await dataFromSql.GetRentAparts();	
            return View(_products);
        }

		//public IActionResult Details(int id)
		//{
  //          var _products = await dataFromSql.GetRentAparts();
  //          var product = _products.FirstOrDefault(p => p.Id == id);
		//	if (product == null)
		//	{
		//		return NotFound();
		//	}
		//	return View(product);
		//}

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
