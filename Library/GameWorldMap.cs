using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class GameWorldMap
    {
        public string name { get; }
        public string resourcePackageName { get; set; }

        public GameDate gameDate;

        public MasterData masterData;

        public GameData gameData;

        public MainTileMap mainTileMap;

        //public DetailTileMap detailTileMap;

        //public DetailTileMapData detailTileMapData;

        public GameWorldProcessor gameWorldProcessor { get; private set; }
        public GameResourceProcessor gameResourceProcessor { get; private set; }

        public GameWorldMap(string name)
        {
            this.name = name;
        }

        public void init()
        {
            gameWorldProcessor = new GameWorldProcessor(name);
            gameResourceProcessor = new GameResourceProcessor(resourcePackageName);
        }
    }
}
