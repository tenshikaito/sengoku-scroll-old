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
    public class UIEditGameWorldDialog : UIDialog
    {
        private ListView listView;

        public UIEditGameWorldDialog(
            GameSystem gs,
            Action addButtonClicked,
            Action<string> deleteButtonClicked,
            Action<string> editGameWorldButtonClicked) : base(gs)
        {
            this.setCommandWindow(w.edit).setCenter(true);

            var tlp = new TableLayoutPanel()
            {
                ColumnCount = 2,
                RowCount = 1,
                Dock = DockStyle.Fill,
                MinimumSize = new Size(640, 480)
            }.addColumnStyle(80).addColumnStyle(20).addTo(panel);

            listView = new ListView().init().addColumn(w.name).addTo(tlp);

            var edit = new Action(() =>
            {
                var o = listView.FocusedItem;
                if (o == null) return;
                editGameWorldButtonClicked((string)o.Tag);
            });

            listView.DoubleClick += (s, e) => edit();

            var p = new FlowLayoutPanel().init().setAutoSizeP().addTo(tlp);

            new Button().init(w.add, addButtonClicked).setAutoSize().addTo(p);
            new Button().init(w.delete, () =>
            {
                var o = listView.FocusedItem;
                if (o == null) return;
                deleteButtonClicked((string)o.Tag);
            }).setAutoSize().addTo(p);
            new Button().init(w.edit_game_world, edit).setAutoSize().addTo(p);

            addConfirmButton();

            listView.autoResizeColumns(3);
        }

        public void setData(List<string> list)
        {
            listView.BeginUpdate();
            listView.Items.Clear();
            listView.Items.AddRange(list.Select(o => new ListViewItem() { Tag = o, Text = o }).ToArray());
            listView.autoResizeColumns(3);
            listView.EndUpdate();
        }

        public void delete(string name)
        {
            var item = listView.Items.Cast<ListViewItem>().FirstOrDefault(o => (string)o.Tag == name);

            if (item == null) return;

            listView.Items.Remove(item);
        }
    }
}
