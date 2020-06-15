using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class DetailTileMap : TileMap
    {
        public DetailMapTile[] tiles;

        public int id;

        public Dictionary<int, int> road;
        public Dictionary<int, TileFunctionType> tileFunctionType;

        public DetailMapTile this[int index] => tiles[index];

        public DetailMapTile this[MapPoint p] => this[getIndex(p)];

        public DetailTileMap(Size s) : base(s)
        {
        }

        public void each(Action<int, DetailMapTile> foreachCallback) => each(0, count, foreachCallback);

        public void each(int startIndex, int length, Action<int, DetailMapTile> foreachCallback)
        {
            for (int i = startIndex, l = count; i < length; ++i)
            {
                if (i >= l) break;
                foreachCallback(i, this[i]);
            }
        }

        public void setTerrain(MapPoint p, byte id) => tiles[getIndex(p)].terrain = id;
    }
}
