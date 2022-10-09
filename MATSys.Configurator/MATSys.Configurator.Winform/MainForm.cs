using MATSys.Configurator.Core;
using System.Text.Json.Nodes;

namespace MATSys.Configurator.Winform
{
    public partial class MainForm : Form
    {
        private AssemblyLoader? _loader;
        private const string title_prefix = "MATSys Configurator";
        private string binDirectory = "";
        private IEnumerable<MATSysInformation>? _modules;
        private IEnumerable<MATSysInformation>? _plugins;
        private IEnumerable<MATSysInformation>? _allLibs;

        public MainForm(string rootPath)
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(rootPath) && Directory.Exists(rootPath))
            {
                LoadBinDirectory(rootPath);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void MainForm_Shown(object sender, EventArgs e)
        {
            tsStatLabel.Text = $"Path is not assigned yet";
        }
        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            treeView1.Nodes.Clear();

            if (e.IsSelected && e.Item.Group.Name == "Modules")
            {
                var cmds = _loader.ShowSupportedCommands(e.Item.SubItems[0].Text);
                foreach (var item in cmds)
                {
                    var node = treeView1.Nodes.Add(item.Split("=")[0]);
                    node.ToolTipText = item;
                    foreach (var param in item.Split("=")[1].Split(","))
                    {
                        node.Nodes.Add(param);
                    }
                }
            }
        }
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadBinDirectory(folderBrowserDialog1.SelectedPath);
            }

        }

        private void LoadBinDirectory(string rootDirectory)
        {
            listView1.Items.Clear();
            listView1.Groups.Clear();
            _modules = null;
            _plugins = null;
            _allLibs = null;

            binDirectory = rootDirectory;
            tsStatLabel.Text = $"Folder is assigned: {binDirectory}";
            _loader = new AssemblyLoader(binDirectory);
            _modules = _loader.ListAllModules();
            _plugins = _loader.ListAllPlugins();
            _allLibs = _modules.Concat(_plugins);

            InitializeListView();
            InitializeDataGridView();

        }
        private void InitializeListView()
        {
            var grp = listView1.Groups.Add("Modules", "Modules");
            foreach (var item in _modules)
            {
                var owner = listView1.Items.Add(item.Type);
                owner.Group = grp;
                owner.ToolTipText = Path.Combine(binDirectory, item.AssemblyPath);
                var col = new ListViewItem.ListViewSubItemCollection(owner);
                col.Add(item.Alias);
                col.Add(item.AssemblyPath);

            }
            grp = listView1.Groups.Add("Plugins", "Plugins");
            foreach (var item in _plugins)
            {

                var owner = listView1.Items.Add(item.Type);
                owner.Group = grp;
                owner.ToolTipText = Path.Combine(binDirectory, item.AssemblyPath);
                var col = new ListViewItem.ListViewSubItemCollection(owner);
                col.Add(item.Alias);
                col.Add(item.AssemblyPath);

            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

        }
        private void InitializeDataGridView()
        {
            #region Clear contents
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            #endregion

            #region Setup Appearance
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AllowDrop = true;
            dgv.AllowUserToAddRows = false;

            #endregion

            #region Configure Context Menu Strip
            removeCurrentItemToolStripMenuItem.Click += (sender, e) => { };

            dgv.ContextMenuStrip = ctxtMS_dgv;

            #endregion

            #region Configure Columns

            var col = new DataGridViewTextBoxColumn();
            col.HeaderText = "Alias";
            dgv.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "Module";
            col.ReadOnly = true;
            dgv.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "Transceiver";
            col.ReadOnly = true;
            dgv.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "Notifier";
            col.ReadOnly = true;
            dgv.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "Recorder";
            col.ReadOnly = true;
            dgv.Columns.Add(col);

            #endregion

            #region Configure DataGridView DragDrop Event
            dgv.DragDrop += (sender, e) =>
            {
                var data = e.Data.GetData(typeof(MATSysInformation)) as MATSysInformation;
                var newP = dgv.PointToClient(new Point(e.X, e.Y));
                DataGridView.HitTestInfo hit = dgv.HitTest(newP.X, newP.Y);
                var rowIdx = hit.RowIndex;
                var colIdx = hit.ColumnIndex;
                if (rowIdx == -1)
                {
                    if (data.Category == "Module")
                    {
                        var item1 = _allLibs.FirstOrDefault(x => x.Type == "EmptyTransceiver");
                        var item2 = _allLibs.FirstOrDefault(x => x.Type == "EmptyNotifier");
                        var item3 = _allLibs.FirstOrDefault(x => x.Type == "EmptyRecorder");
                        dgv.Rows.Add(null, data, item1, item2, item3);
                    }
                }
                else
                {
                    dgv.Rows[rowIdx].Cells[colIdx].Value = data;
                }

            };
            dgv.DragOver += (sender, e) =>
            {


                var data = e.Data.GetData(typeof(MATSysInformation)) as MATSysInformation;

                var newP = dgv.PointToClient(new Point(e.X, e.Y));
                DataGridView.HitTestInfo hit = dgv.HitTest(newP.X, newP.Y);
                if (hit.RowIndex >= 0)
                {
                    bool condition =
                   (data.Category == "Module" && hit.ColumnIndex == 1)
                   || (data.Category == "Transceiver" && hit.ColumnIndex == 2)
                   || (data.Category == "Notifier" && hit.ColumnIndex == 3)
                   || (data.Category == "Recorder" && hit.ColumnIndex == 4);
                    if (condition)
                    {
                        e.Effect = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                }
                else
                {
                    if (data.Category == "Module")
                    {
                        e.Effect = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }

                }



            };
            #endregion

            #region Configure Cell Click Event
            dgv.CellClick += (sender, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 1)
                {
                    var value = (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                    if (value != null)
                    {
                        propertyGrid1.SelectedObject = (value as MATSysInformation).Setting;
                    }
                    else
                    {
                        propertyGrid1.SelectedObject = new object();
                    }
                }


            };

            #endregion
        }
        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var item = _allLibs.FirstOrDefault(x => x.Type == (e.Item as ListViewItem).SubItems[0].Text);
            listView1.DoDragDrop(item, DragDropEffects.Move);

        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        private void removeCurrentItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pos = dgv.PointToClient(MousePosition);
            DataGridView.HitTestInfo hit = dgv.HitTest(pos.X, pos.Y);
            if (hit.RowIndex >= 0)
            {
                dgv.Rows.RemoveAt(hit.RowIndex);
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new ExportDialog(_allLibs);
            if (f.ShowDialog() == DialogResult.OK)
            {
                List<ExportingDataType> list = new List<ExportingDataType>();
                foreach (DataGridViewRow item in dgv.Rows)
                {
                    list.Add(new ExportingDataType(
                        item.Cells[0].Value as string,
                        item.Cells[1].Value as MATSysInformation,
                        item.Cells[2].Value as MATSysInformation,
                        item.Cells[3].Value as MATSysInformation,
                        item.Cells[4].Value as MATSysInformation
                        ));
                }
                var nodes = ExportUtility.ExportToJsonNode(list);

                if (f.EnableNLog)
                {
                    (nodes["MATSys"] as JsonObject).Add("EnableNLogInJsonFile", f.EnableNLog);
                    (nodes.Root as JsonObject).Add("NLog", f.Node_Nlog);
                }
                if (f.EnableTransceiver && f.Node_Transceiver != null)
                {
                    (nodes["MATSys"] as JsonObject).Add("Transceiver", f.Node_Transceiver);
                }
                if (f.EnableScript)
                {
                    (nodes["MATSys"] as JsonObject).Add("Scripts", f.Node_Script);
                }
                ExportUtility.SaveToFile(nodes, binDirectory);



            }
        }
    }
}