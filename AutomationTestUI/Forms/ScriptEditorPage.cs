using MATSys.Automation;
using Microsoft.Extensions.DependencyInjection;
using WeifenLuo.WinFormsUI.Docking;

namespace AutomationTestUI.Forms
{
    public partial class ScriptEditorPage : SubForm
    {
        private static ScriptEditorPage page;

        private Rectangle dragBoxFromMouseDown;
        private int dropSourceIndex;
        private int dropDestinationIndex;
        private BindingSource bs = new BindingSource();
        private ScriptEditorPage()
        {
            InitializeComponent();
            base.ToolStrip = toolStrip1;
        }

        public static ScriptEditorPage OpenForm(DockPanel dp, DockState ds, ServiceProvider sp, ToolStripPanel tsp)
        {
            if (page == null || page.IsDisposed)
            {
                page = new ScriptEditorPage();
                page.ServiceProvider = sp;
                tsp.Controls.Add(page.toolStrip1);
                page.FormClosed += (sender, e) => { tsp.Controls.Remove(page.toolStrip1); };
                page.Show(dp, ds);
            }
            return page;
        }
        public static ScriptEditorPage OpenForm(DockPane dp, DockAlignment da, double portion, ServiceProvider sp, ToolStripPanel tsp)
        {
            if (page == null || page.IsDisposed)
            {
                page = new ScriptEditorPage();
                page.ServiceProvider = sp;
                tsp.Controls.Add(page.toolStrip1);
                page.FormClosed += (sender, e) => { tsp.Controls.Remove(page.toolStrip1); };
                page.Show(dp, da, portion);
            }
            return page;
        }




        private void EditorFormView_Load(object sender, EventArgs e)
        {
            var tp = ServiceProvider.GetService<TestPackage>();
            bs.DataSource = (tp.Items);
            dgv.DataSource = bs;
            tp.FileLoaded += (sender, e) =>
            {
                bs.DataSource = ((TestPackage)sender).Items;
                dgv.DataSource = bs;

            };

        }

        private void DecorateDGV()
        {
            dgv.BorderStyle = BorderStyle.Fixed3D;
            dgv.BackgroundColor = SystemColors.Control;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SunkenHorizontal;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToOrderColumns = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.RowHeadersVisible = false;
            dgv.AutoGenerateColumns = false;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //add columns
            dgv.Columns.Clear();
            dgv.Columns.Add(new DataGridViewCheckBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                HeaderText = "Skip?",
                ToolTipText = "Skip this item or not",
                DataPropertyName = "Skip",
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                HeaderText = "#",
                ToolTipText = "Test item Number",
                DataPropertyName = "Order",
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Name",
                ToolTipText = "Test item name",
                DataPropertyName = "Name",
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Parameters",
                ToolTipText = "Parameters for the test item",
                //DataPropertyName = "Parameters",                
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Conditions",
                ToolTipText = "Parameters for the pass/fail process",
                //DataPropertyName = "Conditions",
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                HeaderText = "Retry",
                ToolTipText = "Number of retrying if fail",
                DataPropertyName = "Retry",
            });

            this.Refresh();

        }

        private void dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewColumn column = dgv.Columns[e.ColumnIndex];
            object data = dgv.Rows[e.RowIndex].DataBoundItem;

            switch (column.HeaderText)
            {
                case "Conditions":
                case "Parameters":
                    data = data.GetType().GetProperty(column.HeaderText).GetValue(data);
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = data;
                    break;
                default:
                    break;
            }


        }


        #region Datagridview drapdrop effect
        private void dgv_MouseMove(object sender, MouseEventArgs e)
        {
            var dgv = (DataGridView)sender;

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty &&
                !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    // Proceed with the drag and drop, passing in the list item.                    
                    DragDropEffects dropEffect = dgv.DoDragDrop(
                          dgv.Rows[dropSourceIndex],
                          DragDropEffects.Move);
                }
            }
        }


        private void dgv_DragDrop(object sender, DragEventArgs e)
        {
            var dgv = (DataGridView)sender;
            var package = ServiceProvider.GetService<TestPackage>();
            // The mouse locations are relative to the screen, so they must be 
            // converted to client coordinates.
            Point clientPoint = dgv.PointToClient(new Point(e.X, e.Y));

            // Get the row index of the item the mouse is below. 
            dropDestinationIndex = dgv.HitTest(clientPoint.X, clientPoint.Y).RowIndex;
            var itemTobeMoved = package.Items[dropSourceIndex];

            if (dropDestinationIndex > -1)
            {

                // If the drag operation was a move then remove and insert the row.
                if (e.Effect == DragDropEffects.Move)
                {
                    package.Items.RemoveAt(dropSourceIndex);
                    package.Items.Insert(dropDestinationIndex, itemTobeMoved);
                }
                dgv.Invalidate();
            }

        }

        private void dgv_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void dgv_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            dropSourceIndex = dgv.HitTest(e.X, e.Y).RowIndex;

            if (dropSourceIndex != -1)
            {
                // Remember the point where the mouse down occurred. 
                // The DragSize indicates the size that the mouse can move 
                // before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                dragBoxFromMouseDown = new Rectangle(
                          new Point(
                            e.X - (dragSize.Width / 2),
                            e.Y - (dragSize.Height / 2)),
                      dragSize);
            }
            else
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;
        }


        #endregion


        private void EditorPage_Shown(object sender, EventArgs e)
        {
            DecorateDGV();
        }

        
    }
}
