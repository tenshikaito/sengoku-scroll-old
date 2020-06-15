using Client.Model;
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
    public class UIStartGameDialog : UIEditableDialog
    {
        public Action refreshButtonClicked;

        public UIStartGameDialog(GameSystem gs) : base(gs)
        {
            this.setCommandWindow(w.scene_title.start_game).setCenter(true);

            listView.addColumn(w.name).addColumn(w.ip).addColumn(w.status);

            addButton(w.refresh, refreshButtonClicked);

            addConfirmButtons();
        }

        public void setData(List<ServerInfo> list) => setData(list.Select(o => new ListViewItem()
        {
            Tag = o.code,
            Text = o.name
        }.addColumn(o.ip + ":" + o.port).addColumn(w.symbol_none)).ToArray());
    }
}
