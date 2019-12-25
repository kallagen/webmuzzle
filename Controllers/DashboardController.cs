using Microsoft.AspNetCore.Mvc;
using TSensor.Web.Models.Repository;

namespace TSensor.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IBroadcastRepository repository;

        public DashboardController(IBroadcastRepository repository)
        {
            this.repository = repository;
        }

        [Route("")]
        public IActionResult Index()
        {
            var actualValues = repository.GetActualSensorValues();

            return View(actualValues);
        }
    }
}
