using TSensor.Proxy.Logger;

namespace TSensor.Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Config();
            ILogger logger = 
                config.LoggerType == LoggerType.DEBUG ? new ConsoleLogger() as ILogger : new ErrorFileLogger(config);

            new SerialService(config, logger).Run();
        }
    }
}