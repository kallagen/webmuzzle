using System;
using System.ComponentModel.DataAnnotations;

namespace TSensor.Web.ViewModels.PointGroup
{
    public class PointGroupCreateEditViewModel : ViewModelBase
    {
        public Guid PointGroupGuid { get; set; }
        [Required(ErrorMessage = "Укажите название")]
        [StringLength(128, ErrorMessage = "Слишком длинное название")]
        public string Name { get; set; }
    }
}