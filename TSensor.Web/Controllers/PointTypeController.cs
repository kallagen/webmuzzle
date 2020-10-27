using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Repository;
using TSensor.Web.Models.Services;
using TSensor.Web.ViewModels;
using TSensor.Web.ViewModels.PointType;

namespace TSensor.Web.Controllers
{
    public class PointTypeController : Controller
    {
        private readonly IPointTypeRepository _repository;

        public PointTypeController(IPointTypeRepository repository)
        {
            _repository = repository;
        }

        [Authorize(Policy = "Admin")]
        [Route("type/list")]
        public IActionResult List()
        {
            var viewModel = new SearchViewModel<PointType>
            {
                Data = _repository.List()
            };

            var successMessage = TempData["PointType.List.SuccessMessage"] as string;
            if (!string.IsNullOrEmpty(successMessage))
            {
                viewModel.SuccessMessage = successMessage;
            }
            var errorMessage = TempData["PointType.List.ErrorMessage"] as string;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                viewModel.ErrorMessage = errorMessage;
            }

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("type/new")]
        public IActionResult Create()
        {
            var viewModel = new PointTypeCreateEditViewModel();

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("type/new")]
        [HttpPost]
        public IActionResult Create(PointTypeCreateEditViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("Create", "PointType");
            }

            viewModel.Name = viewModel.Name?.Trim()?.ToUpper();

            if (ModelState.IsValid)
            {
                var pointTypeGuid = _repository.Create(viewModel.Name, viewModel.NewImage.Base64PngImage());
                if (pointTypeGuid == null)
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
                else
                {
                    var pointTypeUrl = Url.Action("Edit", "PointType", new { pointTypeGuid });
                    TempData["PointType.List.SuccessMessage"] =
                        $"Тип объектов <a href=\"{pointTypeGuid}\">\"{viewModel.Name}\"</a> создан";

                    return RedirectToAction("List", "PointType");
                }
            }

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("type/{pointTypeGuid}")]
        public IActionResult Edit(string pointTypeGuid)
        {
            if (Guid.TryParse(pointTypeGuid, out var _pointTypeGuid))
            {
                var type = _repository.GetByGuid(_pointTypeGuid);
                if (type != null)
                {
                    var viewModel = new PointTypeCreateEditViewModel
                    {
                        PointTypeGuid = type.PointTypeGuid,
                        Name = type.Name,
                        Image = type.Image
                    };

                    var successMessage = TempData["PointType.Edit.SuccessMessage"] as string;
                    if (!string.IsNullOrEmpty(successMessage))
                    {
                        viewModel.SuccessMessage = successMessage;
                    }
                    var errorMessage = TempData["PointType.Edit.ErrorMessage"] as string;
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        viewModel.ErrorMessage = errorMessage;
                    }

                    return View(viewModel);
                }
            }

            return NotFound();
        }

        [Authorize(Policy = "Admin")]
        [Route("type/{pointTypeGuid}")]
        [HttpPost]
        public IActionResult Edit(PointTypeCreateEditViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("List", "PointType");
            }

            viewModel.Name = viewModel.Name?.Trim()?.ToUpper();

            if (ModelState.IsValid)
            {
                var editResult = _repository.Edit(viewModel.PointTypeGuid, viewModel.Name, viewModel.NewImage.Base64PngImage());
                if (editResult)
                {
                    var pointTypeUrl = Url.Action("Edit", "PointType", new { pointTypeGuod = viewModel.PointTypeGuid });
                    TempData["PointType.List.SuccessMessage"] =
                        $"Тип объектов <a href=\"{pointTypeUrl}\">\"{viewModel.Name}\"</a> изменен";

                    return RedirectToAction("List", "PointType");
                }
                else
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
            }

            var type = _repository.GetByGuid(viewModel.PointTypeGuid);
            if (type != null)
            {
                viewModel.Image = type.Image;
                return View(viewModel);
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize(Policy = "Admin")]
        [Route("type/remove")]
        [HttpPost]
        public IActionResult Remove(string pointTypeGuid)
        {
            if (!Guid.TryParse(pointTypeGuid, out var _pointTypeGuid))
            {
                return NotFound();
            }
            else
            {
                if (_repository.Remove(_pointTypeGuid))
                {
                    TempData["PointType.List.SuccessMessage"] = "Тип объектов удален";
                }
                else
                {
                    TempData["PointType.List.ErrorMessage"] = "При удалении типа объектов произошла ошибка";
                }
                return RedirectToAction("List", "PointType");
            }
        }

        private new IActionResult NotFound()
        {
            ViewBag.Title = "Тип объектов не найден";
            ViewBag.BackTitle = "назад к списку типов объектов";
            ViewBag.BackUrl = Url.ActionLink("List", "PointType");

            return View("NotFound");
        }
    }
}