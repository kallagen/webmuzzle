using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public class TankRepository : RepositoryBase, ITankRepository
    {
        public TankRepository(string connectionString) : base(connectionString) { }

        public IEnumerable<Tank> GetTankListByPoint(Guid pointGuid)
        {
            return Query<Tank>(@"
                SELECT 
                    TankGuid, Name, DualMode,
                    MainDeviceGuid, MainIZKId, MainSensorId,
                    SecondDeviceGuid, SecondIZKId, SecondSensorId
                FROM Tank 
                WHERE PointGuid = @pointGuid", new { pointGuid });
        }

        public Tank GetTankByGuid(Guid tankGuid)
        {
            return QueryFirst<Tank>(@"
                SELECT TOP 1
                    TankGuid, Name, DualMode,
                    MainDeviceGuid, MainIZKId, MainSensorId,
                    SecondDeviceGuid, SecondIZKId, SecondSensorId
                FROM Tank 
                WHERE TankGuid = @tankGuid", new { tankGuid });
        }

        public Guid? Create(Guid pointGuid, string name, bool dualMode,
            string mainDeviceGuid, string mainIZKId, string mainSensorId,
            string secondDeviceGuid, string secondIZKId, string secondSensorId)
        {
            return QueryFirst<Guid?>(@"
                DECLARE @guid UNIQUEIDENTIFIER = NEWID()

                INSERT [Tank](
                    TankGuid, PointGuid, [Name], DualMode,
                    MainDeviceGuid, MainIZKId, MainSensorId,
                    SecondDeviceGuid, SecondIZKId, SecondSensorId)
                VALUES(
                    @guid, @pointGuid, @name, @dualMode,
                    @mainDeviceGuid, @mainIZKId, @mainSensorId,
                    @secondDeviceGuid, @secondIZKId, @secondSensorId)
                
                SELECT TankGuid FROM [Tank] WHERE TankGuid = @guid",
                new
                {
                    pointGuid,
                    name,
                    dualMode,
                    mainDeviceGuid,
                    mainIZKId,
                    mainSensorId,
                    secondDeviceGuid,
                    secondIZKId,
                    secondSensorId
                });
        }

        public bool Edit(Guid tankGuid, Guid pointGuid, string name, bool dualMode,
            string mainDeviceGuid, string mainIZKId, string mainSensorId,
            string secondDeviceGuid, string secondIZKId, string secondSensorId)
        {
            return QueryFirst<int?>(@"
                UPDATE [Tank] SET 
                    [Name] = @name,
                    DualMode = @dualMode,
                    MainDeviceGuid = @mainDeviceGuid, 
                    MainIZKId = @mainIZKId, 
                    MainSensorId = @mainSensorId,
                    SecondDeviceGuid = @secondDeviceGuid, 
                    SecondIZKId = @secondIZKId, 
                    SecondSensorId = @secondSensorId
                WHERE TankGuid = @tankGuid AND PointGuid = @pointGuid

                SELECT @@ROWCOUNT",
                new
                {
                    tankGuid,
                    pointGuid,
                    name,
                    dualMode,
                    mainDeviceGuid,
                    mainIZKId,
                    mainSensorId,
                    secondDeviceGuid,
                    secondIZKId,
                    secondSensorId
                }) == 1;
        }

        public bool Remove(Guid tankGuid)
        {
            throw new NotImplementedException();
        }
    }
}