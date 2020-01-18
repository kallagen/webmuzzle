using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using TSensor.Web.Models.Repository;
using TSensor.Web.Models.Services.Log;

namespace TSensor.Web.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly IRepository _repository;
        private readonly FileLogService _logService;

        public ApiController(IRepository repository, FileLogService logService)
        {
            _repository = repository;
            _logService = logService;
        }

        [NonAction]
        private IActionResult Error(string message)
        {
            return Json(new { success = false, error = message });
        }

        [Route("sensorvalue/push")]
        [HttpPost]
        public IActionResult PushSensorValue(string v, string d, string g)
        {
            try
            {
                _logService.Write("rawinput", $"{g} {d} {v}");

                if (string.IsNullOrWhiteSpace(v))
                {
                    return Error("missing sensor value");
                }

                var dateParseResult = DateTime.TryParseExact(d, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var eventDateUTC);
                if (!dateParseResult)
                {
                    return Error("wrong event utc date");
                }

                if (string.IsNullOrWhiteSpace(g))
                {
                    return Error("missing device guid");
                }

                if (_repository.PushValue(
                    Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    v, eventDateUTC, g))
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Error("no record inserted to db()");
                }
            }
            catch (Exception e)
            {
                return Error(e.ToString());
            }
        }
    }
}
