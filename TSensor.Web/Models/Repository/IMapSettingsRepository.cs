using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IMapSettingsRepository
    {
        public MapSettings GetSettings();
        public bool SaveSettings(int maxZoom, decimal defaultLongitude, decimal defaultLatitude);
        public bool UploadPushpinImage(string image);
    }
}
