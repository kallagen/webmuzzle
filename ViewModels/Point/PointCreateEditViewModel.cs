using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TSensor.Web.ViewModels.Point
{
    public class PointCreateEditViewModel : SearchViewModel<Models.Entity.Tank>
    {
        public Guid PointGuid { get; set; }
        [Required(ErrorMessage = "Укажите название")]
        [StringLength(128, ErrorMessage = "Слишком длинное название")]
        public string Name { get; set; }
        [StringLength(128, ErrorMessage = "Слишком длинный адрес")]
        public string Address { get; set; }
        [StringLength(128, ErrorMessage = "Слишком длинный телефон")]
        public string Phone { get; set; }
        [StringLength(128, ErrorMessage = "Слишком длинный email")]
        public string Email { get; set; }
        public string Description { get; set; }

        public IEnumerable<Models.Entity.User> UserList { get; set; }
        public bool HasUser =>
            UserList?.Any() == true;

        public IEnumerable<Models.Entity.User> AvailableUserList { get; set; }
        public bool HasAvailableUser =>
            AvailableUserList?.Any() == true;
    }
}