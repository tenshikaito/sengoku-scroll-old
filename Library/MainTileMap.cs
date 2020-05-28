using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class MainTileMap : TileMap
    {
        public MainMapTile[] tiles;
        public Dictionary<int, byte> terrain;

        public MainMapTile? this[int index] => isOutOfBounds(index) ? (MainMapTile?)null : tiles[index];

        public MainMapTile? this[MapPoint p] => this[getIndex(p)];

        public MainTileMap(Size s) : base(s)
        {
        }

        public void each(Action<int, MainMapTile?> foreachCallback) => each(0, count, foreachCallback);

        public void each(int startIndex, int length, Action<int, MainMapTile?> foreachCallback)
        {
            for (int i = startIndex, l = count; i < length; ++i)
            {
                if (i >= l) break;
                foreachCallback(i, this[i]);
            }
        }

        public void setTerrain(MapPoint p, byte id, bool isSurface = true)
        {
            var index = getIndex(p);

            if (isSurface) terrain[index] = id;

            else tiles[index].terrain = id;
        }

        public (MainMapTile? mt, byte? tid) getTerrain(MapPoint p)
        {
            var t = this[p];

            var tt = terrain.TryGetValue(getIndex(p), out var value) ? (byte?)value : null;

            return (t, tt);
        }
    }
}
