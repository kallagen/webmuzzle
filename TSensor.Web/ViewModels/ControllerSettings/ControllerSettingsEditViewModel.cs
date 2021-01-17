using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TSensor.Web.ViewModels.ControllerSettings
{
    public class ControllerSettingsEditViewModel : ViewModelBase
    {
        [Required(ErrorMessage = "Заполните команду")]
        [StringLength(100)]
        [Display(Name = "Команда для контролера")]
        public string CommandForController { get; set; }
        
        [Required(ErrorMessage = "Заполните DeviceGuid контроллера")]
        [MinLength(5, ErrorMessage = "Минимальная длинна 5. Введите DeviceGuid (содержится в конфиге каждого контроллера)")]
        [Display(Name = "DeviceGuid контроллера")]
        public string DeviceGuid { get; set; }
        
        public int IzkNumber { get; set; }

    }
}
