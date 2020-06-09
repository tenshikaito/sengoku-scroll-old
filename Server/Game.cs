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
        private Dictionary<int, GamePlayer> players = new Dictionary<int, GamePlayer>();
        private ManualResetEvent @lock = new ManualResetEvent(false);
        private bool isRunning = false;

        private Option option;
        private GameServer gameServer;

        public GameWorldMap gameWorldMap;

        public string gameWorldName { get; }

        public Game(Option o, string gameWorldName)
        {
            this.gameWorldName = gameWorldName;

            option = o;

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
            var gw = new GameWorld(gameWorldName);

            gw.init();

            var gwp = gw.gameWorldProcessor;

            gameWorldMap = gwp.game.loadGameData(gwp.game.loadMasterData(gw));

            isRunning = true;

            gameServer.bind(option.port).start();
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

            gameServer?.Dispose();
        }
    }
}
