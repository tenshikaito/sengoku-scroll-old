using Client.Scene;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Client.UI
{
    public partial class SceneOuterMapControlPanel
    {
        public Wording wording;

        protected SceneOuterMap sceneOuterMap;

        private StrongholdInfoPanel strongholdInfoPanel;

        private Panel panel;

        public SceneOuterMapControlPanel(SceneOuterMap som, Panel p)
        {
            sceneOuterMap = som;
            panel = p;

            strongholdInfoPanel = new StrongholdInfoPanel(this);
        }

        public void setStronghold(Stronghold s)
        {
            strongholdInfoPanel.setValue(s);
        }
    }

    public partial class SceneOuterMapControlPanel
    {
        public abstract class InfoPanelBase
        {
            protected SceneOuterMapControlPanel controlPanel;

            protected Wording w => controlPanel.wording;
            protected Panel panel => controlPanel.panel;
            protected abstract Control[] controls { get; }

            public InfoPanelBase(SceneOuterMapControlPanel p) => controlPanel = p;

            protected void clear() => panel.Controls.Clear();
            protected void add() => panel.Controls.AddRange(controls);
        }
    }

    public partial class SceneOuterMapControlPanel
    {
        public class StrongholdInfoPanel : InfoPanelBase
        {
            private FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            private Label lbName = new Label();

            private Button btnEnter;

            protected override Control[] controls => new Control[]
            {
                lbName,
                buttonPanel
            };

            public StrongholdInfoPanel(SceneOuterMapControlPanel p) : base(p)
            {
                btnEnter = new Button()
                {
                    Text = "进入城内",
                };

                btnEnter.Click += (s, e) => controlPanel.sceneOuterMap.enterCurrentStronghold();

                buttonPanel.Controls.Add(btnEnter);
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
