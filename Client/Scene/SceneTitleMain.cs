using Client.UI.SceneTitle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Scene
{
    public class SceneTitleMain : SceneBase
    {
        private UIMainMenuWindow uiMainMenuWindow;

        public SceneTitleMain(GameSystem gs) : base(gs)
        {
        }

        public override void start()
        {
            showMainDialog();
        }

        public override void finish()
        {
            uiMainMenuWindow?.Close();
        }

        public void showMainDialog()
        {
            if (uiMainMenuWindow == null) uiMainMenuWindow = new UIMainMenuWindow(gameSystem, onStartGame, onEditGame, onSelectPlayer);

            uiMainMenuWindow.Text = gameSystem.currentPlayer.name;
            uiMainMenuWindow.Visible = true;
        }

        private void onStartGame()
        {
            gameSystem.sceneToTitleStartGame();
        }

        private void onEditGame()
        {
            gameSystem.sceneToTitleEditGame();
        }

        private void onSelectPlayer()
        {
            gameSystem.sceneToTitlePlayer();
        }
    }
}
