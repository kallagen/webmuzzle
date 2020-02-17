using Microsoft.AspNetCore.Mvc;
using TSensor.Web.Models.Repository;
using TSensor.Web.Models.Services.Security;

namespace TSensor.Web.ViewComponents
{
    public class FavoriteMenu : ViewComponent
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly AuthService _authService;

        public FavoriteMenu(IFavoriteRepository favoriteRepository, AuthService authService)
        {
            _favoriteRepository = favoriteRepository;
            _authService = authService;
        }

        public IViewComponentResult Invoke()
        {
            var favoriteList = _favoriteRepository.ListByUser(_authService.CurrentUserGuid);

            return View(favoriteList);
        }
    }
}
