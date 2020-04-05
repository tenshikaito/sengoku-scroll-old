using Client.Helper;
using Client.Scene;
using Library;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Region = Library.Model.Region;

namespace Client
{
    public class FormMain : Form
    {
        private GameSystem gameSystem;
        private Option option;

        private GameObject gameRoot;

        public Dispatcher dispatcher;

        public FormMain()
        {
            loadOption();

            initWindow();

            initSystem();
        }

        private void loadOption()
        {
            option = new Option();

            refreshDrawTimeSpan();
        }

        private void initSystem()
        {
            gameRoot = new GameObject();

            gameGraphic = new GameGraphic()
            {
                defaultFontName = "MingLiU",
                defaultFontSize = 12
            };

            gameSystem = new GameSystem()
            {
                formMain = this,
                screenWidth = Width,
                screenHeight = Height,
                sceneManager = gameRoot,
                gameGraphic = gameGraphic
            };

            gameSystem.init();
            gameSystem.sceneToTitle();
            //gameSystem.initGame();
            //gameSystem.sceneToOuterMap();

            //var gw = new GameWorld("test")
            //{
            //    gameWorldMasterData = new GameWorldMasterData()
            //    {
            //        terrain = new Dictionary<int, Terrain>(),
            //        region = new Dictionary<int, Region>(),
            //        culture = new Dictionary<int, Culture>(),
            //        religion = new Dictionary<int, Religion>(),
            //        road = new Dictionary<int, Road>(),
            //        strongholdType = new Dictionary<int, Stronghold.Type>()
            //    }//gw.gameData.toJson().fromJson<GameData>();
            //};

            //new UI.UIEditGameWorldDatabaseWindow(gameSystem, gw).Show();

            refreshBuffer();
        }

        private void initWindow()
        {
            Width = option.screenWidth;
            Height = option.screenHeight;
            Text = option.title;
            Icon = new Icon("Icon.ico");

            dispatcher = new Dispatcher(mainActions);

            this.setCenter();
        }

        private void refreshBuffer()
        {
            bufferGraphic = new BufferedGraphicsContext().Allocate(CreateGraphics(), DisplayRectangle);
            gameGraphic.g = bufferGraphic.Graphics;
        }

        protected override void OnLoad(EventArgs e)
        {
            run();
        }

        private BufferedGraphics bufferGraphic;

        private bool isDragging;
        private int dragCount;
        private Point dragStartPoint;
        private Point dragPoint;
        private int dragPointOffsetX => dragPoint.X - dragStartPoint.X;
        private int dragPointOffsetY => dragPoint.Y - dragStartPoint.Y;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            gameRoot.onMouseMoved(e);

            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;

                if (++dragCount > 0)
                {
                    if (dragCount == 1)
                    {
                        dragStartPoint = e.Location;
                    }
                    else
                    {
                        dragPoint = e.Location;
                        gameRoot.onMouseDragging(e, new Point(dragPointOffsetX * 2, dragPointOffsetY * 2));
                        dragCount = 0;
                    }
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (!isDragging) gameRoot.onMouseClicked(e);

            isDragging = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            gameRoot.onMousePressed(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            dragCount = 0;

            gameRoot.onMouseReleased(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            gameRoot.onMouseWheelScrolled(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            //scene.keyPressed(e);
        }

        private int count;
        private DateTime lastUpdateTime = DateTime.Now;
        private GameGraphic gameGraphic;
        private TimeSpan oneSecondTimeSpan = TimeSpan.FromSeconds(1);
        private TimeSpan drawTimeSpan = TimeSpan.FromSeconds(1d / 60);

        protected override void OnPaint(PaintEventArgs e)
        {
            draw();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        private void refreshDrawTimeSpan()
        {
            drawTimeSpan = TimeSpan.FromSeconds(1d / option.fps);
        }

        public void run()
        {
            Task.Run(() =>
            {
                var now = DateTime.Now;
                var lastUpdateTime = now;
                var lastDrawTime = now;
                var ups = TimeSpan.FromSeconds(1d / 60);
                var updateAction = new Action(update);

                while (true)
                {
                    try
                    {
                        now = DateTime.Now;

                        if (now - lastUpdateTime >= ups)
                        {
                            BeginInvoke(updateAction);
                            lastUpdateTime = now;
                        }

                        if (now - lastDrawTime >= drawTimeSpan)
                        {
                            Invalidate();
                            lastDrawTime = now;
                        }

                        Thread.Sleep(1);
                    }
                    catch
                    {
                        break;
                    }
                }
            });
        }

        private List<Action> mainActions = new List<Action>();

        private void update()
        {
            dispatcher.update();

            if (mainActions.Any())
            {
                mainActions.ForEach(o => o());

                mainActions.Clear();
            }

            gameRoot.onUpdate();
        }

        private void draw()
        {
            gameGraphic.fillRactangle(Color.Black, ClientRectangle);

            gameRoot.onDraw();

            bufferGraphic.Render();

            ++count;

            var now = DateTime.Now;

            if (now - lastUpdateTime >= oneSecondTimeSpan)
            {
                Text = $"{option.title} FPS:{count}";
                count = 0;
                lastUpdateTime = now;
            }
        }

        public class Dispatcher
        {
            private Queue<Action> actions = new Queue<Action>();
            private List<Action> mainActions;

            public Dispatcher(List<Action> mainActions) => this.mainActions = mainActions;

            public void update()
            {
                lock (actions)
                {
                    while (actions.Any()) mainActions.Add(actions.Dequeue());
                }
            }

            public void invoke(Action a)
            {
                lock (actions)
                {
                    actions.Enqueue(a);
                }
            }

        }
    }

}
