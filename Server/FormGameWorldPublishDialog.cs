using Library;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class FormGameWorldPublishDialog : UIConfirmDialog
    {
        public Action<string> selectGameWorldMap;

        private ListView listView;

        public FormGameWorldPublishDialog(GameSystem gs) : base(gs)
        {
            MinimizeBox = false;
            MaximizeBox = false;

            this.init(w.publish).setAutoSizeF();

            listView = new ListView()
            {
                MinimumSize = new Size(480, 420),
                Scrollable = false
            }.init().addTo(panel);

            listView.addColumn(w.name);

            listView.DoubleClick += (s, e) => onGameMapSelected();

            var data = GameWorldProcessor.getMapList();

            listView.Items.AddRange(data.Select(o => new ListViewItem() { Tag = o, Text = o }).ToArray());

            listView.selectFirstRow().autoResizeColumns();

            okButtonClicked = onGameMapSelected;

            addConfirmButtons(w.publish);
        }

        private void onGameMapSelected()
        {
            if (listView.FocusedItem == null) return;

            selectGameWorldMap?.Invoke((string)listView.FocusedItem.Tag);
        }
    }
}
