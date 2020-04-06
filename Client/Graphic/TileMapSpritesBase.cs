using Library;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.Graphic
{
    public abstract partial class TileMapSpritesBase : GameObject
    {
        public int width;
        public int height;

        public int tileWidthCount;
        public int tileHeightCount;

        public abstract int tileWidth { get; }
        public abstract int tileHeight { get; }

        public Image terrainImage;
        public Image tileObjects;

        public MapPoint cursorPosition;

        protected GameSystem gameSystem;
        protected GameWorld gameWorld;

        protected bool isEditor;

        protected TimeSpan updateSpriteInterval = TimeSpan.FromSeconds(0.5);
        protected DateTime lastUpdateSpriteTime = DateTime.Now;

        protected GameOption gameOption => gameWorld.gameOption;
        protected GameWorldOuterMapData gameMapData => gameWorld.gameOuterMapData;
        protected abstract Map map { get; }

        protected Camera camera => gameWorld.camera;

        protected GameGraphic gameGraphic => gameSystem.gameGraphic;

        public TileMapSpritesBase(GameSystem gs, GameWorld gw, string terrainImageFileName, string tileObjectImageFileName, bool isEditor)
        {
            gameSystem = gs;
            gameWorld = gw;

            this.isEditor = isEditor;

            var gwp = gw.getGameWorldProcessor();

            terrainImage = gw.cache.getImage(gwp.getFilePath(terrainImageFileName));
            tileObjects = gw.cache.getImage(gwp.getFilePath(tileObjectImageFileName));
        }

        public void resize()
        {
            width = map.column * tileWidth;
            height = map.row * tileHeight;
            tileWidthCount = camera.width / tileWidth + 2;
            tileHeightCount = camera.height / tileHeight + 2;
        }

        public MapPoint getTileLocation(MouseEventArgs e) => getTileLocation(e.X, e.Y);

        public MapPoint getTileLocation(Point p) => getTileLocation(p.X, p.Y);

        public MapPoint getTileLocation(int sx = 0, int sy = 0)
        {
            sx += camera.x;
            sy += camera.y;
            return new MapPoint(sx >= 0 ? sx / tileWidth : sx / tileWidth - 1, sy >= 0 ? sy / tileHeight : sy / tileHeight - 1);
        }

        public override void update()
        {
            var now = DateTime.Now;

            if (now - lastUpdateSpriteTime >= updateSpriteInterval)
            {
                lastUpdateSpriteTime = now;

                updateTileSprite();
            }
        }

        protected virtual void updateTileSprite()
        {

        }

        public override void draw()
        {
            var g = gameGraphic;
            var vp = getTileLocation();
            var vx = vp.x;
            var vy = vp.y;
            var offsetX = camera.x - tileWidth * vx;
            var offsetY = camera.y - tileHeight * vy;

            for (int y = 0; y < tileHeightCount; ++y)
            {
                var fy = vy + y;

                for (int x = 0; x < tileWidthCount; ++x)
                {
                    var fx = vx + x;

                    var p = new MapPoint(fx, fy);

                    var sx = x * tileWidth - offsetX;
                    var sy = y * tileHeight - offsetY;

                    draw(g, fx, fy, p, sx, sy);
                }
            }
        }

        protected abstract void draw(GameGraphic g, int fx, int fy, MapPoint p, int sx, int sy);

        public override void mouseMoved(MouseEventArgs e) => cursorPosition = getTileLocation(e);

        protected class TileSpriteAnimation
        {
            private List<Point> tileAnimation;
            private int index;

            public Point currentPoint => tileAnimation[index];

            public TileSpriteAnimation(List<Point> ta)
            {
                tileAnimation = ta;
            }

            public void update() => index = ++index % tileAnimation.Count;
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
    }

    //public abstract partial class TileMapSpritesBase<T>
    //{
    //    #region ViewMode
    //    public abstract class ViewMode
    //    {
    //        public abstract void drawTerrain(TileGameMap tgm, AutoTileSprite s, Tile t);

    //        public abstract void drawRange(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf);

    //        public abstract void drawTerritory(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf);

    //        public abstract void drawStronghold(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf);

    //        public abstract void drawUnit(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf);
    //    }

    //    public class StrongholdViewMode : ViewMode
    //    {
    //        public override void drawTerrain(TileGameMap tgm, AutoTileSprite s, Tile t)
    //        {
    //            s.color = Color.White;
    //        }

    //        public override void drawRange(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf)
    //        {
    //            s.color = getForceColor(u.force);
    //        }

    //        public override void drawTerritory(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf)
    //        {
    //            s.color = getForceColor(c.id);
    //        }

    //        public override void drawStronghold(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf)
    //        {
    //            s.color = getForceColor(c.id);
    //        }

    //        public override void drawUnit(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf)
    //        {
    //            s.color = tgm.getRelationshipColor(g.force, u.force, g.id == u.id, false);
    //        }
    //    }

    //    public class ProvinceViewMode : ViewMode
    //    {
    //        public override void drawTerrain(TileGameMap tgm, AutoTileSprite s, Tile t)
    //        {
    //            s.color = getForceColor(t.region);
    //        }

    //        public override void drawRange(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf)
    //        {
    //            s.color = Color.Transparent;
    //        }

    //        public override void drawTerritory(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf)
    //        {
    //            s.color = Color.Transparent;
    //        }

    //        public override void drawStronghold(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf)
    //        {
    //            s.color = getForceColor(c.force);
    //        }

    //        public override void drawUnit(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf)
    //        {
    //            s.color = tgm.getRelationshipColor(g.force, u.force, g.id == u.id, false);
    //        }
    //    }

    //    public class ForceViewMode : ViewMode
    //    {
    //        public override void drawTerrain(TileGameMap tgm, AutoTileSprite s, Tile t)
    //        {
    //            s.color = Color.White;
    //        }

    //        public override void drawRange(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf)
    //        {
    //            s.color = getForceColor(u.force);
    //        }

    //        public override void drawTerritory(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf)
    //        {
    //            s.color = getForceColor(c.force);
    //        }

    //        public override void drawStronghold(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf)
    //        {
    //            s.color = getForceColor(c.force);
    //        }

    //        public override void drawUnit(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf)
    //        {
    //            s.color = getForceColor(u.force);
    //        }
    //    }

    //    public class SuzerainViewMode : ViewMode
    //    {
    //        public override void drawTerrain(TileGameMap tgm, AutoTileSprite s, Tile t)
    //        {
    //            s.color = Color.White;
    //        }

    //        public override void drawRange(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf)
    //        {
    //            s.color = getForceColor(u.isHomeless ? u.force : uf.isIndependence ? uf.suzerain : uf.id);
    //        }

    //        public override void drawTerritory(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf)
    //        {
    //            s.color = getForceColor(c.isHomeless ? c.force : cf.isIndependence ? cf.suzerain : cf.id);
    //        }

    //        public override void drawStronghold(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf)
    //        {
    //            s.color = getForceColor(c.isHomeless ? c.force : cf.isIndependence ? cf.suzerain : cf.id);
    //        }

    //        public override void drawUnit(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf)
    //        {
    //            s.color = getForceColor(u.isHomeless ? u.force : uf.isIndependence ? uf.suzerain : uf.id);
    //        }
    //    }

    //    public class OrbitViewMode : ViewMode
    //    {
    //        public override void drawTerrain(TileGameMap tgm, AutoTileSprite s, Tile t)
    //        {
    //            s.color = Color.White;
    //        }

    //        public override void drawRange(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf)
    //        {
    //            s.color = getForceColor(!u.isHomeless && uf.isBelongToOrbit ? uf.orbit : u.force);
    //        }

    //        public override void drawTerritory(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf)
    //        {
    //            s.color = getForceColor(!c.isHomeless && cf.isBelongToOrbit ? cf.orbit : c.force);
    //        }

    //        public override void drawStronghold(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf)
    //        {
    //            s.color = getForceColor(!c.isHomeless && cf.isBelongToOrbit ? cf.orbit : c.force);
    //        }

    //        public override void drawUnit(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf)
    //        {
    //            s.color = getForceColor(!u.isHomeless && uf.isBelongToOrbit ? uf.orbit : u.force);
    //        }
    //    }

    //    public class ForceDiplomacyViewMode : ViewMode
    //    {
    //        public override void drawTerrain(TileGameMap tgm, AutoTileSprite s, Tile t)
    //        {
    //            s.color = Color.White;
    //        }

    //        public override void drawRange(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf)
    //        {
    //            s.color = tgm.getRelationshipColor(g.force, u.force, g.id == u.id, false);
    //        }

    //        public override void drawTerritory(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf)
    //        {
    //            s.color = tgm.getRelationshipColor(g.force, c.force, g.id == c.lord, true);
    //        }

    //        public override void drawStronghold(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf)
    //        {
    //            s.color = tgm.getRelationshipColor(g.force, c.force, g.id == c.lord, true);
    //        }

    //        public override void drawUnit(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf)
    //        {
    //            s.color = tgm.getRelationshipColor(g.force, u.force, g.id == u.commander, false);
    //        }
    //    }

    //    public class SuzerainDiplomacyViewMode : ViewMode
    //    {
    //        public override void drawTerrain(TileGameMap tgm, AutoTileSprite s, Tile t)
    //        {
    //            s.color = Color.White;
    //        }

    //        public override void drawRange(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf)
    //        {
    //            s.color = tgm.getRelationshipColor(
    //                g.isHomeless ? g.force : gf.isIndependence ? gf.suzerain : gf.id,
    //                u.isHomeless ? u.force : uf.isIndependence ? uf.suzerain : uf.id,
    //                g.id == u.id, false);
    //        }

    //        public override void drawTerritory(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf)
    //        {
    //            s.color = tgm.getRelationshipColor(
    //                g.isHomeless ? g.force : gf.isIndependence ? gf.suzerain : gf.id,
    //                c.isHomeless ? c.force : cf.isIndependence ? cf.suzerain : cf.id,
    //                g.id == c.lord, true);
    //        }

    //        public override void drawStronghold(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf)
    //        {
    //            s.color = tgm.getRelationshipColor(
    //                g.isHomeless ? g.force : gf.isIndependence ? gf.suzerain : gf.id,
    //                c.isHomeless ? c.force : cf.isIndependence ? cf.suzerain : cf.id,
    //                g.id == c.lord, true);
    //        }

    //        public override void drawUnit(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf)
    //        {
    //            s.color = tgm.getRelationshipColor(
    //                g.isHomeless ? g.force : gf.isIndependence ? gf.suzerain : gf.id,
    //                u.isHomeless ? u.force : uf.isIndependence ? uf.suzerain : uf.id,
    //                g.id == u.commander, false);
    //        }
    //    }

    //    public class OrbitDiplomacyViewMode : ViewMode
    //    {
    //        public override void drawTerrain(TileGameMap tgm, AutoTileSprite s, Tile t)
    //        {
    //            s.color = Color.White;
    //        }

    //        public override void drawRange(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf)
    //        {
    //            s.color = tgm.getRelationshipColor(
    //                !g.isHomeless && gf.isBelongToOrbit ? gf.orbit : g.force,
    //                !u.isHomeless && uf.isBelongToOrbit ? uf.orbit : u.force,
    //                g.id == u.id, false);
    //        }

    //        public override void drawTerritory(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf)
    //        {
    //            s.color = tgm.getRelationshipColor(
    //                !g.isHomeless && gf.isBelongToOrbit ? gf.orbit : g.force,
    //                !c.isHomeless && cf.isBelongToOrbit ? cf.orbit : c.force,
    //                g.id == c.lord, true);
    //        }

    //        public override void drawStronghold(TileGameMap tgm, Sprite s, General g, Stronghold c, Force gf, Force cf)
    //        {
    //            s.color = tgm.getRelationshipColor(
    //                !g.isHomeless && gf.isBelongToOrbit ? gf.orbit : g.force,
    //                !c.isHomeless && cf.isBelongToOrbit ? cf.orbit : c.force,
    //                g.id == c.lord, true);
    //        }

    //        public override void drawUnit(TileGameMap tgm, Sprite s, General g, Unit u, Force gf, Force uf)
    //        {
    //            s.color = tgm.getRelationshipColor(
    //                !g.isHomeless && gf.isBelongToOrbit ? gf.orbit : g.force,
    //                !u.isHomeless && uf.isBelongToOrbit ? uf.orbit : u.force,
    //                g.id == u.commander, false);
    //        }
    //    }
    //    #endregion
    //}
}
