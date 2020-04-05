using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Graphic
{
    public class Sprite : SpriteBase
    {
        public Image bitmap { get; set; }

        public Rectangle? sourceRectangle { get; set; }

        public Sprite()
        {

        }
    }
}
