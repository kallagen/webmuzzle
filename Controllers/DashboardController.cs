using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TSensor.Web.Models.Repository;

namespace TSensor.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IPointRepository _pointRepository;

        public DashboardController(IPointRepository pointRepository)
        {
            _pointRepository = pointRepository;
        }

        [Route("")]
        public IActionResult Default()
        {
            return RedirectToAction("Index", "Dashboard");
        }

        [Route("dashboard")]
        public IActionResult Index()
        {
            /*var allSensorActualValues = _pointRepository.GetSensorActualState()
                .GroupBy(p => p.PointGuid).OrderBy(p =>
                {
                    if (p.Any(t => !t.PointGuid.HasValue))
                    {
                        return 0;
                    }

                    var hasError = p.Any(t => t.IsError);
                    var hasWarning = p.Any(t => t.IsWarning);

                    if (hasError && hasWarning)
                    {
                        return 1;
                    }
                    else if (hasError)
                    {
                        return 2;
                    }
                    else if (hasWarning)
                    {
                        return 3;
                    }
                    else
                    {
                        return 4;
                    }
                }).ThenBy(p => p.First().PointName);
            return View(allSensorActualValues);*/
            return View();
        }

        [Route("dashboard")]
        [HttpPost]
        public IActionResult Index(string[] guidList)
        {
            //todo check user rights

            var tankGuidList = (guidList ?? Enumerable.Empty<string>())
                .Select(p => Guid.TryParse(p, out var tankGuid) ? (Guid?)tankGuid : null)
                .Where(p => p != null);

            var sensorActualValues = _pointRepository.GetSensorActualState(tankGuidList)
                .OrderBy(t =>
                {
                    if (t.IsError && t.IsWarning)
                    {
                        return 1;
                    }
                    else if (t.IsError)
                    {
                        return 2;
                    }
                    else if (t.IsWarning)
                    {
                        return 3;
                    }
                    else
                    {
                        return 4;
                    }
                }).ThenBy(t => t.PointName).ThenBy(t => t.TankName);

            ViewBag.SelectedMenuElements = guidList;
            return View(sensorActualValues);
        }
    }
}
