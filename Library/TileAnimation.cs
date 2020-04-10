using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class TileAnimation
    {
        public int id;
        public float interval;
        public List<Frame> frames;

        public class Frame
        {
            public string fileName;
            public Point vertex;
        }
    }
}
