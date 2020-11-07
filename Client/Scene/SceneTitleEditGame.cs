using Client.Game;
using Client.UI;
using Client.UI.SceneTitle;
using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.Scene
{
    public class SceneTitleEditGame : SceneBase
    {
        private UIEditGameDialog uiEditGameDialog;

        private UIGameWorldDetailDialog uiGameWorldDetailDialog;

        public SceneTitleEditGame(GameSystem gs) : base(gs)
        {
        }

        public override void start()
        {
            onEditGame();
        }

        public override void finish()
        {
            uiEditGameDialog.Close();
        }

        private void onEditGame()
        {
            uiEditGameDialog = new UIEditGameDialog(gameSystem)
            {
                Visible = true,
                addButtonClicked = onEditGameAddButtonClicked,
                removeButtonClicked = onEditGameRemoveButtonClicked,
                editButtonClicked = onEditGameEditButtonClicked,
                okButtonClicked = () =>
                {
                    uiEditGameDialog.Close();
                    gameSystem.sceneToTitleMain();
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
                    uiGameWorldDetailDialog.Cursor = Cursors.WaitCursor;

                    uiGameWorldDetailDialog.isBtnOkEnabled = uiGameWorldDetailDialog.isBtnCancelEnabled = false;

                    var (name, width, height) = uiGameWorldDetailDialog.value;

                    var r = await onCreateGameWorld(name, width, height);

                    uiGameWorldDetailDialog.isBtnOkEnabled = uiGameWorldDetailDialog.isBtnCancelEnabled = true;

                    uiGameWorldDetailDialog.Cursor = Cursors.Default;

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

                    gw.init(await gw.gameWorldManager.map.loadGameWorldData());

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

                var gwp = new GameWorldManager(name);

                try
                {
                    gwp.map.delete();
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

            if (width < Constant.mapMinSize || height < Constant.mapMinSize)
            {
                new UIDialog(gameSystem, "error", $"map size > {Constant.mapMinSize}").ShowDialog(formMain);
                return false;
            }

            if (width > Constant.mapMaxSize || height > Constant.mapMaxSize)
            {
                new UIDialog(gameSystem, "error", $"map size < {Constant.mapMaxSize}").ShowDialog(formMain);
                return false;
            }

            var gwp = new GameWorldManager(name);

            if (!await gwp.map.create(width, height))
            {
                new UIDialog(gameSystem, "error", "create_failed").ShowDialog(formMain);
                return false;
            }

            return true;
        }

        private void loadGameWorldMapList() => uiEditGameDialog.setData(GameWorldManager.getMapList());
    }
}
