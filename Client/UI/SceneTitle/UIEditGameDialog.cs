using Client.Helper;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.UI.SceneTitle
{
    public class UIEditGameDialog : UIEditableDialog
    {
        public UIEditGameDialog(GameSystem gs) : base(gs)
        {
            this.setCommandWindow(w.scene_title.edit_game).setCenter(true);

            listView.addColumn(w.name);

            btnEdit.Text = w.edit_game_world;

            listView.DoubleClick += (s, e) => btnEdit.PerformClick();

            addConfirmButton(w.close);
        }
    }
}
