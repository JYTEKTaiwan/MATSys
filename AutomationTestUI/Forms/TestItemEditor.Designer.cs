namespace AutomationTestUI.Forms
{
    partial class TestItemEditor
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            openFileDialog1 = new OpenFileDialog();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            textBox_name = new TextBox();
            checkBox_skip = new CheckBox();
            numericUpDown_retry = new NumericUpDown();
            textBox_args = new TextBox();
            textBox_dscp = new TextBox();
            comboBox_plugID = new ComboBox();
            textBox_param = new TextBox();
            textBox_cond = new TextBox();
            label9 = new Label();
            comboBox_method = new ComboBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            groupBox1 = new GroupBox();
            label10 = new Label();
            numericUpDown_order = new NumericUpDown();
            groupBox2 = new GroupBox();
            dgv_cond = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dgv_param = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            button_update = new Button();
            splitContainer1 = new SplitContainer();
            tableLayoutPanel1 = new TableLayoutPanel();
            button_restore = new Button();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_retry).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_order).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv_cond).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgv_param).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // openFileDialog1
            // 
            openFileDialog1.Filter = "dll files (*.dll)|*.dll";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 35);
            label1.Name = "label1";
            label1.Size = new Size(66, 15);
            label1.TabIndex = 0;
            label1.Text = "Item Name";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(45, 110);
            label2.Name = "label2";
            label2.Size = new Size(34, 15);
            label2.TabIndex = 0;
            label2.Text = "Skip?";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(45, 140);
            label3.Name = "label3";
            label3.Size = new Size(34, 15);
            label3.TabIndex = 0;
            label3.Text = "Retry";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(272, 34);
            label4.Name = "label4";
            label4.Size = new Size(66, 15);
            label4.TabIndex = 0;
            label4.Text = "Arguments";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(271, 110);
            label5.Name = "label5";
            label5.Size = new Size(67, 15);
            label5.TabIndex = 0;
            label5.Text = "Description";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(24, 34);
            label6.Name = "label6";
            label6.Size = new Size(55, 15);
            label6.TabIndex = 0;
            label6.Text = "Plugin ID";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(13, 81);
            label7.Name = "label7";
            label7.Size = new Size(66, 15);
            label7.TabIndex = 0;
            label7.Text = "Parameters";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(273, 81);
            label8.Name = "label8";
            label8.Size = new Size(65, 15);
            label8.TabIndex = 0;
            label8.Text = "Conditions";
            // 
            // textBox_name
            // 
            textBox_name.Location = new Point(85, 32);
            textBox_name.Name = "textBox_name";
            textBox_name.PlaceholderText = "Test Item Name";
            textBox_name.Size = new Size(182, 23);
            textBox_name.TabIndex = 1;
            // 
            // checkBox_skip
            // 
            checkBox_skip.AutoSize = true;
            checkBox_skip.Location = new Point(85, 111);
            checkBox_skip.Name = "checkBox_skip";
            checkBox_skip.Size = new Size(15, 14);
            checkBox_skip.TabIndex = 2;
            checkBox_skip.UseVisualStyleBackColor = true;
            // 
            // numericUpDown_retry
            // 
            numericUpDown_retry.Location = new Point(85, 138);
            numericUpDown_retry.Name = "numericUpDown_retry";
            numericUpDown_retry.Size = new Size(60, 23);
            numericUpDown_retry.TabIndex = 3;
            // 
            // textBox_args
            // 
            textBox_args.Location = new Point(344, 35);
            textBox_args.Multiline = true;
            textBox_args.Name = "textBox_args";
            textBox_args.PlaceholderText = "Arguments";
            textBox_args.Size = new Size(182, 50);
            textBox_args.TabIndex = 1;
            // 
            // textBox_dscp
            // 
            textBox_dscp.Location = new Point(344, 111);
            textBox_dscp.Multiline = true;
            textBox_dscp.Name = "textBox_dscp";
            textBox_dscp.PlaceholderText = "Description";
            textBox_dscp.Size = new Size(182, 50);
            textBox_dscp.TabIndex = 1;
            // 
            // comboBox_plugID
            // 
            comboBox_plugID.FormattingEnabled = true;
            comboBox_plugID.Location = new Point(85, 34);
            comboBox_plugID.Name = "comboBox_plugID";
            comboBox_plugID.Size = new Size(161, 23);
            comboBox_plugID.TabIndex = 4;
            // 
            // textBox_param
            // 
            textBox_param.Location = new Point(85, 81);
            textBox_param.Multiline = true;
            textBox_param.Name = "textBox_param";
            textBox_param.PlaceholderText = "Serialized Text";
            textBox_param.Size = new Size(180, 49);
            textBox_param.TabIndex = 5;
            // 
            // textBox_cond
            // 
            textBox_cond.Location = new Point(344, 81);
            textBox_cond.Multiline = true;
            textBox_cond.Name = "textBox_cond";
            textBox_cond.PlaceholderText = "Serialized Text";
            textBox_cond.Size = new Size(182, 47);
            textBox_cond.TabIndex = 5;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(254, 34);
            label9.Name = "label9";
            label9.Size = new Size(84, 15);
            label9.TabIndex = 0;
            label9.Text = "Method Name";
            // 
            // comboBox_method
            // 
            comboBox_method.FormattingEnabled = true;
            comboBox_method.Location = new Point(344, 34);
            comboBox_method.Name = "comboBox_method";
            comboBox_method.Size = new Size(161, 23);
            comboBox_method.TabIndex = 4;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.Controls.Add(groupBox1);
            flowLayoutPanel1.Controls.Add(groupBox2);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(577, 515);
            flowLayoutPanel1.TabIndex = 10;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(textBox_args);
            groupBox1.Controls.Add(textBox_name);
            groupBox1.Controls.Add(textBox_dscp);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(checkBox_skip);
            groupBox1.Controls.Add(label10);
            groupBox1.Controls.Add(numericUpDown_retry);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(numericUpDown_order);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label4);
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(542, 190);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Test Item Information";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(42, 76);
            label10.Name = "label10";
            label10.Size = new Size(37, 15);
            label10.TabIndex = 0;
            label10.Text = "Order";
            // 
            // numericUpDown1
            // 
            numericUpDown_order.Location = new Point(85, 74);
            numericUpDown_order.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            numericUpDown_order.Minimum = new decimal(new int[] { 1000000, 0, 0, int.MinValue });
            numericUpDown_order.Name = "numericUpDown1";
            numericUpDown_order.Size = new Size(60, 23);
            numericUpDown_order.TabIndex = 3;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(dgv_cond);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(dgv_param);
            groupBox2.Controls.Add(comboBox_plugID);
            groupBox2.Controls.Add(textBox_cond);
            groupBox2.Controls.Add(label9);
            groupBox2.Controls.Add(textBox_param);
            groupBox2.Controls.Add(comboBox_method);
            groupBox2.Location = new Point(3, 199);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(537, 289);
            groupBox2.TabIndex = 9;
            groupBox2.TabStop = false;
            groupBox2.Text = "Command Detail";
            // 
            // dgv_cond
            // 
            dgv_cond.AllowUserToAddRows = false;
            dgv_cond.AllowUserToDeleteRows = false;
            dgv_cond.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv_cond.BackgroundColor = SystemColors.Control;
            dgv_cond.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.ActiveCaption;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.ActiveCaption;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgv_cond.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgv_cond.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv_cond.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2 });
            dgv_cond.Location = new Point(344, 134);
            dgv_cond.Name = "dgv_cond";
            dgv_cond.RowHeadersVisible = false;
            dgv_cond.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgv_cond.Size = new Size(180, 140);
            dgv_cond.TabIndex = 7;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "Name";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            dataGridViewTextBoxColumn1.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Value";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // dgv_param
            // 
            dgv_param.AllowUserToAddRows = false;
            dgv_param.AllowUserToDeleteRows = false;
            dgv_param.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv_param.BackgroundColor = SystemColors.Control;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.ActiveCaption;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.ActiveCaption;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgv_param.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgv_param.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv_param.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2 });
            dgv_param.Location = new Point(85, 136);
            dgv_param.Name = "dgv_param";
            dgv_param.RowHeadersVisible = false;
            dgv_param.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgv_param.Size = new Size(180, 140);
            dgv_param.TabIndex = 6;
            // 
            // Column1
            // 
            Column1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column1.HeaderText = "Name";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            Column1.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Column2
            // 
            Column2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column2.HeaderText = "Value";
            Column2.Name = "Column2";
            Column2.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // button_update
            // 
            button_update.Dock = DockStyle.Left;
            button_update.Location = new Point(93, 3);
            button_update.Name = "button_update";
            button_update.Size = new Size(66, 35);
            button_update.TabIndex = 0;
            button_update.Text = "Update";
            button_update.UseVisualStyleBackColor = true;
            button_update.Click += button_update_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.Location = new Point(10, 10);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(flowLayoutPanel1);
            splitContainer1.Size = new Size(577, 560);
            splitContainer1.SplitterDistance = 41;
            splitContainer1.TabIndex = 8;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 374F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(button_restore, 2, 0);
            tableLayoutPanel1.Controls.Add(button_update, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(577, 41);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // button_restore
            // 
            button_restore.Dock = DockStyle.Left;
            button_restore.Location = new Point(183, 3);
            button_restore.Name = "button_restore";
            button_restore.Size = new Size(66, 35);
            button_restore.TabIndex = 0;
            button_restore.Text = "Restore";
            button_restore.UseVisualStyleBackColor = true;
            button_restore.Click += button_restore_Click;
            // 
            // TestItemEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(597, 580);
            Controls.Add(splitContainer1);
            Name = "TestItemEditor";
            Padding = new Padding(10);
            Text = "TestItemEditor";
            ((System.ComponentModel.ISupportInitialize)numericUpDown_retry).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_order).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgv_cond).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgv_param).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private OpenFileDialog openFileDialog1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private TextBox textBox_name;
        private CheckBox checkBox_skip;
        private NumericUpDown numericUpDown_retry;
        private TextBox textBox_args;
        private TextBox textBox_dscp;
        private ComboBox comboBox_plugID;
        private TextBox textBox_param;
        private TextBox textBox_cond;
        private Label label9;
        private ComboBox comboBox_method;
        private Label label10;
        private NumericUpDown numericUpDown_order;
        private Button button_update;
        private SplitContainer splitContainer1;
        private Button button_restore;
        private TableLayoutPanel tableLayoutPanel1;
        private DataGridView dgv_param;
        private DataGridView dgv_cond;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private FlowLayoutPanel flowLayoutPanel1;
    }
}