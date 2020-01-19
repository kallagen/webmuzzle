using System.Linq;
using TSensor.Web.Models.Services.Security;

namespace TSensor.Web.ViewModels.User
{
    public class UserSearchViewModel : SearchViewModel<Models.Entity.User>
    {
        public string FilterSearch { get; set; }
        public string FilterRole { get; set; }

        public bool HasInactiveRecord =>
            Data.Any(p => p.IsInactive);

        public string GetRoleName(string role) =>
            AuthService.Roles.GetRoleName(role);        
    }
}