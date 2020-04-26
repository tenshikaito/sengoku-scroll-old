using Library;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public class FormMain : Form
    {
        private Option option;
        private Wording wording;

        private NotifyIcon notifyIcon;

        private TextBox tbPort;
        private Button btnStart;
        private Button btnStop;
        private Button btnRefresh;
        private ListView lvGameWorld;

        private Game game;

        public FormMain()
        {
            loadOption();

            initWindow();

            initSystem();
        }

        private void loadOption()
        {
            option = new Option();
            wording = new Wording();
        }

        private void initWindow()
        {
            Text = option.title;
            StartPosition = FormStartPosition.CenterScreen;
            Icon = new Icon("Icon.ico");

            this.setAutoSizeF();

            var w = wording;

            notifyIcon = new NotifyIcon()
            {
                Icon = Icon,
                Text = option.title
            };

            notifyIcon.DoubleClick += (s, e) =>
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    WindowState = FormWindowState.Normal;

                    Activate();

                    ShowInTaskbar = true;

                    notifyIcon.Visible = false;
                }
            };

            var panel = new FlowLayoutPanel().init().setAutoSizeP().addTo(this);

            var p = new FlowLayoutPanel().init(FlowDirection.LeftToRight).setAutoSizeP().addTo(panel);

            new Label().init(w.port + ":").setAutoSize().setRightCenter().addTo(p);
            tbPort = new TextBox() { Text = option.port.ToString() }.addTo(p);

            btnStart = new Button().init(w.start, onStartButtonClicked).addTo(p);
            btnStop = new Button() { Enabled = false }.init(w.stop, onStopButtonClicked).addTo(p);
            btnRefresh = new Button().init(w.refresh, onRefreshButtonClicked).addTo(p);

            lvGameWorld = new ListView()
            {
                Height = 480,
                Scrollable = true,
            }.init().addColumn(wording.name).addTo(panel);

            lvGameWorld.autoResizeColumns();
        }

        private void initSystem()
        {
            loadGameWorldList();
        }

        private void onStartButtonClicked()
        {
            var gameName = lvGameWorld.FocusedItem;

            if(gameName == null)
            {
                MessageBox.Show("select a item", "info");

                return;
            }

            game = new Game((string)gameName.Tag);

            game.start();

            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void onStopButtonClicked()
        {
            game.stop();

            game = null;

            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void onRefreshButtonClicked()
        {
            loadGameWorldList();
        }

        private void loadGameWorldList()
        {
            var list = GameWorldProcessor.getGameWorldMapList();

            lvGameWorld.Items.Clear();

            lvGameWorld.Items.AddRange(list.Select(o => new ListViewItem() { Text = o, Tag = o }).ToArray());
        }

        protected override void OnResize(EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                ShowInTaskbar = false;

                notifyIcon.Visible = true;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (MessageBox.Show("exit?", "confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                game?.stop();
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
