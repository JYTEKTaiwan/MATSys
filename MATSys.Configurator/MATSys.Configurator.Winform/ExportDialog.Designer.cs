namespace MATSys.Configurator.Winform
{
    partial class ExportDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportDialog));
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.richTextBox_nlog = new System.Windows.Forms.RichTextBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.propertyGrid_transceiver = new System.Windows.Forms.PropertyGrid();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.richTextBox_scripts = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(23, 13);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(145, 19);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Enbel NLog in Settings";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // richTextBox_nlog
            // 
            this.richTextBox_nlog.Location = new System.Drawing.Point(23, 38);
            this.richTextBox_nlog.Name = "richTextBox_nlog";
            this.richTextBox_nlog.Size = new System.Drawing.Size(238, 341);
            this.richTextBox_nlog.TabIndex = 1;
            this.richTextBox_nlog.Text = resources.GetString("richTextBox_nlog.Text");
            this.richTextBox_nlog.Visible = false;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(296, 13);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(122, 19);
            this.checkBox2.TabIndex = 0;
            this.checkBox2.Text = "Enable Transceiver";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(296, 38);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(228, 23);
            this.comboBox1.TabIndex = 2;
            this.comboBox1.Visible = false;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // propertyGrid_transceiver
            // 
            this.propertyGrid_transceiver.BackColor = System.Drawing.SystemColors.Info;
            this.propertyGrid_transceiver.HelpBackColor = System.Drawing.SystemColors.Info;
            this.propertyGrid_transceiver.HelpVisible = false;
            this.propertyGrid_transceiver.Location = new System.Drawing.Point(296, 67);
            this.propertyGrid_transceiver.Name = "propertyGrid_transceiver";
            this.propertyGrid_transceiver.Size = new System.Drawing.Size(228, 312);
            this.propertyGrid_transceiver.TabIndex = 8;
            this.propertyGrid_transceiver.ToolbarVisible = false;
            this.propertyGrid_transceiver.ViewBackColor = System.Drawing.SystemColors.Info;
            this.propertyGrid_transceiver.Visible = false;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(557, 13);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(94, 19);
            this.checkBox3.TabIndex = 0;
            this.checkBox3.Text = "Enable Script";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // richTextBox_scripts
            // 
            this.richTextBox_scripts.Location = new System.Drawing.Point(557, 38);
            this.richTextBox_scripts.Name = "richTextBox_scripts";
            this.richTextBox_scripts.Size = new System.Drawing.Size(238, 341);
            this.richTextBox_scripts.TabIndex = 1;
            this.richTextBox_scripts.Text = "";
            this.richTextBox_scripts.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(579, 405);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 36);
            this.button1.TabIndex = 9;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(704, 405);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(91, 36);
            this.button2.TabIndex = 9;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ExportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(817, 457);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.propertyGrid_transceiver);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.richTextBox_scripts);
            this.Controls.Add(this.richTextBox_nlog);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Name = "ExportDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ExportForm";
            this.Shown += new System.EventHandler(this.ExportDialog_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckBox checkBox1;
        private RichTextBox richTextBox_nlog;
        private CheckBox checkBox2;
        private ComboBox comboBox1;
        private PropertyGrid propertyGrid_transceiver;
        private CheckBox checkBox3;
        private RichTextBox richTextBox_scripts;
        private Button button1;
        private Button button2;
    }
}