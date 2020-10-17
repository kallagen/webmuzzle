using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using TSensor.Web.Models.Repository;
using TSensor.Web.ViewModels.MapSettings;

namespace TSensor.Web.Controllers
{
    public class MapSettingsController : Controller
    {
        private readonly IMapSettingsRepository _repository;

        public MapSettingsController(IMapSettingsRepository repository)
        {
            _repository = repository;
        }

        [Authorize(Policy = "Admin")]
        [Route("settings/map")]
        public IActionResult Edit()
        {
            var entity = _repository.GetSettings();

            var viewModel = new MapSettingsEditViewModel
            {
                MaxZoom = entity.MaxZoom,
                PushpinImage = entity.PushpinImage
            };

            var successMessage = TempData["MapSettings.Edit.SuccessMessage"] as string;
            if (!string.IsNullOrEmpty(successMessage))
            {
                viewModel.SuccessMessage = successMessage;
            }
            var errorMessage = TempData["MapSettings.Edit.ErrorMessage"] as string;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                viewModel.ErrorMessage = errorMessage;
            }

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("settings/map")]
        [HttpPost]
        public IActionResult Edit(MapSettingsEditViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("Edit", "MapSettings");
            }

            if (ModelState.IsValid)
            {
                var result = _repository.SaveSettings(viewModel.MaxZoom);
                if (result)
                {
                    TempData["MapSettings.Edit.SuccessMessage"] =
                        $"Настройки карты успешно изменены";

                    return RedirectToAction("Edit", "MapSettings");
                }
                else
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
            }

            viewModel.PushpinImage = _repository.GetSettings()?.PushpinImage;

            return View(viewModel);
        }

        [Authorize(Policy = "Admin")]
        [Route("settings/map/pushpin/upload")]
        [HttpPost]
        public IActionResult UploadPushpinImage(IFormFile file)
        {
            if (file != null)
            {
                try
                {
                    using var memoryStream = new MemoryStream();
                    file.CopyTo(memoryStream);

                    var pushpinData = Convert.ToBase64String(memoryStream.ToArray());
                    if (_repository.UploadPushpinImage(
                        $"data:image/png;base64,{pushpinData}"))
                    {
                        TempData["MapSettings.Edit.SuccessMessage"] =
                            "Иконка объекта успешно загружена";
                    }
                    else 
                    {
                        TempData["MapSettings.Edit.ErrorMessage"] =
                            "При загрузки иконки объекта произошла ошибка";
                    }                    
                }
                catch
                {
                    TempData["MapSettings.Edit.ErrorMessage"] =
                        "При загрузки иконки объекта произошла ошибка";
                }
            }
            else
            {
                TempData["MapSettings.Edit.ErrorMessage"] =
                    "Выберите иконку для объекта";
            }

            return RedirectToAction("Edit", "MapSettings");
        }
    }
}
