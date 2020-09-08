using Microsoft.Extensions.Configuration;

namespace TSensor.FakeSensor
{
    public class Config
    {
        public readonly string ApiPushValueUrl;
        public readonly string ApiPushCoordinatesUrl;

        public Config()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appconfig.json")
                .Build();

            ApiPushValueUrl = $"{config["apiUrl"]}/sensorvalue/push";
            ApiPushCoordinatesUrl = $"{config["apiUrl"]}/coordinates/push";
        }
    }
}
