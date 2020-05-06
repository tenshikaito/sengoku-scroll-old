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
    public partial class UIEditGameWorldMainTileMapMenuWindow : UIWindow
    {
        private ListView lvTerrain;

        public UIEditGameWorldMainTileMapMenuWindow(
            GameSystem gs,
            Action pointer,
            Action brush,
            Action rectangle,
            Action fill,
            Action detailTileMap,
            Action<int> onTerrainSelected) : base(gs)
        {
            this.setCommandWindow(w.main_tile_map).setAutoSizeF().setCenter();

            StartPosition = FormStartPosition.Manual;
            Location = new Point(gs.formMain.Location.X, gs.formMain.Location.Y + 240);

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

            new Button().init(w.detail_tile_map, detailTileMap).setAutoSize().addTo(p);

            var tc = new TabControl().init().addTo(panel);

            var tp = new TabPage().init(w.terrain.text).addTo(tc);

            lvTerrain = new ListView() { MinimumSize = new Size(360, 540) }.init().addTo(tp);

            lvTerrain.addColumn(w.name).autoResizeColumns(-3);

            lvTerrain.SelectedIndexChanged += (s, e) =>
            {
                if (lvTerrain.FocusedItem == null) return;

                onTerrainSelected((int)lvTerrain.FocusedItem.Tag);
            };
        }

        public void setTerrain(List<TerrainImage> data)
        {
            lvTerrain.Items.Clear();

            lvTerrain.BeginUpdate();

            lvTerrain.Items.AddRange(data.Select(o => new ListViewItem() { Tag = o.id, Text = o.name }).ToArray());

            lvTerrain.autoResizeColumns(-3);

            lvTerrain.EndUpdate();
        }
    }
}
