using System.ComponentModel.DataAnnotations;

namespace TSensor.Web.ViewModels.ControllerSettings
{
    public class ControllerSettingsEditViewModel : ViewModelBase
    {
        [Required(ErrorMessage = "Заполните команду")]
        [StringLength(100)]
        [Display(Name = "Команда для контролера")]
        public string CommandForController { get; set; }
    }
}