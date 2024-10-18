using AutomationTestUI.Dialogs;
using MATSys;
using MATSys.Automation;
using MATSys.Commands;
using MATSys.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;


namespace AutomationTestUI.Forms
{
    public partial class TestItemEditor : SubForm
    {
        private static TestItemEditor page;
        private Dictionary<string, string> dict = new Dictionary<string, string>();
        private BindingSource bs_param = new BindingSource();
        public static event EventHandler TestItemChanged;
        public static TestItemEditor Form
        {
            get
            {
                if (page == null || page.IsDisposed)
                {
                    page = new TestItemEditor();
                }
                return page;
            }
        }

        public TestItem Item { get; private set; }

        private TestItemEditor()
        {
            InitializeComponent();
        }
        public void Load(TestItem ti)
        {
            Item = ti;
            comboBox_plugID.SelectedIndexChanged -= PluginChanged;
            comboBox_method.SelectedIndexChanged -= MethodChanged;
            dgv_cond.CellValueChanged -= dgv_cond_CellValueChanged;
            dgv_param.CellValueChanged-= dgv_param_CellValueChanged;
            BindToUI();
            comboBox_plugID.SelectedIndexChanged += PluginChanged;
            comboBox_method.SelectedIndexChanged += MethodChanged;
            dgv_cond.CellValueChanged += dgv_cond_CellValueChanged;
            dgv_param.CellValueChanged += dgv_param_CellValueChanged;
            this.ActiveControl = null;

        }

        private void dgv_cond_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            JsonObject jobj = new JsonObject();
            foreach (DataGridViewRow row in dgv_cond.Rows)
            {
                var v = JsonValue.Create(row.Cells[1].Value);
                jobj.Add(row.Cells[0].Value.ToString(), v);
            }
            textBox_cond.Text = jobj.ToJsonString();
        }

