using TSensor.Proxy.Logger;

namespace TSensor.Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Config();
            ILogger logger = new ConsoleLogger();

            new SerialService(config, logger).Run();
        }
    }
}