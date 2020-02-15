using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public class TankRepository : RepositoryBase, ITankRepository
    {
        public TankRepository(string connectionString) : base(connectionString) { }

        public IEnumerable<Tank> GetListByPoint(Guid pointGuid)
        {
            return Query<Tank>(@"
                SELECT 
                    TankGuid, Name, DualMode,
                    MainDeviceGuid, MainIZKId, MainSensorId,
                    SecondDeviceGuid, SecondIZKId, SecondSensorId
                FROM Tank 
                WHERE PointGuid = @pointGuid", new { pointGuid });
        }

        public Tank GetByGuid(Guid tankGuid)
        {
            return QueryFirst<Tank>(@"
                SELECT TOP 1
                    TankGuid, Name, ProductGuid, DualMode,
                    MainDeviceGuid, MainIZKId, MainSensorId,
                    SecondDeviceGuid, SecondIZKId, SecondSensorId, Description
                FROM Tank 
                WHERE TankGuid = @tankGuid", new { tankGuid });
        }

        public Guid? Create(Guid pointGuid, string name, Guid? productGuid, bool dualMode,
            string mainDeviceGuid, string mainIZKId, string mainSensorId,
            string secondDeviceGuid, string secondIZKId, string secondSensorId, string description)
        {
            return QueryFirst<Guid?>(@"
                DECLARE @guid UNIQUEIDENTIFIER = NEWID()

                INSERT [Tank](
                    TankGuid, PointGuid, [Name], ProductGuid, DualMode,
                    MainDeviceGuid, MainIZKId, MainSensorId,
                    SecondDeviceGuid, SecondIZKId, SecondSensorId, Description)
                VALUES(
                    @guid, @pointGuid, @name, @productGuid, @dualMode,
                    @mainDeviceGuid, @mainIZKId, @mainSensorId,
                    @secondDeviceGuid, @secondIZKId, @secondSensorId, @description)
                
                SELECT TankGuid FROM [Tank] WHERE TankGuid = @guid",
                new
                {
                    pointGuid,
                    name,
                    productGuid,
                    dualMode,
                    mainDeviceGuid,
                    mainIZKId,
                    mainSensorId,
                    secondDeviceGuid,
                    secondIZKId,
                    secondSensorId,
                    description
                });
        }

        public bool Edit(Guid tankGuid, Guid pointGuid, string name, Guid? productGuid, 
            bool dualMode, string mainDeviceGuid, string mainIZKId, string mainSensorId,
            string secondDeviceGuid, string secondIZKId, string secondSensorId, string description)
        {
            return QueryFirst<int?>(@"
                UPDATE [Tank] SET 
                    [Name] = @name,
                    ProductGuid = @productGuid,
                    DualMode = @dualMode,
                    MainDeviceGuid = @mainDeviceGuid, 
                    MainIZKId = @mainIZKId, 
                    MainSensorId = @mainSensorId,
                    SecondDeviceGuid = @secondDeviceGuid, 
                    SecondIZKId = @secondIZKId, 
                    SecondSensorId = @secondSensorId,
                    Description = @description
                WHERE TankGuid = @tankGuid AND PointGuid = @pointGuid

                SELECT @@ROWCOUNT",
                new
                {
                    tankGuid,
                    pointGuid,
                    name,
                    productGuid,
                    dualMode,
                    mainDeviceGuid,
                    mainIZKId,
                    mainSensorId,
                    secondDeviceGuid,
                    secondIZKId,
                    secondSensorId,
                    description
                }) == 1;
        }

        public bool Remove(Guid tankGuid, Guid pointGuid)
        {
            return QueryFirst<int?>(@"
                DELETE [Tank] 
                WHERE TankGuid = @tankGuid AND PointGuid = @pointGuid
                    
                SELECT @@ROWCOUNT",
                new { tankGuid, pointGuid }) == 1;
        }
    }
}