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

        public List<UserInfo> user;
        public UserInfo currentUser;

        public Option option;
        public Wording wording;

        public GameGraphic gameGraphic;

        public Camera camera;

        public GameObject sceneManager;

        public int screenWidth => option.screenWidth;
        public int screenHeight => option.screenHeight;

        public async Task init()
        {
            user = await FileHelper.loadUser<List<UserInfo>>();

            var lines = (await FileHelper.loadLines("charset/system.dat")).Union(await FileHelper.loadLines("charset/zh-tw.dat"));

            wording = new Wording("zh-tw", lines.Where(o => !o.StartsWith("#") && !string.IsNullOrWhiteSpace(o)).Select(o =>
            {
                var line = o.Split('=');
                return new KeyValuePair<string, string>(line[0], line[1]);
            }).ToDictionary(o => o.Key, o => o.Value));

            camera = new Camera(screenWidth, screenHeight);

            formMain.Resize += (s, e) => camera.setSize(formMain.Width, formMain.Height);
        }

        public void sceneToTitle(bool isLogined) => sceneManager.switchStatus(new SceneTitle(this, isLogined));

        public void sceneToEditGame(GameWorld gw) => sceneManager.switchStatus(new SceneEditGameWorld(this, gw));

        public void sceneToWaiting() => sceneManager.switchStatus(new SceneWaiting(this, $"{wording.loading} ..."));
    }
}
