using Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class GameWorldData
    {
        public string name { get; }
        public string resourcePackageName { get; set; }

        public GameDate gameDate;

        public TileMap tileMap;

        public MasterData masterData;

        public GameData gameData;

        public GameWorldData(string name)
        {
            this.name = name;
        }

        public class MasterData
        {
            public Dictionary<int, Region> region;
            public Dictionary<int, Culture> culture;
            public Dictionary<int, Religion> religion;
            public Dictionary<int, Road> road;

            public Dictionary<int, Terrain> tileMapTerrain;
            //public Dictionary<int, Stronghold.Type> strongholdType;

            public Dictionary<int, TerrainImage> terrainImage;

            public TileMapImageInfo tileMapViewImageInfo;
            public TileMapImageInfo tileMapDetailImageInfo;
        }

        public class GameData
        {
            public IncreasedIdDictionary<Force> force;
            public IncreasedIdDictionary<Province> province;
            public IncreasedIdDictionary<Stronghold> stronghold;

            public IncreasedIdDictionary<Player> player;
        }
    }
}
