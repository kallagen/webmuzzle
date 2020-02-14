using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TSensor.Web.ViewModels.PointGroup
{
    public class PointGroupCreateEditViewModel : SearchViewModel<Models.Entity.Point>
    {
        public Guid PointGroupGuid { get; set; }
        [Required(ErrorMessage = "Укажите название")]
        [StringLength(128, ErrorMessage = "Слишком длинное название")]
        public string Name { get; set; }

        public IEnumerable<Models.Entity.Point> AvailablePointList { get; set; }
        public bool HasAvailablePoint =>
            AvailablePointList?.Any() == true;

        public IEnumerable<Models.Entity.User> UserList { get; set; }
        public bool HasUser =>
            UserList?.Any() == true;
        public IEnumerable<Models.Entity.User> AvailableUserList { get; set; }
        public bool HasAvailableUser =>
            AvailableUserList?.Any() == true;
    }
}