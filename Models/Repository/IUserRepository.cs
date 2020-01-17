using TSensor.Web.Models.Entity;
using System;
using System.Collections.Generic;

namespace TSensor.Web.Models.Repository
{
    public interface IUserRepository
    {
        public User Auth(string login, string password);
        public IEnumerable<User> Search(string search, string roleSysName, bool showRemoved);
        public User GetUserByLogin(string login);
        public Guid? Create(string login, string name, string password, string role);
        public User GetUserByGuid(Guid userGuid);
        public bool Edit(Guid userGuid, string name, string role, bool isRemoved);
        public bool ChangePassword(Guid userGuid, string password);
    }
}
