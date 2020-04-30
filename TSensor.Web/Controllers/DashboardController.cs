using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Repository;
using TSensor.Web.Models.Services;
using TSensor.Web.Models.Services.Security;
using TSensor.Web.ViewModels.Dashboard;

namespace TSensor.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IPointRepository _pointRepository;
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly AuthService _authService;

        public DashboardController(IPointRepository pointRepository, IFavoriteRepository favoriteRepository,
            AuthService authService)
        {
            _pointRepository = pointRepository;
            _favoriteRepository = favoriteRepository;
            _authService = authService;
        }

        private IActionResult ActualSensorValues(IEnumerable<Guid> guidList, Favorite favorite = null, 
            string errorMessage = null)
        {
            //todo check user rights

            var comparer = new AlphanumComparer();

            var viewModel = new ActualSensorValuesViewModel
            {
                ActualSensorValueList = _pointRepository.GetSensorActualState(guidList)
                    .OrderBy(t => t.PointName, comparer).ThenBy(t => t.TankName, comparer),
                Favorite = favorite
            };

            ViewBag.SelectedMenuElements = guidList;
            
            if (errorMessage != null)
            {
                viewModel.ErrorMessage = errorMessage;
            }
            return View("ActualSensorValues", viewModel);
        }

        [Route("")]
        public IActionResult Default()
        {
            return RedirectToAction("Index", "Dashboard");
        }

        [Route("dashboard")]
        public IActionResult Index()
        {
            return View("ActualSensorValues");
        }

        private static IEnumerable<Guid> FilterRequestList(IEnumerable<string> list)
        {
            return (list ?? Enumerable.Empty<string>())
                .Select(p => Guid.TryParse(p, out var tankGuid) ? (Guid?)tankGuid : null)
                .Where(p => p != null)
                .Select(p => p.Value)
                .Distinct();
        }

        [Route("dashboard")]
        [HttpPost]
        public IActionResult Index(IEnumerable<string> guidList)
        {
            return ActualSensorValues(FilterRequestList(guidList));
        }

        [Route("favorite/{favoriteGuid}")]
        public IActionResult Favorite(string favoriteGuid)
        {
            if (Guid.TryParse(favoriteGuid, out var _favoriteGuid))
            {
                var favorite = _favoriteRepository.GetByGuid(_favoriteGuid);
                if (favorite != null)
                {
                    return ActualSensorValues(favorite.ItemList, favorite);
                }
            }

            ViewBag.Title = "Избранное не найдено";

            return View("NotFound");
        }

        [Route("favorite/add")]
        [HttpPost]
        public IActionResult AddFavorite(string name, IEnumerable<string> valueList)
        {
            var guidList = FilterRequestList(valueList);

            name = name?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                name = "Избранные объекты";
            }

            var favoriteGuid = _favoriteRepository.Create(_authService.CurrentUserGuid, name, guidList);
            if (favoriteGuid.HasValue)
            {
                return RedirectToAction("Favorite", "Dashboard", new { favoriteGuid });
            }
            else
            {
                return ActualSensorValues(guidList, errorMessage: "При сохранении избранного произошла ошибка");
            }
        }

        [Route("favorite/remove")]
        [HttpPost]
        public IActionResult RemoveFavorite(string favoriteGuid, IEnumerable<string> valueList)
        {
            var guidList = FilterRequestList(valueList);

            if (Guid.TryParse(favoriteGuid, out var _favoriteGuid))
            {
                var removeResult = _favoriteRepository.Remove(_authService.CurrentUserGuid, _favoriteGuid);
                if (removeResult)
                {
                    return ActualSensorValues(guidList);
                }
            }

            return ActualSensorValues(guidList, errorMessage: "При удалении избранного произошла ошибка");
        }

        [Authorize(Policy = "Admin")]
        [Route("sensor/notassigned")]
        public IActionResult NotAssigned()
        {
            var notAssignedPointInfo = _pointRepository.GetNotAssignedSensorState();

            return View(notAssignedPointInfo);
        }
    }
}
