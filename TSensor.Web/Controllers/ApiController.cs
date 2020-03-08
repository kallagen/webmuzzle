using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Repository;
using TSensor.Web.Models.Services.Log;

namespace TSensor.Web.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly IApiRepository _apiRepository;
        private readonly FileLogService _logService;

        public ApiController(IApiRepository apiRepository, FileLogService logService)
        {
            _apiRepository = apiRepository;
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
        public async Task<IActionResult> PushSensorValue(string v, string d, string g)
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
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var eventUTCDate);
                if (!dateParseResult)
                {
                    return Error("wrong event utc date", value, date, guid);
                }

                if (string.IsNullOrWhiteSpace(guid))
                {
                    return Error("missing device guid", value, date, guid);
                }

                var sensorValue = ActualSensorValue.Parse(value);
                sensorValue.DeviceGuid = guid;
                sensorValue.EventUTCDate = eventUTCDate;

                if (await _apiRepository.PushValueAsync(
                    Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    sensorValue, value))
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

        [Route("sensorvalue/archive/push")]
        [HttpPost]
        public async Task<IActionResult> PushArchivedSensorValues(string d, string a)
        {
            var value = a;
            var deviceGuid = d;

            if (string.IsNullOrWhiteSpace(deviceGuid))
            {
                return Error("missing device guid", value, null, deviceGuid);
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                return Error("empty archive, nothing to insert", value, null, deviceGuid);
            }

            try
            {
                var valueList = a.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p =>
                    {
                        var lexem = p.Split(";");

                        var eventDateParseResult = DateTime.TryParseExact(lexem[0],
                        new[]
                        {
                            "yyyy-MM-dd HH:mm:ss.fff",
                            "yyyy-MM-dd HH:mm:ss.ff",
                            "yyyy-MM-dd HH:mm:ss.f",
                            "yyyy-MM-dd HH:mm:ss.",
                            "yyyy-MM-dd HH:mm:ss"
                        },
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var eventUTCDate);

                        if (!eventDateParseResult)
                        {
                            return null;
                        }

                        try
                        {
                            var value = ActualSensorValue.Parse(lexem[1], storeRaw: true);
                            value.EventUTCDate = eventUTCDate;

                            return value;
                        }
                        catch
                        {
                            return null;
                        }
                    }).Where(p => p != null);

                if (valueList.Any())
                {
                    await _apiRepository.PushArchivedValuesAsync(
                    Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    deviceGuid, valueList);

                    return Json(new { success = true });
                }
                else
                {
                    return Error("no readable records in archive", value, null, deviceGuid);
                }
            }
            catch (Exception ex)
            {
                return Error(ex.ToString(), value, null, deviceGuid);
            }
        }
    }
}
