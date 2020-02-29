using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TSensor.Web
{
    public class Program
    {
        public const string GLOBAL_ERROR_MESSAGE = "Произошла ошибка";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
