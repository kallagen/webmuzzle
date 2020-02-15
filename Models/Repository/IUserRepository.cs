using TSensor.Web.Models.Entity;
using System;
using System.Collections.Generic;

namespace TSensor.Web.Models.Repository
{
    public interface IUserRepository
    {
        public User Auth(string login, string password);
        public IEnumerable<User> Search(string search, string roleSysName);
        public User GetByLogin(string login);
        public Guid? Create(string login, string name, string password, string role, string description);
        public User GetByGuid(Guid userGuid);
        public bool Edit(Guid userGuid, string name, string role, bool isInactive, string description);
        public bool ChangePassword(Guid userGuid, string password);
        public bool Remove(Guid userGuid);

        public bool AddPointUser(Guid pointGuid, Guid userGuid);
        public bool RemovePointUser(Guid pointGuid, Guid userGuid);

        public bool AddPointGroupUser(Guid pointGroupGuid, Guid userGuid);
        public bool RemovePointGroupUser(Guid pointGroupGuid, Guid userGuid);
    }
}
