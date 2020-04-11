using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class OuterTileMap : TileMap
    {
        public OuterMapTile[] tiles;

        public OuterMapTile? this[int index] => isOutOfBounds(index) ? (OuterMapTile?)null : tiles[index];

        public OuterMapTile? this[MapPoint p] => this[getIndex(p)];

        public OuterTileMap(Size s) : base(s)
        {
        }

        public void each(Action<int, OuterMapTile?> foreachCallback) => each(0, count, foreachCallback);

        public void each(int startIndex, int length, Action<int, OuterMapTile?> foreachCallback)
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
