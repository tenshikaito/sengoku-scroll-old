using Client.Graphic;
using Client.UI;
using Library;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Client.Scene
{
    public partial class SceneInnerMap : SceneBase
    {
        public GameStateManager gameStateManager;

        private GameWorld gameWorld;
        private InnerTileMapSprites tileMapSprites;

        private Option option => gameSystem.option;
        private Camera camera => gameSystem.camera;

        private SceneInnerMapControlPanel controlPanel;

        private GameInnerMap gameInnerMap;
        private FlowLayoutPanel uiPanel;

        public SceneInnerMap(GameSystem gs, GameWorld gw) : base(gs)
        {
            gameSystem = gs;
            gameWorld = gw;

            //tileMapSprites = new InnerTileMapSprites(gs, gw, 48, 48);

            gameStateManager = new GameStateManager(this);

            gameStateManager.switchStatus(gameStateManager.sceneMapDefaultGameStatus);

            initGraphic();
        }

        private void initGraphic()
        {
            var width = 360;

            uiPanel = new FlowLayoutPanel()
            {
                Width = width,
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.TopDown
            };

            controlPanel = new SceneInnerMapControlPanel(this, uiPanel);
            controlPanel.wording = gameSystem.wording;
        }

        private void characterMove(MapPoint p)
        {
            gameWorld.currentCharacter.location = p;

            refreshInfoPanel();
        }

        public void exitStronghold()
        {
            var c = gameWorld.currentCharacter;
            c.inStronghold = null;
            c.location = gameInnerMap.currentStronghold.location;
            gameSystem.sceneToOuterMap();
        }

        private void refreshInfoPanel()
        {
            var p = gameWorld.currentCharacter.location;

        }

        //public void setMap(GameInnerMap gim)
        //{
        //    gameInnerMap = gim;
        //    tileMapSprites.gameMap = gim;
        //    tileMapSprites.resize();

        //    controlPanel.setStronghold(gim.currentStronghold);
        //}

        public override void update()
        {
        }

        public override void draw()
        {
            tileMapSprites.draw();
        }

        private void dragCamera(Point p)
        {
            camera.x -= p.X;
            camera.y -= p.Y;
        }

        public void stateToDefaultGameStatus()
        {
            gameStateManager.switchStatus(gameStateManager.sceneMapDefaultGameStatus);
        }

        public void stateToModalGameStatus()
        {
            gameStateManager.switchStatus(gameStateManager.sceneMapModalGameStatus);
        }

        public void stateToSelectGameStatus(Action starting, Action finishing, Action<MouseEventArgs> click)
        {
            gameStateManager.sceneMapSelectGameStatus.starting = starting;
            gameStateManager.sceneMapSelectGameStatus.finishing = finishing;
            gameStateManager.sceneMapSelectGameStatus.mouseButtonClicked = click;
            gameStateManager.switchStatus(gameStateManager.sceneMapSelectGameStatus);
        }


        public void keyPressed(KeyEventArgs e)
        {
            var scrollSpeed = option.scrollSpeed;

            if (e.Control) scrollSpeed *= 2;

            if (e.KeyCode == Keys.Up) camera.y -= scrollSpeed;
            else if (e.KeyCode == Keys.Down) camera.y += scrollSpeed;
            if (e.KeyCode == Keys.Left) camera.x -= scrollSpeed;
            else if (e.KeyCode == Keys.Right) camera.x += scrollSpeed;
        }
    }

    public partial class SceneInnerMap
    {
        public class GameStateManager : GameObject
        {
            protected SceneInnerMap sceneMap;

            public readonly SceneMapDefaultGameStatus sceneMapDefaultGameStatus;
            public readonly SceneMapModalGameStatus sceneMapModalGameStatus;
            public readonly SceneMapSelectGameStatus sceneMapSelectGameStatus;

            public GameStateManager(SceneInnerMap sm)
            {
                sceneMap = sm;

                sceneMapDefaultGameStatus = new SceneMapDefaultGameStatus(sm);
                sceneMapModalGameStatus = new SceneMapModalGameStatus(sm);
                sceneMapSelectGameStatus = new SceneMapSelectGameStatus(sm);
;
                switchStatus(sceneMapDefaultGameStatus);
            }

            public abstract class SceneMapGameStatus : GameObject
            {
                protected SceneInnerMap sceneMap;

                public SceneMapGameStatus(SceneInnerMap sm)
                {
                    sceneMap = sm;
                }
            }

            public class SceneMapDefaultGameStatus : SceneMapGameStatus
            {
                public SceneMapDefaultGameStatus(SceneInnerMap sm) : base(sm)
                {
                }

                public override void mouseClicked(MouseEventArgs e)
                {
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            sceneMap.tileMapSprites.mouseClicked(e);
                            break;
                        case MouseButtons.Right:
                            if (MessageBox.Show("exit stronghold?", "confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                sceneMap.exitStronghold();
                            }
                            break;
                    }
                }
            }

            public class SceneMapModalGameStatus : SceneMapGameStatus
            {
                public SceneMapModalGameStatus(SceneInnerMap sm) : base(sm)
                {
                }
            }

            public class SceneMapSelectGameStatus : SceneMapGameStatus
            {
                public Action starting, finishing;
                public Action<MouseEventArgs> mouseButtonClicked;

                public SceneMapSelectGameStatus(SceneInnerMap sm) : base(sm)
                {
                }

                public void init()
                {
                    starting = finishing = null;
                    mouseButtonClicked = null;
                }

                public override void start()
                {
                    starting?.Invoke();
                }

                public override void finish()
                {
                    finishing?.Invoke();
                }

                public override void mouseClicked(MouseEventArgs e)
                {
                    mouseButtonClicked?.Invoke(e);
                }

                public override void mouseDragging(MouseEventArgs e, Point p)
                {
                    sceneMap.dragCamera(p);
                }
            }
        }
    }
}
