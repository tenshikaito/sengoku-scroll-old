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
        private MapSpritesInfo mapSpritesInfo;

        public override int tileWidth => tileMapImageInfo.tileSize.Width;

        public override int tileHeight => tileMapImageInfo.tileSize.Height;

        protected override Map map => gameMapData.data;
        private TileMap tileMap => gameMapData.data;

        public OuterTileMapSprites(GameSystem gs, GameWorld gw, OuterTileMapImageInfo mii, MapSpritesInfo msi, bool isEditor = false)
            : base(gs, gw, mii.terrainImageFileName, mii.tileObjectImageFileName, isEditor)
        {
            tileMapImageInfo = mii;
            mapSpritesInfo = msi;

            terrainSprite = mii.terrainAnimation.Values.Select(o => new KeyValuePair<int, TileSprite>(o.id, new TileSprite(o))).ToDictionary(o => o.Key, o => o.Value);
            strongholdSprite = mii.strongholdAnimation.Values.Select(o => new KeyValuePair<int, TileSprite>(o.id, new TileSprite(o))).ToDictionary(o => o.Key, o => o.Value);

            tileSprite = new AutoTileSprite(this);

            resize();
        }

        protected override void updateTileSprite()
        {
            foreach (var o in terrainSprite.Values) o.update();

            foreach (var o in strongholdSprite.Values) o.update();
        }

        protected override void draw(GameGraphic g, int fx, int fy, MapPoint p, int sx, int sy)
        {
            if (map.isOutOfBounds(p)) return;

            var tile = (Tile)tileMap[p];

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

        private class TileSprite
        {
            private TileAnimation tileAnimation;
            private int index;

            public Point currentPoint => tileAnimation.frames[index];

            public TileSprite(TileAnimation ta)
            {
                tileAnimation = ta;
            }

            public void update()
            {
                index = ++index % tileAnimation.frames.Count;
            }
        }

        public class MapSpritesInfo
        {
            private Dictionary<int, byte> terrainBorder = new Dictionary<int, byte>();

            private GameWorld gameWorld;
            private TileMap tileMap => gameWorld.gameOuterMapData.data;

            public MapSpritesInfo(GameWorld gw) => gameWorld = gw;

            public byte checkTerrainBorder(MapPoint p)
            {
                int index = tileMap.getIndex(p);

                if (!terrainBorder.TryGetValue(index, out var type))
                {
                    terrainBorder[index] = calculateTileMargin(p);
                }

                return type;
            }

            public void resetTileFlag(MapPoint p)
            {
                removeTileFlag(p);

                recoveryTileFlag(p);
            }

            public void removeTileFlag(MapPoint p) => tileMap.eachRangedRectangle(p, new Map.Size(1), o => terrainBorder.Remove(tileMap.getIndex(o)));

            public void recoveryTileFlag(MapPoint p) => tileMap.eachRangedRectangle(p, new Map.Size(1), o => checkTerrainBorder(o));

            public byte calculateTileMargin(MapPoint p)
            {
                if (tileMap.isOutOfBounds(p)) return 0;

                var t = (Tile)tileMap[p];
                int y = p.y,
                    x = p.x;
                var tt = gameWorld.gameWorldMasterData.terrain[t.terrain];
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

            private void calculateTileMargin(ref byte flag, int x, int y, Terrain t, byte direction)
            {
                var p = new MapPoint(x, y);

                if (tileMap.isOutOfBounds(p)) return;

                var tt = tileMap[p];
                var ttt = (Tile)tt;

                var tttt = gameWorld.gameWorldMasterData.terrain[ttt.terrain];

                if (t.isWater != tttt.isWater) flag |= direction;
            }
        }

        private AutoTileSprite tileSprite;

        private Dictionary<int, TileSprite> terrainSprite = new Dictionary<int, TileSprite>();

        private void drawTerrain(GameGraphic g, MapPoint p, int x, int y, Tile t)
        {
            if (!terrainSprite.TryGetValue(t.terrain, out var s)) return;

            var point = s.currentPoint;

            var ts = tileSprite;

            ts.position = new Point(x, y);

            //viewMode.drawTerrain(this, ts, t);

            var type = mapSpritesInfo.checkTerrainBorder(p);

            ts.refresh(terrainImage, point, type);

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

        public void resetTileFlag(MapPoint p) => mapSpritesInfo.resetTileFlag(p);

        public void removeTileFlag(MapPoint p) => mapSpritesInfo.removeTileFlag(p);

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

        private Dictionary<int, TileSprite> strongholdSprite = new Dictionary<int, TileSprite>();

        private void drawStronghold(GameGraphic g, int x, int y, int strongholdId)
        {
            if (!strongholdSprite.TryGetValue(strongholdId, out var s)) return;

            var p = s.currentPoint;

            g.drawImage(tileObjects, x, y, new Rectangle()
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
    }
}
