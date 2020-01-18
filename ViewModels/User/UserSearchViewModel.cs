using TSensor.Web.Models.Services.Security;

namespace TSensor.Web.ViewModels.User
{
    public class UserSearchViewModel : SearchRemovableViewModel<Models.Entity.User>
    {
        public string FilterSearch { get; set; }
        public string FilterRole { get; set; }

        public string GetRoleName(string role) =>
            AuthService.Roles.GetRoleName(role);        
    }
}