using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Graphic
{
    public class SpriteText : SpriteBase
    {
        public string text;

        public float interval;

        public Font font;

        public float fontSize;

        public SpriteText()
        {
        }

        public SpriteText(SpriteBase parent) : base(parent)
        {
        }
    }
}
