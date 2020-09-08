using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace TSensor.Web
{
    public class Program
    {
        public const string GLOBAL_ERROR_MESSAGE = "Произошла ошибка";

        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            CreateHostBuilder(config).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(IConfigurationRoot config) =>
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    if (config.GetValue<bool>("useKestrel"))
                    {
                        webBuilder = webBuilder.UseKestrel();
                    }

                    webBuilder.UseStartup<Startup>();
                });
    }
}
