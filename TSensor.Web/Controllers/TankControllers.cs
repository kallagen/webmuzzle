using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Globalization;
using System.Linq;
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
            viewModel.SecondDeviceGuid = viewModel.SecondDeviceGuid?.Trim();
            viewModel.WeightChangeDelta = viewModel.WeightChangeDelta?.Trim();
            viewModel.WeightChangeTimeout = viewModel.WeightChangeTimeout?.Trim();

            viewModel.Validate(ModelState);
            if (ModelState.IsValid)
            {
                var tankGuid = _tankRepository.Create(
                    viewModel.PointGuid.Value, viewModel.Name, viewModel.ProductGuid, viewModel.DualMode,
                    viewModel.MainDeviceGuid, viewModel.MainIZKId, viewModel.MainSensorId,
                    viewModel.SecondDeviceGuid, viewModel.SecondIZKId, viewModel.SecondSensorId,
                    viewModel.Description, viewModel.ParsedWeightChangeDelta, viewModel.ParsedWeightChangeTimeout);
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
                Description = tank.Description,
                WeightChangeDelta = tank.WeightChangeDelta?.ToString(),
                WeightChangeTimeout = tank.WeightChangeTimeout?.ToString()
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
            viewModel.SecondDeviceGuid = viewModel.SecondDeviceGuid?.Trim();
            viewModel.WeightChangeDelta = viewModel.WeightChangeDelta?.Trim();
            viewModel.WeightChangeTimeout = viewModel.WeightChangeTimeout?.Trim();

            viewModel.Validate(ModelState);
            if (ModelState.IsValid && viewModel.TankGuid.HasValue)
            {
                var editResult = _tankRepository.Edit(
                    viewModel.TankGuid.Value, viewModel.PointGuid.Value, viewModel.Name,
                    viewModel.ProductGuid, viewModel.DualMode,
                    viewModel.MainDeviceGuid, viewModel.MainIZKId, viewModel.MainSensorId,
                    viewModel.SecondDeviceGuid, viewModel.SecondIZKId, viewModel.SecondSensorId,
                    viewModel.Description, viewModel.ParsedWeightChangeDelta, viewModel.ParsedWeightChangeTimeout);
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

        [Route("tank/{guid}")]
        public IActionResult Details(string guid)
        {
            if (Guid.TryParse(guid, out var _tankGuid))
            {
                var tankInfo = _tankRepository.GetTankActualSensorValues(_tankGuid);
                if (tankInfo != null)
                {
                    var viewModel = new TankDetailsViewModel
                    {
                        TankGuid = tankInfo.TankGuid,
                        TankName = tankInfo.TankName,
                        DualMode = tankInfo.DualMode,
                        ProductName = tankInfo.ProductName
                    };

                    if (tankInfo.InsertDate != null)
                    {
                        viewModel.Value = ActualSensorValue.Parse(tankInfo);
                    }

                    return View(viewModel);
                }
            }

            ViewBag.Title = "Резервуар не найден";
            return View("NotFound");
        }

        private void FillExportHeaders(ExcelWorksheet sheet, bool dualMode)
        {
            var idx = 1;

            sheet.Cells[1, idx++].Value = "Дата события";
            sheet.Cells[1, idx++].Value = "Адрес ИЗК";
            sheet.Cells[1, idx++].Value = "Тип посылки";
            sheet.Cells[1, idx++].Value = "Адрес датчика";
            sheet.Cells[1, idx++].Value = "Номер канала";
            sheet.Cells[1, idx++].Value = "pressureAndTempSensorState";
            sheet.Cells[1, idx++].Value = "Версия ПО датчика";
            sheet.Cells[1, idx++].Value = "Бит сигнализации";
            sheet.Cells[1, idx++].Value = "Уровень, мм";
            if (dualMode)
            {
                sheet.Cells[1, idx++].Value = "Сигнальный уровень, атм";
            }
            sheet.Cells[1, idx++].Value = "Давление измер, атм";
            sheet.Cells[1, idx++].Value = "Объем, %";
            sheet.Cells[1, idx++].Value = "Объем, м³";
            sheet.Cells[1, idx++].Value = "Масса жидкости, т";
            sheet.Cells[1, idx++].Value = "Масса пара, т";
            sheet.Cells[1, idx++].Value = "Плотность жидкости, кг/м³";
            sheet.Cells[1, idx++].Value = "Плотность пара, кг/м³";
            sheet.Cells[1, idx++].Value = "Диэлектрическая проницаемость жидкости";
            sheet.Cells[1, idx++].Value = "Диэлектрическая проницаемость пара";
            sheet.Cells[1, idx++].Value = "Температура нижнего датчика ?, °C";
            sheet.Cells[1, idx++].Value = "Температура, °C";
            sheet.Cells[1, idx++].Value = "Температура, °C";
            sheet.Cells[1, idx++].Value = "Температура, °C";
            sheet.Cells[1, idx++].Value = "Температура, °C";
            sheet.Cells[1, idx++].Value = "Температура верхнего датчика?, °C";
            sheet.Cells[1, idx++].Value = "Температура платы, °C";
            sheet.Cells[1, idx++].Value = "Период платы";
            sheet.Cells[1, idx++].Value = "plateServiceParam";
            sheet.Cells[1, idx++].Value = "Состав среды, %";
            sheet.Cells[1, idx++].Value = "Измеренная емкость платы, Пф";
            sheet.Cells[1, idx++].Value = "plateServiceParam2";
            sheet.Cells[1, idx++].Value = "plateServiceParam3";
            sheet.Cells[1, idx++].Value = "sensorWorkMode";
            sheet.Cells[1, idx++].Value = "plateServiceParam4";
            sheet.Cells[1, idx++].Value = "plateServiceParam5";
            sheet.Cells[1, idx++].Value = "Контрольная сумма";
        }

        private void FillExportValues(ExcelWorksheet sheet, ActualSensorValue value, int row, bool dualMode)
        {
            var idx = 1;
            sheet.Cells[row, idx++].Value = value.EventUTCDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
            sheet.Cells[row, idx++].Value = value.izkNumber;
            sheet.Cells[row, idx++].Value = value.banderolType;
            sheet.Cells[row, idx++].Value = value.sensorSerial;
            sheet.Cells[row, idx++].Value = value.sensorChannel;
            sheet.Cells[row, idx++].Value = value.pressureAndTempSensorState;
            sheet.Cells[row, idx++].Value = value.sensorFirmwareVersionAndReserv;
            sheet.Cells[row, idx++].Value = value.alarma;
            sheet.Cells[row, idx++].Value = value.environmentLevel;
            if (dualMode)
            {
                sheet.Cells[row, idx++].Value = value.pressureFilter;
            }            
            sheet.Cells[row, idx++].Value = value.pressureMeasuring;
            sheet.Cells[row, idx++].Value = value.levelInPercent;
            sheet.Cells[row, idx++].Value = value.environmentVolume;
            sheet.Cells[row, idx++].Value = value.liquidEnvironmentLevel;
            sheet.Cells[row, idx++].Value = value.steamMass;
            sheet.Cells[row, idx++].Value = value.liquidDensity;
            sheet.Cells[row, idx++].Value = value.steamDensity;
            sheet.Cells[row, idx++].Value = value.dielectricPermeability;
            sheet.Cells[row, idx++].Value = value.dielectricPermeability2;
            sheet.Cells[row, idx++].Value = value.t1;
            sheet.Cells[row, idx++].Value = value.t2;
            sheet.Cells[row, idx++].Value = value.t3;
            sheet.Cells[row, idx++].Value = value.t4;
            sheet.Cells[row, idx++].Value = value.t5;
            sheet.Cells[row, idx++].Value = value.t6;
            sheet.Cells[row, idx++].Value = value.plateTemp;
            sheet.Cells[row, idx++].Value = value.period;
            sheet.Cells[row, idx++].Value = value.plateServiceParam;
            sheet.Cells[row, idx++].Value = value.environmentComposition;
            sheet.Cells[row, idx++].Value = value.cs1;
            sheet.Cells[row, idx++].Value = value.plateServiceParam2;
            sheet.Cells[row, idx++].Value = value.plateServiceParam3;
            sheet.Cells[row, idx++].Value = value.sensorWorkMode;
            sheet.Cells[row, idx++].Value = value.plateServiceParam4;
            sheet.Cells[row, idx++].Value = value.plateServiceParam5;
            sheet.Cells[row, idx++].Value = value.crc;
        }

        [Route("tank/values/export")]
        [HttpPost]
        public IActionResult Export(string tankGuid, string exportDateStart, string exportDateEnd)
        {
            Tank tank = null;
            if (Guid.TryParse(tankGuid, out var _tankGuid))
            {
                tank = _tankRepository.GetByGuid(_tankGuid);
            }

            if (tank == null)
            {
                ViewBag.Title = "Резервуар не найден";
                return View("NotFound");
            }

            if (
                DateTime.TryParseExact(exportDateStart, new[] { "dd.MM.yyyy", "dd.MM.yyyy HH:mm", "dd.MM.yyyy H:mm" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var _dateStart) &&
                DateTime.TryParseExact(exportDateEnd, new[] { "dd.MM.yyyy", "dd.MM.yyyy HH:mm", "dd.MM.yyyy H:mm" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var _dateEnd))
            {
                var data = _tankRepository.GetTankSensorValuesHistory(_tankGuid, _dateStart.ToUniversalTime(), _dateEnd.ToUniversalTime());

                using var package = new ExcelPackage();

                package.Workbook.Worksheets.Add("Показания основного датчика");
                var sheet = package.Workbook.Worksheets[0];
                sheet.Name = "Показания основного датчика";

                FillExportHeaders(sheet, tank.DualMode);

                var row = 2;
                foreach (var value in data.Where(p => p.IsSecond != true))
                {
                    FillExportValues(sheet, value, row++, tank.DualMode);
                }

                if (tank.DualMode)
                {
                    package.Workbook.Worksheets.Add("Показания дополнительного датчика");
                    var secondSheet = package.Workbook.Worksheets[1];
                    secondSheet.Name = "Показания дополнительного датчика";

                    FillExportHeaders(secondSheet, tank.DualMode);

                    row = 2;
                    foreach (var value in data.Where(p => p.IsSecond == true))
                    {
                        FillExportValues(secondSheet, value, row++, tank.DualMode);
                    }
                }

                return new FileContentResult(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"Экспорт {tank.Name} {tank.ProductName} с {_dateStart:dd.MM.yyyy HH.mm)} по {_dateEnd:dd.MM.yyyy HH.mm}.xlsx"
                };
            }
            else
            {
                ViewBag.Title = "Неверные значения дат при экспорте";
                return View("NotFound");
            }
        }
    }
}