        private void dgv_param_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            
            JsonObject jobj = new JsonObject();
            foreach (DataGridViewRow row in dgv_param.Rows)
            {
                var v = JsonValue.Create(row.Cells[1].Value);
                jobj.Add(row.Cells[0].Value.ToString(),v);
            }
            textBox_param.Text = jobj.ToJsonString();
        }

        private void MethodChanged(object? sender, EventArgs e)
        {
            var tp = ServiceProvider.GetService<TestPackage>();
            var selectedPlugin = tp.Plugins.First(x => x.Alias == comboBox_plugID.Text);
            var assem = Assembly.LoadFrom(selectedPlugin.Path);
            var type = assem.GetTypes().First(x => x.AssemblyQualifiedName == selectedPlugin.QualifiedName);
            var method = type.GetMethods().Where(x => x.GetCustomAttributes<MATSysCommandAttribute>().Any()).First(x => x.Name == comboBox_method.Text);

            var param = method.GetParameters();
            dgv_param.Rows.Clear();
            foreach (var item in param[0].ParameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                var idx = dgv_param.Rows.Add(item.Name, null);
                dgv_param.Rows[idx].Cells[1].ValueType = item.PropertyType;
            }
            dgv_cond.Rows.Clear();
            foreach (var item in param[1].ParameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                var idx = dgv_cond.Rows.Add(item.Name, null);
                dgv_cond.Rows[idx].Cells[1].ValueType = item.PropertyType;
            }
            textBox_param.Text = textBox_cond.Text = "";
        }

        private void PluginChanged(object? sender, EventArgs e)
        {
            var tp = ServiceProvider.GetService<TestPackage>();
            var selectedPlugin = tp.Plugins.First(x => x.Alias == comboBox_plugID.Text);
            var assem = Assembly.LoadFrom(selectedPlugin.Path);
            var type = assem.GetTypes().First(x => x.AssemblyQualifiedName == selectedPlugin.QualifiedName);
            var methods = type.GetMethods().Where(x => x.GetCustomAttributes<MATSysCommandAttribute>().Any()).Select(x => x.Name).ToArray();
            
            dgv_param.Rows.Clear();
            dgv_cond.Rows.Clear();
            
            comboBox_method.Items.Clear();
            comboBox_method.Items.AddRange(methods);

            comboBox_method.Text = textBox_param.Text = textBox_cond.Text = "";

        }

        private void BindToUI()
        {
            var tp = ServiceProvider.GetService<TestPackage>();
            
            //initial value configuration
            textBox_name.Text = Item.Name;
            checkBox_skip.Checked = Item.Skip;
            numericUpDown_order.Value = Item.Order;
            numericUpDown_retry.Value = Item.Retry;
            textBox_args.Text = Item.Arguments;
            textBox_dscp.Text = Item.Description;

            comboBox_plugID.Items.Clear();
            comboBox_plugID.Items.AddRange(tp.Plugins.Select(x => x.Alias).ToArray());           

            comboBox_method.Items.Clear();            
            dgv_param.Rows.Clear();
            dgv_cond.Rows.Clear();
            textBox_param.Text = textBox_cond.Text = comboBox_method.Text = comboBox_plugID.Text= "";


            //if pluginID is null or empty, leave immediately
            if (string.IsNullOrEmpty(Item.PluginID)) return;

            comboBox_plugID.Text = Item.PluginID;
            var selectedPlugin = tp.Plugins.First(x => x.Alias == Item.PluginID);
            var assem = Assembly.LoadFrom(selectedPlugin.Path);
            var type = assem.GetTypes().First(x => x.AssemblyQualifiedName == selectedPlugin.QualifiedName);
            var methods = type.GetMethods().Where(x => x.GetCustomAttributes<MATSysCommandAttribute>().Any()).Select(x => x.Name).ToArray();
            
            comboBox_method.Items.AddRange(methods);
                        
            comboBox_method.Text = string.IsNullOrEmpty(Item.Method)?Item.Name: Item.Method;

            
            foreach (var item in Item.Parameters)
            {
                var row = dgv_param.Rows.Add(item.Key, item.Value);
                dgv_param.Rows[row].Cells[1].ValueType = item.Value.GetType();
            }
            textBox_param.Text = Item.Parameters.ToString();

            
            foreach (var item in Item.Conditions)
            {
                var row = dgv_cond.Rows.Add(item.Key, item.Value);
                dgv_cond.Rows[row].Cells[1].ValueType = item.Value.GetType();
            }
            textBox_cond.Text = Item.Conditions.ToString();

        }

        private void button_update_Click(object sender, EventArgs e)
        {
            //ID must exist, and path and qualifiedName might be empty
            Item.Name = textBox_name.Text;
            Item.Skip = checkBox_skip.Checked;
            Item.Retry = (int)numericUpDown_retry.Value;
            Item.Order = (int)numericUpDown_order.Value;
            Item.Arguments = textBox_args.Text;
            Item.Description = textBox_dscp.Text;
            Item.Method = comboBox_method.Text;
            Item.PluginID = comboBox_plugID.Text;
            if (string.IsNullOrEmpty(textBox_param.Text))
            {
                Item.Parameters = new ParameterCollection();
            }
            else
            {              
                Item.Parameters = ParameterCollection.Create(GetParametersFromUI());
            }


            if (string.IsNullOrEmpty(textBox_cond.Text))
            {
                Item.Conditions = new ParameterCollection();
            }
            else
            {
                Item.Conditions = ParameterCollection.Create(GetConditionsFromUI());
            }

            TestItemChanged?.Invoke(this, EventArgs.Empty);
        }
        private IEnumerable<(string,object)> GetParametersFromUI()
        {
            foreach (DataGridViewRow row in dgv_param.Rows)
            {
                yield return (row.Cells[0].Value.ToString(), row.Cells[1].Value);
            }
        }
        private IEnumerable<(string, object)> GetConditionsFromUI()
        {
            foreach (DataGridViewRow row in dgv_cond.Rows)
            {
                yield return (row.Cells[0].Value.ToString(), row.Cells[1].Value);
            }
        }


        private void button_restore_Click(object sender, EventArgs e)
        {
            Load(Item);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
