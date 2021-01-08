using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TSensor.Web.Models.Repository;
using TSensor.Web.ViewModels.ControllerSettings;

namespace TSensor.Web.Controllers
{
    [Authorize(Policy = "Admin")]
    public class ControllerController : Controller
    {
        private readonly IControllerSettingsRepository _settingsRepository;
        private readonly IControllerCommandRepository _commandRepository;

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

            var viewModel = new ControllerSettingsEditViewModel()
            {
                CommandForController = ""
            };
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

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("settings/controller/reset")]
        [HttpPost]
        public IActionResult Reset(ControllerSettingsEditViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("Edit", "Controller");
            }
            
            //TODO
            // if (КОНТРОЛЛЕР ВЫКЛЮЧЕН)
            // {
            //     ModelState.AddModelError("Controller", "Контроллер выключен или перезагружается");
            // }

            //TODO ВАЛИДИРОВАТЬ будем по конкретным полям тк кк не для всех действий все на должно быть валидно
            if (ModelState.IsValid)
            {
                // var result = _repository.SendNewControllerCommand(viewModel.MaxZoom, 
                //     viewModel.DefaultLongitude, viewModel.DefaultLatitude);
                // if (result)
                // {
                //TODO отправлять в репу то что введено
                TempData["ControllerSettings.Edit.SuccessMessage"] =
                    $"Контроллеру отправлена команда перезапуска";
                
                return RedirectToAction("Edit", "Controller");
                // }
                // else
                // {
                //     viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                // }
            }
            
            return RedirectToAction("Edit", "Controller");
        }

        [Authorize(Policy = "Admin")]
        [Route("controller/send")]
        [HttpPost]
        public IActionResult Send(ControllerSettingsEditViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("Edit", "Controller");
            }

            
            if (string.IsNullOrEmpty( viewModel.CommandForController))
            {
                ModelState.AddModelError("CommantForController", "Комманда должна быть заполнена");
            }
            else if (viewModel.CommandForController.Length > 100)
            {
                ModelState.AddModelError("CommantForController", "Длинна команды должна быть меньше 100 символов");
            }
            

            var modelstate = ModelState.IsValid;
            if (ModelState.IsValid)
            {
                  
                var guid = _commandRepository.UploadCommand(viewModel.CommandForController);
                if (guid != null)
                {
                    TempData["ControllerSettings.Edit.SuccessMessage"] =
                        $"Команда {viewModel.CommandForController} отправлена";
                }
                else
                {
                    TempData["ControllerSettings.Edit.ErrorMessage"] =
                        $"Ошибка при отправке команды";
                }
            }


            return View(viewModel);
            // return RedirectToAction("Edit", "Controller");
        }

    }
}