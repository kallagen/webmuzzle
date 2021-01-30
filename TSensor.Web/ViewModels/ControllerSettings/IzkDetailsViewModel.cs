using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.ViewModels.ControllerSettings
{
    public class IzkDetailsViewModel : ViewModelBase
    {
        [Required(ErrorMessage = "Заполните команду")]
        [StringLength(100)]
        [Display(Name = "Команда для контролера")]
        public string CommandForController { get; set; }
        
        [Required(ErrorMessage = "Заполните DeviceGuid контроллера")]
        [MinLength(5, ErrorMessage = "Минимальная длинна 5. Введите DeviceGuid (содержится в конфиге каждого контроллера)")]
        [Display(Name = "DeviceGuid контроллера")]
        public string DeviceGuid { get; set; }
        public int? IzkNumber { get; set; }
        
        
        public bool IsMassmeter { get; set; }
        
        public ActualSensorValue Value { get; set; }
        
        public IEnumerable<int> SensorChannelList { get; set; }

    }
}
