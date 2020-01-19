using TSensor.Web.Models.Entity;
using System;
using System.Collections.Generic;

namespace TSensor.Web.Models.Repository
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public UserRepository(string connectionString) : base(connectionString) { }

        public User Auth(string login, string password)
        {
            return QueryFirst<User>(@"
                SELECT TOP 1 UserGuid, Login, Name, Role
                FROM [User] 
                WHERE [Login] = @login AND [Password] = @password AND IsInactive = 0",
                new { login, password });
        }

        public IEnumerable<User> Search(string search, string role)
        {
            search = string.IsNullOrWhiteSpace(search) ? null : $"%{search}%";

            return Query<User>(@"
                SELECT UserGuid, [Login], [Name], Role, IsInactive
                FROM [User]
                WHERE 
                    (@search IS NULL OR [Login] LIKE @search OR [Name] LIKE @search) AND
                    (@role IS NULL OR Role = @role)
                ORDER BY [Login]",
                new { search, role });
        }

        public User GetUserByLogin(string login)
        {
            return QueryFirst<User>(@"
                SELECT TOP 1 UserGuid 
                FROM [User] 
                WHERE [Login] = @login",
                new { login });
        }

        public Guid? Create(string login, string name, string password, string role)
        {
            return QueryFirst<Guid?>(@"
                DECLARE @guid UNIQUEIDENTIFIER = NEWID()

                INSERT [User](UserGuid, [Login], [Name], Password, Role)
                VALUES(@guid, @login, @name, @password, @role)

                SELECT UserGuid FROM [User] WHERE UserGuid = @guid",
                new { name, login, password, role });
        }

        public User GetUserByGuid(Guid userGuid)
        {
            return QueryFirst<User>(@"
                SELECT UserGuid, [Login], [Name], Role, IsInactive
                FROM [User] WHERE UserGuid = @userGuid", new { userGuid });
        }

        public bool Edit(Guid userGuid, string name, string role, bool isInactive)
        {
            return QueryFirst<int?>(@"
                UPDATE [User] SET 
                    [Name] = @name,
                    Role = @role,
                    IsInactive = @isInactive
                WHERE UserGuid = @userGuid

                SELECT @@ROWCOUNT",
                new { userGuid, name, role, isInactive }) == 1;
        }

        public bool ChangePassword(Guid userGuid, string password)
        {
            return QueryFirst<int?>(@"
                UPDATE [User] SET Password = @password WHERE UserGuid = @userGuid

                SELECT @@ROWCOUNT",
                new { userGuid, password }) == 1;
        }
    }
}
