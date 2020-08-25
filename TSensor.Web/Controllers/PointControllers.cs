using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Repository;
using TSensor.Web.ViewModels;
using TSensor.Web.ViewModels.Point;

namespace TSensor.Web.Controllers
{
    public class PointController : Controller
    {
        private readonly IPointRepository _pointRepository;
        private readonly ITankRepository _tankRepository;
        private readonly IUserRepository _userRepository;

        private readonly decimal mapPointDefaultLocationLongitude = 0;
        private readonly decimal mapPointDefaultLocationLatitude = 0;

        public PointController(IPointRepository pointRepository, ITankRepository tankRepository,
            IUserRepository userRepository, IConfiguration configuration)
        {
            _pointRepository = pointRepository;
            _tankRepository = tankRepository;
            _userRepository = userRepository;

            var coordinates = configuration.GetSection("defaultPointPosition").Get<IEnumerable<decimal>>().ToArray();
            if (coordinates?.Length == 2)
            {
                mapPointDefaultLocationLongitude = coordinates[0];
                mapPointDefaultLocationLatitude = coordinates[1];
            }
        }

        [Authorize(Policy = "Admin")]
        [Route("point/list")]
        public IActionResult List()
        {
            var viewModel = new SearchViewModel<Point>
            {
                Data = _pointRepository.List()
            };

            var successMessage = TempData["Point.List.SuccessMessage"] as string;
            if (!string.IsNullOrEmpty(successMessage))
            {
                viewModel.SuccessMessage = successMessage;
            }
            var errorMessage = TempData["Point.List.ErrorMessage"] as string;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                viewModel.ErrorMessage = errorMessage;
            }

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("point/new")]
        public IActionResult Create()
        {
            var viewModel = new PointCreateEditViewModel
            {
                DefaultLongitude = mapPointDefaultLocationLongitude,
                DefaultLatitude = mapPointDefaultLocationLatitude
            };

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("point/new")]
        [HttpPost]
        public IActionResult Create(PointCreateEditViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("Create", "Point");
            }

            viewModel.Name = viewModel.Name?.Trim();
            viewModel.Address = viewModel.Address?.Trim();
            viewModel.Phone = viewModel.Phone?.Trim();
            viewModel.Email = viewModel.Email?.Trim();

