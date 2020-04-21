﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using TSensor.Proxy.Logger;

namespace TSensor.Proxy
{
    public class PortListener
    {
        private readonly Config _config;
        private readonly ILogger _logger;
        private readonly ArchiveService _archiveService;

        private readonly SerialPort port;

        private void Log(string message, Elapsed elapsed = null, bool isError = false)
        {
            _logger.Log(message, prefix: port.PortName, elapsed, isError);
        }

        public PortListener(string portName, Config config, ILogger logger, ArchiveService archiveService)
        {
            _config = config;
            _logger = logger;
            _archiveService = archiveService;

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
                using var elapsed = Elapsed.Create;
                var eventDate = elapsed.Start.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff");

                var result = await Http.Http.PostAsync(_config.ApiUrlSendValue,
                    new Dictionary<string, string>
                    {
                            { "v", strData },
                            { "d", eventDate },
                            { "g", _config.DeviceGuid }
                    });

                if (result.Exception != null)
                {
                    Log("sending error", isError: true);
                    Log(result.Exception.Message, isError: true);

                    _archiveService.Write(port.PortName, $"{eventDate};{strData}");
                }
                else
                {
                    Log($"data sended({result.Content})", elapsed);
                }
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
