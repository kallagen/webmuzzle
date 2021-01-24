using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TSensor.Web.Models.Controller;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Repository;
using TSensor.Web.ViewModels.ControllerSettings;

namespace TSensor.Web.Controllers
{
    [Route("controller")]
    public class ControllerController : Controller
    {
        private readonly IControllerSettingsRepository _settingsRepository;
        private readonly IControllerCommandRepository _commandRepository;
        private const string RESET_COMMAND = "RESET";

        public ControllerController(
            IControllerSettingsRepository settingsRepository,
            IControllerCommandRepository commandRepository
        )
        {
            _settingsRepository = settingsRepository;
            _commandRepository = commandRepository;
        }

        [Route("controller/send")]
        public IActionResult Edit()
        {

            var viewModel = new IzkDetailsViewModel()
            { CommandForController = "" };
            
            var successMessage = TempData["ControllerSettings.Edit.SuccessMessage"] as string;
            if (!string.IsNullOrEmpty(successMessage))
            {
                viewModel.SuccessMessage = successMessage;
            }

            var errorMessage = TempData["ControllerSettings.Edit.ErrorMessage"] as string;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                viewModel.ErrorMessage = errorMessage;
            }

            return View("Details", viewModel);

            // return View(viewModel);
        }
        
        [Authorize(Policy = "Admin")]
        [Route("controller/send")]
        [HttpPost]
        public IActionResult Edit(IzkDetailsViewModel viewModel)
        {
            if (viewModel == null)
                return RedirectToAction("Edit", "Controller");
            
            if (string.IsNullOrEmpty( viewModel.CommandForController))
                ModelState.AddModelError("CommantForController", "Комманда должна быть заполнена");
            else if (viewModel.CommandForController.Length > 100)
                ModelState.AddModelError("CommantForController", "Длинна команды должна быть меньше 100 символов");

            if (!string.IsNullOrEmpty(viewModel.DeviceGuid) && ModelState.IsValid)
            {
                //TODO каким то образом устройсства должны появлятся в ControllerSettings репе
                if (!string.IsNullOrEmpty(viewModel.DeviceGuid) && _settingsRepository.ControllerExist(viewModel.DeviceGuid))
                {

                    var guid = _commandRepository.UploadCommand(viewModel.CommandForController, viewModel.DeviceGuid, viewModel.IzkNumber ?? -1); //TODO gavr возможно не -1
                    if (guid != null)
                    {
                        TempData["ControllerSettings.Edit.SuccessMessage"] =
                            $"Команда {viewModel.CommandForController} отправлена";
                        return RedirectToAction("Edit", "Controller");
                    }
                    else
                    {
                        TempData["ControllerSettings.Edit.ErrorMessage"] =
                            $"Ошибка при отправке команды";
                        return RedirectToAction("Edit", "Controller");
                    }
                }
                else
                {
                    TempData["ControllerSettings.Edit.ErrorMessage"] =
                        $"Введенный GUID девайса не найден в базе";
                    return RedirectToAction("Edit", "Controller");
                }
            }

            return View("Details", viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("settings/reset")]
        [HttpPost]
        public IActionResult Reset(IzkDetailsViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("Edit", "Controller");
            }

            if (!string.IsNullOrEmpty(viewModel.DeviceGuid) && _settingsRepository.ControllerExist(viewModel.DeviceGuid))
            {
                var guid = _commandRepository.UploadCommand(RESET_COMMAND, viewModel.DeviceGuid, viewModel.IzkNumber ?? -1); //TODO gavr возможно не -1
                if (guid != null)
                {
                    TempData["ControllerSettings.Edit.SuccessMessage"] =
                        $"Контроллеру отправлена команда перезапуска";
                    return RedirectToAction("Edit", "Controller");
                }
                else
                {
                    TempData["ControllerSettings.Edit.ErrorMessage"] =
                        $"Ошибка при отправке команды";
                    return RedirectToAction("Edit", "Controller");
                }
            }
            else
            {
                TempData["ControllerSettings.Edit.ErrorMessage"] =
                    $"Введенный GUID девайса не найден в базе";
                return RedirectToAction("Edit", "Controller");
            }
            
            
            return RedirectToAction("Edit", "Controller");
        }

        
        [Route("lastcommand/get")]
        [HttpGet]
        public async Task<ActionResult<LatestControllerCommand>> GetLastCommand(string deviceGuid)
        {
            var latestCC = await _commandRepository.GetLastCommand(deviceGuid);
            if (latestCC == null)
                return NotFound();
            
            return latestCC;
        }
        
