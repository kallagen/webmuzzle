using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace TSensor.Proxy
{
    public class PortListener : IDisposable
    {
        private readonly Logger _logger;
        private readonly Config _config;

        private string _portName;
        private SerialPort port;

        private bool IsRunning;

        private void Log(string message, Elapsed elapsed = null)
        {
            _logger.Write(message, elapsed: elapsed, prefix: $"{_portName}:");
        }

        public PortListener(string portName, Config config, Logger logger)
        {
            _config = config;
            _logger = logger;

            _portName = portName;

            using (var elapsed = Elapsed.Create)
            {
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

                Log("initialized", elapsed);
            }
        }

        private const int MESSAGE_SIZE = 128;
        private async void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var port = sender as SerialPort;

            var strData = port.ReadLine();
            Log($"{strData.Length} bytes received");

            if (strData.Length == MESSAGE_SIZE)
            {
                try
                {
                    using (var elapsed = Elapsed.Create)
                    {
                        var content = await Http.PostAsync(_config.ApiUrl, new Dictionary<string, string>
                        {
                            { "v", strData },
                            { "d", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") },
                            { "g", _config.DeviceGuid }
                        });

                        Log("data sended", elapsed);
                    }
                }
                catch (Exception ex)
                {
                    Log("sending error");
                    _logger.Write(ex);
                }
            }
        }

        private void Port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            port.DataReceived -= Port_DataReceived;
            port.ErrorReceived -= Port_ErrorReceived;

            if (port.IsOpen)
            {
                try
                {
                    port.Close();
                    Log("closed");
                }
                catch (Exception ex)
                {
                    Log("closing error");
                    _logger.Write(ex);
                }
            }
            else
            {
                Log("already closed");
            }

            IsRunning = false;
            Log("disposed");
        }

        public void Run()
        {
            try
            {
                port.Open();
                IsRunning = true;

                Log("is open");
            }
            catch (Exception ex)
            {
                Log("opening error");
                _logger.Write(ex);

                Dispose();
            }

            while (IsRunning) { }
        }
    }
}
