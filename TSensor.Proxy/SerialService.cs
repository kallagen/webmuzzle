using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace TSensor.Proxy
{
    public class SerialService
    {
        private readonly string apiUrl;
        private readonly string deviceGuid;

        private List<SerialPort> portCollection = new List<SerialPort>();

        public SerialService(Config config)
        {
            apiUrl = $"http://{config.ApiHost}/api/sensorvalue/push";
            deviceGuid = config.DeviceGuid;
        }

        private const int MESSAGE_SIZE = 128;

        private void Dispose()
        {
            foreach (var port in portCollection)
            {
                port.DataReceived -= Port_DataReceived;
                port.Close();
            }

            portCollection = new List<SerialPort>();
        }

        private IEnumerable<string> GetUsbComPort()
        {
            return SerialPort.GetPortNames().Where(p => p.Contains("USB"));
        }

        private void Init()
        {
            foreach (var portName in GetUsbComPort())
            {
                var port = new SerialPort(portName)
                {
                    BaudRate = 19200,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    DataBits = 8,
                    Handshake = Handshake.None,
                    RtsEnable = true
                };
                port.DataReceived += Port_DataReceived;

                portCollection.Add(port);

                port.Open();
            }
        }

        private async void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var strData = (sender as SerialPort).ReadLine();
            if (strData.Length == MESSAGE_SIZE)
            {
                try
                {
                    var dateStart = DateTime.Now;

                    var content = await Http.PostAsync(apiUrl, new Dictionary<string, string>
                    {
                       { "v", strData },
                       { "d", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") },
                       { "g", deviceGuid }
                    });

                    var passedMs = (DateTime.Now - dateStart).TotalMilliseconds;

                    Console.WriteLine($"{passedMs}ms: {content}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public void Refresh()
        {
            Dispose();
            Init();
        }
    }
}
