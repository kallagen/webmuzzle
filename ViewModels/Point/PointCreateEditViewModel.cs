using System;
using System.ComponentModel.DataAnnotations;

namespace TSensor.Web.ViewModels.Point
{
    public class PointCreateEditViewModel : ViewModelBase
    {
        public Guid PointGuid { get; set; }
        [Required(ErrorMessage = "Укажите название")]
        [StringLength(128, ErrorMessage = "Слишком длинное название")]
        public string Name { get; set; }
    }
}