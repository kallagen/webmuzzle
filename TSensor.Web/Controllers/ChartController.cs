using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TSensor.Web.Models.Repository;
using TSensor.Web.ViewModels.Chart;

namespace TSensor.Web.Controllers
{
    [Authorize]
    public class ChartController : Controller
    {
        private readonly IOLAPRepository _OLAPRepository;

        public ChartController(IOLAPRepository OLAPRepository)
        {
            _OLAPRepository = OLAPRepository;
        }

        [Route("chart/data")]
        [HttpPost]
        public IActionResult Data(ChartViewModel viewModel)
        {
            var datasets =
                (viewModel == null ||
                !viewModel.DateStart.HasValue ||
                !viewModel.DateEnd.HasValue ||
                (viewModel.DateEnd.Value - viewModel.DateStart.Value).TotalDays > 15 ||
                string.IsNullOrEmpty(viewModel.MainParam)
                ? Enumerable.Empty<object>()
                : GetData(viewModel.TankGuidList, viewModel.DateStart.Value, viewModel.DateEnd.Value,
                    viewModel.MainParam, viewModel.AdditionalParam));

            return Json(new { datasets });
        }

        private const string CHART_COLOR_MAIN = "#244E6D";
        private const string CHART_COLOR_SECOND = "#E57145";

        private IEnumerable<object> GetData(IEnumerable<Guid> tankGuidList,
            DateTime dateStart, DateTime dateEnd,
            string mainParam, string additionalParam)
        {
            if (dateEnd > DateTime.Now)
            {
                dateEnd = DateTime.Now;
            }

            var data = _OLAPRepository.GetSensorValues(tankGuidList,
                dateStart.ToUniversalTime(), dateEnd.ToUniversalTime(),
                mainParam, additionalParam);

            //период меньше 3 часов - показываем сырые значения
            //период меньше суток - показываем минуты
            //все остальное - показываем часы

            var totalHours = (dateEnd - dateStart).TotalHours;
            return data.Select(dataset =>
            {
                return new
                {
                    fill = "false",
                    dataset.Key.label,
                    data = dataset.Value
                        .Select(p => new
                        {
                            x = totalHours > 24 ? new DateTime(p.Key.Year, p.Key.Month, p.Key.Day, p.Key.Hour, 0, 0) :
                                (totalHours > 3 ? new DateTime(p.Key.Year, p.Key.Month, p.Key.Day, p.Key.Hour, p.Key.Minute, 0) : p.Key),
                            y = p.Value
                        })
                        .GroupBy(p => p.x)
                        .Select(p => new { x = p.Key, y = Math.Round(p.Max(v => (decimal)v.y), 3) })
                        .OrderBy(p => p.x),
                    backgroundColor = dataset.Key.isSecond ? CHART_COLOR_SECOND : CHART_COLOR_MAIN,
                    borderColor = dataset.Key.isSecond ? CHART_COLOR_SECOND : CHART_COLOR_MAIN,
                    borderWidth = 2,
                    pointRadius = 1,
                    pointHoverRadius = 2,
                    yAxisID = dataset.Key.isSecond ? "y-axis-additional" : "y-axis-main",
                    cubicInterpolationMode = "monotone"
                };
            });
        }

        [Route("chart")]
        [HttpPost]
        public IActionResult Index(ChartViewModel viewModel)
        {
            if (viewModel == null || viewModel.TankGuidList?.Any() != true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            if (viewModel.MainParam == null)
            {
                viewModel.MainParam = "WEIGHT";
            }
            if (viewModel.DateStart == null)
            {
                viewModel.DateStart = DateTime.Now.AddHours(-36);
            }
            if (viewModel.DateEnd == null)
            {
                viewModel.DateEnd = DateTime.Now;
            }
            else
            {
                if (viewModel.DateEnd > DateTime.Now)
                {
                    viewModel.DateEnd = DateTime.Now;
                }
            }

            if ((viewModel.DateEnd.Value - viewModel.DateStart.Value).TotalDays > 15)
            {
                viewModel.DateEnd = viewModel.DateStart.Value.AddDays(15);
                ModelState.Remove("DateEnd");

                viewModel.ErrorMessage = "Нельзя указать период больше 15 суток. Конец периода был автоматически изменен";
            }

            viewModel.Values = GetData(viewModel.TankGuidList,
                viewModel.DateStart.Value, viewModel.DateEnd.Value,
                viewModel.MainParam, viewModel.AdditionalParam);

            return View(viewModel);
        }
    }
}