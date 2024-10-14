using MATSys.Plugins;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using WeifenLuo.WinFormsUI.Docking;
using MATSys.Automation;
using System.Xml.Linq;

namespace AutomationTestUI.Forms
{
    public partial class PackageExplorer : SubForm
    {
        private static bool isShown = false;
        private static PackageExplorer page;
        private BindingSource bs = new BindingSource();
        private ContextMenuStrip ctxtMs_plugins = new ContextMenuStrip();
        private ContextMenuStrip ctxtMs_scipts = new ContextMenuStrip();
        private Rectangle dragBoxFromMouseDown;
        private int dropSourceIndex;
        private int dropDestinationIndex;

        private PackageExplorer()
        {
            InitializeComponent();
            DecorateTreeView();
            DecorateContextMenuStrip();
            PluginEditor.PluginChanged += contentChanged;
            TestItemEditor.TestItemChanged += contentChanged;

        }

        private void contentChanged(object? sender, EventArgs e)
        {
            var tp = this.ServiceProvider.GetService<TestPackage>();
            UpdateTreeView(tp);
            
        }
        public static PackageExplorer Form
        {
            get
            {
                if (page == null || page.IsDisposed)
                {
                    page = new PackageExplorer();
                }
                return page;
            }
        }


        public void LoadTestPackage(TestPackage tp)
        {
            var package = ServiceProvider.GetService<TestPackage>();
            package.Items = tp.Items;
            package.Plugins = tp.Plugins;
        }
        private void DecorateTreeView()
        {
            trv.ShowNodeToolTips = true;
            trv.Font = new Font(trv.Font.FontFamily, 12);
            trv.NodeMouseDoubleClick += Trv_NodeMouseDoubleClick;
            trv.NodeMouseClick += Trv_NodeMouseClick;
            trv.AfterSelect += Trv_AfterSelect;

        }

        private void Trv_AfterSelect(object? sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 2 && e.Node.Parent.Text.Contains("Plugins"))
            {
                var tp = ServiceProvider.GetService<TestPackage>();
                var pi = tp.Plugins[e.Node.Index];
                var f = PluginEditor.Form as PluginEditor;
                f.ServiceProvider = this.ServiceProvider;
                f.Load(pi);
                f.Open(this.Pane, DockAlignment.Bottom, 0.45);
                return;
            }
            if (e.Node.Level == 2 && e.Node.Parent.Text.Contains("TestScripts"))
            {
                var tp = ServiceProvider.GetService<TestPackage>();
                var ti = tp.Items[e.Node.Index];
                var f = TestItemEditor.Form as TestItemEditor;
                f.ServiceProvider = this.ServiceProvider;
                f.Load(ti);
                f.Open(this.DockPanel, DockState.Document);
                return;
            }

        }

        private void DecorateContextMenuStrip()
        {
            ctxtMs_plugins.Items.Clear();
            var tsMenuItem_addPlugin = new ToolStripMenuItem();
            tsMenuItem_addPlugin.Text = "Add Plugin";
            tsMenuItem_addPlugin.Click += AddPlugin;
            ctxtMs_plugins.Items.Add(tsMenuItem_addPlugin);
            var tsMenuItem_removePlugin = new ToolStripMenuItem();
            tsMenuItem_removePlugin.Text = "Remove Plugin";
            tsMenuItem_removePlugin.Click += RemovePlugin;
            ctxtMs_plugins.Items.Add(tsMenuItem_removePlugin);

            ctxtMs_scipts.Items.Clear();
            var tsMenuItem_addItem = new ToolStripMenuItem();
            tsMenuItem_addItem.Text = "Add Test Item";
            tsMenuItem_addItem.Click += AddTestItem;
            ctxtMs_scipts.Items.Add(tsMenuItem_addItem);
            var tsMenuItem_removeItem = new ToolStripMenuItem();
            tsMenuItem_removeItem.Text = "Remove Test Item";
            tsMenuItem_removeItem.Click += RemoveTestItem;
            ctxtMs_scipts.Items.Add(tsMenuItem_removeItem);

        }

        private void RemoveTestItem(object? sender, EventArgs e)
        {
            var tp = this.ServiceProvider.GetService<TestPackage>();
            tp.Items.RemoveAt(trv.SelectedNode.Index);
            contentChanged(null, null);
        }

        private void AddTestItem(object? sender, EventArgs e)
        {
            var tp = this.ServiceProvider.GetService<TestPackage>();
            var newItem = new TestItem();
            newItem.Name = newItem.GetHashCode().ToString();
            var idx = -1;
            if (trv.SelectedNode.Level == 2)
            {
                idx = trv.SelectedNode.Index;
            }
            else
            {
                //root node, add to the last
                idx = trv.SelectedNode.Nodes.Count;
            }

            tp.Items.Insert(idx, newItem);
            contentChanged(null, null);
            var node = trv.Nodes["TestPackage"].Nodes["TestScripts"].Nodes[newItem.GetHashCode().ToString()];
            trv.SelectedNode = node;
        }

        private void RemovePlugin(object? sender, EventArgs e)
        {
            var tp = this.ServiceProvider.GetService<TestPackage>();
            tp.Plugins.RemoveAt(trv.SelectedNode.Index);
            contentChanged(null, null);
        }

