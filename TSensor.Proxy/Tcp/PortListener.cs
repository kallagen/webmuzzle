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

        private void LogDebug(string message)
        {
            Log(message);
        }

        private readonly ManualResetEventSlim acceptDoneEvent = new ManualResetEventSlim(false);

        private Socket listener;

        public PortListener(int port, Config config, ILogger logger, ArchiveService archiveService)
        {
            _port = port;
            _logger = logger;

            outputService = new OutputService(port.ToString(), config, logger, archiveService);

            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Any, port));
        }

        public void Run()
        {
            listener.Listen(20);

            while (true)
            {
                Log("started listening");

                acceptDoneEvent.Reset();

                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                acceptDoneEvent.Wait();
            }
        }

        public void AcceptCallback(IAsyncResult result)
        {
            Log("connection established");
            acceptDoneEvent.Set();

            var listener = (result.AsyncState as Socket).EndAccept(result);

            var state = new PortReceiveState
            {
                Listener = listener
            };
            listener.BeginReceive(state.Buffer, 0, PortReceiveState.BUFFER_SIZE, 0,
                new AsyncCallback(ReadCallback), state);
        }

        private const int MESSAGE_SIZE = 129;
        public async void ReadCallback(IAsyncResult result)
        {
            try
            {
                var state = result.AsyncState as PortReceiveState;
                var listener = state.Listener;

                var bytesRead = listener.EndReceive(result);
                if (bytesRead > 0)
                {
                    var content = Encoding.ASCII.GetString(state.Buffer, 0, bytesRead);
                    LogDebug(content);
                    Log($"{content.Length} bytes received");

                    if (content.Length == MESSAGE_SIZE)
                    {
                        await outputService.Process(content.Substring(0, 127));
                    }
                }

                listener.BeginReceive(state.Buffer, 0, PortReceiveState.BUFFER_SIZE, 0,
                    new AsyncCallback(ReadCallback), state);
            }
            catch (Exception e)
            {
                Log("receiving error", isError: true);
                Log(e.Message, isError: true);
            }
        }
    }
}