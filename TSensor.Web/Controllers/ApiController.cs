using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Repository;
using TSensor.Web.Models.Services;
using TSensor.Web.Models.Services.Log;
using TSensor.Web.ViewModels;

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

                var sensorValue = ActualSensorValue.TryParse(value);
                if (sensorValue == null)
                {
                    return Error("wrong value format", value, date, guid);
                }

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
                var result = await ParseArchiveAsync(value, deviceGuid, legacySupport: true);
                if (result.parsed != 0)
                {
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

        private async Task<dynamic> ParseArchiveAsync(string content, string deviceGuid = null, bool legacySupport = false)
        {
            var error = 0;

            var valueList = (content ?? string.Empty).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Where(p => !string.IsNullOrEmpty(p))
                .Select(p =>
                {
                    bool isLegacy;

                    var lexem = p.Split(";");
                    if (lexem.Length == 3)
                    {
                        isLegacy = false;
                    }
                    else if (legacySupport && lexem.Length == 2)
                    {
                        isLegacy = true;
                    }
                    else
                    {
                        error++;
                        return null;
                    }

                    var eventDateParseResult = DateTime.TryParseExact(
                        isLegacy ? lexem[0] : lexem[1],
                        new[]
                        {
                            "yyyy-MM-dd HH:mm:ss.fff",
                            "yyyy-MM-dd HH:mm:ss.ff",
                            "yyyy-MM-dd HH:mm:ss.f",
                            "yyyy-MM-dd HH:mm:ss.",
                            "yyyy-MM-dd HH:mm:ss"
                        }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var eventUTCDate);
                    if (eventDateParseResult)
                    {
                        var value = ActualSensorValue.TryParse(
                            isLegacy ? lexem[1] : lexem[2], storeRaw: true);

                        if (value != null)
                        {
                            value.EventUTCDate = eventUTCDate;
                            value.DeviceGuid = isLegacy ? deviceGuid : lexem[0];
                        }

                        return value;
                    }

                    error++;
                    return null;
                }).Where(p => p != null);

            if (valueList.Any())
            {
                await _apiRepository.PushArchivedValuesAsync(
                    Request.HttpContext.Connection.RemoteIpAddress.ToString(), valueList);
            }

            return new
            {
                error,
                parsed = valueList.Count()
            };
        }

        [Authorize(Policy = "Admin")]
        [Route("archive/upload")]
        public IActionResult UploadArchive()
        {
            var viewModel = new ViewModelBase();

            var successMessage = TempData["Api.UploadArchive.SuccessMessage"] as string;
            if (!string.IsNullOrEmpty(successMessage))
            {
                viewModel.SuccessMessage = successMessage;
            }
            var errorMessage = TempData["Api.UploadArchive.ErrorMessage"] as string;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                viewModel.ErrorMessage = errorMessage;
            }

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("archive/upload")]
        [HttpPost]
        public async Task<IActionResult> UploadArchive(IFormFile file)
        {
            if (file != null)
            {
                try
                {
                    using var reader = new StreamReader(file.OpenReadStream());
                    var result = await ParseArchiveAsync(reader.ReadToEnd());

                    if (result.parsed != 0)
                    {
                        TempData["Api.UploadArchive.SuccessMessage"] =
                            $"Загружено {result.parsed} показаний датчика";
                        return RedirectToAction("UploadArchive", "Api");
                    }
                    else
                    {
                        TempData["Api.UploadArchive.ErrorMessage"] =
                            "Файл не содержит подходящих показаний с датчиков";
                    }
                }
                catch
                {
                    TempData["Api.UploadArchive.ErrorMessage"] = Program.GLOBAL_ERROR_MESSAGE;
                }
            }
            else
            {
                TempData["Api.UploadArchive.ErrorMessage"] =
                            "Файл не содержит подходящих показаний с датчиков";
            }

            return RedirectToAction("UploadArchive", "Api");
        }

        [Route("coordinates/push")]
        [HttpPost]
        public async Task<IActionResult> PushCoordinates(string d, string lon, string lat)
        {
            if (string.IsNullOrEmpty(d))
            {
                return Json(new { success = false, error = "missing device guid" });
            }
            if (!lon.TryParseDecimal(out var _lon))
            {
                return Json(new { success = false, error = "wrong longitude" });
            }
            if (!lat.TryParseDecimal(out var _lat))
            {
                return Json(new { success = false, error = "wrong latitude" });
            }

            try
            {
                await _apiRepository.UploadPointCoordinatesAsync(d, _lon, _lat);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}