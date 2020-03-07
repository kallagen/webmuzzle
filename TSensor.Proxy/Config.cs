using Microsoft.Extensions.Configuration;

namespace TSensor.Proxy
{
    public class Config
    {
        public string ApiUrl { get; private set; }
        public string DeviceGuid { get; private set; }
        public int CheckPortInterval { get; private set; }

        public Config()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appconfig.json")
                .Build();

            ApiUrl = $"http://{config["apiHost"]}/api/sensorvalue/push";
            DeviceGuid = config["deviceGuid"];
            CheckPortInterval = int.Parse(config["portCheckInterval"]) * 1000;
        }
    }
}
