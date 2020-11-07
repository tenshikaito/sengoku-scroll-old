using Client.Game;
using Client.Model;
using Client.Scene;
using Library;
using Library.Helper;
using Library.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class GameSystem
    {
        public FormMain formMain;

        public List<PlayerInfo> players;
        public PlayerInfo currentPlayer;

        public Option option;
        public Wording wording;

        public GameGraphic gameGraphic;

        public Camera camera;

        public GameObject sceneManager;

        public int screenWidth => option.screenWidth;
        public int screenHeight => option.screenHeight;

        public void init()
        {
            camera = new Camera(screenWidth, screenHeight);

            formMain.Resize += (s, e) => camera.setSize(formMain.Width, formMain.Height);
        }

        public SceneTitlePlayer sceneToTitlePlayer()
        {
            var s = new SceneTitlePlayer(this);

            sceneManager.switchStatus(s);

            return s;
        }

        public SceneTitleMain sceneToTitleMain()
        {
            var s = new SceneTitleMain(this);

            sceneManager.switchStatus(s);

            return s;
        }

        public SceneTitleStartGame sceneToTitleStartGame()
        {
            var s = new SceneTitleStartGame(this);

            sceneManager.switchStatus(s);

            return s;
        }

        public SceneTitleEditGame sceneToTitleEditGame()
        {
            var s = new SceneTitleEditGame(this);

            sceneManager.switchStatus(s);

            return s;
        }

        public SceneGame sceneToGame(GameWorld gw)
        {
            var s = new SceneGame(this, gw);

            sceneManager.switchStatus(s);

            return s;
        }

        public SceneEditGameWorld sceneToEditGame(GameWorld gw)
        {
            var s = new SceneEditGameWorld(this, gw);

            sceneManager.switchStatus(s);

            return s;
        }

        public SceneWaiting sceneToWaiting()
        {
            var s = new SceneWaiting(this, $"{wording.loading} ...");

            sceneManager.switchStatus(s);

            return s;
        }

        public void dispatchSceneToGame(GameWorld gw) => formMain.dispatcher.invoke(() => sceneToGame(gw));

        public void dispatchSceneToEditGame(GameWorld gw) => formMain.dispatcher.invoke(() => sceneToEditGame(gw));
    }
}
