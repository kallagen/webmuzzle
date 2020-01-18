using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TSensor.Web.Models.Repository;

namespace TSensor.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IBroadcastRepository repository;

        public DashboardController(IBroadcastRepository repository)
        {
            this.repository = repository;
        }

        [Route("")]
        public IActionResult Default()
        {
            return RedirectToAction("Index", "Dashboard");
        }

        [Route("dashboard")]
        public IActionResult Index()
        {
            var actualValues = repository.GetActualSensorValues();

            return View("Old", actualValues);
        }
    }
}
