using Client.Scene;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Client.UI
{
    public partial class SceneInnerMapControlPanel
    {
        public Wording wording;

        protected SceneInnerMap sceneInnerMap;

        private StrongholdInfoPanel strongholdInfoPanel;

        private Panel panel;

        public SceneInnerMapControlPanel(SceneInnerMap sim, Panel p)
        {
            sceneInnerMap = sim;
            panel = p;

            strongholdInfoPanel = new StrongholdInfoPanel(this);
        }

        public void setStronghold(Stronghold s)
        {
            strongholdInfoPanel.setValue(s);
        }
    }

    public partial class SceneInnerMapControlPanel
    {
        public abstract class InfoPanelBase
        {
            protected SceneInnerMapControlPanel controlPanel;

            protected Wording w => controlPanel.wording;
            protected Panel panel => controlPanel.panel;
            protected abstract Control[] controls { get; }

            public InfoPanelBase(SceneInnerMapControlPanel p) => controlPanel = p;

            protected void clear() => panel.Controls.Clear();
            protected void add() => panel.Controls.AddRange(controls);
        }
    }

    public partial class SceneInnerMapControlPanel
    {
        public class StrongholdInfoPanel : InfoPanelBase
        {
            private FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            private Label lbName = new Label();

            private Button btnExit;

            protected override Control[] controls => new Control[]
            {
                lbName,
                buttonPanel
            };

            public StrongholdInfoPanel(SceneInnerMapControlPanel p) : base(p)
            {
                btnExit = new Button()
                {
                    Text = "离开城内",
                };

                btnExit.Click += (s, e) => controlPanel.sceneInnerMap.exitStronghold();

                buttonPanel.Controls.Add(btnExit);
            }

            public void setValue(Stronghold s)
            {
                clear();

                setInfo(s);
                setButtons(s);

                add();
            }

            private void setInfo(Stronghold s)
            {
                lbName.Text = $"{w.stronghold}:{s.name}";
            }

            private void setButtons(Stronghold s)
            {

            }
        }
    }

}
