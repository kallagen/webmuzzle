using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Repository;
using TSensor.Web.Models.Security;
using TSensor.Web.Models.Services;
using TSensor.Web.Models.Services.Security;
using TSensor.Web.ViewModels;
using TSensor.Web.ViewModels.ControllerSettings;
using TSensor.Web.ViewModels.Dashboard;


namespace TSensor.Web.Controllers
{
    [Route("Izk")]
    public class IzkController: Controller
    {
        private readonly AuthService _authService;
        private readonly LicenseManager _licenseManager;
        private readonly IPointRepository _pointRepository;
        private readonly ITankRepository _tankRepository;
        public IzkController(AuthService authService, LicenseManager licenseManager, IPointRepository  pointRepository, ITankRepository tankRepository)
        {
            _authService = authService;
            _licenseManager = licenseManager;
            _tankRepository = tankRepository;
            _pointRepository = pointRepository;
        }
        
        [Authorize(Policy = "Admin")]
        [Route("controller/list")]
        public IActionResult List()
        {
            var pointList = _pointRepository.List();
            var tanks = new HashSet<Tank>();
            foreach (var point in pointList)
            {
                var tanksFromOnePoint = _tankRepository.GetListByPoint(point.PointGuid);
                tanks.UnionWith(tanksFromOnePoint);
            }
            
            var viewModel = new SearchViewModel<Tank>
            {
                Data = tanks
            };

            var successMessage = TempData["Point.List.SuccessMessage"] as string;
            if (!string.IsNullOrEmpty(successMessage))
            {
                viewModel.SuccessMessage = successMessage;
            }
            var errorMessage = TempData["Point.List.ErrorMessage"] as string;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                viewModel.ErrorMessage = errorMessage;
            }

            return View(viewModel);
        }
        
        
        //Создается по вью модели на каждый Izk из списка и вызывается ендпоинт с соответствующей вью моделью
        [Authorize(Policy = "Admin")]
        [Route("izk/{mainDeviceGuid}/{mainIZKId}")]
        public IActionResult Edit(string mainDeviceGuid, int? mainIZKId)
        {
           
            var viewModel = new IzkDetailsViewModel(){DeviceGuid = mainDeviceGuid, IzkNumber = mainIZKId};
            ViewBag.DeviceGuid = mainDeviceGuid;
            ViewBag.IzkNumber = mainIZKId ?? -1;
            return View(viewModel);
             
            

            ViewBag.Title = "Объект не найден";
            ViewBag.BackTitle = "назад к списку объектов";
            ViewBag.BackUrl = Url.ActionLink("List", "Point");

            return View("NotFound");
        }

        // public IActionResult SelectController(IEnumerable<Guid> guidList, Favorite favorite = null,
        //     string errorMessage = null)
        // {
        //     if (!_licenseManager.IsValid(out var reason))
        //     {
        //         return LicenseExpired(reason);
        //     }
        //
        //     var comparer = new AlphanumComparer();
        //
        //     var viewModel = new ActualSensorValuesViewModel
        //     {
        //         ActualSensorValueList = _pointRepository.GetSensorActualState(guidList)
        //             .OrderBy(t => t.PointName, comparer)
        //             .ThenBy(t => t.TankName, comparer),
        //         Favorite = favorite
        //     };
        //
        //     ViewBag.SelectedMenuElements = guidList;
        //
        //     if (errorMessage != null)
        //     {
        //         viewModel.ErrorMessage = errorMessage;
        //     }
        //     return View("SelectController", viewModel);
        // }

        // private IActionResult LicenseExpired(InvalidLicenseReason reason)
        // {
        //     switch (reason)
        //     {
        //         case InvalidLicenseReason.NotFound:
        //             ViewBag.Title = "Лицензия отсутствует";
        //             break;
        //         case InvalidLicenseReason.Corrupted:
        //             ViewBag.Title = "Лицензия неправильная или повреждена";
        //             break;
        //         case InvalidLicenseReason.Expired:
        //             ViewBag.Title = "Срок действия лицензии истек";
        //             break;
        //         case InvalidLicenseReason.MaxSensorCount:
        //             ViewBag.Title = "Количество резервуаров больше, чем позволяет использовать текущая лицензия";
        //             break;
        //         default: break;
        //     }
        //
        //     return View("LicenseExpired");
        // }
    }
}