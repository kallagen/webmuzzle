using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TSensor.Web.Models.Services.Log;

namespace TSensor.Web.Controllers
{
    public class SystemController : Controller
    {
        private readonly FileLogService _logService;

        public SystemController(FileLogService logService)
        {
            _logService = logService;
        }

        [Route("error")]
        public IActionResult Error()
        {
            var error =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>()?.Error;
            if (error != null)
            {
                _logService.Write(LogCategory.SystemException, error.Message);
            }

            return View();
        }
    }
}