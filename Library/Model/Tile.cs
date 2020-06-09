using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Model
{
    public struct MainMapTile
    {
        public byte terrain;
        public byte? terrainSurface;
        /// <summary>地区</summary>
        public byte region;
    }

    public struct DetailMapTile
    {
        public byte terrain;
        public byte height;
        public TileFunctionType functionType;
    }

    public enum TileFunctionType : byte
    {
        none = 0,
        agriculture = 1,
        industry = 2,
        business = 3,
        residence = 4
    }
}
