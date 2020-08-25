using System;
using System.ComponentModel.DataAnnotations;

namespace TSensor.Web.ViewModels.Product
{
    public class ProductCreateEditViewModel : ViewModelBase
    {
        public Guid ProductGuid { get; set; }
        [Required(ErrorMessage = "Укажите название")]
        [StringLength(32, ErrorMessage = "Слишком длинное название")]
        public string Name { get; set; }

        public bool IsGas { get; set; }
    }
}