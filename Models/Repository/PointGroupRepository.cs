using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public class PointGroupRepository : RepositoryBase, IPointGroupRepository
    {
        public PointGroupRepository(string connectionString) : base(connectionString) { }

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
            return QueryFirst<PointGroup>(@"
                SELECT PointGroupGuid, Name
                FROM PointGroup
                WHERE PointGroupGuid = @pointGroupGuid", new { pointGroupGuid });
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
    }
}
