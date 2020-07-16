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
    public class UIPlayerDialog : UIEditableDialog
    {
        public string name => listView.FocusedItem == null ? null : (string)listView.FocusedItem.Tag;

        public UIPlayerDialog(GameSystem gs) : base(gs)
        {
            this.setCommandWindow(w.scene_title.start_game).setCenter(true);

            listView.addColumn(w.name);

            listView.DoubleClick += (s, e) => btnOk.PerformClick();

            addConfirmButton(w.ok);
        }
    }
}
