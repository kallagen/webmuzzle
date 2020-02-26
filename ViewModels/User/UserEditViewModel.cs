using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TSensor.Web.Models.Services.Security;

namespace TSensor.Web.ViewModels.User
{
    public class UserEditViewModel : ViewModelBase
    {
        public Guid UserGuid { get; set; }
        public string Login { get; set; }
        [Required(ErrorMessage = "Укажите имя")]
        [StringLength(32, ErrorMessage = "Слишком длинное имя")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Укажите фамилию")]
        [StringLength(32, ErrorMessage = "Слишком длинная фамилия")]
        public string LastName { get; set; }
        [StringLength(32, ErrorMessage = "Слишком длинное отчество")]
        public string Patronymic { get; set; }
        public bool SetNewPassword { get; set; }
        [StringLength(32, ErrorMessage = "Слишком длинный пароль")]
        public string NewPassword { get; set; }
        [StringLength(32, ErrorMessage = "Слишком длинный пароль")]
        public string NewPasswordConfirm { get; set; }
        public string Role { get; set; }
        public string Description { get; set; }
        public bool IsInactive { get; set; }

        public string Name =>
            string.Join(" ", new[] { FirstName, LastName, Patronymic }.Where(p => !string.IsNullOrWhiteSpace(p)));

        public void Validate(ModelStateDictionary modelState)
        {
            if (!AuthService.Roles.HasRole(Role))
            {
                modelState?.AddModelError("Role", "Укажите группу");
            }
            if (SetNewPassword && string.IsNullOrWhiteSpace(NewPassword))
            {
                modelState?.AddModelError("NewPassword", "Укажите новый пароль");
            }
            if (SetNewPassword && string.IsNullOrWhiteSpace(NewPasswordConfirm))
            {
                modelState?.AddModelError("NewPasswordConfirm", "Укажите новый пароль еще раз");
            }
            if (SetNewPassword && NewPassword != NewPasswordConfirm)
            {
                modelState?.AddModelError("NewPasswordConfirm", "Пароль и подтверждение не совпадают");
            }
        }
    }
}