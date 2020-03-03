using Microsoft.Extensions.Configuration;

namespace TSensor.Proxy
{
    public class Config
    {
        public string ApiHost { get; private set; }
        public string DeviceGuid { get; private set; }

        public Config()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appconfig.json")
                .Build();

            ApiHost = config["apiHost"];
            DeviceGuid = config["deviceGuid"];
        }
    }
}
