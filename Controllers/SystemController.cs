using Microsoft.AspNetCore.Mvc;

namespace TSensor.Web.Controllers
{
    public class SystemController : Controller
    {
        [Route("error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}