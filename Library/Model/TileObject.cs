using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Model
{
    public class TileObject : BaseObject
    {
        public int id { get; set; }
        public MapPoint location;

        public bool isInvalid { get; set; }

        public static implicit operator MapPoint(TileObject to) => to.location;
    }
}
