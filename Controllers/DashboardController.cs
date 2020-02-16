using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var allSensorActualValues = _pointRepository.GetSensorActualState()
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
            return View(allSensorActualValues);
        }

        [Route("dashboard")]
        [HttpPost]
        public IActionResult Index(string[] tankGuid)
        {
            return View();
        }
    }
}
