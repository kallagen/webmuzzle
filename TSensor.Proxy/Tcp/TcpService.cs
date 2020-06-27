using System.Threading;
using System.Threading.Tasks;
using TSensor.Proxy.Logger;

namespace TSensor.Proxy.Tcp
{
    public class TcpService
    {
        private readonly Config _config;
        private readonly ILogger _logger;

        private readonly ArchiveService archiveService;

        public TcpService(Config config, ILogger logger)
        {
            _config = config;
            _logger = logger;

            archiveService = new ArchiveService(_config, _logger);
        }

        public void Run()
        {
            foreach (var port in _config.TCPPortList)
            {
                Task.Run(() =>
                {
                    var portListener = new PortListener(port, _config, _logger, archiveService);
                    portListener.Run();
                });                
            }

            if (_config.IsApiOutputMode)
            {
                archiveService.Run();
            }

            Thread.Sleep(Timeout.Infinite);
        }
    }
}