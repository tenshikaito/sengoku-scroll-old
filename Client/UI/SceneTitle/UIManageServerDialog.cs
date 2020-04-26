using Client.Helper;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.UI.SceneTitle
{
    public class UIManageServerDialog : UIConfirmDialog
    {
        private TextBox tbPort;
        private ListView listView;

        public UIManageServerDialog(GameSystem gs, Func<string, bool> connect) : base(gs)
        {
            this.setCommandWindow(w.scene_title.manage_game_world).setAutoSizeF();

            var p = new FlowLayoutPanel().init(FlowDirection.LeftToRight).addTo(panel);

            new Label().init(w.port).setMiddleCenter().setAutoSize().addTo(p);
            tbPort = new TextBox() { Text = gs.option.port.ToString() }.addTo(p);

            var btnConnect = new Button().setAutoSize().addTo(p);

            btnConnect.init(w.connect, () => btnConnect.Enabled = connect(tbPort.Text));

            var tlp = new TableLayoutPanel()
            {
                ColumnCount = 2,
                RowCount = 1
            }.addColumnStyle(80).addColumnStyle(20).addTo(panel);

            listView = new ListView() { MinimumSize = new Size(480, 320) }.init().addTo(tlp);

            //var p = new FlowLayoutPanel().init().addTo(tlp);

            //new Button().init(w.scene_title.manage_game_world, );
        }
    }
}
