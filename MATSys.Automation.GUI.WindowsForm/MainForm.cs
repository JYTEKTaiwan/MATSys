using MATSys.Automation.GUI.WindowsForm.Layout;
using System.Windows.Forms;

namespace MATSys.Automation.GUI.WindowsForm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            PerformMainLayout();
        }


        private void PerformMainLayout()
        {
            var layout = new MainLayout();
            layout.Dock= DockStyle.Fill;
            layout.TopLevel = false;
            layout.Show();
            this.Controls.Add(layout);


        }
    }
}
