using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class GameWorldMap
    {
        public MasterData masterData = new MasterData();

        public GameData gameData = new GameData();

        public GameDate gameDate = new GameDate();

        public MainTileMap mainTileMap;

        public MainTileMapData mainTileMapData;

        public DetailTileMap detailTileMap;

        public DetailTileMapData detailTileMapData;
    }
}
