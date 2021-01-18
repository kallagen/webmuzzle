using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Repository;
using TSensor.Web.Models.Services;
using TSensor.Web.Models.Services.Email;
using TSensor.Web.Models.Services.Log;
using TSensor.Web.Models.Services.Sms;
using TSensor.Web.ViewModels;

namespace TSensor.Web.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly IApiRepository _apiRepository;
        private readonly FileLogService _logService;
        private readonly SmsService _smsService;
        private readonly EmailService _emailService;

        private readonly int movementDelta;
        private readonly int holdTimeout;
        private readonly decimal criticalLiquidLeveldelta;
        private readonly string templateMovementStart;
        private readonly string templateMovementEnd;
        private readonly string templateLiquidLevelChangedUpStart;
        private readonly string templateLiquidLevelChangedUpEnd;
        private readonly string templateLiquidLevelChangedDownStart;
        private readonly string templateLiquidLevelChangedDownEnd;
        private static int countStoredValue = 6;
        
        enum State
        {
            UP,
            DOWN,
            NOTHING
        }
        
        private static Dictionary<Guid?, State> _statePerTank = new Dictionary<Guid?, State>();
        private static Dictionary<Guid?, List<decimal>> _lastValuesPerTank = new Dictionary<Guid?, List<decimal>>();
        public ApiController(IConfiguration configuration, IApiRepository apiRepository, 
            FileLogService logService, SmsService smsService, EmailService emailService)
        {
            _apiRepository = apiRepository;
            _logService = logService;
            _smsService = smsService;
            _emailService = emailService;

            movementDelta = configuration.GetValue<int>("movementDelta");
            criticalLiquidLeveldelta = configuration.GetValue<decimal>("criticalLiquidLeveldelta");
            holdTimeout = configuration.GetValue<int>("holdTimeout");
            countStoredValue = configuration.GetValue<int>("countStoredValue");


            templateMovementStart = configuration.GetValue<string>("smsTemplateMovementStart");
            templateMovementEnd = configuration.GetValue<string>("smsTemplateMovementEnd");
            
            templateLiquidLevelChangedUpStart = configuration.GetValue<string>("TemplateLiquidLevelChangedUpStart");
            templateLiquidLevelChangedUpEnd = configuration.GetValue<string>("TemplateLiquidLevelChangedUpEnd");
            templateLiquidLevelChangedDownStart = configuration.GetValue<string>("TemplateLiquidLevelChangedDownStart");
            templateLiquidLevelChangedDownEnd = configuration.GetValue<string>("TemplateLiquidLevelChangedDownEnd");
            
        }

        [NonAction]
        private IActionResult Error(string message, string value, string date, string guid)
        {
            _logService.Write(LogCategory.InputError, $"{guid} {date} {value} {message}");

            return Json(new { success = false, error = message });
        }
        
        
        
        [Route("sensorvalue/push")]
        [HttpPost]
        public async Task<IActionResult> PushSensorValue(string v, string d, string g)
        {
            var date = d;
            var guid = g;
            var value = v;

            if (v == null || d == null || g == null) 
                 return Error("empty archive, nothing to insert", value, null,  guid);

            try
            {
                _logService.Write(LogCategory.RawInput, $"{guid} {date} {value}");
                
                #region ErrorChecks
                //TODO раскоменти gavr !!!
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
                
                #endregion
                
                // var sensorValue = new ActualSensorValue(){liquidEnvironmentLevel = decimal.Parse(v), TankGuid = Guid.Parse(guid)};
                // var oldSensorValue = await _apiRepository.TakeLastValueAsync(sensorValue);
                // var eventUTCDate = DateTime.Today;
                sensorValue.DeviceGuid = guid;
                sensorValue.EventUTCDate = eventUTCDate;
                #region CheckStartEndFilling

                if (sensorValue.TankGuid is Guid tankGuid)
                {
                    if (!_lastValuesPerTank.ContainsKey(tankGuid))
                    {
                        _lastValuesPerTank[tankGuid] = new List<decimal>();
                    }

                    if (!_statePerTank.ContainsKey(tankGuid))
                    {
                        _statePerTank[tankGuid] = State.NOTHING;
                    }

                    //
                    if (_lastValuesPerTank[tankGuid].Count < countStoredValue - 1)
                    {
                        _lastValuesPerTank[tankGuid].Add(sensorValue.liquidEnvironmentLevel);
                    }
                    else if (_lastValuesPerTank[tankGuid].Count == countStoredValue - 1)
                    {
                        _lastValuesPerTank[tankGuid].Add(sensorValue.liquidEnvironmentLevel);
                        var state = recognizeState(tankGuid);

                        switch (_statePerTank[tankGuid])
                        {
                            case State.UP:
                            {
                                if (state == State.NOTHING)
                                {
                                    _logService.Write(LogCategory.LiquidLevel, "Changed from UP to NOTHING\n\n\n");

                                    _emailService.Send(
                                        "Налив закончился",
                                        await PrepareMessageLiquidLevelChangedTemplate(
                                            templateLiquidLevelChangedUpEnd,
                                            DateTime.Now,
                                            sensorValue.TankGuid.ToString()
                                        )
                                    );
                                    _statePerTank[tankGuid] = state;
                                }
                            }
                                break;
                            case State.DOWN:
                            {
                                if (state == State.NOTHING)
                                {
                                    _logService.Write(LogCategory.LiquidLevel, "Changed from DOWN to NOTHING\n\n\n");

                                    _emailService.Send(
                                        "Слив закончился",
                                        await PrepareMessageLiquidLevelChangedTemplate(
                                            templateLiquidLevelChangedDownEnd,
                                            DateTime.Now,
                                            sensorValue.TankGuid.ToString()
                                        )
                                    );
                                    _statePerTank[tankGuid] = state;
                                }
                            }
                                break;
                            case State.NOTHING:
                            {
                                if (state == State.UP)
                                {
                                    _logService.Write(LogCategory.LiquidLevel, "Changed from NOTHING to UP\n\n\n");
                                    _emailService.Send(
                                        "Начался налив",
                                        await PrepareMessageLiquidLevelChangedTemplate(
                                            templateLiquidLevelChangedUpStart,
                                            DateTime.Now,
                                            sensorValue.TankGuid.ToString()
                                        )
                                    );
                                    _statePerTank[tankGuid] = state;
                                }
                                else if (state == State.DOWN)
                                {
                                    _logService.Write(LogCategory.LiquidLevel, "Changed from NOTHING to DOWN\n\n\n");
                                    _emailService.Send(
                                        "Начался слив",
                                        await PrepareMessageLiquidLevelChangedTemplate(
                                            templateLiquidLevelChangedDownStart,
                                            DateTime.Now,
                                            sensorValue.TankGuid.ToString()
                                        )
                                    );
                                    _statePerTank[tankGuid] = state;
                                }
                            }
                                break;
                            // default:
                            //     throw new ArgumentOutOfRangeException();
                        }

                        _lastValuesPerTank[tankGuid].RemoveAt(0);
                    }
                    else if (_lastValuesPerTank[tankGuid].Count > countStoredValue - 1)
                    {
                        while (_lastValuesPerTank[tankGuid].Count != countStoredValue - 1)
                        {
                            _lastValuesPerTank[tankGuid].RemoveAt(0);
                        }
                    }
                }


                #endregion
                

                #region PushNewValueToDB
                
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
                
                #endregion

            }
            catch (Exception exception)
            {
                return Error(exception.ToString(), value, date, guid);
            }
        }

        #region StateRecognize

        
        private State recognizeState(Guid tankGuid)
        {
            var abstractDeltas = calcAbstractDeltas(calcDeltas(_lastValuesPerTank[tankGuid]));

            var builder = new StringBuilder();
            builder.Append($"lastValues of tank: {tankGuid}\n");
            builder.Append(string.Join(", ", _lastValuesPerTank[tankGuid]) + "\n");
            builder.Append("abstractDeltas:\n");
            builder.Append(string.Join(", ", abstractDeltas) + "\n");
            _logService.Write(LogCategory.LiquidLevel, builder.ToString());
            
            var average = abstractDeltas.Average();
            if (average >= 0.5)
            {
                return State.UP;
            } 
            else if (average <= -0.5)
            {
                return State.DOWN;
            }
            else
            {
                return State.NOTHING;
            }
        }

        public List<decimal> calcDeltas(List<decimal> values)
        {
            var list = new List<decimal>();
            for (int i = 0; i < values.Count-1; i++)
            {
                list.Add(values[i+1] - values[i]);
            }
           
            return list;
        }
        
        public List<int> calcAbstractDeltas(List<decimal> diffs)
        {
            //Пример: {0,0,0,1,1,1} -- начался набор
            // {1,1,1,0,0,0} набор закончился
            var list = new List<int>();
            foreach (var diff in diffs)
            {
                var absDiff = Math.Abs(diff);
                if (diff > 0 && absDiff >= criticalLiquidLeveldelta)
                {
                    list.Add(1);
                } 
                else if (diff < 0 && absDiff >= criticalLiquidLeveldelta )
                {
                    list.Add(-1);
                }
                else
                {
                    list.Add(0);
                }
            }
            return list;
        }

        #endregion
      

        //очень грубое приближение, расчитываем на плоскости, только для положительных исходя из 1 градус = 111.1 км
        private bool IsCoordinatesChangedSignificantly(decimal lon1, decimal lat1, decimal lon2, decimal lat2)
        {
            var lonD = (double)(lon1 - lon2);
            var latD = (double)(lat1 - lat2);

            return Math.Sqrt(lonD * lonD + latD * latD) * 111.1 * 1000 > movementDelta;
        }

        private string PrepareMessageMovementTemplate(string template, string pointName, DateTime date)
        {
            return template
                .Replace("{point}", pointName)
                .Replace("{date}", date.ToString("dd.MM.yyyy"))
                .Replace("{time}", date.ToString("HH:mm"));
        }
        
        private async Task<string> PrepareMessageLiquidLevelChangedTemplate(string template, DateTime date, string tankGuid)
        {
            var pointTankNameFromGuid = await _apiRepository.TakePointTankNameFromGuidAsync(tankGuid);
            return template
                .Replace("{name}", pointTankNameFromGuid)
                .Replace("{date}", date.ToString("dd.MM.yyyy"))
                .Replace("{time}", date.ToString("HH:mm"));
        }
        
        [Route("coordinates/push")]
        [HttpGet]
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
                var pointInfoList = await _apiRepository.UploadPointCoordinatesAsync(d, _lon, _lat);
                foreach (var point in pointInfoList)
                {
                    var isPointMoving = point.IsMoving as bool?;
                    var pointLastMovingDateUTC = point.LastMovingDateUTC as DateTime?;
                    var newLongitude = point.Longitude as decimal?;
                    var newLatitude = point.Latitude as decimal?;

                    var coordinatesChanged = newLongitude != _lon || newLatitude != _lat ?
                            true as bool? : null;
                    var coordinatesChangedSignificantly = newLongitude.HasValue && newLatitude.HasValue && 
                        IsCoordinatesChangedSignificantly(_lon, _lat, newLongitude.Value, newLatitude.Value);

                    bool? isMoving = null;
                    DateTime? lastMovingDateUTC = null;

                    var currentDateUTC = DateTime.Now.ToUniversalTime();

                    if (isPointMoving == true)
                    {
                        if (coordinatesChangedSignificantly)
                        {
                            lastMovingDateUTC = currentDateUTC;
                        }
                        else
                        {
                            if ((currentDateUTC - pointLastMovingDateUTC.Value).TotalSeconds > holdTimeout)
                            {
                                _emailService.Send(
                                    "Остановка",
                                    PrepareMessageMovementTemplate(
                                        templateMovementEnd,
                                        point.PointName,
                                        pointLastMovingDateUTC.Value.ToLocalTime()
                                    )
                                );
                                
                                isMoving = false;
                            }
                        }
                    }
                    else
                    {
                        if (coordinatesChangedSignificantly)
                        {
                            _emailService.Send(
                                "Начало движения",
                                PrepareMessageMovementTemplate(
                                    templateMovementStart,
                                    point.PointName,
                                    DateTime.Now
                                )
                            );
                            
                            isMoving = true;
                            lastMovingDateUTC = currentDateUTC;
                        }
                    }

                    await _apiRepository.UpdatePointCoordinate(point.PointGuid, _lon, _lat,
                        coordinatesChanged, isMoving, lastMovingDateUTC);
                }

                return Json(new { success = true, command = "DELAY SAS" });
            }
            catch (Exception ex)
            {
                _logService.Write(LogCategory.Exception, ex.Message);

                return Json(new { success = false, error = ex.Message });
            }
        }

        
        #region Archive

        [Route("sensorvalue/archive/push")]
        [HttpPost]
        public async Task<IActionResult> PushArchivedSensorValues(string d, IFormFile file)
        {
            var deviceGuid = d;

            using var reader = new StreamReader(file.OpenReadStream());
            var value = reader.ReadToEnd() ?? string.Empty;

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
    
        #endregion
        
    }
}