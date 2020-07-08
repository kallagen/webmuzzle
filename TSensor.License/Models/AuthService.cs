using Microsoft.Extensions.Configuration;
using System;
using System.Security.Claims;

namespace TSensor.License.Models
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool Validate(string login, string password)
        {
            return login == _configuration["Login"] &&
                password == _configuration["Password"];
        }

        public static ClaimsPrincipal CreateUserPrincipal()
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim("LoginDate", DateTime.Now.ToString()),
                        new Claim(ClaimsIdentity.DefaultNameClaimType, "ADMIN"),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, "ADMIN")
                    },
                    "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType));
        }
    }
}