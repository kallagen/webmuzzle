using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public class BroadcastRepository : RepositoryBase, IBroadcastRepository
    {
        public BroadcastRepository(string connectionString) : base(connectionString) { }

        public IEnumerable<ActualSensorValue> GetActualSensorValues()
        {
            return Query<ActualSensorValue>(@"
                SELECT 
    	                s.SensorGuid,
    	                q.SensorValueId, q.InsertDate,
                        DATEADD(HOUR, DATEDIFF(HOUR, GETUTCDATE(), GETDATE()), q.EventDateUTC) AS EventDate, 
                        SUBSTRING(q.Value, 2, 127) AS Value
                FROM
    	                (SELECT DISTINCT SensorGuid AS SensorGuid FROM SensorValueRaw) s
    	                OUTER APPLY
    	                (
    	                	    SELECT TOP 1 *
    	                	    FROM SensorValueRaw svr 
    	                	    WHERE svr.SensorGuid = s.SensorGuid
    	                	    ORDER BY EventDateUTC DESC
    	                ) q");
        }

        public IEnumerable<SensorValue> GetPointsCurrentState()
        {
            return Query<SensorValue>(@"
				SELECT 
					p.PointGuid, t.TankGuid,
					p.Name AS PointName, t.Name AS TankName,
					m.InsertDate AS MainSensorLastDate, s.InsertDate AS SecondSensorLastDate,
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
					LEFT JOIN ActualSensorValue m ON t.TankGuid = m.TankGuid AND m.IsSecond = 0
					LEFT JOIN ActualSensorValue s ON t.TankGuid = s.TankGuid AND t.DualMode = 1 AND s.IsSecond = 1");
        }
    }
}