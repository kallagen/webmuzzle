using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TSensor.Web.Models.Services.Security;

namespace TSensor.Web.ViewModels.User
{
    public class UserCreateViewModel : ViewModelBase
    {
        [Required(ErrorMessage = "Укажите логин")]
        [StringLength(32, ErrorMessage = "Слишком длинный логин")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Укажите имя")]
        [StringLength(32, ErrorMessage = "Слишком длинное имя")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Укажите фамилию")]
        [StringLength(32, ErrorMessage = "Слишком длинная фамилия")]
        public string LastName { get; set; }
        [StringLength(32, ErrorMessage = "Слишком длинное отчество")]
        public string Patronymic { get; set; }
        [Required(ErrorMessage = "Укажите пароль")]
        [StringLength(32, ErrorMessage = "Слишком длинный пароль")]
        public string Password { get; set; }
        public string Role { get; set; }
        public string Description { get; set; }

        public string Name =>
            string.Join(" ", new[] { FirstName, LastName, Patronymic }.Where(p => !string.IsNullOrWhiteSpace(p)));

        public void Validate(ModelStateDictionary modelState)
        {
            if (!AuthService.Roles.HasRole(Role))
            {
                modelState?.AddModelError("Role", "Укажите группу");
            }
        }
    }
}