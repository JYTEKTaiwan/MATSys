using MATSys.Automation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using WeifenLuo.WinFormsUI.Docking;

namespace AutomationTestUI.Forms
{
    public partial class MainFormView : SubForm
    {
        private WinformViewConfiguration config;
        private SubForm f = null;
        private TestPackage tp;
        private bool isLoaded = false;
        public MainFormView()
        {
            InitializeComponent();
            config = JsonNode.Parse(File.ReadAllText("appsettings.json"))[WinformViewConfiguration.Key].Deserialize<WinformViewConfiguration>();
            vS2015LightTheme1.ColorPalette.MainWindowActive.Background = Color.AliceBlue;
        }
        private void OpenView()
        {
            switch (config.DefaultView)
            {
                case "Editor":
                    editorModeToolStripMenuItem_Click(null, null);
                    break;
                default:
                    break;
            }
        }
        private void editorModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = PackageExplorer.Form;
            f.ServiceProvider = this.ServiceProvider;
            f.Open(dockPanel1, DockState.DockLeft);
        }

        private void executionModeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void analyToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tp = base.ServiceProvider.GetService<TestPackage>();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var path = openFileDialog1.FileName;
                tp.LoadFromXML(path);
                Parent.Text = path;
                isLoaded = true;
            }
        }

        private void MainFormView_Load(object sender, EventArgs e)
        {
            OpenView();

        }

        private void itemEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isLoaded) return;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var file = saveFileDialog1.FileName;

                tp.Export(file);
            }
        }

        private void packageExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = PackageExplorer.Form;
            f.ServiceProvider = this.ServiceProvider;
            f.Open(dockPanel1, DockState.DockLeft);
        }

        private void createNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tp = base.ServiceProvider.GetService<TestPackage>();
            tp = new TestPackage();
            var f = PackageExplorer.Form;
            f.ServiceProvider = this.ServiceProvider;
            f.LoadTestPackage(tp);
            f.Open(dockPanel1, DockState.DockLeft);

        }
    }

    internal struct WinformViewConfiguration
    {
        public const string Key = "Winform";
        public string DefaultView { get; set; }
    }
    public class SubForm : DockContent
    {

        public ToolStrip ToolStrip { get; protected set; }

        public ServiceProvider ServiceProvider { get;set; }

        public SubForm Open(DockPanel dp, DockState ds)
        {
            this.Show(dp, ds);
            return this;
        }

        public SubForm Open(DockPane dp, DockAlignment da, double proportion)
        {
            this.Show(dp, da,proportion);
            return this;

        }
    }
}
