using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TSensor.Web.Models.Repository;

namespace TSensor.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IBroadcastRepository _broadcastRepository;

        public DashboardController(IBroadcastRepository broadcastRepository)
        {
            _broadcastRepository = broadcastRepository;
        }

        [Route("")]
        public IActionResult Default()
        {
            return RedirectToAction("Index", "Dashboard");
        }

        [Route("dashboard")]
        public IActionResult Index()
        {
            var allSensorActualValues = _broadcastRepository.GetAllSensorActualState();

            return View(allSensorActualValues);
        }
    }
}
