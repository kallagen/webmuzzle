using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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

        public PointController(IPointRepository pointRepository, ITankRepository tankRepository)
        {
            _pointRepository = pointRepository;
            _tankRepository = tankRepository;
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
            var viewModel = new PointCreateEditViewModel();

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

            if (ModelState.IsValid)
            {
                var pointGuid = _pointRepository.Create(viewModel.Name);
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
                        Data = _tankRepository.GetListByPoint(point.PointGuid)
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

            if (ModelState.IsValid)
            {
                var editResult = _pointRepository.Edit(viewModel.PointGuid, viewModel.Name);
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

            viewModel.Data = _tankRepository.GetListByPoint(viewModel.PointGuid);
            return View(viewModel);
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

        [Route("point/all")]
        public IActionResult All()
        {
            var data = _pointRepository.GetAllPointInfo().GroupBy(p => p.PointGuid)
                .OrderBy(p =>
                {
                    var hasError = p.Any(t => t.IsError);
                    var hasWarning = p.Any(t => t.IsWarning);

                    if (hasError & hasWarning)
                    {
                        return 0;
                    }
                    else if (hasError)
                    {
                        return 1;
                    }
                    else if (hasWarning)
                    {
                        return 2;
                    }
                    else
                    {
                        return 3;
                    }
                }).ThenBy(p => p.First().PointName);
            return View(data);
        }

        [Route("point/{pointGuid}/state")]
        public IActionResult State(string pointGuid)
        {
            if (Guid.TryParse(pointGuid, out var _pointGuid))
            {
                var pointActualState = _pointRepository.GetSensorActualState(_pointGuid);
                if (pointActualState.Any())
                {
                    return View(pointActualState);
                }
            }

            ViewBag.Title = "Объект не найден";
            ViewBag.BackTitle = "назад к списку объектов";
            ViewBag.BackUrl = Url.ActionLink("All", "Point");

            return View("NotFound");
        }
    }
}