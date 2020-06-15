using Library;
using Library.Helper;
using Library.Network;
using Server.Command;
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
    public class Game
    {
        public ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        public GameWorld gameWorld;

        private IncreasedIdDictionary<GamePlayer> connectedPlayer = new IncreasedIdDictionary<GamePlayer>();
        private Dictionary<int, GamePlayer> onlinePlayer = new Dictionary<int, GamePlayer>();

        private bool isRunning = false;

        private Option option;
        private GameServer gameServer;

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

        private void onClientConnected(GameClient gc)
        {
            lock (connectedPlayer)
            {
                var p = new GamePlayer()
                {
                    id = connectedPlayer.getNextId()
                };

                connectedPlayer[p.id] = p;
            }
        }

        private void onClientDisconnected(GameClient gc)
        {
            lock (connectedPlayer)
            {
                var p = connectedPlayer.map.Values.FirstOrDefault(o => o.gameClient.ip == gc.ip);

                if (p != null) connectedPlayer.map.Remove(p.id);
            }

            lock (onlinePlayer)
            {
                var p = onlinePlayer.Values.FirstOrDefault(o => o.gameClient.ip == gc.ip);

                if (p != null) onlinePlayer.Remove(p.id);
            }
        }

        private void onDataReceived(GameClient gc, string data)
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

            gameWorld = gwp.game.loadGameData(gwp.game.loadMasterData(gw));

            isRunning = true;

            gameServer.bind(option.port).start();
        }

        private void processData(GameClient gc, string data)
        {
            Console.WriteLine(gc.ip + ":" + data);
        }

        public void stop()
        {
            if (!isRunning) return;

            isRunning = false;

            gameServer.stop();
        }
    }
}
