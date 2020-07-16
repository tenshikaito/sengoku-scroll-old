using Library.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.UI
{
    public class UIEditableDialog : UIConfirmDialog
    {
        protected ListView listView;
        protected TableLayoutPanel pButtons;
        protected Button btnAdd;
        protected Button btnEdit;
        protected Button btnRemove;

        public Action addButtonClicked;
        public Action<string> removeButtonClicked;
        public Action<string> editButtonClicked;

        public string selectedValue => listView.FocusedItem == null ? null : (string)listView.FocusedItem.Tag;

        public UIEditableDialog(GameSystem gs) : base(gs)
        {
            var tlp = new TableLayoutPanel()
            {
                ColumnCount = 2,
                RowCount = 1,
                Dock = DockStyle.Fill,
                MinimumSize = new Size(640, 480)
            }.addColumnStyle(80).addColumnStyle(20).addTo(panel);

            listView = new ListView().init().addTo(tlp);

            var p = pButtons = new TableLayoutPanel()
            {
                ColumnCount = 1,
                RowCount = 4,
                Dock = DockStyle.Fill
            }.addColumnStyle(100).setAutoSizeP().addTo(tlp);

            btnAdd = new Button() { Dock = DockStyle.Fill }.init(w.add, () => addButtonClicked?.Invoke()).addTo(p);
            btnEdit = new Button() { Dock = DockStyle.Fill }.init(w.edit, () =>
            {
                var o = listView.FocusedItem;
                if (o == null) return;
                editButtonClicked?.Invoke((string)o.Tag);
            }).addTo(p);
            btnRemove = new Button() { Dock = DockStyle.Fill }.init(w.remove, () =>
            {
                var o = listView.FocusedItem;
                if (o == null) return;
                removeButtonClicked?.Invoke((string)o.Tag);
            }).addTo(p);

            listView.autoResizeColumns();
        }

        protected Button addButton(string text, Action onButtonClicked)
        {
            ++pButtons.RowCount;

            return new Button().init(text, onButtonClicked).addTo(pButtons);
        }

        public void setData(List<string> list) => setData(list.Select(o => new ListViewItem() { Tag = o, Text = o }).ToArray());

        protected void setData(ListViewItem[] list)
        {
            listView.BeginUpdate();
            listView.Items.Clear();
            listView.Items.AddRange(list);
            listView.autoResizeColumns();
            listView.selectFirstRow();
            listView.EndUpdate();
        }
    }
}
