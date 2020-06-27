using TSensor.Proxy.Com;
using TSensor.Proxy.Logger;
using TSensor.Proxy.Tcp;

namespace TSensor.Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Config();
            var logger = config.LoggerType == LoggerType.DEBUG 
                ? new ConsoleLogger() as ILogger : new ErrorFileLogger(config);

            if (config.IsComInputMode)
            {
                new SerialService(config, logger).Run();
            }
            else if (config.IsTcpInputMode)
            {
                new TcpService(config, logger).Run();
            }            
        }
    }
}