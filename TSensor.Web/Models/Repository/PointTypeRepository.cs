using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public class PointTypeRepository : RepositoryBase, IPointTypeRepository
    {
        public PointTypeRepository(string connectionString) : base(connectionString) { }

        public IEnumerable<PointType> List()
        {
            return Query<PointType>(@"
                SELECT PointTypeGuid, Name
                FROM PointType");
        }

        public PointType GetByGuid(Guid pointTypeGuid)
        {
            return QueryFirst<PointType>(@"
                SELECT Name, [Image]
                FROM PointType
                WHERE PointTypeGuid = @pointTypeGuid", new { pointTypeGuid });
        }

        public Guid? Create(string name, string image)
        {
            return QueryFirst<Guid?>(@"
                DECLARE @guid UNIQUEIDENTIFIER = NEWID()

                INSERT [PointType](
                    PointTypeGuid, [Name], [Image])
                VALUES(
                    @guid, @name, @image)
                
                SELECT PointTypeGuid FROM [PointType] WHERE PointTypeGuid = @guid",
                new { name, image });
        }

        public bool Edit(Guid pointTypeGuid, string name, string image)
        {
            return QueryFirst<int?>(@"
                UPDATE [PointType] SET
                    [Name] = @name,
                    [Image] = ISNULL(@image, [Image])
                WHERE PointTypeGuid = @pointTypeGuid

                SELECT @@ROWCOUNT", new { pointTypeGuid, name, image }) == 1;
        }

        public bool Remove(Guid pointTypeGuid)
        {
            return QueryFirst<int?>(@"
                DELETE PointType 
                WHERE PointTypeGuid = @pointTypeGuid
                    
                SELECT @@ROWCOUNT",
                new { pointTypeGuid }) == 1;
        }
    }
}