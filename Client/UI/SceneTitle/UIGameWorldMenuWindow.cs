using Client.Helper;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.UI.SceneTitle
{
    public class UIGameWorldMenuWindow : UIDialog
    {
        public UIGameWorldMenuWindow(GameSystem gs, Action selectGameWorld, Action manageGameWorld) : base(gs)
        {
            this.setCommandWindow(w.scene_title.start_game).setAutoSizeF();

            new Button().init(w.scene_title.select_game_world, selectGameWorld).addTo(panel);

            new Button().init(w.scene_title.manage_game_world, manageGameWorld).addTo(panel);

            addConfirmButton(w.cancel);
        }
    }
}
