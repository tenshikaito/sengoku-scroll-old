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

        private ConcurrentQueue<KeyValuePair<GameClient, string>> messages = new ConcurrentQueue<KeyValuePair<GameClient, string>>();

        private ManualResetEventSlim messageLock = new ManualResetEventSlim(false);

        private GameCommandProcessor gameCommandProcessor;

        private bool isRunning = false;

        private Option option;
        private GameServer gameServer;

        public string gameWorldName { get; }

        public Game(Option o, string gameWorldName)
        {
            this.gameWorldName = gameWorldName;

            option = o;

            gameCommandProcessor = new GameCommandProcessor(this);

            connectedPlayer.init();

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
                    id = connectedPlayer.getNextId(),
                    gameClient = gc,
                };

                connectedPlayer[p.id] = p;
            }
        }

        private void onClientDisconnected(GameClient gc) => disconnect(gc);

        private void onDataReceived(GameClient gc, string data)
        {
            messages.Enqueue(new KeyValuePair<GameClient, string>(gc, data));

            messageLock.Set();
        }

        public Game bind(int port)
        {
            gameServer.bind(port);

            return this;
        }

        public void start()
        {
            isRunning = true;

            var gw = new GameWorld(gameWorldName);

            gw.init();

            var gwp = gw.gameWorldProcessor;

            gameWorld = gwp.game.loadGameData(gwp.game.loadMasterData(gw));

            Task.Run(processMessage);

            gameServer.bind(option.port).start();
        }

        private void processMessage()
        {
            while (isRunning)
            {
                messageLock.Wait();

                if (!isRunning) break;

                if (!messages.TryDequeue(out var msg))
                {
                    messageLock.Reset();

                    continue;
                }

                try
                {
                    var cmd = msg.Value.fromCommandString();

                    gameCommandProcessor.execute(msg.Key, cmd.name, cmd.data).Wait();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                if (messages.IsEmpty) messageLock.Reset();
            }
        }

        public void disconnect(GameClient gc)
        {
            lock (connectedPlayer)
            {
                var p = connectedPlayer.map.Values.FirstOrDefault(o => o.gameClient == gc);

                if (p != null) connectedPlayer.map.Remove(p.id);
            }

            lock (onlinePlayer)
            {
                var p = onlinePlayer.Values.FirstOrDefault(o => o.gameClient == gc);

                if (p != null) onlinePlayer.Remove(p.id);
            }
        }

        public void stop()
        {
            if (!isRunning) return;

            isRunning = false;

            messageLock.Set();
            gameServer.stop();
        }
    }
}
