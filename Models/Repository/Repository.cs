using System;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public class Repository : RepositoryBase, IRepository
    {
        public Repository(string connectionString) : base(connectionString) { }

        public bool PushValue(string ip, SensorValue value, DateTime eventDateUTC, string deviceGuid)
        {
            var sensorGuid = value.Raw.Substring(1, 2) + value.Raw.Substring(5, 2);

            QueryFirst<int>(@"
                INSERT SensorValueRaw(SensorGuid, [Ip], [Value], EventDateUTC, DeviceGuid)
                VALUES (@sensorGuid, @ip, @value, @eventDateUTC, @deviceGuid)
                
                SELECT @@ROWCOUNT", new
            {
                sensorGuid,
                ip,
                value = value.Raw,
                eventDateUTC,
                deviceGuid
            });

            return QueryFirst<int>(@"
				INSERT SensorValue(
					TankGuid, [Raw], DeviceGuid,
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
				VALUES (@TankGuid, @Raw, @DeviceGuid,
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
    }
}