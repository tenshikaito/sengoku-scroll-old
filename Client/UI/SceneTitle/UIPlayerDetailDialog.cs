using Client.Helper;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Client.UI.SceneTitle
{
    public class UIPlayerDetailDialog : UIConfirmDialog
    {
        private TextBox tbName;

        public string name => tbName.Text;

        public UIPlayerDetailDialog(GameSystem gs) : base(gs)
        {
            this.setCommandWindow(w.detail);

            var p = new TableLayoutPanel()
            {
                ColumnCount = 2
            }.setAutoSizeP().addTo(panel);

            new Label() { Text = w.name }.setRightCenter().setAutoSize().addTo(p);

            tbName = new TextBox() { Text = "player" }.addTo(p);

            addConfirmButtons();
        }
    }
}
