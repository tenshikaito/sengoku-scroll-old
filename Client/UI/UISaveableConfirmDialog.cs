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
    public class UISaveableConfirmDialog : UIConfirmDialog
    {
        public Action applyButtonClicked;

        protected Button btnApply = new Button();

        public UISaveableConfirmDialog(GameSystem gs) : base(gs)
        {
            CancelButton = null;

            btnApply.Click += (s, e) => applyButtonClicked?.Invoke();
        }

        protected UIDialog addSaveableConfirmButtons()
        {
            var p = new FlowLayoutPanel().init(FlowDirection.RightToLeft).setAutoSizeP().addTo(panel);

            btnApply.addTo(p).Text = w.apply;
            btnCancel.addTo(p).Text = w.cancel;
            btnOk.addTo(p).Text = w.ok;

            return this;
        }
    }
}
