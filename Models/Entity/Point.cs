using System;
using System.Collections.Generic;

namespace TSensor.Web.Models.Entity
{
    public class Point
    {
        public Guid PointGuid { get; set; }
        public string Name { get; set; }

        public IEnumerable<User> UserList { get; set; }
        public IEnumerable<User> AvailableUserList { get; set; }
    }
}