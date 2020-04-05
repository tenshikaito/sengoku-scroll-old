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

        public string terrainImageFileName;

        public string tileObjectImageFileName;

        public Dictionary<int, TileAnimation> terrainAnimation;
    }

    public class OuterTileMapImageInfo : TileMapImageInfo
    {
        public Dictionary<int, TileAnimation> strongholdAnimation;
    }

    public class InnerTileMapImageInfo : TileMapImageInfo
    {

    }
}
