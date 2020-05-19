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
        private TileMapImageInfo tileMapImageInfo;
        private DetailMapSpritesInfo mapSpritesInfo;

        private AutoTileSprite tileSprite;

        private Dictionary<int, TileSpriteAnimation> terrainSpriteSpring;
        private Dictionary<int, TileSpriteAnimation> terrainSpriteSummer;
        private Dictionary<int, TileSpriteAnimation> terrainSpriteAutumn;
        private Dictionary<int, TileSpriteAnimation> terrainSpriteWinter;

        protected override TileMap map => gameWorld.mainTileMap;

        private DetailTileMap tileMap => mapSpritesInfo.detailTileMap;

        public override int tileWidth => tileMapImageInfo.tileSize.Width;

        public override int tileHeight => tileMapImageInfo.tileSize.Height;

        public DetailTileMapSprites(GameSystem gs, GameWorld gw, TileMapImageInfo mii, DetailMapSpritesInfo msi, bool isEditor = false)
            : base(gs, gw, isEditor)
        {
            tileMapImageInfo = mii;
            mapSpritesInfo = msi;

            var ti = gameWorld.masterData.terrainImage;
            var list = ti.Values.ToList();

            terrainSpriteSpring = ti.ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value.animationViewSpring));
            terrainSpriteSummer = ti.ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value.animationViewSummer));
            terrainSpriteAutumn = ti.ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value.animationViewAutumn));
            terrainSpriteWinter = ti.ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value.animationViewWinter));

            list.ForEach(o => o.animationViewSpring?.ForEach(oo => gameWorld.getTileMapImage(oo.fileName)));
            list.ForEach(o => o.animationViewSummer?.ForEach(oo => gameWorld.getTileMapImage(oo.fileName)));
            list.ForEach(o => o.animationViewAutumn?.ForEach(oo => gameWorld.getTileMapImage(oo.fileName)));
            list.ForEach(o => o.animationViewWinter?.ForEach(oo => gameWorld.getTileMapImage(oo.fileName)));

            tileSprite = new AutoTileSprite(this);

            resize();
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
            var s = null as TileSpriteAnimation;

            switch (gameWorld.gameDate.season)
            {
                case GameDate.Season.spring:
                    if (!terrainSpriteSpring.TryGetValue(t.terrain, out s)) return;
                    break;
                case GameDate.Season.summer:
                    if (!terrainSpriteSummer.TryGetValue(t.terrain, out s)) return;
                    break;
                case GameDate.Season.autumn:
                    if (!terrainSpriteAutumn.TryGetValue(t.terrain, out s)) return;
                    break;
                case GameDate.Season.winter:
                    if (!terrainSpriteWinter.TryGetValue(t.terrain, out s)) return;
                    break;
            }

            var img = gameWorld.getTileMapImage(s.fileName);

            if (img == null) return;

            var point = s.currentPoint;

            var ts = tileSprite;

            ts.position = new Point(x, y);

            //viewMode.drawTerrain(this, ts, t);

            var type = mapSpritesInfo.checkTerrainBorder(p);

            ts.refresh(img, point, type);

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
            protected override Dictionary<int, Terrain> terrain => gameWorld.masterData.detailTileMapTerrain;

            public DetailMapSpritesInfo(GameWorld gw) : base(gw)
            {
            }

            public override byte calculateTileMargin(MapPoint p, bool isSurface = false)
            {
                if (tileMap.isOutOfBounds(p)) return 0;

                var t = (DetailMapTile)detailTileMap[p];
                int y = p.y, x = p.x;

                if (!gameWorld.masterData.detailTileMapTerrain.TryGetValue(t.terrain, out var tt)) return 0;

                byte flag = 0;

                calculateTileMargin(ref flag, x - 1, y - 1, tt, AutoTileCalculator.topLeft, isSurface);
                calculateTileMargin(ref flag, x, y - 1, tt, AutoTileCalculator.top, isSurface);
                calculateTileMargin(ref flag, x + 1, y - 1, tt, AutoTileCalculator.topRight, isSurface);
                calculateTileMargin(ref flag, x - 1, y, tt, AutoTileCalculator.left, isSurface);
                calculateTileMargin(ref flag, x + 1, y, tt, AutoTileCalculator.right, isSurface);
                calculateTileMargin(ref flag, x - 1, y + 1, tt, AutoTileCalculator.bottomLeft, isSurface);
                calculateTileMargin(ref flag, x, y + 1, tt, AutoTileCalculator.bottom, isSurface);
                calculateTileMargin(ref flag, x + 1, y + 1, tt, AutoTileCalculator.bottomRight, isSurface);

                return flag;
            }

            protected void calculateTileMargin(ref byte flag, int x, int y, Terrain t, byte direction, bool isSurface)
            {
                var p = new MapPoint(x, y);

                if (tileMap.isOutOfBounds(p)) return;

                var tt = detailTileMap[p];
                if (!gameWorld.masterData.detailTileMapTerrain.TryGetValue(tt.Value.terrain, out var ttt)) return;

                if (t.imageId != ttt.imageId) flag |= direction;
            }
        }
    }
}
