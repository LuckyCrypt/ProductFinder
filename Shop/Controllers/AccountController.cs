using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Models.Account;
using System.Security.Claims;
using ToDoList.Domain;
using ToDoList.Domain.Entities;

namespace Shop.Controllers
{
	public class AccountController : ToDoBaseController
	{
		private readonly ToDoListContext _context;
		public IActionResult Index()
		{
			return View();
		}

		public AccountController(ToDoListContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> LoginAsync([Bind(Prefix = "l")] LoginViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View("Index", new AccountViewModel
				{
					LoginViewModel = model
				});
			}
				var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);

			if (user is null) 
			{
				ViewBag.Error = "Некорректные логин и(или) пароль!";
                return View("Index", new AccountViewModel
                {
                    LoginViewModel = model
                });
            }
            await AuthenticateAsync(user);
			return RedirectToAction("Index","Home");
        }

        private async Task AuthenticateAsync(User user)
        {
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
				new Claim(ClaimsIdentity.DefaultNameClaimType,user.Login)
			};
			var id = new ClaimsIdentity(claims,"applicationCookie",ClaimsIdentity.DefaultNameClaimType,ClaimsIdentity.DefaultRoleClaimType);
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(id));
        }


		public async Task<IActionResult> RegisterAsync([Bind(Prefix = "r")] RegisterViewModel model)
		{
            if (!ModelState.IsValid)
            {
                return View("Index", new AccountViewModel
                {
					RegisterViewModel = model
                });
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Login);

            if (user is not null)
            {
                ViewBag.Error = "Пользователь с таким логином уже существует!";
                return View("Index", new AccountViewModel
                {
                    RegisterViewModel = model
                });
            }

			user = new User(model.Login,model.Password);
			_context.Users.Add(user);
			await _context.SaveChangesAsync();

            await AuthenticateAsync(user);
            return RedirectToAction("Index", "Home");
        }
		public async Task<IActionResult> LogoutAsync()
		{
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
 