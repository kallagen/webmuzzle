using System;
using System.IO.Ports;
using System.Threading;
using Modbus.Device;



namespace TSensor.Proxy.Commands
{
    public class CommandEvaluator
    {
        public static void Eval(string command, out string error)
        {
            try
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                error = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                //TODO
                bool wasError = false;
                error = wasError ? "error" : null;
            }
        }

        public void RestartController(string portName)
        {
            SerialPort serialPort = new SerialPort(portName); //Create a new SerialPort object.
            serialPort.BaudRate = 19200;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.Handshake = Handshake.None;
            serialPort.RtsEnable = true;
          
            serialPort.Open();
            
            ModbusSerialMaster master = ModbusSerialMaster.CreateAscii(serialPort);
            
            // byte slaveID = 1;
            ushort startAddress = 0;
            ushort numOfPoints = 1;
            // master.WriteSingleRegister(slaveID, startAddress, 65535);

            // ushort[] holding_register = master.ReadHoldingRegisters(slaveID, startAddress,
            //     numOfPoints);
        }


    }
}