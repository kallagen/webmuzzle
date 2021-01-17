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

                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                }
                
                ModbusSerialMaster master = ModbusSerialMaster.CreateAscii(serialPort);
                using (master)
                {
                    master.WriteSingleRegister(command.izkNumber, command.startAddress, command.value);
                }

                return new CommandsService.CommandEvalResult(false);
            }
            catch (Exception e)
            {
                return new CommandsService.CommandEvalResult(true, e.Message);
            }
            
        }
        
        // public void RestartController(string portName, CommandsService.ParsedCommand parsedCommand , byte izkNumber, ushort startAddress, ushort numOfPoints)
        // {
        //     SerialPort serialPort = new SerialPort(portName); //Create a new SerialPort object.
        //     serialPort.BaudRate = 19200;
        //     serialPort.DataBits = 8;
        //     serialPort.Parity = Parity.None;
        //     serialPort.StopBits = StopBits.One;
        //     serialPort.Handshake = Handshake.None;
        //     serialPort.RtsEnable = true;
        //   
        //     serialPort.Open();
        //     
        //     ModbusSerialMaster master = ModbusSerialMaster.CreateAscii(serialPort);
        //     
        //     byte slaveID = izkNumber;
        //     // ushort startAddress = 0;
        //     // ushort numOfPoints = 1;
        //     master.WriteSingleRegister(slaveID, startAddress, 65535);
        //
        //     // ushort[] holding_register = master.ReadHoldingRegisters(slaveID, startAddress,
        //     //     numOfPoints);
        // }


    }
}