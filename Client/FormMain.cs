using Client.Graphic;
using Client.Helper;
using Client.Scene;
using Library;
using Library.Helper;
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
        public Dispatcher dispatcher;

        private GameSystem gameSystem;

        private GameObject gameRoot;

        private Option option;

        private SpriteRectangle background;

        public FormMain()
        {
            loadOption();

            initWindow();

            initSystem();
        }

        private void loadOption()
        {
            option = new Option();
        }

        private async void initSystem()
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
                option = option,
                sceneManager = gameRoot,
                gameGraphic = gameGraphic
            };

            background = new SpriteRectangle()
            {
                color = Color.Black,
                isFill = true,
                size = ClientSize
            };

            await gameSystem.init();
            gameSystem.sceneToTitle(false);

            refreshBuffer();
        }

        private void initWindow()
        {
            Width = option.screenWidth;
            Height = option.screenHeight;
            Text = option.title;
            Icon = new Icon("Icon.ico");
            MaximizeBox = false;

            dispatcher = new Dispatcher();

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

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            gameRoot.onKeyPressed(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            gameRoot.onKeyPressing(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            gameRoot.onKeyReleased(e);
        }

        private int count;
        private DateTime lastUpdateTime = DateTime.Now;
        private GameGraphic gameGraphic;
        private TimeSpan oneSecondTimeSpan = TimeSpan.FromSeconds(1);
        private TimeSpan updateTimeSpan = TimeSpan.FromSeconds(1d / 60);
        private TimeSpan drawTimeSpan = TimeSpan.FromSeconds(1d / 60);
        private TimeSpan updateTimeCost = TimeSpan.Zero;
        private TimeSpan drawTimeCost = TimeSpan.Zero;

        protected override void OnPaint(PaintEventArgs e)
        {
            draw();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        public void run()
        {
            Task.Run(() =>
            {
                var now = DateTime.Now;
                var lastUpdateTime = now;
                var lastDrawTime = now;
                var updateAction = new Action(update);

                while (true)
                {
                    try
                    {
                        now = DateTime.Now;

                        if (now - lastUpdateTime >= (updateTimeSpan - updateTimeCost))
                        {
                            BeginInvoke(updateAction);
                            lastUpdateTime = now;
                        }

                        if (now - lastDrawTime >= (drawTimeSpan - drawTimeCost))
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

        private void update()
        {
            var now = DateTime.Now;

            dispatcher.update();

            gameRoot.onUpdate();

            updateTimeCost = DateTime.Now - now;
        }

        private void draw()
        {
            var now = DateTime.Now;

            gameGraphic.drawRectangle(background);

            gameRoot.onDraw();

            bufferGraphic.Render();

            ++count;

            if (now - lastUpdateTime >= oneSecondTimeSpan)
            {
                Text = $"{option.title} FPS:{count}";
                count = 0;
                lastUpdateTime = now;
            }

            drawTimeCost = DateTime.Now - now;
        }

        public class Dispatcher
        {
            private Queue<Action> actions = new Queue<Action>();
            private List<Action> list = new List<Action>();

            public void update()
            {
                lock (actions)
                {
                    if (actions.Any())
                    {
                        list.AddRange(actions);

                        actions.Clear();
                    }
                }

                if (list.Any())
                {
                    list.ForEach(o => o());

                    list.Clear();
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
