﻿using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;

namespace TSensor.Web.Models.Services.Security
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;

        public AuthService(IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            _configuration = configuration;
            _httpContext = httpContext;
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


        public static ClaimsPrincipal CreateUserPrincipal(Guid userGuid, string name, string role)
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim("LoginDate", DateTime.Now.ToString()),
                        new Claim("Guid", userGuid.ToString()),
                        new Claim(ClaimsIdentity.DefaultNameClaimType, name),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
                    },
                    "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType));
        }

        public static readonly RoleCollection Roles = new RoleCollection();
    }
}