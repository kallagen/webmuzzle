using Microsoft.Extensions.Configuration;

namespace TSensor.Proxy
{
    public class Config
    {
        public string ApiUrlSendValue { get; private set; }
        public string ApiUrlSendArchive { get; private set; }
        public string DeviceGuid { get; private set; }
        public int CheckPortInterval { get; private set; }
        public int ArchiveSendInterval { get; private set; }
        public int MaxArchiveFileSize { get; private set; }

        public Config()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appconfig.json")
                .Build();

            ApiUrlSendValue = $"http://{config["apiHost"]}/api/sensorvalue/push";
            ApiUrlSendArchive = $"http://{config["apiHost"]}/api/sensorvalue/archive/push";
            DeviceGuid = config["deviceGuid"];
            CheckPortInterval = int.Parse(config["portCheckInterval"]) * 1000;
            ArchiveSendInterval = int.Parse(config["archiveSendInterval"]) * 1000;
            MaxArchiveFileSize = int.Parse(config["maxArchiveFileSize"]) * 1024;
        }
    }
}
