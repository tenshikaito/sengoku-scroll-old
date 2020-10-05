using Client.Command;
using Client.Game;
using Client.Graphic;
using Client.Helper;
using Client.Model;
using Client.UI;
using Client.UI.SceneTitle;
using Library;
using Library.Helper;
using Library.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.Scene
{
    public partial class SceneGame : SceneBase
    {
        private GameWorld gameWorld;

        private TileMapStatus mainTileMapStatus;

        private Camera camera => gameWorld.camera;

        public SceneGame(GameSystem gs, GameWorld gw) : base(gs)
        {
            gameWorld = gw;

            mainTileMapStatus = new TileMapStatus(this);
        }

        public override void start()
        {
            switchStatus(mainTileMapStatus);
        }

        public override void finish()
        {
            mainTileMapStatus.finish();
        }
    }

    public partial class SceneGame : SceneBase
    {
        public class Status : GameObject
        {
            protected SceneGame scene;

            public Status(SceneGame s) => scene = s;

            public virtual void setDrawMode(DrawMode dm)
            {

            }

            protected void onPointerButtonClicked() => setDrawMode(DrawMode.pointer);

            protected void onBrushButtonClicked() => setDrawMode(DrawMode.brush);

            protected void onRectangleButtonClicked() => setDrawMode(DrawMode.rectangle);

            public enum DrawMode
            {
                pointer,
                brush,
                rectangle,
            }
        }

        public class TileMapStatus : Status
        {
            private ZoomableTileMapSprites<TileMapSprites> zoomableTileMapSprites;
            private TileMapSprites.MapSpritesInfo mainMapSpritesInfo;

            private UITileInfoPanel uiTileInfoPanel;

            private PointerStatus pointerStatus;
            private DrawTileStatus drawTileStatus;
            private DrawTileRectangleStatus drawTileRectangleStatus;

            private Status currentStatus;

            public TileMapSprites tileMap => zoomableTileMapSprites.tileMapSprites;

            public TileMapStatus(SceneGame s) : base(s)
            {
                mainMapSpritesInfo = new TileMapSprites.MapSpritesInfo(s.gameWorld);

                zoomableTileMapSprites = new ZoomableTileMapSprites<TileMapSprites>();

                addChild(zoomableTileMapSprites);

                uiTileInfoPanel = new UITileInfoPanel(s.gameSystem, new Point(30, s.formMain.Height - 108));

                addChild(uiTileInfoPanel);

                loadMap();

                //uiEditGameWorldMainTileMapMenuWindow = new UIEditGameWorldMainTileMapMenuWindow(
                //    scene.gameSystem,
                //    onPointerButtonClicked,
                //    onBrushButtonClicked,
                //    onRectangleButtonClicked,
                //    onFillButtonClicked,
                //    onDetailTileMapButtonClicked,
                //    scene.onTerrainSelected);

                pointerStatus = new PointerStatus(this);
                drawTileStatus = new DrawTileStatus(this);
                drawTileRectangleStatus = new DrawTileRectangleStatus(this);
            }

            private void loadMap()
            {
                var s = scene;
                var msi = mainMapSpritesInfo;

                zoomableTileMapSprites.setTileMap(new List<TileMapSprites>()
                {
                    new MainTileMapViewSprites(s.gameSystem, s.gameWorld, msi, true),
                    new MainTileMapDetailSprites(s.gameSystem, s.gameWorld, msi, true)
                });
            }

            public override void start()
            {
                //uiEditGameWorldMainTileMapMenuWindow.Show(scene.formMain);

                setDrawMode(DrawMode.pointer);

                //uiEditGameWorldMainTileMapMenuWindow.setTerrain(
                //    scene.gameWorld.masterData.mainTileMapTerrain.Values.ToList(),
                //    scene.gameWorld.masterData.terrainImage);
            }

            public override void finish()
            {
                //uiEditGameWorldMainTileMapMenuWindow.Hide();

                //uiEditGameWorldDetailTileMapListDialog?.Close();
                //uiEditGameWorldDetailTileMapListDialog = null;
            }

            public override void mouseMoved(MouseEventArgs e)
            {
                var gw = scene.gameWorld;
                var gm = gw.gameWorldData;
                var tm = gm.tileMap;

                var cursorPos = tileMap.cursorPosition;

                if (tm.isOutOfBounds(cursorPos))
                {
                    uiTileInfoPanel.setText(null, null, null);

                    return;
                }

                var t = tm[cursorPos];
                var mt = gm.masterData.tileMapTerrain;

                mt.TryGetValue(t.terrainSurface ?? t.terrain, out var tt); ;

                uiTileInfoPanel.setText(tt.name, null, null);
            }

            public override void mouseWheelScrolled(MouseEventArgs e)
            {
                var cursorPos = tileMap.cursorPosition;
                var camera = scene.camera;
                var center = camera.center;
                var sCenter = camera.translateWorldToScreen(center);
                var tileVertex = tileMap.getTileLocation(sCenter);
                bool flag;

                if (e.Delta >= 0) flag = zoomableTileMapSprites.next();
                else flag = zoomableTileMapSprites.previous();

                if (flag)
                {
                    tileMap.cursorPosition = cursorPos;

                    if (e.Delta >= 0) camera.center = new Point(cursorPos.x * tileMap.tileWidth, cursorPos.y * tileMap.tileHeight);
                    else camera.center = new Point(tileVertex.x * tileMap.tileWidth, tileVertex.y * tileMap.tileHeight);
                }
            }

            private void onDetailTileMapButtonClicked()
            {
                //if (uiEditGameWorldDetailTileMapListDialog == null)
                //{
                //    uiEditGameWorldDetailTileMapListDialog = new UIEditGameWorldDetailTileMapListDialog(
                //        scene.gameSystem, scene.gameWorld.masterData.detailTileMapInfo, scene.onDetailTileMapSelected)
                //    {
                //        okButtonClicked = () =>
                //        {
                //            uiEditGameWorldDetailTileMapListDialog.Close();
                //            uiEditGameWorldDetailTileMapListDialog = null;
                //        }
                //    };

                //    uiEditGameWorldDetailTileMapListDialog.Show(scene.formMain);
                //}
            }

            public override void setDrawMode(DrawMode dm)
            {
                if (currentStatus != null) children.Remove(currentStatus);

                switch (dm)
                {
                    case DrawMode.pointer: addStatus(pointerStatus); break;
                    case DrawMode.brush: addStatus(drawTileStatus); break;
                    case DrawMode.rectangle: addStatus(drawTileRectangleStatus); break;
                }
            }

            private void addStatus(Status s) => addChild(currentStatus = s);

            public void refreshTileMap() => loadMap();

            public class Status : GameObject
            {
                protected TileMapStatus gameStatus;

                protected SceneGame scene => gameStatus.scene;

                protected GameSystem gameSystem => scene.gameSystem;

                protected GameWorld gameWorld => scene.gameWorld;

                protected GameWorldData gameWorldData => gameWorld.gameWorldData;

                protected TileMap tileMap => gameWorldData.tileMap;

                public Status(TileMapStatus s) => gameStatus = s;
            }

            public class PointerStatus : Status
            {
                public PointerStatus(TileMapStatus s) : base(s)
                {
                }

                public override void mouseDragging(MouseEventArgs e, Point p)
                {
                    gameStatus.scene.camera.dragCamera(p);
                }
            }

            public class DrawTileStatus : Status
            {
                public DrawTileStatus(TileMapStatus s) : base(s)
                {
                }

                public override void mouseReleased(MouseEventArgs e) => draw(e);

                public override void mouseDragging(MouseEventArgs e, Point p) => draw(e);

                private void draw(MouseEventArgs e)
                {
                    //var p = gameStatus.tileMap.getTileLocation(e);

                    //if (tileMap.isOutOfBounds(p)) return;

                    //switch (scene.drawContent)
                    //{
                    //    case DrawContent.terrain:

                    //        var tid = (byte)scene.drawContentId;

                    //        var t = scene.gameWorld.masterData.mainTileMapTerrain[tid];

                    //        tileMap.setTerrain(p, tid, t.isSurface);

                    //        if (!t.isSurface) tileMap.terrainSurface.Remove(tileMap.getIndex(p));

                    //        gameStatus.mainMapSpritesInfo.resetTileFlag(p);

                    //        break;
                    //}
                }
            }

            public class DrawTileRectangleStatus : Status
            {
                public Point? startPoint;
                public SpriteRectangle selector;

                public DrawTileRectangleStatus(TileMapStatus s) : base(s)
                {
                    selector = new SpriteRectangle()
                    {
                        color = Color.White,
                        width = 1,
                    };
                }

                public override void mousePressed(MouseEventArgs e)
                {
                    if (gameWorldData.tileMap.isOutOfBounds(gameStatus.tileMap.cursorPosition)) return;

                    startPoint = e.Location;

                    selector.position = e.Location;
                    selector.size = new Size(1, 1);
                }

                public override void mouseReleased(MouseEventArgs e)
                {
                    //if (startPoint == null) return;

                    //startPoint = null;

                    //switch (scene.drawContent)
                    //{
                    //    case DrawContent.terrain:

                    //        var tlp = selector.position;
                    //        var trp = new Point(selector.position.X + selector.size.Width, selector.position.Y);
                    //        var blp = new Point(selector.position.X, selector.position.Y + selector.size.Height);

                    //        var tl = gameStatus.tileMap.getTileLocation(tlp);
                    //        var tr = gameStatus.tileMap.getTileLocation(trp);
                    //        var bl = gameStatus.tileMap.getTileLocation(blp);

                    //        var tm = gameWorld.mainTileMap;

                    //        tm.checkBound(ref tl);
                    //        tm.checkBound(ref tr);
                    //        tm.checkBound(ref bl);

                    //        ++tr.x;
                    //        ++bl.y;

                    //        var width = tr.x - tl.x;
                    //        var height = bl.y - tl.y;
                    //        var tid = (byte)scene.drawContentId;
                    //        var t = scene.gameWorld.masterData.mainTileMapTerrain[tid];

                    //        tm.eachRectangle(tl, new TileMap.Size(height, width), o =>
                    //        {
                    //            tileMap.setTerrain(o, tid, t.isSurface);

                    //            if (!t.isSurface) tileMap.terrainSurface.Remove(tileMap.getIndex(o));

                    //            gameStatus.mainMapSpritesInfo.resetTileFlag(o);
                    //        });

                    //        break;
                    //}

                    //selector.size = Size.Empty;
                }

                public override void mouseDragging(MouseEventArgs e, Point p)
                {
                    if (startPoint == null) return;

                    var sp = startPoint.Value;

                    selector.position = sp;
                    selector.size = new Size(e.X - sp.X, e.Y - sp.Y);
                }

                public override void draw()
                {
                    if (startPoint == null) return;

                    gameSystem.gameGraphic.drawRectangle(selector);
                }
            }
        }
    }
}
