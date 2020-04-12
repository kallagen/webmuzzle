using System.IO.Ports;
using System.Linq;
using System.Threading;
using TSensor.Proxy.Logger;

namespace TSensor.Proxy
{
    public class SerialService
    {
        private readonly Config _config;
        private readonly ILogger _logger;

        public SerialService(Config config, ILogger logger)
        {
            _config = config;
            _logger = logger;
        }

        public void Run()
        {
            foreach (var portName in SerialPort.GetPortNames().Where(p => p.Contains("USB")))
            {
                var portListener = new PortListener(portName, _config, _logger);
                portListener.Run();
            }

            Thread.Sleep(Timeout.Infinite);
        }
    }
}