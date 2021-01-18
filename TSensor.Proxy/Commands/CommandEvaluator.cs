using System;
using System.IO.Ports;
using System.Threading;
using Modbus.Device;



namespace TSensor.Proxy.Commands
{
    public class CommandEvaluator
    {
        public static CommandsService.CommandEvalResult Eval(string portName, ParsedCommand command)
        {
            try
            {
                SerialPort serialPort = new SerialPort(portName); //Create a new SerialPort object.
                serialPort.BaudRate = 19200;
                serialPort.DataBits = 8;
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.Handshake = Handshake.None;
                serialPort.RtsEnable = true;

                Console.Out.WriteLine("ДО 3");
                if (!serialPort.IsOpen)
                {
                    Console.Out.WriteLine("ВО ВРЕМЯ 3");
                    serialPort.Open();
                }
                Console.Out.WriteLine("ПОСЛЕ 3");

                
                ModbusSerialMaster master = ModbusSerialMaster.CreateAscii(serialPort);
                using (master)
                {
                    Console.Out.WriteLine("До записи в порт: " + command.izkNumber + " " + command.startAddress + " " + command.value);

                    master.WriteSingleRegister(command.izkNumber, command.startAddress, command.value);
                    Console.Out.WriteLine("После Записи в порт");

                }

                return new CommandsService.CommandEvalResult(false);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("ОШИБКА при записи: " + e.Message);
                return new CommandsService.CommandEvalResult(true, e.Message);
            }
            
        }
        

    }
}