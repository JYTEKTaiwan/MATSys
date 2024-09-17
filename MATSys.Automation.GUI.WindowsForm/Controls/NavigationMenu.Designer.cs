namespace MATSys.Automation.GUI.WindowsForm.Controls
{
    partial class NavigationMenu
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NavigationMenu));
            imageList1 = new ImageList(components);
            flowLayoutPanel1 = new FlowLayoutPanel();
            navigationMenuItem1 = new NavigationMenuItem();
            navigationMenuItem2 = new NavigationMenuItem();
            navigationMenuItem3 = new NavigationMenuItem();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "JYTek.ico");
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.Controls.Add(navigationMenuItem1);
            flowLayoutPanel1.Controls.Add(navigationMenuItem2);
            flowLayoutPanel1.Controls.Add(navigationMenuItem3);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(155, 383);
            flowLayoutPanel1.TabIndex = 0;
            flowLayoutPanel1.WrapContents = false;
            // 
            // navigationMenuItem1
            // 
            navigationMenuItem1.Location = new Point(3, 3);
            navigationMenuItem1.Name = "navigationMenuItem1";
            navigationMenuItem1.Size = new Size(149, 50);
            navigationMenuItem1.TabIndex = 0;
            navigationMenuItem1.Title = "Item1";
            // 
            // navigationMenuItem2
            // 
            navigationMenuItem2.Location = new Point(3, 59);
            navigationMenuItem2.Name = "navigationMenuItem2";
            navigationMenuItem2.Size = new Size(149, 50);
            navigationMenuItem2.TabIndex = 1;
            navigationMenuItem2.Title = "Item2";
            // 
            // navigationMenuItem3
            // 
            navigationMenuItem3.Location = new Point(3, 115);
            navigationMenuItem3.Name = "navigationMenuItem3";
            navigationMenuItem3.Size = new Size(149, 50);
            navigationMenuItem3.TabIndex = 2;
            navigationMenuItem3.Title = "Item3";
            // 
            // NavigationMenu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(flowLayoutPanel1);
            Name = "NavigationMenu";
            Size = new Size(155, 383);
            flowLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ImageList imageList1;
        private FlowLayoutPanel flowLayoutPanel1;
        private NavigationMenuItem navigationMenuItem1;
        private NavigationMenuItem navigationMenuItem2;
        private NavigationMenuItem navigationMenuItem3;
    }
}
