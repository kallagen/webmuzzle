using System;
using System.ComponentModel.DataAnnotations;

namespace TSensor.Web.ViewModels.Point
{
    public class PointCreateEditViewModel : SearchViewModel<Models.Entity.Tank>
    {
        public Guid PointGuid { get; set; }
        [Required(ErrorMessage = "Укажите название")]
        [StringLength(128, ErrorMessage = "Слишком длинное название")]
        public string Name { get; set; }
    }
}