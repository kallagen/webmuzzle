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

        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }

        public List<Tank> TankList { get; set; } = new List<Tank>();

        public IEnumerable<User> UserList { get; set; }
        public IEnumerable<User> AvailableUserList { get; set; }

        public static Point From(dynamic entity)
        {
            return new Point
            {
                PointGuid = entity.PointGuid,
                Name = entity.Name,
                Address = entity.Address,
                Phone = entity.Phone,
                Email = entity.Email,
                Description = entity.Description
            };
        }
    }
}