using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TSensor.Proxy.Logger;

namespace TSensor.Proxy.Tcp
{
    public class PortListener
    {
        private readonly ILogger _logger;

        private readonly int _port;

        private readonly OutputService outputService;

        private void Log(string message, Elapsed elapsed = null, bool isError = false)
        {
            _logger.Log(message, prefix: _port.ToString(), elapsed, isError);
        }

        private readonly TcpListener listener;

        public PortListener(int port, Config config, ILogger logger, ArchiveService archiveService)
        {
            _port = port;
            _logger = logger;

            outputService = new OutputService(port.ToString(), config, logger, archiveService);

            listener = new TcpListener(IPAddress.Any, port);
        }

        public void Run()
        {
            listener.Start();

            TcpClient client = null;

            try
            {
                while (true)
                {
                    Log("started listening");
                    client = listener.AcceptTcpClient();

                    Log("connection established");
                    var worker = new Thread(new ParameterizedThreadStart(ReadCallback));
                    worker.Start(client);
                }
            }
            catch (Exception e)
            {
                Log("accept error", isError: true);
                Log(e.Message, isError: true);

                listener.Stop();
            }
            finally
            {
                if (client != null)
                {
                    client.Dispose();
                }

                client = null;
            }
        }

        private const int MESSAGE_SIZE = 129;
        public async void ReadCallback(object client)
        {
            var _client = client as TcpClient;

            using var stream = _client.GetStream();

            var buffer = new byte[MESSAGE_SIZE];
            int bytesReceived;

            try
            {
                while ((bytesReceived = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    var content = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

                    Log($"{content.Length} bytes received");

                    if (content.Length == MESSAGE_SIZE)
                    {
                        await outputService.Process(content.Substring(0, 127));
                    }
                }
            }
            catch (Exception e)
            {
                Log("receiving error", isError: true);
                Log(e.Message, isError: true);

                _client.Close();
            }
        }
    }
}