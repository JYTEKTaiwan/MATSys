using MATSys.FrontPanel.View;
using System.Windows.Forms;

namespace MATSys.FrontPanel
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void toolStripButton_hardware_Click(object sender, System.EventArgs e)
        {
            var mainPanel = toolStripContainer1.ContentPanel;
            mainPanel.Controls.Clear();
            var f = new DeviceView();
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            f.Show();
            mainPanel.Controls.Add(f);
        }
    }
}
