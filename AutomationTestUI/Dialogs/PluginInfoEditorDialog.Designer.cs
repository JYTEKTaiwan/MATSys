namespace AutomationTestUI.Dialogs
{
    partial class PluginInfoEditorDialog
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
            tableLayoutPanel1 = new TableLayoutPanel();
            label3 = new Label();
            textBox_path = new TextBox();
            label2 = new Label();
            label1 = new Label();
            textBox_id = new TextBox();
            textBox_fullname = new TextBox();
            comboBox_mods = new ComboBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            button_ok = new Button();
            button_cancel = new Button();
            button_browse = new Button();
            openFileDialog1 = new OpenFileDialog();
            tableLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 89.92538F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10.0746269F));
            tableLayoutPanel1.Controls.Add(label3, 0, 2);
            tableLayoutPanel1.Controls.Add(textBox_path, 1, 1);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(textBox_id, 1, 0);
            tableLayoutPanel1.Controls.Add(textBox_fullname, 1, 3);
            tableLayoutPanel1.Controls.Add(comboBox_mods, 1, 2);
            tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 1, 4);
            tableLayoutPanel1.Controls.Add(button_browse, 2, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(10, 20, 10, 10);
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.Size = new Size(405, 326);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // label3
            // 
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(13, 153);
            label3.Name = "label3";
            label3.Padding = new Padding(0, 3, 0, 0);
            tableLayoutPanel1.SetRowSpan(label3, 2);
            label3.Size = new Size(64, 133);
            label3.TabIndex = 4;
            label3.Text = "Qualified Name";
            label3.TextAlign = ContentAlignment.TopRight;
            // 
            // textBox_path
            // 
            textBox_path.Dock = DockStyle.Fill;
            textBox_path.Location = new Point(83, 53);
            textBox_path.Multiline = true;
            textBox_path.Name = "textBox_path";
            textBox_path.ReadOnly = true;
            textBox_path.Size = new Size(277, 97);
            textBox_path.TabIndex = 3;
            textBox_path.TabStop = false;
            // 
            // label2
            // 
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(13, 50);
            label2.Name = "label2";
            label2.Padding = new Padding(0, 3, 0, 0);
            label2.Size = new Size(64, 103);
            label2.TabIndex = 2;
            label2.Text = "Path";
            label2.TextAlign = ContentAlignment.TopRight;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(13, 20);
            label1.Name = "label1";
            label1.Padding = new Padding(0, 3, 0, 0);
            label1.Size = new Size(64, 30);
            label1.TabIndex = 0;
            label1.Text = "ID";
            label1.TextAlign = ContentAlignment.TopRight;
            // 
            // textBox_id
            // 
            tableLayoutPanel1.SetColumnSpan(textBox_id, 2);
            textBox_id.Dock = DockStyle.Fill;
            textBox_id.Location = new Point(83, 23);
            textBox_id.Name = "textBox_id";
            textBox_id.Size = new Size(309, 23);
            textBox_id.TabIndex = 1;
            textBox_id.TabStop = false;
            // 
            // textBox_fullname
            // 
            tableLayoutPanel1.SetColumnSpan(textBox_fullname, 2);
            textBox_fullname.Dock = DockStyle.Fill;
            textBox_fullname.Location = new Point(83, 186);
            textBox_fullname.Multiline = true;
            textBox_fullname.Name = "textBox_fullname";
            textBox_fullname.ReadOnly = true;
            textBox_fullname.Size = new Size(309, 97);
            textBox_fullname.TabIndex = 6;
            textBox_fullname.TabStop = false;
            // 
            // comboBox_mods
            // 
            tableLayoutPanel1.SetColumnSpan(comboBox_mods, 2);
            comboBox_mods.Dock = DockStyle.Fill;
            comboBox_mods.FormattingEnabled = true;
            comboBox_mods.Location = new Point(83, 156);
            comboBox_mods.Name = "comboBox_mods";
            comboBox_mods.Size = new Size(309, 23);
            comboBox_mods.TabIndex = 9;
            comboBox_mods.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(button_ok);
            flowLayoutPanel1.Controls.Add(button_cancel);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(80, 286);
            flowLayoutPanel1.Margin = new Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(283, 30);
            flowLayoutPanel1.TabIndex = 10;
            // 
            // button_ok
            // 
            button_ok.Location = new Point(3, 3);
            button_ok.Name = "button_ok";
            button_ok.Size = new Size(75, 23);
            button_ok.TabIndex = 0;
            button_ok.TabStop = false;
            button_ok.Text = "OK";
            button_ok.UseVisualStyleBackColor = true;
            button_ok.Click += button_ok_Click;
            // 
            // button_cancel
            // 
            button_cancel.Location = new Point(84, 3);
            button_cancel.Name = "button_cancel";
            button_cancel.Size = new Size(75, 23);
            button_cancel.TabIndex = 1;
            button_cancel.TabStop = false;
            button_cancel.Text = "Cancel";
            button_cancel.UseVisualStyleBackColor = true;
            button_cancel.Click += button_cancel_Click;
            // 
            // button_browse
            // 
            button_browse.Dock = DockStyle.Top;
            button_browse.Location = new Point(366, 53);
            button_browse.Name = "button_browse";
            button_browse.Size = new Size(26, 23);
            button_browse.TabIndex = 11;
            button_browse.TabStop = false;
            button_browse.Text = "...";
            button_browse.UseVisualStyleBackColor = true;
            button_browse.Click += button_browse_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.AddToRecent = false;
            openFileDialog1.DefaultExt = "dll";
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "dll files (*.dll)|*.dll";
            openFileDialog1.Title = "Choose dll";
            // 
            // PluginInfoEditorDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = button_cancel;
            ClientSize = new Size(405, 326);
            ControlBox = false;
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "PluginInfoEditorDialog";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Plugin Editor";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label label3;
        private TextBox textBox_path;
        private Label label2;
        private Label label1;
        private TextBox textBox_id;
        private TextBox textBox_fullname;
        private ComboBox comboBox_mods;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button button_ok;
        private Button button_cancel;
        private Button button_browse;
        private OpenFileDialog openFileDialog1;
    }
}