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
    public partial class NavigationMenuItem : UserControl
    {
        [Browsable(true)]
        [Category("Action")]        
        public EventHandler ButtonClick;
        public NavigationMenuItem()
        {
            InitializeComponent();
        }
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Title
        {
            get
            {
                return button1.Text;
            }
            set
            {
                button1.Text = value;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ButtonClick?.Invoke(this, e);
        }
    }
}
