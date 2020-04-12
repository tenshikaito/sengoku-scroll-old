using Client.Graphic;
using Client.Helper;
using Client.UI;
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

        private UIEditGameWorldDetailTileMapDialog uiEditGameWorldDetailTileMapDialog;

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
                onPointerButtonClicked,
                onBrushButtonClicked,
                onRectangleButtonClicked,
                onFillButtonClicked,
                onDetailTileMapButtonClicked,
                onDatabaseButtonClicked,
                onSaveButtonClicked,
                onExitButtonClicked,
                onTerrainSelected);

            uiEditGameWorldMenuWindow.setTerrain(gameWorld.masterData.terrain.Values.ToList());
        }

        private enum DrawContent
        {
            terrain
        }

        private void onTerrainSelected(byte id)
        {
            drawContent = DrawContent.terrain;

            drawContentId = id;
        }

        public override void start()
        {
            switchStatus(mainTileMapStatus);
        }

        public override void finish()
        {
            uiEditGameWorldMenuWindow.Close();
        }

        private void onPointerButtonClicked() => (status as Status).setDrawMode(DrawMode.pointer);

        private void onBrushButtonClicked() => (status as Status).setDrawMode(DrawMode.brush);

        private void onRectangleButtonClicked() => (status as Status).setDrawMode(DrawMode.rectangle);

        private void onFillButtonClicked() => (status as Status).setDrawMode(DrawMode.fill);

        private void onDetailTileMapButtonClicked()
        {
            if (uiEditGameWorldDetailTileMapDialog == null)
            {
                uiEditGameWorldDetailTileMapDialog = new UIEditGameWorldDetailTileMapDialog(
                    gameSystem, gameWorld.masterData.detailTileMapInfo, onDetailTileMapSelected)
                {
                    okButtonClicked = () =>
                    {
                        uiEditGameWorldDetailTileMapDialog.Close();
                        uiEditGameWorldDetailTileMapDialog = null;
                    }
                };

                uiEditGameWorldDetailTileMapDialog.Show(formMain);
            }
        }

        private void onDetailTileMapSelected(int id)
        {
            switchStatus(waitingStatus);

            Task.Run(() =>
            {
                var gwp = gameWorld.getGameWorldProcessor();

                var itm = gameWorld.masterData.detailTileMapInfo[id];

                var tm = gwp.loadDetailTileMap(id, itm.size.column, itm.size.row);

                detailTileMapStatus.setTileMap(tm);

                detailTileMapStatus.resetFlag();

                dispatcher.invoke(() => switchStatus(detailTileMapStatus));
            });
        }

        private void onDatabaseButtonClicked()
        {
            if (uiEditGameWorldDatabaseWindow != null) return;

            uiEditGameWorldDatabaseWindow = new UIEditGameWorldDatabaseWindow(gameSystem, gameWorld)
            {
                okButtonClicked = onDatabaseWindowOkButtonClicked,
                cancelButtonClicked = () => uiEditGameWorldDatabaseWindow.Close(),
                applyButtonClicked = saveDatabase
            };

            uiEditGameWorldDatabaseWindow.FormClosing += (s, e) => uiEditGameWorldDatabaseWindow = null;
            uiEditGameWorldDatabaseWindow.ShowDialog(formMain);
        }

        private void onDatabaseWindowOkButtonClicked()
        {
            saveDatabase();

            uiEditGameWorldDatabaseWindow.Close();
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
            gameWorld.getGameWorldProcessor().save(gameWorld);
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
        }

        public class MainTileMapStatus : Status
        {
            private ZoomableTileMapSprites<MainTileMapSprites> zoomableTileMapSprites;
            private MainTileMapSprites.MainMapSpritesInfo mainMapSpritesInfo;

            private PointerStatus pointerStatus;
            private DrawTileStatus drawTileStatus;
            private DrawTileRectangleStatus drawTileRectangleStatus;
            private DrawTileFillStatus drawTileFillStatus;

            private Status currentStatus;

            public MainTileMapSprites tileMap => zoomableTileMapSprites.tileMapSprites;

            public MainTileMapStatus(SceneEditGameWorld s) : base(s)
            {
                var msi = mainMapSpritesInfo = new MainTileMapSprites.MainMapSpritesInfo(s.gameWorld);

                zoomableTileMapSprites = new ZoomableTileMapSprites<MainTileMapSprites>(
                    s.gameWorld.masterData.mainTileMapImageInfo.Values.Select(
                        o => new MainTileMapSprites(s.gameSystem, s.gameWorld, o, msi, true)).ToList());

                pointerStatus = new PointerStatus(this);
                drawTileStatus = new DrawTileStatus(this);
                drawTileRectangleStatus = new DrawTileRectangleStatus(this);
                drawTileFillStatus = new DrawTileFillStatus(this);
            }

            public override void start()
            {
                children.Add(zoomableTileMapSprites);

                scene.uiEditGameWorldMenuWindow.Show(scene.formMain);

                setDrawMode(DrawMode.pointer);
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

                            tileMap.setTerrain(p, (byte)scene.drawContentId);

                            gameStatus.mainMapSpritesInfo.resetTileFlag(p);

                            break;
                    }
                }
            }

            public class DrawTileRectangleStatus : Status
            {
                public Point? startPoint;
                public Rectangle rectangle;

                public DrawTileRectangleStatus(MainTileMapStatus s) : base(s)
                {
                }

                public override void mousePressed(MouseEventArgs e)
                {
                    if (gameWorld.mainTileMap.isOutOfBounds(gameStatus.tileMap.cursorPosition)) return;

                    startPoint = e.Location;
                }

                public override void mouseReleased(MouseEventArgs e)
                {
                    if (startPoint == null) return;

                    startPoint = null;

                    switch (scene.drawContent)
                    {
                        case DrawContent.terrain:

                            var tlp = rectangle.Location;
                            var trp = new Point(rectangle.X + rectangle.Width, rectangle.Y);
                            var blp = new Point(rectangle.X, rectangle.Y + rectangle.Height);

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

                                gameStatus.mainMapSpritesInfo.resetTileFlag(o);
                            });

                            break;
                    }

                    rectangle = Rectangle.Empty;
                }

                public override void mouseDragging(MouseEventArgs e, Point p)
                {
                    if (startPoint == null) return;

                    var sp = startPoint.Value;

                    rectangle = new Rectangle(sp.X, sp.Y, e.X - sp.X, e.Y - sp.Y);
                }

                public override void draw()
                {
                    if (startPoint == null) return;

                    gameSystem.gameGraphic.drawRactangle(Color.White, rectangle);
                }
            }

            public class DrawTileFillStatus : Status
            {
                private Stack<MapPoint> points = new Stack<MapPoint>();
                private HashSet<MapPoint> foundPoints = new HashSet<MapPoint>();
                private HashSet<MapPoint> markedPoints = new HashSet<MapPoint>();
                private byte selectedTerrainId;

                public DrawTileFillStatus(MainTileMapStatus s) : base(s)
                {
                }

                public override void mouseReleased(MouseEventArgs e)
                {
                    var p = scene.mainTileMapStatus.tileMap.getTileLocation(e);

                    var t = tileMap[p];

                    if (t == null) return;

                    selectedTerrainId = t.Value.terrain;

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

                        if (tileMap[p].Value.terrain == selectedTerrainId)
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

                    list.ForEach(gameStatus.mainMapSpritesInfo.resetTileFlag);
                }

                private void add(int x, int y)
                {
                    points.Push(new MapPoint(x, y));
                }
            }
        }


        public class DetailTileMapStatus : Status
        {
            private ZoomableTileMapSprites<DetailTileMapSprites> zoomableTileMapSprites;
            private DetailTileMapSprites.DetailMapSpritesInfo detailMapSpritesInfo;

            private PointerStatus pointerStatus;
            private DrawTileStatus drawTileStatus;
            private DrawTileRectangleStatus drawTileRectangleStatus;
            private DrawTileFillStatus drawTileFillStatus;

            private Status currentStatus;

            public DetailTileMapSprites tileMap => zoomableTileMapSprites.tileMapSprites;

            public DetailTileMapStatus(SceneEditGameWorld s) : base(s)
            {
                detailMapSpritesInfo = new DetailTileMapSprites.DetailMapSpritesInfo(s.gameWorld);

                zoomableTileMapSprites = new ZoomableTileMapSprites<DetailTileMapSprites>(
                    s.gameWorld.masterData.detailTileMapImageInfo.Values.Select(
                        o => new DetailTileMapSprites(s.gameSystem, s.gameWorld, o, detailMapSpritesInfo, true)).ToList());

                pointerStatus = new PointerStatus(this);
                drawTileStatus = new DrawTileStatus(this);
                drawTileRectangleStatus = new DrawTileRectangleStatus(this);
                drawTileFillStatus = new DrawTileFillStatus(this);
            }

            public void setTileMap(DetailTileMap tm) => detailMapSpritesInfo.detailTileMap = tm;

            public void resetFlag() => detailMapSpritesInfo.removeTileFlag();

            public override void start()
            {
                children.Add(zoomableTileMapSprites);

                setDrawMode(DrawMode.pointer);
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
                public Rectangle rectangle;

                public DrawTileRectangleStatus(DetailTileMapStatus s) : base(s)
                {
                }

                public override void mousePressed(MouseEventArgs e)
                {
                    if (gameWorld.mainTileMap.isOutOfBounds(gameStatus.tileMap.cursorPosition)) return;

                    startPoint = e.Location;
                }

                public override void mouseReleased(MouseEventArgs e)
                {
                    if (startPoint == null) return;

                    startPoint = null;

                    switch (scene.drawContent)
                    {
                        case DrawContent.terrain:

                            var tlp = rectangle.Location;
                            var trp = new Point(rectangle.X + rectangle.Width, rectangle.Y);
                            var blp = new Point(rectangle.X, rectangle.Y + rectangle.Height);

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

                    rectangle = Rectangle.Empty;
                }

                public override void mouseDragging(MouseEventArgs e, Point p)
                {
                    if (startPoint == null) return;

                    var sp = startPoint.Value;

                    rectangle = new Rectangle(sp.X, sp.Y, e.X - sp.X, e.Y - sp.Y);
                }

                public override void draw()
                {
                    if (startPoint == null) return;

                    gameSystem.gameGraphic.drawRactangle(Color.White, rectangle);
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

                    var t = tileMap[p];

                    if (t == null) return;

                    selectedTerrainId = t.Value.terrain;

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

                        if (tileMap[p].Value.terrain == selectedTerrainId)
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

                private void add(int x, int y)
                {
                    points.Push(new MapPoint(x, y));
                }
            }
        }
    }
}
