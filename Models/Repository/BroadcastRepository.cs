using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
	public class BroadcastRepository : RepositoryBase, IBroadcastRepository
    {
        public BroadcastRepository(string connectionString) : base(connectionString) { }

        public IEnumerable<SensorValue> GetActualSensorValues()
        {
            return Query<SensorValue>(@"
				SELECT 
					InsertDate,					
					DeviceGuid,
					izkNumber AS IzkNumber,
					sensorSerial AS SensorSerial,
			        environmentLevel AS EnvironmentLevel,
					levelInPercent AS LevelInPercent,
					environmentVolume AS EnvironmentVolume,
					liquidEnvironmentLevel AS LiquidEnvironmentLevel,
					liquidDensity AS LiquidDensity,
					t1 AS T1,
					t2 AS T2,
					t3 AS T3,
					t4 AS T4,
					t5 AS T5,
					t6 AS T6
				FROM ActualSensorValue");
        }        
    }
}