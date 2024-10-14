namespace AutomationTestUI.Forms
{
    partial class PluginViewerPage
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
            components = new System.ComponentModel.Container();
            openFileDialog1 = new OpenFileDialog();
            dgv = new DataGridView();
            contextMenuStrip1 = new ContextMenuStrip(components);
            addPluginToolStripMenuItem = new ToolStripMenuItem();
            removePluginToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // openFileDialog1
            // 
            openFileDialog1.DefaultExt = "dll";
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // dgv
            // 
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.Dock = DockStyle.Fill;
            dgv.EnableHeadersVisualStyles = false;
            dgv.GridColor = Color.Black;
            dgv.Location = new Point(0, 0);
            dgv.MultiSelect = false;
            dgv.Name = "dgv";
            dgv.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.RowHeadersVisible = false;
            dgv.RowHeadersWidth = 62;
            dgv.Size = new Size(413, 366);
            dgv.TabIndex = 3;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(24, 24);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { addPluginToolStripMenuItem, removePluginToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(155, 48);
            // 
            // addPluginToolStripMenuItem
            // 
            addPluginToolStripMenuItem.Name = "addPluginToolStripMenuItem";
            addPluginToolStripMenuItem.Size = new Size(154, 22);
            addPluginToolStripMenuItem.Text = "Add Plugin";
            addPluginToolStripMenuItem.Click += addPluginToolStripMenuItem_Click;
            // 
            // removePluginToolStripMenuItem
            // 
            removePluginToolStripMenuItem.Name = "removePluginToolStripMenuItem";
            removePluginToolStripMenuItem.Size = new Size(154, 22);
            removePluginToolStripMenuItem.Text = "Remove Plugin";
            removePluginToolStripMenuItem.Click += removePluginToolStripMenuItem_Click;
            // 
            // PluginViewerPage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(413, 366);
            ControlBox = false;
            Controls.Add(dgv);
            FormBorderStyle = FormBorderStyle.None;
            Name = "PluginViewerPage";
            TabText = "Plugins";
            Text = "Plugins";
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private OpenFileDialog openFileDialog1;
        private DataGridView dgv;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem addPluginToolStripMenuItem;
        private ToolStripMenuItem removePluginToolStripMenuItem;
    }
}