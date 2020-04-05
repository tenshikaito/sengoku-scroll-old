using Library;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static Library.GameWorldInnerMapData;

namespace Client.Graphic
{
    public class InnerTileMapSprites : TileMapSpritesBase
    {
        private InnerTileMapImageInfo tileMapImageInfo;
        protected override Map map => gameMapData.data;
        protected TileMap tileMap => gameMapData.data;

        public override int tileWidth => tileMapImageInfo.tileSize.Width;

        public override int tileHeight => tileMapImageInfo.tileSize.Height;

        public InnerTileMapSprites(GameSystem gs, GameWorld gw, InnerTileMapImageInfo mii, bool isEditor = false)
            : base(gs, gw, mii.terrainImageFileName, mii.tileObjectImageFileName, isEditor)
        {
            tileMapImageInfo = mii;

            resize();
        }

        protected override void draw(GameGraphic g, int fx, int fy, MapPoint p, int sx, int sy)
        {
            if (tileMap.isOutOfBounds(p)) return;

            var tile = (Tile)tileMap[p];

            drawTile(g, sx, sy, tile);

            if (!isEditor)
            {
                var playerLocation = gameWorld.currentCharacter.location;

                if (playerLocation.x == fx && playerLocation.y == fy) drawCurrentCharacter(g, sx, sy);
            }
        }

        public void drawTile(GameGraphic g, int x, int y, Tile t)
        {
            switch (t.terrain)
            {
                case 0:
                    g.fillRactangle(Color.Green, new Rectangle()
                    {
                        X = x,
                        Y = y,
                        Width = tileWidth,
                        Height = tileHeight,
                    });
                    break;
                case 1:
                    g.fillRactangle(Color.Blue, new Rectangle()
                    {
                        X = x,
                        Y = y,
                        Width = tileWidth,
                        Height = tileHeight,
                    });
                    break;
            }
        }

        public void drawCurrentCharacter(GameGraphic g, int x, int y)
        {
            g.fillRactangle(Color.Red, new Rectangle()
            {
                X = x,
                Y = y,
                Width = tileWidth,
                Height = tileHeight,
            });
        }

        public void drawStronghold(GameGraphic g, int x, int y, int strongholdId)
        {
            g.drawImage(tileObjects, new Point(x, y), new Rectangle()
            {
                X = 48,
                Y = 144,
                Width = tileWidth,
                Height = tileHeight
            });
        }

        public override void draw()
        {
            var g = gameGraphic;
            var vp = getTileLocation();
            var vx = vp.x;
            var vy = vp.y;
            var offsetX = camera.x - tileWidth * vx;
            var offsetY = camera.y - tileHeight * vy;

            var playerLocation = gameWorld.currentCharacter.location;

            for (int y = 0; y < tileHeightCount; ++y)
            {
                var fy = vy + y;

                for (int x = 0; x < tileWidthCount; ++x)
                {
                    var fx = vx + x;

                    var p = new MapPoint(fx, fy);

                    if (tileMap.isOutOfBounds(p)) continue;

                    var tile = (Tile)tileMap[p];

                    var sx = x * tileWidth - offsetX;
                    var sy = y * tileHeight - offsetY;

                    drawTile(g, sx, sy, tile);

                    if (playerLocation.x == fx && playerLocation.y == fy) drawCurrentCharacter(g, sx, sy);
                }
            }
        }
    }
}
