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
using static Library.MainTileMapData;

namespace Client.Graphic
{
    public abstract class MainTileMapSprites : TileMapSpritesBase
    {
        private MainMapSpritesInfo mapSpritesInfo;

        private AutoTileSprite tileSprite;

        private Dictionary<int, TileSpriteAnimation> terrainSpriteSpring;
        private Dictionary<int, TileSpriteAnimation> terrainSpriteSummer;
        private Dictionary<int, TileSpriteAnimation> terrainSpriteAutumn;
        private Dictionary<int, TileSpriteAnimation> terrainSpriteWinter;

        public override int tileWidth => tileMapImageInfo.tileSize.Width;

        public override int tileHeight => tileMapImageInfo.tileSize.Height;

        protected override TileMap map => gameWorld.mainTileMap;

        private MainTileMap tileMap => gameWorld.mainTileMap;

        protected abstract MainTileMapImageInfo tileMapImageInfo { get; }

        private DateTime lastUpdateTime = DateTime.Now;
        private static TimeSpan nextFrameSpan = TimeSpan.FromMilliseconds(500);

        public MainTileMapSprites(GameSystem gs, GameWorld gw, MainMapSpritesInfo msi, bool isEditor = false)
            : base(gs, gw, isEditor)
        {
            mapSpritesInfo = msi;

            init(msi);

            tileSprite = new AutoTileSprite(this);

            resize();
        }

        public void init(MainMapSpritesInfo msi)
        {
            refresh();
        }

        public override void refresh()
        {
            var mii = tileMapImageInfo;
            var msi = mapSpritesInfo;

            terrainSpriteSpring = mii.terrainAnimationSpring.ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value));
            terrainSpriteSummer = mii.terrainAnimationSummer.ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value));
            terrainSpriteAutumn = mii.terrainAnimationAutumn.ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value));
            terrainSpriteWinter = mii.terrainAnimationWinter.ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value));

            //strongholdSprite = mii.strongholdAnimation.ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value));

            mii.terrainAnimationSpring.Values.ToList().ForEach(o => o.ForEach(oo => gameWorld.getImage(oo.fileName)));
            mii.terrainAnimationSummer.Values.ToList().ForEach(o => o.ForEach(oo => gameWorld.getImage(oo.fileName)));
            mii.terrainAnimationAutumn.Values.ToList().ForEach(o => o.ForEach(oo => gameWorld.getImage(oo.fileName)));
            mii.terrainAnimationWinter.Values.ToList().ForEach(o => o.ForEach(oo => gameWorld.getImage(oo.fileName)));

            //mii.strongholdAnimation.Values.ToList().ForEach(o => gameWorld.getImage(o.fileName));

            mapSpritesInfo.removeTileFlag();
        }

        public override void update()
        {
            var now = DateTime.Now;

            if (now - lastUpdateTime >= nextFrameSpan)
            {
                lastUpdateTime = now;

                var terrainSprite = null as Dictionary<int, TileSpriteAnimation>;

                switch (gameWorld.gameDate.season)
                {
                    case GameDate.Season.spring: terrainSprite = terrainSpriteSpring; break;
                    case GameDate.Season.summer: terrainSprite = terrainSpriteSummer; break;
                    case GameDate.Season.autumn: terrainSprite = terrainSpriteAutumn; break;
                    case GameDate.Season.winter: terrainSprite = terrainSpriteWinter; break;
                }

                foreach (var o in terrainSprite.Values) o.update();

                foreach (var o in strongholdSprite.Values) o.update();
            }
        }

        protected override void draw(GameGraphic g, int fx, int fy, MapPoint p, int sx, int sy)
        {
            if (map.isOutOfBounds(p)) return;

            var tile = (MainMapTile)tileMap[p];

            drawTerrain(g, p, sx, sy, tile);

            if (!isEditor)
            {
                var playerLocation = gameWorld.currentCharacter.location;

                if (playerLocation.x == fx && playerLocation.y == fy) drawCurrentCharacter(g, sx, sy);
            }

            var strongholdId = mainTileMapData.stronghold.getId(tileMap, p);

            if (strongholdId != null) drawStronghold(g, sx, sy, (int)strongholdId);

            if (p == cursorPosition) drawCursor(g, sx, sy);
        }

        private void drawTerrain(GameGraphic g, MapPoint p, int x, int y, MainMapTile t)
        {
            var s = null as TileSpriteAnimation;

            switch (gameWorld.gameDate.season)
            {
                case GameDate.Season.spring:
                    if (!terrainSpriteSpring.TryGetValue(t.terrain, out var s1)) return;
                    s = s1;
                    break;
                case GameDate.Season.summer:
                    if (!terrainSpriteSummer.TryGetValue(t.terrain, out var s2)) return;
                    s = s2;
                    break;
                case GameDate.Season.autumn:
                    if (!terrainSpriteAutumn.TryGetValue(t.terrain, out var s3)) return;
                    s = s3;
                    break;
                case GameDate.Season.winter:
                    if (!terrainSpriteWinter.TryGetValue(t.terrain, out var s4)) return;
                    s = s4;
                    break;
            }

            var img = gameWorld.getImage(s.fileName);

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

        public class MainMapSpritesInfo : MapSpritesInfo
        {
            protected override TileMap tileMap => gameWorld.mainTileMap;
            protected override Dictionary<int, Terrain> terrain => gameWorld.masterData.terrain;

            public MainTileMap mainTileMap => gameWorld.mainTileMap;

            public MainMapSpritesInfo(GameWorld gw) : base(gw)
            {
            }

            public override byte calculateTileMargin(MapPoint p)
            {
                if (tileMap.isOutOfBounds(p)) return 0;

                var t = (MainMapTile)mainTileMap[p];
                var y = p.y;
                var x = p.x;

                if (!gameWorld.masterData.terrainImage.TryGetValue(t.terrain, out var ti)) return 0;

                if (!terrain.TryGetValue(ti.terrainId, out var tt)) return 0;

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

                var tt = mainTileMap[p];
                var ttt = (MainMapTile)tt;

                if (terrain.TryGetValue(gameWorld.masterData.terrainImage[ttt.terrain].terrainId, out var tttt))
                {
                    if (t.isWater != tttt.isWater) flag |= direction;
                }
            }
        }
    }

    public class MainTileMapViewSprites : MainTileMapSprites
    {
        protected override MainTileMapImageInfo tileMapImageInfo => gameWorld.masterData.mainTileMapViewImageInfo;
     
        public MainTileMapViewSprites(GameSystem gs, GameWorld gw, MainMapSpritesInfo msi, bool isEditor = false) : base(gs, gw, msi, isEditor)
        {
        }
    }

    public class MainTileMapDetailSprites : MainTileMapSprites
    {
        protected override MainTileMapImageInfo tileMapImageInfo => gameWorld.masterData.mainTileMapDetailImageInfo;

        public MainTileMapDetailSprites(GameSystem gs, GameWorld gw, MainMapSpritesInfo msi, bool isEditor = false) : base(gs, gw, msi, isEditor)
        {
        }
    }
}
