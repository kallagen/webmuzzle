using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Repository;
using TSensor.Web.ViewModels;
using TSensor.Web.ViewModels.PointGroup;

namespace TSensor.Web.Controllers
{
    public class PointGroupController : Controller
    {
        private readonly IPointGroupRepository _pointGroupRepository;

        public PointGroupController(IPointGroupRepository pointGroupRepository)
        {
            _pointGroupRepository = pointGroupRepository;
        }

        [Authorize(Policy = "Admin")]
        [Route("group/list")]
        public IActionResult List()
        {
            var viewModel = new SearchViewModel<PointGroup>
            {
                Data = _pointGroupRepository.List()
            };

            var successMessage = TempData["PointGroup.List.SuccessMessage"] as string;
            if (!string.IsNullOrEmpty(successMessage))
            {
                viewModel.SuccessMessage = successMessage;
            }
            var errorMessage = TempData["PointGroup.List.ErrorMessage"] as string;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                viewModel.ErrorMessage = errorMessage;
            }

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("group/new")]
        public IActionResult Create()
        {
            var viewModel = new PointGroupCreateEditViewModel();

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("group/new")]
        [HttpPost]
        public IActionResult Create(PointGroupCreateEditViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("Create", "PointGroup");
            }

            viewModel.Name = viewModel.Name?.Trim();

            if (ModelState.IsValid)
            {
                var pointGroupGuid = _pointGroupRepository.Create(viewModel.Name);
                if (pointGroupGuid == null)
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
                else
                {
                    var pointUrl = Url.Action("Edit", "PointGroup", new { pointGroupGuid });
                    TempData["PointGroup.List.SuccessMessage"] =
                        $"Группа объектов <a href=\"{pointUrl}\">\"{viewModel.Name}\"</a> создана";

                    return RedirectToAction("List", "PointGroup");
                }
            }

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("group/{pointGroupGuid}")]
        public IActionResult Edit(string pointGroupGuid)
        {
            if (Guid.TryParse(pointGroupGuid, out var _pointGroupGuid))
            {
                var group = _pointGroupRepository.GetByGuid(_pointGroupGuid);
                if (group != null)
                {
                    var viewModel = new PointGroupCreateEditViewModel
                    {
                        PointGroupGuid = group.PointGroupGuid,
                        Name = group.Name,
                        //Data = _tankRepository.GetTankListByPoint(point.PointGuid)
                    };

                    var successMessage = TempData["PointGroup.Edit.SuccessMessage"] as string;
                    if (!string.IsNullOrEmpty(successMessage))
                    {
                        viewModel.SuccessMessage = successMessage;
                    }
                    var errorMessage = TempData["PointGroup.Edit.ErrorMessage"] as string;
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        viewModel.ErrorMessage = errorMessage;
                    }

                    return View(viewModel);
                }
            }

            ViewBag.Title = "Группа объектов не найдена";
            ViewBag.BackTitle = "назад к списку групп объектов";
            ViewBag.BackUrl = Url.ActionLink("List", "PointGroup");

            return View("NotFound");
        }

        [Authorize(Policy = "Admin")]
        [Route("group/{pointGroupGuid}")]
        [HttpPost]
        public IActionResult Edit(PointGroupCreateEditViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("List", "PointGroup");
            }

            viewModel.Name = viewModel.Name?.Trim();

            if (ModelState.IsValid)
            {
                var editResult = _pointGroupRepository.Edit(viewModel.PointGroupGuid, viewModel.Name);
                if (editResult)
                {
                    var pointUrl = Url.Action("Edit", "PointGroup", new { pointGroupGuid = viewModel.PointGroupGuid });
                    TempData["PointGroup.List.SuccessMessage"] =
                        $"Группа объектов <a href=\"{pointUrl}\">\"{viewModel.Name}\"</a> изменена";

                    return RedirectToAction("List", "PointGroup");
                }
                else
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
            }

            //viewModel.Data = _tankRepository.GetTankListByPoint(viewModel.PointGuid);
            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("group/remove")]
        [HttpPost]
        public IActionResult Remove(string pointGroupGuid)
        {
            if (!Guid.TryParse(pointGroupGuid, out var _pointGroupGuid))
            {
                ViewBag.Title = "Группа объектов не найдена";
                ViewBag.BackTitle = "назад к списку групп объектов";
                ViewBag.BackUrl = Url.ActionLink("List", "PointGroup");

                return View("NotFound");
            }
            else
            {
                if (_pointGroupRepository.Remove(_pointGroupGuid))
                {
                    TempData["PointGroup.List.SuccessMessage"] = "Группа объектов удалена";
                }
                else
                {
                    TempData["PointGroup.List.ErrorMessage"] = "При удалении группы объектов произошла ошибка";
                }
                return RedirectToAction("List", "PointGroup");
            }
        }
    }
}