            if (ModelState.IsValid)
            {
                var pointGuid = _pointRepository.Create(viewModel.Name,
                    viewModel.Address, viewModel.Phone, viewModel.Email, viewModel.Description,
                    viewModel.LongitudeParsed, viewModel.LatitudeParsed);
                if (pointGuid == null)
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
                else
                {
                    var pointUrl = Url.Action("Edit", "Point", new { pointGuid });
                    TempData["Point.List.SuccessMessage"] =
                        $"Объект <a href=\"{pointUrl}\">\"{viewModel.Name}\"</a> создан";

                    return RedirectToAction("List", "Point");
                }
            }

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("point/{pointGuid}")]
        public IActionResult Edit(string pointGuid)
        {
            if (Guid.TryParse(pointGuid, out var _pointGuid))
            {
                var point = _pointRepository.GetByGuid(_pointGuid);
                if (point != null)
                {
                    var viewModel = new PointCreateEditViewModel
                    {
                        PointGuid = point.PointGuid,
                        Name = point.Name,
                        Address = point.Address,
                        Phone = point.Phone,
                        Email = point.Email,
                        Description = point.Description,

                        Longitude = point.Longitude?.ToString(),
                        Latitude = point.Latitude?.ToString(),

                        DefaultLongitude = mapPointDefaultLocationLongitude,
                        DefaultLatitude = mapPointDefaultLocationLatitude,                        

                        Data = _tankRepository.GetListByPoint(point.PointGuid),
                        UserList = point.UserList,
                        AvailableUserList = point.AvailableUserList                        
                    };

                    var successMessage = TempData["Point.Edit.SuccessMessage"] as string;
                    if (!string.IsNullOrEmpty(successMessage))
                    {
                        viewModel.SuccessMessage = successMessage;
                    }
                    var errorMessage = TempData["Point.Edit.ErrorMessage"] as string;
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        viewModel.ErrorMessage = errorMessage;
                    }

                    return View(viewModel);
                }
            }

            ViewBag.Title = "Объект не найден";
            ViewBag.BackTitle = "назад к списку объектов";
            ViewBag.BackUrl = Url.ActionLink("List", "Point");

            return View("NotFound");
        }

        [Authorize(Policy = "Admin")]
        [Route("point/{pointGuid}")]
        [HttpPost]
        public IActionResult Edit(PointCreateEditViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("List", "Point");
            }

            viewModel.Name = viewModel.Name?.Trim();
            viewModel.Address = viewModel.Address?.Trim();
            viewModel.Phone = viewModel.Phone?.Trim();
            viewModel.Email = viewModel.Email?.Trim();

            viewModel.Validate(ModelState);
            if (ModelState.IsValid)
            {
                var editResult = _pointRepository.Edit(viewModel.PointGuid, viewModel.Name,
                    viewModel.Address, viewModel.Phone, viewModel.Email, viewModel.Description,
                    viewModel.LongitudeParsed, viewModel.LatitudeParsed);
                if (editResult)
                {
                    var pointUrl = Url.Action("Edit", "Point", new { pointGuid = viewModel.PointGuid });
                    TempData["Point.List.SuccessMessage"] =
                        $"Объект <a href=\"{pointUrl}\">\"{viewModel.Name}\"</a> изменен";

                    return RedirectToAction("List", "Point");
                }
                else
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
            }

            var point = _pointRepository.GetByGuid(viewModel.PointGuid);
            if (point != null)
            {
                viewModel.Data = _tankRepository.GetListByPoint(viewModel.PointGuid);
                viewModel.UserList = point.UserList;                
                viewModel.AvailableUserList = point.AvailableUserList;
                return View(viewModel);
            }
            else
            {
                ViewBag.Title = "Объект не найден";
                ViewBag.BackTitle = "назад к списку объектов";
                ViewBag.BackUrl = Url.ActionLink("List", "Point");

                return View("NotFound");
            }
        }

        [Authorize(Policy = "Admin")]
        [Route("point/remove")]
        [HttpPost]
        public IActionResult Remove(string pointGuid)
        {
            if (!Guid.TryParse(pointGuid, out var _pointGuid))
            {
                ViewBag.Title = "Объект не найден";
                ViewBag.BackTitle = "назад к списку объектов";
                ViewBag.BackUrl = Url.ActionLink("List", "Point");

                return View("NotFound");
            }
            else
            {
                if (_pointRepository.Remove(_pointGuid))
                {
                    TempData["Point.List.SuccessMessage"] = "Объект удален";
                }
                else
                {
                    TempData["Point.List.ErrorMessage"] = "При удалении объекта произошла ошибка";
                }
                return RedirectToAction("List", "Point");
            }
        }

        [Authorize(Policy = "Admin")]
        [Route("point/user/add")]
        [HttpPost]
        public IActionResult AddUser(string pointGuid, string userGuid)
        {
            Point point = null;
            if (Guid.TryParse(pointGuid, out var _pointGuid))
            {
                point = _pointRepository.GetByGuid(_pointGuid);
            }
            if (point == null)
            {
                ViewBag.Title = "Объект не найден";
                ViewBag.BackTitle = "назад к списку объектов";
                ViewBag.BackUrl = Url.ActionLink("List", "Point");

                return View("NotFound");
            }

            User user = null;
            if (Guid.TryParse(userGuid, out var _userGuid))
            {
                user = _userRepository.GetByGuid(_userGuid);
                if (user != null)
                {
                    var addResult = _userRepository.AddPointUser(_pointGuid, _userGuid);
                    if (addResult)
                    {
                        var userUrl = Url.Action("Edit", "User", new { userGuid = _userGuid });
                        TempData["Point.Edit.SuccessMessage"] =
                            $"Оператору <a href=\"{userUrl}\">\"{user.Name}\"</a> выдано разрешение на доступ к объекту";
                    }
                    else
                    {
                        TempData["Point.Edit.ErrorMessage"] = Program.GLOBAL_ERROR_MESSAGE;
                    }
                }
            }

            if (user == null)
            {
                TempData["Point.Edit.ErrorMessage"] = "Оператор не найден";
            }

            return RedirectToAction("Edit", "Point", new { pointGuid = _pointGuid });
        }

        [Authorize(Policy = "Admin")]
        [Route("point/user/remove")]
        [HttpPost]
        public IActionResult RemoveUser(string pointGuid, string userGuid)
        {
            if (!Guid.TryParse(pointGuid, out var _pointGuid) ||
                !Guid.TryParse(userGuid, out var _userGuid))
            {
                ViewBag.Title = "Объект не найден";
                ViewBag.BackTitle = "назад к списку объектов";
                ViewBag.BackUrl = Url.ActionLink("List", "Point");

                return View("NotFound");
            }
            else
            {
                if (_userRepository.RemovePointUser(_pointGuid, _userGuid))
                {
                    TempData["Point.Edit.SuccessMessage"] = "Оператору запрещен доступ к объекту";
                    return RedirectToAction("Edit", "Point", new { pointGuid = _pointGuid });
                }
                else
                {
                    TempData["Point.List.ErrorMessage"] = "При запрещении доступа оператору к объекту произошла ошибка";
                    return RedirectToAction("List", "Point");
                }
            }
        }
    }
}