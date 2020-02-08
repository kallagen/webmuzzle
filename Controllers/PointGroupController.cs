using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Repository;
using TSensor.Web.ViewModels;
using TSensor.Web.ViewModels.PointGroup;

namespace TSensor.Web.Controllers
{
    public class PointGroupController : LayoutControllerBase
    {
        private readonly IPointGroupRepository _pointGroupRepository;
        private readonly IPointRepository _pointRepository;

        public PointGroupController(IPointGroupRepository pointGroupRepository, IPointRepository pointRepository)
            : base(pointGroupRepository)
        {
            _pointGroupRepository = pointGroupRepository;
            _pointRepository = pointRepository;
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
                    var pointGroupUrl = Url.Action("Edit", "PointGroup", new { pointGroupGuid });
                    TempData["PointGroup.List.SuccessMessage"] =
                        $"Группа объектов <a href=\"{pointGroupUrl}\">\"{viewModel.Name}\"</a> создана";

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
                        Data = group.PointList,
                        AvailablePointList = group.AvailablePointList
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

            return NotFound();
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
                    var pointGroupUrl = Url.Action("Edit", "PointGroup", new { pointGroupGuid = viewModel.PointGroupGuid });
                    TempData["PointGroup.List.SuccessMessage"] =
                        $"Группа объектов <a href=\"{pointGroupUrl}\">\"{viewModel.Name}\"</a> изменена";

                    return RedirectToAction("List", "PointGroup");
                }
                else
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
            }

            var group = _pointGroupRepository.GetByGuid(viewModel.PointGroupGuid);
            if (group != null)
            {
                viewModel.Data = group.PointList;
                viewModel.AvailablePointList = group.AvailablePointList;
                return View(viewModel);
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize(Policy = "Admin")]
        [Route("group/remove")]
        [HttpPost]
        public IActionResult Remove(string pointGroupGuid)
        {
            if (!Guid.TryParse(pointGroupGuid, out var _pointGroupGuid))
            {
                return NotFound();
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

        [Authorize(Policy = "Admin")]
        [Route("group/point/add")]
        public IActionResult AddPoint(string pointGroupGuid, string pointGuid)
        {
            PointGroup group = null;
            if (Guid.TryParse(pointGroupGuid, out var _pointGroupGuid))
            {
                group = _pointGroupRepository.GetByGuid(_pointGroupGuid);
            }
            if (group == null)
            {
                return NotFound();
            }

            Point point = null;
            if (Guid.TryParse(pointGuid, out var _pointGuid))
            {
                point = _pointRepository.GetByGuid(_pointGuid);
                if (point != null)
                {
                    var addResult = _pointGroupRepository.AddPoint(_pointGroupGuid, _pointGuid);
                    if (addResult)
                    {
                        var pointUrl = Url.Action("Edit", "Point", new { pointGuid = _pointGuid });
                        TempData["PointGroup.Edit.SuccessMessage"] =
                            $"Объект <a href=\"{pointUrl}\">\"{point.Name}\"</a> добавлен в группу";
                    }
                    else
                    {
                        TempData["PointGroup.Edit.ErrorMessage"] = Program.GLOBAL_ERROR_MESSAGE;
                    }
                }
            }

            if (point == null)
            {
                TempData["PointGroup.Edit.ErrorMessage"] = "Объект не найден";
            }

            return RedirectToAction("Edit", "PointGroup", new { pointGroupGuid = _pointGroupGuid });
        }

        [Authorize(Policy = "Admin")]
        [Route("group/point/remove")]
        [HttpPost]
        public IActionResult RemovePoint(string pointGroupGuid, string pointGuid)
        {
            if (!Guid.TryParse(pointGroupGuid, out var _pointGroupGuid) ||
                !Guid.TryParse(pointGuid, out var _pointGuid))
            {
                return NotFound();
            }
            else
            {
                if (_pointGroupRepository.RemovePoint(_pointGroupGuid, _pointGuid))
                {
                    TempData["PointGroup.Edit.SuccessMessage"] = "Объект удален из группы";
                    return RedirectToAction("Edit", "PointGroup", new { pointGroupGuid = _pointGroupGuid });
                }
                else
                {
                    TempData["PointGroup.List.ErrorMessage"] = "При удалении объекта из группы произошла ошибка";
                    return RedirectToAction("List", "PointGroup");
                }
            }
        }

        private new IActionResult NotFound()
        {
            ViewBag.Title = "Группа объектов не найдена";
            ViewBag.BackTitle = "назад к списку групп объектов";
            ViewBag.BackUrl = Url.ActionLink("List", "PointGroup");

            return View("NotFound");
        }
    }
}