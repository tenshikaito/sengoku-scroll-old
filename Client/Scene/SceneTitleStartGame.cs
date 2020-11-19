using Client.Command;
using Client.Model;
using Client.UI;
using Client.UI.SceneTitle;
using Library.Helper;
using Library.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.Scene
{
    public class SceneTitleStartGame : SceneBase
    {
        private UIStartGameDialog uiStartGameDialog;

        private UIGameServerDetailDialog uiGameServerDetailDialog;

        private ConcurrentDictionary<string, TestServerData> map = new ConcurrentDictionary<string, TestServerData>();

        public SceneTitleStartGame(GameSystem gs) : base(gs)
        {
        }

        public override void start()
        {
            onStartGame();
        }

        public override void finish()
        {
            uiStartGameDialog?.Close();
        }

        private void onStartGame()
        {
            uiStartGameDialog = new UIStartGameDialog(gameSystem)
            {
                Visible = true,
                addButtonClicked = onStartGameAddButtonClicked,
                removeButtonClicked = onStartGameRemoveButtonClicked,
                editButtonClicked = onStartGameEditButtonClicked,
                refreshButtonClicked = onStartGameRefreshButtonClicked,
                okButtonClicked = onStartGameOkButtonClicked,
                cancelButtonClicked = () =>
                {
                    uiStartGameDialog.Close();
                    gameSystem.sceneToTitleMain();
                }
            };

            loadGameServerList();

            testServer();
        }

        private void testServer()
        {
            map.Clear();

            var servers = gameSystem.currentPlayer.servers;

            uiStartGameDialog.setData(servers);

            servers.ForEach(async o =>
            {
                try
                {
                    await TestServerCommand.send(o, map, uiStartGameDialog, dispatcher);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            });
        }

        private void onStartGameAddButtonClicked()
        {
            uiStartGameDialog.Visible = false;

            uiGameServerDetailDialog = new UIGameServerDetailDialog(gameSystem)
            {
                Visible = true,
                okButtonClicked = async () =>
                {
                    var (txtName, txtIp, txtPort) = uiGameServerDetailDialog.value;

                    if (!checkServer(ref txtName, ref txtIp, ref txtPort, out var port)) return;

                    gameSystem.currentPlayer.servers.Add(new ServerInfo()
                    {
                        code = Guid.NewGuid().ToString(),
                        name = txtName,
                        ip = txtIp,
                        port = port
                    });

                    await FileHelper.savePlayer(gameSystem.players);

                    loadGameServerList();

                    uiGameServerDetailDialog.Close();

                    uiStartGameDialog.Visible = true;
                },
                cancelButtonClicked = () =>
                {
                    uiGameServerDetailDialog.Close();

                    uiStartGameDialog.Visible = true;
                }
            };
        }

        private void onStartGameEditButtonClicked(string code)
        {
            uiStartGameDialog.Visible = false;

            uiGameServerDetailDialog = new UIGameServerDetailDialog(gameSystem)
            {
                Visible = true,
                okButtonClicked = async () =>
                {
                    var (txtName, txtIp, txtPort) = uiGameServerDetailDialog.value;

                    if (!checkServer(ref txtName, ref txtIp, ref txtPort, out var port)) return;

                    var si = gameSystem.currentPlayer.servers.SingleOrDefault(o => o.code == code);

                    si.name = txtName;
                    si.ip = txtIp;
                    si.port = port;

                    await FileHelper.savePlayer(gameSystem.players);

                    loadGameServerList();

                    uiGameServerDetailDialog.Close();

                    uiStartGameDialog.Visible = true;
                },
                cancelButtonClicked = () =>
                {
                    uiGameServerDetailDialog.Close();

                    uiStartGameDialog.Visible = true;
                }
            };
        }

        private void onStartGameRemoveButtonClicked(string code)
        {
            var name = gameSystem.currentPlayer.servers.SingleOrDefault(o => o.code == code).name;

            var dialog = new UIConfirmDialog(gameSystem, "confirm", $"remove {name} ?");

            dialog.okButtonClicked = async () =>
            {
                gameSystem.currentPlayer.servers.RemoveAll(o => o.code == code);

                await FileHelper.savePlayer(gameSystem.players);

                loadGameServerList();

                dialog.Close();
            };

            dialog.ShowDialog(formMain);
        }

        private void onStartGameRefreshButtonClicked()
        {
            testServer();
        }

        private void onStartGameOkButtonClicked()
        {
            var code = uiStartGameDialog.selectedValue;

            if (code == null) return;

            if(!map.TryGetValue(code, out var ov) || ov == null || ov.ping < 0)
            {
                MessageBox.Show("server disconnected.");

                return;
            }

            var si = gameSystem.currentPlayer.servers.SingleOrDefault(o => o.code == code);

            var s = gameSystem.sceneToWaiting();

            Task.Run(async () =>
            {
                try
                {
                    await JoinGameCommand.execute(si, gameSystem.currentPlayer, s, gameSystem);
                }
                catch (SocketException e)
                when (e.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    MessageBox.Show("server disconnected.");

                    dispatcher.invoke(() => gameSystem.sceneToTitleStartGame());
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            });
        }

        private bool checkServer(ref string txtName, ref string txtIp, ref string txtPort, out int port)
        {
            port = 0;

            if (string.IsNullOrWhiteSpace(txtName = txtName.Trim()))
            {
                MessageBox.Show("game_server_name");
                return false;
            }

            if (!IPAddress.TryParse(txtIp = txtIp.Trim(), out _))
            {
                MessageBox.Show("ip");
                return false;
            }

            if (!int.TryParse(txtPort.Trim(), out port))
            {
                MessageBox.Show("port");
                return false;
            }

            return true;
        }

        private void loadGameServerList() => uiStartGameDialog.setData(gameSystem.currentPlayer.servers);
    }
}
