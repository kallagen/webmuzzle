using System;
using System.ComponentModel.DataAnnotations;

namespace TSensor.Web.ViewModels.Tank
{
    public class TankCreateEditViewModel : ViewModelBase
    {
        public Guid? TankGuid { get; set; }
        public Guid? PointGuid { get; set; }
        public string PointName { get; set; }
        [Required(ErrorMessage = "Укажите название")]
        [StringLength(128, ErrorMessage = "Слишком длинное название")]
        public string Name { get; set; }
        public bool DualMode { get; set; }
        [StringLength(5, ErrorMessage = "Значение должно быть не больше 5 символов")]
        public string MainDeviceGuid { get; set; }
        [StringLength(2, ErrorMessage = "Значение должно быть не больше 2 символов")]
        public string MainIZKId { get; set; }
        [StringLength(2, ErrorMessage = "Значение должно быть не больше 2 символов")]
        public string MainSensorId { get; set; }
        [StringLength(5, ErrorMessage = "Значение должно быть не больше 5 символов")]
        public string SecondDeviceGuid { get; set; }
        [StringLength(2, ErrorMessage = "Значение должно быть не больше 2 символов")]
        public string SecondIZKId { get; set; }
        [StringLength(2, ErrorMessage = "Значение должно быть не больше 2 символов")]
        public string SecondSensorId { get; set; }
    }
}
