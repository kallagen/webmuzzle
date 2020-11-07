using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
	public class ApiRepository : RepositoryBase, IApiRepository
    {
        public ApiRepository(string connectionString) : base(connectionString) { }

        public async Task<bool> PushValueAsync(string ip, ActualSensorValue value, string rawValue)
        {
			await ExecuteAsync(@"
                INSERT SensorValueRaw([Ip], [Value], EventUTCDate, DeviceGuid)
                VALUES (@ip, @rawValue, @EventUTCDate, @DeviceGuid)", 
				new { ip, rawValue, value.EventUTCDate, value.DeviceGuid });

			/*
            if (value.pressureAndTempSensorState == "00")
            {
				return true;
            }
			*/

            return QueryFirst<int>(@"
				DECLARE
					@findTankGuid uniqueidentifier,
					@findIsSecond bit = 0
					
					SELECT TOP 1 @findTankGuid = TankGuid, @findIsSecond = CASE 
						WHEN DualMode = 0 THEN NULL
						WHEN DualMode = 1 AND SecondDeviceGuid = @DeviceGuid AND SecondIZKId = @izkNumber AND SecondSensorId = @sensorSerial THEN 1 
						ELSE 0 END
					FROM Tank
					WHERE 
						(MainDeviceGuid = @DeviceGuid AND MainIZKId = @izkNumber AND MainSensorId = @sensorSerial) OR
						(DualMode = 1 AND SecondDeviceGuid = @DeviceGuid AND SecondIZKId = @izkNumber AND SecondSensorId = @sensorSerial)

				IF @findTankGuid IS NOT NULL BEGIN
					INSERT SensorValue(
						TankGuid, IsSecond, DeviceGuid, EventUTCDate,
						izkNumber, banderolType, sensorSerial, sensorChannel, pressureAndTempSensorState,
						sensorFirmwareVersionAndReserv, alarma, 
						environmentLevel, 
						pressureFilter, pressureMeasuring,
						levelInPercent, environmentVolume, liquidEnvironmentLevel, steamMass, liquidDensity, 
						steamDensity, dielectricPermeability, dielectricPermeability2, t1, t2, t3, t4, t5, t6, 
						plateTemp, [period], plateServiceParam, environmentComposition, cs1, plateServiceParam2,
						plateServiceParam3, sensorWorkMode, plateServiceParam4, plateServiceParam5, crc)
					VALUES(
						@findTankGuid, @findIsSecond, @DeviceGuid, @EventUTCDate,
						@izkNumber, @banderolType, @sensorSerial, @sensorChannel, @pressureAndTempSensorState,
						@sensorFirmwareVersionAndReserv, @alarma,
						CASE WHEN @findIsSecond IS NULL THEN @environmentLevel ELSE @environmentLevel * 10 END, 
						@pressureFilter, @pressureMeasuring,
						@levelInPercent, @environmentVolume, @liquidEnvironmentLevel, @steamMass, @liquidDensity,
						@steamDensity, @dielectricPermeability, @dielectricPermeability2, @t1, @t2, @t3, @t4, @t5, @t6,
						@plateTemp, @period, @plateServiceParam, @environmentComposition, @cs1, @plateServiceParam2,
						@plateServiceParam3, @sensorWorkMode, @plateServiceParam4, @plateServiceParam5, @crc)
				END

				DECLARE @lastEventUTCDate datetime = (SELECT TOP 1 EventUTCDate
					FROM ActualSensorValue 
					WHERE
						TankGuid = @findTankGuid OR
						(DeviceGuid = @DeviceGuid AND izkNumber = @izkNumber AND sensorSerial = @sensorSerial))

				IF @lastEventUTCDate IS NULL OR @eventUTCDate > @lastEventUTCDate BEGIN
					DELETE FROM ActualSensorValue
					WHERE
						(TankGuid = @findTankGuid AND ISNULL(@findIsSecond, 0) != 1) OR
						(DeviceGuid = @DeviceGuid AND izkNumber = @izkNumber AND sensorSerial = @sensorSerial)

					IF ISNULL(@findIsSecond, 0) != 1 BEGIN
						INSERT ActualSensorValue(
							TankGuid, DeviceGuid, EventUTCDate,
							izkNumber, banderolType, sensorSerial, sensorChannel, pressureAndTempSensorState,
							sensorFirmwareVersionAndReserv, alarma, environmentLevel, pressureFilter, pressureMeasuring,
							levelInPercent, environmentVolume, liquidEnvironmentLevel, steamMass, liquidDensity,
							steamDensity, dielectricPermeability, dielectricPermeability2, t1, t2, t3, t4, t5, t6,
							plateTemp, [period], plateServiceParam, environmentComposition, cs1, plateServiceParam2,
							plateServiceParam3, sensorWorkMode, plateServiceParam4, plateServiceParam5, crc)
						VALUES (
							@findTankGuid, @DeviceGuid, @EventUTCDate,
							@izkNumber, @banderolType, @sensorSerial, @sensorChannel, @pressureAndTempSensorState,
							@sensorFirmwareVersionAndReserv, @alarma, @environmentLevel, @pressureFilter, @pressureMeasuring,
							@levelInPercent, @environmentVolume, @liquidEnvironmentLevel, @steamMass, @liquidDensity,
							@steamDensity, @dielectricPermeability, @dielectricPermeability2, @t1, @t2, @t3, @t4, @t5, @t6,
							@plateTemp, @period, @plateServiceParam, @environmentComposition, @cs1, @plateServiceParam2,
							@plateServiceParam3, @sensorWorkMode, @plateServiceParam4, @plateServiceParam5, @crc)
					END
				END

				SELECT @@ROWCOUNT", value) == 1;
        }

        public async Task PushArchivedValuesAsync(string ip, IEnumerable<ActualSensorValue> valueList)
        {
            await ExecuteAsync(@"
				INSERT SensorValueRaw([Ip], [Value], EventUTCDate, DeviceGuid)
                VALUES (@ip, @rawValue, @eventUTCDate, @deviceGuid)",
                valueList.Select(p => new { ip, rawValue = p.Raw, eventUTCDate = p.EventUTCDate, deviceGuid = p.DeviceGuid }));

			var metaDict = valueList
				.Select(p => new { p.izkNumber, p.sensorSerial, p.DeviceGuid }).Distinct()
				.Select(p =>
				{
					var meta = QueryFirst<dynamic>(@"
						SELECT TOP 1 
							TankGuid, 
							CAST(CASE 
								WHEN DualMode = 0 THEN NULL
								WHEN DualMode = 1 AND 
									SecondDeviceGuid = @deviceGuid AND 
									SecondIZKId = @izkNumber AND 
									SecondSensorId = @sensorSerial THEN 1 
								ELSE 0 END AS bit) AS IsSecond 
						FROM Tank
						WHERE
							(MainDeviceGuid = @deviceGuid AND MainIZKId = @izkNumber AND MainSensorId = @sensorSerial) OR
							(DualMode = 1 AND SecondDeviceGuid = @deviceGuid AND SecondIZKId = @izkNumber AND SecondSensorId = @sensorSerial)", new { p.DeviceGuid, p.izkNumber, p.sensorSerial });

					if (meta != null)
					{
						return new
						{
							p.izkNumber,
							p.sensorSerial,
							meta.TankGuid,
							meta.IsSecond
						};
					}
					else
					{
						return null;
					}
				}).Where(p => p != null);

			await ExecuteAsync(@"
				INSERT SensorValue(
					TankGuid, IsSecond, DeviceGuid, EventUTCDate,
					izkNumber, banderolType, sensorSerial, sensorChannel, pressureAndTempSensorState,
					sensorFirmwareVersionAndReserv, alarma, 
					environmentLevel, 
					pressureFilter, pressureMeasuring,
					levelInPercent, environmentVolume, liquidEnvironmentLevel, steamMass, liquidDensity, 
					steamDensity, dielectricPermeability, dielectricPermeability2, t1, t2, t3, t4, t5, t6, 
					plateTemp, [period], plateServiceParam, environmentComposition, cs1, plateServiceParam2,
					plateServiceParam3, sensorWorkMode, plateServiceParam4, plateServiceParam5, crc)
				VALUES (
					@TankGuid, @IsSecond, @DeviceGuid, @EventUTCDate,
					@izkNumber, @banderolType, @sensorSerial, @sensorChannel, @pressureAndTempSensorState,
					@sensorFirmwareVersionAndReserv, @alarma, 
					CASE WHEN @IsSecond IS NULL THEN @environmentLevel ELSE @environmentLevel * 10 END,
					@pressureFilter, @pressureMeasuring,
					@levelInPercent, @environmentVolume, @liquidEnvironmentLevel, @steamMass, @liquidDensity,
					@steamDensity, @dielectricPermeability, @dielectricPermeability2, @t1, @t2, @t3, @t4, @t5, @t6,
					@plateTemp, @period, @plateServiceParam, @environmentComposition, @cs1, @plateServiceParam2,
					@plateServiceParam3, @sensorWorkMode, @plateServiceParam4, @plateServiceParam5, @crc)",
	 			valueList.Select(v =>
				{
					var meta = metaDict.FirstOrDefault(p => p.izkNumber == v.izkNumber && p.sensorSerial == v.sensorSerial);
					if (meta != null)
					{
						v.DeviceGuid = v.DeviceGuid;
	
						v.TankGuid = meta.TankGuid;
						v.IsSecond = meta.IsSecond;
	
						return v;
					}
					else
					{
						return null;
					}
				}).Where(p => p != null));
        }

		public async Task<IEnumerable<dynamic>> UploadPointCoordinatesAsync(string deviceGuid, 
			decimal longitude, decimal latitude)
        {
			return await QueryAsync<dynamic>(@"
				SELECT DISTINCT p.PointGuid, p.Name AS PointName, Longitude, Latitude, IsMoving, LastMovingDateUTC
				FROM Point p
					JOIN Tank t ON p.PointGuid = t.PointGuid
				WHERE
					t.MainDeviceGuid = @deviceGuid OR 
					(t.DualMode = 1 AND t.SecondDeviceGuid = @deviceGuid)

				INSERT PointCoordinates(PointGuid, Longitude, Latitude)
				SELECT DISTINCT p.PointGuid, @longitude, @latitude
				FROM Point p
					JOIN Tank t ON p.PointGuid = t.PointGuid
				WHERE 
					t.MainDeviceGuid = @deviceGuid OR 
					(t.DualMode = 1 AND t.SecondDeviceGuid = @deviceGuid)",
				new { deviceGuid, longitude, latitude });
		}

		public async Task UpdatePointCoordinate(Guid pointGuid, decimal longitude, decimal latitude,
			bool? coordinatesChanged, bool? isMoving, DateTime? lastMovingDateUTC)
        {
			await ExecuteAsync(@"
				UPDATE Point SET
					Longitude = @longitude,
					Latitude = @latitude,
					CoordinatesChanged = ISNULL(@coordinatesChanged, CoordinatesChanged),
					IsMoving = ISNULL(@isMoving, IsMoving),
					LastMovingDateUTC = ISNULL(@lastMovingDateUTC, LastMovingDateUTC)
				WHERE PointGuid = @pointGuid",
				new
				{
					longitude, latitude, coordinatesChanged,
					isMoving, lastMovingDateUTC,
					pointGuid
				});
		}
	}
}