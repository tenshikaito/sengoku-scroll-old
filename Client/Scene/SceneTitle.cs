using Client.UI;
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
            if (MessageBox.Show("edit game world?", "confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                uiEditGameWorldDialog.Visible = false;
                gameSystem.sceneToWaiting();
                gameSystem.formMain.Focus();

                Task.Run(() =>
                {
                    var gwp = new GameWorldProcessor(name);

                    var gw = gwp.load(gameSystem.option.screenWidth, gameSystem.option.screenHeight);

                    dispatcher.invoke(()=> gameSystem.sceneToEditGame(gw));
                });
            }
        }

        private void onDeleteButtonClicked(string name)
        {
            if (MessageBox.Show("delete?", "confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var gwp = new GameWorldProcessor(name);

                try
                {
                    gwp.delete();
                }
                catch (Exception e)
                {
                    MessageBox.Show("create_failed:" + e.Message);
                    return;
                }

                uiEditGameWorldDialog.delete(name);
            }
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

            if (!gwp.createGameWorldDirectory(width, height))
            {
                MessageBox.Show("create_failed");
                return;
            }

            uiCreateGameWorldDialog.Close();
            loadGameWorldMapList();
        }

        private void loadGameWorldMapList()
        {
            uiEditGameWorldDialog.setData(GameWorldProcessor.getGameWorldList());
        }
    }
}
