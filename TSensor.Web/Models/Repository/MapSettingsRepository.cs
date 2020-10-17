using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public class MapSettingsRepository : RepositoryBase, IMapSettingsRepository
    {
        public MapSettingsRepository(string connectionString) : base(connectionString) { }
        public MapSettings GetSettings()
        {
            return QueryFirst<MapSettings>(@"
                SELECT TOP 1 
                    MaxZoom, PushpinImage,
                    DefaultLongitude, DefaultLatitude
                FROM MapSettings");
        }

        public bool SaveSettings(int maxZoom, 
            decimal defaultLongitude, decimal defaultLatitude)
        {
            return QueryFirst<int?>(@"
                UPDATE MapSettings SET
                    MaxZoom = @maxZoom,
                    DefaultLongitude = @defaultLongitude,
                    DefaultLatitude = @defaultLatitude

                SELECT @@ROWCOUNT", new { maxZoom, defaultLongitude, defaultLatitude }) > 0;
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
