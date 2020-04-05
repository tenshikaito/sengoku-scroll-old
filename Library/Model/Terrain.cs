using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Library.Model
{
    /// <summary>地形样式，同一种类型可能有不同样式</summary>
    public class Terrain : BaseObject
    {
        public byte id;
        public string code;
        public string name;
        public bool isGrass;
        public bool isHill;
        public bool isMountain;
        public bool isHarbour;
        public bool isWater;
        public bool isFreshWater;
        public bool isDeepWater;
    }
}
