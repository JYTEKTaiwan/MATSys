namespace AutomationTestUI.Forms
{
    partial class PackageExplorer
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
            TreeNode treeNode1 = new TreeNode("Node1");
            TreeNode treeNode2 = new TreeNode("Node2");
            TreeNode treeNode3 = new TreeNode("Node0", new TreeNode[] { treeNode1, treeNode2 });
            TreeNode treeNode4 = new TreeNode("Node4");
            TreeNode treeNode5 = new TreeNode("Node5");
            TreeNode treeNode6 = new TreeNode("Node6");
            TreeNode treeNode7 = new TreeNode("Node7");
            TreeNode treeNode8 = new TreeNode("Node8");
            TreeNode treeNode9 = new TreeNode("Node9");
            TreeNode treeNode10 = new TreeNode("Node10");
            TreeNode treeNode11 = new TreeNode("Node11");
            TreeNode treeNode12 = new TreeNode("Node12");
            TreeNode treeNode13 = new TreeNode("Node13");
            TreeNode treeNode14 = new TreeNode("Node3", new TreeNode[] { treeNode4, treeNode5, treeNode6, treeNode7, treeNode8, treeNode9, treeNode10, treeNode11, treeNode12, treeNode13 });
            trv = new TreeView();
            SuspendLayout();
            // 
            // trv
            // 
            trv.AllowDrop = true;
            trv.Dock = DockStyle.Fill;
            trv.Location = new Point(0, 0);
            trv.Name = "trv";
            treeNode1.Name = "Node1";
            treeNode1.Text = "Node1";
            treeNode2.Name = "Node2";
            treeNode2.Text = "Node2";
            treeNode3.Name = "Node0";
            treeNode3.Text = "Node0";
            treeNode4.Name = "Node4";
            treeNode4.Text = "Node4";
            treeNode5.Name = "Node5";
            treeNode5.Text = "Node5";
            treeNode6.Name = "Node6";
            treeNode6.Text = "Node6";
            treeNode7.Name = "Node7";
            treeNode7.Text = "Node7";
            treeNode8.Name = "Node8";
            treeNode8.Text = "Node8";
            treeNode9.Name = "Node9";
            treeNode9.Text = "Node9";
            treeNode10.Name = "Node10";
            treeNode10.Text = "Node10";
            treeNode11.Name = "Node11";
            treeNode11.Text = "Node11";
            treeNode12.Name = "Node12";
            treeNode12.Text = "Node12";
            treeNode13.Name = "Node13";
            treeNode13.Text = "Node13";
            treeNode14.Name = "Node3";
            treeNode14.Text = "Node3";
            trv.Nodes.AddRange(new TreeNode[] { treeNode3, treeNode14 });
            trv.ShowNodeToolTips = true;
            trv.Size = new Size(219, 450);
            trv.TabIndex = 0;
            trv.ItemDrag += trv_ItemDrag;
            trv.DragDrop += trv_DragDrop;
            trv.DragEnter += trv_DragEnter;
            // 
            // PackageExplorer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(219, 450);
            ControlBox = false;
            Controls.Add(trv);
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight;
            FormBorderStyle = FormBorderStyle.None;
            Name = "PackageExplorer";
            TabText = "PackageExplorer";
            Text = "PackageExplorer";
            FormClosing += ItemEditorPage_FormClosing;
            Load += PackgeExplorer_Load;
            Shown += ItemEditorPage_Shown;
            ResumeLayout(false);
        }

        #endregion
        private TreeView trv;
    }
}