        private void AddPlugin(object? sender, EventArgs e)
        {
            var tp = this.ServiceProvider.GetService<TestPackage>();
            var newItem = new PluginInfo();
            newItem.Alias = newItem.GetHashCode().ToString();
            var idx = -1;
            if (trv.SelectedNode.Level == 2)
            {
                idx = trv.SelectedNode.Index;
            }
            else
            {
                //root node, add to the last
                idx = trv.SelectedNode.Nodes.Count;
            }
            tp.Plugins.Insert(idx, newItem);
            contentChanged(null, null);
            var node = trv.Nodes["TestPackage"].Nodes["Plugins"].Nodes[newItem.GetHashCode().ToString()];
            trv.SelectedNode = node;

        }

        private void Trv_NodeMouseClick(object? sender, TreeNodeMouseClickEventArgs e)
        {
            if (trv.SelectedNode == null) return;
            else if (e.Button == MouseButtons.Right && trv.SelectedNode.FullPath == e.Node.FullPath)
            {

                if (e.Node.Level == 2 && e.Node.Parent.Text.Contains("Plugins"))
                {
                    ctxtMs_plugins.Items[1].Enabled = true;
                    ctxtMs_plugins.Show(trv, new System.Drawing.Point(e.X, e.Y));
                }
                else if (e.Node.Level == 1 && e.Node.Text.Contains("Plugins"))
                {
                    ctxtMs_plugins.Items[1].Enabled = false;
                    ctxtMs_plugins.Show(trv, new System.Drawing.Point(e.X, e.Y));
                }
                else if (e.Node.Level == 2 && e.Node.Parent.Text.Contains("Scripts"))
                {
                    ctxtMs_scipts.Items[1].Enabled = true;
                    ctxtMs_scipts.Show(trv, new System.Drawing.Point(e.X, e.Y));
                }
                else if (e.Node.Level == 1 && e.Node.Text.Contains("Scripts"))
                {
                    ctxtMs_scipts.Items[1].Enabled = false;
                    ctxtMs_scipts.Show(trv, new System.Drawing.Point(e.X, e.Y));
                }
            }
        }

        private void Trv_NodeMouseDoubleClick(object? sender, TreeNodeMouseClickEventArgs e)
        {


        }

        private void PackgeExplorer_Load(object sender, EventArgs e)
        {
            var tp = ServiceProvider.GetService<TestPackage>();
            UpdateTreeView(tp);
            tp.FileLoaded += (sender, e) =>
            {
                var package = (TestPackage)sender;
                UpdateTreeView(tp);
            };

        }

        private void UpdateTreeView(TestPackage tp)
        {
            trv.Nodes.Clear();
            var font = trv.Font;
            var root = trv.Nodes.Add("TestPackage", "TestPackage");
            var pluginNodes = root.Nodes.Add("Plugins", $"Plugins ({tp.Plugins.Count})");
            foreach (PluginInfo pi in tp.Plugins)
            {
                var str = string.IsNullOrEmpty(pi.QualifiedName) ? "" : pi.QualifiedName.Split(',')[0];
                var node = pluginNodes.Nodes.Add(pi.GetHashCode().ToString(), $"[{pi.Alias}] {str}");
                node.ToolTipText = pi.Path;
                node.Tag = pi;
            }
            var itemNodes = root.Nodes.Add("TestScripts", $"TestScripts ({tp.Items.Where(x => !x.Skip).Count()})");
            foreach (TestItem ti in tp.Items)
            {
                var node = itemNodes.Nodes.Add(ti.GetHashCode().ToString(), $"[{ti.Order}] {ti.Name}");
                UpdateIfSkip(node, ti.Skip);

                node.ToolTipText = $"Skip:{ti.Skip}; Retry:{ti.Retry}";
                node.Tag = ti;
            }
            trv.ExpandAll();

        }

        private void UpdateIfSkip(TreeNode node, bool skip)
        {
            var color = skip ? Color.Gray : SystemColors.WindowText;
            var font = skip ? new Font(trv.Font, FontStyle.Strikeout) : trv.Font;

            node.ForeColor = color;
            node.NodeFont = font;

        }
        private void ItemEditorPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            isShown = false;
        }

        private void ItemEditorPage_Shown(object sender, EventArgs e)
        {
            isShown = true;
        }

        private void trv_DragDrop(object sender, DragEventArgs e)
        {
            var tp = ServiceProvider.GetService<TestPackage>();

            // Retrieve the client coordinates of the drop location.
            Point targetPoint = trv.PointToClient(new Point(e.X, e.Y));

            // Retrieve the node at the drop location.
            TreeNode targetNode = trv.GetNodeAt(targetPoint);

            // Retrieve the node that was dragged.
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

            // Cannot drop to another group
            if (draggedNode.Parent != targetNode.Parent) return;
            // Cannot drop to the different level
            if (draggedNode.Level != targetNode.Level) return;


            if (targetNode.Parent.Text.Contains("Plugins"))
            {

                tp.Plugins.Swap(targetNode.Index, draggedNode.Index);
            }
            else
            {
                tp.Items.Swap(targetNode.Index, draggedNode.Index);

            }


            contentChanged(null, null);

        }

        private void trv_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var node = (TreeNode)e.Item;
            // Only level-2 item can be dragged
            if (node.Level != 2) return;

            DoDragDrop(e.Item, DragDropEffects.Move);

        }

        private void trv_DragEnter(object sender, DragEventArgs e)
        {
            
            e.Effect = DragDropEffects.Move;
        }
    }
}
