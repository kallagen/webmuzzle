using System;
using System.Collections.Generic;

namespace TSensor.Web.Models.Entity
{
    public class PointGroup
    {
        public Guid PointGroupGuid { get; set; }
        public string Name { get; set; }

        public IEnumerable<Point> PointList { get; set; }
        public IEnumerable<Point> AvailablePointList { get; set; }
        
        public IEnumerable<User> UserList { get; set; }
        public IEnumerable<User> AvailableUserList { get; set; }            
    }
}
