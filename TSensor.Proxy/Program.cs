using System.IO.Ports;
using TSensor.Proxy.Com;
using TSensor.Proxy.Commands;
using TSensor.Proxy.Gps;
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
            var commandsRepository = new CommandsRepository(logger, config);

            if (config.UseGps)
            {
                new GpsService(config, logger).Run();
                logger.Log("gps enabled");
            }
            else
            {
                logger.Log("gps disabled");
            }
            
            
            
            if (config.UseCommandSendingApi && config.IsComInputMode)
            {
                ComPortsRepository.FillAndOpenAll(config);
                new CommandsService(config, logger, commandsRepository).Run();
            }
            else
                logger.Log("Command Sending API disabled because of DeviceGuid or CommandGetInterval is empty in config or its not COM input mode");
            

            if (config.IsComInputMode)
            {
                ComPortsRepository.FillAndOpenAll(config);
                new SerialService(config, logger).Run();
            }
            else if (config.IsTcpInputMode)
            {
                new TcpService(config, logger).Run();
            }
            
        }
    }
}