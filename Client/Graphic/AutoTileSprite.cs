using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Client.Graphic.AutoTileCalculator;

namespace Client.Graphic
{
    public class AutoTileSprite
    {
        private int tileWidth, tileHeight, innerTileWidth, innerTileHeight;

        public Sprite sprite1 = new Sprite();
        public Sprite sprite2 = new Sprite();
        public Sprite sprite3 = new Sprite();
        public Sprite sprite4 = new Sprite();

        public Image bitmap
        {
            get => sprite1.bitmap;
            set
            {
                sprite1.bitmap = value;
                sprite2.bitmap = value;
                sprite3.bitmap = value;
                sprite4.bitmap = value;
            }
        }

        public Point position
        {
            get => sprite1.position;
            set
            {
                sprite1.position = value;
                sprite2.position = new Point(value.X + innerTileWidth, value.Y);
                sprite3.position = new Point(value.X, value.Y + innerTileHeight);
                sprite4.position = new Point(value.X + innerTileWidth, value.Y + innerTileHeight);
            }
        }

        public Color color
        {
            get => sprite1.color;
            set
            {
                sprite1.color = value;
                sprite2.color = value;
                sprite3.color = value;
                sprite4.color = value;
            }
        }

        public AutoTileSprite(TileMapSpritesBase m)
        {
            tileWidth = m.tileWidth;
            tileHeight = m.tileHeight;
            innerTileWidth = tileWidth / 2;
            innerTileHeight = tileHeight / 2;
        }

        public void refresh(Image bitmap, Point p, byte different)
        {
            fillSprite(bitmap, p, sprite1, cTopLeft, different);
            fillSprite(bitmap, p, sprite2, cTopRight, different);
            fillSprite(bitmap, p, sprite3, cBottomLeft, different);
            fillSprite(bitmap, p, sprite4, cBottomRight, different);
        }

        private void fillSprite(Image bitmap, Point p, Sprite s, ICalculator c, byte different)
        {
            var (x, y) = c.calculate(p.X, p.Y, tileWidth, tileHeight, innerTileWidth, innerTileHeight, different);

            s.bitmap = bitmap;
            s.sourceRectangle = new Rectangle(x, y, innerTileWidth, innerTileHeight);
        }
    }
}
