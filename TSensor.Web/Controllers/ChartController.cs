using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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

            var data = _OLAPRepository.GetSensorValues(viewModel.TankGuidList,
                viewModel.DateStart.Value.ToUniversalTime(), viewModel.DateEnd.Value.ToUniversalTime(),
                viewModel.MainParam, viewModel.AdditionalParam);

            //период меньше 3 часов - показываем сырые значения
            //период меньше суток - показываем минуты
            //все остальное - показываем часы

            var totalHours = (viewModel.DateEnd.Value - viewModel.DateStart.Value).TotalHours;
            viewModel.Values = data.Select(dataset =>
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
                        .Select(p => new { x = p.Key, y = Math.Round(p.Average(v => (decimal)v.y), 3) }),
                    backgroundColor = dataset.Key.isSecond ? "#4BC0C0" : "#36A2EB",
                    borderColor = dataset.Key.isSecond ? "#4BC0C0" : "#36A2EB",
                    yAxisID = dataset.Key.isSecond ? "y-axis-additional" : "y-axis-main"
                };
            });

            return View(viewModel);
        }
    }
}