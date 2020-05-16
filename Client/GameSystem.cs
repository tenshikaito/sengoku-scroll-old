using Client.Scene;
using Library;
using Library.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Client
{
    public class GameSystem
    {
        public FormMain formMain;

        public Option option;
        public Wording wording;

        public GameGraphic gameGraphic;

        public Camera camera;

        public GameObject sceneManager;

        public int screenWidth => option.screenWidth;
        public int screenHeight => option.screenHeight;

        public void init()
        {
            var lines = File.ReadAllLines("charset/system.dat", Encoding.UTF8).Union(File.ReadAllLines("charset/zh-tw.dat", Encoding.UTF8));

            wording = new Wording("zh-tw", lines.Where(o => !o.StartsWith("#") && !string.IsNullOrWhiteSpace(o)).Select(o =>
            {
                var line = o.Split('=');
                return new KeyValuePair<string, string>(line[0], line[1]);
            }).ToDictionary(o => o.Key, o => o.Value));

            camera = new Camera(screenWidth, screenHeight);
        }

        public void sceneToTitle() => sceneManager.switchStatus(new SceneTitle(this));

        public void sceneToEditGame(GameWorld gw)
        {
            sceneManager.switchStatus(new SceneEditGameWorld(this, gw));
        }

        public void sceneToWaiting()
        {
            sceneManager.switchStatus(new SceneWaiting(this, wording.loading));
        }
    }
}
