using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ParsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogHistory()
        {
            // TODO (фаза парсера): вызвать реальный парсер техники и записать результат в БД.
            TempData["ParsMessage"] = "LogHistory выполнен (заглушка)";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogToDb()
        {
            // TODO (фаза парсера): подключить запуск парсинга из веб-интерфейса.
            TempData["ParsMessage"] = "LogToDb выполнен (заглушка)";
            return RedirectToAction("Index");
        }
    }
}
