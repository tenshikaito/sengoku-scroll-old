﻿using Client.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.UI
{
    public class UIConfirmDialog : UIDialog
    {
        public Action cancelButtonClicked;

        protected Button btnCancel = new Button();

        public UIConfirmDialog(GameSystem gs) : base(gs)
        {
            CancelButton = btnCancel;

            btnCancel.Click += (s, e) => cancelButtonClicked?.Invoke();
        }

        protected UIDialog addConfirmButtons(string okButtonText = null, string cancelButtonText = null)
        {
            var tlp = new TableLayoutPanel()
            {
                ColumnCount = 2,
                RowCount = 1,
                Dock = DockStyle.Fill
            }.setAutoSizeP().addTo(panel);

            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            btnOk.addTo(tlp);
            btnOk.Text = okButtonText ?? w.ok;
            btnOk.Anchor = AnchorStyles.Right;
            btnCancel.addTo(tlp);
            btnCancel.Text = cancelButtonText ?? w.cancel;
            btnCancel.Anchor = AnchorStyles.Left;

            return this;
        }
    }
}
