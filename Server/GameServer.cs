using Library;
using Library.Helper;
using Library.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class GameServer : IDisposable
    {
        private Dictionary<string, TcpClient> connectedGameClients = new Dictionary<string, TcpClient>();
        private Encoding encoding = Encoding.UTF8;

        private TcpListener tcpListener;

        public bool isRunning { get; private set; } = false;
        public Action<TcpClient> clientConnected;
        public Action<TcpClient> clientDisconnected;
        public Action<TcpClient, string> dataReceived;

        public GameServer bind(int port)
        {
            tcpListener = new TcpListener(IPAddress.Any, port);

            return this;
        }

        public void start()
        {
            tcpListener.Start();

            isRunning = true;

            _ = accept();
        }

        private async Task accept()
        {
            var tc = await tcpListener.AcceptTcpClientAsync();

            if (!isRunning) return;

            connectedGameClients[tc.getIp()] = tc;

            clientConnected?.Invoke(tc);

            var ns = tc.GetStream();
            var list = new List<byte>(10240);
            var buffer = new byte[4096];

            _ = accept();

            _ = receive(tc, ns, list, buffer);
        }

        private async Task receive(TcpClient tc, NetworkStream ns, List<byte> list, byte[] buffer)
        {
            var length = await ns.ReadAsync(buffer, 0, buffer.Length);

            if (length > 0)
            {
                list.AddRange(buffer.Take(length));

                if (isRunning) _ = receive(tc, ns, list, buffer);
            }
            else
            {
                disconnect(tc, false);

                clientDisconnected?.Invoke(tc);
            }

            processData(tc, list);
        }

        private void processData(TcpClient tc, List<byte> list)
        {
            if (list.Count < 4) return;

            var length = BitConverter.ToInt32(list.Take(4).ToArray(), 0);

            if (list.Count - 4 >= length)
            {
                list.RemoveRange(0, 4);

                var data = list.Take(length).ToArray();

                list.RemoveRange(0, length);

                var s = encoding.GetString(data);

                dataReceived?.Invoke(tc, s);
                
                processData(tc, list);
            }
        }

        public void disconnect(TcpClient tc, bool isAutoDisconnect = true)
        {
            var ip = tc.getIp();

            connectedGameClients.Remove(ip);

            if (isAutoDisconnect) tc.Close();
        }

        public void stop()
        {
            if (!isRunning) return;

            isRunning = false;

            tcpListener.Stop();
        }

        public void Dispose()
        {
            stop();
        }
    }
}
