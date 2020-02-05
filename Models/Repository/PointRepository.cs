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

        public Point GetByGuid(Guid pointGuid)
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

        public IEnumerable<PointTankInfo> GetAllPointInfo()
        {
            return Query<PointTankInfo>(@"
                SELECT
                    p.PointGuid, p.Name AS PointName,
					t.TankGuid, t.Name AS TankName, t.DualMode AS DualMode,
					m.InsertDate AS MainSensorLastDate, s.InsertDate AS SecondSensorLastDate
                FROM Point p
                    LEFT JOIN Tank t ON p.PointGuid = t.PointGuid
                    LEFT JOIN ActualSensorValue m ON t.TankGuid = m.TankGuid AND m.IsSecond = 0
                    LEFT JOIN ActualSensorValue s ON t.TankGuid = s.TankGuid AND t.DualMode = 1 AND s.IsSecond = 1");
        }

        public IEnumerable<ActualSensorValue> GetSensorActualState(Guid? pointGuid = null)
        {
            return Query<ActualSensorValue>(@"
				SELECT 
					p.PointGuid, p.Name AS PointName,
					t.TankGuid, t.Name AS TankName, t.DualMode AS DualMode,
					m.InsertDate AS MainSensorLastDate, s.InsertDate AS SecondSensorLastDate,
					t.MainDeviceGuid, t.MainIZKId, t.MainSensorId,
					t.SecondDeviceGuid, t.SecondIZKId, t.SecondSensorId,

					m.DeviceGuid,
					m.izkNumber,
					m.banderolType,
					m.sensorSerial,
					m.sensorChannel,
					m.pressureAndTempSensorState,
					m.sensorFirmwareVersionAndReserv,
					m.alarma,
					m.environmentLevel,
					m.pressureFilter,
					m.pressureMeasuring,
					m.levelInPercent,
					m.environmentVolume,
					m.liquidEnvironmentLevel,
					m.steamMass,
					m.liquidDensity,
					m.steamDensity,
					m.dielectricPermeability,
					m.dielectricPermeability2,
					ISNULL(s.t1, m.t1) AS t1,
					ISNULL(s.t2, m.t2) AS t2,
					ISNULL(s.t3, m.t3) AS t3,
					ISNULL(s.t4, m.t4) AS t4,
					ISNULL(s.t5, m.t5) AS t5,
					ISNULL(s.t6, m.t6) AS t6,
					m.plateTemp,
					m.[period],
					m.plateServiceParam,
					m.environmentComposition,
					m.cs1,
					m.plateServiceParam2,
					m.plateServiceParam3,
					m.sensorWorkMode,
					m.plateServiceParam4,
					m.plateServiceParam5,
					m.crc
				FROM Point p
					LEFT JOIN Tank t ON p.PointGuid = t.PointGuid
					FULL JOIN ActualSensorValue m ON t.MainDeviceGuid = m.DeviceGuid AND t.MainIZKId = m.izkNumber AND t.MainSensorId = m.sensorSerial
					LEFT JOIN ActualSensorValue s ON t.DualMode = 1 AND t.SecondDeviceGuid = s.DeviceGuid AND t.SecondIZKId = s.izkNumber AND t.SecondSensorId = s.sensorSerial
				WHERE 
					(@pointGuid IS NOT NULL AND p.PointGuid = @pointGuid) OR @pointGuid IS NULL AND ((t.TankGuid IS NOT NULL OR m.InsertDate IS NOT NULL) AND NOT EXISTS(SELECT 1 
						FROM Tank t1 
						WHERE t1.SecondDeviceGuid = m.DeviceGuid AND t1.SecondIZKId = m.izkNumber AND t1.SecondSensorId = m.sensorSerial))", new { pointGuid });
        }
    }
}