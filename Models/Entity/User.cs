using System;

namespace TSensor.Web.Models.Entity
{
    public class User : IRemovableEntity
    {
        public Guid UserGuid { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public bool IsRemoved { get; set; }
    }
}
