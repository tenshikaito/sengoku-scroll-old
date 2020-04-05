using Client.Graphic;
using Client.Helper;
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
    public partial class SceneOuterMap : SceneBase
    {
        public GameStateManager gameStateManager;

        private GameGraphic gameGraphic => gameSystem.gameGraphic;
        private OuterTileMapSprites tileMapSprites;

        private GameWorld gameWorld;
        private GameWorldMasterData gameData => gameWorld.gameWorldMasterData;
        private GameOuterMap gameMap => gameWorld.gameMap;
        private Option option => gameSystem.option;
        private Camera camera => gameWorld.camera;

        private SceneOuterMapControlPanel controlPanel;
        private FlowLayoutPanel uiPanel;

        public SceneOuterMap(GameSystem gs, GameWorld gw) : base(gs)
        {
            gameSystem = gs;
            gameWorld = gw;

            //tileMapSprites = new OuterTileMapSprites(gs, gw, 48, 48);

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

            controlPanel = new SceneOuterMapControlPanel(this, uiPanel);
            controlPanel.wording = gameSystem.wording;
        }

        //public void onTileClicked(MapPoint p)
        //{
        //    var c = gameWorld.currentCharacter;

        //    if(c.location == p)
        //    {
        //        var id = gameMap.stronghold.getId(gameMap, p);

        //        if (id != null)
        //        {
        //            var s = gameData.stronghold[(int)id];

        //            if (MessageBox.Show($"enter stronghold {s.name}?", "confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
        //            {
        //                enterCurrentStronghold();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        characterMove(p);
        //    }

        //}

        private void characterMove(MapPoint p)
        {
            gameWorld.currentCharacter.location = p;

            refreshInfoPanel();
        }

        private void refreshInfoPanel()
        {
            var p = gameWorld.currentCharacter.location;

            var id = gameMap.stronghold.getId(gameMap, p);

            if (id != null)
            {
                var s = gameData.stronghold[(int)id];

                controlPanel.setStronghold(s);
            }
        }

        public void enterCurrentStronghold()
        {
            var c = gameWorld.currentCharacter;
            var p = c.location;

            var id = gameMap.stronghold.getId(gameMap, p);

            if (id != null)
            {
                var s = gameData.stronghold[(int)id];

                c.inStronghold = s.id;
                c.location = new MapPoint(0, 0);

                gameSystem.sceneToInnerMap(s);
            }
        }

        public override void update()
        {
        }

        public override void draw()
        {
            tileMapSprites.draw();
            gameStateManager.draw();
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

    public partial class SceneOuterMap
    {
        public class GameStateManager : StateManager<GameStatusAdapter>
        {
            protected SceneOuterMap sceneMap;
            protected GameGraphic gameGraphic => sceneMap.gameGraphic;

            public readonly SceneMapDefaultGameStatus sceneMapDefaultGameStatus;
            public readonly SceneMapModalGameStatus sceneMapModalGameStatus;
            public readonly SceneMapSelectGameStatus sceneMapSelectGameStatus;

            public GameStateManager(SceneOuterMap sm)
            {
                sceneMap = sm;

                sceneMapDefaultGameStatus = new SceneMapDefaultGameStatus(sm);
                sceneMapModalGameStatus = new SceneMapModalGameStatus(sm);
                sceneMapSelectGameStatus = new SceneMapSelectGameStatus(sm);

                switchStatus(sceneMapDefaultGameStatus);
            }

            public abstract class SceneMapGameStatus : GameStatusAdapter
            {
                protected SceneOuterMap sceneMap;

                public SceneMapGameStatus(SceneOuterMap sm)
                {
                    sceneMap = sm;
                }
            }

            public class SceneMapDefaultGameStatus : SceneMapGameStatus
            {
                public SceneMapDefaultGameStatus(SceneOuterMap sm) : base(sm)
                {
                }

                public override void start()
                {
#warning show
                    //sceneMap.uiManager.showGameCommandWindow();
                }

                public override void finish()
                {
#warning hide
                    //sceneMap.uiManager.hideCommandWindow();
                }

                public override void mouseClicked(MouseEventArgs e)
                {
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            sceneMap.tileMapSprites.mouseClicked(e);
                            break;
                    }
                }

                public override void mouseDragging(MouseEventArgs e, Point p)
                {
                    sceneMap.camera.dragCamera(p);
                }

                public override void mouseEntered(MouseEventArgs e)
                {
                }

                public override void mouseExited(MouseEventArgs e)
                {
                }

                public override void mouseMoved(MouseEventArgs e)
                {
                }

                public override void mousePressed(MouseEventArgs e)
                {
                }

                public override void mouseReleased(MouseEventArgs e)
                {
                }

                public override void mouseWheelScrolled(MouseEventArgs e)
                {
                }
            }

            public class SceneMapModalGameStatus : SceneMapGameStatus
            {
                public SceneMapModalGameStatus(SceneOuterMap sm) : base(sm)
                {
                }

                public override void start()
                {
                }

                public override void finish()
                {
                }

                public override void mouseClicked(MouseEventArgs e)
                {
                }

                public override void mouseDragging(MouseEventArgs e, Point p)
                {
                }

                public override void mouseEntered(MouseEventArgs e)
                {
                }

                public override void mouseExited(MouseEventArgs e)
                {
                }

                public override void mouseMoved(MouseEventArgs e)
                {
                }

                public override void mousePressed(MouseEventArgs e)
                {
                }

                public override void mouseReleased(MouseEventArgs e)
                {
                }

                public override void mouseWheelScrolled(MouseEventArgs e)
                {
                }
            }

            public class SceneMapSelectGameStatus : SceneMapGameStatus
            {
                public Action starting, finishing;
                public Action<MouseEventArgs> mouseButtonClicked;

                public SceneMapSelectGameStatus(SceneOuterMap sm) : base(sm)
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
                    sceneMap.camera.dragCamera(p);
                }

                public override void mouseEntered(MouseEventArgs e)
                {
                }

                public override void mouseExited(MouseEventArgs e)
                {
                }

                public override void mouseMoved(MouseEventArgs e)
                {
                }

                public override void mousePressed(MouseEventArgs e)
                {
                }

                public override void mouseReleased(MouseEventArgs e)
                {
                }

                public override void mouseWheelScrolled(MouseEventArgs e)
                {
                }
            }
        }
    }
}
