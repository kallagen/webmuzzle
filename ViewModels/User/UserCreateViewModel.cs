using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using TSensor.Web.Models.Services.Security;

namespace TSensor.Web.ViewModels.User
{
    public class UserCreateViewModel : ViewModelBase
    {
        [Required(ErrorMessage = "Укажите логин")]
        [StringLength(32, ErrorMessage = "Слишком длинный логин")]
        public string Login { get; set; }
        [StringLength(64, ErrorMessage = "Слишком длинное имя")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Укажите пароль")]
        [StringLength(32, ErrorMessage = "Слишком длинный пароль")]
        public string Password { get; set; }
        public string Role { get; set; }

        public void Validate(ModelStateDictionary modelState)
        {
            if (!AuthService.Roles.HasRole(Role))
            {
                modelState?.AddModelError("Role", "Укажите группу");
            }
        }
    }
}