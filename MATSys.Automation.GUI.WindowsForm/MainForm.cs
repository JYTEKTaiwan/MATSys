using System.Windows.Forms;
using MATSys.Automation.GUI.WindowsForm.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace MATSys.Automation.GUI.WindowsForm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();         
            
            ConfigureServices();
            PerformDefaultLayout();
        }

        private void PerformDefaultLayout()
        {
            
            var content_editor = new ScriptEditorPage();
            content_editor.CloseButtonVisible = false;  
            content_editor.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
            
            //var content_property = new PropertyPage();
            //content_property.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
            //new PropertyPage().Show(content_property.Pane, WeifenLuo.WinFormsUI.Docking.DockAlignment.Bottom,0.5);


        }

        private void ConfigureServices()
        {
            var services=Services.ServiceHub.Instance;
        }

    }
}
