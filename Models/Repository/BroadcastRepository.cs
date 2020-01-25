﻿using System;
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
					InsertDate,
					DeviceGuid,
					izkNumber,
					banderolType,
					sensorSerial,
					sensorChannel,
					pressureAndTempSensorState,
					sensorFirmwareVersionAndReserv,
					alarma,
					environmentLevel,
					pressureFilter,
					pressureMeasuring,
					levelInPercent,
					environmentVolume,
					liquidEnvironmentLevel,
					steamMass,
					liquidDensity,
					steamDensity,
					dielectricPermeability,
					dielectricPermeability2,
					t1,
					t2,
					t3,
					t4,
					t5,
					t6,
					plateTemp,
					[period],
					plateServiceParam,
					environmentComposition,
					cs1,
					plateServiceParam2,
					plateServiceParam3,
					sensorWorkMode,
					plateServiceParam4,
					plateServiceParam5,
					crc
				FROM ActualSensorValue");
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
					FULL JOIN ActualSensorValue m ON t.TankGuid = m.TankGuid AND m.IsSecond = 0
					LEFT JOIN ActualSensorValue s ON t.TankGuid = s.TankGuid AND t.DualMode = 1 AND s.IsSecond = 1
				WHERE 
					(@pointGuid IS NULL AND (t.TankGuid IS NOT NULL OR m.IsSecond = 0)) OR
					(@pointGuid IS NOT NULL AND p.PointGuid = @pointGuid)", new { pointGuid });
        }
    }
}