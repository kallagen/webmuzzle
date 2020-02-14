﻿using System;
using System.Collections.Generic;
using System.Linq;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Services.Security;

namespace TSensor.Web.Models.Repository
{
    public class PointGroupRepository : RepositoryBase, IPointGroupRepository
    {
        public PointGroupRepository(string connectionString) : base(connectionString) { }

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

        public Guid? Create(string name)
        {
            return QueryFirst<Guid?>(@"
                DECLARE @guid UNIQUEIDENTIFIER = NEWID()

                INSERT [PointGroup](
                    PointGroupGuid, [Name])
                VALUES(
                    @guid, @name)
                
                SELECT PointGroupGuid FROM [PointGroup] WHERE PointGroupGuid = @guid", new { name });
        }

        public bool Edit(Guid pointGroupGuid, string name)
        {
            return QueryFirst<int?>(@"
                UPDATE [PointGroup] SET 
                    [Name] = @name
                WHERE PointGroupGuid = @pointGroupGuid

                SELECT @@ROWCOUNT", new { pointGroupGuid, name }) == 1;
        }

        public PointGroup GetByGuid(Guid pointGroupGuid)
        {
            var group = QueryFirst<PointGroup>(@"
                SELECT PointGroupGuid, Name
                FROM PointGroup
                WHERE PointGroupGuid = @pointGroupGuid", new { pointGroupGuid });

            if (group != null)
            {
                var pointList = Query<dynamic>(@"
                    SELECT DISTINCT p.PointGuid, p.Name, pgp.PointGroupGuid
                    FROM Point p
                        LEFT JOIN PointGroupPoint pgp ON pgp.PointGuid = p.PointGuid AND
                            pgp.PointGroupGuid = @pointGroupGuid", new { pointGroupGuid });

                group.PointList = pointList.Where(p => p.PointGroupGuid != null)
                    .Select(p => new Point { PointGuid = p.PointGuid, Name = p.Name });
                group.AvailablePointList = pointList.Where(p => p.PointGroupGuid == null)
                    .Select(p => new Point { PointGuid = p.PointGuid, Name = p.Name });

                var userList = Query<dynamic>($@"
                    SELECT u.UserGuid, u.Name, u.Login, upgr.PointGroupGuid
                    FROM [User] u 
                        LEFT JOIN UserPointGroupRights upgr ON 
                            upgr.UserGuid = u.UserGuid AND upgr.PointGroupGuid = @pointGroupGuid
                    WHERE u.[Role] = '{RoleCollection.Operator}'",
                    new { pointGroupGuid });

                group.UserList = userList.Where(p => p.PointGroupGuid != null)
                    .Select(p => new User { UserGuid = p.UserGuid, Name = p.Name, Login = p.Login });
                group.AvailableUserList = userList.Where(p => p.PointGroupGuid == null)
                    .Select(p => new User { UserGuid = p.UserGuid, Name = p.Name, Login = p.Login });
            }

            return group;
        }

        public IEnumerable<PointGroup> List()
        {
            return Query<PointGroup>(@"
                SELECT PointGroupGuid, Name
                FROM PointGroup");
        }

        public bool Remove(Guid pointGroupGuid)
        {
            return QueryFirst<int?>(@"
                DELETE PointGroup 
                WHERE PointGroupGuid = @pointGroupGuid
                    
                SELECT @@ROWCOUNT",
                new { pointGroupGuid }) == 1;
        }

        public IEnumerable<PointGroup> GetPointGroupStructure(Guid userGuid)
        {
            var pointList = Query<dynamic>(@"
                SELECT p.PointGuid, p.Name AS PointName,
                	pg.PointGroupGuid, pg.Name AS PointGroupName
                FROM Point p
                	LEFT JOIN PointGroupPoint pgp ON pgp.PointGuid = p.PointGuid
                	FULL JOIN PointGroup pg ON pg.PointGroupGuid = pgp.PointGroupGuid");

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
                            Name = p.PointName
                        })
                };
            });
        }
    }
}
