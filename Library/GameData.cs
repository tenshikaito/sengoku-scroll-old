using Library.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Library
{
    public class GameData
    {
        public IncreasedIdDictionary<Force> force;
        public IncreasedIdDictionary<Province> province;
        public IncreasedIdDictionary<Stronghold> stronghold;
    }

    public class MainTileMapData
    {
        public Dictionary<int, int> territory;
        public Dictionary<int, int> road;

        public Dictionary<int, int> stronghold;
        public Dictionary<int, int> unit;
    }

    public class DetailTileMapData
    {
        public int strongholdId;

        public Dictionary<int, int> road;
        public Dictionary<int, TileFunctionType> tileFunctionType;
    }

    public static class MapDataExtension
    {
        public static T? getId<T>(this Dictionary<int, T> map, TileMap tm, MapPoint p) where T : struct
        {
            if (tm.isOutOfBounds(p)) return null;

            if (map.TryGetValue(tm.getIndex(p), out var value)) return value;

            return null;
        }
    }
}
