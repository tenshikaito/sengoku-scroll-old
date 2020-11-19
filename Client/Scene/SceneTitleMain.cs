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
            uiMainMenuWindow = new UIMainMenuWindow(gameSystem, onStartGame, onEditGame, onSelectPlayer)
            {
                Text = gameSystem.currentPlayer.name,
                Visible = true
            };
        }

        public override void finish()
        {
            uiMainMenuWindow?.Close();
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
