using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Repository;
using TSensor.Web.ViewModels.Tank;

namespace TSensor.Web.Controllers
{
    [Authorize(Policy = "Admin")]
    public class TankController : Controller
    {
        private readonly IPointRepository _pointRepository;
        private readonly ITankRepository _tankRepository;
        private readonly IProductRepository _productRepository;

        public TankController(IPointRepository pointRepository, ITankRepository tankRepository,
            IProductRepository productRepository)
        {
            _pointRepository = pointRepository;
            _tankRepository = tankRepository;
            _productRepository = productRepository;
        }

        [Route("point/{pointGuid}/tank/new")]
        public IActionResult Create(string pointGuid)
        {
            if (Guid.TryParse(pointGuid, out var _pointGuid))
            {
                var point = _pointRepository.GetByGuid(_pointGuid);
                if (point != null)
                {
                    var viewModel = new TankCreateEditViewModel
                    {
                        PointGuid = point.PointGuid,
                        PointName = point.Name,
                        ProductList = _productRepository.List()
                    };

                    return View(viewModel);
                }
            }

            return NotFound();
        }

        [Route("point/{pointGuid}/tank/new")]
        [HttpPost]
        public IActionResult Create(TankCreateEditViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("List", "Point");
            }

            Point point = null;
            if (viewModel?.PointGuid.HasValue == true)
            {
                point = _pointRepository.GetByGuid(viewModel.PointGuid.Value);
            }
            if (point == null)
            {
                return NotFound();
            }

            viewModel.Name = viewModel.Name?.Trim();
            viewModel.MainDeviceGuid = viewModel.MainDeviceGuid?.Trim();
            viewModel.MainIZKId = viewModel.MainIZKId?.ToUpper()?.Trim();
            viewModel.MainSensorId = viewModel.MainSensorId?.ToUpper()?.Trim();
            viewModel.SecondDeviceGuid = viewModel.SecondDeviceGuid?.Trim();
            viewModel.SecondIZKId = viewModel.SecondIZKId?.ToUpper()?.Trim();
            viewModel.SecondSensorId = viewModel.SecondSensorId?.ToUpper()?.Trim();

            if (ModelState.IsValid)
            {
                var tankGuid = _tankRepository.Create(
                    viewModel.PointGuid.Value, viewModel.Name, viewModel.ProductGuid, viewModel.DualMode,
                    viewModel.MainDeviceGuid, viewModel.MainIZKId, viewModel.MainSensorId,
                    viewModel.SecondDeviceGuid, viewModel.SecondIZKId, viewModel.SecondSensorId,
                    viewModel.Description);
                if (tankGuid == null)
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
                else
                {
                    var tankUrl = Url.Action("Edit", "Tank", new { pointGuid = viewModel.PointGuid.Value, tankGuid });
                    TempData["Point.Edit.SuccessMessage"] =
                        $"Резервуар <a href=\"{tankUrl}\">\"{viewModel.Name}\"</a> добавлен";

                    return RedirectToAction("Edit", "Point", new { pointGuid = viewModel.PointGuid.Value });
                }
            }

            viewModel.ProductList = _productRepository.List();
            viewModel.PointName = point.Name;
            return View(viewModel);
        }

        [Route("point/{pointGuid}/tank/{tankGuid}")]
        public IActionResult Edit(string pointGuid, string tankGuid)
        {
            Point point = null;
            if (Guid.TryParse(pointGuid, out var _pointGuid))
            {
                point = _pointRepository.GetByGuid(_pointGuid);
            }
            if (point == null)
            {
                return NotFound();
            }

            Tank tank = null;
            if (Guid.TryParse(tankGuid, out var _tankGuid))
            {
                tank = _tankRepository.GetByGuid(_tankGuid);
            }
            if (tank == null)
            {
                TempData["Point.Edit.ErrorMessage"] = "Резервуар не найден";
                return RedirectToAction("Edit", "Point", new { pointGuid = point.PointGuid });
            }

            var viewModel = new TankCreateEditViewModel
            {
                TankGuid = tank.TankGuid,
                PointGuid = point.PointGuid,
                PointName = point.Name,
                Name = tank.Name,
                ProductGuid = tank.ProductGuid,
                MainDeviceGuid = tank.MainDeviceGuid,
                MainIZKId = tank.MainIZKId,
                MainSensorId = tank.MainSensorId,
                DualMode = tank.DualMode,
                SecondDeviceGuid = tank.SecondDeviceGuid,
                SecondIZKId = tank.SecondIZKId,
                SecondSensorId = tank.SecondSensorId,
                Description = tank.Description
            };

            viewModel.ProductList = _productRepository.List();
            return View(viewModel);
        }

