using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MATSys.Automation.GUI.WindowsForm.Controls
{
    public partial class NavigationMenu : UserControl
    {


        private Dictionary<string, NavigationMenuItem> dict = new Dictionary<string, NavigationMenuItem>();
        public NavigationMenu()
        {
            InitializeComponent();
            foreach (var item in flowLayoutPanel1.Controls)
            {
                var ctrl = (NavigationMenuItem)item;
                ctrl.ButtonClick += ItemSelected;
                dict.Add(ctrl.Title, ctrl);
            }
        }

        private void ItemSelected(object sender, EventArgs e)
        {
            var item = (NavigationMenuItem)sender;
            var idx=dict.Keys.ToList().IndexOf(item.Title);

            MessageBox.Show(idx.ToString());

        }
    }
}
