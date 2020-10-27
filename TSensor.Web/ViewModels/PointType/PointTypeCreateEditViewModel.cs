using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace TSensor.Web.ViewModels.PointType
{
    public class PointTypeCreateEditViewModel : ViewModelBase
    {
        public Guid PointTypeGuid { get; set; }
        [Required(ErrorMessage = "Укажите название")]
        [StringLength(128, ErrorMessage = "Слишком длинное название")]
        public string Name { get; set; }
        public IFormFile NewImage { get; set; }
        public string Image { get; set; }
    }
}