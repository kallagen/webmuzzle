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

        private readonly ArchiveService archiveService;

        public SerialService(Config config, ILogger logger)
        {
            _config = config;
            _logger = logger;

            archiveService = new ArchiveService(_config, _logger);
        }

        public void Run()
        {
            foreach (var portName in SerialPort.GetPortNames()
                .Where(p => !_config.IsLinux || p.Contains("USB")))
            {
                var portListener = new PortListener(portName, _config, _logger, archiveService);
                portListener.Run();
            }

            archiveService.Run();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}