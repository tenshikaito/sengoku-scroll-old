using Client.Helper;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Client.UI.SceneTitle
{
    public class UIGameServerDetailDialog : UIConfirmDialog
    {
        private TextBox tbName;
        private TextBox tbIp;
        private TextBox tbPort;

        public (string name, string ip, string port) value => (tbName.Text, tbIp.Text, tbPort.Text);

        public UIGameServerDetailDialog(GameSystem gs) : base(gs)
        {
            this.setCommandWindow(w.add);

            var p = new TableLayoutPanel()
            {
                ColumnCount = 2
            }.setAutoSizeP().addTo(panel);

            new Label() { Text = w.name }.setRightCenter().setAutoSize().addTo(p);

            tbName = new TextBox() { Text = "server" }.addTo(p);

            new Label() { Text = w.ip }.setRightCenter().setAutoSize().addTo(p);

            tbIp = new TextBox() { Text = "127.0.0.1" }.addTo(p);

            new Label() { Text = w.port }.setRightCenter().setAutoSize().addTo(p);

            tbPort = new TextBox() { Text = "7789" }.addTo(p);

            addConfirmButtons();
        }
    }
}
