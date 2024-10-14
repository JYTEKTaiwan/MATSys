namespace AutomationTestUI.Forms
{
    partial class MainFormView
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
            menuStrip1 = new MenuStrip();
            toolStripMenuItem1 = new ToolStripMenuItem();
            createNewToolStripMenuItem = new ToolStripMenuItem();
            loadToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            editorModeToolStripMenuItem = new ToolStripMenuItem();
            executionModeToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            packageExplorerToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            tsContainer = new ToolStripContainer();
            dockPanel1 = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            vS2015LightTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2015LightTheme();
            vS2015BlueTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme();
            openFileDialog1 = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();
            menuStrip1.SuspendLayout();
            tsContainer.ContentPanel.SuspendLayout();
            tsContainer.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem2, viewToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { createNewToolStripMenuItem, loadToolStripMenuItem, saveAsToolStripMenuItem });
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(37, 20);
            toolStripMenuItem1.Text = "File";
            // 
            // createNewToolStripMenuItem
            // 
            createNewToolStripMenuItem.Name = "createNewToolStripMenuItem";
            createNewToolStripMenuItem.Size = new Size(180, 22);
            createNewToolStripMenuItem.Text = "Create new";
            createNewToolStripMenuItem.Click += createNewToolStripMenuItem_Click;
            // 
            // loadToolStripMenuItem
            // 
            loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            loadToolStripMenuItem.Size = new Size(180, 22);
            loadToolStripMenuItem.Text = "Load";
            loadToolStripMenuItem.Click += loadToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(180, 22);
            saveAsToolStripMenuItem.Text = "Save as";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.DropDownItems.AddRange(new ToolStripItem[] { editorModeToolStripMenuItem, executionModeToolStripMenuItem });
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(54, 20);
            toolStripMenuItem2.Text = "Open..";
            // 
            // editorModeToolStripMenuItem
            // 
            editorModeToolStripMenuItem.Name = "editorModeToolStripMenuItem";
            editorModeToolStripMenuItem.Size = new Size(158, 22);
            editorModeToolStripMenuItem.Text = "Script panel";
            editorModeToolStripMenuItem.Click += editorModeToolStripMenuItem_Click;
            // 
            // executionModeToolStripMenuItem
            // 
            executionModeToolStripMenuItem.Name = "executionModeToolStripMenuItem";
            executionModeToolStripMenuItem.Size = new Size(158, 22);
            executionModeToolStripMenuItem.Text = "Execution panel";
            executionModeToolStripMenuItem.Click += executionModeToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripSeparator2, packageExplorerToolStripMenuItem, toolStripSeparator1 });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "View";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(161, 6);
            // 
            // packageExplorerToolStripMenuItem
            // 
            packageExplorerToolStripMenuItem.Name = "packageExplorerToolStripMenuItem";
            packageExplorerToolStripMenuItem.Size = new Size(164, 22);
            packageExplorerToolStripMenuItem.Text = "Package Explorer";
            packageExplorerToolStripMenuItem.Click += packageExplorerToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(161, 6);
            // 
            // tsContainer
            // 
            // 
            // tsContainer.ContentPanel
            // 
            tsContainer.ContentPanel.Controls.Add(dockPanel1);
            tsContainer.ContentPanel.Size = new Size(800, 401);
            tsContainer.Dock = DockStyle.Fill;
            tsContainer.Location = new Point(0, 24);
            tsContainer.Name = "tsContainer";
            tsContainer.Size = new Size(800, 426);
            tsContainer.TabIndex = 1;
            tsContainer.Text = "toolStripContainer1";
            // 
            // dockPanel1
            // 
            dockPanel1.Dock = DockStyle.Fill;
            dockPanel1.DockBackColor = Color.FromArgb(238, 238, 242);
            dockPanel1.DockBottomPortion = 0.2D;
            dockPanel1.DockLeftPortion = 0.35D;
            dockPanel1.DockRightPortion = 0.2D;
            dockPanel1.DockTopPortion = 0.2D;
            dockPanel1.Location = new Point(0, 0);
            dockPanel1.Name = "dockPanel1";
            dockPanel1.Padding = new Padding(6);
            dockPanel1.ShowAutoHideContentOnHover = false;
            dockPanel1.Size = new Size(800, 401);
            dockPanel1.TabIndex = 0;
            dockPanel1.Theme = vS2015LightTheme1;
            // 
            // openFileDialog1
            // 
            openFileDialog1.DefaultExt = "xml";
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.DefaultExt = "xml";
            saveFileDialog1.Filter = "xml files (*.xml)|*.xml";
            // 
            // MainFormView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tsContainer);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.None;
            MainMenuStrip = menuStrip1;
            Name = "MainFormView";
            Text = "MainFormView";
            Load += MainFormView_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tsContainer.ContentPanel.ResumeLayout(false);
            tsContainer.ResumeLayout(false);
            tsContainer.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripContainer tsContainer;
        private ToolStripMenuItem createNewToolStripMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem editorModeToolStripMenuItem;
        private ToolStripMenuItem executionModeToolStripMenuItem;
        private OpenFileDialog openFileDialog1;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel1;
        private WeifenLuo.WinFormsUI.Docking.VS2015LightTheme vS2015LightTheme1;
        private WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme vS2015BlueTheme1;
        private SaveFileDialog saveFileDialog1;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem packageExplorerToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
    }
}