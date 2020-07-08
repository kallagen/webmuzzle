using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TSensor.License.Models;
using TSensor.License.ViewModels;

namespace TSensor.License.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [NonAction]
        private IActionResult StartPage()
        {
            return RedirectToAction("List", "License");
        }

        [Route("license/login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return StartPage();
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.QueryString.Value))
                {
                    return RedirectToAction("Login", "Auth");
                }
                else
                {
                    return View();
                }
            }
        }

        [Route("license/login")]
        [HttpPost]
        public async Task<IActionResult> Login(AuthLoginViewModel viewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                return StartPage();
            }

            if (viewModel != null &&
                !string.IsNullOrWhiteSpace(viewModel.Login) &&
                !string.IsNullOrWhiteSpace(viewModel.Password))
            {
                if (_authService.Validate(viewModel.Login, viewModel.Password))
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        AuthService.CreateUserPrincipal(),
                        new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTime.Now.AddDays(30)
                        });
                    return StartPage();
                }
                else
                {
                    viewModel.ErrorMessage = "Неправильный логин или пароль";
                }
            }

            return View(viewModel);
        }

        [Route("license/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }

        [Route("youshallnotpass")]
        public IActionResult AccessDenied()
        {
            if (!string.IsNullOrEmpty(Request.QueryString.Value))
            {
                return RedirectToAction("AccessDenied", "Auth");
            }
            else
            {
                return View("AccessDenied");
            }
        }
    }
}