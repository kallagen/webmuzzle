using System;
using System.Collections.Generic;

namespace TSensor.Web.Models.Entity
{
    public class PointGroup
    {
        public static readonly Guid RootGuid = new Guid("00000000-0000-0000-0000-000000000000");
        public Guid PointGroupGuid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public IEnumerable<Point> PointList { get; set; }
        public IEnumerable<Point> AvailablePointList { get; set; }
        
        public IEnumerable<User> UserList { get; set; }
        public IEnumerable<User> AvailableUserList { get; set; }
    }
}
