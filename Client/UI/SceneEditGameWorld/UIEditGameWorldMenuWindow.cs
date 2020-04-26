using Client.Helper;
using Library.Helper;
using Library.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.UI.SceneEditGameWorld
{
    public partial class UIEditGameWorldMenuWindow : UIWindow
    {
        public UIEditGameWorldMenuWindow(
            GameSystem gs,
            Action database,
            Action save,
            Action exit) : base(gs)
        {
            this.setCommandWindow(w.scene_title.edit_game).setAutoSizeF().setCenter();

            StartPosition = FormStartPosition.Manual;
            Location = gs.formMain.Location;

            var p = panel;

            new Button().init(w.scene_edit_game_world.database, database).setAutoSize().addTo(p);

            new Button().init(w.scene_edit_game_world.save, save).setAutoSize().addTo(p);

            new Button().init(w.scene_edit_game_world.exit, exit).setAutoSize().addTo(p);
        }
    }
}
