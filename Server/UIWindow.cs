using Library.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Server
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

        protected T addMessage<T>(string text) where T : UIWindow
        {
            new Label() { Margin = new Padding(20) }.init(text).setAutoSize().setMiddleCenter().addTo(panel);

            return (T)this;
        }
    }
}
