using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public class MapSettingsRepository : RepositoryBase, IMapSettingsRepository
    {
        public MapSettingsRepository(string connectionString) : base(connectionString) { }
        public MapSettings GetSettings()
        {
            return QueryFirst<MapSettings>(@"
                SELECT TOP 1 MaxZoom, PushpinImage FROM MapSettings");
        }

        public bool SaveSettings(int maxZoom)
        {
            return QueryFirst<int?>(@"
                UPDATE MapSettings SET
                    MaxZoom = @maxZoom

                SELECT @@ROWCOUNT", new { maxZoom }) > 0;
        }

        public bool UploadPushpinImage(string image)
        {
            return QueryFirst<int?>(@"
                UPDATE MapSettings SET
                    PushpinImage = @image

                SELECT @@ROWCOUNT", new { image }) > 0;
        }
    }
}
