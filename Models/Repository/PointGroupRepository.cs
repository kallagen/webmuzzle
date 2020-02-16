using System;
using System.Collections.Generic;
using System.Linq;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Services.Security;

namespace TSensor.Web.Models.Repository
{
    public class PointGroupRepository : RepositoryBase, IPointGroupRepository
    {
        public PointGroupRepository(string connectionString) : base(connectionString) { }

        public IEnumerable<PointGroup> List()
        {
            return Query<PointGroup>(@"
                SELECT PointGroupGuid, Name, Description
                FROM PointGroup");
        }

        public PointGroup GetByGuid(Guid pointGroupGuid)
        {
            var group = QueryFirst<PointGroup>(@"
                SELECT PointGroupGuid, Name, Description
                FROM PointGroup
                WHERE PointGroupGuid = @pointGroupGuid", new { pointGroupGuid });

            if (group != null)
            {
                var pointList = Query<dynamic>(@"
                    SELECT DISTINCT p.PointGuid, p.Name, p.[Address], p.Phone, p.Email,
                        p.Description, pgp.PointGroupGuid
                    FROM Point p
                        LEFT JOIN PointGroupPoint pgp ON pgp.PointGuid = p.PointGuid AND
                            pgp.PointGroupGuid = @pointGroupGuid", new { pointGroupGuid });

                group.PointList = pointList.Where(p => p.PointGroupGuid != null)
                    .Select(p => new Point { PointGuid = p.PointGuid, Name = p.Name, 
                        Address = p.Address, Phone = p.Phone, Email = p.Email, Description = p.Description });
                group.AvailablePointList = pointList.Where(p => p.PointGroupGuid == null)
                    .Select(p => new Point { PointGuid = p.PointGuid, Name = p.Name,
                        Address = p.Address, Phone = p.Phone, Email = p.Email, Description = p.Description });

                var userList = Query<dynamic>($@"
                    SELECT u.UserGuid, u.Name, u.Login, u.Description, upgr.PointGroupGuid
                    FROM [User] u 
                        LEFT JOIN UserPointGroupRights upgr ON 
                            upgr.UserGuid = u.UserGuid AND upgr.PointGroupGuid = @pointGroupGuid
                    WHERE u.[Role] = '{RoleCollection.Operator}'",
                    new { pointGroupGuid });

                group.UserList = userList.Where(p => p.PointGroupGuid != null)
                    .Select(p => new User { UserGuid = p.UserGuid, Name = p.Name, Login = p.Login, 
                        Description = p.Description });
                group.AvailableUserList = userList.Where(p => p.PointGroupGuid == null)
                    .Select(p => new User { UserGuid = p.UserGuid, Name = p.Name, Login = p.Login,
                        Description = p.Description });
            }

            return group;
        }

        public Guid? Create(string name, string descirption)
        {
            return QueryFirst<Guid?>(@"
                DECLARE @guid UNIQUEIDENTIFIER = NEWID()

                INSERT [PointGroup](
                    PointGroupGuid, [Name], Description)
                VALUES(
                    @guid, @name, @description)
                
                SELECT PointGroupGuid FROM [PointGroup] WHERE PointGroupGuid = @guid", 
                new { name, descirption });
        }

        public bool Edit(Guid pointGroupGuid, string name, string description)
        {
            return QueryFirst<int?>(@"
                UPDATE [PointGroup] SET 
                    [Name] = @name,
                    Description = @description
                WHERE PointGroupGuid = @pointGroupGuid

                SELECT @@ROWCOUNT", new { pointGroupGuid, name, description }) == 1;
        }

        public bool Remove(Guid pointGroupGuid)
        {
            return QueryFirst<int?>(@"
                DELETE PointGroup 
                WHERE PointGroupGuid = @pointGroupGuid
                    
                SELECT @@ROWCOUNT",
                new { pointGroupGuid }) == 1;
        }

        public bool AddPoint(Guid pointGroupGuid, Guid pointGuid)
        {
            return QueryFirst<int?>(@"
                INSERT PointGroupPoint(PointGroupGuid, PointGuid)
                VALUES(@pointGroupGuid, @pointGuid)

                SELECT @@ROWCOUNT", new { pointGroupGuid, pointGuid }) == 1;
        }

        public bool RemovePoint(Guid pointGroupGuid, Guid pointGuid)
        {
            return QueryFirst<int?>(@"
                DELETE PointGroupPoint
                WHERE PointGroupGuid = @pointGroupGuid AND PointGuid = @pointGuid

                SELECT @@ROWCOUNT", new { pointGroupGuid, pointGuid }) == 1;
        }

        public IEnumerable<PointGroup> GetPointGroupStructure(Guid? userGuid)
        {
            var pointList = Query<dynamic>(@"
                SELECT DISTINCT p.PointGuid, p.Name AS PointName,
                	pg.PointGroupGuid, pg.Name AS PointGroupName
                FROM Point p
                	LEFT JOIN PointGroupPoint pgp ON pgp.PointGuid = p.PointGuid
                	FULL JOIN PointGroup pg ON pg.PointGroupGuid = pgp.PointGroupGuid
                	LEFT JOIN UserPointGroupRights upgr ON upgr.PointGroupGuid = pg.PointGroupGuid
                	LEFT JOIN UserPointRights upr ON upr.PointGuid = p.PointGuid
                WHERE
                	@userGuid IS NULL OR
                	(
                		(pg.PointGroupGuid IS NOT NULL AND upgr.UserGuid = @userGuid) OR
                		(pg.PointGroupGuid IS NULL AND upr.UserGuid = @userGuid)
                	)", new { userGuid });

            var tankList = Query<dynamic>(@"
                SELECT TankGuid, PointGuid, Name 
                FROM Tank
                WHERE PointGuid IN @pointGuidList",
                new { pointGuidList = pointList.Select(p => p.PointGuid).Where(p => p != null).Distinct() });

            return pointList.GroupBy(p => p.PointGroupGuid).Select(g =>
            {
                return new PointGroup
                {
                    PointGroupGuid = g.Key ?? default(Guid),
                    Name = g.FirstOrDefault()?.PointGroupName,
                    PointList = g.Where(p => p.PointGuid != null)
                        .Select(p => new Point
                        {
                            PointGuid = p.PointGuid,
                            Name = p.PointName,
                            TankList = tankList.Where(t => t.PointGuid == p.PointGuid)
                                .Select(t => new Tank { TankGuid = t.TankGuid, Name = t.Name })                            
                        })
                };
            });
        }
    }
}
