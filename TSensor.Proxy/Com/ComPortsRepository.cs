using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO.Ports;
using System.Linq;

namespace TSensor.Proxy.Com
{
    public class ComPortsRepository
    {
         public static IList<string> comPortsNames = new List<string>();
         public static bool NamesPortListFilled = false;
         public static bool SerialPortListFilled = false;
         public static IList<SerialPort> SerialPorts = new List<SerialPort>();
         public static IDictionary<string, SerialPort> PortNamesToSerialPorts = new Dictionary<string, SerialPort>();
         
         public static IDictionary<byte, string> IzkNumbersToPortNames = new Dictionary<byte, string>();
         
         public static void FillComPortsNamesList(Config config)
         {
             if (comPortsNames.Count == 0)
             {
                 var tmp = SerialPort.GetPortNames();
                 foreach (var portName in SerialPort.GetPortNames()
                     .Where(p => !config.IsLinux || p.Contains("USB"))
                     .Where(p => !config.COMPortList.Any() || config.COMPortList.Contains(p.ToUpper())))
                 {
                     comPortsNames.Add(portName);
                 }

                 NamesPortListFilled = true;
             }
         }

         public static void FillSerialPorts()
         {
             foreach (var portName in comPortsNames)
             {
                 var serialPort = new SerialPort(portName)
                 {
                     BaudRate = 19200,
                     Parity = Parity.None,
                     StopBits = StopBits.One,
                     DataBits = 8,
                     Handshake = Handshake.None,
                     RtsEnable = true
                 };
                 SerialPorts.Add(serialPort);
                 PortNamesToSerialPorts[portName] = serialPort;
             }

             SerialPortListFilled = true;
         }

         public static void OpenAllPorts()
         {
             foreach (var serialPort in SerialPorts)
                 if (!serialPort.IsOpen)
                     serialPort.Open();
         }
         
         public static void CloseAllPorts()
         {
             foreach (var serialPort in SerialPorts)
                 if (serialPort.IsOpen)
                     serialPort.Close();
         }

         public static void FillAndOpenAll(Config config)
         {
             if (SerialPortListFilled)
             {
                 return;
             }
             FillComPortsNamesList(config);
             FillSerialPorts();
             OpenAllPorts();
         }

         private static void RefreshComPorts()
         {
         }
    }
}