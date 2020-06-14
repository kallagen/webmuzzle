using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Repository;
using TSensor.Web.Models.Services;
using TSensor.Web.ViewModels;
using TSensor.Web.ViewModels.Tank;

namespace TSensor.Web.Controllers
{
    [Authorize(Policy = "Admin")]
    public class MassmeterController : Controller
    {
        private readonly ITankRepository _tankRepository;
        private readonly IPointRepository _pointRepository;

        public MassmeterController(ITankRepository tankRepository, IPointRepository pointRepository)
        {
            _tankRepository = tankRepository;
            _pointRepository = pointRepository;
        }        

        [Authorize(Policy = "Admin")]
        [Route("massmeter/list")]
        public IActionResult List()
        {
            var viewModel = new SearchViewModel<Tank>
            {
                Data = _tankRepository.GetListByPoint(PointRepository.MASSMETER_POINT_GUID)
            };

            var successMessage = TempData["Massmeter.List.SuccessMessage"] as string;
            if (!string.IsNullOrEmpty(successMessage))
            {
                viewModel.SuccessMessage = successMessage;
            }
            var errorMessage = TempData["Massmeter.List.ErrorMessage"] as string;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                viewModel.ErrorMessage = errorMessage;
            }

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("massmeter/new")]
        public IActionResult Create()
        {
            var viewModel = new TankCreateEditViewModel();

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("massmeter/new")]
        [HttpPost]
        public IActionResult Create(TankCreateEditViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("Create", "Massmeter");
            }

            viewModel.Name = viewModel.Name?.Trim();
            viewModel.MainDeviceGuid = viewModel.MainDeviceGuid?.Trim();
            viewModel.SecondDeviceGuid = viewModel.SecondDeviceGuid?.Trim();

            if (ModelState.IsValid)
            {
                var massmeterGuid = _tankRepository.Create(
                    PointRepository.MASSMETER_POINT_GUID, viewModel.Name, productGuid: null, dualMode: true,
                    viewModel.MainDeviceGuid, viewModel.MainIZKId, viewModel.MainSensorId,
                    viewModel.SecondDeviceGuid, viewModel.SecondIZKId, viewModel.SecondSensorId,
                    viewModel.Description, weightChangeDelta: null, weightChangeTimeout: null);
                if (massmeterGuid == null)
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
                else
                {
                    var massmeterUrl = Url.Action("Edit", "Massmeter", new { massmeterGuid });
                    TempData["Massmeter.List.SuccessMessage"] =
                        $"Массомер <a href=\"{massmeterUrl}\">\"{viewModel.Name}\"</a> добавлен";

                    return RedirectToAction("List", "Massmeter");
                }
            }

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("massmeter/{massmeterGuid}")]
        public IActionResult Edit(string massmeterGuid)
        {
            if (Guid.TryParse(massmeterGuid, out var _massmeterGuid))
            {
                var massmeter = _tankRepository.GetByGuid(_massmeterGuid);
                if (massmeter != null)
                {
                    var viewModel = new TankCreateEditViewModel
                    {
                        TankGuid = massmeter.TankGuid,
                        Name = massmeter.Name,
                        MainDeviceGuid = massmeter.MainDeviceGuid,
                        MainIZKId = massmeter.MainIZKId,
                        MainSensorId = massmeter.MainSensorId,
                        SecondDeviceGuid = massmeter.SecondDeviceGuid,
                        SecondIZKId = massmeter.SecondIZKId,
                        SecondSensorId = massmeter.SecondSensorId,
                        Description = massmeter.Description
                    };

                    var successMessage = TempData["Massmeter.Edit.SuccessMessage"] as string;
                    if (!string.IsNullOrEmpty(successMessage))
                    {
                        viewModel.SuccessMessage = successMessage;
                    }
                    var errorMessage = TempData["Massmeter.Edit.ErrorMessage"] as string;
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
        [Route("massmeter/{massmeterGuid}")]
        [HttpPost]
        public IActionResult Edit(TankCreateEditViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("List", "Massmeter");
            }

            viewModel.Name = viewModel.Name?.Trim();
            viewModel.MainDeviceGuid = viewModel.MainDeviceGuid?.Trim();
            viewModel.SecondDeviceGuid = viewModel.SecondDeviceGuid?.Trim();

            if (ModelState.IsValid)
            {
                var editResult = _tankRepository.Edit(
                    viewModel.TankGuid.Value, PointRepository.MASSMETER_POINT_GUID, viewModel.Name,
                    productGuid: null, dualMode: true,
                    viewModel.MainDeviceGuid, viewModel.MainIZKId, viewModel.MainSensorId,
                    viewModel.SecondDeviceGuid, viewModel.SecondIZKId, viewModel.SecondSensorId,
                    viewModel.Description, weightChangeDelta: null, weightChangeTimeout: null);
                if (editResult)
                {
                    var massmeterUrl = Url.Action("Edit", "Massmeter", new { massmeterGuid = viewModel.TankGuid.Value });
                    TempData["Massmeter.List.SuccessMessage"] =
                        $"Массомер <a href=\"{massmeterUrl}\">\"{viewModel.Name}\"</a> изменен";

                    return RedirectToAction("List", "Massmeter");
                }
                else
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
            }

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("massmeter/remove")]
        [HttpPost]
        public IActionResult Remove(string massmeterGuid)
        {
            if (!Guid.TryParse(massmeterGuid, out var _massmeterGuid))
            {
                return NotFound();
            }
            else
            {
                if (_tankRepository.Remove(PointRepository.MASSMETER_POINT_GUID, _massmeterGuid))
                {
                    TempData["Massmeter.List.SuccessMessage"] = "Объект удален";
                }
                else
                {
                    TempData["Massmeter.List.ErrorMessage"] = "При удалении массомера произошла ошибка";
                }
                return RedirectToAction("List", "Massmeter");
            }
        }

        private new IActionResult NotFound()
        {
            ViewBag.Title = "Объект не найден";
            ViewBag.BackTitle = "назад к списку массомеров";
            ViewBag.BackUrl = Url.ActionLink("List", "Massmeter");

            return View("NotFound");
        }

        [Authorize(Policy = "Admin")]
        [Route("massmeter/state")]
        public IActionResult ActualSensorValues()
        {
            var comparer = new AlphanumComparer();

            var massmeterList = _tankRepository.GetListByPoint(PointRepository.MASSMETER_POINT_GUID);
            var data = _pointRepository.GetSensorActualState(massmeterList.Select(p => p.TankGuid))
                .OrderBy(t => t.PointName, comparer)
                .ThenBy(t => t.TankName, comparer);

            return View(data);
        }
    }
}