using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace MATSys.Automation.GUI.WindowsForm.Pages
{
    public partial class PropertyPage : DockContent
    {
        private TestPackage tp;
        private IServiceProvider sp;

        public PropertyPage()
        {
            InitializeComponent();
            numericUpDown1.Controls[0].Visible = numericUpDown2.Controls[0].Visible = false;
            sp = Services.ServiceHub.Instance.Value.ServiceProvider;
            tp = sp.GetRequiredService<TestPackage>();
            tp.TestItemSelectionChanged += Tp_TestItemSelectionChanged;
        }

        private void Tp_TestItemSelectionChanged(object? sender, TestItem e)
        {
            Update(e);

        }

        private void Update(TestItem ti)
        {
            label1.Text = ti.Name;
            numericUpDown1.Value = ti.Order;
            checkBox1.Checked = ti.Skip;
            numericUpDown2.Value = ti.Retry;
            textBox1.Text = ti.Arguments;
            textBox2.Text = ti.Description;

            DataTable dt = new DataTable();
            dt.Columns.Add("Key", typeof(string));
            dt.Columns.Add("Value", typeof(object));
            ti.Worker.Parameters.ToList()
                .ForEach(kvp => dt.Rows.Add(new object[] { kvp.Key, kvp.Value }));
            dataGridView1.DataSource = dt;

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Key", typeof(string));
            dt2.Columns.Add("Value", typeof(object));
            ti.Worker.Conditions.ToList()
                .ForEach(kvp => dt2.Rows.Add(new object[] { kvp.Key, kvp.Value }));
            dataGridView2.DataSource = dt2;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
