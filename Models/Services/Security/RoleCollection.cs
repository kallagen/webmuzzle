using System.Collections;
using System.Collections.Generic;

namespace TSensor.Web.Models.Services.Security
{
    public class RoleCollection : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> roles = new Dictionary<string, string>
        {
            { Admin, "Администратор" },
            { Operator, "Оператор" }
        };

        public const string Admin = "admin";
        public const string Operator = "operator";

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return roles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return roles.GetEnumerator();
        }

        public bool HasRole(string role)
            => role != null && roles.ContainsKey(role);
        public string GetRoleName(string role)
            => HasRole(role) ? roles[role] : null;
    }
}
