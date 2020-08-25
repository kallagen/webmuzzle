using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using TSensor.Web.Models.Repository;
using TSensor.Web.Models.Services.Security;
using TSensor.Web.ViewModels.Auth;

namespace TSensor.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserRepository _repository;
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserRepository repository, AuthService authService, IConfiguration configuration)
        {
            _repository = repository;
            _authService = authService;
            _configuration = configuration;
        }

        [NonAction]
        private IActionResult StartPage()
        {
            return RedirectToAction("Map", "Dashboard");
        }

        [Route("login")]
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

        [Route("login")]
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
                var user = _repository.Auth(viewModel.Login, _authService.EncryptPassword(viewModel.Password));
                if (user == null)
                {
                    viewModel.ErrorMessage = "Неправильный логин или пароль";
                }
                else
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        AuthService.CreateUserPrincipal(user.UserGuid, user.Name, user.Role),
                        new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTime.Now.AddDays(30)
                        });
                    return StartPage();
                }
            }

            return View(viewModel);
        }

        [Route("logout")]
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

        [Route("version")]
        public IActionResult Version()
        {
            return Content(_authService.Version);
        }

        [Route("iframe/{userGuid}")]
        public async Task<IActionResult> IFrame(string userGuid)
        {
            if (_configuration.GetValue<bool>("allowIframe") &&
                Guid.TryParse(userGuid, out var _userGuid))
            {
                var user = _repository.GetByGuid(_userGuid);
                if (user?.IsInactive == false && user?.Role == RoleCollection.Operator)
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        if (_authService.CurrentUserGuid != _userGuid || !_authService.IsCurrentUserEmbedded)
                        {
                            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        }
                    }

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        AuthService.CreateUserPrincipal(user.UserGuid, user.Name, user.Role, isEmbedded: true),
                        new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTime.Now.AddDays(30)
                        });
                    return StartPage();
                }
            }

            return Ok();
        }
    }
}