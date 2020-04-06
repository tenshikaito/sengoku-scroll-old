using Client.Graphic;
using Client.Helper;
using Client.UI;
using Library;
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

        private OuterTileMapStatus outerTileMapStatus;

        private UIEditGameWorldMenuWindow uiEditGameWorldMenuWindow;

        private UIEditGameWorldDatabaseWindow uiEditGameWorldDatabaseWindow;

        private DrawContent drawContent = DrawContent.terrain;
        private int drawContentId = 0;

        private Camera camera => gameWorld.camera;

        public SceneEditGameWorld(GameSystem gs, GameWorld gw) : base(gs)
        {
            gameWorld = gw;

            outerTileMapStatus = new OuterTileMapStatus(this);

            uiEditGameWorldMenuWindow = new UIEditGameWorldMenuWindow(
                gs,
                onPointerButtonClicked,
                onBrushButtonClicked,
                onRectangleButtonClicked,
                onFillButtonClicked,
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
            uiEditGameWorldDatabaseWindow.Show(uiEditGameWorldMenuWindow);
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
            save();

            MessageBox.Show("data saved.", "alert", MessageBoxButtons.OK);
        }

        private void onExitButtonClicked()
        {
            if (MessageBox.Show("exit?", "alert", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                gameSystem.sceneToTitle();
            }
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

            private PointerStatus pointerStatus;
            private DrawTileStatus drawTileStatus;
            private DrawTileRectangleStatus drawTileRectangleStatus;
            private DrawTileFillStatus drawTileFillStatus;

            private Status currentStatus;

            public OuterTileMapSprites tileMap => zoomableTileMapSprites.outerTileMap;

            public OuterTileMapStatus(SceneEditGameWorld s) : base(s)
            {
                var msi = new OuterTileMapSprites.MapSpritesInfo(s.gameWorld);

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
                    camera.center = new Point(tileVertex.x * tileMap.tileWidth, tileVertex.y * tileMap.tileHeight);
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

                protected GameWorldOuterMapData outerMap => gameWorld.gameOuterMapData;

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

                public override void mouseClicked(MouseEventArgs e) => draw(e);

                public override void mouseDragging(MouseEventArgs e, Point p) => draw(e);

                private void draw(MouseEventArgs e)
                {
                    var p = gameStatus.tileMap.getTileLocation(e);

                    switch (scene.drawContent)
                    {
                        case DrawContent.terrain:

                            outerMap.data.setTerrain(p, (byte)scene.drawContentId);

                            gameStatus.tileMap.resetTileFlag(p);

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

                            var width = tr.x - tl.x;
                            var height = bl.y - tl.y;

                            tm.eachRectangle(tl, new Map.Size(height, width), o =>
                            {
                                outerMap.data.setTerrain(o, (byte)scene.drawContentId);

                                gameStatus.tileMap.resetTileFlag(o);

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

                    var t = outerMap.data[p];

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

                        if (outerMap.data.isOutOfBounds(p) || foundPoints.Contains(p))
                        {
                            points.Pop();

                            continue;
                        }

                        foundPoints.Add(p);

                        if (outerMap.data[p].Value.terrain == selectedTerrainId)
                        {
                            markedPoints.Add(p);

                            if (check(p.x + 1, p.y)) continue;
                            if (check(p.x - 1, p.y)) continue;
                            if (check(p.x, p.y + 1)) continue;
                            if (check(p.x, p.y - 1)) continue;
                        }
                        else
                        {
                            points.Pop();
                        }
                    }

                    var terrainId = (byte)scene.drawContentId;

                    var list = markedPoints.ToList();
                    
                    list.ForEach(o =>
                    {
                        scene.outerTileMapStatus.zoomableTileMapSprites.outerTileMap.removeTileFlag(o);

                        outerMap.data.setTerrain(o, terrainId);
                    });

                    list.ForEach(scene.outerTileMapStatus.zoomableTileMapSprites.outerTileMap.recoveryTileFlag);
                }

                private bool check(int x, int y)
                {
                    var p = new MapPoint(x, y);

                    points.Push(p);

                    return false;
                }
            }
        }
    }
}
