using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Helper
{
    public static class UIExtensionHelper
    {
        public static T addTo<T>(this T c, Control oc) where T : Control
        {
            oc.Controls.Add(c);

            return c;
        }

        public static Form setAutoSizeF(this Form f)
        {
            f.setAutoSize();
            f.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            return f;
        }

        public static Form setCenter(this Form f, bool isForce = false)
        {
            if (isForce)
            {
                var rect = SystemInformation.WorkingArea;
                f.Location = new Point((rect.Width - f.Width) / 2, (rect.Height - f.Height) / 2 - 20);
            }
            else
            {
                f.StartPosition = FormStartPosition.CenterScreen;
            }

            return f;
        }

        public static Form init(this Form f, string title)
        {
            f.Text = title;

            return f;
        }

        public static T setCommandWindow<T>(this T f, string title) where T : Form
        {
            f.Text = title;
            f.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            f.SizeGripStyle = SizeGripStyle.Hide;

            f.hideCommandButton();

            return f;
        }

        public static T setAutoSize<T>(this T c) where T : Control
        {
            c.AutoSize = true;

            return c;
        }

        public static T refreshListViewOnClick<T>(this T c, ListView lv, Action<ListView> focus) where T : Control
        {
            var a = new EventHandler((s, e) => focus(lv));

            c.Click += a;

            return c;
        }

        public static T setAutoSizeP<T>(this T p) where T : Panel
        {
            p.setAutoSize();
            p.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            return p;
        }

        public static Form hideCommandButton(this Form f)
        {
            f.ControlBox = false;

            return f;
        }

        public static FlowLayoutPanel init(this FlowLayoutPanel p, FlowDirection fd = FlowDirection.TopDown)
        {
            p.Dock = DockStyle.Fill;
            p.FlowDirection = fd;

            return p;
        }

        public static Button init(this Button b, string text, Action onClick, bool isFill = true)
        {
            if (isFill) b.Dock = DockStyle.Fill;
            b.Text = text;
            b.Click += (s, e) => onClick();

            return b;
        }

        public static Button setAutoSize(this Button b)
        {
            b.AutoSize = true;
            b.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            return b;
        }

        public static TextBox setMultiLine(this TextBox tb)
        {
            tb.Multiline = true;

            return tb;
        }

        public static Label init(this Label l, string text)
        {
            l.Text = text;

            return l;
        }

        public static Label setLeftCenter(this Label l)
        {
            l.Dock = DockStyle.Left;
            l.TextAlign = ContentAlignment.MiddleLeft;

            return l;
        }

        public static Label setMiddleCenter(this Label l)
        {
            l.TextAlign = ContentAlignment.MiddleCenter;

            return l;
        }

        public static Label setRightCenter(this Label l)
        {
            l.Dock = DockStyle.Right;
            l.TextAlign = ContentAlignment.MiddleRight;

            return l;
        }

        public static CheckBox init(this CheckBox cb, string text = null, bool isChecked = false, Action onClick = null)
        {
            cb.Dock = DockStyle.Fill;
            cb.Text = text;
            cb.Checked = isChecked;
            if (onClick != null) cb.Click += (s, e) => onClick();

            return cb;
        }

        public static CheckBox setButtonStyle(this CheckBox cb)
        {
            cb.Appearance = Appearance.Button;

            return cb;
        }

        public static TabControl init(this TabControl tc)
        {
            tc.Dock = DockStyle.Fill;

            return tc;
        }

        public static TabPage init(this TabPage tp, string title)
        {
            tp.Text = title;

            return tp;
        }

        public static ListView init(this ListView lv)
        {
            lv.Dock = DockStyle.Fill;
            lv.FullRowSelect = true;
            lv.View = View.Details;

            return lv;
        }

        public static ListView addColumn(this ListView lv, string title, int width = 120, HorizontalAlignment ha = HorizontalAlignment.Center)
        {
            lv.Columns.Add(title, width, ha);

            return lv;
        }

        public static ListView autoResizeColumns(this ListView lv, int offset = 0)
        {
            lv.BeginUpdate();

            lv.Columns.Cast<ColumnHeader>().ToList().ForEach(o =>
            {
                o.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);

                var widthHeader = o.Width;

                o.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

                if (widthHeader > o.Width) o.Width = widthHeader - offset;
            });

            lv.EndUpdate();

            return lv;
        }

        public static ListViewItem addColumn(this ListViewItem lvi, string text)
        {
            lvi.SubItems.Add(text);

            return lvi;
        }

        public static ComboBox initDropDownList<TKey>(
            this ComboBox cb,
            IList<KeyValuePair<TKey, string>> data,
            TKey? value,
            string valueField = "Key",
            string textField = "Value") where TKey : struct

        {
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
            cb.DataSource = data;
            cb.ValueMember = valueField;
            cb.DisplayMember = textField;
            if (value != null) cb.SelectedValue = value;

            return cb;
        }
    }
}
