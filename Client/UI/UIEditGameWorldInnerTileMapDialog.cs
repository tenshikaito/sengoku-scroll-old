using Client.Helper;
using Library;
using Library.Helper;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.UI
{
    public class UIEditGameWorldInnerTileMapDialog : UIDialog
    {
        private ListView listView;

        public UIEditGameWorldInnerTileMapDialog(GameSystem gs, Dictionary<int, InnerTileMapInfo> data, Action<int> selectedInnerTileMap) : base(gs)
        {
            this.setCommandWindow(w.inner_tile_map).setAutoSizeF();

            StartPosition = FormStartPosition.Manual;
            Location = new Point(gs.formMain.Location.X, Location.Y);

            listView = new ListView() { MinimumSize = new Size(240, 560)}.init().addColumn(w.name).addTo(panel);

            listView.DoubleClick += (s, e) =>
            {
                if (listView.FocusedItem == null) return;

                selectedInnerTileMap((int)listView.FocusedItem.Tag);
            };

            addConfirmButton();

            setData(data);
        }

        private void setData(Dictionary<int, InnerTileMapInfo> data)
        {
            listView.Items.Clear();

            listView.Items.AddRange(data.Values.Select(o => new ListViewItem()
            {
                Tag = o.id,
                Text = o.name
            }).ToArray());
        }
    }
}
