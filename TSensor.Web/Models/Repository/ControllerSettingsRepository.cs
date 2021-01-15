using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public class ControllerSettingsRepository : RepositoryBase, IControllerSettingsRepository
    {
        public ControllerSettingsRepository(string connectionString) : base(connectionString) { }
        
        public ControllerSettings GetSettings()
        {
            return QueryFirst<ControllerSettings>(@"
                SELECT TOP 1 
                   State
                FROM ControllerSettings");
        }

        public bool SaveSettings(decimal testValue)
        {
            throw new System.NotImplementedException();
        }

        public bool ControllerExist(string deviceGuid)
        {
            var query = QueryFirst<string>(@"
                SELECT TOP 1 
                   DeviceGuid
                FROM ActualSensorValue
                WHERE DeviceGuid = @deviceGuid",
                new {deviceGuid});
            return query != null;
        }

        public IList<string> GetAllDeviceGuid()
        {
            return QueryFirst<IList<string>>(@"
                SELECT DeviceGuid
                FROM ActualSensorValue");
        }
    }
}