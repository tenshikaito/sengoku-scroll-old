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
            gameWorldMasterData = gw.masterData.toJson().fromJson<MasterData>();

            MaximizeBox = false;

            this.init(w.scene_edit_game_world.database).setCenter();

            var tc = tabControl = new TabControl() { MinimumSize = new System.Drawing.Size(720, 480) }.init().setAutoSize().addTo(panel);

            new TabPageTerrain(this, gameWorldMasterData.mainTileMapTerrain, gameWorldMasterData.terrainImage).addTo(tc);

            new TabPageRegion(this, gameWorldMasterData.region).addTo(tc);

            new TabPageCulture(this, gameWorldMasterData.culture).addTo(tc);

            new TabPageReligion(this, gameWorldMasterData.religion).addTo(tc);

            new TabPageRoad(this, gameWorldMasterData.road).addTo(tc);

            new TabPageStronghold(this, gameWorldMasterData.strongholdType, gameWorldMasterData).addTo(tc);

            //new TabPageDetailTileMapInfo(this, gameWorldMasterData.detailTileMapInfo, gameWorldMasterData.detailTileMapTerrain).addTo(tc);

            new TabPageTerrainImage(this, gameWorldMasterData.terrainImage, gameWorldMasterData.mainTileMapTerrain).addTo(tc);

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

                    btnDeleteClick(lv, (int)lv.FocusedItem.Tag);
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
            private Dictionary<int, TerrainImage> terrainImage;
            private int? currentId;

            private TextBox tbName;
            private ComboBox cbImage;
            private CheckBox cbIsGrass;
            private CheckBox cbIsHill;
            private CheckBox cbIsMountain;
            private CheckBox cbIsHabour;
            private CheckBox cbIsWater;
            private CheckBox cbIsFreshWater;
            private CheckBox cbIsDeepWater;

            public TabPageTerrain(UIEditGameWorldDatabaseWindow bw, Dictionary<int, Terrain> data, Dictionary<int, TerrainImage> terrainImage) : base(bw)
            {
                this.data = data;
                this.terrainImage = terrainImage;

                this.init(w.terrain.text).setAutoSizeP();

                bw.tabControl.SelectedIndexChanged += (s, e) =>
                {
                    if (bw.tabControl.SelectedTab == this) refresh();
                };

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
                    AutoScroll = true
                }.addTo(gb);

                new Label().init(w.name + ":").setAutoSize().setRightCenter().addTo(p3);
                tbName = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.terrain.text + ":").setAutoSize().setRightCenter().addTo(p3);
                cbImage = new ComboBox().initDropDownList<int>().addTo(p3);

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

                refresh();
            }

            private void initData(ListView lv) => data.Values.ToList().ForEach(o => lv.Items.Add(setRow(new ListViewItem() { Tag = o.id }, o)));

            private void refresh() => cbImage.setDropDownList(terrainImage.Select(o => new KeyValuePair<int, string>(o.Key, o.Value.name)).ToList());

            private void onAddButtonClicked(ListView lv)
            {
                var max = data.getMaxId(-1);

                if (++max > int.MaxValue) return;

                var oo = new Terrain()
                {
                    id = (int)max,
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
                lv.Items.Remove(lv.Items.Cast<ListViewItem>().First(o => (int)o.Tag == id));

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

                    if (cbImage.SelectedValue != null) oo.imageId = (int)cbImage.SelectedValue;

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
                cbImage.SelectedValue = o.imageId;
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

                if (++max > int.MaxValue) return;

                var oo = new Region()
                {
                    id = (int)max,
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
                lv.Items.Remove(lv.Items.Cast<ListViewItem>().First(o => (int)o.Tag == id));

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

                new Label().init("").addTo(p3);

                initData(lv);

                lv.autoResizeColumns();
            }

            private void initData(ListView lv) => data.Values.ToList().ForEach(o => lv.Items.Add(setRow(new ListViewItem() { Tag = o.id }, o)));

            private void onAddButtonClicked(ListView lv)
            {
                var max = data.getMaxId(-1);

                if (++max > int.MaxValue) return;

                var oo = new Culture()
                {
                    id = max,
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
                lv.Items.Remove(lv.Items.Cast<ListViewItem>().First(o => (int)o.Tag == id));

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
                    var oo = data[(int)currentId];

                    oo.name = tbName.Text;

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

                if (++max > int.MaxValue) return;

                var oo = new Religion()
                {
                    id = max,
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
                lv.Items.Remove(lv.Items.Cast<ListViewItem>().First(o => (int)o.Tag == id));

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
                    var oo = data[(int)currentId];

                    oo.name = tbName.Text;
                    oo.isPolytheism = cbIsPolytheism.Checked;

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

                new Label().init("").addTo(p3);

                initData(lv);

                lv.autoResizeColumns();
            }

            private void initData(ListView lv) => data.Values.ToList().ForEach(o => lv.Items.Add(setRow(new ListViewItem() { Tag = o.id }, o)));

            private void onAddButtonClicked(ListView lv)
            {
                var max = data.getMaxId(0);

                if (++max > int.MaxValue) return;

                var oo = new Road()
                {
                    id = (int)max,
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
                lv.Items.Remove(lv.Items.Cast<ListViewItem>().First(o => (int)o.Tag == id));

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
                    var oo = data[(int)currentId];

                    oo.name = tbName.Text;

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

                    onListViewClicked(lv, (int)lv.FocusedItem.Tag);
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

                if (++max > int.MaxValue) return;

                var oo = new Stronghold.Type()
                {
                    id = max,
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
                lv.Items.Remove(lv.Items.Cast<ListViewItem>().First(o => (int)o.Tag == id));

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
                    var oo = data[(int)currentId];

                    oo.name = tbName.Text;
                    oo.culture = int.TryParse(tbCulture.Text, out var c) ? c : (int?)null;
                    oo.introduction = tbIntroduction.Text;

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
                tbCulture.Text = o.culture?.ToString() ?? string.Empty;
                tbIntroduction.Text = o.introduction;
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

    public partial class UIEditGameWorldDatabaseWindow
    {
        private partial class TabPageTerrainImage : TabPageBase
        {
            private Dictionary<int, TerrainImage> data;
            private Dictionary<int, Terrain> terrain;
            private int? currentId;

            private TextBox tbName;
            private TextBox tbAnimationViewSpring;
            private TextBox tbAnimationViewSummer;
            private TextBox tbAnimationViewAutumn;
            private TextBox tbAnimationViewWinter;
            private TextBox tbAnimationDetailSpring;
            private TextBox tbAnimationDetailSummer;
            private TextBox tbAnimationDetailAutumn;
            private TextBox tbAnimationDetailWinter;

            public TabPageTerrainImage(
                UIEditGameWorldDatabaseWindow bw, Dictionary<int, TerrainImage> data, Dictionary<int, Terrain> terrain)
                : base(bw)
            {
                this.data = data;
                this.terrain = terrain;

                this.init(w.terrain_image.text).setAutoSizeP();

                var (sc, pl, lv) = createIndexControl(onAddButtonClicked, onDeleteButtonClicked);

                lv.addColumn(w.id)
                    .addColumn(w.name)
                    .addColumn(w.terrain_image.text);

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

                var width = 320;
                var height = 48;

                new Label().init(w.name + ":").setAutoSize().setRightCenter().addTo(p3);
                tbName = new TextBox().refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.terrain_image.animation_view_spring + ":").setAutoSize().setRightCenter().addTo(p3);
                tbAnimationViewSpring = new TextBox() { Multiline = true, Width = width, Height = height }.refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.terrain_image.animation_view_summer + ":").setAutoSize().setRightCenter().addTo(p3);
                tbAnimationViewSummer = new TextBox() { Multiline = true, Width = width, Height = height }.refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.terrain_image.animation_view_autumn + ":").setAutoSize().setRightCenter().addTo(p3);
                tbAnimationViewAutumn = new TextBox() { Multiline = true, Width = width, Height = height }.refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.terrain_image.animation_view_winter + ":").setAutoSize().setRightCenter().addTo(p3);
                tbAnimationViewWinter = new TextBox() { Multiline = true, Width = width, Height = height }.refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.terrain_image.animation_detail_spring + ":").setAutoSize().setRightCenter().addTo(p3);
                tbAnimationDetailSpring = new TextBox() { Multiline = true, Width = width, Height = height }.refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.terrain_image.animation_detail_summer + ":").setAutoSize().setRightCenter().addTo(p3);
                tbAnimationDetailSummer = new TextBox() { Multiline = true, Width = width, Height = height }.refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.terrain_image.animation_detail_autumn + ":").setAutoSize().setRightCenter().addTo(p3);
                tbAnimationDetailAutumn = new TextBox() { Multiline = true, Width = width, Height = height }.refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init(w.terrain_image.animation_detail_winter + ":").setAutoSize().setRightCenter().addTo(p3);
                tbAnimationDetailWinter = new TextBox() { Multiline = true, Width = width, Height = height }.refreshListViewOnClick(lv, refresh).addTo(p3);

                new Label().init("").addTo(p3);

                initData(lv);

                lv.autoResizeColumns();
            }

            private void initData(ListView lv) => data.Values.ToList().ForEach(o => lv.Items.Add(setRow(new ListViewItem() { Tag = o.id }, o)));

            private void onAddButtonClicked(ListView lv)
            {
                var max = data.getMaxId(0);

                if (++max > int.MaxValue) return;

                var oo = new TerrainImage()
                {
                    id = max,
                    name = w.terrain_image.text,
                    animationView = new List<TileAnimationFrame>(),
                    animationViewSpring = new List<TileAnimationFrame>(),
                    animationViewSummer = new List<TileAnimationFrame>(),
                    animationViewAutumn = new List<TileAnimationFrame>(),
                    animationViewWinter = new List<TileAnimationFrame>(),
                    animationViewSnow = new List<TileAnimationFrame>(),
                    animationDetail = new List<TileAnimationFrame>(),
                    animationDetailSpring = new List<TileAnimationFrame>(),
                    animationDetailSummer = new List<TileAnimationFrame>(),
                    animationDetailAutumn = new List<TileAnimationFrame>(),
                    animationDetailWinter = new List<TileAnimationFrame>(),
                    animationDetailSnow = new List<TileAnimationFrame>()
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

            private ListViewItem setRow(ListViewItem lvi, TerrainImage o)
            {
                lvi.Text = o.id.ToString();

                lvi.addColumn(o.name);

                return lvi;
            }

            private void refresh(ListView lv)
            {
                if (currentId != null)
                {
                    var oo = data[(int)currentId];

                    oo.name = tbName.Text;

                    var showDialog = new Action<string>(text =>
                    {
                        new UIDialog(window.gameSystem, "alert", text).ShowDialog();
                    });

                    try
                    {
                        oo.animationViewSpring = tbAnimationViewSpring.Text.toAnimationFrameList();
                    }
                    catch
                    {
                        showDialog(w.terrain_image.animation_view_spring);
                    }

                    try
                    {
                        oo.animationViewSummer = tbAnimationViewSummer.Text.toAnimationFrameList();
                    }
                    catch
                    {
                        showDialog(w.terrain_image.animation_view_summer);
                    }

                    try
                    {
                        oo.animationViewAutumn = tbAnimationViewAutumn.Text.toAnimationFrameList();
                    }
                    catch
                    {
                        showDialog(w.terrain_image.animation_view_autumn);
                    }

                    try
                    {
                        oo.animationViewWinter = tbAnimationViewWinter.Text.toAnimationFrameList();
                    }
                    catch
                    {
                        showDialog(w.terrain_image.animation_view_winter);
                    }

                    try
                    {
                        oo.animationDetailSpring = tbAnimationDetailSpring.Text.toAnimationFrameList();
                    }
                    catch
                    {
                        showDialog(w.terrain_image.animation_detail_spring);
                    }

                    try
                    {
                        oo.animationDetailSummer = tbAnimationDetailSummer.Text.toAnimationFrameList();
                    }
                    catch
                    {
                        showDialog(w.terrain_image.animation_detail_summer);
                    }

                    try
                    {
                        oo.animationDetailAutumn = tbAnimationDetailAutumn.Text.toAnimationFrameList();
                    }
                    catch
                    {
                        showDialog(w.terrain_image.animation_detail_autumn);
                    }

                    try
                    {
                        oo.animationDetailWinter = tbAnimationDetailWinter.Text.toAnimationFrameList();
                    }
                    catch
                    {
                        showDialog(w.terrain_image.animation_detail_winter);
                    }

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
                tbAnimationViewSpring.Text = o.animationViewSpring.toString();
                tbAnimationViewSummer.Text = o.animationViewSummer.toString();
                tbAnimationViewAutumn.Text = o.animationViewAutumn.toString();
                tbAnimationViewWinter.Text = o.animationViewWinter.toString();
                tbAnimationDetailSpring.Text = o.animationDetailSpring.toString();
                tbAnimationDetailSummer.Text = o.animationDetailSummer.toString();
                tbAnimationDetailAutumn.Text = o.animationDetailAutumn.toString();
                tbAnimationDetailWinter.Text = o.animationDetailWinter.toString();
            }
        }
    }
}
