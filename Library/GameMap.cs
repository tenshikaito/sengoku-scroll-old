using Library.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Library
{
    public class GameOuterMap
    {
        public TileMap data;

        public Dictionary<int, int> territory;
        public Dictionary<int, int> road;

        public Dictionary<int, int> stronghold;
        public Dictionary<int, int> unit;

        public Dictionary<int, OuterMapImageInfo> tileMapImageInfo;

        public class OuterMapImageInfo : TileMapImageInfo
        {
            public Dictionary<int, Point> stronghold;
        }
    }

    public class GameInnerMap
    {
        public Stronghold currentStronghold;
        public TileMap data;

        public Dictionary<int, int> road;
        public Dictionary<int, TileFunctionType> tileFunctionType;

        public Dictionary<int, InnerMapImageInfo> tileMapImageInfo;

        public class InnerMapImageInfo : TileMapImageInfo
        {

        }
    }

    public static class GameMapExtension
    {
        public static T? getId<T>(this Dictionary<int, T> map, GameOuterMap gm, MapPoint p) where T : struct
        {
            if (gm.data.isOutOfBounds(p)) return null;

            if (map.TryGetValue(gm.data.getIndex(p), out var value)) return value;

            return null;
        }

        public static T? getId<T>(this Dictionary<int, T> map, GameInnerMap gm, MapPoint p) where T : struct
        {
            if (gm.data.isOutOfBounds(p)) return null;

            if (map.TryGetValue(gm.data.getIndex(p), out var value)) return value;

            return null;
        }
    }
}
