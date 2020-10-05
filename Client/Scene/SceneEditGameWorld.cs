using Client.Game;
using Client.Graphic;
using Client.Helper;
using Client.UI;
using Client.UI.SceneEditGameWorld;
using Library;
using Library.Helper;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.Scene
{
    public partial class SceneEditGameWorld : SceneBase
    {
        private GameWorld gameWorld;

        private SceneWaiting waitingStatus;
        private TileMapStatus tileMapStatus;

        private UIConfirmDialog uiConfirmDialog;

        private UIEditGameWorldMenuWindow uiEditGameWorldMenuWindow;

        private UIEditGameWorldDatabaseWindow uiEditGameWorldDatabaseWindow;

        private DrawContent drawContent = DrawContent.terrain;
        private int drawContentId = 0;

        private GameWorldData gameMap => gameWorld.gameWorldData;
        private Camera camera => gameWorld.camera;

        public SceneEditGameWorld(GameSystem gs, GameWorld gw) : base(gs)
        {
            gameWorld = gw;

            waitingStatus = new SceneWaiting(gs, gs.wording.loading);
            tileMapStatus = new TileMapStatus(this);

            uiEditGameWorldMenuWindow = new UIEditGameWorldMenuWindow(
                gs,
                onDatabaseButtonClicked,
                onSaveButtonClicked,
                onExitButtonClicked);
        }

        private enum DrawContent
        {
            terrain
        }

        private void onTerrainSelected(int id)
        {
            drawContent = DrawContent.terrain;

            drawContentId = id;
        }

        public override void start()
        {
            uiEditGameWorldMenuWindow.Show(formMain);

            switchStatus(tileMapStatus);
        }

        public override void finish()
        {
            tileMapStatus.finish();

            uiEditGameWorldMenuWindow.Close();
        }

        private void onDatabaseButtonClicked()
        {
            if (uiEditGameWorldDatabaseWindow != null) return;

            uiEditGameWorldDatabaseWindow = new UIEditGameWorldDatabaseWindow(gameSystem, gameWorld)
            {
                okButtonClicked = onDatabaseWindowOkButtonClicked,
                cancelButtonClicked = () => uiEditGameWorldDatabaseWindow.Close(),
                applyButtonClicked = onDatabaseWindowApplyButtonClicked
            };

            uiEditGameWorldDatabaseWindow.FormClosing += (s, e) => uiEditGameWorldDatabaseWindow = null;
            uiEditGameWorldDatabaseWindow.ShowDialog(formMain);
        }

        private void onDatabaseWindowOkButtonClicked()
        {
            saveDatabase();

            uiEditGameWorldDatabaseWindow.Close();

            tileMapStatus.refreshTileMap();
        }

        private void onDatabaseWindowApplyButtonClicked()
        {
            saveDatabase();

            new UIDialog(gameSystem, "alert", "saved").ShowDialog();

            tileMapStatus.refreshTileMap();
        }

        private void saveDatabase()
        {
            gameMap.masterData = uiEditGameWorldDatabaseWindow.gameWorldMasterData;
        }

        private async void onSaveButtonClicked()
        {
            try
            {
                await save();

                new UIDialog(gameSystem, "message", "data saved.").ShowDialog(formMain);
            }
            catch (Exception e)
            {
                new UIDialog(gameSystem, "error", "save error!" + e.Message).ShowDialog(formMain);
            }
        }

        private void onExitButtonClicked()
        {
            if (uiConfirmDialog != null) return;

            var dialog = uiConfirmDialog = new UIConfirmDialog(gameSystem, "confirm", "exit?");

            dialog.okButtonClicked = () =>
            {
                dialog.Close();

                uiConfirmDialog = null;

                gameSystem.sceneToTitle(true);
            };

            dialog.cancelButtonClicked = () =>
            {
                dialog.Close();

                uiConfirmDialog = null;
            };

            dialog.ShowDialog(formMain);
        }

        private async Task save()
        {
            await gameWorld.gameWorldManager.map.saveGameWorldData(gameMap);
        }

        public enum DrawMode
        {
            pointer,
            brush,
            rectangle,
            fill
        }
    }

    public partial class SceneEditGameWorld
    {
        public class Status : GameObject
        {
            protected SceneEditGameWorld scene;

            public Status(SceneEditGameWorld s) => scene = s;

            public virtual void setDrawMode(DrawMode dm)
            {

            }

            protected void onPointerButtonClicked() => setDrawMode(DrawMode.pointer);

            protected void onBrushButtonClicked() => setDrawMode(DrawMode.brush);

            protected void onRectangleButtonClicked() => setDrawMode(DrawMode.rectangle);

            protected void onFillButtonClicked() => setDrawMode(DrawMode.fill);
        }

        public class TileMapStatus : Status
        {
            private ZoomableTileMapSprites<TileMapSprites> zoomableTileMapSprites;
            private TileMapSprites.MapSpritesInfo mapSpritesInfo;

            private UITileInfoPanel uiTileInfoPanel;

            private UIEditGameWorldMainTileMapMenuWindow uiEditGameWorldMainTileMapMenuWindow;

            private PointerStatus pointerStatus;
            private DrawTileStatus drawTileStatus;
            private DrawTileRectangleStatus drawTileRectangleStatus;
            private DrawTileFillStatus drawTileFillStatus;

            private Status currentStatus;

            public TileMapSprites tileMap => zoomableTileMapSprites.tileMapSprites;

            public TileMapStatus(SceneEditGameWorld s) : base(s)
            {
                mapSpritesInfo = new TileMapSprites.MapSpritesInfo(s.gameWorld);

                zoomableTileMapSprites = new ZoomableTileMapSprites<TileMapSprites>();

                addChild(zoomableTileMapSprites);

                uiTileInfoPanel = new UITileInfoPanel(s.gameSystem, new Point(30, s.formMain.Height - 108));

                addChild(uiTileInfoPanel);

                loadMap();

                uiEditGameWorldMainTileMapMenuWindow = new UIEditGameWorldMainTileMapMenuWindow(
                    scene.gameSystem,
                    onPointerButtonClicked,
                    onBrushButtonClicked,
                    onRectangleButtonClicked,
                    onFillButtonClicked,
                    scene.onTerrainSelected);

                pointerStatus = new PointerStatus(this);
                drawTileStatus = new DrawTileStatus(this);
                drawTileRectangleStatus = new DrawTileRectangleStatus(this);
                drawTileFillStatus = new DrawTileFillStatus(this);
            }

            private void loadMap()
            {
                var s = scene;
                var msi = mapSpritesInfo;

                zoomableTileMapSprites.setTileMap(new List<TileMapSprites>()
                {
                    new MainTileMapViewSprites(s.gameSystem, s.gameWorld, msi, true),
                    new MainTileMapDetailSprites(s.gameSystem, s.gameWorld, msi, true)
                });
            }

            public override void start()
            {
                uiEditGameWorldMainTileMapMenuWindow.Show(scene.formMain);

                setDrawMode(DrawMode.pointer);

                uiEditGameWorldMainTileMapMenuWindow.setTerrain(
                    scene.gameMap.masterData.tileMapTerrain.Values.ToList(),
                    scene.gameMap.masterData.terrainImage);
            }

            public override void finish()
            {
                uiEditGameWorldMainTileMapMenuWindow.Hide();
            }

            public override void mouseMoved(MouseEventArgs e)
            {
                var gw = scene.gameWorld;
                var gwd = gw.gameWorldData;
                var tm = gwd.tileMap;

                var cursorPos = tileMap.cursorPosition;

                if (tm.isOutOfBounds(cursorPos))
                {
                    uiTileInfoPanel.setText(null, null, null);

                    return;
                }

                var t = tm[cursorPos];
                var mt = gwd.masterData.tileMapTerrain;

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

            public override void setDrawMode(DrawMode dm)
            {
                if (currentStatus != null) children.Remove(currentStatus);

                switch (dm)
                {
                    case DrawMode.pointer: addStatus(pointerStatus); break;
                    case DrawMode.brush: addStatus(drawTileStatus); break;
                    case DrawMode.rectangle: addStatus(drawTileRectangleStatus); break;
                    case DrawMode.fill: addStatus(drawTileFillStatus); break;
                }
            }

            private void addStatus(Status s) => addChild(currentStatus = s);

            public void refreshTileMap() => loadMap();

            public class Status : GameObject
            {
                protected TileMapStatus gameStatus;

                protected SceneEditGameWorld scene => gameStatus.scene;

                protected GameSystem gameSystem => scene.gameSystem;

                protected GameWorld gameWorld => scene.gameWorld;

                protected GameWorldData gameMap => scene.gameMap;

                protected TileMap tileMap => gameMap.tileMap;

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
                    var p = gameStatus.tileMap.getTileLocation(e);

                    if (tileMap.isOutOfBounds(p)) return;

                    switch (scene.drawContent)
                    {
                        case DrawContent.terrain:

                            var tid = (byte)scene.drawContentId;

                            var t = scene.gameMap.masterData.tileMapTerrain[tid];

                            tileMap.setTerrain(p, tid, t.isSurface);

                            if (!t.isSurface) tileMap.terrainSurface.Remove(tileMap.getIndex(p));

                            gameStatus.mapSpritesInfo.resetTileFlag(p);

                            break;
                    }
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
                    if (gameMap.tileMap.isOutOfBounds(gameStatus.tileMap.cursorPosition)) return;

                    startPoint = e.Location;

                    selector.position = e.Location;
                    selector.size = new Size(1, 1);
                }

                public override void mouseReleased(MouseEventArgs e)
                {
                    if (startPoint == null) return;

                    startPoint = null;

                    switch (scene.drawContent)
                    {
                        case DrawContent.terrain:

                            var tlp = selector.position;
                            var trp = new Point(selector.position.X + selector.size.Width, selector.position.Y);
                            var blp = new Point(selector.position.X, selector.position.Y + selector.size.Height);

                            var tl = gameStatus.tileMap.getTileLocation(tlp);
                            var tr = gameStatus.tileMap.getTileLocation(trp);
                            var bl = gameStatus.tileMap.getTileLocation(blp);

                            var tm = gameMap.tileMap;

                            tm.checkBound(ref tl);
                            tm.checkBound(ref tr);
                            tm.checkBound(ref bl);

                            ++tr.x;
                            ++bl.y;

                            var width = tr.x - tl.x;
                            var height = bl.y - tl.y;
                            var tid = (byte)scene.drawContentId;
                            var t = scene.gameMap.masterData.tileMapTerrain[tid];

                            tm.eachRectangle(tl, new TileMap.Size(height, width), o =>
                            {
                                tileMap.setTerrain(o, tid, t.isSurface);

                                if (!t.isSurface) tileMap.terrainSurface.Remove(tileMap.getIndex(o));

                                gameStatus.mapSpritesInfo.resetTileFlag(o);
                            });

                            break;
                    }

                    selector.size = Size.Empty;
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

            public class DrawTileFillStatus : Status
            {
                private Stack<MapPoint> points = new Stack<MapPoint>();
                private HashSet<MapPoint> foundPoints = new HashSet<MapPoint>();
                private HashSet<MapPoint> markedPoints = new HashSet<MapPoint>();
                private byte selectedTerrainId;
                private byte? selectedTerrainSurfaceId;

                public DrawTileFillStatus(TileMapStatus s) : base(s)
                {
                }

                public override void mouseReleased(MouseEventArgs e)
                {
                    var p = scene.tileMapStatus.tileMap.getTileLocation(e);

                    if (tileMap.isOutOfBounds(p)) return;

                    var t = tileMap[p];

                    var tdid = (byte)scene.drawContentId;
                    var td = scene.gameMap.masterData.tileMapTerrain[tdid];

                    selectedTerrainId = t.terrain;
                    selectedTerrainSurfaceId = t.terrainSurface;

                    markedPoints.Clear();
                    foundPoints.Clear();
                    points.Clear();

                    points.Push(p);

                    while (true)
                    {
                        if (!points.Any()) break;

                        p = points.Peek();

                        if (tileMap.isOutOfBounds(p) || foundPoints.Contains(p))
                        {
                            points.Pop();

                            continue;
                        }

                        foundPoints.Add(p);

                        t = tileMap[p];

                        if (t.terrain == selectedTerrainId
                            && t.terrainSurface == selectedTerrainSurfaceId)
                        {
                            markedPoints.Add(p);

                            add(p.x + 1, p.y);
                            add(p.x - 1, p.y);
                            add(p.x, p.y + 1);
                            add(p.x, p.y - 1);
                        }
                        else
                        {
                            points.Pop();
                        }
                    }

                    var list = markedPoints.ToList();

                    list.ForEach(o => tileMap.setTerrain(o, tdid, td.isSurface));

                    if (!td.isSurface) list.ForEach(o => tileMap.terrainSurface.Remove(tileMap.getIndex(o)));

                    list.ForEach(gameStatus.mapSpritesInfo.resetTileFlag);
                }

                private void add(int x, int y) => points.Push(new MapPoint(x, y));
            }
        }
    }
}
