using System.Net.Sockets;

namespace TSensor.Proxy.Tcp
{
    public class PortReceiveState
    {
        public const int BUFFER_SIZE = 129;

        public byte[] Buffer { get; set; } = new byte[BUFFER_SIZE];
        public Socket Listener { get; set; }
    }
}
