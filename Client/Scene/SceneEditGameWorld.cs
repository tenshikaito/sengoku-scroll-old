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
        private OuterTileMapStatus outerTileMapStatus;
        private InnerTileMapStatus innerTileMapStatus;

        private UIConfirmDialog uiConfirmDialog;

        private UIEditGameWorldMenuWindow uiEditGameWorldMenuWindow;

        private UIEditGameWorldDatabaseWindow uiEditGameWorldDatabaseWindow;

        private UIEditGameWorldInnerTileMapDialog uiEditGameWorldInnerTileMapDialog;

        private DrawContent drawContent = DrawContent.terrain;
        private int drawContentId = 0;

        private Camera camera => gameWorld.camera;

        public SceneEditGameWorld(GameSystem gs, GameWorld gw) : base(gs)
        {
            gameWorld = gw;

            waitingStatus = new SceneWaiting(gs, gs.wording.loading);
            outerTileMapStatus = new OuterTileMapStatus(this);
            innerTileMapStatus = new InnerTileMapStatus(this);

            uiEditGameWorldMenuWindow = new UIEditGameWorldMenuWindow(
                gs,
                onPointerButtonClicked,
                onBrushButtonClicked,
                onRectangleButtonClicked,
                onFillButtonClicked,
                onInnerTileMapButtonClicked,
                onDatabaseButtonClicked,
                onSaveButtonClicked,
                onExitButtonClicked,
                onTerrainSelected);

            uiEditGameWorldMenuWindow.setTerrain(gameWorld.gameWorldMasterData.terrain.Values.ToList());
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
            switchStatus(outerTileMapStatus);
        }

        public override void finish()
        {
            uiEditGameWorldMenuWindow.Close();
        }

        private void onPointerButtonClicked() => (status as Status).setDrawMode(DrawMode.pointer);

        private void onBrushButtonClicked() => (status as Status).setDrawMode(DrawMode.brush);

        private void onRectangleButtonClicked() => (status as Status).setDrawMode(DrawMode.rectangle);

        private void onFillButtonClicked() => (status as Status).setDrawMode(DrawMode.fill);

        private void onInnerTileMapButtonClicked()
        {
            if (uiEditGameWorldInnerTileMapDialog == null)
            {
                uiEditGameWorldInnerTileMapDialog = new UIEditGameWorldInnerTileMapDialog(
                    gameSystem, gameWorld.gameWorldMasterData.innerTileMapInfo, onInnerTileMapSelected)
                {
                    okButtonClicked = () =>
                    {
                        uiEditGameWorldInnerTileMapDialog.Close();
                        uiEditGameWorldInnerTileMapDialog = null;
                    }
                };

                uiEditGameWorldInnerTileMapDialog.Show(formMain);
            }
        }

        private void onInnerTileMapSelected(int id)
        {
            switchStatus(waitingStatus);

            Task.Run(() =>
            {
                var gwp = gameWorld.getGameWorldProcessor();

                var itm = gameWorld.gameWorldMasterData.innerTileMapInfo[id];

                var tm = gwp.loadInnerTileMap(id, itm.size.column, itm.size.row);

                innerTileMapStatus.setTileMap(tm);

                innerTileMapStatus.resetFlag();

                dispatcher.invoke(() => switchStatus(innerTileMapStatus));
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
            uiEditGameWorldDatabaseWindow.Show();
        }

        private void onDatabaseWindowOkButtonClicked()
        {
            saveDatabase();

            uiEditGameWorldDatabaseWindow.Close();
        }

        private void saveDatabase()
        {
            gameWorld.gameWorldMasterData = uiEditGameWorldDatabaseWindow.gameWorldMasterData;
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

        public class OuterTileMapStatus : Status
        {
            private ZoomableTileMapSprites<OuterTileMapSprites> zoomableTileMapSprites;
            private OuterTileMapSprites.OuterMapSpritesInfo outerMapSpritesInfo;

            private PointerStatus pointerStatus;
            private DrawTileStatus drawTileStatus;
            private DrawTileRectangleStatus drawTileRectangleStatus;
            private DrawTileFillStatus drawTileFillStatus;

            private Status currentStatus;

            public OuterTileMapSprites tileMap => zoomableTileMapSprites.tileMapSprites;

            public OuterTileMapStatus(SceneEditGameWorld s) : base(s)
            {
                var msi = outerMapSpritesInfo = new OuterTileMapSprites.OuterMapSpritesInfo(s.gameWorld);

                zoomableTileMapSprites = new ZoomableTileMapSprites<OuterTileMapSprites>(
                    s.gameWorld.gameWorldMasterData.outerTileMapImageInfo.Values.Select(
                        o => new OuterTileMapSprites(s.gameSystem, s.gameWorld, o, msi, true)).ToList());

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
                protected OuterTileMapStatus gameStatus;

                protected SceneEditGameWorld scene => gameStatus.scene;

                protected GameSystem gameSystem => scene.gameSystem;

                protected GameWorld gameWorld => scene.gameWorld;

                protected OuterTileMap tileMap => gameStatus.outerMapSpritesInfo.outerTileMap;

                public Status(OuterTileMapStatus s) => gameStatus = s;
            }

            public class PointerStatus : Status
            {
                public PointerStatus(OuterTileMapStatus s) : base(s)
                {
                }

                public override void mouseDragging(MouseEventArgs e, Point p)
                {
                    gameStatus.scene.camera.dragCamera(p);
                }
            }

            public class DrawTileStatus : Status
            {
                public DrawTileStatus(OuterTileMapStatus s) : base(s)
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

                            gameStatus.outerMapSpritesInfo.resetTileFlag(p);

                            break;
                    }
                }
            }

            public class DrawTileRectangleStatus : Status
            {
                public Point? startPoint;
                public Rectangle rectangle;

                public DrawTileRectangleStatus(OuterTileMapStatus s) : base(s)
                {
                }

                public override void mousePressed(MouseEventArgs e)
                {
                    if (gameWorld.gameOuterMapData.data.isOutOfBounds(gameStatus.tileMap.cursorPosition)) return;

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

                            var tm = gameWorld.gameOuterMapData.data;

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

                                gameStatus.outerMapSpritesInfo.resetTileFlag(o);
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

                public DrawTileFillStatus(OuterTileMapStatus s) : base(s)
                {
                }

                public override void mouseReleased(MouseEventArgs e)
                {
                    var p = scene.outerTileMapStatus.tileMap.getTileLocation(e);

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

                    list.ForEach(gameStatus.outerMapSpritesInfo.resetTileFlag);
                }

                private void add(int x, int y)
                {
                    points.Push(new MapPoint(x, y));
                }
            }
        }


        public class InnerTileMapStatus : Status
        {
            private ZoomableTileMapSprites<InnerTileMapSprites> zoomableTileMapSprites;
            private InnerTileMapSprites.InnerMapSpritesInfo innerMapSpritesInfo;

            private PointerStatus pointerStatus;
            private DrawTileStatus drawTileStatus;
            private DrawTileRectangleStatus drawTileRectangleStatus;
            private DrawTileFillStatus drawTileFillStatus;

            private Status currentStatus;

            public InnerTileMapSprites tileMap => zoomableTileMapSprites.tileMapSprites;

            public InnerTileMapStatus(SceneEditGameWorld s) : base(s)
            {
                innerMapSpritesInfo = new InnerTileMapSprites.InnerMapSpritesInfo(s.gameWorld);

                zoomableTileMapSprites = new ZoomableTileMapSprites<InnerTileMapSprites>(
                    s.gameWorld.gameWorldMasterData.innerTileMapImageInfo.Values.Select(
                        o => new InnerTileMapSprites(s.gameSystem, s.gameWorld, o, innerMapSpritesInfo, true)).ToList());

                pointerStatus = new PointerStatus(this);
                drawTileStatus = new DrawTileStatus(this);
                drawTileRectangleStatus = new DrawTileRectangleStatus(this);
                drawTileFillStatus = new DrawTileFillStatus(this);
            }

            public void setTileMap(InnerTileMap tm) => innerMapSpritesInfo.innerTileMap = tm;

            public void resetFlag() => innerMapSpritesInfo.removeTileFlag();

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
                protected InnerTileMapStatus gameStatus;

                protected SceneEditGameWorld scene => gameStatus.scene;

                protected GameSystem gameSystem => scene.gameSystem;

                protected GameWorld gameWorld => scene.gameWorld;

                protected InnerTileMap tileMap => gameStatus.innerMapSpritesInfo.innerTileMap;

                public Status(InnerTileMapStatus s) => gameStatus = s;
            }

            public class PointerStatus : Status
            {
                public PointerStatus(InnerTileMapStatus s) : base(s)
                {
                }

                public override void mouseDragging(MouseEventArgs e, Point p)
                {
                    gameStatus.scene.camera.dragCamera(p);
                }
            }

            public class DrawTileStatus : Status
            {
                public DrawTileStatus(InnerTileMapStatus s) : base(s)
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

                            gameStatus.innerMapSpritesInfo.resetTileFlag(p);

                            break;
                    }
                }
            }

            public class DrawTileRectangleStatus : Status
            {
                public Point? startPoint;
                public Rectangle rectangle;

                public DrawTileRectangleStatus(InnerTileMapStatus s) : base(s)
                {
                }

                public override void mousePressed(MouseEventArgs e)
                {
                    if (gameWorld.gameOuterMapData.data.isOutOfBounds(gameStatus.tileMap.cursorPosition)) return;

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

                            var tm = gameWorld.gameOuterMapData.data;

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

                                gameStatus.innerMapSpritesInfo.resetTileFlag(o);
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

                public DrawTileFillStatus(InnerTileMapStatus s) : base(s)
                {
                }

                public override void mouseReleased(MouseEventArgs e)
                {
                    var p = scene.outerTileMapStatus.tileMap.getTileLocation(e);

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

                    list.ForEach(gameStatus.innerMapSpritesInfo.resetTileFlag);
                }

                private void add(int x, int y)
                {
                    points.Push(new MapPoint(x, y));
                }
            }
        }
    }
}
