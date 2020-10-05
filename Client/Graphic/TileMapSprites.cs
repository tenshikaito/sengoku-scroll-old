using Client.Game;
using Client.Helper;
using Library;
using Library.Helper;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Graphic
{
    public abstract class TileMapSprites : TileMapSpritesBase
    {
        private MapSpritesInfo mapSpritesInfo;

        private AutoTileSprite tileSprite;
        private SpriteRectangle selector;

        protected Dictionary<int, TileSpriteAnimation> terrainSprite;
        protected Dictionary<int, TileSpriteAnimation> terrainSpriteSpring;
        protected Dictionary<int, TileSpriteAnimation> terrainSpriteSummer;
        protected Dictionary<int, TileSpriteAnimation> terrainSpriteAutumn;
        protected Dictionary<int, TileSpriteAnimation> terrainSpriteWinter;
        protected Dictionary<int, TileSpriteAnimation> terrainSpriteSnow;

        public override int tileWidth => tileMapImageInfo.tileSize.Width;

        public override int tileHeight => tileMapImageInfo.tileSize.Height;

        protected override TileMap map => gameWorldData.tileMap;

        private TileMap tileMap => map;

        protected abstract TileMapImageInfo tileMapImageInfo { get; }

        private DateTime lastUpdateTime = DateTime.Now;
        private static TimeSpan nextFrameSpan = TimeSpan.FromMilliseconds(500);

        public TileMapSprites(GameSystem gs, GameWorld gw, MapSpritesInfo msi, bool isEditor = false)
            : base(gs, gw, isEditor)
        {
            mapSpritesInfo = msi;

            tileSprite = new AutoTileSprite(this);

            int size = 4;

            selector = new SpriteRectangle()
            {
                color = Color.White,
                width = size,
                size = new Size(tileWidth - size, tileHeight - size)
            };

            resize();
        }

        public override void update()
        {
            var now = DateTime.Now;

            if (now - lastUpdateTime >= nextFrameSpan)
            {
                lastUpdateTime = now;

                foreach (var o in this.terrainSprite.Values) o.update();

                var terrainSprite = null as Dictionary<int, TileSpriteAnimation>;

                switch (gameWorldData.gameDate.season)
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

            var tile = tileMap[p];

            drawTerrain(g, p, sx, sy, tile, false, false);

            if (!isEditor)
            {
                var playerLocation = gameWorld.currentPlayer.location;

                if (playerLocation.x == fx && playerLocation.y == fy) drawCurrentCharacter(g, sx, sy);
            }

            var strongholdId = tileMap.stronghold.getId(tileMap, p);

            if (strongholdId != null) drawStronghold(g, sx, sy, (int)strongholdId);

            if (p == cursorPosition) drawCursor(g, sx, sy);
        }

        private void drawTerrain(GameGraphic g, MapPoint p, int x, int y, MapTile t, bool isSurface, bool isSurfaceDone)
        {
            var md = gameWorldData.masterData;
            var tt = md.tileMapTerrain[isSurface ? t.terrainSurface.Value : t.terrain];
            var s = terrainSprite[tt.imageId];

            switch (gameWorldData.gameDate.season)
            {
                case GameDate.Season.spring:
                    if (terrainSpriteSpring.TryGetValue(tt.imageId, out var s1)) s = s1;
                    break;
                case GameDate.Season.summer:
                    if (terrainSpriteSummer.TryGetValue(tt.imageId, out var s2)) s = s2;
                    break;
                case GameDate.Season.autumn:
                    if (terrainSpriteAutumn.TryGetValue(tt.imageId, out var s3)) s = s3;
                    break;
                case GameDate.Season.winter:
                    if (terrainSpriteWinter.TryGetValue(tt.imageId, out var s4)) s = s4;
                    break;
            }

            if (!s.hasOne) return;

            var img = gameWorld.getTileMapImage(s.fileName);

            if (img == null) return;

            var point = s.currentPoint;

            var ts = tileSprite;

            ts.position = new Point(x, y);

            //viewMode.drawTerrain(this, ts, t);

            var type = mapSpritesInfo.checkTerrainBorder(p, isSurface);

            ts.refresh(img, point, type);

            g.drawSprite(ts);

            if (t.terrainSurface != null && !isSurfaceDone)
            {
                drawTerrain(g, p, x, y, t, true, true);
            }
        }

        private void drawCurrentCharacter(GameGraphic g, int x, int y)
        {
        }

        private Dictionary<int, TileSpriteAnimation> strongholdSprite = new Dictionary<int, TileSpriteAnimation>();

        private void drawStronghold(GameGraphic g, int x, int y, int strongholdId)
        {
            if (!strongholdSprite.TryGetValue(strongholdId, out var s)) return;

            var p = s.currentPoint;

            //g.drawImage(gameWorld.getTileMapImage(s.fileName), x, y, new Rectangle()
            //{
            //    X = p.X,
            //    Y = p.Y,
            //    Width = tileWidth,
            //    Height = tileHeight
            //});
        }

        private void drawCursor(GameGraphic g, int sx, int sy)
        {
            selector.position = new Point(sx, sy);

            g.drawRectangle(selector);
        }

        public new class MapSpritesInfo : TileMapSpritesBase.MapSpritesInfo
        {
            public MapSpritesInfo(GameWorld gw) : base(gw)
            {
            }

            public override byte calculateTileMargin(MapPoint p, bool isSurface = false)
            {
                if (tileMap.isOutOfBounds(p)) return 0;

                var t = tileMap[p];
                var y = p.y;
                var x = p.x;

                var tid = t.terrain;

                if (isSurface)
                {
                    if (t.terrainSurface == null) return 0;
                    else tid = t.terrainSurface.Value;
                }

                if (!gameWorldData.masterData.tileMapTerrain.TryGetValue(tid, out var tt)) return 0;

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

                var tt = tileMap[p];

                var tid = tt.terrain;

                if (isSurface)
                {
                    if (tt.terrainSurface == null)
                    {
                        flag |= direction;
                        return;
                    }
                    else
                    {
                        tid = tt.terrainSurface.Value;
                    }
                }

                if (!gameWorldData.masterData.tileMapTerrain.TryGetValue(tid, out var ttt)) return;

                if (t.imageId != ttt.imageId) flag |= direction;
            }
        }
    }

    public class MainTileMapViewSprites : TileMapSprites
    {
        protected override TileMapImageInfo tileMapImageInfo => gameWorldData.masterData.tileMapViewImageInfo;

        public MainTileMapViewSprites(GameSystem gs, GameWorld gw, MapSpritesInfo msi, bool isEditor = false) : base(gs, gw, msi, isEditor)
        {
            var ti = gameWorldData.masterData.terrainImage;
            var list = ti.Values.ToList();

            terrainSprite = ti.Where(o => o.Value.animationView != null).ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value.animationView));
            terrainSpriteSpring = ti.Where(o => o.Value.animationViewSpring != null).ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value.animationViewSpring));
            terrainSpriteSummer = ti.Where(o => o.Value.animationViewSummer != null).ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value.animationViewSummer));
            terrainSpriteAutumn = ti.Where(o => o.Value.animationViewAutumn != null).ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value.animationViewAutumn));
            terrainSpriteWinter = ti.Where(o => o.Value.animationViewWinter != null).ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value.animationViewWinter));
            terrainSpriteSnow = ti.Where(o => o.Value.animationViewSnow != null).ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value.animationViewSnow));

            //strongholdSprite = mii.strongholdAnimation.ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value));

            list.ForEach(o => o.animationView?.ForEach(oo => gameWorld.getTileMapImage(oo.fileName)));
            list.ForEach(o => o.animationViewSpring?.ForEach(oo => gameWorld.getTileMapImage(oo.fileName)));
            list.ForEach(o => o.animationViewSummer?.ForEach(oo => gameWorld.getTileMapImage(oo.fileName)));
            list.ForEach(o => o.animationViewAutumn?.ForEach(oo => gameWorld.getTileMapImage(oo.fileName)));
            list.ForEach(o => o.animationViewWinter?.ForEach(oo => gameWorld.getTileMapImage(oo.fileName)));
            list.ForEach(o => o.animationViewSnow?.ForEach(oo => gameWorld.getTileMapImage(oo.fileName)));

            //mii.strongholdAnimation.Values.ToList().ForEach(o => gameWorld.getImage(o.fileName));
        }
    }

    public class MainTileMapDetailSprites : TileMapSprites
    {
        protected override TileMapImageInfo tileMapImageInfo => gameWorldData.masterData.tileMapDetailImageInfo;

        public MainTileMapDetailSprites(GameSystem gs, GameWorld gw, MapSpritesInfo msi, bool isEditor = false) : base(gs, gw, msi, isEditor)
        {
            var ti = gameWorldData.masterData.terrainImage;
            var list = ti.Values.ToList();

            terrainSprite = ti.Where(o => o.Value.animationDetail != null).ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value.animationDetail));
            terrainSpriteSpring = ti.Where(o => o.Value.animationDetailSpring != null).ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value.animationDetailSpring));
            terrainSpriteSummer = ti.Where(o => o.Value.animationDetailSummer != null).ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value.animationDetailSummer));
            terrainSpriteAutumn = ti.Where(o => o.Value.animationDetailAutumn != null).ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value.animationDetailAutumn));
            terrainSpriteWinter = ti.Where(o => o.Value.animationDetailWinter != null).ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value.animationDetailWinter));
            terrainSpriteSnow = ti.Where(o => o.Value.animationDetailSnow != null).ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value.animationDetailSnow));

            //strongholdSprite = mii.strongholdAnimation.ToDictionary(o => o.Key, o => new TileSpriteAnimation(o.Value));

            list.ForEach(o => o.animationDetail?.ForEach(oo => gameWorld.getTileMapImage(oo.fileName)));
            list.ForEach(o => o.animationDetailSpring?.ForEach(oo => gameWorld.getTileMapImage(oo.fileName)));
            list.ForEach(o => o.animationDetailSummer?.ForEach(oo => gameWorld.getTileMapImage(oo.fileName)));
            list.ForEach(o => o.animationDetailAutumn?.ForEach(oo => gameWorld.getTileMapImage(oo.fileName)));
            list.ForEach(o => o.animationDetailWinter?.ForEach(oo => gameWorld.getTileMapImage(oo.fileName)));
            list.ForEach(o => o.animationDetailSnow?.ForEach(oo => gameWorld.getTileMapImage(oo.fileName)));

            //mii.strongholdAnimation.Values.ToList().ForEach(o => gameWorld.getImage(o.fileName));
        }
    }
}