        [Route("point/{pointGuid}/tank/{tankGuid}")]
        [HttpPost]
        public IActionResult Edit(TankCreateEditViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("List", "Point");
            }

            Point point = null;
            if (viewModel?.PointGuid.HasValue == true)
            {
                point = _pointRepository.GetByGuid(viewModel.PointGuid.Value);
            }
            if (point == null)
            {
                return NotFound();
            }

            viewModel.Name = viewModel.Name?.Trim();
            viewModel.MainDeviceGuid = viewModel.MainDeviceGuid?.Trim();
            viewModel.MainIZKId = viewModel.MainIZKId?.ToUpper()?.Trim();
            viewModel.MainSensorId = viewModel.MainSensorId?.ToUpper()?.Trim();
            viewModel.SecondDeviceGuid = viewModel.SecondDeviceGuid?.Trim();
            viewModel.SecondIZKId = viewModel.SecondIZKId?.ToUpper()?.Trim();
            viewModel.SecondSensorId = viewModel.SecondSensorId?.ToUpper()?.Trim();

            if (ModelState.IsValid && viewModel.TankGuid.HasValue)
            {
                var editResult = _tankRepository.Edit(
                    viewModel.TankGuid.Value, viewModel.PointGuid.Value, viewModel.Name, 
                    viewModel.ProductGuid, viewModel.DualMode,
                    viewModel.MainDeviceGuid, viewModel.MainIZKId, viewModel.MainSensorId,
                    viewModel.SecondDeviceGuid, viewModel.SecondIZKId, viewModel.SecondSensorId,
                    viewModel.Description);
                if (editResult)
                {
                    var tankUrl = Url.Action("Edit", "Tank", new { pointGuid = viewModel.PointGuid.Value, tankGuid = viewModel.TankGuid.Value });
                    TempData["Point.Edit.SuccessMessage"] =
                        $"Резервуар <a href=\"{tankUrl}\">\"{viewModel.Name}\"</a> изменен";

                    return RedirectToAction("Edit", "Point", new { pointGuid = viewModel.PointGuid.Value });
                }
                else
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
            }

            viewModel.ProductList = _productRepository.List();
            viewModel.PointName = point.Name;
            return View(viewModel);
        }

        [Route("tank/remove")]
        [HttpPost]
        public IActionResult Remove(string tankGuid, string pointGuid)
        {
            if (!Guid.TryParse(pointGuid, out var _pointGuid) ||
                !Guid.TryParse(tankGuid, out var _tankGuid))
            {
                return NotFound();
            }
            else
            {
                if (_tankRepository.Remove(_tankGuid, _pointGuid))
                {
                    TempData["Point.Edit.SuccessMessage"] = "Резервуар удален";
                    return RedirectToAction("Edit", "Point", new { pointGuid = _pointGuid });
                }
                else
                {
                    TempData["Point.List.ErrorMessage"] = "При удалении резервуара произошла ошибка";
                    return RedirectToAction("List", "Point");
                }
            }
        }

        private new IActionResult NotFound()
        {
            ViewBag.Title = "Объект не найден";
            ViewBag.BackTitle = "назад к списку объектов";
            ViewBag.BackUrl = Url.ActionLink("List", "Point");

            return View("NotFound");
        }
    }
}