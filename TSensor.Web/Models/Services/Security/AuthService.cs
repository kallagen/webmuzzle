using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace TSensor.Web.Models.Services.Security
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AuthService(IConfiguration configuration, IHttpContextAccessor httpContext,
            IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _httpContext = httpContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public string EncryptPassword(string password)
        {
            return Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password,
                    Convert.FromBase64String(_configuration["PasswordSalt"] ?? string.Empty),
                    KeyDerivationPrf.HMACSHA1,
                    iterationCount: 4096,
                    numBytesRequested: 32));
        }

        public Guid CurrentUserGuid =>
            Guid.Parse(_httpContext.HttpContext.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value);

        public bool IsCurrentUserEmbedded =>
            _httpContext.HttpContext.User.Claims.FirstOrDefault(p => p.Type == "IsEmbedded")?.Value == "true";

        public static ClaimsPrincipal CreateUserPrincipal(Guid userGuid, string name, string role, bool isEmbedded = false)
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim("LoginDate", DateTime.Now.ToString()),
                        new Claim("Guid", userGuid.ToString()),
                        new Claim("IsEmbedded", isEmbedded.ToString()),
                        new Claim(ClaimsIdentity.DefaultNameClaimType, name),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
                    },
                    "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType));
        }

        public static readonly RoleCollection Roles = new RoleCollection();

        private string version = null;
        public string Version
        {
            get
            {
                if (version == null)
                {
                    version = File.ReadAllText(
                        Path.Combine(_webHostEnvironment.ContentRootPath, "version.current"));
                }

                return version;
            }
        }
    }
}