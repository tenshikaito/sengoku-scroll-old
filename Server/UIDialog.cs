using Library.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public class UIDialog : UIWindow
    {
        public Action okButtonClicked;

        protected Button btnOk = new Button();

        public UIDialog(GameSystem gs) : base(gs)
        {
            AcceptButton = btnOk;
            CancelButton = btnOk;

            btnOk.Click += (s, e) => okButtonClicked?.Invoke();
        }

        public UIDialog(GameSystem gs, string title, string text, Form owner = null) : this(gs)
        {
            this.setCommandWindow(title).addMessage<UIDialog>(text);

            if (owner != null) Owner = owner;

            addConfirmButton();

            okButtonClicked = Close;
        }

        protected UIDialog addConfirmButton(string okButtonText = null)
        {
            btnOk.addTo(panel);
            btnOk.Text = okButtonText ?? w.ok;
            btnOk.Anchor = AnchorStyles.None;

            return this;
        }
    }
}
