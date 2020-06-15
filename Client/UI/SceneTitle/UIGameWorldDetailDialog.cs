using Client.Helper;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Client.UI.SceneTitle
{
    public class UIGameWorldDetailDialog : UIConfirmDialog
    {
        private TextBox tbName;
        private TextBox tbWidth;
        private TextBox tbHeight;

        public (string name, string width, string height) value => (tbName.Text, tbWidth.Text, tbHeight.Text);

        public UIGameWorldDetailDialog(GameSystem gs) : base(gs)
        {
            this.setCommandWindow(w.add);

            var p = new TableLayoutPanel()
            {
                ColumnCount = 2
            }.setAutoSizeP().addTo(panel);

            new Label() { Text = w.name }.setRightCenter().setAutoSize().addTo(p);

            tbName = new TextBox() { Text = "test" }.addTo(p);

            new Label() { Text = w.width }.setRightCenter().setAutoSize().addTo(p);

            tbWidth = new TextBox() { Text = "100" }.addTo(p);

            new Label() { Text = w.height }.setRightCenter().setAutoSize().addTo(p);

            tbHeight = new TextBox() { Text = "100" }.addTo(p);

            addConfirmButtons();
        }
    }
}
