using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Domain.Entities;
using Shop.Models.Account;

namespace Shop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind(Prefix = "l")] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", new AccountViewModel { LoginViewModel = model });

            var result = await _signInManager.PasswordSignInAsync(
                model.Login, model.Password, isPersistent: true, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ViewBag.Error = "Некорректные логин и(или) пароль!";
                return View("Index", new AccountViewModel { LoginViewModel = model });
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind(Prefix = "r")] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", new AccountViewModel { RegisterViewModel = model });

            var user = new ApplicationUser
            {
                UserName = model.Login,
                Email = model.Email,
                FirstName = model.FirstName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ViewBag.Error = string.Join(" ", result.Errors.Select(e => e.Description));
                return View("Index", new AccountViewModel { RegisterViewModel = model });
            }

            await _userManager.AddToRoleAsync(user, DbSeeder.ClientRole);
            await _signInManager.SignInAsync(user, isPersistent: true);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
