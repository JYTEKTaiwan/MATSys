namespace AutomationTestUI.Forms
{
    partial class ScriptEditorPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptEditorPage));
            toolStrip1 = new ToolStrip();
            toolStripButton_addItem = new ToolStripButton();
            toolStripButton_removeItem = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            dgv = new DataGridView();
            ColumnSkip = new DataGridViewCheckBoxColumn();
            ColumnOrder = new DataGridViewTextBoxColumn();
            ColumnName = new DataGridViewTextBoxColumn();
            ColumnParam = new DataGridViewTextBoxColumn();
            ColumnCondition = new DataGridViewTextBoxColumn();
            ColumnRetry = new DataGridViewTextBoxColumn();
            openFileDialog1 = new OpenFileDialog();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new Size(24, 24);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButton_addItem, toolStripButton_removeItem, toolStripSeparator1 });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new Padding(0, 0, 2, 0);
            toolStrip1.Size = new Size(800, 27);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_addItem
            // 
            toolStripButton_addItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButton_addItem.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            toolStripButton_addItem.Image = (Image)resources.GetObject("toolStripButton_addItem.Image");
            toolStripButton_addItem.ImageTransparentColor = Color.Magenta;
            toolStripButton_addItem.Name = "toolStripButton_addItem";
            toolStripButton_addItem.Size = new Size(24, 24);
            toolStripButton_addItem.Text = "+";
            toolStripButton_addItem.ToolTipText = "add";
            // 
            // toolStripButton_removeItem
            // 
            toolStripButton_removeItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButton_removeItem.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            toolStripButton_removeItem.Image = (Image)resources.GetObject("toolStripButton_removeItem.Image");
            toolStripButton_removeItem.ImageTransparentColor = Color.Magenta;
            toolStripButton_removeItem.Name = "toolStripButton_removeItem";
            toolStripButton_removeItem.Size = new Size(23, 24);
            toolStripButton_removeItem.Text = "-";
            toolStripButton_removeItem.ToolTipText = "remove";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 27);
            // 
            // dgv
            // 
            dgv.AllowDrop = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BorderStyle = BorderStyle.Fixed3D;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.Columns.AddRange(new DataGridViewColumn[] { ColumnSkip, ColumnOrder, ColumnName, ColumnParam, ColumnCondition, ColumnRetry });
            dgv.Dock = DockStyle.Fill;
            dgv.Location = new Point(0, 27);
            dgv.Name = "dgv";
            dgv.RowHeadersWidth = 62;
            dgv.Size = new Size(800, 423);
            dgv.TabIndex = 0;
            dgv.CellFormatting += dgv_CellFormatting;
            dgv.DragDrop += dgv_DragDrop;
            dgv.DragOver += dgv_DragOver;
            dgv.MouseDown += dgv_MouseDown;
            dgv.MouseMove += dgv_MouseMove;
            // 
            // ColumnSkip
            // 
            ColumnSkip.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ColumnSkip.HeaderText = "Skip?";
            ColumnSkip.MinimumWidth = 8;
            ColumnSkip.Name = "ColumnSkip";
            ColumnSkip.Width = 40;
            // 
            // ColumnOrder
            // 
            ColumnOrder.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ColumnOrder.HeaderText = "#";
            ColumnOrder.MinimumWidth = 8;
            ColumnOrder.Name = "ColumnOrder";
            ColumnOrder.Width = 39;
            // 
            // ColumnName
            // 
            ColumnName.HeaderText = "Name";
            ColumnName.MinimumWidth = 8;
            ColumnName.Name = "ColumnName";
            // 
            // ColumnParam
            // 
            ColumnParam.HeaderText = "Parameters";
            ColumnParam.MinimumWidth = 8;
            ColumnParam.Name = "ColumnParam";
            // 
            // ColumnCondition
            // 
            ColumnCondition.HeaderText = "Conditions";
            ColumnCondition.MinimumWidth = 8;
            ColumnCondition.Name = "ColumnCondition";
            // 
            // ColumnRetry
            // 
            ColumnRetry.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ColumnRetry.HeaderText = "Retry";
            ColumnRetry.MinimumWidth = 8;
            ColumnRetry.Name = "ColumnRetry";
            ColumnRetry.Width = 59;
            // 
            // openFileDialog1
            // 
            openFileDialog1.DefaultExt = "dll";
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // ScriptEditorPage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            ControlBox = false;
            Controls.Add(dgv);
            Controls.Add(toolStrip1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "ScriptEditorPage";
            TabText = "Scripts";
            Text = "Scripts";
            Load += EditorFormView_Load;
            Shown += EditorPage_Shown;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButton_addItem;
        private ToolStripButton toolStripButton_removeItem;
        private DataGridView dgv;
        private OpenFileDialog openFileDialog1;
        private ToolStripSeparator toolStripSeparator1;
        private DataGridViewCheckBoxColumn ColumnSkip;
        private DataGridViewTextBoxColumn ColumnOrder;
        private DataGridViewTextBoxColumn ColumnName;
        private DataGridViewTextBoxColumn ColumnParam;
        private DataGridViewTextBoxColumn ColumnCondition;
        private DataGridViewTextBoxColumn ColumnRetry;
    }
}