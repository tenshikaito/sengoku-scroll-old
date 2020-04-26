using Client.Helper;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Client.UI.SceneTitle
{
    public class UIMainMenuWindow : UIWindow
    {
        public UIMainMenuWindow(GameSystem gs, Action startGame, Action editGame) : base(gs)
        {
            this.setCommandWindow(w.scene_title.start).setAutoSizeF().setCenter();

            new Button().init(w.scene_title.start_game, startGame).setAutoSize().addTo(panel);

            new Button().init(w.scene_title.edit_game, editGame).setAutoSize().addTo(panel);
        }
    }
}
