using System;

namespace TSensor.Web.Models.Repository
{
    public class Repository : RepositoryBase, IRepository
    {
        public Repository(string connectionString) : base(connectionString) { }

        public bool PushValue(string ip, string value, DateTime eventDateUTC, string deviceGuid)
        {
            var sensorGuid = value.Substring(1, 2) + value.Substring(5, 2);

            return QueryFirst<int>(@"
                INSERT SensorValueRaw(SensorGuid, [Ip], [Value], EventDateUTC, DeviceGuid)
                VALUES (@sensorGuid, @ip, @value, @eventDateUTC, @deviceGuid)
                
                SELECT @@ROWCOUNT", new
            {
                sensorGuid,
                ip,
                value,
                eventDateUTC,
                deviceGuid
            }) == 1;
        }
    }
}