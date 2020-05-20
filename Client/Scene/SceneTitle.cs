using Client.Helper;
using Client.UI;
using Client.UI.SceneTitle;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.Scene
{
    public class SceneTitle : SceneBase
    {
        private UIMainMenuWindow uiMainMenuWindow;
        private UIEditGameWorldDialog uiEditGameWorldDialog;
        private UICreateGameWorldDialog uiCreateGameWorldDialog;

        public SceneTitle(GameSystem gs) : base(gs)
        {
        }

        public override void start()
        {
            uiMainMenuWindow = new UIMainMenuWindow(gameSystem, onStartGame, onEditGame)
            {
                Visible = true
            };
        }

        public override void finish()
        {
            uiMainMenuWindow.Close();
        }

        private void onStartGame()
        {
        }

        private void onEditGame()
        {
            uiMainMenuWindow.Visible = false;

            uiEditGameWorldDialog = new UIEditGameWorldDialog(gameSystem, onAddButtonClicked, onDeleteButtonClicked, onEditGameWorldButtonClicked)
            {
                Visible = true,
                okButtonClicked = () =>
                {
                    uiEditGameWorldDialog.Close();
                    uiMainMenuWindow.Visible = true;
                }
            };

            loadGameWorldMapList();
        }

        private void onAddButtonClicked()
        {
            uiEditGameWorldDialog.Visible = false;

            uiCreateGameWorldDialog = new UICreateGameWorldDialog(gameSystem, onCreateGameWorld)
            {
                Visible = true,
                okButtonClicked = () => uiEditGameWorldDialog.Visible = true,
                cancelButtonClicked = () =>
                {
                    uiCreateGameWorldDialog.Close();
                    uiEditGameWorldDialog.Visible = true;
                }
            };
        }

        private void onEditGameWorldButtonClicked(string name)
        {
            var dialog = new UIConfirmDialog(gameSystem, "confirm", "edit game world?");

            dialog.okButtonClicked = () =>
            {
                dialog.Close();

                uiEditGameWorldDialog.Close();

                gameSystem.sceneToWaiting();

                Task.Run(() =>
                {
                    var gw = new GameWorld(name)
                    {
                        camera = new Camera(gameSystem.option.screenWidth, gameSystem.option.screenHeight),
                    };

                    var gwp = new GameWorldProcessor(name);

                    gwp.loadMapMasterData(gw);

                    gw.init();

                    gw.gameDate.addDay(90);

                    dispatcher.invoke(() => gameSystem.sceneToEditGame(gw));
                });
            };

            dialog.ShowDialog(formMain);
        }

        private void onDeleteButtonClicked(string name)
        {
            var dialog = new UIConfirmDialog(gameSystem, "confirm", "delete?", formMain);

            dialog.okButtonClicked = () =>
            {
                dialog.Close();

                var gwp = new GameWorldProcessor(name);

                try
                {
                    gwp.deleteMapMasterData();
                }
                catch (Exception e)
                {
                    MessageBox.Show("create_failed:" + e.Message);
                    return;
                }

                uiEditGameWorldDialog.delete(name);
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

            if (!gwp.createMapDirectory(width, height))
            {
                MessageBox.Show("create_failed");
                return;
            }

            uiCreateGameWorldDialog.Close();

            loadGameWorldMapList();
        }

        private void loadGameWorldMapList()
        {
            uiEditGameWorldDialog.setData(GameWorldProcessor.getMapList());
        }
    }
}
