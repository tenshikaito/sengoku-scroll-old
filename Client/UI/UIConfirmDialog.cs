using Client.Helper;
using Library.Helper;
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

        public bool isBtnCancelEnabled
        {
            get => btnCancel.Enabled;
            set => btnCancel.Enabled = value;
        }

        public UIConfirmDialog(GameSystem gs) : base(gs)
        {
            CancelButton = btnCancel;

            btnCancel.Click += (s, e) => cancelButtonClicked?.Invoke();
        }

        public UIConfirmDialog(GameSystem gs, string title, string text) : this(gs)
        {
            this.setCommandWindow(title).addMessage<UIConfirmDialog>(text);

            addConfirmButtons();

            cancelButtonClicked = Close;
        }

        public UIConfirmDialog addConfirmButtons(string okButtonText = null, string cancelButtonText = null)
        {
            var tlp = new TableLayoutPanel()
            {
                ColumnCount = 2,
                RowCount = 1,
                Dock = DockStyle.Fill
            }.addColumnStyle(50).addColumnStyle(50).setAutoSizeP().addTo(panel);

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
