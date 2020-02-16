using System;
using System.Collections.Generic;

namespace TSensor.Web.Models.Entity
{
    public class Point
    {
        public Guid PointGuid { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }

        public IEnumerable<Tank> TankList { get; set; }

        public IEnumerable<User> UserList { get; set; }
        public IEnumerable<User> AvailableUserList { get; set; }
    }
}