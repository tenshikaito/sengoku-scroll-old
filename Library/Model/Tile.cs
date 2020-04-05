using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Model
{
    public struct Tile
    {
        public byte terrain;
        /// <summary>地区</summary>
        public byte region;
    }

    public enum TileFunctionType : byte
    {
        None = 0,
        Agriculture = 1,
        Industry = 2,
        Business = 3,
        Residence = 4
    }
}