        //Сюда приходим когда уже выбрали конкретный контроллер со страницы SelectController
        [Route("controller/{guid}/details")]
        public IActionResult Details(string deviceGuid, string mainIzkId)
        {
            if (Guid.TryParse(deviceGuid, out var _deviceGuid) && int.TryParse(mainIzkId, out var _izkNumber))
            {
                var viewModel = new IzkDetailsViewModel()
                {
                    DeviceGuid = _deviceGuid.ToString(),
                    IzkNumber = _izkNumber,
                };
                return View(viewModel);
                // if (tankInfo != null)
                // {
                //     var viewModel = new TankDetailsViewModel
                //     {
                //         TankGuid = _tankGuid,
                //         TankName = tankInfo.TankName,
                //         DualMode = tankInfo.DualMode,
                //         PointName = tankInfo.PointName,
                //         ProductName = tankInfo.ProductName,
                //         HasCalibrationRights = HttpContext.User.IsInRole("ADMIN"),
                //         HasCalibrationData = _tankRepository.HasTankCalibrationData(_tankGuid),
                //         IsMassmeter = tankInfo.PointGuid == PointRepository.MASSMETER_POINT_GUID
                //     };
                //
                //     if (viewModel.HasCalibrationRights && !isMassmeter)
                //     {
                //         viewModel.MassmeterList = _tankRepository.GetListByPoint(PointRepository.MASSMETER_POINT_GUID)
                //             .ToDictionary(p => p.TankGuid, p => p.Name);
                //     }
                //
                //     if (tankInfo.InsertDate != null)
                //     {
                //         viewModel.Value = ActualSensorValue.Parse(tankInfo);
                //     }
                //
                //     var successMessage = TempData["Tank.Details.SuccessMessage"] as string;
                //     if (!string.IsNullOrEmpty(successMessage))
                //     {
                //         viewModel.SuccessMessage = successMessage;
                //     }
                //     var errorMessage = TempData["Tank.Details.ErrorMessage"] as string;
                //     if (!string.IsNullOrEmpty(errorMessage))
                //     {
                //         viewModel.ErrorMessage = errorMessage;
                //     }
                //
                //     return View("Details", viewModel);
                // }
            }

            ViewBag.Title = "ИЗК не найден";
            return View("NotFound");
        }


        #region setStatus

        [Route("command/setfail")]
        [HttpPost]
        public async Task<ActionResult<ControllerCommand>> SetWhyCommandFailed(
            string deviceGuid, 
            string commandGuid,
            string failReason
        )
        {
            var latestCC = await _commandRepository.SetFailReason(commandGuid, failReason);
            if (latestCC == null)
                return NotFound();
            
            return latestCC;
        } 
        
        [Route("command/setcomplete")]
        [HttpPost]
        public async Task<ActionResult<ControllerCommand>> SetCommandComplete(
            string deviceGuid, 
            string commandGuid
        )
        {
            var latestCC = await _commandRepository.SetCompleteState(commandGuid);
            if (latestCC == null)
                return NotFound();
            
            return latestCC;
        } 
        
        // disabled, enabled, restarting 
        [Route("status/set")]
        [HttpPost]
        public async Task<ActionResult<string>> SetControllerStatus(
            string deviceGuid, 
            string newStatus
        )
        {
            if (!new string[] {"disabled", "enabled", "restarting"}.Contains(newStatus))
            {
                return NotFound("New status must be one of disabled/enabled/restarting");
            }
            
            var commandGuid = await _commandRepository.UploadDeviceStatus(deviceGuid, newStatus);
            if (commandGuid == null)
                return NotFound();
            
            return commandGuid;
        }

        #endregion setStatus
        
    }
}