using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Areas.Admin.Models;
using Shop.Domain;
using Shop.Domain.Entities;

namespace Shop.Areas.Admin.Controllers
{
    public class CategoriesController : AdminBaseController
    {
        private readonly DBContext _context;

        public CategoriesController(DBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.Parent)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(categories);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateParentsAsync(null);
            return View(new CategoryFormModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormModel model)
        {
            if (await _context.Categories.AnyAsync(c => c.Slug == model.Slug))
                ModelState.AddModelError(nameof(model.Slug), "Такой slug уже существует");

            if (!ModelState.IsValid)
            {
                await PopulateParentsAsync(null);
                return View(model);
            }

            _context.Categories.Add(new Category
            {
                Name = model.Name,
                Slug = model.Slug,
                IconOrImage = model.IconOrImage,
                ParentCategoryId = model.ParentCategoryId
            });
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Категория создана";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var c = await _context.Categories.FindAsync(id);
            if (c is null) return NotFound();

            await PopulateParentsAsync(id);
            return View(new CategoryFormModel
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                IconOrImage = c.IconOrImage,
                ParentCategoryId = c.ParentCategoryId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryFormModel model)
        {
            var c = await _context.Categories.FindAsync(model.Id);
            if (c is null) return NotFound();

            if (await _context.Categories.AnyAsync(x => x.Slug == model.Slug && x.Id != model.Id))
                ModelState.AddModelError(nameof(model.Slug), "Такой slug уже существует");
            if (model.ParentCategoryId == model.Id)
                ModelState.AddModelError(nameof(model.ParentCategoryId), "Категория не может быть родителем самой себя");

            if (!ModelState.IsValid)
            {
                await PopulateParentsAsync(model.Id);
                return View(model);
            }

            c.Name = model.Name;
            c.Slug = model.Slug;
            c.IconOrImage = model.IconOrImage;
            c.ParentCategoryId = model.ParentCategoryId;
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Категория обновлена";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _context.Categories
                .Include(x => x.Products)
                .Include(x => x.Children)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (c is null) return NotFound();

            if (c.Products.Any() || c.Children.Any())
            {
                TempData["Ok"] = "Нельзя удалить категорию с товарами или подкатегориями";
                return RedirectToAction(nameof(Index));
            }

            _context.Categories.Remove(c);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Категория удалена";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateParentsAsync(int? excludeId)
        {
            var items = await _context.Categories
                .Where(c => excludeId == null || c.Id != excludeId)
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();
            ViewBag.Parents = items;
        }
    }
}
