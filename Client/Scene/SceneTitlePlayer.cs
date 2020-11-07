using Client.Model;
using Client.UI;
using Client.UI.SceneTitle;
using Library;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Scene
{
    public class SceneTitlePlayer : SceneBase
    {
        private UIPlayerDialog uiPlayerDialog;
        private UIPlayerDetailDialog uiPlayerDetailDialog;

        public SceneTitlePlayer(GameSystem gs) : base(gs)
        {
        }

        public override void start()
        {
            uiPlayerDialog = new UIPlayerDialog(gameSystem)
            {
                addButtonClicked = onPlayerAddButtonClicked,
                removeButtonClicked = onPlayerRemoveButtonClicked,
                editButtonClicked = onPlayerEditButtonClicked,
                okButtonClicked = onPlayerOkButtonClicked
            };

            uiPlayerDialog.setData(gameSystem.players.Select(o => o.name).ToList());

            uiPlayerDialog.Visible = true;
        }

        public override void finish()
        {
            uiPlayerDialog?.Close();
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

            gameSystem.sceneToTitleMain();
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
    }
}
