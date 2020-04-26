using Library.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Library.Network
{
    public class GameClient
    {
        public TcpClient tcpClient;
        public NetworkStream networkStream;

        private List<byte> dataBytes = new List<byte>(2048);

        public GameClient(TcpClient tc)
        {
            tcpClient = tc;

            networkStream = tc.GetStream();
        }

        public void write(string data)
        {
            dataBytes.Clear();

            var ns = networkStream;

            var buffer = BitConverter.GetBytes(data.Length);

            dataBytes.AddRange(buffer);

            buffer = Encoding.UTF8.GetBytes(data);

            dataBytes.AddRange(buffer);

            ns.Write(dataBytes.ToArray(), 0, data.Length);
        }

        public string getIp() => tcpClient.getIp();

        public static implicit operator TcpClient(GameClient gc) => gc.tcpClient;

        public static implicit operator NetworkStream(GameClient gc) => gc.networkStream;
    }
}
