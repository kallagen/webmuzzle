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
                SELECT UserGuid, [Login], [Name], Role, IsInactive, Description
                FROM [User]
                WHERE 
                    (@search IS NULL OR [Login] LIKE @search OR [Name] LIKE @search) AND
                    (@role IS NULL OR Role = @role)
                ORDER BY [Login]",
                new { search, role });
        }

        public User GetByLogin(string login)
        {
            return QueryFirst<User>(@"
                SELECT TOP 1 UserGuid 
                FROM [User] 
                WHERE [Login] = @login",
                new { login });
        }

        public Guid? Create(string login, string name, string password, string role, string description)
        {
            return QueryFirst<Guid?>(@"
                DECLARE @guid UNIQUEIDENTIFIER = NEWID()

                INSERT [User](UserGuid, [Login], [Name], Password, Role, Description)
                VALUES(@guid, @login, @name, @password, @role, @description)

                SELECT UserGuid FROM [User] WHERE UserGuid = @guid",
                new { name, login, password, role, description });
        }

        public User GetByGuid(Guid userGuid)
        {
            return QueryFirst<User>(@"
                SELECT UserGuid, [Login], [Name], Role, IsInactive, Description
                FROM [User] WHERE UserGuid = @userGuid", new { userGuid });
        }

        public bool Edit(Guid userGuid, string name, string role, bool isInactive, string description)
        {
            return QueryFirst<int?>(@"
                UPDATE [User] SET 
                    [Name] = @name,
                    Role = @role,
                    IsInactive = @isInactive,
                    Description = @description
                WHERE UserGuid = @userGuid

                SELECT @@ROWCOUNT",
                new { userGuid, name, role, isInactive, description }) == 1;
        }

        public bool ChangePassword(Guid userGuid, string password)
        {
            return QueryFirst<int?>(@"
                UPDATE [User] SET Password = @password WHERE UserGuid = @userGuid

                SELECT @@ROWCOUNT",
                new { userGuid, password }) == 1;
        }

        public bool Remove(Guid userGuid)
        {
            return QueryFirst<int?>(@"
                DELETE [User] 
                WHERE UserGuid = @userGuid
                    
                SELECT @@ROWCOUNT",
                new { userGuid }) == 1;
        }

        public bool AddPointUser(Guid pointGuid, Guid userGuid)
        {
            return QueryFirst<int?>(@"
                IF NOT EXISTS (SELECT 1
                    FROM UserPointRights
                    WHERE PointGuid = @pointGuid AND UserGuid = @userGuid)

                    INSERT UserPointRights(PointGuid, UserGuid)
                    VALUES(@pointGuid, @userGuid)

                SELECT @@ROWCOUNT", new { pointGuid, userGuid }) == 1;
        }

        public bool RemovePointUser(Guid pointGuid, Guid userGuid)
        {
            return QueryFirst<int?>(@"
                DELETE UserPointRights
                WHERE PointGuid = @pointGuid AND UserGuid = @userGuid

                SELECT @@ROWCOUNT",
                new { pointGuid, userGuid }) == 1;
        }

        public bool AddPointGroupUser(Guid pointGroupGuid, Guid userGuid)
        {
            return QueryFirst<int?>(@"
                IF NOT EXISTS (SELECT 1
                    FROM UserPointGroupRights
                    WHERE PointGroupGuid = @pointGroupGuid AND UserGuid = @userGuid)

                    INSERT UserPointGroupRights(PointGroupGuid, UserGuid)
                    VALUES(@pointGroupGuid, @userGuid)

                SELECT @@ROWCOUNT", new { pointGroupGuid, userGuid }) == 1;
        }

        public bool RemovePointGroupUser(Guid pointGroupGuid, Guid userGuid)
        {
            return QueryFirst<int?>(@"
                DELETE UserPointGroupRights
                WHERE PointGroupGuid = @pointGroupGuid AND UserGuid = @userGuid

                SELECT @@ROWCOUNT",
                new { pointGroupGuid, userGuid }) == 1;
        }
    }
}
