using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using TSensor.Web.Models.Repository;
using TSensor.Web.Models.Services.Security;
using TSensor.Web.ViewModels.User;

namespace TSensor.Web.Controllers
{
    [Authorize(Policy = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserRepository _repository;
        private readonly AuthService _authService;
        private readonly IMemoryCache _memoryCache;

        public UserController(IUserRepository repository, AuthService authService, IMemoryCache memoryCache)
        {
            _repository = repository;
            _authService = authService;
            _memoryCache = memoryCache;
        }

        [Route("user/search")]
        public IActionResult Search(string s, string r)
        {
            var search = s;
            var role = r;

            var viewModel = new UserSearchViewModel
            {
                FilterSearch = search,
                FilterRole = role,
                Data = _repository.Search(search, role)
            };

            var successMessage = TempData["User.Search.SuccessMessage"] as string;
            if (!string.IsNullOrEmpty(successMessage))
            {
                viewModel.SuccessMessage = successMessage;
            }
            var errorMessage = TempData["User.Search.ErrorMessage"] as string;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                viewModel.ErrorMessage = errorMessage;
            }

            return View(viewModel);
        }

        [Route("user/search")]
        [HttpPost]
        public IActionResult Search(UserSearchViewModel viewModel)
        {
            return RedirectToAction("Search", "User",
                new
                {
                    s = viewModel?.FilterSearch,
                    r = viewModel?.FilterRole
                });
        }

        [Route("user/create")]
        public IActionResult Create()
        {
            var viewModel = new UserCreateViewModel();

            return View(viewModel);
        }

        [Route("user/create")]
        [HttpPost]
        public IActionResult Create(UserCreateViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("Create", "User");
            }

            viewModel.Login = viewModel.Login?.Trim();
            viewModel.Name = viewModel.Name?.Trim();

            viewModel.Validate(ModelState);
            if (_repository.GetUserByLogin(viewModel.Login) != null)
            {
                ModelState.AddModelError("Login", "Пользователь с таким логином уже есть");
            }

            if (ModelState.IsValid)
            {
                var userGuid = _repository.Create(viewModel.Login, viewModel.Name,
                    _authService.EncryptPassword(viewModel.Password), viewModel.Role);

                if (userGuid == null)
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
                else
                {
                    var userUrl = Url.ActionLink("Edit", "User", new { userGuid });
                    TempData["User.Search.SuccessMessage"] = string.IsNullOrEmpty(viewModel.Name) ?
                        $"<a href=\"{userUrl}\">Новый</a> пользователь создан" :
                        $"Пользователь <a href=\"{userUrl}\">\"{viewModel.Name}\"</a> создан";

                    return RedirectToAction("Search", "User");
                }
            }

            return View(viewModel);
        }

        [Route("user/{userGuid}")]
        public IActionResult Edit(string userGuid)
        {
            if (Guid.TryParse(userGuid, out var _userGuid))
            {
                var user = _repository.GetUserByGuid(_userGuid);
                if (user != null)
                {
                    var viewModel = new UserEditViewModel
                    {
                        UserGuid = user.UserGuid,
                        Login = user.Login,
                        Name = user.Name,
                        Role = user.Role,
                        IsInactive = user.IsInactive
                    };

                    return View(viewModel);
                }
            }

            ViewBag.Title = "Пользователь не найден";
            ViewBag.BackTitle = "назад к списку пользователей";
            ViewBag.BackUrl = Url.ActionLink("Search", "User");

            return View("NotFound");
        }

        [Route("user/{userGuid}")]
        [HttpPost]
        public IActionResult Edit(UserEditViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("Search", "User");
            }

            viewModel.Name = viewModel.Name?.Trim();

            viewModel.Validate(ModelState);
            var userByLogin = _repository.GetUserByLogin(viewModel.Login);
            if (userByLogin != null && userByLogin.UserGuid != viewModel.UserGuid)
            {
                ModelState.AddModelError("Login", "Пользователь с таким логином уже есть");
            }

            if (ModelState.IsValid)
            {
                var editResult = _repository.Edit(viewModel.UserGuid, viewModel.Name,
                    viewModel.Role, viewModel.IsInactive);

                var changePasswordResult = true;
                if (viewModel.SetNewPassword)
                {
                    changePasswordResult = _repository.ChangePassword(viewModel.UserGuid,
                        _authService.EncryptPassword(viewModel.NewPassword));
                }

                if (editResult && changePasswordResult)
                {
                    if (!((viewModel.IsInactive || viewModel.SetNewPassword) &&
                        _authService.CurrentUserGuid == viewModel.UserGuid))
                    {
                        var userUrl = Url.Action("Edit", "User", new { userGuid = viewModel.UserGuid });
                        TempData["User.Search.SuccessMessage"] = string.IsNullOrWhiteSpace(viewModel.Name) ?
                            $"<a href=\"{userUrl}\">Пользователь</a> отредактирован" :
                            $"Пользователь <a href=\"{userUrl}\">\"{viewModel.Name}\"</a> отредактирован";
                    }

                    SetUserNewPrincipalInMemory(viewModel);

                    return RedirectToAction("Search", "User");
                }
                else
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
            }

            var user = _repository.GetUserByGuid(viewModel.UserGuid);
            viewModel.Login = user.Login;

            return View(viewModel);
        }

        private void SetUserNewPrincipalInMemory(UserEditViewModel viewModel)
        {
            _memoryCache.Set($"UserLastUpdate{viewModel.UserGuid}", DateTime.Now);

            if (viewModel.SetNewPassword || viewModel.IsInactive)
            {
                _memoryCache.Set($"UserReject{viewModel.UserGuid}", true);
            }

            _memoryCache.Set($"UserName{viewModel.UserGuid}", viewModel.Name);
            _memoryCache.Set($"UserRole{viewModel.UserGuid}", viewModel.Role);
        }

        [Route("user/remove")]
        [HttpPost]
        public ActionResult Remove(string userGuid)
        {
            if (!Guid.TryParse(userGuid, out var _userGuid))
            {
                ViewBag.Title = "Пользователь не найден";
                ViewBag.BackTitle = "назад к списку пользователей";
                ViewBag.BackUrl = Url.ActionLink("Search", "User");

                return View("NotFound");
            }
            else
            {
                if (_repository.Remove(_userGuid))
                {
                    TempData["User.Search.SuccessMessage"] = "Пользователь удален";
                }
                else
                {
                    TempData["User.Search.ErrorMessage"] = "При удалении пользователя произошла ошибка";
                }
                return RedirectToAction("Search", "User");
            }
        }
    }
}