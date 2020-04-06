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

namespace Client.UI
{
    public partial class UIEditGameWorldDatabaseWindow : UISaveableConfirmDialog
    {
        public GameWorldMasterData gameWorldMasterData;

        public UIEditGameWorldDatabaseWindow(GameSystem gs, GameWorld gw) : base(gs)
        {
            gameWorldMasterData = gw.gameWorldMasterData;

            MaximizeBox = false;

            this.init(w.scene_edit_game_world.database).setCenter();

            var tc = new TabControl() { MinimumSize = new System.Drawing.Size(720, 480) }.init().setAutoSize().addTo(panel);

            new TabPageTerrain(this, gameWorldMasterData.terrain).addTo(tc);

            new TabPageRegion(this, gameWorldMasterData.region).addTo(tc);

            new TabPageCulture(this, gameWorldMasterData.culture).addTo(tc);

            new TabPageReligion(this, gameWorldMasterData.religion).addTo(tc);

            new TabPageRoad(this, gameWorldMasterData.road).addTo(tc);

            new TabPageStronghold(this, gameWorldMasterData.strongholdType).addTo(tc);

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
                }.addTo(this);

                sc.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
                sc.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

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
                }.addTo(gb);

                pl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80));
                pl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));

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

                this.init(w.terrain.name).setAutoSizeP();

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
                }.addTo(sc);

                p2.RowStyles.Add(new RowStyle(SizeType.Percent, 80));
                p2.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

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
                var max = data.Any() ? data.Max(o => o.Key) : -1;

                if (++max > byte.MaxValue) return;

                var oo = new Terrain()
                {
                    id = (byte)max,
                    name = w.terrain.name,
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

                this.init(w.region.name).setAutoSizeP();

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
                }.addTo(sc);

                p2.RowStyles.Add(new RowStyle(SizeType.Percent, 80));
                p2.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

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
                var max = data.Any() ? data.Max(o => o.Key) : -1;

                if (++max > byte.MaxValue) return;

                var oo = new Region()
                {
                    id = (byte)max,
                    name = w.region.name,
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

                this.init(w.culture.name).setAutoSizeP();

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
                }.addTo(sc);

                p2.RowStyles.Add(new RowStyle(SizeType.Percent, 80));
                p2.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

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
                var max = data.Any() ? data.Max(o => o.Key) : -1;

                if (++max > byte.MaxValue) return;

                var oo = new Culture()
                {
                    id = (byte)max,
                    name =w.culture.name,
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

                this.init(w.religion.name).setAutoSizeP();

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
                }.addTo(sc);

                p2.RowStyles.Add(new RowStyle(SizeType.Percent, 80));
                p2.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

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
                var max = data.Any() ? data.Max(o => o.Key) : -1;

                if (++max > byte.MaxValue) return;

                var oo = new Religion()
                {
                    id = (byte)max,
                    name = w.religion.name,
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

                this.init(w.road.name).setAutoSizeP();

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
                }.addTo(sc);

                p2.RowStyles.Add(new RowStyle(SizeType.Percent, 80));
                p2.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

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
                var max = data.Any() ? data.Max(o => o.Key) : 0;

                if (++max > byte.MaxValue) return;

                var oo = new Road()
                {
                    id = (byte)max,
                    name = w.road.name,
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
            private Dictionary<int, Stronghold.Type> data;
            private int? currentId;

            private TextBox tbName;
            private TextBox tbIntroduction;

            public TabPageStronghold(UIEditGameWorldDatabaseWindow bw, Dictionary<int, Stronghold.Type> data) : base(bw)
            {
                this.data = data;

                this.init(w.stronghold_type.name).setAutoSizeP();

                var (sc, pl, lv) = createIndexControl(onAddButtonClicked, onDeleteButtonClicked);

                lv.addColumn(w.id)
                    .addColumn(w.name)
                    .addColumn(w.code);

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
                }.addTo(sc);

                p2.RowStyles.Add(new RowStyle(SizeType.Percent, 80));

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

                new Label().init(w.introduction + ":").setAutoSize().setRightCenter().addTo(p3);
                tbIntroduction = new TextBox().refreshListViewOnClick(lv, refresh).setMultiLine().addTo(p3);

                new Label().init("").addTo(p3);

                initData(lv);

                lv.autoResizeColumns();
            }

            private void initData(ListView lv) => data.Values.ToList().ForEach(o => lv.Items.Add(setRow(new ListViewItem() { Tag = o.id }, o)));

            private void onAddButtonClicked(ListView lv)
            {
                var max = data.Any() ? data.Max(o => o.Key) : 0;

                if (++max > byte.MaxValue) return;

                var oo = new Stronghold.Type()
                {
                    id = (byte)max,
                    name = w.stronghold_type.name,
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

                return lvi.addColumn(o.name);
            }

            private void refresh(ListView lv)
            {
                if (currentId != null)
                {
                    var oo = data[(byte)currentId];

                    oo.name = tbName.Text;
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
                tbIntroduction.Text = o.introduction;
            }
        }
    }

    public partial class UIEditGameWorldDatabaseWindow
    {
        private class TabPageOuterMapTileImageInfo : TabPageBase
        {
            private Dictionary<int, OuterTileMapImageInfo> data;
            private int? currentId;

            private TextBox tbName;
            private TextBox tbTileWidth;
            private TextBox tbTileHeight;

            public TabPageOuterMapTileImageInfo(UIEditGameWorldDatabaseWindow bw, Dictionary<int, OuterTileMapImageInfo> data) : base(bw)
            {
                this.data = data;

                this.init(w.stronghold_type.name).setAutoSizeP();

                var (sc, pl, lv) = createIndexControl(onAddButtonClicked, onDeleteButtonClicked);

                lv.addColumn(w.id)
                    .addColumn(w.name)
                    .addColumn(w.tile_map_image_info.tile_size);

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
                }.addTo(sc);

                p2.RowStyles.Add(new RowStyle(SizeType.Percent, 80));

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
                var max = data.Any() ? data.Max(o => o.Key) : 0;

                if (++max > byte.MaxValue) return;

                var oo = new OuterTileMapImageInfo()
                {
                    id = (byte)max,
                    name = w.tile_map_image_info.name,
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

            private ListViewItem setRow(ListViewItem lvi, OuterTileMapImageInfo o)
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
}
