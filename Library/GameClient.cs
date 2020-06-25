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

        public async Task connect(string ip, int port)
        {
            await tcpClient.ConnectAsync(ip, port);
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
            var r = NetworkHelper.splitDataStream(receiveDataBytes);

            if (r.hasResult)
            {
                dataReceived?.Invoke(this, r.data);

                processData();
            }
        }

        public async Task write(string data)
        {
            sendDataBytes.Clear();

            var ns = networkStream;

            var r = NetworkHelper.combineDataStream(sendDataBytes, data);

            await ns.WriteAsync(r, 0, r.Length);

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
