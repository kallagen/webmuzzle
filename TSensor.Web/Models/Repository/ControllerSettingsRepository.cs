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
                   ResetState
                FROM ControllerSettings");
        }

        public bool SaveSettings(decimal testValue)
        {
            throw new System.NotImplementedException();
        }

        public bool ResetController()
        {
            throw new System.NotImplementedException();
        }
    }
}