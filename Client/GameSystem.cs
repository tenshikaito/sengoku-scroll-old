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
        public int screenWidth;
        public int screenHeight;

        public FormMain formMain;

        public Option option;
        public Wording wording;

        public Cache cache = new Cache();

        public GameGraphic gameGraphic;

        public Camera camera;

        public GameObject sceneManager;
        //public SceneTitle sceneTitle;
        //public SceneEditGameWorld sceneEditGameWorld;
        //public SceneOuterMap sceneOuterMap;
        //public SceneInnerMap sceneInnerMap;

        public void init()
        {
            option = new Option();

            var lines = File.ReadAllLines("Data/Charset/system.dat", Encoding.UTF8).Union(File.ReadAllLines("Data/Charset/zh-tw.dat", Encoding.UTF8));

            wording = new Wording("zh-tw", lines.Where(o => !o.StartsWith("#") && !string.IsNullOrWhiteSpace(o)).Select(o =>
            {
                var line = o.Split('=');
                return new KeyValuePair<string, string>(line[0], line[1]);
            }).ToDictionary(o => o.Key, o => o.Value));

            camera = new Camera(screenWidth, screenHeight);
        }

        public void sceneToTitle() => sceneManager.switchStatus(new SceneTitle(this));

        public void initGame()
        {
            //var tm = new TileMap(new TileMap.Size(1000))
            //{
            //    tiles = new Tile[1000 * 1000]
            //};
            //var s = new Stronghold()
            //{
            //    id = 1,
            //    name = "城堡1",
            //    location = new MapPoint(1, 1)
            //};
            //var unit = new Troop()
            //{
            //    id = 1,
            //    name = "移民队",
            //    type = Troop.Type.Sattler,
            //    hp = 1000,
            //    location = new MapPoint(0, 0),
            //    commander = 1
            //};

            //var gm = new GameOuterMap()
            //{
            //    data = tm,
            //    stronghold = new Dictionary<int, int>()
            //    {
            //        { tm.getIndex(s), s.id }
            //    },
            //    unit = new Dictionary<int, int>()
            //    {
            //        { tm.getIndex(unit), unit.id }
            //    }
            //};
            //var gw = new GameWorld("test")
            //{
            //    gameMap = gm,
            //    gameWorldMasterData = new GameWorldMasterData()
            //    {
            //        stronghold = new IncreasedIdDictionary<Stronghold>()
            //        {
            //            map = new Dictionary<int, Stronghold>()
            //            {
            //                { s.id, s }
            //            }
            //        }
            //    },
            //    camera = new Camera(screenWidth, screenHeight),
            //    currentCharacter = new Character()
            //    {
            //        id = 1,
            //        name = "test",
            //        status = Character.Status.unit,
            //        unitId = 1
            //    }
            //};

            //sceneOuterMap = new SceneOuterMap(this, gw);
            //sceneInnerMap = new SceneInnerMap(this, gw);
        }

        public void sceneToEditGame(GameWorld gw)
        {
            sceneManager.switchStatus(new SceneEditGameWorld(this, gw));
        }

        public void sceneToWaiting()
        {
            sceneManager.switchStatus(new SceneWaiting(this, "now loading..."));
        }

        public void sceneToOuterMap()
        {
            //sceneManager.switchStatus(sceneOuterMap);
        }

        public void sceneToInnerMap(Stronghold s)
        {
            //sceneInnerMap.setMap(new GameInnerMap()
            //{
            //    currentStronghold = s,
            //    data = new TileMap(new TileMap.Size(1000))
            //    {
            //        tiles = new Tile[1000 * 1000]
            //    }
            //});

            //sceneManager.switchStatus(sceneInnerMap);
        }
    }
}
