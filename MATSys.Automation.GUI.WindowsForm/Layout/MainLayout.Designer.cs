namespace MATSys.Automation.GUI.WindowsForm.Layout
{
    partial class MainLayout
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
            tableLayout = new TableLayoutPanel();
            logoControl1 = new Controls.LogoControl();
            headerControl1 = new Controls.HeaderControl();
            navigationMenu1 = new Controls.NavigationMenu();
            tableLayout.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayout
            // 
            tableLayout.ColumnCount = 2;
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 161F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayout.Controls.Add(logoControl1, 0, 0);
            tableLayout.Controls.Add(headerControl1, 1, 0);
            tableLayout.Controls.Add(navigationMenu1, 0, 1);
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.Location = new Point(0, 0);
            tableLayout.Name = "tableLayout";
            tableLayout.RowCount = 2;
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayout.Size = new Size(800, 450);
            tableLayout.TabIndex = 0;
            // 
            // logoControl1
            // 
            logoControl1.BackColor = SystemColors.Window;
            logoControl1.Dock = DockStyle.Fill;
            logoControl1.Location = new Point(3, 3);
            logoControl1.Name = "logoControl1";
            logoControl1.Size = new Size(155, 44);
            logoControl1.TabIndex = 0;
            // 
            // headerControl1
            // 
            headerControl1.Dock = DockStyle.Fill;
            headerControl1.Location = new Point(164, 3);
            headerControl1.Name = "headerControl1";
            headerControl1.Size = new Size(633, 44);
            headerControl1.TabIndex = 2;
            // 
            // navigationMenu1
            // 
            navigationMenu1.Dock = DockStyle.Fill;
            navigationMenu1.Location = new Point(3, 53);
            navigationMenu1.Name = "navigationMenu1";
            navigationMenu1.Size = new Size(155, 394);
            navigationMenu1.TabIndex = 3;
            // 
            // MainLayout
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tableLayout);
            FormBorderStyle = FormBorderStyle.None;
            Name = "MainLayout";
            Text = "MainLayout";
            tableLayout.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayout;
        private Controls.LogoControl logoControl1;
        private Controls.HeaderControl headerControl1;
        private Controls.NavigationMenu navigationMenu1;
    }
}