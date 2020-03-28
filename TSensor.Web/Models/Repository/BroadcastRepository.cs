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
					InsertDate, DeviceGuid,
                    izkNumber, banderolType, sensorSerial, sensorChannel
                    sensorFirmwareVersionAndReserv,
                    alarma, environmentLevel, pressureFilter, pressureMeasuring,
                    levelInPercent, environmentVolume, liquidEnvironmentLevel,
                    steamMass, liquidDensity, steamDensity, dielectricPermeability,
                    dielectricPermeability2, t1, t2, t3, t4, t5, t6, plateTemp,
                    period, environmentComposition, cs1, crc
				FROM ActualSensorValue
                WHERE InsertDate >= DATEADD(HOUR, -1, GETDATE())");
        }
    }
}