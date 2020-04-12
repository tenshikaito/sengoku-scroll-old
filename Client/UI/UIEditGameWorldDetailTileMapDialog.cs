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
    public class UIEditGameWorldDetailTileMapDialog : UIDialog
    {
        private ListView listView;

        public UIEditGameWorldDetailTileMapDialog(GameSystem gs, Dictionary<int, DetailTileMapInfo> data, Action<int> selectedDetailTileMap) : base(gs)
        {
            this.setCommandWindow(w.detail_tile_map).setAutoSizeF();

            StartPosition = FormStartPosition.Manual;
            Location = new Point(gs.formMain.Location.X, Location.Y);

            listView = new ListView() { MinimumSize = new Size(240, 560)}.init().addColumn(w.name).addTo(panel);

            listView.DoubleClick += (s, e) =>
            {
                if (listView.FocusedItem == null) return;

                selectedDetailTileMap((int)listView.FocusedItem.Tag);
            };

            addConfirmButton();

            setData(data);
        }

        private void setData(Dictionary<int, DetailTileMapInfo> data)
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
