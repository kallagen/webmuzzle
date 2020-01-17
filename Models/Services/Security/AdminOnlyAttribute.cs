using Microsoft.AspNetCore.Authorization;

namespace TSensor.Web.Models.Services.Security
{
    public class AdminOnlyAttribute : AuthorizeAttribute
    {
        public AdminOnlyAttribute() : base()
        {
            Roles = RoleCollection.Admin;
        }
    }
}
