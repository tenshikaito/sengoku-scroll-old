using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class TileMapImageInfo
    {
        public int id;

        public string name;

        public Size tileSize;

        public Dictionary<byte, TileAnimation> terrainAnimation;
    }

    public class MainTileMapImageInfo : TileMapImageInfo
    {
        public Dictionary<int, TileAnimation> strongholdAnimation;
    }

    public class DetailTileMapImageInfo : TileMapImageInfo
    {

    }
}
