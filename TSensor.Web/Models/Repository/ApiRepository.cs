using System;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public class ApiRepository : RepositoryBase, IApiRepository
    {
        public ApiRepository(string connectionString) : base(connectionString) { }

        public bool PushValue(string ip, ActualSensorValue value, DateTime eventDateUTC)
        {
            QueryFirst<int>(@"
                INSERT SensorValueRaw([Ip], [Value], EventDateUTC, DeviceGuid)
                VALUES (@ip, @value, @eventDateUTC, @deviceGuid)
                
                SELECT @@ROWCOUNT", new
            {
                ip,
                value = value.Raw,
                deviceGuid = value.DeviceGuid,
                eventDateUTC
            });

            if (value.pressureAndTempSensorState != "00")
            {
                return QueryFirst<int>(@"
				DECLARE
					@findTankGuid uniqueidentifier,

					@findIsSecond bit = 0
					
					SELECT TOP 1 @findTankGuid = TankGuid, @findIsSecond = CASE WHEN
						DualMode = 1 AND SecondDeviceGuid = @DeviceGuid AND SecondIZKId = @izkNumber AND SecondSensorId = @sensorSerial THEN 1 ELSE 0 END
					FROM Tank
					WHERE 
						(MainDeviceGuid = @DeviceGuid AND MainIZKId = @izkNumber AND MainSensorId = @sensorSerial) OR
						(DualMode = 1 AND SecondDeviceGuid = @DeviceGuid AND SecondIZKId = @izkNumber AND SecondSensorId = @sensorSerial)

				INSERT SensorValue(
					TankGuid, IsSecond, [Raw], DeviceGuid,
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
					crc)
				VALUES (@findTankGuid, @findIsSecond, @Raw, @DeviceGuid,
					@izkNumber,
					@banderolType,
					@sensorSerial,
					@sensorChannel,
					@pressureAndTempSensorState,
					@sensorFirmwareVersionAndReserv,
					@alarma,
					@environmentLevel,
					@pressureFilter,
					@pressureMeasuring,
					@levelInPercent,
					@environmentVolume,
					@liquidEnvironmentLevel,
					@steamMass,
					@liquidDensity,
					@steamDensity,
					@dielectricPermeability,
					@dielectricPermeability2,
					@t1,
					@t2,
					@t3,
					@t4,
					@t5,
					@t6,
					@plateTemp,
					@period,
					@plateServiceParam,
					@environmentComposition,
					@cs1,
					@plateServiceParam2,
					@plateServiceParam3,
					@sensorWorkMode,
					@plateServiceParam4,
					@plateServiceParam5,
					@crc)

				DELETE FROM ActualSensorValue 
				WHERE
					(TankGuid = @findTankGuid AND IsSecond = @findIsSecond) OR
					(DeviceGuid = @DeviceGuid AND izkNumber = @izkNumber AND sensorSerial = @sensorSerial)

				INSERT ActualSensorValue(
					TankGuid, IsSecond, [Raw], DeviceGuid,
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
					crc)
				VALUES (@findTankGuid, @findIsSecond, @Raw, @DeviceGuid,
					@izkNumber,
					@banderolType,
					@sensorSerial,
					@sensorChannel,
					@pressureAndTempSensorState,
					@sensorFirmwareVersionAndReserv,
					@alarma,
					@environmentLevel,
					@pressureFilter,
					@pressureMeasuring,
					@levelInPercent,
					@environmentVolume,
					@liquidEnvironmentLevel,
					@steamMass,
					@liquidDensity,
					@steamDensity,
					@dielectricPermeability,
					@dielectricPermeability2,
					@t1,
					@t2,
					@t3,
					@t4,
					@t5,
					@t6,
					@plateTemp,
					@period,
					@plateServiceParam,
					@environmentComposition,
					@cs1,
					@plateServiceParam2,
					@plateServiceParam3,
					@sensorWorkMode,
					@plateServiceParam4,
					@plateServiceParam5,
					@crc)

				SELECT @@ROWCOUNT", value) == 1;
            }
			else
			{
				return true;
			}
        }
    }
}