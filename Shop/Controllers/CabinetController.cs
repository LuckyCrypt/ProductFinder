using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Domain.Entities;
using Shop.Models.Cabinet;
using Shop.Services;

namespace Shop.Controllers
{
    [Authorize]
    public class CabinetController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IFavoriteService _favorites;

        public CabinetController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IFavoriteService favorites)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _favorites = favorites;
        }

        // /cabinet — профиль
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Challenge();

            return View(new ProfileViewModel
            {
                UserName = user.UserName ?? "",
                FirstName = user.FirstName,
                Email = user.Email
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Challenge();

            model.UserName = user.UserName ?? "";
            if (!ModelState.IsValid) return View(model);

            user.FirstName = model.FirstName;
            user.Email = model.Email;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                return View(model);
            }

            TempData["Ok"] = "Профиль сохранён";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Challenge();

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["Ok"] = "Пароль изменён";
            return RedirectToAction(nameof(Index));
        }

        // /cabinet/favorites — избранное
        public async Task<IActionResult> Favorites()
        {
            var userId = _userManager.GetUserId(User)!;
            var products = await _favorites.GetForUserAsync(userId);
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int productId, string? returnUrl)
        {
            var userId = _userManager.GetUserId(User)!;
            var added = await _favorites.ToggleAsync(userId, productId);
            TempData["Ok"] = added ? "Добавлено в избранное" : "Удалено из избранного";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction(nameof(Favorites));
        }
    }
}
