using Client.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Client.UI
{
    public abstract class UIWindow : Form
    {
        protected GameSystem gameSystem;
        protected Wording w => gameSystem.wording;

        protected FlowLayoutPanel panel = new FlowLayoutPanel();

        public UIWindow(GameSystem gs)
        {
            gameSystem = gs;

            Owner = gs.formMain;

            panel.init().setAutoSizeP().addTo(this);

            this.setAutoSizeF().setCenter();
        }
    }
}
