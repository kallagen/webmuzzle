using System;
using System.Collections.Generic;
using System.Linq;
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
                    t.TankGuid, t.Name AS TankName, t.DualMode, p.Name AS PointName, pr.Name AS ProductName, InsertDate, DeviceGuid,
                    izkNumber, banderolType, sensorSerial, sensorChannel, pressureAndTempSensorState,
                    sensorFirmwareVersionAndReserv, alarma, environmentLevel, pressureFilter, pressureMeasuring,
                    levelInPercent, environmentVolume, liquidEnvironmentLevel, steamMass, liquidDensity, steamDensity,
                    dielectricPermeability, dielectricPermeability2, t1, t2, t3, t4, t5, t6,  plateTemp,
                    period, plateServiceParam, environmentComposition, cs1, plateServiceParam2, plateServiceParam3,
                    sensorWorkMode, plateServiceParam4, plateServiceParam5, crc
				FROM Tank t
                    LEFT JOIN Point p ON p.PointGuid = t.PointGuid
                    LEFT JOIN Product pr ON t.ProductGuid = pr.ProductGuid
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

        public IEnumerable<dynamic> GetTankWithoutSensorList()
        {
            return Query<dynamic>(@"
                SELECT p.Name AS PointName, t.TankGuid, t.Name AS TankName, pr.Name AS ProductName,
	                CASE 
                        WHEN
		                    ISNULL(t.MainDeviceGuid, '') = '' AND ISNULL(t.MainIZKId, '') = '' AND ISNULL(MainSensorId, '') = '' AND
		                    ISNULL(t.SecondDeviceGuid, '') = '' AND ISNULL(t.SecondIZKId, '') = '' AND ISNULL(SecondSensorId, '') = '' AND t.DualMode = 1 THEN 0
		                WHEN
		                    ISNULL(t.MainDeviceGuid, '') = '' AND ISNULL(t.MainIZKId, '') = '' AND ISNULL(MainSensorId, '') = '' THEN 1
		                WHEN
		                    ISNULL(t.SecondDeviceGuid, '') = '' AND ISNULL(t.SecondIZKId, '') = '' AND ISNULL(SecondSensorId, '') = '' AND t.DualMode = 1 THEN 2
		                ELSE -1 END AS Mode
                FROM Tank t
                	LEFT JOIN Point p ON p.PointGuid = t.PointGuid
                	LEFT JOIN Product pr ON pr.ProductGuid = t.ProductGuid
                WHERE
                	(ISNULL(t.MainDeviceGuid, '') = '' AND ISNULL(t.MainIZKId, '') = '' AND ISNULL(MainSensorId, '') = '') OR
                	(ISNULL(t.SecondDeviceGuid, '') = '' AND ISNULL(t.SecondIZKId, '') = '' AND ISNULL(SecondSensorId, '') = '' AND t.DualMode = 1)");
        }

        public bool SetSensorValue(bool isSecondSensor, Guid tankGuid, string deviceGuid, int izkId, int sensorId)
        {
            return QueryFirst<int>(@"
                UPDATE Tank SET
                    MainDeviceGuid = CASE WHEN @isSecondSensor = 1 THEN MainDeviceGuid ELSE @deviceGuid END,
                    MainIZKId = CASE WHEN @isSecondSensor = 1 THEN MainIZKId ELSE @izkId END,
                    MainSensorId = CASE WHEN @isSecondSensor = 1 THEN MainSensorId ELSE @sensorId END,
                    
                    SecondDeviceGuid = CASE WHEN @isSecondSensor = 1 THEN @deviceGuid ELSE SecondDeviceGuid END,
                    SecondIZKId = CASE WHEN @isSecondSensor = 1 THEN @izkId ELSE SecondIZKId END,
                    SecondSensorId = CASE WHEN @isSecondSensor = 1 THEN @sensorId ELSE SecondSensorId END
                WHERE TankGuid = @tankGuid

                SELECT @@ROWCOUNT", new { isSecondSensor, tankGuid, deviceGuid, izkId, sensorId }) == 1;
        }

        #region CalibrationData
        public bool HasTankCalibrationData(Guid tankGuid)
        {
            return QueryFirst<int?>(@"
                SELECT TOP 1 1
                FROM TankСalibrationData
                WHERE TankGuid = @tankGuid", new { tankGuid }) == 1;
        }

        public Dictionary<int, decimal> GetTankCalibrationData(Guid tankGuid)
        {
            return Query<dynamic>(@"
                SELECT [Level], [Value]
                FROM TankСalibrationData
                WHERE TankGuid = @tankGuid", new { tankGuid })
                .Select(p => new KeyValuePair<int, decimal>(p.Level, p.Value))
                .ToDictionary(p => p.Key, v => v.Value);
        }

        public bool UploadTankCalibrationData(Guid tankGuid, Dictionary<int, decimal> data)
        {
            QueryFirst<int?>(@"
                DELETE TankСalibrationData
                WHERE TankGuid = @tankGuid

                SELECT @@ROWCOUNT",
                new { tankGuid });

            var result = true;
            foreach (var item in data)
            {
                result = result && QueryFirst<int?>(@"
                    INSERT TankСalibrationData(TankGuid, [Level], [Value])
                    VALUES(@tankGuid, @level, @value)

                    SELECT @@ROWCOUNT", new { tankGuid, level = item.Key, value = item.Value }) == 1;
            }

            return result;
        }
        #endregion
    }
}