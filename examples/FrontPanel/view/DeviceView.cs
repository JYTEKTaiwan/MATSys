using MATSys.FrontPanel.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MATSys.FrontPanel.View
{
    public partial class DeviceView : Form
    {
        private readonly DeviceViewModel _vm;
        public DeviceView()
        {
            InitializeComponent();
            _vm = new DeviceViewModel();
        }
    }
}
