using System;
using System.ComponentModel.DataAnnotations;

namespace TSensor.Web.ViewModels.MapSettings
{
    public class MapSettingsEditViewModel : ViewModelBase
    {
        [Required(ErrorMessage = "Укажите значение")]
        [Range(4, 20, ErrorMessage = "Значение должно быть от 4 до 20")]
        public int MaxZoom { get; set; }

        [Required(ErrorMessage = "Укажите значение")]
        [Range(-180, 180, ErrorMessage = "Значение должно быть от -180 до 180")]
        public decimal DefaultLongitude { get; set; }

        [Required(ErrorMessage = "Укажите значение")]
        [Range(-90, 90, ErrorMessage = "Значение должно быть от -90 до 90")]
        public decimal DefaultLatitude { get; set; }
        public string PushpinImage { get; set; }
    }
}
