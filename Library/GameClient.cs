using Library.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class GameClient
    {
        public TcpClient tcpClient;
        public NetworkStream networkStream;
        public Action<GameClient> clientDisconnected;
        public Action<GameClient, string> dataReceived;

        private Encoding encoding;
        private List<byte> receiveDataBytes;
        private List<byte> sendDataBytes;
        private byte[] buffer;

        public GameClient(TcpClient tc, Encoding e, int dataBufferSize = 10240, int socketBufferSize = 4096)
        {
            tcpClient = tc;

            encoding = e;
            receiveDataBytes = new List<byte>(dataBufferSize);
            sendDataBytes = new List<byte>(dataBufferSize);
            buffer = new byte[socketBufferSize];
        }

        public void start()
        {
            networkStream = tcpClient.GetStream();

            _ = receive();
        }

        private async Task receive()
        {
            var length = await networkStream.ReadAsync(buffer, 0, buffer.Length);

            if (length > 0)
            {
                receiveDataBytes.AddRange(buffer.Take(length));

                _ = receive();
            }
            else
            {
                clientDisconnected?.Invoke(this);

                disconnect();
            }

            processData();
        }

        private void processData()
        {
            var list = receiveDataBytes;

            if (list.Count < 4) return;

            var length = BitConverter.ToInt32(list.Take(4).ToArray(), 0);

            if (list.Count - 4 >= length)
            {
                list.RemoveRange(0, 4);

                var data = list.Take(length).ToArray();

                list.RemoveRange(0, length);

                var s = encoding.GetString(data);

                dataReceived?.Invoke(this, s);

                processData();
            }
        }

        public async Task write(string data)
        {
            sendDataBytes.Clear();

            var ns = networkStream;

            var buffer = BitConverter.GetBytes(data.Length);

            sendDataBytes.AddRange(buffer);

            buffer = encoding.GetBytes(data);

            sendDataBytes.AddRange(buffer);

            await ns.WriteAsync(sendDataBytes.ToArray(), 0, sendDataBytes.Count);

            await ns.FlushAsync();
        }

        public void disconnect()
        {
            networkStream.Close();
            tcpClient.Close();
        }

        public string ip => tcpClient.getIp();

        public static implicit operator TcpClient(GameClient gc) => gc.tcpClient;

        public static implicit operator NetworkStream(GameClient gc) => gc.networkStream;
    }
}
