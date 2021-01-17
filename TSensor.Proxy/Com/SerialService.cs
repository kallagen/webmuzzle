using System.IO.Ports;
using System.Linq;
using System.Threading;
using TSensor.Proxy.Logger;

namespace TSensor.Proxy.Com
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
            if (!ComPortsRepository.NamesPortListFilled)
            {
                ComPortsRepository.FillComPortsNamesList(_config);
            }
            
            foreach (var portKeyValuePair in ComPortsRepository.PortNamesToSerialPorts)
            {
                var portListener = new PortListener(portKeyValuePair.Key, _config, _logger, archiveService, portKeyValuePair.Value);
                portListener.Run();
            }

            if (_config.IsApiOutputMode)
            {
                archiveService.Run();
            }

            Thread.Sleep(Timeout.Infinite);
        }
    }
}