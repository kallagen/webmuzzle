using System.Threading.Tasks;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public class ApiRepository : RepositoryBase, IApiRepository
    {
        public ApiRepository(string connectionString) : base(connectionString) { }

        public async Task<bool> PushValueAsync(string ip, ActualSensorValue value, string rawValue)
        {
            await QueryFirstAsync<int>(@"
                INSERT SensorValueRaw([Ip], [Value], EventUTCDate, DeviceGuid)
                VALUES (@ip, @rawValue, @eventUTCDate, @deviceGuid)
                
                SELECT @@ROWCOUNT", new
            {
                ip,
				rawValue,
				eventUTCDate = value.EventUTCDate,
                deviceGuid = value.DeviceGuid
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

				IF @findTankGuid IS NOT NULL BEGIN
					INSERT SensorValue(
						TankGuid, IsSecond, DeviceGuid, EventUTCDate,
						izkNumber, banderolType, sensorSerial, sensorChannel, pressureAndTempSensorState,
						sensorFirmwareVersionAndReserv, alarma, environmentLevel, pressureFilter, pressureMeasuring,
						levelInPercent, environmentVolume, liquidEnvironmentLevel, steamMass, liquidDensity, 
						steamDensity, dielectricPermeability, dielectricPermeability2, t1, t2, t3, t4, t5, t6, 
						plateTemp, [period], plateServiceParam, environmentComposition, cs1, plateServiceParam2,
						plateServiceParam3, sensorWorkMode, plateServiceParam4, plateServiceParam5, crc)
					VALUES (
						@findTankGuid, @findIsSecond, @DeviceGuid, @EventUTCDate,
						@izkNumber, @banderolType, @sensorSerial, @sensorChannel, @pressureAndTempSensorState,
						@sensorFirmwareVersionAndReserv, @alarma, @environmentLevel, @pressureFilter, @pressureMeasuring,
						@levelInPercent, @environmentVolume, @liquidEnvironmentLevel, @steamMass, @liquidDensity,
						@steamDensity, @dielectricPermeability, @dielectricPermeability2, @t1, @t2, @t3, @t4, @t5, @t6,
						@plateTemp, @period, @plateServiceParam, @environmentComposition, @cs1, @plateServiceParam2,
						@plateServiceParam3, @sensorWorkMode, @plateServiceParam4, @plateServiceParam5, @crc)
				END
				
				DECLARE @lastEventUTCDate = (SELECT TOP 1 EventUTCDate
					FROM ActualSensorValue 
					WHERE
						(TankGuid = @findTankGuid AND IsSecond = @findIsSecond) OR
						(DeviceGuid = @DeviceGuid AND izkNumber = @izkNumber AND sensorSerial = @sensorSerial)

				IF @lastEventUTCDate IS NULL OR @eventUTCDate > @lastEventUTCDate BEGIN
					DELETE FROM ActualSensorValue 
					WHERE
						(TankGuid = @findTankGuid AND IsSecond = @findIsSecond) OR
						(DeviceGuid = @DeviceGuid AND izkNumber = @izkNumber AND sensorSerial = @sensorSerial)

					INSERT ActualSensorValue(
						TankGuid, IsSecond, DeviceGuid, EventUTCDate,
						izkNumber, banderolType, sensorSerial, sensorChannel, pressureAndTempSensorState,
						sensorFirmwareVersionAndReserv, alarma, environmentLevel, pressureFilter, pressureMeasuring,
						levelInPercent, environmentVolume, liquidEnvironmentLevel, steamMass, liquidDensity,
						steamDensity, dielectricPermeability, dielectricPermeability2, t1, t2, t3, t4, t5, t6,
						plateTemp, [period], plateServiceParam, environmentComposition, cs1, plateServiceParam2,
						plateServiceParam3, sensorWorkMode, plateServiceParam4, plateServiceParam5, crc)
					VALUES (
						@findTankGuid, @findIsSecond, @DeviceGuid, @EventUTCDate,
						@izkNumber, @banderolType, @sensorSerial, @sensorChannel, @pressureAndTempSensorState,
						@sensorFirmwareVersionAndReserv, @alarma, @environmentLevel, @pressureFilter, @pressureMeasuring,
						@levelInPercent, @environmentVolume, @liquidEnvironmentLevel, @steamMass, @liquidDensity,
						@steamDensity, @dielectricPermeability, @dielectricPermeability2, @t1, @t2, @t3, @t4, @t5, @t6,
						@plateTemp, @period, @plateServiceParam, @environmentComposition, @cs1, @plateServiceParam2,
						@plateServiceParam3, @sensorWorkMode, @plateServiceParam4, @plateServiceParam5, @crc)
				END

				SELECT @@ROWCOUNT", value) == 1;
            }
			else
			{
				return true;
			}
        }
    }
}