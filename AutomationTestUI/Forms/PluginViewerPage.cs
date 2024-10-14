using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Reflection;
using WeifenLuo.WinFormsUI.Docking;
using MATSys.Automation;
using MATSys;
using System.Drawing.Text;
using System.Windows.Forms;
using ExCSS;
using System.Linq.Expressions;
using AutomationTestUI.Dialogs;
using System.Collections.Generic;

namespace AutomationTestUI.Forms
{
    public partial class PluginViewerPage : SubForm
    {
        private static bool isShown = false;
        private static PluginViewerPage page;

        private Rectangle dragBoxFromMouseDown;
        private int dropSourceIndex;
        private int dropDestinationIndex;
        private PluginInfo current;
        private BindingSource bs = new BindingSource();

        private PluginViewerPage()
        {
            InitializeComponent();
            this.Padding = new Padding(5, 10, 5, 10);
            DecorateDGV();
            this.Load += PluginViewerPage_Load;

        }

        private void PluginViewerPage_Load(object? sender, EventArgs e)
        {
            var tp = ServiceProvider.GetService<TestPackage>();
            bs.DataSourceChanged += bindingSource_DataSourceChanged;
            bs.DataMemberChanged += bindingSource_DataMemberChanged;
            bs.DataSource = tp.Plugins;
            tp.FileLoaded += (sender, e) =>
            {
                var package = (TestPackage)sender;
                bs.DataSource = package.Plugins;
            };
        }

        private void bindingSource_DataMemberChanged(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void bindingSource_DataSourceChanged(object? sender, EventArgs e)
        {
            dgv.DataSource = null;
            dgv.DataSource = bs;           
        }

        private void DecorateDGV()
        {
            //general setting
            dgv.ReadOnly = true;
            dgv.BorderStyle = BorderStyle.None;
            dgv.BackgroundColor = SystemColors.Control;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToOrderColumns = false;
            dgv.AllowUserToDeleteRows = false;

            
            //rows configuration
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToResizeRows = true;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            
            
            //columns configuration
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.AllowUserToResizeColumns = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;            

            //decorate the column headers
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.WindowText;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = SystemColors.WindowText;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            var font = new Font(dgv.ColumnHeadersDefaultCellStyle.Font, System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Font = font;

            //configure default cell style
            dgv.DefaultCellStyle.BackColor = SystemColors.ControlLightLight;
            dgv.DefaultCellStyle.ForeColor = SystemColors.WindowText;
            dgv.DefaultCellStyle.SelectionBackColor = SystemColors.GradientActiveCaption;
            dgv.DefaultCellStyle.SelectionForeColor = SystemColors.WindowText;
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;

            //add columns
            dgv.Columns.Clear();
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                HeaderText = "ID",
                ToolTipText = "Unique ID of plugin",
                DataPropertyName = "ID",
                FillWeight = 10F,                
                MinimumWidth = 80,                                
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Path",
                ToolTipText = "Dll path of plugin",
                DataPropertyName = "Path",
                //DividerWidth = 3,
                FillWeight=40F,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "QualifiedName",
                ToolTipText = "Assembly qualified name of plugin",
                FillWeight = 50F,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DataPropertyName = "QualifiedName",
            });


            //add events
            dgv.CellDoubleClick += Dgv_CellDoubleClick;
            dgv.CellPainting += dgv_CellPainting;
            dgv.MouseClick += dgv_MouseClick;
            this.Refresh();
        }

        private void Dgv_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                var tp = ServiceProvider.GetService<TestPackage>();
                var item = tp.Plugins[e.RowIndex];
                if (PluginInfoEditorDialog.ShowDialog(item) == DialogResult.OK)
                {
                    dgv.Refresh();
                }
            }
        }

        public static PluginViewerPage OpenForm(DockPanel dp, DockState ds, ServiceProvider sp, ToolStripPanel tsp)
        {
            if (page == null || page.IsDisposed)
            {
                page = new PluginViewerPage();
                page.ServiceProvider = sp;
                //tsp.Controls.Add(page.toolStrip1);
                //page.FormClosed += (sender, e) => { tsp.Controls.Remove(page.toolStrip1); };
                page.Show(dp, ds);
            }
            return page;
        }
        public static PluginViewerPage OpenForm(DockPane dp, DockAlignment da, double portion, ServiceProvider sp, ToolStripPanel tsp)
        {
            if (page == null || page.IsDisposed)
            {
                page = new PluginViewerPage();
                page.ServiceProvider = sp;
                //tsp.Controls.Add(page.toolStrip1);
                //page.FormClosed += (sender, e) => { tsp.Controls.Remove(page.toolStrip1); };
                page.Show(dp, da, portion);
            }
            return page;
        }

        private void PluginViewerPage_Resize(object sender, EventArgs e)
        {

        }


        private void dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.Value == null)
                return;
            if (e.ColumnIndex==1)
            {
                var s = e.Graphics.MeasureString(e.Value.ToString(), dgv.Font);

                if (s.Width > dgv.Columns[e.ColumnIndex].Width)
                {
                    var color = dgv.Rows[e.RowIndex].Selected ? e.CellStyle.SelectionBackColor : e.CellStyle.BackColor;
                    using (
              Brush gridBrush = new SolidBrush(this.dgv.GridColor),
              backColorBrush = new SolidBrush(color))
                    {
                        e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
                        e.Graphics.DrawString(e.Value.ToString(), dgv.Font, Brushes.Black, e.CellBounds, StringFormat.GenericDefault);
                        dgv.Rows[e.RowIndex].Height = (int)(s.Height * Math.Ceiling(s.Width / dgv.Columns[e.ColumnIndex].Width));
                        e.Handled = true;
                    }
                }
            }
            
        }

        private void addPluginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pi = (PluginInfo)bs.AddNew();
            pi.Alias = pi.GetHashCode().ToString();
            var idx = bs.IndexOf(pi);
            dgv.Rows[idx].Selected = true;
            Dgv_CellDoubleClick(dgv, new DataGridViewCellEventArgs(0, idx));
            dgv.Refresh();
        }

        private void removePluginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var idx = dgv.SelectedRows[0].Index;
            bs.RemoveAt(idx);
            dgv.Refresh();
        }

        private void dgv_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int currentMouseOverRow = dgv.HitTest(e.X, e.Y).RowIndex;
                if (currentMouseOverRow != -1)
                {
                    dgv.Rows[currentMouseOverRow].Selected = true;
                    contextMenuStrip1.Items[1].Enabled = currentMouseOverRow >= 0;
                    contextMenuStrip1.Show(dgv, new System.Drawing.Point(e.X, e.Y));
                }
            }
        }
    }


}
