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
                    t.TankGuid, t.Name, t.ProductGuid, p.Name AS ProductName, t.DualMode,
                    t.MainDeviceGuid, t.MainIZKId, t.MainSensorId,
                    t.SecondDeviceGuid, t.SecondIZKId, t.SecondSensorId, 
                    t.Description, t.WeightChangeDelta, t.WeightChangeTimeout
                FROM Tank t
                    LEFT JOIN Product p ON t.ProductGuid = p.ProductGuid
                WHERE PointGuid = @pointGuid", new { pointGuid });
        }

        public Tank GetByGuid(Guid tankGuid)
        {
            return QueryFirst<Tank>(@"
                SELECT TOP 1
                    TankGuid, Name, ProductGuid, DualMode,
                    MainDeviceGuid, MainIZKId, MainSensorId,
                    SecondDeviceGuid, SecondIZKId, SecondSensorId, Description,
                    WeightChangeDelta, WeightChangeTimeout
                FROM Tank 
                WHERE TankGuid = @tankGuid", new { tankGuid });
        }

        public Guid? Create(Guid pointGuid, string name, Guid? productGuid, bool dualMode,
            string mainDeviceGuid, int? mainIZKId, int? mainSensorId,
            string secondDeviceGuid, int? secondIZKId, int? secondSensorId, string description,
            decimal? weightChangeDelta, int? weightChangeTimeout)
        {
            return QueryFirst<Guid?>(@"
                DECLARE @guid UNIQUEIDENTIFIER = NEWID()

                INSERT [Tank](
                    TankGuid, PointGuid, [Name], ProductGuid, DualMode,
                    MainDeviceGuid, MainIZKId, MainSensorId,
                    SecondDeviceGuid, SecondIZKId, SecondSensorId, 
                    Description, WeightChangeDelta, WeightChangeTimeout)
                VALUES(
                    @guid, @pointGuid, @name, @productGuid, @dualMode,
                    @mainDeviceGuid, @mainIZKId, @mainSensorId,
                    @secondDeviceGuid, @secondIZKId, @secondSensorId, 
                    @description, @weightChangeDelta, @weightChangeTimeout)
                
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
                    description,
                    weightChangeDelta,
                    weightChangeTimeout
                });
        }

        public bool Edit(Guid tankGuid, Guid pointGuid, string name, Guid? productGuid, 
            bool dualMode, string mainDeviceGuid, int? mainIZKId, int? mainSensorId,
            string secondDeviceGuid, int? secondIZKId, int? secondSensorId, 
            string description, decimal? weightChangeDelta, int? weightChangeTimeout)
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
                    Description = @description,
                    WeightChangeDelta = @weightChangeDelta,
                    WeightChangeTimeout = @weightChangeTimeout
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
                    description,
                    weightChangeDelta,
                    weightChangeTimeout
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

        public dynamic GetTankActualSensorValues(Guid tankGuid)
        {
            return QueryFirst<dynamic>(@"
				SELECT
                    t.TankGuid, t.Name AS TankName, t.DualMode, p.Name AS ProductName, InsertDate, DeviceGuid,
                    izkNumber, banderolType, sensorSerial, sensorChannel, pressureAndTempSensorState,
                    sensorFirmwareVersionAndReserv, alarma, environmentLevel, pressureFilter, pressureMeasuring,
                    levelInPercent, environmentVolume, liquidEnvironmentLevel, steamMass, liquidDensity, steamDensity,
                    dielectricPermeability, dielectricPermeability2, t1, t2, t3, t4, t5, t6,  plateTemp,
                    period, plateServiceParam, environmentComposition, cs1, plateServiceParam2, plateServiceParam3,
                    sensorWorkMode, plateServiceParam4, plateServiceParam5, crc
				FROM Tank t
                    LEFT JOIN Product p ON t.ProductGuid = p.ProductGuid
					LEFT JOIN ActualSensorValue asv ON
                        t.MainDeviceGuid = asv.DeviceGuid AND t.MainIZKId = asv.izkNumber AND t.MainSensorId = asv.sensorSerial
                WHERE
					t.TankGuid = @tankGuid", new { tankGuid });
        }

        public IEnumerable<ActualSensorValue> GetTankSensorValuesHistory(Guid tankGuid, DateTime dateStart, DateTime dateEnd)
        {
            return Query<ActualSensorValue>(@"
                SELECT
                    EventUTCDate, IsSecond,
                    izkNumber, banderolType, sensorSerial, sensorChannel, pressureAndTempSensorState,
                    sensorFirmwareVersionAndReserv, alarma, environmentLevel, pressureFilter, pressureMeasuring,
                    levelInPercent, environmentVolume, liquidEnvironmentLevel, steamMass, liquidDensity, steamDensity,
                    dielectricPermeability, dielectricPermeability2, t1, t2, t3, t4, t5, t6, plateTemp,
                    period, plateServiceParam, environmentComposition, cs1, plateServiceParam2, plateServiceParam3,
                    sensorWorkMode, plateServiceParam4, plateServiceParam5, crc
                FROM SensorValue
                WHERE TankGuid = @tankGuid AND EventUTCDate >= @dateStart AND EventUTCDate < @dateEnd
                ORDER BY EventUTCDate",
                new { tankGuid, dateStart, dateEnd });
        }
    }
}