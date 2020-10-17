using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IMapSettingsRepository
    {
        public MapSettings GetSettings();
        public bool SaveSettings(int maxZoom);
        public bool UploadPushpinImage(string image);
    }
}
