using Client.Graphic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Client
{
    public class GameGraphic
    {
        public Graphics g;

        public string defaultFontName;
        public float defaultFontSize;

        private Dictionary<Color, SolidBrush> solidBrushMap = new Dictionary<Color, SolidBrush>();

        private Dictionary<string, Pen> penMap = new Dictionary<string, Pen>();

        private Dictionary<string, Font> fontMap = new Dictionary<string, Font>();

        public GameGraphic()
        {

        }

        private Pen getPen(Color c, float width)
        {
            var key = $"{c.ToString()}_{width}";

            if (!penMap.TryGetValue(key, out var value)) penMap[key] = value = new Pen(c, width);

            return value;
        }

        private SolidBrush getSolidBrush(Color c)
        {
            if (!solidBrushMap.TryGetValue(c, out var value)) solidBrushMap[c] = value = new SolidBrush(c);

            return value;
        }

        private Font getFont(string fontName, float fontSize, FontStyle fs = FontStyle.Regular)
        {
            var key = $"{fontName}_{fontSize}_{fs.ToString()}";

            if (!fontMap.TryGetValue(key, out var value)) fontMap[key] = value = new Font(fontName, fontSize, fs);

            return value;
        }

        public Font getDefaultFont() => getFont(defaultFontName, defaultFontSize);

        public void drawRactangle(Color c, Rectangle r, float width = 1)
        {
            g.DrawRectangle(getPen(c, width), r);
        }

        public void fillRactangle(Color c, Rectangle r)
        {
            g.FillRectangle(getSolidBrush(c), r);
        }

        public void drawImage(Image img, Point tr, Rectangle sr) => g.DrawImage(img, tr.X, tr.Y, sr, GraphicsUnit.Pixel);

        public void drawImage(Image img, int x, int y, Rectangle sr) => g.DrawImage(img, x, y, sr, GraphicsUnit.Pixel);

        public SizeF measureDefaultText(SpriteText s) => g.MeasureString(s.text, s.font);

        public void drawText(SpriteText s) => g.DrawString(s.text, s.font, getSolidBrush(s.color), s.displayPosition);

        public void drawSprite(AutoTileSprite ts)
        {
            drawSprite(ts.sprite1);
            drawSprite(ts.sprite2);
            drawSprite(ts.sprite3);
            drawSprite(ts.sprite4);
        }

        public void drawSprite(Sprite s)
        {
            if (s.sourceRectangle != null)
            {
                var sr = s.sourceRectangle.Value;

                s.sourceRectangle = new Rectangle(sr.X, sr.Y, sr.Width, sr.Height);
                //s.sourceRectangle = new Rectangle(sr.X - 1, sr.Y - 1, sr.Width + 1, sr.Height + 1);
            }

            drawImage(s.bitmap, s.displayPosition, s.sourceRectangle ?? new Rectangle(0, 0, s.bitmap.Width, s.bitmap.Height));
        }
    }
}
