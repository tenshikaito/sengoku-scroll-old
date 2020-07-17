using Client.Command;
using Client.Model;
using Client.UI;
using Client.UI.SceneTitle;
using Library;
using Library.Helper;
using Library.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.Scene
{
    public class SceneTitle : SceneBase
    {
        private UIMainMenuWindow uiMainMenuWindow;

        private UIPlayerDialog uiPlayerDialog;

        private UIStartGameDialog uiStartGameDialog;
        private UIEditGameDialog uiEditGameDialog;

        private UIPlayerDetailDialog uiPlayerDetailDialog;
        private UIGameServerDetailDialog uiGameServerDetailDialog;
        private UIGameWorldDetailDialog uiGameWorldDetailDialog;

        private bool isLogined;

        public SceneTitle(GameSystem gs, bool isLogined = false) : base(gs)
        {
            this.isLogined = isLogined;
        }

        public override void start()
        {
            if (isLogined) showMainDialog();
            else showPlayerDialog();
        }

        public override void finish()
        {
            uiPlayerDialog?.Close();
            uiMainMenuWindow?.Close();
        }

        private void showPlayerDialog()
        {
            if (uiPlayerDialog == null)
            {
                uiPlayerDialog = new UIPlayerDialog(gameSystem)
                {
                    addButtonClicked = onPlayerAddButtonClicked,
                    removeButtonClicked = onPlayerRemoveButtonClicked,
                    editButtonClicked = onPlayerEditButtonClicked,
                    okButtonClicked = onPlayerOkButtonClicked
                };

                uiPlayerDialog.setData(gameSystem.players.Select(o => o.name).ToList());
            }

            uiPlayerDialog.Visible = true;
        }

        private void showMainDialog()
        {
            if (uiMainMenuWindow == null) uiMainMenuWindow = new UIMainMenuWindow(gameSystem, onStartGame, onEditGame, onSelectPlayer);

            uiMainMenuWindow.Text = gameSystem.currentPlayer.name;
            uiMainMenuWindow.Visible = true;
        }

        private void onPlayerAddButtonClicked()
        {
            uiPlayerDialog.Visible = false;

            uiPlayerDetailDialog = new UIPlayerDetailDialog(gameSystem)
            {
                Text = gameSystem.wording.add,
                okButtonClicked = async () =>
                {
                    var name = uiPlayerDetailDialog.name;

                    if (!checkPlayer(ref name)) return;

                    gameSystem.players.Add(new PlayerInfo()
                    {
                        name = name,
                        code = Guid.NewGuid().ToString(),
                        servers = new List<ServerInfo>()
                    });

                    await FileHelper.savePlayer(gameSystem.players);

                    uiPlayerDialog.setData(gameSystem.players.Select(o => o.name).ToList());

                    uiPlayerDetailDialog.Close();

                    uiPlayerDialog.Visible = true;
                },
                cancelButtonClicked = () =>
                {
                    uiPlayerDetailDialog.Close();

                    uiPlayerDialog.Visible = true;
                }
            };

            uiPlayerDetailDialog.Show();
        }

        private void onPlayerRemoveButtonClicked(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;

            var dialog = new UIConfirmDialog(gameSystem, "confirm", $"remove player {name} ?");

            dialog.okButtonClicked = async () =>
            {
                gameSystem.players.RemoveAll(o => o.name == name);

                await FileHelper.savePlayer(gameSystem.players);

                uiPlayerDialog.setData(gameSystem.players.Select(o => o.name).ToList());

                dialog.Close();
            };

            dialog.ShowDialog(formMain);
        }

        private void onPlayerEditButtonClicked(string oldName)
        {
            uiPlayerDialog.Visible = false;

            uiPlayerDetailDialog = new UIPlayerDetailDialog(gameSystem)
            {
                Text = gameSystem.wording.edit,
                okButtonClicked = async () =>
                {
                    var newName = uiPlayerDetailDialog.name;

                    if (!checkPlayer(ref newName)) return;

                    gameSystem.players.SingleOrDefault(o => o.name == oldName).name = newName;

                    await FileHelper.savePlayer(gameSystem.players);

                    uiPlayerDialog.setData(gameSystem.players.Select(o => o.name).ToList());

                    uiPlayerDetailDialog.Close();

                    uiPlayerDialog.Visible = true;
                },
                cancelButtonClicked = () =>
                {
                    uiPlayerDetailDialog.Close();

                    uiPlayerDialog.Visible = true;
                }
            };

            uiPlayerDetailDialog.Show();
        }

        private void onPlayerOkButtonClicked()
        {
            var name = uiPlayerDialog.name;

            if (string.IsNullOrWhiteSpace(name))
            {
                new UIDialog(gameSystem, "alert", "select a player").ShowDialog(formMain);

                return;
            }

            var player = gameSystem.players.SingleOrDefault(o => o.name == name);

            if (player == null)
            {
                new UIConfirmDialog(gameSystem, "error", "select a player").ShowDialog(uiPlayerDialog);

                return;
            }

            gameSystem.currentPlayer = player;

            uiPlayerDialog.Visible = false;

            showMainDialog();
        }

        private bool checkPlayer(ref string name)
        {
            var n = name;

            if (string.IsNullOrWhiteSpace(name = name.Trim())) return false;

            if (gameSystem.players.Any(o => o.name == n))
            {
                new UIDialog(gameSystem, "error", "name existed").ShowDialog();

                return false;
            }

            return true;
        }

        private void onStartGame()
        {
            uiMainMenuWindow.Visible = false;

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
                    uiMainMenuWindow.Visible = true;
                }
            };

            loadGameServerList();

            testServer();
        }

        private void testServer()
        {
            var servers = gameSystem.currentPlayer.servers;

            var map = new ConcurrentDictionary<string, TestServerData>();

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

            uiStartGameDialog.Visible = false;

            var si = gameSystem.currentPlayer.servers.SingleOrDefault(o => o.code == code);

            var s = gameSystem.sceneToWaiting();

            Task.Run(async () =>
            {
                try
                {
                    await JoinGameCommand.execute(si, gameSystem.currentPlayer.code, s, gameSystem);

                    dispatcher.invoke(() => uiStartGameDialog.Close());
                }
                catch (SocketException e)
                when (e.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    MessageBox.Show("server disconnected.");

                    dispatcher.invoke(() =>
                    {
                        uiStartGameDialog.Visible = true;

                        gameSystem.sceneManager.switchStatus(this);
                    });
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

        private void onEditGame()
        {
            uiMainMenuWindow.Visible = false;

            uiEditGameDialog = new UIEditGameDialog(gameSystem)
            {
                Visible = true,
                addButtonClicked = onEditGameAddButtonClicked,
                removeButtonClicked = onEditGameRemoveButtonClicked,
                editButtonClicked = onEditGameEditButtonClicked,
                okButtonClicked = () =>
                {
                    uiEditGameDialog.Close();
                    uiMainMenuWindow.Visible = true;
                }
            };

            loadGameWorldMapList();
        }

        private void onEditGameAddButtonClicked()
        {
            uiEditGameDialog.Visible = false;

            uiGameWorldDetailDialog = new UIGameWorldDetailDialog(gameSystem)
            {
                Visible = true,
                okButtonClicked = async () =>
                {
                    var (name, width, height) = uiGameWorldDetailDialog.value;

                    var r = await onCreateGameWorld(name, width, height);

                    if (!r) return;

                    uiGameWorldDetailDialog.Close();

                    loadGameWorldMapList();

                    uiEditGameDialog.Visible = true;
                },
                cancelButtonClicked = () =>
                {
                    uiGameWorldDetailDialog.Close();
                    uiEditGameDialog.Visible = true;
                }
            };
        }

        private void onEditGameEditButtonClicked(string name)
        {
            var dialog = new UIConfirmDialog(gameSystem, "confirm", "edit game world?");

            dialog.okButtonClicked = () =>
            {
                dialog.Close();

                uiEditGameDialog.Close();

                gameSystem.sceneToWaiting();

                Task.Run(async () =>
                {
                    var gw = new GameWorld(name)
                    {
                        camera = new Camera(gameSystem.option.screenWidth, gameSystem.option.screenHeight),
                    };

                    gw.init();

                    await gw.gameWorldProcessor.map.loadMasterData(gw);

                    gameSystem.dispatchSceneToEditGame(gw);
                });
            };

            dialog.ShowDialog(formMain);
        }

        private void onEditGameRemoveButtonClicked(string name)
        {
            var dialog = new UIConfirmDialog(gameSystem, "confirm", "delete?");

            dialog.okButtonClicked = () =>
            {
                dialog.Close();

                var gwp = new GameWorldProcessor(name);

                try
                {
                    gwp.map.deleteDirectory();
                }
                catch (Exception e)
                {
                    MessageBox.Show("create_failed:" + e.Message);
                    return;
                }

                loadGameWorldMapList();
            };

            dialog.ShowDialog(formMain);
        }

        private async ValueTask<bool> onCreateGameWorld(string name, string txtWidth, string txtHeight)
        {
            name = name.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                new UIDialog(gameSystem, "error", "game_world_name").ShowDialog(formMain);
                return false;
            }
            if (!int.TryParse(txtWidth.Trim(), out var width))
            {
                new UIDialog(gameSystem, "error", "width_number").ShowDialog(formMain);
                return false;
            }
            if (!int.TryParse(txtHeight.Trim(), out var height))
            {
                new UIDialog(gameSystem, "error", "height_number").ShowDialog(formMain);
                return false;
            }

            if (width < 100 || height < 100)
            {
                new UIDialog(gameSystem, "error", "map size > 100").ShowDialog(formMain);
                return false;
            }

            var gwp = new GameWorldProcessor(name);

            if (!await gwp.map.createDirectory(width, height))
            {
                new UIDialog(gameSystem, "error", "create_failed").ShowDialog(formMain);
                return false;
            }

            return true;
        }

        private void onSelectPlayer()
        {
            uiMainMenuWindow.Visible = false;

            showPlayerDialog();
        }

        private void loadGameServerList() => uiStartGameDialog.setData(gameSystem.currentPlayer.servers);

        private void loadGameWorldMapList() => uiEditGameDialog.setData(GameWorldProcessor.getMapList());
    }
}
