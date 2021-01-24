using System;
using System.IO.Ports;
using System.Threading;
using Modbus.Device;
using TSensor.Proxy.Logger;


namespace TSensor.Proxy.Commands
{
    public class CommandEvaluator
    {
        public static CommandsService.CommandEvalResult Eval(string portName, ParsedCommand command, ILogger _logger)
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

                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                }

                ModbusSerialMaster master = ModbusSerialMaster.CreateAscii(serialPort);
                using (master)
                {
                    _logger.Log($"В порт {command.izkNumber} с начальным адресом: {command.startAddress} будет записано {command.value}");
                    serialPort.WriteTimeout = 5000;
                    serialPort.ReadTimeout = 5000;
                    master.WriteSingleRegister(command.izkNumber, command.startAddress, command.value);
                    _logger.Log("Команда записана");
                }

                return new CommandsService.CommandEvalResult(false);
            }
            catch (Exception e)
            {
                _logger.Log("ОШИБКА при записи: " + e.Message, isError: true);
                return new CommandsService.CommandEvalResult(true, e.Message);
            }
            
        }
        

    }
}