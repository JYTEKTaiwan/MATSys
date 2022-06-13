
namespace MATSys.FrontPanel
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.toolStrip_configuration = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_hardware = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_GUI = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_logger = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_export = new System.Windows.Forms.ToolStripButton();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip_configuration.SuspendLayout();
            this.toolStripContainer1.LeftToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // toolStrip_configuration
            // 
            this.toolStrip_configuration.AllowItemReorder = true;
            this.toolStrip_configuration.AutoSize = false;
            this.toolStrip_configuration.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip_configuration.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip_configuration.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip_configuration.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_hardware,
            this.toolStripButton_GUI,
            this.toolStripButton_logger,
            this.toolStripButton_export});
            this.toolStrip_configuration.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
            this.toolStrip_configuration.Location = new System.Drawing.Point(0, 4);
            this.toolStrip_configuration.Name = "toolStrip_configuration";
            this.toolStrip_configuration.Padding = new System.Windows.Forms.Padding(2);
            this.toolStrip_configuration.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip_configuration.Size = new System.Drawing.Size(60, 270);
            this.toolStrip_configuration.TabIndex = 1;
            // 
            // toolStripButton_hardware
            // 
            this.toolStripButton_hardware.CheckOnClick = true;
            this.toolStripButton_hardware.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_hardware.Image = global::MATSys.FrontPanel.Properties.Resources.hardware;
            this.toolStripButton_hardware.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_hardware.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.toolStripButton_hardware.Name = "toolStripButton_hardware";
            this.toolStripButton_hardware.Size = new System.Drawing.Size(36, 36);
            this.toolStripButton_hardware.Text = "Hardware Configuration";
            this.toolStripButton_hardware.Click += new System.EventHandler(this.toolStripButton_hardware_Click);
            // 
            // toolStripButton_GUI
            // 
            this.toolStripButton_GUI.CheckOnClick = true;
            this.toolStripButton_GUI.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_GUI.Image = global::MATSys.FrontPanel.Properties.Resources.GUI;
            this.toolStripButton_GUI.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_GUI.Name = "toolStripButton_GUI";
            this.toolStripButton_GUI.Size = new System.Drawing.Size(36, 36);
            this.toolStripButton_GUI.Text = "GUI Configuration";
            // 
            // toolStripButton_logger
            // 
            this.toolStripButton_logger.CheckOnClick = true;
            this.toolStripButton_logger.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_logger.Image = global::MATSys.FrontPanel.Properties.Resources.log;
            this.toolStripButton_logger.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_logger.Name = "toolStripButton_logger";
            this.toolStripButton_logger.Size = new System.Drawing.Size(36, 36);
            this.toolStripButton_logger.Text = "Logger Configuration";
            this.toolStripButton_logger.ToolTipText = "Logger Configuration";
            // 
            // toolStripButton_export
            // 
            this.toolStripButton_export.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_export.Image = global::MATSys.FrontPanel.Properties.Resources.export;
            this.toolStripButton_export.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_export.Name = "toolStripButton_export";
            this.toolStripButton_export.Size = new System.Drawing.Size(36, 36);
            this.toolStripButton_export.Text = "ExportSetting";
            this.toolStripButton_export.ToolTipText = "ExportSetting";
            // 
            // ContentPanel
            // 
            this.ContentPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ContentPanel.Size = new System.Drawing.Size(1467, 1016);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(942, 495);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // toolStripContainer1.LeftToolStripPanel
            // 
            this.toolStripContainer1.LeftToolStripPanel.Controls.Add(this.toolStrip_configuration);
            this.toolStripContainer1.LeftToolStripPanel.Margin = new System.Windows.Forms.Padding(3);
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1002, 520);
            this.toolStripContainer1.TabIndex = 4;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1002, 520);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MATSys.FrontPanel";
            this.toolStrip_configuration.ResumeLayout(false);
            this.toolStrip_configuration.PerformLayout();
            this.toolStripContainer1.LeftToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridViewTextBoxColumn acquiredDataDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn templateDataDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn indexDataDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn voltageRangeDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn multiplierDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn offsetDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn differentialDataGridViewCheckBoxColumn1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn secondaryAxisDataGridViewCheckBoxColumn1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn showDataGridViewCheckBoxColumn1;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStrip toolStrip_configuration;
        private System.Windows.Forms.ToolStripButton toolStripButton_hardware;
        private System.Windows.Forms.ToolStripButton toolStripButton_GUI;
        private System.Windows.Forms.ToolStripButton toolStripButton_logger;
        private System.Windows.Forms.ToolStripButton toolStripButton_export;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
    }
}

