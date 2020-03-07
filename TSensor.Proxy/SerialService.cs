using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TSensor.Proxy
{
    public class SerialService
    {
        private readonly Config _config;
        private readonly Logger _logger;

        private readonly object Locker = new object();
        private List<string> portCollection = new List<string>();

        private Timer checkPortTimer;

        public SerialService(Config config, Logger logger)
        {
            _config = config;
            _logger = logger;

            checkPortTimer = new Timer(new TimerCallback(CheckPort), portCollection, 0, _config.CheckPortInterval);
        }

        private void CheckPort(object state)
        {
            IEnumerable<string> ports;
            using (var elapsed = Elapsed.Create)
            {
                ports = SerialPort.GetPortNames().Where(p => p.Contains("USB"));
                _logger.Write($"actual ports data received: {ports.Count()} ports", elapsed);
            }

            foreach (var portName in ports)
            {
                if (portCollection.Any(p => p == portName))
                {
                    _logger.Write($"{portName}: already initialized");
                }
                else
                {
                    Task.Run(() =>
                    {
                        lock (Locker)
                        {
                            portCollection.Add(portName);
                        }

                        new PortListener(portName, _config, _logger).Run();

                        lock (Locker)
                        {
                            portCollection.Remove(portName);
                        }
                    });
                }
            }

            //todo remove not existed ports
        }

        public void Run()
        {
            while (true) { }
        }
    }
}