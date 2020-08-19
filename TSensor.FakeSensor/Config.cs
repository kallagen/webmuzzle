using Microsoft.Extensions.Configuration;

namespace TSensor.FakeSensor
{
    public class Config
    {
        public readonly string ApiUrl;

        public Config()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appconfig.json")
                .Build();

            ApiUrl = config["apiUrl"];
        }
    }
}
