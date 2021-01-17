using System;
using System.Globalization;
using System.IO.Ports;
using System.Threading.Tasks;
using TSensor.Proxy.Commands;
using TSensor.Proxy.Logger;

namespace TSensor.Proxy.Com
{
    public class PortListener
    {
        private readonly ILogger _logger;
        private readonly SerialPort _serialPort;
        private readonly OutputService _outputService;
        private readonly string _portName;
        
        public static bool Flag2 = false;
        public static bool FlagPortTableReady = false;

        private void Log(string message, Elapsed elapsed = null, bool isError = false)
        {
            _logger.Log(message, prefix: _serialPort.PortName, elapsed, isError);
        }

        public PortListener(string portName, Config config, ILogger logger, ArchiveService archiveService, SerialPort serialSerialPort)
        {
            _logger = logger;
            _outputService = new OutputService(portName, config, logger, archiveService);
            _serialPort = serialSerialPort;
            _portName = portName;
            
            _serialPort.DataReceived += SerialPortDataReceived;
            _serialPort.ErrorReceived += SerialPortErrorReceived;
            SerialPortDataReceived(_serialPort, null); //TODO gavr убрать
        }

        private const int MESSAGE_SIZE = 128;
        private const int MESSAGE_SIZE2 = 6;
        
        private async void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var port = sender as SerialPort;
            if (port == null) return;
            
            if (!port.IsOpen)
            {
                port.Open();
            }
            
            if (CommandsService.Flag1 == false)
            {
                Flag2 = false;
                
                var strData = port.ReadLine();
                Log($"{strData.Length} bytes received");
                
                if (strData.Length == MESSAGE_SIZE)
                {
                    var byteIzkNum = byte.Parse(strData.Substring(1, 2), NumberStyles.HexNumber);
                    ComPortsRepository.IzkNumbersToPortNames[byteIzkNum] = _portName;
                    _logger.Log($"Добавлено соотношение izkNum: {byteIzkNum} к {_portName}");
                    FlagPortTableReady = true;
                    await _outputService.Process(strData);
                }
            }
            else
            {
                Flag2 = true;
                _logger.Log("Flag1 is true, Flag2 setted to true");
            }
               
            
        }
        
        private async void SerialPortDataReceived2(object sender, SerialDataReceivedEventArgs e)
        {
            var port = sender as SerialPort;
            if (port == null) return;
            
            if (!port.IsOpen)
            {
                port.Open();
            }
            
            if (CommandsService.Flag1 == false)
            {
                Flag2 = false;
                
                // var strData = port.ReadLine();
                var strData = ":50asd";
                Log($"{strData.Length} bytes received");
                
                if (strData.Length <= MESSAGE_SIZE2)
                {
                    var byteIzkNum = byte.Parse(strData.Substring(1, 2), NumberStyles.HexNumber);
                    ComPortsRepository.IzkNumbersToPortNames[byteIzkNum] = _portName;
                    FlagPortTableReady = true;
                    await _outputService.Process(strData);
                }
            }
            else
            {
                Flag2 = true;
                _logger.Log("Flag1 is true, Flag2 setted to true");
            }
               
            Task.Delay(10000).ContinueWith(t=> SerialPortDataReceived2(port, null) );
        }

        private void SerialPortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            try
            {
                var port = sender as SerialPort;

                port.DataReceived -= SerialPortDataReceived;
                port.ErrorReceived -= SerialPortErrorReceived;
            }
            catch { }

            Log("error received", isError: true);
        }

        public void Run()
        {
            try
            {
                _serialPort.Open();
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
