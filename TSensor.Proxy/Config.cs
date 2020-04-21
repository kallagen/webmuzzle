using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;

namespace TSensor.Proxy
{
    public class Config
    {
        public string ApiUrlSendValue { get; private set; }
        public string ApiUrlSendArchive { get; private set; }
        public string DeviceGuid { get; private set; }
        public int ArchiveSendInterval { get; private set; }
        public int MaxArchiveFileSize { get; private set; }
        public int MaxErrorFileSize { get; private set; }
        public string LoggerType { get; private set; }

        public bool IsLinux =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public Config()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appconfig.json")
                .Build();

            ApiUrlSendValue = $"http://{config["apiHost"]}/api/sensorvalue/push";
            ApiUrlSendArchive = $"http://{config["apiHost"]}/api/sensorvalue/archive/push";
            DeviceGuid = config["deviceGuid"];
            ArchiveSendInterval = int.Parse(config["archiveSendInterval"]) * 1000;
            MaxArchiveFileSize = int.Parse(config["maxArchiveFileSize"]) * 1024;
            MaxErrorFileSize = int.Parse(config["maxErrorFileSize"]) * 1024;
            LoggerType = config["loggerType"];
        }
    }
}
