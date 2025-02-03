/*using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Shop.Controllers;
using Shop.Models.Account;
using ToDoList.Domain.Entities;
using ToDoList.Domain;
using Microsoft.AspNetCore.Authentication;

namespace ВашеПриложение.Controllers
{
    public class AccountController : ToDoBaseController
    {
        private readonly ToDoListContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(ToDoListContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync([Bind(Prefix = "l")] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", new AccountViewModel
                {
                    LoginViewModel = model
                });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Login);

            if (user is null)
            {
                TempData["Error"] = "Некорректные логин и(или) пароль!";
                return View("Index", new AccountViewModel
                {
                    LoginViewModel = model
                });
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                TempData["Error"] = "Некорректные логин и(или) пароль!";
                return View("Index", new AccountViewModel
                {
                    LoginViewModel = model
                });
            }

            await AuthenticateAsync(user);
            return RedirectToAction("Index", "Home");
        }

        private async Task AuthenticateAsync(User user)
        {
            var claims = new List<Claim>
          {
           new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
           new Claim(ClaimsIdentity.DefaultNameClaimType,user.Login)
          };
            var id = new ClaimsIdentity(claims, "applicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }


        [HttpPost]
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

            if (user != null)
            {
                TempData["RegisterError"] = "Пользователь с таким логином уже существует!";
                return View("Index", new AccountViewModel
                {
                    RegisterViewModel = model
                });
            }

            var newUser = new User(model.Login, _passwordHasher.HashPassword(null, model.Password));

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            await AuthenticateAsync(newUser);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


    }
}*/