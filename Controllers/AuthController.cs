using Microsoft.AspNetCore.Mvc;

namespace TSensor.Web.Controllers
{
    public class AuthController : Controller
    {
        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }
    }
}
