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

        public TileMap tileMap;

        //public DetailTileMap detailTileMap;

        //public DetailTileMapData detailTileMapData;

        public GameWorldMap(string name)
        {
            this.name = name;
        }
    }
}
