using Client.Helper;
using Library;
using Library.Helper;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.UI.SceneEditGameWorld
{
    public partial class UIEditGameWorldDatabaseWindow : UISaveableConfirmDialog
    {
        public MasterData gameWorldMasterData;

        private TabControl tabControl;

        public UIEditGameWorldDatabaseWindow(GameSystem gs, GameWorld gw) : base(gs)
        {
            gameWorldMasterData = gw.masterData;

            MaximizeBox = false;

            this.init(w.scene_edit_game_world.database).setCenter();

            var tc = tabControl = new TabControl() { MinimumSize = new System.Drawing.Size(720, 480) }.init().setAutoSize().addTo(panel);

            new TabPageTerrain(this, gameWorldMasterData.terrain).addTo(tc);

            new TabPageRegion(this, gameWorldMasterData.region).addTo(tc);

            new TabPageCulture(this, gameWorldMasterData.culture).addTo(tc);

            new TabPageReligion(this, gameWorldMasterData.religion).addTo(tc);

            new TabPageRoad(this, gameWorldMasterData.road).addTo(tc);

            new TabPageStronghold(this, gameWorldMasterData.strongholdType, gameWorldMasterData).addTo(tc);

            new TabPageDetailTileMapInfo(this, gameWorldMasterData.detailTileMapInfo, gameWorldMasterData.terrain).addTo(tc);

            new TabPageMainMapTileImageInfo(this, gameWorldMasterData.mainTileMapImageInfo, gameWorldMasterData).addTo(tc);

            new TabPageDetailMapTileImageInfo(this, gameWorldMasterData.detailTileMapImageInfo, gameWorldMasterData).addTo(tc);

            addSaveableConfirmButtons();
        }

        private class TabPageBase : TabPage
        {
            protected UIEditGameWorldDatabaseWindow window;

            protected Wording w => window.w;

            public TabPageBase(UIEditGameWorldDatabaseWindow bw) => window = bw;

            protected (TableLayoutPanel sc,
                TableLayoutPanel pl,
                ListView lv)
                createIndexControl(Action<ListView> btnAddClick, Action<ListView, int> btnDeleteClick)
            {
                var sc = new TableLayoutPanel()
                {
                    RowCount = 2,
                    ColumnCount = 1,
                    Dock = DockStyle.Fill
                }.addRowStyle(50).addRowStyle(50).addTo(this);

                var gb = new GroupBox()
                {
                    Dock = DockStyle.Fill,
                    Text = w.type
                }.addTo(sc);

                var pl = new TableLayoutPanel()
                {
                    RowCount = 1,
                    ColumnCount = 2,
                    Dock = DockStyle.Fill,
                }.addColumnStyle(80).addColumnStyle(20).addTo(gb);

                var lv = new ListView()
                {
                    Dock = DockStyle.Fill,

                }.init().addTo(pl);

                var p2 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1
                }.addTo(pl);

                new Button().init(w.add, () => btnAddClick(lv)).addTo(p2);
                new Button().init(w.delete, () =>
                {
                    if (lv.FocusedItem == null) return;

                    btnDeleteClick(lv, (byte)lv.FocusedItem.Tag);
                }).addTo(p2);

                new Label().addTo(p2);

                return (sc, pl, lv);
            }
        }
    }

    public partial class UIEditGameWorldDatabaseWindow
    {
        private class TabPageTerrain : TabPageBase
        {
            private Dictionary<int, Terrain> data;
            private int? currentId;

            private TextBox tbName;
            private CheckBox cbIsGrass;
            private CheckBox cbIsHill;
            private CheckBox cbIsMountain;
            private CheckBox cbIsHabour;
            private CheckBox cbIsWater;
            private CheckBox cbIsFreshWater;
            private CheckBox cbIsDeepWater;

            public TabPageTerrain(UIEditGameWorldDatabaseWindow bw, Dictionary<int, Terrain> data) : base(bw)
            {
                this.data = data;

                this.init(w.terrain.text).setAutoSizeP();

                var (sc, pl, lv) = createIndexControl(onAddButtonClicked, onDeleteButtonClicked);

                lv.addColumn(w.id)
                    .addColumn(w.name)
                    .addColumn(w.terrain.is_grass)
                    .addColumn(w.terrain.is_hill)
                    .addColumn(w.terrain.is_mountain)
                    .addColumn(w.terrain.is_habour)
                    .addColumn(w.terrain.is_water)
                    .addColumn(w.terrain.is_fresh_water)
                    .addColumn(w.terrain.is_deep_water);

                lv.Click += (s, e) =>
                {
                    if (lv.FocusedItem == null) return;

                    onListViewClicked(lv, (byte)lv.FocusedItem.Tag);
                };

                var p2 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 2
                }.addRowStyle(80).addRowStyle(20).addTo(sc);

                var gb = new GroupBox()
                {
                    Dock = DockStyle.Fill,
                    Text = w.scene_edit_game_world.property
                }.addTo(p2);

                var p3 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    AutoScroll = true
                }.addTo(gb);

                new Label().init(w.name + ":").setAutoSize().setRightCenter().addTo(p3);
                tbName = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.terrain.is_grass + ":").setAutoSize().setRightCenter().addTo(p3);
                cbIsGrass = new CheckBox().init().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.terrain.is_hill + ":").setAutoSize().setRightCenter().addTo(p3);
                cbIsHill = new CheckBox().init().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.terrain.is_mountain + ":").setAutoSize().setRightCenter().addTo(p3);
                cbIsMountain = new CheckBox().init().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.terrain.is_habour + ":").setAutoSize().setRightCenter().addTo(p3);
                cbIsHabour = new CheckBox().init().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.terrain.is_water + ":").setAutoSize().setRightCenter().addTo(p3);
                cbIsWater = new CheckBox().init().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.terrain.is_fresh_water + ":").setAutoSize().setRightCenter().addTo(p3);
                cbIsFreshWater = new CheckBox().init().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.terrain.is_deep_water + ":").setAutoSize().setRightCenter().addTo(p3);
                cbIsDeepWater = new CheckBox().init().refreshListViewOnClick(lv, refresh).addTo(p3);

                initData(lv);

                lv.autoResizeColumns();
            }

            private void initData(ListView lv) => data.Values.ToList().ForEach(o => lv.Items.Add(setRow(new ListViewItem() { Tag = o.id }, o)));

            private void onAddButtonClicked(ListView lv)
            {
                var max = data.getMaxId(-1);

                if (++max > byte.MaxValue) return;

                var oo = new Terrain()
                {
                    id = (byte)max,
                    name = w.terrain.text,
                };

                data[oo.id] = oo;

                var lvi = new ListViewItem()
                {
                    Tag = oo.id
                };

                lv.Items.Add(setRow(lvi, oo));

                lv.autoResizeColumns();
            }

            private void onDeleteButtonClicked(ListView lv, int id)
            {
                lv.Items.Remove(lv.Items.Cast<ListViewItem>().First(o => (byte)o.Tag == id));

                data.Remove(id);

                if (currentId == id) currentId = null;

                lv.autoResizeColumns();
            }

            private ListViewItem setRow(ListViewItem lvi, Terrain o)
            {
                lvi.Text = o.id.ToString();

                return lvi.addColumn(o.name)
                    .addColumn(o.isGrass.getSymbol(w))
                    .addColumn(o.isHill.getSymbol(w))
                    .addColumn(o.isMountain.getSymbol(w))
                    .addColumn(o.isHarbour.getSymbol(w))
                    .addColumn(o.isWater.getSymbol(w))
                    .addColumn(o.isFreshWater.getSymbol(w))
                    .addColumn(o.isDeepWater.getSymbol(w));
            }

            private void refresh(ListView lv)
            {
                if (currentId != null)
                {
                    var oo = data[(int)currentId];

                    oo.name = tbName.Text;
                    oo.isGrass = cbIsGrass.Checked;
                    oo.isHill = cbIsHill.Checked;
                    oo.isMountain = cbIsMountain.Checked;
                    oo.isHarbour = cbIsHabour.Checked;
                    oo.isWater = cbIsWater.Checked;
                    oo.isFreshWater = cbIsFreshWater.Checked;
                    oo.isDeepWater = cbIsDeepWater.Checked;

                    var lvi = lv.Items.Cast<ListViewItem>().FirstOrDefault(o => (byte)o.Tag == currentId);

                    if (lvi != null)
                    {
                        lvi.SubItems.Clear();
                        setRow(lvi, oo);
                        lv.autoResizeColumns();
                    }
                }
            }

            private void onListViewClicked(ListView lv, int id)
            {
                refresh(lv);

                currentId = id;
                var o = data[id];

                tbName.Text = o.name;
                cbIsGrass.Checked = o.isGrass;
                cbIsHill.Checked = o.isHill;
                cbIsMountain.Checked = o.isMountain;
                cbIsHabour.Checked = o.isHarbour;
                cbIsWater.Checked = o.isWater;
                cbIsFreshWater.Checked = o.isFreshWater;
                cbIsDeepWater.Checked = o.isDeepWater;
            }
        }
    }

    public partial class UIEditGameWorldDatabaseWindow
    {
        private class TabPageRegion : TabPageBase
        {
            private Dictionary<int, Region> data;
            private int? currentId;

            private TextBox tbName;
            private ComboBox cbClimate;

            public TabPageRegion(UIEditGameWorldDatabaseWindow bw, Dictionary<int, Region> data) : base(bw)
            {
                this.data = data;

                this.init(w.region.text).setAutoSizeP();

                var (sc, pl, lv) = createIndexControl(onAddButtonClicked, onDeleteButtonClicked);

                lv.addColumn(w.id)
                    .addColumn(w.name)
                    .addColumn(w.region.climate);

                lv.Click += (s, e) =>
                {
                    if (lv.FocusedItem == null) return;

                    onListViewClicked(lv, (byte)lv.FocusedItem.Tag);
                };

                var p2 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 2
                }.addRowStyle(80).addRowStyle(20).addTo(sc);

                var gb = new GroupBox()
                {
                    Dock = DockStyle.Fill,
                    Text = w.scene_edit_game_world.property
                }.addTo(p2);

                var p3 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    AutoScroll = true
                }.addTo(gb);

                new Label().init(w.name + ":").setAutoSize().setRightCenter().addTo(p3);
                tbName = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.region.climate + ":").setAutoSize().setRightCenter().addTo(p3);
                cbClimate = new ComboBox()
                    .initDropDownList(Library.Model.Region.list.Select(o => new KeyValuePair<Climate, string>(o, w.region[o.ToString()])).ToList(), Climate.normal)
                    .refreshListViewOnClick(lv, refresh)
                    .addTo(p3);

                new Label().init("").addTo(p3);

                initData(lv);

                lv.autoResizeColumns();
            }

            private void initData(ListView lv) => data.Values.ToList().ForEach(o => lv.Items.Add(setRow(new ListViewItem() { Tag = o.id }, o)));

            private void onAddButtonClicked(ListView lv)
            {
                var max = data.getMaxId(-1);

                if (++max > byte.MaxValue) return;

                var oo = new Region()
                {
                    id = (byte)max,
                    name = w.region.text,
                };

                data[oo.id] = oo;

                var lvi = new ListViewItem()
                {
                    Tag = oo.id
                };

                lv.Items.Add(setRow(lvi, oo));

                lv.autoResizeColumns();
            }

            private void onDeleteButtonClicked(ListView lv, int id)
            {
                lv.Items.Remove(lv.Items.Cast<ListViewItem>().First(o => (byte)o.Tag == id));

                data.Remove(id);

                if (currentId == id) currentId = null;

                lv.autoResizeColumns();
            }

            private ListViewItem setRow(ListViewItem lvi, Region o)
            {
                lvi.Text = o.id.ToString();

                return lvi.addColumn(o.name)
                    .addColumn(w.region[o.climate.ToString()]);
            }

            private void refresh(ListView lv)
            {
                if (currentId != null)
                {
                    var oo = data[(int)currentId];

                    oo.name = tbName.Text;
                    oo.climate = (Climate)cbClimate.SelectedValue;

                    var lvi = lv.Items.Cast<ListViewItem>().FirstOrDefault(o => (byte)o.Tag == currentId);

                    if (lvi != null)
                    {
                        lvi.SubItems.Clear();
                        setRow(lvi, oo);
                        lv.autoResizeColumns();
                    }
                }
            }

            private void onListViewClicked(ListView lv, int id)
            {
                refresh(lv);

                currentId = id;
                var o = data[id];

                tbName.Text = o.name;
                cbClimate.SelectedValue = o.climate;
            }
        }
    }

    public partial class UIEditGameWorldDatabaseWindow
    {
        private class TabPageCulture : TabPageBase
        {
            private Dictionary<int, Culture> data;
            private int? currentId;

            private TextBox tbName;

            public TabPageCulture(UIEditGameWorldDatabaseWindow bw, Dictionary<int, Culture> data) : base(bw)
            {
                this.data = data;

                this.init(w.culture.text).setAutoSizeP();

                var (sc, pl, lv) = createIndexControl(onAddButtonClicked, onDeleteButtonClicked);

                lv.addColumn(w.id)
                    .addColumn(w.name);

                lv.Click += (s, e) =>
                {
                    if (lv.FocusedItem == null) return;

                    onListViewClicked(lv, (byte)lv.FocusedItem.Tag);
                };

                var p2 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 2
                }.addRowStyle(80).addRowStyle(20).addTo(sc);

                var gb = new GroupBox()
                {
                    Dock = DockStyle.Fill,
                    Text = w.scene_edit_game_world.property
                }.addTo(p2);

                var p3 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 2,
                    AutoScroll = true
                }.addTo(gb);

                new Label().init(w.name + ":").setAutoSize().setRightCenter().addTo(p3);
                tbName = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init("").addTo(p3);

                initData(lv);

                lv.autoResizeColumns();
            }

            private void initData(ListView lv) => data.Values.ToList().ForEach(o => lv.Items.Add(setRow(new ListViewItem() { Tag = o.id }, o)));

            private void onAddButtonClicked(ListView lv)
            {
                var max = data.getMaxId(-1);

                if (++max > byte.MaxValue) return;

                var oo = new Culture()
                {
                    id = (byte)max,
                    name = w.culture.text,
                };

                data[oo.id] = oo;

                var lvi = new ListViewItem()
                {
                    Tag = oo.id
                };

                lv.Items.Add(setRow(lvi, oo));

                lv.autoResizeColumns();
            }

            private void onDeleteButtonClicked(ListView lv, int id)
            {
                lv.Items.Remove(lv.Items.Cast<ListViewItem>().First(o => (byte)o.Tag == id));

                data.Remove(id);

                if (currentId == id) currentId = null;

                lv.autoResizeColumns();
            }

            private ListViewItem setRow(ListViewItem lvi, Culture o)
            {
                lvi.Text = o.id.ToString();

                return lvi.addColumn(o.name);
            }

            private void refresh(ListView lv)
            {
                if (currentId != null)
                {
                    var oo = data[(byte)currentId];

                    oo.name = tbName.Text;

                    var lvi = lv.Items.Cast<ListViewItem>().FirstOrDefault(o => (byte)o.Tag == currentId);

                    if (lvi != null)
                    {
                        lvi.SubItems.Clear();
                        setRow(lvi, oo);
                        lv.autoResizeColumns();
                    }
                }
            }

            private void onListViewClicked(ListView lv, int id)
            {
                refresh(lv);

                currentId = id;
                var t = data[id];

                tbName.Text = t.name;
            }
        }
    }

    public partial class UIEditGameWorldDatabaseWindow
    {
        private class TabPageReligion : TabPageBase
        {
            private Dictionary<int, Religion> data;
            private int? currentId;

            private TextBox tbName;
            private CheckBox cbIsPolytheism;

            public TabPageReligion(UIEditGameWorldDatabaseWindow bw, Dictionary<int, Religion> data) : base(bw)
            {
                this.data = data;

                this.init(w.religion.text).setAutoSizeP();

                var (sc, pl, lv) = createIndexControl(onAddButtonClicked, onDeleteButtonClicked);

                lv.addColumn(w.id)
                    .addColumn(w.name)
                    .addColumn(w.religion.is_polytheism);

                lv.Click += (s, e) =>
                {
                    if (lv.FocusedItem == null) return;

                    onListViewClicked(lv, (byte)lv.FocusedItem.Tag);
                };

                var p2 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 2
                }.addRowStyle(80).addRowStyle(20).addTo(sc);

                var gb = new GroupBox()
                {
                    Dock = DockStyle.Fill,
                    Text = w.scene_edit_game_world.property
                }.addTo(p2);

                var p3 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 2,
                    AutoScroll = true
                }.addTo(gb);

                new Label().init(w.name + ":").setAutoSize().setRightCenter().addTo(p3);
                tbName = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.religion.is_polytheism + ":").setAutoSize().setRightCenter().addTo(p3);
                cbIsPolytheism = new CheckBox().init().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init("").addTo(p3);

                initData(lv);

                lv.autoResizeColumns();
            }

            private void initData(ListView lv) => data.Values.ToList().ForEach(o => lv.Items.Add(setRow(new ListViewItem() { Tag = o.id }, o)));

            private void onAddButtonClicked(ListView lv)
            {
                var max = data.getMaxId(-1);

                if (++max > byte.MaxValue) return;

                var oo = new Religion()
                {
                    id = (byte)max,
                    name = w.religion.text,
                };

                data[oo.id] = oo;

                var lvi = new ListViewItem()
                {
                    Tag = oo.id
                };

                lv.Items.Add(setRow(lvi, oo));

                lv.autoResizeColumns();
            }

            private void onDeleteButtonClicked(ListView lv, int id)
            {
                lv.Items.Remove(lv.Items.Cast<ListViewItem>().First(o => (byte)o.Tag == id));

                data.Remove(id);

                if (currentId == id) currentId = null;

                lv.autoResizeColumns();
            }

            private ListViewItem setRow(ListViewItem lvi, Religion o)
            {
                lvi.Text = o.id.ToString();

                return lvi.addColumn(o.name);
            }

            private void refresh(ListView lv)
            {
                if (currentId != null)
                {
                    var oo = data[(byte)currentId];

                    oo.name = tbName.Text;
                    oo.isPolytheism = cbIsPolytheism.Checked;

                    var lvi = lv.Items.Cast<ListViewItem>().FirstOrDefault(o => (byte)o.Tag == currentId);

                    if (lvi != null)
                    {
                        lvi.SubItems.Clear();
                        setRow(lvi, oo);
                        lv.autoResizeColumns();
                    }
                }
            }

            private void onListViewClicked(ListView lv, int id)
            {
                refresh(lv);

                currentId = id;
                var o = data[id];

                tbName.Text = o.name;
                cbIsPolytheism.Checked = o.isPolytheism;
            }
        }
    }

    public partial class UIEditGameWorldDatabaseWindow
    {
        private class TabPageRoad : TabPageBase
        {
            private Dictionary<int, Road> data;
            private int? currentId;

            private TextBox tbName;

            public TabPageRoad(UIEditGameWorldDatabaseWindow bw, Dictionary<int, Road> data) : base(bw)
            {
                this.data = data;

                this.init(w.road.text).setAutoSizeP();

                var (sc, pl, lv) = createIndexControl(onAddButtonClicked, onDeleteButtonClicked);

                lv.addColumn(w.id)
                    .addColumn(w.name);

                lv.Click += (s, e) =>
                {
                    if (lv.FocusedItem == null) return;

                    onListViewClicked(lv, (byte)lv.FocusedItem.Tag);
                };

                var p2 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 2
                }.addRowStyle(80).addRowStyle(20).addTo(sc);

                var gb = new GroupBox()
                {
                    Dock = DockStyle.Fill,
                    Text = w.scene_edit_game_world.property
                }.addTo(p2);

                var p3 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 2,
                    AutoScroll = true
                }.addTo(gb);

                new Label().init(w.name + ":").setAutoSize().setRightCenter().addTo(p3);
                tbName = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init("").addTo(p3);

                initData(lv);

                lv.autoResizeColumns();
            }

            private void initData(ListView lv) => data.Values.ToList().ForEach(o => lv.Items.Add(setRow(new ListViewItem() { Tag = o.id }, o)));

            private void onAddButtonClicked(ListView lv)
            {
                var max = data.getMaxId(0);

                if (++max > byte.MaxValue) return;

                var oo = new Road()
                {
                    id = (byte)max,
                    name = w.road.text,
                };

                data[oo.id] = oo;

                var lvi = new ListViewItem()
                {
                    Tag = oo.id
                };

                lv.Items.Add(setRow(lvi, oo));

                lv.autoResizeColumns();
            }

            private void onDeleteButtonClicked(ListView lv, int id)
            {
                lv.Items.Remove(lv.Items.Cast<ListViewItem>().First(o => (byte)o.Tag == id));

                data.Remove(id);

                if (currentId == id) currentId = null;

                lv.autoResizeColumns();
            }

            private ListViewItem setRow(ListViewItem lvi, Road o)
            {
                lvi.Text = o.id.ToString();

                return lvi.addColumn(o.name);
            }

            private void refresh(ListView lv)
            {
                if (currentId != null)
                {
                    var oo = data[(byte)currentId];

                    oo.name = tbName.Text;

                    var lvi = lv.Items.Cast<ListViewItem>().FirstOrDefault(o => (byte)o.Tag == currentId);

                    if (lvi != null)
                    {
                        lvi.SubItems.Clear();
                        setRow(lvi, oo);
                        lv.autoResizeColumns();
                    }
                }
            }

            private void onListViewClicked(ListView lv, int id)
            {
                refresh(lv);

                currentId = id;
                var o = data[id];

                tbName.Text = o.name;
            }
        }
    }

    public partial class UIEditGameWorldDatabaseWindow
    {
        private class TabPageStronghold : TabPageBase
        {
            private MasterData gameData;
            private Dictionary<int, Stronghold.Type> data;
            private int? currentId;

            private TextBox tbName;
            private TextBox tbCulture;
            private TextBox tbIntroduction;

            public TabPageStronghold(
                UIEditGameWorldDatabaseWindow bw, Dictionary<int, Stronghold.Type> data, MasterData gameData)
                : base(bw)
            {
                this.gameData = gameData;
                this.data = data;

                this.init(w.stronghold_type.text).setAutoSizeP();

                var (sc, pl, lv) = createIndexControl(onAddButtonClicked, onDeleteButtonClicked);

                lv.addColumn(w.id)
                    .addColumn(w.name)
                    .addColumn(w.culture.text);

                lv.Click += (s, e) =>
                {
                    if (lv.FocusedItem == null) return;

                    onListViewClicked(lv, (byte)lv.FocusedItem.Tag);
                };

                var p2 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 2
                }.addRowStyle(80).addTo(sc);

                var gb = new GroupBox()
                {
                    Dock = DockStyle.Fill,
                    Text = w.scene_edit_game_world.property
                }.addTo(p2);

                var p3 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 3,
                    AutoScroll = true
                }.addTo(gb);

                new Label().init(w.name + ":").setAutoSize().setRightCenter().addTo(p3);
                tbName = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.culture.text + ":").setAutoSize().setRightCenter().addTo(p3);
                tbCulture = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.introduction + ":").setAutoSize().setRightCenter().addTo(p3);
                tbIntroduction = new TextBox().refreshListViewOnClick(lv, refresh).setMultiLine().addTo(p3);

                new Label().init("").addTo(p3);

                initData(lv);

                lv.autoResizeColumns();
            }

            private void initData(ListView lv) => data.Values.ToList().ForEach(o => lv.Items.Add(setRow(new ListViewItem() { Tag = o.id }, o)));

            private void onAddButtonClicked(ListView lv)
            {
                var max = data.getMaxId(0);

                if (++max > byte.MaxValue) return;

                var oo = new Stronghold.Type()
                {
                    id = (byte)max,
                    name = w.stronghold_type.text,
                    introduction = string.Empty
                };

                data[oo.id] = oo;

                var lvi = new ListViewItem()
                {
                    Tag = oo.id
                };

                lv.Items.Add(setRow(lvi, oo));

                lv.autoResizeColumns();
            }

            private void onDeleteButtonClicked(ListView lv, int id)
            {
                lv.Items.Remove(lv.Items.Cast<ListViewItem>().First(o => (byte)o.Tag == id));

                data.Remove(id);

                if (currentId == id) currentId = null;

                lv.autoResizeColumns();
            }

            private ListViewItem setRow(ListViewItem lvi, Stronghold.Type o)
            {
                lvi.Text = o.id.ToString();

                return lvi.addColumn(o.name).addColumn(o.culture != null && gameData.culture.TryGetValue(o.culture.Value, out var c) ? $"{c.id}:{c.name}" : w.none);
            }

            private void refresh(ListView lv)
            {
                if (currentId != null)
                {
                    var oo = data[(byte)currentId];

                    oo.name = tbName.Text;
                    oo.culture = int.TryParse(tbCulture.Text, out var c) ? c : (int?)null;
                    oo.introduction = tbIntroduction.Text;

                    var lvi = lv.Items.Cast<ListViewItem>().FirstOrDefault(o => (byte)o.Tag == currentId);

                    if (lvi != null)
                    {
                        lvi.SubItems.Clear();

                        setRow(lvi, oo);

                        lv.autoResizeColumns();
                    }
                }
            }

            private void onListViewClicked(ListView lv, int id)
            {
                refresh(lv);

                currentId = id;
                var o = data[id];

                tbName.Text = o.name;
                tbCulture.Text = o.culture?.ToString() ?? string.Empty;
                tbIntroduction.Text = o.introduction;
            }
        }
    }

    public partial class UIEditGameWorldDatabaseWindow
    {
        private partial class TabPageMainMapTileImageInfo : TabPageBase
        {
            private Dictionary<int, MainTileMapImageInfo> data;
            private int? currentId;

            private TextBox tbName;
            private TextBox tbTileWidth;
            private TextBox tbTileHeight;

            private TabPageMainMapTileImageInfoTerrain tpTerrain;

            public TabPageMainMapTileImageInfo(
                UIEditGameWorldDatabaseWindow bw, Dictionary<int, MainTileMapImageInfo> data, MasterData gameData)
                : base(bw)
            {
                this.data = data;

                this.init(w.main_tile_map_image_info.text).setAutoSizeP();

                var (sc, pl, lv) = createIndexControl(onAddButtonClicked, onDeleteButtonClicked);

                lv.addColumn(w.id)
                    .addColumn(w.name)
                    .addColumn(w.tile_map_image_info.tile_size);

                lv.Click += (s, e) =>
                {
                    if (lv.FocusedItem == null) return;

                    onListViewClicked(lv, (int)lv.FocusedItem.Tag);
                };

                var p2 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 1,
                }.addColumnStyle(33).addColumnStyle(66).addTo(sc);

                var gb = new GroupBox()
                {
                    Dock = DockStyle.Fill,
                    Text = w.scene_edit_game_world.property
                }.addTo(p2);

                var p3 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 2,
                    AutoScroll = true
                }.addTo(gb);

                new Label().init(w.name + ":").setAutoSize().setRightCenter().addTo(p3);
                tbName = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.tile_map_image_info.tile_width + ":").setAutoSize().setRightCenter().addTo(p3);
                tbTileWidth = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.tile_map_image_info.tile_height + ":").setAutoSize().setRightCenter().addTo(p3);
                tbTileHeight = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init("").addTo(p3);

                gb = new GroupBox()
                {
                    Dock = DockStyle.Fill,
                    Text = w.detail
                }.addTo(p2);

                var tc = new TabControl() { Dock = DockStyle.Fill }.init().setAutoSize().addTo(gb);

                tpTerrain = new TabPageMainMapTileImageInfoTerrain(bw, lv).addTo(tc);

                initData(lv);

                lv.autoResizeColumns();
            }

            private void initData(ListView lv) => data.Values.ToList().ForEach(o => lv.Items.Add(setRow(new ListViewItem() { Tag = o.id }, o)));

            private void onAddButtonClicked(ListView lv)
            {
                var max = data.getMaxId(0);

                if (++max > int.MaxValue) return;

                var oo = new MainTileMapImageInfo()
                {
                    id = max,
                    name = w.tile_map_image_info.text,
                    terrainAnimation = new Dictionary<byte, TileAnimation>(),
                    strongholdAnimation = new Dictionary<int, TileAnimation>()
                };

                data[oo.id] = oo;

                var lvi = new ListViewItem()
                {
                    Tag = oo.id
                };

                lv.Items.Add(setRow(lvi, oo));

                lv.autoResizeColumns();
            }

            private void onDeleteButtonClicked(ListView lv, int id)
            {
                lv.Items.Remove(lv.Items.Cast<ListViewItem>().First(o => (int)o.Tag == id));

                data.Remove(id);

                if (currentId == id) currentId = null;

                lv.autoResizeColumns();
            }

            private ListViewItem setRow(ListViewItem lvi, MainTileMapImageInfo o)
            {
                lvi.Text = o.id.ToString();

                lvi.addColumn(o.name);
                lvi.addColumn($"({o.tileSize.Width},{o.tileSize.Height})");

                return lvi;
            }

            private void refresh(ListView lv)
            {
                if (currentId != null)
                {
                    var oo = data[(int)currentId];

                    oo.name = tbName.Text;
                    if (int.TryParse(tbTileWidth.Text, out var width)) oo.tileSize.Width = width;
                    if (int.TryParse(tbTileHeight.Text, out var height)) oo.tileSize.Height = height;

                    var lvi = lv.Items.Cast<ListViewItem>().FirstOrDefault(o => (int)o.Tag == currentId);

                    if (lvi != null)
                    {
                        lvi.SubItems.Clear();

                        setRow(lvi, oo);

                        lv.autoResizeColumns();
                    }
                }
            }

            private void onListViewClicked(ListView lv, int id)
            {
                refresh(lv);

                currentId = id;
                var o = data[id];

                tbName.Text = o.name;
                tbTileWidth.Text = o.tileSize.Width.ToString();
                tbTileHeight.Text = o.tileSize.Height.ToString();

                tpTerrain.setData(o);

                //tpTerrain.setData(o);
                //tpTerrain.refreshTerrain();
            }
        }
    }

    public partial class UIEditGameWorldDatabaseWindow
    {
        private partial class TabPageMainMapTileImageInfo : TabPageBase
        {
            private class TabPageMainMapTileImageInfoTerrain : TabPageBase
            {
                private MainTileMapImageInfo data;

                private Label lbStatus;
                private TextBox tbContent;

                public TabPageMainMapTileImageInfoTerrain(UIEditGameWorldDatabaseWindow bw, ListView lv) : base(bw)
                {
                    this.init(w.terrain.text).setAutoSizeP();

                    var tlp = new TableLayoutPanel()
                    {
                        RowCount = 2,
                        ColumnCount = 2,
                        Dock = DockStyle.Fill
                    }.addColumnStyle(50).addColumnStyle(50).addTo(this);

                    lbStatus = new Label().init(string.Empty).setAutoSize().setLeftCenter().addTo(tlp);

                    tlp.SetColumnSpan(lbStatus, 2);

                    tbContent = new TextBox()
                    {
                        Dock = DockStyle.Fill,
                        Multiline = true,
                        AcceptsReturn = true,
                        ScrollBars = ScrollBars.Both
                    }.refreshListViewOnClick(lv, refresh).addTo(tlp);

                    new TextBox()
                    {
                        Dock = DockStyle.Fill,
                        ReadOnly = true,
                        Multiline = true,
                        ScrollBars = ScrollBars.Horizontal,
                        Text = new Dictionary<int, TileAnimation>()
                        {
                            { 0, new TileAnimation()
                            {
                                id = 0,
                                interval = 0.5f,
                                frames = new List<TileAnimation.Frame>()
                                {
                                    new TileAnimation.Frame()
                                    {
                                        fileName = "path/file_name.png",
                                        vertex = new System.Drawing.Point()
                                        {
                                            X = 0,
                                            Y = 0
                                        }
                                    }
                                }
                            }
                            }
                        }.toJson(true)
                    }.addTo(tlp);
                }

                public void setData(MainTileMapImageInfo data)
                {
                    this.data = data;

                    tbContent.Text = data.terrainAnimation.toJson(true);
                }

                private void refresh(ListView lv)
                {
                    if (data == null) return;

                    if (lv.FocusedItem == null) return;

                    try
                    {
                        var json = tbContent.Text.fromJson<Dictionary<byte, TileAnimation>>();

                        foreach (var o in json.Values)
                        {
                            if (o.frames == null) o.frames = new List<TileAnimation.Frame>();
                        }

                        data.terrainAnimation = json;

                        lbStatus.Text = w.tile_map_image_info.edit_success;
                    }
                    catch (Exception e)
                    {
                        lbStatus.Text = $"{w.tile_map_image_info.edit_failure}{e.Message.Substring(e.Message.IndexOf(':'))}";
                    }
                }
            }
        }
    }

    public partial class UIEditGameWorldDatabaseWindow
    {
        private partial class TabPageDetailMapTileImageInfo : TabPageBase
        {
            private Dictionary<int, DetailTileMapImageInfo> data;
            private int? currentId;

            private TextBox tbName;
            private TextBox tbTileWidth;
            private TextBox tbTileHeight;

            private TabPageDetailMapTileImageInfoTerrain tpTerrain;

            public TabPageDetailMapTileImageInfo(
                UIEditGameWorldDatabaseWindow bw, Dictionary<int, DetailTileMapImageInfo> data, MasterData gameData)
                : base(bw)
            {
                this.data = data;

                this.init(w.detail_tile_map_image_info.text).setAutoSizeP();

                var (sc, pl, lv) = createIndexControl(onAddButtonClicked, onDeleteButtonClicked);

                lv.addColumn(w.id)
                    .addColumn(w.name)
                    .addColumn(w.tile_map_image_info.tile_size);

                lv.Click += (s, e) =>
                {
                    if (lv.FocusedItem == null) return;

                    onListViewClicked(lv, (int)lv.FocusedItem.Tag);
                };

                var p2 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 1,
                }.addColumnStyle(33).addColumnStyle(66).addTo(sc);

                var gb = new GroupBox()
                {
                    Dock = DockStyle.Fill,
                    Text = w.scene_edit_game_world.property
                }.addTo(p2);

                var p3 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 2,
                    AutoScroll = true
                }.addTo(gb);

                new Label().init(w.name + ":").setAutoSize().setRightCenter().addTo(p3);
                tbName = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.tile_map_image_info.tile_width + ":").setAutoSize().setRightCenter().addTo(p3);
                tbTileWidth = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.tile_map_image_info.tile_height + ":").setAutoSize().setRightCenter().addTo(p3);
                tbTileHeight = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init("").addTo(p3);

                gb = new GroupBox()
                {
                    Dock = DockStyle.Fill,
                    Text = w.detail
                }.addTo(p2);

                var tc = new TabControl() { Dock = DockStyle.Fill }.init().setAutoSize().addTo(gb);

                tpTerrain = new TabPageDetailMapTileImageInfoTerrain(bw, lv).addTo(tc);

                initData(lv);

                lv.autoResizeColumns();
            }

            private void initData(ListView lv) => data.Values.ToList().ForEach(o => lv.Items.Add(setRow(new ListViewItem() { Tag = o.id }, o)));

            private void onAddButtonClicked(ListView lv)
            {
                var max = data.getMaxId(0);

                if (++max > int.MaxValue) return;

                var oo = new DetailTileMapImageInfo()
                {
                    id = max,
                    name = w.tile_map_image_info.text,
                    terrainAnimation = new Dictionary<byte, TileAnimation>(),
                };

                data[oo.id] = oo;

                var lvi = new ListViewItem()
                {
                    Tag = oo.id
                };

                lv.Items.Add(setRow(lvi, oo));

                lv.autoResizeColumns();
            }

            private void onDeleteButtonClicked(ListView lv, int id)
            {
                lv.Items.Remove(lv.Items.Cast<ListViewItem>().First(o => (int)o.Tag == id));

                data.Remove(id);

                if (currentId == id) currentId = null;

                lv.autoResizeColumns();
            }

            private ListViewItem setRow(ListViewItem lvi, DetailTileMapImageInfo o)
            {
                lvi.Text = o.id.ToString();

                lvi.addColumn(o.name);
                lvi.addColumn($"({o.tileSize.Width},{o.tileSize.Height})");

                return lvi;
            }

            private void refresh(ListView lv)
            {
                if (currentId != null)
                {
                    var oo = data[(int)currentId];

                    oo.name = tbName.Text;
                    if (int.TryParse(tbTileWidth.Text, out var width)) oo.tileSize.Width = width;
                    if (int.TryParse(tbTileHeight.Text, out var height)) oo.tileSize.Height = height;

                    var lvi = lv.Items.Cast<ListViewItem>().FirstOrDefault(o => (int)o.Tag == currentId);

                    if (lvi != null)
                    {
                        lvi.SubItems.Clear();

                        setRow(lvi, oo);

                        lv.autoResizeColumns();
                    }
                }
            }

            private void onListViewClicked(ListView lv, int id)
            {
                refresh(lv);

                currentId = id;
                var o = data[id];

                tbName.Text = o.name;
                tbTileWidth.Text = o.tileSize.Width.ToString();
                tbTileHeight.Text = o.tileSize.Height.ToString();

                tpTerrain.setData(o);

                //tpTerrain.setData(o);
                //tpTerrain.refreshTerrain();
            }
        }
    }

    public partial class UIEditGameWorldDatabaseWindow
    {
        private partial class TabPageDetailMapTileImageInfo : TabPageBase
        {
            private class TabPageDetailMapTileImageInfoTerrain : TabPageBase
            {
                private DetailTileMapImageInfo data;

                private Label lbStatus;
                private TextBox tbContent;

                public TabPageDetailMapTileImageInfoTerrain(UIEditGameWorldDatabaseWindow bw, ListView lv) : base(bw)
                {
                    this.init(w.terrain.text).setAutoSizeP();

                    var tlp = new TableLayoutPanel()
                    {
                        RowCount = 2,
                        ColumnCount = 2,
                        Dock = DockStyle.Fill
                    }.addColumnStyle(50).addColumnStyle(50).addTo(this);

                    lbStatus = new Label().init(string.Empty).setAutoSize().setLeftCenter().addTo(tlp);

                    tlp.SetColumnSpan(lbStatus, 2);

                    tbContent = new TextBox()
                    {
                        Dock = DockStyle.Fill,
                        Multiline = true,
                        AcceptsReturn = true,
                        ScrollBars = ScrollBars.Both
                    }.refreshListViewOnClick(lv, refresh).addTo(tlp);

                    new TextBox()
                    {
                        Dock = DockStyle.Fill,
                        ReadOnly = true,
                        Multiline = true,
                        ScrollBars = ScrollBars.Horizontal,
                        Text = new Dictionary<int, TileAnimation>()
                        {
                            { 0, new TileAnimation()
                            {
                                id = 0,
                                interval = 0.5f,
                                frames = new List<TileAnimation.Frame>()
                                {
                                    new TileAnimation.Frame()
                                    {
                                        fileName = "path/file_name.png",
                                        vertex = new System.Drawing.Point()
                                        {
                                            X = 0,
                                            Y = 0
                                        }
                                    }
                                }
                            }
                            }
                        }.toJson(true)
                    }.addTo(tlp);
                }

                public void setData(DetailTileMapImageInfo data)
                {
                    this.data = data;

                    tbContent.Text = data.terrainAnimation.toJson(true);
                }

                private void refresh(ListView lv)
                {
                    if (data == null) return;

                    if (lv.FocusedItem == null) return;

                    try
                    {
                        var json = tbContent.Text.fromJson<Dictionary<byte, TileAnimation>>();

                        foreach (var o in json.Values)
                        {
                            if (o.frames == null) o.frames = new List<TileAnimation.Frame>();
                        }

                        data.terrainAnimation = json;

                        lbStatus.Text = w.tile_map_image_info.edit_success;
                    }
                    catch (Exception e)
                    {
                        lbStatus.Text = $"{w.tile_map_image_info.edit_failure}{e.Message.Substring(e.Message.IndexOf(':'))}";
                    }
                }
            }
        }
    }

    public partial class UIEditGameWorldDatabaseWindow
    {
        private partial class TabPageDetailTileMapInfo : TabPageBase
        {
            private Dictionary<int, DetailTileMapInfo> data;
            private Dictionary<int, Terrain> terrain;
            private int? currentId;

            private TextBox tbName;
            private TextBox tbMapWidth;
            private TextBox tbMapHeight;
            private ComboBox cbTerrain;

            public TabPageDetailTileMapInfo(
                UIEditGameWorldDatabaseWindow bw, Dictionary<int, DetailTileMapInfo> data, Dictionary<int, Terrain> terrain)
                : base(bw)
            {
                this.data = data;
                this.terrain = terrain;

                this.init(w.detail_tile_map).setAutoSizeP();

                bw.tabControl.SelectedIndexChanged += (s, e) =>
                {
                    if (bw.tabControl.SelectedTab == this) refresh();
                };

                var (sc, pl, lv) = createIndexControl(onAddButtonClicked, onDeleteButtonClicked);

                lv.addColumn(w.id)
                    .addColumn(w.name)
                    .addColumn(w.width)
                    .addColumn(w.height)
                    .addColumn(w.terrain.text);

                lv.Click += (s, e) =>
                {
                    if (lv.FocusedItem == null) return;

                    onListViewClicked(lv, (int)lv.FocusedItem.Tag);
                };

                var p2 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 2
                }.addRowStyle(80).addRowStyle(20).addTo(sc);

                var gb = new GroupBox()
                {
                    Dock = DockStyle.Fill,
                    Text = w.scene_edit_game_world.property
                }.addTo(p2);

                var p3 = new TableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 2,
                    AutoScroll = true
                }.addTo(gb);

                new Label().init(w.name + ":").setAutoSize().setRightCenter().addTo(p3);
                tbName = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.width + ":").setAutoSize().setRightCenter().addTo(p3);
                tbMapWidth = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.height + ":").setAutoSize().setRightCenter().addTo(p3);
                tbMapHeight = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.terrain.text + ":").setAutoSize().setRightCenter().addTo(p3);
                cbTerrain = new ComboBox().initDropDownList<int>().addTo(p3);

                new Label().init("").addTo(p3);

                initData(lv);

                lv.autoResizeColumns();

                refresh();
            }

            private void initData(ListView lv) => data.Values.ToList().ForEach(o => lv.Items.Add(setRow(new ListViewItem() { Tag = o.id }, o)));

            private void refresh() => cbTerrain.setDropDownList(terrain.Select(o => new KeyValuePair<int, string>(o.Key, o.Value.name)).ToList());

            private void onAddButtonClicked(ListView lv)
            {
                var max = data.getMaxId(0);

                if (++max > int.MaxValue) return;

                var oo = new DetailTileMapInfo()
                {
                    id = max,
                    name = w.detail_tile_map,
                    size = new TileMap.Size(100, 100),
                };

                data[oo.id] = oo;

                var lvi = new ListViewItem()
                {
                    Tag = oo.id
                };

                lv.Items.Add(setRow(lvi, oo));

                lv.autoResizeColumns();
            }

            private void onDeleteButtonClicked(ListView lv, int id)
            {
                lv.Items.Remove(lv.Items.Cast<ListViewItem>().First(o => (int)o.Tag == id));

                data.Remove(id);

                if (currentId == id) currentId = null;

                lv.autoResizeColumns();
            }

            private ListViewItem setRow(ListViewItem lvi, DetailTileMapInfo o)
            {
                lvi.Text = o.id.ToString();

                lvi.addColumn(o.name);
                lvi.addColumn(o.size.column.ToString());
                lvi.addColumn(o.size.row.ToString());
                lvi.addColumn(terrain.TryGetValue(o.terrainId, out var t) ? t.name : w.symbol_unselected);

                return lvi;
            }

            private void refresh(ListView lv)
            {
                if (currentId != null)
                {
                    var oo = data[(int)currentId];

                    oo.name = tbName.Text;
                    oo.size.column = int.TryParse(tbMapWidth.Text, out var width) ? width : 100;
                    oo.size.row = int.TryParse(tbMapHeight.Text, out var height) ? height : 100;
                    if (cbTerrain.SelectedValue != null) oo.terrainId = (int)cbTerrain.SelectedValue;

                    var lvi = lv.Items.Cast<ListViewItem>().FirstOrDefault(o => (int)o.Tag == currentId);

                    if (lvi != null)
                    {
                        lvi.SubItems.Clear();

                        setRow(lvi, oo);

                        lv.autoResizeColumns();
                    }
                }
            }

            private void onListViewClicked(ListView lv, int id)
            {
                refresh(lv);

                currentId = id;
                var o = data[id];

                tbName.Text = o.name;
                tbMapWidth.Text = o.size.column.ToString();
                tbMapHeight.Text = o.size.row.ToString();
                cbTerrain.SelectedValue = o.terrainId;
            }
        }
    }

    #region 图形化编辑界面模板、状态基本完成、因为结构麻烦而废弃
    //public partial class UIEditGameWorldDatabaseWindow
    //{
    //    private partial class TabPageMainMapTileImageInfo : TabPageBase
    //    {
    //        private class TabPageMainMapTileImageInfoTerrain : TabPageBase
    //        {
    //            private MainTileMapImageInfo data;
    //            private Dictionary<int, Terrain> terrain;

    //            private TextBox tbInterval;
    //            private ListView lvTerrain;
    //            private ListView lvFrame;

    //            public TabPageMainMapTileImageInfoTerrain(UIEditGameWorldDatabaseWindow bw, Dictionary<int, Terrain> terrain) : base(bw)
    //            {
    //                this.terrain = terrain;

    //                this.init(w.terrain.name).setAutoSizeP();

    //                var sc = new TableLayoutPanel()
    //                {
    //                    RowCount = 1,
    //                    ColumnCount = 3,
    //                    Dock = DockStyle.Fill
    //                }.addColumnStyle(25).addColumnStyle(75).addTo(this);

    //                var gb = new GroupBox()
    //                {
    //                    Dock = DockStyle.Fill,
    //                    Text = w.type
    //                }.addTo(sc);

    //                lvTerrain = new ListView().init().addColumn(w.name).addTo(gb);

    //                gb = new GroupBox()
    //                {
    //                    Dock = DockStyle.Fill,
    //                    Text = w.tile_map_image_info.frame
    //                }.addTo(sc);

    //                var mtlp = new TableLayoutPanel()
    //                {
    //                    RowCount = 2,
    //                    ColumnCount = 2,
    //                    Dock = DockStyle.Fill
    //                }.addRowStyle(80).addRowStyle(20).addColumnStyle(50).addColumnStyle(50).addTo(gb);

    //                lvFrame = new ListView().init().addColumn(w.name).addTo(mtlp);

    //                mtlp.SetColumnSpan(lvFrame, 2);

    //                new Button().init("+", () =>
    //                {
    //                    if (data == null) return;

    //                    if (lvTerrain.FocusedItem == null) return;

    //                    var id = (byte)lvTerrain.FocusedItem.Tag;

    //                    var frame = new TileAnimation.Frame();

    //                    data.terrainAnimation[id].frames.Add(frame);

    //                    lvFrame.Items.Add(new ListViewItem()
    //                    {
    //                        Tag = frame
    //                    });
    //                }).addTo(mtlp);

    //                new Button().init("-", () =>
    //                {
    //                    if (lvTerrain.FocusedItem == null) return;

    //                    if (lvFrame.FocusedItem == null) return;

    //                    var id = (byte)lvTerrain.FocusedItem.Tag;

    //                    var frame = lvFrame.FocusedItem.Tag as TileAnimation.Frame;

    //                    data.terrainAnimation[id].frames.Remove(frame);

    //                    lvFrame.Items.Remove(lvFrame.FocusedItem);
    //                }).addTo(mtlp);

    //                gb = new GroupBox()
    //                {
    //                    Dock = DockStyle.Fill,
    //                    Text = w.scene_edit_game_world.property
    //                }.addTo(sc);

    //                var rtlp = new TableLayoutPanel()
    //                {
    //                    RowCount = 3,
    //                    ColumnCount = 2,
    //                    Dock = DockStyle.Fill
    //                }.addTo(gb);

    //                new Label().init(w.tile_map_image_info.interval + ":").setAutoSize().setRightCenter().addTo(rtlp);
    //                tbInterval = new TextBox()/*.refreshListViewOnClick(lvTerrain, refresh)*/.addTo(rtlp);

    //                new Label().init(w.tile_map_image_info.file_name).setAutoSize().setRightCenter().addTo(rtlp);
    //                var tbFileName = new TextBox().addTo(rtlp);

    //                new Label().init(w.tile_map_image_info.position).setAutoSize().setRightCenter().addTo(rtlp);
    //                var tbVertex = new TextBox().addTo(rtlp);

    //                lvTerrain.SelectedIndexChanged += (s, e) =>
    //                {
    //                    lvFrame.Items.Clear();
    //                    tbInterval.Text = string.Empty;
    //                    tbFileName.Text = string.Empty;
    //                    tbVertex.Text = string.Empty;

    //                    if (lvTerrain.FocusedItem == null) return;

    //                    var id = (byte)lvTerrain.FocusedItem.Tag;

    //                    if (!data.terrainAnimation.TryGetValue(id, out var value)) return;

    //                    tbInterval.Text = value.interval.ToString();

    //                    refreshFrame();
    //                };

    //                lvFrame.SelectedIndexChanged += (s, e) =>
    //                {
    //                    if (lvFrame.FocusedItem == null) return;

    //                    var data = lvFrame.FocusedItem.Tag as TileAnimation.Frame;

    //                    tbFileName.Text = data.fileName;
    //                    tbVertex.Text = $"{data.vertex.X},{data.vertex.Y}";

    //                    // select image
    //                };

    //                lvTerrain.autoResizeColumns(-5);
    //                lvFrame.autoResizeColumns(-5);
    //            }

    //            public void refreshTerrain()
    //            {
    //                lvTerrain.Items.Clear();

    //                lvTerrain.Items.AddRange(terrain.Values.ToList().Select(o => new ListViewItem()
    //                {
    //                    Tag = o.id,
    //                    Text = o.name
    //                }).ToArray());

    //                lvTerrain.autoResizeColumns(-5);
    //            }

    //            public void refreshFrame()
    //            {
    //                lvFrame.Items.Clear();

    //                if (lvTerrain.FocusedItem == null) return;

    //                var terrainId = (byte)lvTerrain.FocusedItem.Tag;

    //                lvFrame.Items.AddRange(data.terrainAnimation[terrainId].frames.ToList().Select(o => new ListViewItem()
    //                {
    //                    Tag = o,
    //                    Text = $"{o.vertex.X},{o.vertex.Y}"
    //                }).ToArray());

    //                lvFrame.autoResizeColumns(-5);
    //            }

    //            public void setData(MainTileMapImageInfo data) => this.data = data;
    //        }
    //    }
    //}
    #endregion
}
