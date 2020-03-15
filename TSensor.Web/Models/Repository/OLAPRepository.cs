using System;
using System.Collections.Generic;
using System.Linq;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public class OLAPRepository : RepositoryBase, IOLAPRepository
    {
        public OLAPRepository(string connectionString) : base(connectionString) { }

        public async void AggregateAsync()
        {
            await ExecuteAsync(@"
                DECLARE @currentDate DATETIME = GETUTCDATE();
                SET @currentDate = DATEADD(MILLISECOND, -1 * DATEPART(MILLISECOND, @currentDate),
                    DATEADD(SECOND, -1 * DATEPART(SECOND, @currentDate), @currentDate))

                DELETE FROM AggregatedSensorValue
                WHERE EXISTS (SELECT 1 
                    FROM SensorValue sv
                    WHERE sv.IsAggregated = 0 AND 
                        sv.EventUTCDate < @currentDate AND
                        AggregatedSensorValue.TankGuid = sv.TankGuid AND 
                        DATEADD(MILLISECOND, -1 * DATEPART(MILLISECOND, sv.EventUTCDate),
                            DATEADD(SECOND, -1 * DATEPART(SECOND, sv.EventUTCDate), sv.EventUTCDate)) = AggregatedSensorValue.EventUTCDate)

                INSERT AggregatedSensorValue(TankGuid, EventUTCDate, Weight, Volume, Density, Temperature, Level)
                SELECT
                    sv.TankGuid,
                    DATEADD(MILLISECOND, -1 * DATEPART(MILLISECOND, sv1.EventUTCDate),
                        DATEADD(SECOND, -1 * DATEPART(SECOND, sv1.EventUTCDate), sv1.EventUTCDate)),
                    AVG(CASE WHEN sv1.IsSecond = 1 THEN NULL ELSE sv1.liquidEnvironmentLevel END),
                    AVG(CASE WHEN sv1.IsSecond = 1 THEN NULL ELSE sv1.environmentVolume END),
                    AVG(CASE WHEN sv1.IsSecond = 1 THEN NULL ELSE sv1.liquidDensity END),
                    AVG(CASE WHEN t.DualMode = 1 AND sv1.IsSecond = 0 THEN NULL ELSE (sv1.t1 + sv1.t2 + sv1.t3 + sv1.t4 + sv1.t5 + sv1.t6) / 6 END),
                    AVG(CASE WHEN sv1.IsSecond = 1 THEN NULL ELSE sv1.environmentLevel END)
                FROM SensorValue sv
                    JOIN SensorValue sv1 ON sv.TankGuid = sv1.TankGuid AND 
                        DATEADD(MILLISECOND, -1 * DATEPART(MILLISECOND, sv.EventUTCDate),
                            DATEADD(SECOND, -1 * DATEPART(SECOND, sv.EventUTCDate), sv.EventUTCDate)) = 
                        DATEADD(MILLISECOND, -1 * DATEPART(MILLISECOND, sv1.EventUTCDate),
                            DATEADD(SECOND, -1 * DATEPART(SECOND, sv1.EventUTCDate), sv1.EventUTCDate))
                    JOIN Tank t ON sv.TankGuid = t.TankGuid
                WHERE sv.IsAggregated = 0 AND sv.EventUTCDate < @currentDate 
                GROUP BY sv.TankGuid, DATEADD(MILLISECOND, -1 * DATEPART(MILLISECOND, sv1.EventUTCDate),
                    DATEADD(SECOND, -1 * DATEPART(SECOND, sv1.EventUTCDate), sv1.EventUTCDate))
                
                UPDATE SensorValue SET 
                    IsAggregated = 1
                WHERE EventUTCDate < @currentDate");
        }

        private string GetAggregatedValueColumnName(string paramName)
        {
            return new[] { "Weight", "Volume", "Density", "Temperature", "Level" }
                .Contains(paramName) ? paramName : "NULL";
        }

        public IEnumerable<AggregatedSensorValue> GetAggregatedSensorValues(IEnumerable<Guid> tankGuidList,
            DateTime dateStart, DateTime dateEnd, string paramName, string additionalParamName)
        {
            return Query<AggregatedSensorValue>($@"
                SELECT 
                    TankGuid, EventUTCDate, 
                    {GetAggregatedValueColumnName(paramName)} AS Value, 
                    {GetAggregatedValueColumnName(additionalParamName)} AS AdditionalValue 
                FROM AggregatedSensorValue
                WHERE TankGuid IN @tankGuidList AND
                    EventUTCDate >= @dateStart AND EventUTCDate < @dateEnd",
                new { tankGuidList, dateStart, dateEnd = dateStart.AddDays(1) });
        }
    }
}