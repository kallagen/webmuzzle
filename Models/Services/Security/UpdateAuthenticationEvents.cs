using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TSensor.Web.Models.Services.Security
{
    public class UpdateAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly IMemoryCache _memoryCache;

        public UpdateAuthenticationEvents(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public override Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var claims = context?.Principal.Claims;
            if (claims != null && Guid.TryParse(claims.FirstOrDefault(p => p.Type == "Guid").Value, out var userGuid))
            {
                var currentUserLoginDate = DateTime.Parse(claims.FirstOrDefault(p => p.Type == "LoginDate").Value);
                var userLastUpdateDate = _memoryCache.Get<DateTime?>($"UserLastUpdate{userGuid}");
                if (userLastUpdateDate != null && currentUserLoginDate < userLastUpdateDate)
                {
                    if (_memoryCache.Get<bool?>($"UserReject{userGuid}") == true)
                    {
                        context.RejectPrincipal();
                    }
                    else
                    {
                        var newName = _memoryCache.Get<string>($"UserName{userGuid}");
                        var newRole = _memoryCache.Get<string>($"UserRole{userGuid}");

                        context.ReplacePrincipal(AuthService.CreateUserPrincipal(userGuid, newName, newRole));
                        context.ShouldRenew = true;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
