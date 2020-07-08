using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using TSensor.License.Models;
using TSensor.License.ViewModels;

namespace TSensor.License.Controllers
{
    [Authorize]
    public class LicenseController : Controller
    {
        private readonly IRepository _repository;
        private readonly EncodeService _encodeService;

        public LicenseController(IRepository repository, EncodeService encodeService)
        {
            _repository = repository;
            _encodeService = encodeService;
        }

        [Route("license/list")]
        public IActionResult List()
        {
            var viewModel = new SearchViewModel<Models.License>
            {
                Data = _repository.List()
            };

            var successMessage = TempData["License.List.SuccessMessage"] as string;
            if (!string.IsNullOrEmpty(successMessage))
            {
                viewModel.SuccessMessage = successMessage;
            }
            var errorMessage = TempData["License.List.ErrorMessage"] as string;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                viewModel.ErrorMessage = errorMessage;
            }

            return View(viewModel);
        }

        [Route("license/{licenseGuid}")]
        public IActionResult Download(string licenseGuid)
        {
            if (Guid.TryParse(licenseGuid, out var _licenseGuid))
            {
                var license = _repository.GetByGuid(_licenseGuid);
                if (license != null)
                {
                    return File(Encoding.UTF8.GetBytes(license.Data), "text/plain", $"техносенсор-онлайн {license.Name}.lic");
                }
            }

            return Content(string.Empty);
        }

        [Route("license/new")]
        public IActionResult Create()
        {
            var viewModel = new LicenseCreateViewModel
            {
                ExpireDate = DateTime.Now.Date.AddDays(30).ToString("dd.MM.yyyy"),
                SensorCount = "0"
            };

            return View(viewModel);
        }

        [Route("license/new")]
        [HttpPost]
        public IActionResult Create(LicenseCreateViewModel viewModel)
        {
            if (viewModel == null)
            {
                return RedirectToAction("Create", "License");
            }

            viewModel.Name = viewModel.Name?.Trim();

            viewModel.Validate(ModelState);

            if (ModelState.IsValid)
            {
                var license = new Models.License
                {
                    LicenseGuid = Guid.NewGuid(),
                    Name = viewModel.Name,
                    ExpireDate = viewModel.ExpireDateParsed,
                    SensorCount = viewModel.SensorCountParsed
                };
                license.Data = _encodeService.EncodeLicense(license);
                
                var result = _repository.Create(license);
                if (result)
                {
                    TempData["License.List.SuccessMessage"] =
                        $"Лицензия \"{viewModel.Name}\" создана";

                    return RedirectToAction("List", "License");
                }
                else
                {
                    viewModel.ErrorMessage = Program.GLOBAL_ERROR_MESSAGE;
                }
            }

            return View(viewModel);
        }

        [Route("license/remove")]
        [HttpPost]
        public IActionResult Remove(string licenseGuid)
        {
            if (!Guid.TryParse(licenseGuid, out var _licenseGuid))
            {
                TempData["License.List.ErrorMessage"] = "Лицензия не найдена, скорее всего ее уже удалили";
            }
            else
            {
                var license = _repository.GetByGuid(_licenseGuid);
                if (license == null)
                {
                    TempData["License.List.ErrorMessage"] = "Лицензия не найдена, скорее всего ее уже удалили";
                }
                else
                {
                    if (license.IsActivated)
                    {
                        TempData["License.List.ErrorMessage"] = "Активированная лицензия не может быть удалена";
                    }
                    else
                    {
                        if (_repository.Remove(_licenseGuid))
                        {
                            TempData["License.List.SuccessMessage"] = $"Лицензия \"{license.Name}\" удалена";
                        }
                        else
                        {
                            TempData["License.List.ErrorMessage"] = "При удалении группы объектов произошла ошибка";
                        }
                    }
                }
            }

            return RedirectToAction("List", "License");
        }
    }
}