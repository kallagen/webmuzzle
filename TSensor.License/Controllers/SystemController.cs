using Microsoft.AspNetCore.Mvc;

namespace TSensor.License.Controllers
{
    public class SystemController : Controller
    {
        [Route("")]
        public IActionResult Default()
        {
            return Content(string.Empty);
        }

        [Route("error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}