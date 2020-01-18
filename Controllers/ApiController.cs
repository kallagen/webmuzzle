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
        private IActionResult Error(string message, string value, string date, string guid)
        {
            _logService.Write("inputerror", $"{guid} {date} {value} {message}");

            return Json(new { success = false, error = message });
        }

        [Route("sensorvalue/push")]
        [HttpPost]
        public IActionResult PushSensorValue(string v, string d, string g)
        {
            var date = d;
            var guid = g;
            var value = v;

            try
            {
                _logService.Write("rawinput", $"{guid} {date} {value}");

                if (string.IsNullOrWhiteSpace(value))
                {
                    return Error("missing sensor value", value, date, guid);
                }

                var dateParseResult = DateTime.TryParseExact(date,
                    new[] 
                    {
                        "yyyy-MM-dd HH:mm:ss.fff",
                        "yyyy-MM-dd HH:mm:ss.ff",
                        "yyyy-MM-dd HH:mm:ss.f",
                        "yyyy-MM-dd HH:mm:ss.",
                        "yyyy-MM-dd HH:mm:ss"
                    }, 
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var eventDateUTC);
                if (!dateParseResult)
                {
                    return Error("wrong event utc date", value, date, guid);
                }

                if (string.IsNullOrWhiteSpace(guid))
                {
                    return Error("missing device guid", value, date, guid);
                }

                if (_repository.PushValue(
                    Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    value, eventDateUTC, guid))
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Error("no record inserted to db", value, date, guid);
                }
            }
            catch (Exception exception)
            {
                return Error(exception.ToString(), value, date, guid);
            }
        }
    }
}
