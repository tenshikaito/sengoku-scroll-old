using Client.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Client.UI
{
    public class UICreateGameWorldDialog : UIConfirmDialog
    {
        private TextBox tbName;
        private TextBox tbWidth;
        private TextBox tbHeight;

        public UICreateGameWorldDialog(GameSystem gs, Action<string, string, string> okButtonClicked) : base(gs)
        {
            var p = new TableLayoutPanel()
            {
                ColumnCount = 2
            }.setAutoSizeP().addTo(panel);

            new Label() { Text = w.name }.setRightCenter().setAutoSize().addTo(p);

            tbName = new TextBox() { Text = "test" }.addTo(p);

            new Label() { Text = w.scene_title.game_world_width }.setRightCenter().setAutoSize().addTo(p);

            tbWidth = new TextBox() { Text = "100" }.addTo(p);

            new Label() { Text = w.scene_title.game_world_height }.setRightCenter().setAutoSize().addTo(p);

            tbHeight = new TextBox() { Text = "100" }.addTo(p);

            addConfirmButtons();

            btnOk.Click += (s, e) => okButtonClicked(tbName.Text, tbWidth.Text, tbHeight.Text);
        }

    }
}
