using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using TSensor.Web.Models.Security;
using TSensor.Web.ViewModels;

namespace TSensor.Web.Controllers
{
    [Authorize(Policy = "Admin")]
    public class LicenseController : Controller
    {
        private readonly LicenseManager _licenseManager;

        public LicenseController(LicenseManager licenseManager)
        {
            _licenseManager = licenseManager;
        }

        [Route("license")]
        public IActionResult Index()
        {
            var viewModel = new EntityViewModel<License>
            {
                Data = _licenseManager.Current
            };

            var successMessage = TempData["License.Index.SuccessMessage"] as string;
            if (!string.IsNullOrEmpty(successMessage))
            {
                viewModel.SuccessMessage = successMessage;
            }
            var errorMessage = TempData["License.Index.ErrorMessage"] as string;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                viewModel.ErrorMessage = errorMessage;
            }

            return View(viewModel);
        }

        [Route("license/upload")]
        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                var licenseContent = reader.ReadToEnd();

                if (_licenseManager.Activate(licenseContent))
                {
                    TempData["License.Index.SuccessMessage"] = "Лицензия успешно активирована";

                    return RedirectToAction("Index", "License");
                }
            }
            catch{ }

            TempData["License.Index.ErrorMessage"] = "При активации лицензии произошла ошибка, вероятнее всего неправильный файл лицензии";

            return RedirectToAction("Index", "License");
        }
    }
}
