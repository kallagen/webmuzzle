using System;
using System.Linq;

namespace TSensor.Web.Models.Entity
{
    public class User
    {
        public Guid UserGuid { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Role { get; set; }
        public bool IsInactive { get; set; }
        public string Description { get; set; }

        public string Name =>
            string.Join(" ", new[] { LastName, FirstName, Patronymic }.Where(p => !string.IsNullOrWhiteSpace(p)));

        public static User From(dynamic entity)
        {
            return new User
            {
                UserGuid = entity.UserGuid,
                Login = entity.Login,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Patronymic = entity.Patronymic,                
                Description = entity.Description
            };
        }
    }
}
