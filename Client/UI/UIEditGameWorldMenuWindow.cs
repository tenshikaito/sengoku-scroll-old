using Client.Helper;
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

namespace Client.UI
{
    public partial class UIEditGameWorldMenuWindow : UIWindow
    {
        private ListView lvTerrain;

        public UIEditGameWorldMenuWindow(
            GameSystem gs,
            Action pointer,
            Action brush,
            Action rectangle,
            Action fill,
            Action innerTileMap,
            Action database,
            Action save,
            Action exit,
            Action<byte> onTerrainSelected) : base(gs)
        {
            this.setCommandWindow(w.scene_title.edit_game).setAutoSizeF().setCenter();

            StartPosition = FormStartPosition.Manual;
            Location = gs.formMain.Location;

            var list = new List<CheckBox>();
            var p = new FlowLayoutPanel().init(FlowDirection.LeftToRight).setAutoSizeP().addTo(panel);

            var cbPointer = new CheckBox();

            list.Add(cbPointer.init(w.scene_edit_game_world.pointer, true, () =>
            {
                list.ForEach(o => o.Checked = false);
                cbPointer.Checked = true;
                pointer();
            }).setButtonStyle().setAutoSize().addTo(p));

            var cbBrush = new CheckBox();

            list.Add(cbBrush.init(w.scene_edit_game_world.brush, false, () =>
            {
                list.ForEach(o => o.Checked = false);
                cbBrush.Checked = true;
                brush();
            }).setButtonStyle().setAutoSize().addTo(p));

            var cbRectangle = new CheckBox();

            list.Add(cbRectangle.init(w.scene_edit_game_world.rectangle, false, () =>
            {
                list.ForEach(o => o.Checked = false);
                cbRectangle.Checked = true;
                rectangle();
            }).setButtonStyle().setAutoSize().addTo(p));

            var cbFill = new CheckBox();

            list.Add(cbFill.init(w.scene_edit_game_world.fill, false, () =>
            {
                list.ForEach(o => o.Checked = false);
                cbFill.Checked = true;
                fill();
            }).setButtonStyle().setAutoSize().addTo(p));

            p = new FlowLayoutPanel().init(FlowDirection.LeftToRight).setAutoSizeP().addTo(panel);

            new Button().init(w.inner_tile_map, innerTileMap).setAutoSize().addTo(p);

            new Button().init(w.scene_edit_game_world.database, database).setAutoSize().addTo(p);

            new Button().init(w.scene_edit_game_world.save, save).setAutoSize().addTo(p);

            new Button().init(w.scene_edit_game_world.exit, exit).setAutoSize().addTo(p);

            var tc = new TabControl().init().addTo(panel);

            var tp = new TabPage().init(w.terrain.text).addTo(tc);

            lvTerrain = new ListView().init().addTo(tp);

            lvTerrain.addColumn(w.name).autoResizeColumns(-3);

            lvTerrain.SelectedIndexChanged += (s, e) =>
            {
                if (lvTerrain.FocusedItem == null) return;

                onTerrainSelected((byte)lvTerrain.FocusedItem.Tag);
            };
        }

        public void setTerrain(List<Terrain> data)
        {
            lvTerrain.Items.Clear();

            lvTerrain.BeginUpdate();

            lvTerrain.Items.AddRange(data.Select(o => new ListViewItem() { Tag = o.id, Text = o.name }).ToArray());

            lvTerrain.autoResizeColumns(-3);

            lvTerrain.EndUpdate();
        }
    }
}
