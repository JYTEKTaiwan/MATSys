using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
namespace MATSys.Automation.GUI.WindowsForm.Pages
{
    public partial class ScriptEditorPage : DockContent
    {
        private TestPackage tp;
        private IServiceProvider sp;
        private ToolStrip ts = new ToolStrip();
        public ScriptEditorPage()
        {
            InitializeComponent();            
            base.CloseButtonVisible = false;
            sp = Services.ServiceHub.Instance.Value.ServiceProvider;
            tp = sp.GetRequiredService<TestPackage>();

            for (int i = 0; i < 30; i++)
            {
                tp.Items.Add(new TestItem()
                {
                    Name = $"Item{i}",
                    Description = $"This is TestItem#{i}",
                    Order = i,
                    Skip = false,
                    Retry = 5,
                    Worker = new Worker()
                    {
                        Alias = "mod",
                        MethodName = "",
                        Parameters = { { $"Number", i } },
                        Conditions = { { $"Limit", i * 10 } },
                    }
                });
            }
            dgv.DataSource = tp.Items;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AllowUserToOrderColumns = false;
            dgv.ReadOnly = true;
            dgv.GridColor = SystemColors.Window;
            dgv.RowHeadersVisible=false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.SelectionChanged += Dgv_SelectionChanged;
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            var current = dgv.CurrentCell.RowIndex;            
            tp.CurrentTestItem= tp.Items[current];
        }

        private void Tp_FileExported(object? sender, string e)
        {
            throw new NotImplementedException();
        }

        private void Tp_FileLoaded(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ScriptEditorPage_Load(object sender, EventArgs e)
        {
            
        }


    }
}
