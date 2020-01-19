using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using TSensor.Web.Models.Services.Security;

namespace TSensor.Web.ViewModels.User
{
    public class UserEditViewModel : ViewModelBase
    {
        public Guid UserGuid { get; set; }
        public string Login { get; set; }
        [StringLength(64, ErrorMessage = "Слишком длинное имя")]
        public string Name { get; set; }
        public bool SetNewPassword { get; set; }
        [StringLength(32, ErrorMessage = "Слишком длинный пароль")]
        public string NewPassword { get; set; }
        public string Role { get; set; }
        public bool IsInactive { get; set; }

        public void Validate(ModelStateDictionary modelState)
        {
            if (SetNewPassword && string.IsNullOrWhiteSpace(NewPassword))
            {
                modelState?.AddModelError("NewPassword", "Укажите новый пароль");
            }
            if (!AuthService.Roles.HasRole(Role))
            {
                modelState?.AddModelError("Role", "Укажите группу");
            }
        }
    }
}