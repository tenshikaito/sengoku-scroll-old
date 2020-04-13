using Client.Helper;
using Library;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Client.Graphic
{
    public class DetailTileMapSprites : TileMapSpritesBase
    {
        private DetailTileMapImageInfo tileMapImageInfo;
        private DetailMapSpritesInfo mapSpritesInfo;

        private AutoTileSprite tileSprite;

        private Dictionary<int, TileSpriteAnimation> terrainSprite = new Dictionary<int, TileSpriteAnimation>();

        protected override TileMap map => gameWorld.mainTileMap;

        private DetailTileMap tileMap => mapSpritesInfo.detailTileMap;

        public override int tileWidth => tileMapImageInfo.tileSize.Width;

        public override int tileHeight => tileMapImageInfo.tileSize.Height;

        public DetailTileMapSprites(GameSystem gs, GameWorld gw, DetailTileMapImageInfo mii, DetailMapSpritesInfo msi, bool isEditor = false)
            : base(gs, gw, isEditor)
        {
            init(mii, msi);

            resize();
        }

        public void init(DetailTileMapImageInfo mii, DetailMapSpritesInfo msi)
        {
            tileMapImageInfo = mii;
            mapSpritesInfo = msi;

            terrainSprite = mii.terrainAnimation.Select(o => new KeyValuePair<int, TileSpriteAnimation>(o.Key, new TileSpriteAnimation(o.Value))).ToDictionary(o => o.Key, o => o.Value);

            mii.terrainAnimation.Values.ToList().ForEach(o => o.frames.ForEach(oo => gameWorld.getImage(oo.fileName)));

            tileSprite = new AutoTileSprite(this);
        }

        protected override void draw(GameGraphic g, int fx, int fy, MapPoint p, int sx, int sy)
        {
            if (tileMap.isOutOfBounds(p)) return;

            var t = (DetailMapTile)tileMap[p];

            drawTerrain(g, p, sx, sy, t);

            if (p == cursorPosition) drawCursor(g, sx, sy);

            if (!isEditor)
            {
                var playerLocation = gameWorld.currentCharacter.location;

                if (playerLocation.x == fx && playerLocation.y == fy) drawCurrentCharacter(g, sx, sy);
            }
        }

        private void drawTerrain(GameGraphic g, MapPoint p, int x, int y, DetailMapTile t)
        {
            if (!terrainSprite.TryGetValue(t.terrain, out var s)) return;

            var point = s.currentPoint;

            var ts = tileSprite;

            ts.position = new Point(x, y);

            //viewMode.drawTerrain(this, ts, t);

            var type = mapSpritesInfo.checkTerrainBorder(p);

            ts.refresh(gameWorld.getImage(s.fileName), point, type);

            g.drawSprite(ts);

            //switch (t.terrain)
            //{
            //    case 0:
            //        g.fillRactangle(Color.Green, new Rectangle()
            //        {
            //            X = x,
            //            Y = y,
            //            Width = tileWidth,
            //            Height = tileHeight,
            //        });
            //        break;
            //    case 1:
            //        g.fillRactangle(Color.Blue, new Rectangle()
            //        {
            //            X = x,
            //            Y = y,
            //            Width = tileWidth,
            //            Height = tileHeight,
            //        });
            //        break;
            //}
        }

        public void recoveryTileFlag(MapPoint p) => mapSpritesInfo.recoveryTileFlag(p);

        private void drawCurrentCharacter(GameGraphic g, int x, int y)
        {
            g.fillRactangle(Color.Red, new Rectangle()
            {
                X = x,
                Y = y,
                Width = tileWidth,
                Height = tileHeight,
            });
        }

        private void drawStronghold(GameGraphic g, int x, int y, int strongholdId)
        {
            //g.drawImage(tileObjects, new Point(x, y), new Rectangle()
            //{
            //    X = 48,
            //    Y = 144,
            //    Width = tileWidth,
            //    Height = tileHeight
            //});
        }

        private void drawCursor(GameGraphic g, int sx, int sy)
        {
            int size = 4;

            g.drawRactangle(Color.White, new Rectangle()
            {
                X = sx,
                Y = sy,
                Width = tileWidth - size,
                Height = tileHeight - size
            }, size);
        }

        public class DetailMapSpritesInfo : MapSpritesInfo
        {
            public DetailTileMap detailTileMap;

            protected override TileMap tileMap => detailTileMap;
            protected override Dictionary<int, Terrain> terrain => gameWorld.masterData.terrain;

            public DetailMapSpritesInfo(GameWorld gw) : base(gw)
            {
            }

            public override byte calculateTileMargin(MapPoint p)
            {
                if (tileMap.isOutOfBounds(p)) return 0;

                var t = (DetailMapTile)detailTileMap[p];
                int y = p.y, x = p.x;

                if (!terrain.TryGetValue(t.terrain, out var tt)) return 0;

                byte flag = 0;

                calculateTileMargin(ref flag, x - 1, y - 1, tt, AutoTileCalculator.topLeft);
                calculateTileMargin(ref flag, x, y - 1, tt, AutoTileCalculator.top);
                calculateTileMargin(ref flag, x + 1, y - 1, tt, AutoTileCalculator.topRight);
                calculateTileMargin(ref flag, x - 1, y, tt, AutoTileCalculator.left);
                calculateTileMargin(ref flag, x + 1, y, tt, AutoTileCalculator.right);
                calculateTileMargin(ref flag, x - 1, y + 1, tt, AutoTileCalculator.bottomLeft);
                calculateTileMargin(ref flag, x, y + 1, tt, AutoTileCalculator.bottom);
                calculateTileMargin(ref flag, x + 1, y + 1, tt, AutoTileCalculator.bottomRight);

                return flag;
            }

            protected void calculateTileMargin(ref byte flag, int x, int y, Terrain t, byte direction)
            {
                var p = new MapPoint(x, y);

                if (tileMap.isOutOfBounds(p)) return;

                var tt = detailTileMap[p];
                var ttt = (DetailMapTile)tt;

                if (terrain.TryGetValue(ttt.terrain, out var tttt))
                {
                    if (t.isWater != tttt.isWater) flag |= direction;
                }
            }
        }
    }
}
