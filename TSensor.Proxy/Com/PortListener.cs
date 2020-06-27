using System;
using System.IO.Ports;
using TSensor.Proxy.Logger;

namespace TSensor.Proxy.Com
{
    public class PortListener
    {
        private readonly ILogger _logger;

        private readonly SerialPort port;

        private readonly OutputService outputService;

        private void Log(string message, Elapsed elapsed = null, bool isError = false)
        {
            _logger.Log(message, prefix: port.PortName, elapsed, isError);
        }

        public PortListener(string portName, Config config, ILogger logger, ArchiveService archiveService)
        {
            _logger = logger;

            outputService = new OutputService(portName, config, logger, archiveService);

            port = new SerialPort(portName)
            {
                BaudRate = 19200,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };

            port.DataReceived += Port_DataReceived;
            port.ErrorReceived += Port_ErrorReceived;
        }

        private const int MESSAGE_SIZE = 128;
        private async void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var port = sender as SerialPort;

            var strData = port.ReadLine();
            Log($"{strData.Length} bytes received");

            if (strData.Length == MESSAGE_SIZE)
            {
                await outputService.Process(strData);
            }
        }

        private void Port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            try
            {
                var port = sender as SerialPort;

                port.DataReceived -= Port_DataReceived;
                port.ErrorReceived -= Port_ErrorReceived;
            }
            catch { }

            Log("error received", isError: true);
        }

        public void Run()
        {
            try
            {
                port.Open();
                Log("is open");
            }
            catch (Exception ex)
            {
                Log("opening error", isError: true);
                Log(ex.Message, isError: true);
            }
        }
    }
}
