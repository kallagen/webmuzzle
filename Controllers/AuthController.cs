using Microsoft.AspNetCore.Mvc;

namespace TSensor.Web.Controllers
{
    [Route("login")]
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
