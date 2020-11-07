using Library;
using Library.Helper;
using Library.Network;
using Server.Command;
using Server.Game;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class GameService
    {
        public ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        public GameWorld gameWorld;

        private HashSet<GameClient> connectedClient = new HashSet<GameClient>();
        private Dictionary<int, GamePlayer> onlinePlayer = new Dictionary<int, GamePlayer>();

        private ConcurrentQueue<KeyValuePair<GameClient, string>> messages = new ConcurrentQueue<KeyValuePair<GameClient, string>>();

        private ManualResetEventSlim messageLock = new ManualResetEventSlim(false);

        private CommandProcessor serverCommandProcessor;
        private CommandProcessor gameCommandProcessor;

        private bool isRunning = false;

        private Option option;
        private GameServer gameServer;

        public string gameWorldName { get; }

        public GameService(Option o, string gameWorldName)
        {
            this.gameWorldName = gameWorldName;

            option = o;

            serverCommandProcessor = new CommandProcessor(serverCommandMap);
            gameCommandProcessor = new CommandProcessor(gameCommandMap);

            gameServer = new GameServer()
            {
                clientConnected = onClientConnected,
            };
        }

        private Dictionary<string, CommandBase> serverCommandMap => new Dictionary<string, CommandBase>()
        {
            { nameof(TestServerCommand), new TestServerCommand(this) },
            { nameof(JoinGameCommand), new JoinGameCommand(this) }
        };

        private Dictionary<string, CommandBase> gameCommandMap => new Dictionary<string, CommandBase>()
        {
        };

        private void onClientConnected(GameClient gc)
        {
            lock (connectedClient)
            {
                connectedClient.Add(gc);
            }

            Task.Run(async () =>
            {
                try
                {
                    var (hasResult, data) = await gc.read();

                    var cmd = data.fromCommandString();

                    var r = await serverCommandProcessor.execute(gc, cmd.name, cmd.data);

                    if (!r)
                    {
                        Debug.WriteLine("unknown command: " + cmd.name);

                        disconnect(gc);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            });
        }

        private void onClientDisconnected(GameClient gc) => disconnect(gc);

        private void onDataReceived(GameClient gc, string data)
        {
            messages.Enqueue(new KeyValuePair<GameClient, string>(gc, data));

            messageLock.Set();
        }

        public GameService bind(int port)
        {
            gameServer.bind(port);

            return this;
        }

        public async void start()
        {
            isRunning = true;

            var gw = new GameWorld(gameWorldName);

            var gwp = gw.gameWorldManager;

            var gm = await gwp.game.loadGameWorldData();

            gw.init(gm);

            gameWorld = gw;

            _ = Task.Run(processMessage);

            gameServer.bind(option.port).start();
        }

        private async void processMessage()
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

                    var r = await gameCommandProcessor.execute(msg.Key, cmd.name, cmd.data);

                    if (!r)
                    {
                        Debug.WriteLine("unknown command: " + cmd.name);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

                if (messages.IsEmpty) messageLock.Reset();
            }
        }

        public void disconnect(GameClient gc)
        {
            lock (connectedClient)
            {
                connectedClient.Remove(gc);
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
