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
        private MainTileMapStatus mainTileMapStatus;
        private DetailTileMapStatus detailTileMapStatus;

        private UIConfirmDialog uiConfirmDialog;

        private UIEditGameWorldMenuWindow uiEditGameWorldMenuWindow;

        private UIEditGameWorldDatabaseWindow uiEditGameWorldDatabaseWindow;

        private DrawContent drawContent = DrawContent.terrain;
        private int drawContentId = 0;

        private Camera camera => gameWorld.camera;

        public SceneEditGameWorld(GameSystem gs, GameWorld gw) : base(gs)
        {
            gameWorld = gw;

            waitingStatus = new SceneWaiting(gs, gs.wording.loading);
            mainTileMapStatus = new MainTileMapStatus(this);
            detailTileMapStatus = new DetailTileMapStatus(this);

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

            switchStatus(mainTileMapStatus);
        }

        public override void finish()
        {
            mainTileMapStatus.finish();

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

            mainTileMapStatus.refreshTileMap();
        }

        private void onDatabaseWindowApplyButtonClicked()
        {
            saveDatabase();

            new UIDialog(gameSystem, "alert", "saved").ShowDialog();

            mainTileMapStatus.refreshTileMap();
        }

        private void saveDatabase()
        {
            gameWorld.masterData = uiEditGameWorldDatabaseWindow.gameWorldMasterData;
        }

        private void onSaveButtonClicked()
        {
            try
            {
                save();

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

                gameSystem.sceneToTitle();
            };

            dialog.cancelButtonClicked = () =>
            {
                dialog.Close();

                uiConfirmDialog = null;
            };

            dialog.ShowDialog(formMain);
        }

        private void save()
        {
            gameWorld.gameWorldProcessor.map.saveMasterData(gameWorld);
        }

        private void onDetailTileMapSelected(int id)
        {
            switchStatus(waitingStatus);

            Task.Run(() =>
            {
                var gwp = gameWorld.gameWorldProcessor;

                var itm = gameWorld.masterData.detailTileMapInfo[id];

                var tm = gwp.map.loadDetailTileMap(id, itm.size.column, itm.size.row);

                detailTileMapStatus.setTileMap(id, tm);

                detailTileMapStatus.resetFlag();

                dispatcher.invoke(() => switchStatus(detailTileMapStatus));
            });
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

        public class MainTileMapStatus : Status
        {
            private ZoomableTileMapSprites<MainTileMapSprites> zoomableTileMapSprites;
            private MainTileMapSprites.MainMapSpritesInfo mainMapSpritesInfo;

            private UITileInfoPanel uiTileInfoPanel;

            private UIEditGameWorldMainTileMapMenuWindow uiEditGameWorldMainTileMapMenuWindow;
            private UIEditGameWorldDetailTileMapListDialog uiEditGameWorldDetailTileMapListDialog;

            private PointerStatus pointerStatus;
            private DrawTileStatus drawTileStatus;
            private DrawTileRectangleStatus drawTileRectangleStatus;
            private DrawTileFillStatus drawTileFillStatus;

            private Status currentStatus;

            public MainTileMapSprites tileMap => zoomableTileMapSprites.tileMapSprites;

            public MainTileMapStatus(SceneEditGameWorld s) : base(s)
            {
                mainMapSpritesInfo = new MainTileMapSprites.MainMapSpritesInfo(s.gameWorld);

                zoomableTileMapSprites = new ZoomableTileMapSprites<MainTileMapSprites>();

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
                    onDetailTileMapButtonClicked,
                    scene.onTerrainSelected);

                pointerStatus = new PointerStatus(this);
                drawTileStatus = new DrawTileStatus(this);
                drawTileRectangleStatus = new DrawTileRectangleStatus(this);
                drawTileFillStatus = new DrawTileFillStatus(this);
            }

            private void loadMap()
            {
                var s = scene;
                var msi = mainMapSpritesInfo;

                zoomableTileMapSprites.setTileMap(new List<MainTileMapSprites>()
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
                    scene.gameWorld.masterData.mainTileMapTerrain.Values.ToList(),
                    scene.gameWorld.masterData.terrainImage);
            }

            public override void finish()
            {
                uiEditGameWorldMainTileMapMenuWindow.Hide();

                uiEditGameWorldDetailTileMapListDialog?.Close();
                uiEditGameWorldDetailTileMapListDialog = null;
            }

            public override void mouseMoved(MouseEventArgs e)
            {
                var gw = scene.gameWorld;
                var tm = gw.mainTileMap;

                var cursorPos = tileMap.cursorPosition;

                if (tm.isOutOfBounds(cursorPos))
                {
                    uiTileInfoPanel.setText(null, null, null);

                    return;
                }

                var t = tm[cursorPos];
                var mt = gw.masterData.mainTileMapTerrain;

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
                if (uiEditGameWorldDetailTileMapListDialog == null)
                {
                    uiEditGameWorldDetailTileMapListDialog = new UIEditGameWorldDetailTileMapListDialog(
                        scene.gameSystem, scene.gameWorld.masterData.detailTileMapInfo, scene.onDetailTileMapSelected)
                    {
                        okButtonClicked = () =>
                        {
                            uiEditGameWorldDetailTileMapListDialog.Close();
                            uiEditGameWorldDetailTileMapListDialog = null;
                        }
                    };

                    uiEditGameWorldDetailTileMapListDialog.Show(scene.formMain);
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
                protected MainTileMapStatus gameStatus;

                protected SceneEditGameWorld scene => gameStatus.scene;

                protected GameSystem gameSystem => scene.gameSystem;

                protected GameWorld gameWorld => scene.gameWorld;

                protected MainTileMap tileMap => gameStatus.mainMapSpritesInfo.mainTileMap;

                public Status(MainTileMapStatus s) => gameStatus = s;
            }

            public class PointerStatus : Status
            {
                public PointerStatus(MainTileMapStatus s) : base(s)
                {
                }

                public override void mouseDragging(MouseEventArgs e, Point p)
                {
                    gameStatus.scene.camera.dragCamera(p);
                }
            }

            public class DrawTileStatus : Status
            {
                public DrawTileStatus(MainTileMapStatus s) : base(s)
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

                            var t = scene.gameWorld.masterData.mainTileMapTerrain[tid];

                            tileMap.setTerrain(p, tid, t.isSurface);

                            if (!t.isSurface) tileMap.terrainSurface.Remove(tileMap.getIndex(p));

                            gameStatus.mainMapSpritesInfo.resetTileFlag(p);

                            break;
                    }
                }
            }

            public class DrawTileRectangleStatus : Status
            {
                public Point? startPoint;
                public SpriteRectangle selector;

                public DrawTileRectangleStatus(MainTileMapStatus s) : base(s)
                {
                    selector = new SpriteRectangle()
                    {
                        color = Color.White,
                        width = 1,
                    };
                }

                public override void mousePressed(MouseEventArgs e)
                {
                    if (gameWorld.mainTileMap.isOutOfBounds(gameStatus.tileMap.cursorPosition)) return;

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

                            var tm = gameWorld.mainTileMap;

                            tm.checkBound(ref tl);
                            tm.checkBound(ref tr);
                            tm.checkBound(ref bl);

                            ++tr.x;
                            ++bl.y;

                            var width = tr.x - tl.x;
                            var height = bl.y - tl.y;
                            var tid = (byte)scene.drawContentId;
                            var t = scene.gameWorld.masterData.mainTileMapTerrain[tid];

                            tm.eachRectangle(tl, new TileMap.Size(height, width), o =>
                            {
                                tileMap.setTerrain(o, tid, t.isSurface);

                                if (!t.isSurface) tileMap.terrainSurface.Remove(tileMap.getIndex(o));

                                gameStatus.mainMapSpritesInfo.resetTileFlag(o);
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

                public DrawTileFillStatus(MainTileMapStatus s) : base(s)
                {
                }

                public override void mouseReleased(MouseEventArgs e)
                {
                    var p = scene.mainTileMapStatus.tileMap.getTileLocation(e);

                    if (tileMap.isOutOfBounds(p)) return;

                    var t = tileMap[p];

                    var tdid = (byte)scene.drawContentId;
                    var td = scene.gameWorld.masterData.mainTileMapTerrain[tdid];

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

                    list.ForEach(gameStatus.mainMapSpritesInfo.resetTileFlag);
                }

                private void add(int x, int y) => points.Push(new MapPoint(x, y));
            }
        }


        public class DetailTileMapStatus : Status
        {
            private ZoomableTileMapSprites<DetailTileMapSprites> zoomableTileMapSprites;
            private DetailTileMapSprites.DetailMapSpritesInfo detailMapSpritesInfo;

            private UIEditGameWorldDetailTileMapMenuWindow uiEditGameWorldDetailTileMapMenuWindow;

            private PointerStatus pointerStatus;
            private DrawTileStatus drawTileStatus;
            private DrawTileRectangleStatus drawTileRectangleStatus;
            private DrawTileFillStatus drawTileFillStatus;

            private Status currentStatus;

            private int currentTileMapId;

            public DetailTileMapSprites tileMap => zoomableTileMapSprites.tileMapSprites;

            public DetailTileMapStatus(SceneEditGameWorld s) : base(s)
            {
                detailMapSpritesInfo = new DetailTileMapSprites.DetailMapSpritesInfo(s.gameWorld);

                var list = new TileMapImageInfo[]
                {
                    s.gameWorld.masterData.detailTileMapViewImageInfo,
                    s.gameWorld.masterData.detailTileMapDetailImageInfo,
                };

                zoomableTileMapSprites = new ZoomableTileMapSprites<DetailTileMapSprites>(
                    list.Select(o => new DetailTileMapSprites(s.gameSystem, s.gameWorld, o, detailMapSpritesInfo, true)).ToList());

                children.Add(zoomableTileMapSprites);

                zoomableTileMapSprites.previous();

                uiEditGameWorldDetailTileMapMenuWindow = new UIEditGameWorldDetailTileMapMenuWindow(
                    scene.gameSystem,
                    onPointerButtonClicked,
                    onBrushButtonClicked,
                    onRectangleButtonClicked,
                    onFillButtonClicked,
                    onSaveButtonClicked,
                    onExitButtonClicked,
                    scene.onTerrainSelected);

                pointerStatus = new PointerStatus(this);
                drawTileStatus = new DrawTileStatus(this);
                drawTileRectangleStatus = new DrawTileRectangleStatus(this);
                drawTileFillStatus = new DrawTileFillStatus(this);
            }

            public void setTileMap(int id, DetailTileMap tm)
            {
                currentTileMapId = id;
                detailMapSpritesInfo.detailTileMap = tm;
            }

            public void resetFlag() => detailMapSpritesInfo.removeTileFlag();

            public override void start()
            {
                scene.uiEditGameWorldMenuWindow.Visible = false;

                uiEditGameWorldDetailTileMapMenuWindow.setTerrain(scene.gameWorld.masterData.terrainImage.Values.ToList());

                uiEditGameWorldDetailTileMapMenuWindow.Show(scene.formMain);

                setDrawMode(DrawMode.pointer);
            }

            public override void finish()
            {
                scene.uiEditGameWorldMenuWindow.Visible = true;

                uiEditGameWorldDetailTileMapMenuWindow.Hide();
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

            private void onSaveButtonClicked()
            {
                try
                {
                    var gwp = scene.gameWorld.gameWorldProcessor;

                    gwp.map.saveDetailTileMap(currentTileMapId, detailMapSpritesInfo.detailTileMap);

                    new UIDialog(scene.gameSystem, "alert", "saved.").Show(scene.formMain);
                }
                catch (Exception e)
                {
                    new UIDialog(scene.gameSystem, "alert", "error: " + e.Message).Show(scene.formMain);
                }

            }

            private void onExitButtonClicked()
            {
                if (scene.uiConfirmDialog != null) return;

                var dialog = scene.uiConfirmDialog = new UIConfirmDialog(scene.gameSystem, "confirm", "exit?");

                dialog.okButtonClicked = () =>
                {
                    dialog.Close();

                    scene.uiConfirmDialog = null;

                    scene.switchStatus(scene.mainTileMapStatus);
                };

                dialog.cancelButtonClicked = () =>
                {
                    dialog.Close();

                    scene.uiConfirmDialog = null;
                };

                dialog.ShowDialog(scene.formMain);
            }

            public class Status : GameObject
            {
                protected DetailTileMapStatus gameStatus;

                protected SceneEditGameWorld scene => gameStatus.scene;

                protected GameSystem gameSystem => scene.gameSystem;

                protected GameWorld gameWorld => scene.gameWorld;

                protected DetailTileMap tileMap => gameStatus.detailMapSpritesInfo.detailTileMap;

                public Status(DetailTileMapStatus s) => gameStatus = s;
            }

            public class PointerStatus : Status
            {
                public PointerStatus(DetailTileMapStatus s) : base(s)
                {
                }

                public override void mouseDragging(MouseEventArgs e, Point p)
                {
                    gameStatus.scene.camera.dragCamera(p);
                }
            }

            public class DrawTileStatus : Status
            {
                public DrawTileStatus(DetailTileMapStatus s) : base(s)
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

                            tileMap.setTerrain(p, (byte)scene.drawContentId);

                            gameStatus.detailMapSpritesInfo.resetTileFlag(p);

                            break;
                    }
                }
            }

            public class DrawTileRectangleStatus : Status
            {
                public Point? startPoint;
                public SpriteRectangle selector;

                public DrawTileRectangleStatus(DetailTileMapStatus s) : base(s)
                {
                    selector = new SpriteRectangle()
                    {
                        color = Color.White,
                        width = 1,
                    };
                }

                public override void mousePressed(MouseEventArgs e)
                {
                    if (gameWorld.mainTileMap.isOutOfBounds(gameStatus.tileMap.cursorPosition)) return;

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

                            var tm = gameWorld.mainTileMap;

                            tm.checkBound(ref tl);
                            tm.checkBound(ref tr);
                            tm.checkBound(ref bl);

                            ++tr.x;
                            ++bl.y;

                            var width = tr.x - tl.x;
                            var height = bl.y - tl.y;
                            var t = (byte)scene.drawContentId;

                            tm.eachRectangle(tl, new TileMap.Size(height, width), o =>
                            {
                                tileMap.setTerrain(o, t);

                                gameStatus.detailMapSpritesInfo.resetTileFlag(o);
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

                public DrawTileFillStatus(DetailTileMapStatus s) : base(s)
                {
                }

                public override void mouseReleased(MouseEventArgs e)
                {
                    var p = scene.mainTileMapStatus.tileMap.getTileLocation(e);

                    if (tileMap.isOutOfBounds(p)) return;

                    var t = tileMap[p];

                    selectedTerrainId = t.terrain;

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

                        if (tileMap[p].terrain == selectedTerrainId)
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

                    var terrainId = (byte)scene.drawContentId;

                    var list = markedPoints.ToList();

                    list.ForEach(o => tileMap.setTerrain(o, terrainId));

                    list.ForEach(gameStatus.detailMapSpritesInfo.resetTileFlag);
                }

                private void add(int x, int y) => points.Push(new MapPoint(x, y));
            }
        }
    }
}
