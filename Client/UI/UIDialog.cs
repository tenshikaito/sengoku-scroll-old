using Client.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Client.UI
{
    public abstract class UIDialog : UIWindow
    {
        public Action okButtonClicked;

        protected Button btnOk = new Button();

        public UIDialog(GameSystem gs) : base(gs)
        {
            AcceptButton = btnOk;
            CancelButton = btnOk;

            btnOk.Click += (s, e) => okButtonClicked?.Invoke();
        }

        protected UIDialog addConfirmButton(string okButtonText = null)
        {
            var tlp = new TableLayoutPanel()
            {
                ColumnCount = 1,
                RowCount = 1,
                Dock = DockStyle.Fill
            }.setAutoSizeP().addTo(panel);

            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            btnOk.addTo(panel);
            btnOk.Text = okButtonText ?? w.ok;
            btnOk.Anchor = AnchorStyles.None;

            return this;
        }
    }
}
