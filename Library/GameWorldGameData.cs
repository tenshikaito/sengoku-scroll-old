using Library.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Library
{
    public class GameWorldGameData
    {
        public IncreasedIdDictionary<Force> force;
        public IncreasedIdDictionary<Province> province;
        public IncreasedIdDictionary<Stronghold> stronghold;
    }

    public class IncreasedIdDictionary<TValue>
    {
        public int id;
        public Dictionary<int, TValue> map;

        public TValue this[int index] => map[index];
    }

    public class GameWorldOuterMapData
    {
        public OuterTileMap data;

        public Dictionary<int, int> territory;
        public Dictionary<int, int> road;

        public Dictionary<int, int> stronghold;
        public Dictionary<int, int> unit;
    }

    public class GameWorldInnerMapData
    {
        public Stronghold currentStronghold;
        public InnerTileMap data;

        public Dictionary<int, int> road;
        public Dictionary<int, TileFunctionType> tileFunctionType;
    }

    public static class GameMapExtension
    {
        public static T? getId<T>(this Dictionary<int, T> map, GameWorldOuterMapData gmd, MapPoint p) where T : struct
        {
            if (gmd.data.isOutOfBounds(p)) return null;

            if (map.TryGetValue(gmd.data.getIndex(p), out var value)) return value;

            return null;
        }

        public static T? getId<T>(this Dictionary<int, T> map, GameWorldInnerMapData gmd, MapPoint p) where T : struct
        {
            if (gmd.data.isOutOfBounds(p)) return null;

            if (map.TryGetValue(gmd.data.getIndex(p), out var value)) return value;

            return null;
        }
    }
}
