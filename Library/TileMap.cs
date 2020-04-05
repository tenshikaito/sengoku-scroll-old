using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class TileMap : Map
    {
        public Tile[] tiles;

        public Tile? this[int index] => isOutOfBounds(index) ? (Tile?)null : tiles[index];

        public Tile? this[MapPoint p] => this[getIndex(p)];

        public TileMap(Size s) : base(s)
        {
        }

        public void each(Action<int, Tile?> foreachCallback) => each(0, count, foreachCallback);

        public void each(int startIndex, int length, Action<int, Tile?> foreachCallback)
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
