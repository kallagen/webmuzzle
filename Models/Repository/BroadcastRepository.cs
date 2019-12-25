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
    }
}