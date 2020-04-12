using Client.Helper;
using Library;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Library.GameWorldOuterMapData;

namespace Client.Graphic
{
    public class OuterTileMapSprites : TileMapSpritesBase
    {
        private OuterTileMapImageInfo tileMapImageInfo;
        private OuterMapSpritesInfo mapSpritesInfo;

        private AutoTileSprite tileSprite;

        private Dictionary<int, TileSpriteAnimation> terrainSprite = new Dictionary<int, TileSpriteAnimation>();

        public override int tileWidth => tileMapImageInfo.tileSize.Width;

        public override int tileHeight => tileMapImageInfo.tileSize.Height;

        protected override TileMap map => gameMapData.data;
        private OuterTileMap tileMap => gameMapData.data;

        public OuterTileMapSprites(GameSystem gs, GameWorld gw, OuterTileMapImageInfo mii, OuterMapSpritesInfo msi, bool isEditor = false)
            : base(gs, gw, isEditor)
        {
            tileMapImageInfo = mii;
            mapSpritesInfo = msi;

            terrainSprite = mii.terrainAnimation.Select(o => new KeyValuePair<int, TileSpriteAnimation>(o.Key, new TileSpriteAnimation(o.Value))).ToDictionary(o => o.Key, o => o.Value);
            strongholdSprite = mii.strongholdAnimation.Select(o => new KeyValuePair<int, TileSpriteAnimation>(o.Key, new TileSpriteAnimation(o.Value))).ToDictionary(o => o.Key, o => o.Value);

            mii.terrainAnimation.Values.ToList().ForEach(o => o.frames.ForEach(oo => gameWorld.getImage(oo.fileName)));
            //mii.strongholdAnimation.Values.ToList().ForEach(o => gameWorld.getImage(o.fileName));

            tileSprite = new AutoTileSprite(this);

            resize();
        }

        public override void update()
        {
            var now = DateTime.Now;

            foreach (var o in terrainSprite.Values) o.update(now);

            foreach (var o in strongholdSprite.Values) o.update(now);
        }

        protected override void draw(GameGraphic g, int fx, int fy, MapPoint p, int sx, int sy)
        {
            if (map.isOutOfBounds(p)) return;

            var tile = (OuterMapTile)tileMap[p];

            drawTerrain(g, p, sx, sy, tile);

            if (!isEditor)
            {
                var playerLocation = gameWorld.currentCharacter.location;

                if (playerLocation.x == fx && playerLocation.y == fy) drawCurrentCharacter(g, sx, sy);
            }

            var strongholdId = gameMapData.stronghold.getId(gameMapData, p);

            if (strongholdId != null) drawStronghold(g, sx, sy, (int)strongholdId);

            if (p == cursorPosition) drawCursor(g, sx, sy);
        }

        private void drawTerrain(GameGraphic g, MapPoint p, int x, int y, OuterMapTile t)
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

        private Dictionary<int, TileSpriteAnimation> strongholdSprite = new Dictionary<int, TileSpriteAnimation>();

        private void drawStronghold(GameGraphic g, int x, int y, int strongholdId)
        {
            if (!strongholdSprite.TryGetValue(strongholdId, out var s)) return;

            var p = s.currentPoint;

            g.drawImage(gameWorld.getImage(s.fileName), x, y, new Rectangle()
            {
                X = p.X,
                Y = p.Y,
                Width = tileWidth,
                Height = tileHeight
            });
            //if (mapImageInfo.strongholdAnimation.TryGetValue(strongholdId, out var p))
            //{
            //    g.drawImage(tileObjects, new Rectangle()
            //    {
            //        X = x,
            //        Y = y,
            //        Width = tileWidth,
            //        Height = tileHeight
            //    }, new Rectangle()
            //    {
            //        X = p.X,
            //        Y = p.Y,
            //        Width = tileWidth,
            //        Height = tileHeight
            //    });
            //}
            //else
            //{
            //    g.fillRactangle(Color.Black, new Rectangle()
            //    {
            //        X = x,
            //        Y = y,
            //        Width = tileWidth,
            //        Height = tileHeight,
            //    });
            //}
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

        public class OuterMapSpritesInfo : MapSpritesInfo
        {
            protected override TileMap tileMap => gameWorld.gameOuterMapData.data;
            protected override Dictionary<int, Terrain> terrain => gameWorld.gameWorldMasterData.terrain;

            public OuterTileMap outerTileMap => gameWorld.gameOuterMapData.data;

            public OuterMapSpritesInfo(GameWorld gw) : base(gw)
            {
            }

            public override byte calculateTileMargin(MapPoint p)
            {
                if (tileMap.isOutOfBounds(p)) return 0;

                var t = (OuterMapTile)outerTileMap[p];
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

                var tt = outerTileMap[p];
                var ttt = (OuterMapTile)tt;

                if (terrain.TryGetValue(ttt.terrain, out var tttt))
                {
                    if (t.isWater != tttt.isWater) flag |= direction;
                }
            }
        }
    }
}
