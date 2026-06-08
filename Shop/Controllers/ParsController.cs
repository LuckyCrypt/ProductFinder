using Microsoft.AspNetCore.Mvc;
using ParsElements.Models;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    public class ParsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogHistory()
        {
            // Вызов метода из ParsElements
            //var db = new DatabaseManager("localhost", "postgres", "2028", "postgres");
            //await db.LogHistory();
            TempData["ParsMessage"] = "LogHistory выполнен";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> LogToDb()
        {
/*            var db = new DatabaseManager("localhost", "postgres", "2028", "postgres");
            await db.LogToDb("Вызов LogToDb из веб-интерфейса");*/
            TempData["ParsMessage"] = "LogToDb выполнен";
            return RedirectToAction("Index");
        }
    }
}
