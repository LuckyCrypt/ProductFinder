using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Areas.Admin.Models;
using Shop.Domain;
using Shop.Domain.Entities;

namespace Shop.Areas.Admin.Controllers
{
    public class StoresController : AdminBaseController
    {
        private readonly DBContext _context;

        public StoresController(DBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stores = await _context.Stores.OrderBy(s => s.Name).ToListAsync();
            return View(stores);
        }

        public IActionResult Create() => View(new StoreFormModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StoreFormModel model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.Stores.Add(new Store
            {
                Name = model.Name,
                SiteUrl = model.SiteUrl,
                LogoUrl = model.LogoUrl
            });
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Магазин создан";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var s = await _context.Stores.FindAsync(id);
            if (s is null) return NotFound();
            return View(new StoreFormModel { Id = s.Id, Name = s.Name, SiteUrl = s.SiteUrl, LogoUrl = s.LogoUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StoreFormModel model)
        {
            var s = await _context.Stores.FindAsync(model.Id);
            if (s is null) return NotFound();
            if (!ModelState.IsValid) return View(model);

            s.Name = model.Name;
            s.SiteUrl = model.SiteUrl;
            s.LogoUrl = model.LogoUrl;
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Магазин обновлён";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await _context.Stores.Include(x => x.Offers).FirstOrDefaultAsync(x => x.Id == id);
            if (s is null) return NotFound();

            if (s.Offers.Any())
            {
                TempData["Ok"] = "Нельзя удалить магазин с офферами";
                return RedirectToAction(nameof(Index));
            }

            _context.Stores.Remove(s);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Магазин удалён";
            return RedirectToAction(nameof(Index));
        }
    }
}
