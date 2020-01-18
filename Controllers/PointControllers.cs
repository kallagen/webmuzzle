using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Repository;
using TSensor.Web.ViewModels;
using TSensor.Web.ViewModels.Point;

namespace TSensor.Web.Controllers
{
    [Authorize(Policy = "Admin")]
    public class PointController : Controller
    {
        private readonly IPointRepository _repository;

        public PointController(IPointRepository repository)
        {
            _repository = repository;
        }

        [Route("point/list")]
        public IActionResult List()
        {
            var viewModel = new SearchViewModel<Point>
            {
                Data = _repository.List()
            };

            var successMessage = TempData["Point.List.SuccessMessage"] as string;
            if (!string.IsNullOrEmpty(successMessage))
            {
                viewModel.SuccessMessage = successMessage;
            }

            return View(viewModel);
        }

        [Route("point/new")]
        public IActionResult Create()
        {
            var viewModel = new PointCreateEditViewModel();

            return View(viewModel);
        }

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
                var pointGuid = _repository.Create(viewModel.Name);
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

        [Route("point/{pointGuid}")]
        public IActionResult Edit(string pointGuid)
        {
            if (Guid.TryParse(pointGuid, out var _pointGuid))
            {
                var point = _repository.GetPointByGuid(_pointGuid);
                if (point != null)
                {
                    var viewModel = new PointCreateEditViewModel
                    {
                        PointGuid = point.PointGuid,
                        Name = point.Name
                    };

                    return View(viewModel);
                }
            }

            ViewBag.Title = "Объект не найден";
            ViewBag.BackTitle = "назад к списку объектов";
            ViewBag.BackUrl = Url.ActionLink("List", "Point");

            return View("NotFound");
        }

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
                var editResult = _repository.Edit(viewModel.PointGuid, viewModel.Name);
                if (editResult)
                {
                    var pointUrl = Url.Action("Edit", "Point", new { pointUrl = viewModel.PointGuid });
                    TempData["Point.List.SuccessMessage"] =
                        $"Объект <a href=\"{pointUrl}\">\"{viewModel.Name}\"</a> изменен";

                    return RedirectToAction("List", "Point");
                }
                else
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
            }

            return View(viewModel);
        }
    }
}