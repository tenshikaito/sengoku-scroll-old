using Client.Helper;
using Client.Model;
using Client.UI;
using Client.UI.SceneTitle;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.Scene
{
    public class SceneTitle : SceneBase
    {
        private UIMainMenuWindow uiMainMenuWindow;

        private UIUserDialog uiUserDialog;

        private UIStartGameDialog uiStartGameDialog;
        private UIEditGameDialog uiEditGameDialog;

        private UIUserDetailDialog uiUserDetailDialog;
        private UIGameServerDetailDialog uiGameServerDetailDialog;
        private UIGameWorldDetailDialog uiGameWorldDetailDialog;

        public SceneTitle(GameSystem gs) : base(gs)
        {
        }

        public override void start()
        {
            showUserDialog();
        }

        public override void finish()
        {
            uiUserDialog?.Close();
            uiMainMenuWindow?.Close();
        }

        private void showUserDialog()
        {
            if (uiUserDialog == null)
            {
                uiUserDialog = new UIUserDialog(gameSystem)
                {
                    addButtonClicked = onUserAddButtonClicked,
                    removeButtonClicked = onUserRemoveButtonClicked,
                    editButtonClicked = onUserEditButtonClicked,
                    okButtonClicked = onUserOkButtonClicked
                };

                uiUserDialog.setData(gameSystem.user.Select(o => o.name).ToList());
            }

            uiUserDialog.Visible = true;
        }

        private void showMainDialog()
        {
            if (uiMainMenuWindow == null) uiMainMenuWindow = new UIMainMenuWindow(gameSystem, onStartGame, onEditGame, onSelectUser);

            uiMainMenuWindow.Text = gameSystem.currentUser.name;
            uiMainMenuWindow.Visible = true;
        }

        private void onUserAddButtonClicked()
        {
            uiUserDialog.Visible = false;

            uiUserDetailDialog = new UIUserDetailDialog(gameSystem)
            {
                Text = gameSystem.wording.add,
                okButtonClicked = () =>
                {
                    var name = uiUserDetailDialog.name;

                    if (!checkUser(ref name)) return;

                    gameSystem.user.Add(new UserInfo()
                    {
                        name = name,
                        code = Guid.NewGuid().ToString(),
                        servers = new List<ServerInfo>()
                    });

                    FileHelper.saveUserInfo(gameSystem.user);

                    uiUserDialog.setData(gameSystem.user.Select(o => o.name).ToList());

                    uiUserDetailDialog.Close();

                    uiUserDialog.Visible = true;
                },
                cancelButtonClicked = () =>
                {
                    uiUserDetailDialog.Close();

                    uiUserDialog.Visible = true;
                }
            };

            uiUserDetailDialog.Show();
        }

        private void onUserRemoveButtonClicked(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;

            var dialog = new UIConfirmDialog(gameSystem, "confirm", $"remove user {name} ?");

            dialog.okButtonClicked = () =>
            {
                gameSystem.user.RemoveAll(o => o.name == name);

                FileHelper.saveUserInfo(gameSystem.user);

                uiUserDialog.setData(gameSystem.user.Select(o => o.name).ToList());

                dialog.Close();
            };

            dialog.ShowDialog(formMain);
        }

        private void onUserEditButtonClicked(string oldName)
        {
            uiUserDialog.Visible = false;

            uiUserDetailDialog = new UIUserDetailDialog(gameSystem)
            {
                Text = gameSystem.wording.edit,
                okButtonClicked = () =>
                {
                    var newName = uiUserDetailDialog.name;

                    if (!checkUser(ref newName)) return;

                    gameSystem.user.SingleOrDefault(o => o.name == oldName).name = newName;

                    FileHelper.saveUserInfo(gameSystem.user);

                    uiUserDialog.setData(gameSystem.user.Select(o => o.name).ToList());

                    uiUserDetailDialog.Close();

                    uiUserDialog.Visible = true;
                },
                cancelButtonClicked = () =>
                {
                    uiUserDetailDialog.Close();

                    uiUserDialog.Visible = true;
                }
            };

            uiUserDetailDialog.Show();
        }

        private void onUserOkButtonClicked()
        {
            var name = uiUserDialog.name;

            if (string.IsNullOrWhiteSpace(name))
            {
                new UIDialog(gameSystem, "alert", "select a user").ShowDialog(uiUserDialog);

                return;
            }

            var user = gameSystem.user.SingleOrDefault(o => o.name == name);

            if (user == null)
            {
                new UIConfirmDialog(gameSystem, "error", "select a user").ShowDialog(uiUserDialog);

                return;
            }

            gameSystem.currentUser = user;

            uiUserDialog.Visible = false;

            showMainDialog();
        }

        private bool checkUser(ref string name)
        {
            var n = name;

            if (string.IsNullOrWhiteSpace(name = name.Trim())) return false;

            if (gameSystem.user.Any(o => o.name == n))
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
        }

        private void onStartGameAddButtonClicked()
        {
            uiStartGameDialog.Visible = false;

            uiGameServerDetailDialog = new UIGameServerDetailDialog(gameSystem)
            {
                Visible = true,
                okButtonClicked = () =>
                {
                    var (txtName, txtIp, txtPort) = uiGameServerDetailDialog.value;

                    if (!checkServer(ref txtName, ref txtIp, ref txtPort, out var port)) return;

                    gameSystem.currentUser.servers.Add(new ServerInfo()
                    {
                        code = Guid.NewGuid().ToString(),
                        name = txtName,
                        ip = txtIp,
                        port = port
                    });

                    FileHelper.saveUserInfo(gameSystem.user);

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
                okButtonClicked = () =>
                {
                    var (txtName, txtIp, txtPort) = uiGameServerDetailDialog.value;

                    if (!checkServer(ref txtName, ref txtIp, ref txtPort, out var port)) return;

                    var si = gameSystem.currentUser.servers.SingleOrDefault(o => o.code == code);

                    si.name = txtName;
                    si.ip = txtIp;
                    si.port = port;

                    FileHelper.saveUserInfo(gameSystem.user);

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
            var name = gameSystem.currentUser.servers.SingleOrDefault(o => o.code == code).name;

            var dialog = new UIConfirmDialog(gameSystem, "confirm", $"remove {name} ?");

            dialog.okButtonClicked = () =>
            {
                gameSystem.currentUser.servers.RemoveAll(o => o.code == code);

                FileHelper.saveUserInfo(gameSystem.user);

                loadGameServerList();

                dialog.Close();
            };

            dialog.ShowDialog(formMain);
        }

        private void onStartGameRefreshButtonClicked()
        {

        }

        private void onStartGameOkButtonClicked()
        {

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
                okButtonClicked = () =>
                {
                    var (name, width, height) = uiGameWorldDetailDialog.value;

                    onCreateGameWorld(name, width, height);

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

                Task.Run(() =>
                {
                    var gw = new GameWorld(name)
                    {
                        camera = new Camera(gameSystem.option.screenWidth, gameSystem.option.screenHeight),
                    };

                    gw.init();

                    gw.gameWorldProcessor.map.loadMasterData(gw);

                    dispatcher.invoke(() => gameSystem.sceneToEditGame(gw));
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

        private void onCreateGameWorld(string name, string txtWidth, string txtHeight)
        {
            name = name.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("game_world_name");
                return;
            }
            if (!int.TryParse(txtWidth.Trim(), out var width))
            {
                MessageBox.Show("width_number");
                return;
            }
            if (!int.TryParse(txtHeight.Trim(), out var height))
            {
                MessageBox.Show("height_number");
                return;
            }

            if (width < 100 || height < 100)
            {
                MessageBox.Show("map size > 100");
                return;
            }

            var gwp = new GameWorldProcessor(name);

            if (!gwp.map.createDirectory(width, height))
            {
                MessageBox.Show("create_failed");
                return;
            }

            uiGameWorldDetailDialog.Close();

            loadGameWorldMapList();
        }

        private void onSelectUser()
        {
            uiMainMenuWindow.Visible = false;

            showUserDialog();
        }

        private void loadGameServerList() => uiStartGameDialog.setData(gameSystem.currentUser.servers);

        private void loadGameWorldMapList() => uiEditGameDialog.setData(GameWorldProcessor.getMapList());
    }
}
