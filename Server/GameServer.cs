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
    public class GameServer
    {
        private TcpListener tcpListener;
        private volatile bool isRunning = false;

        public Action<GameClient> clientConnected;
        public Action<GameClient> clientDisconnected;
        public Action<GameClient, string> dataReceived;

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

            var gc = NetworkHelper.getGameClient(tc);

            gc.dataReceived = (otc, s) => dataReceived?.Invoke(otc, s);
            gc.clientDisconnected = otc => clientDisconnected?.Invoke(otc);

            clientConnected?.Invoke(gc);

            _ = accept();

            gc.start();
        }

        public void stop()
        {
            if (!isRunning) return;

            isRunning = false;

            tcpListener.Stop();
        }
    }
}
