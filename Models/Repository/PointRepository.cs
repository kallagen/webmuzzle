using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public class PointRepository : RepositoryBase, IPointRepository
    {
        public PointRepository(string connectionString) : base(connectionString) { }

        public IEnumerable<Point> List()
        {
            return Query<Point>(@"
                SELECT PointGuid, [Name]
                FROM [Point]
                ORDER BY [Name]");
        }

        public Point GetPointByGuid(Guid pointGuid)
        {
            return QueryFirst<Point>(@"
                SELECT PointGuid, [Name]
                FROM [Point] WHERE PointGuid = @pointGuid", new { pointGuid });
        }

        public Guid? Create(string name)
        {
            return QueryFirst<Guid?>(@"
                DECLARE @guid UNIQUEIDENTIFIER = NEWID()

                INSERT [Point](PointGuid, [Name])
                VALUES(@guid, @name)
                
                SELECT PointGuid FROM [Point] WHERE PointGuid = @guid",
                new { name });
        }

        public bool Edit(Guid PointGuid, string name)
        {
            return QueryFirst<int?>(@"
                UPDATE [Point] SET 
                    [Name] = @name
                WHERE PointGuid = @pointGuid

                SELECT @@ROWCOUNT",
                new { PointGuid, name }) == 1;
        }

        public bool Remove(Guid pointGuid)
        {
            return QueryFirst<int?>(@"
                DELETE [Point] 
                WHERE PointGuid = @pointGuid
                    
                SELECT @@ROWCOUNT",
                new { pointGuid }) == 1;
        }
    }
}