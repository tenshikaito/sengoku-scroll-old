using Client.Graphic;
using Client.Helper;
using Client.UI;
using Library;
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
            public PointerStatus pointerStatus;

            public DrawTileStatus drawTileStatus;

            private ZoomableTileMapSprites<OuterTileMapSprites> zoomableTileMapSprites;

            public OuterTileMapSprites tileMap => zoomableTileMapSprites.outerTileMap;

            public OuterTileMapStatus(SceneEditGameWorld s) : base(s)
            {
                var msi = new OuterTileMapSprites.MapSpritesInfo(s.gameWorld);

                zoomableTileMapSprites = new ZoomableTileMapSprites<OuterTileMapSprites>(
                    s.gameWorld.gameWorldMasterData.outerTileMapImageInfo.Values.Select(
                        o => new OuterTileMapSprites(s.gameSystem, s.gameWorld, o, msi, true)).ToList());

                pointerStatus = new PointerStatus(this);
                drawTileStatus = new DrawTileStatus(this);
            }

            public override void start()
            {
                scene.uiEditGameWorldMenuWindow.Show(scene.formMain);

                children.Add(zoomableTileMapSprites);

                setDrawMode(DrawMode.pointer);
            }

            public override void mouseWheelScrolled(MouseEventArgs e)
            {
                var cursorPos = tileMap.cursorPosition;
                var camera = scene.camera;
                var center = camera.center;
                var sCenter = camera.translateWorldToScreen(center);
                var tileVertex = tileMap.getTileLocation(sCenter.X, sCenter.Y);
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
                switch (dm)
                {
                    case DrawMode.pointer:

                        switchStatus(pointerStatus);

                        break;

                    case DrawMode.brush:

                        switchStatus(drawTileStatus);

                        break;
                }
            }

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
                    var p = gameStatus.tileMap.getTileLocation(e.X, e.Y);

                    switch (scene.drawContent)
                    {
                        case DrawContent.terrain:

                            gameStatus.tileMap.resetTileFlag(p);

                            outerMap.data.setTerrain(p, (byte)scene.drawContentId);

                            break;
                    }
                }
            }
        }
    }
}
