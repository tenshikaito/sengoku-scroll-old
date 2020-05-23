using Client.Graphic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UI
{
    public class UITileInfoPanel : GameObject
    {
        private GameSystem gameSystem;
        private GameWorld gameWorld;

        private SpriteRectangle background;
        private SpriteText terrainName;
        private SpriteText regionName;
        private SpriteText strongholdName;

        public UITileInfoPanel(GameSystem gs, GameWorld gw, Point position)
        {
            gameSystem = gs;
            gameWorld = gw;

            var g = gs.gameGraphic;

            background = new SpriteRectangle()
            {
                position = position,
                color = Color.FromArgb(64, Color.Black),
                size = new Size(450, 36),
                isFill = true
            };

            terrainName = new SpriteText()
            {
                position = new Point(position.X + 16, position.Y + 8),
                color = Color.White,
                font = g.getDefaultFont(),
                fontSize = g.defaultFontSize,
                text = "terrainName"
            };

            regionName = new SpriteText()
            {
                position = new Point(position.X + 144, position.Y + 8),
                color = Color.White,
                font = g.getDefaultFont(),
                fontSize = g.defaultFontSize,
                text = "regionName"
            };

            strongholdName = new SpriteText()
            {
                position = new Point(position.X + 272, position.Y + 8),
                color = Color.White,
                font = g.getDefaultFont(),
                fontSize = g.defaultFontSize,
                text = "strongholdName"
            };
        }

        public void setText(string terrainName, string regionName, string strongholdName)
        {
            this.terrainName.text = terrainName;
            this.regionName.text = regionName;
            this.strongholdName.text = strongholdName;
        }

        public override void draw()
        {
            var g = gameSystem.gameGraphic;

            g.drawRectangle(background);
            g.drawText(terrainName);
            g.drawText(regionName);
            g.drawText(strongholdName);
        }
    }
}
