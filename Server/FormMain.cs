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
        private Button btnPublish;
        private Button btnDelete;
        private ListView lvGameWorld;
        private TextBox tbConsole;
        private TextBox tbConsoleInput;

        private GameSystem gameSystem;
        private Game game;

        public FormMain()
        {
            loadOption();

            initWindow();

            initSystem();
        }

        protected override void OnLoad(EventArgs e)
        {
            lvGameWorld.autoResizeColumns();
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
            MinimizeBox = false;
            MaximizeBox = false;

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
            btnPublish = new Button().init(w.publish, onPublishButtonClicked).addTo(p);
            btnDelete = new Button().init(w.delete, onDeleteButtonClicked).addTo(p);

            var tc = new TabControl()
            {
                Height = 480,
            }.init().addTo(panel);

            var tp = new TabPage().init(w.game_world).addTo(tc);

            lvGameWorld = new ListView()
            {
                Scrollable = true,
            }.init().addColumn(w.name).addTo(tp);

            lvGameWorld.DoubleClick += (s, e) => btnStart.PerformClick();

            tp = new TabPage().init(w.console).addTo(tc);

            tbConsole = new TextBox()
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true
            }.addTo(tp);

            tbConsoleInput = new TextBox()
            {
                Dock = DockStyle.Bottom,
                Enabled = false
            }.addTo(tp);
        }

        private void initSystem()
        {
            gameSystem = new GameSystem()
            {
                formMain = this,
                option = option,
                wording = wording
            };

            loadGameWorldList();
        }

        private void onStartButtonClicked()
        {
            var gameName = lvGameWorld.FocusedItem;

            if (gameName == null)
            {
                MessageBox.Show("select a item", "info");

                return;
            }

            game = new Game(option, (string)gameName.Tag);

            game.start();

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnPublish.Enabled = false;
            btnDelete.Enabled = false;
            lvGameWorld.Enabled = false;
            tbConsoleInput.Enabled = true;
        }

        private void onStopButtonClicked()
        {
            game.stop();

            game = null;

            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnPublish.Enabled = true;
            btnDelete.Enabled = true;
            lvGameWorld.Enabled = true;
            tbConsoleInput.Enabled = false;
        }

        private void onPublishButtonClicked()
        {
            var dialog = new FormGameWorldPublishDialog(gameSystem);

            var publish = new Action<string>(name =>
            {
                GameWorldProcessor.publishMap(name);

                loadGameWorldList();

                dialog.Close();
            });

            dialog.selectGameWorldMap = name =>
            {
                var gameWorldList = GameWorldProcessor.getGameList();

                if (gameWorldList.Contains(name))
                {
                    var cd = new UIConfirmDialog(gameSystem, "alert", "existed, overwrite?", this)
                    {
                        okButtonClicked = () => publish(name),
                    };

                    cd.cancelButtonClicked = () => cd.Close();

                    cd.ShowDialog(this);
                }
                else
                {
                    publish(name);
                }
            };

            dialog.ShowDialog(this);
        }

        private void onDeleteButtonClicked()
        {
            var lvi = lvGameWorld.FocusedItem;

            if (lvi == null) return;

            if (MessageBox.Show("delete?", "confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                new GameWorldProcessor((string)lvi.Tag).game.deleteDirectory();
                
                lvGameWorld.Items.Remove(lvi);
            }
        }

        private void loadGameWorldList()
        {
            var list = GameWorldProcessor.getGameList();

            lvGameWorld.Items.Clear();

            lvGameWorld.Items.AddRange(list.Select(o => new ListViewItem() { Text = o, Tag = o }).ToArray());

            lvGameWorld.selectFirstRow();
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
