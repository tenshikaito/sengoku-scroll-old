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
    public class Game : IDisposable
    {
        private ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();
        private bool isRunning = false;

        private GameServer gameServer;

        public GameWorldMap gameWorldMap;

        public string gameWorldName { get; }

        public Game(string gameWorldName)
        {
            this.gameWorldName = gameWorldName;

            gameServer = new GameServer()
            {
                clientConnected = onClientConnected,
                clientDisconnected = onClientDisconnected,
                dataReceived = onDataReceived
            };
        }

        private void onClientConnected(TcpClient tc)
        {
            throw new NotImplementedException();
        }

        private void onClientDisconnected(TcpClient tc)
        {
            throw new NotImplementedException();
        }

        private void onDataReceived(TcpClient tc, string data)
        {
            throw new NotImplementedException();
        }

        public Game bind(int port)
        {
            gameServer.bind(port);

            return this;
        }

        public void start()
        {
            var gwp = new GameWorldProcessor(gameWorldName);

            gameWorldMap = gwp.loadMasterData(new GameWorldMap());

            isRunning = true;

            gameServer.start();
        }

        private void processData(GameClient gc, string data)
        {
            Console.WriteLine(gc.getIp() + ":" + data);
        }

        public void stop()
        {
            if (!isRunning) return;

            isRunning = false;

            gameServer.stop();
        }

        public void Dispose()
        {
            stop();
        }
    }
}
