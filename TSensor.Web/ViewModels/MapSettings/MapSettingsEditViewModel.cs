using System;
using System.ComponentModel.DataAnnotations;

namespace TSensor.Web.ViewModels.MapSettings
{
    public class MapSettingsEditViewModel : ViewModelBase
    {
        [Range(4, 20, ErrorMessage = "Значение должно быть от 4 до 20")]
        public int MaxZoom { get; set; }

        public string PushpinImage { get; set; }
    }
}
