namespace ghGPS.Forms
{
    partial class TheEllipsoids
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TheEllipsoids));
            this.panel1 = new System.Windows.Forms.Panel();
            this.olvPointListTree = new BrightIdeasSoftware.ObjectListView();
            this.clnEllipsoidName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnSemiMajorAxis = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnInverseFlattening = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.panel3 = new System.Windows.Forms.Panel();
            this.metroLabel8 = new MetroSuite.MetroLabel();
            this.metroLabel10 = new MetroSuite.MetroLabel();
            this.tbxInverseFlattening = new MetroSuite.MetroTextbox();
            this.tbxSemi_Major_Axis = new MetroSuite.MetroTextbox();
            this.metroLabel11 = new MetroSuite.MetroLabel();
            this.tbxEllipsoidName = new MetroSuite.MetroTextbox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.btnCancel = new MetroSuite.MetroButton();
            this.btnOK = new MetroSuite.MetroButton();
            this.label2 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvPointListTree)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.olvPointListTree);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(304, 329);
            this.panel1.TabIndex = 6;
            // 
            // olvPointListTree
            // 
            this.olvPointListTree.AllColumns.Add(this.clnEllipsoidName);
            this.olvPointListTree.AllColumns.Add(this.clnSemiMajorAxis);
            this.olvPointListTree.AllColumns.Add(this.clnInverseFlattening);
            this.olvPointListTree.AllowColumnReorder = true;
            this.olvPointListTree.AllowDrop = true;
            this.olvPointListTree.BackColor = System.Drawing.Color.WhiteSmoke;
            this.olvPointListTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.olvPointListTree.CellEditUseWholeCell = false;
            this.olvPointListTree.CheckedAspectName = "";
            this.olvPointListTree.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clnEllipsoidName,
            this.clnSemiMajorAxis,
            this.clnInverseFlattening});
            this.olvPointListTree.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvPointListTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvPointListTree.EmptyListMsg = "";
            this.olvPointListTree.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.olvPointListTree.FullRowSelect = true;
            this.olvPointListTree.GroupWithItemCountFormat = "{0} ({1} SiteID)";
            this.olvPointListTree.GroupWithItemCountSingularFormat = "{0} ({1} person)";
            this.olvPointListTree.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.olvPointListTree.HeaderUsesThemes = true;
            this.olvPointListTree.HideSelection = false;
            this.olvPointListTree.Location = new System.Drawing.Point(0, 0);
            this.olvPointListTree.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.olvPointListTree.Name = "olvPointListTree";
            this.olvPointListTree.OverlayText.Alignment = System.Drawing.ContentAlignment.BottomLeft;
            this.olvPointListTree.OverlayText.Text = "";
            this.olvPointListTree.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvPointListTree.ShowCommandMenuOnRightClick = true;
            this.olvPointListTree.ShowGroups = false;
            this.olvPointListTree.ShowHeaderInAllViews = false;
            this.olvPointListTree.ShowItemToolTips = true;
            this.olvPointListTree.Size = new System.Drawing.Size(304, 219);
            this.olvPointListTree.TabIndex = 17;
            this.olvPointListTree.UseCompatibleStateImageBehavior = false;
            this.olvPointListTree.UseHotItem = true;
            this.olvPointListTree.View = System.Windows.Forms.View.Details;
            this.olvPointListTree.SelectedIndexChanged += new System.EventHandler(this.olvPointListTree1_SelectedIndexChanged);
            // 
            // clnEllipsoidName
            // 
            this.clnEllipsoidName.Text = "Other Stations";
            this.clnEllipsoidName.Width = 285;
            // 
            // clnSemiMajorAxis
            // 
            this.clnSemiMajorAxis.Width = 0;
            // 
            // clnInverseFlattening
            // 
            this.clnInverseFlattening.Width = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.metroLabel8);
            this.panel3.Controls.Add(this.metroLabel10);
            this.panel3.Controls.Add(this.tbxInverseFlattening);
            this.panel3.Controls.Add(this.tbxSemi_Major_Axis);
            this.panel3.Controls.Add(this.metroLabel11);
            this.panel3.Controls.Add(this.tbxEllipsoidName);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 219);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(304, 110);
            this.panel3.TabIndex = 1;
            // 
            // metroLabel8
            // 
            this.metroLabel8.AutoSize = true;
            this.metroLabel8.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroLabel8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.metroLabel8.Location = new System.Drawing.Point(2, 79);
            this.metroLabel8.Name = "metroLabel8";
            this.metroLabel8.Size = new System.Drawing.Size(129, 15);
            this.metroLabel8.TabIndex = 22;
            this.metroLabel8.Text = "Inverse Flattening (1/f):";
            // 
            // metroLabel10
            // 
            this.metroLabel10.AutoSize = true;
            this.metroLabel10.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroLabel10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.metroLabel10.Location = new System.Drawing.Point(16, 49);
            this.metroLabel10.Name = "metroLabel10";
            this.metroLabel10.Size = new System.Drawing.Size(113, 15);
            this.metroLabel10.TabIndex = 18;
            this.metroLabel10.Text = "Semi-Major Axis (a):";
            // 
            // tbxInverseFlattening
            // 
            this.tbxInverseFlattening.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxInverseFlattening.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxInverseFlattening.DefaultColor = System.Drawing.Color.White;
            this.tbxInverseFlattening.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxInverseFlattening.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxInverseFlattening.ForeColor = System.Drawing.Color.Black;
            this.tbxInverseFlattening.HideSelection = false;
            this.tbxInverseFlattening.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxInverseFlattening.Location = new System.Drawing.Point(131, 75);
            this.tbxInverseFlattening.Name = "tbxInverseFlattening";
            this.tbxInverseFlattening.PasswordChar = '\0';
            this.tbxInverseFlattening.ReadOnly = true;
            this.tbxInverseFlattening.Size = new System.Drawing.Size(168, 23);
            this.tbxInverseFlattening.TabIndex = 23;
            // 
            // tbxSemi_Major_Axis
            // 
            this.tbxSemi_Major_Axis.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxSemi_Major_Axis.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxSemi_Major_Axis.DefaultColor = System.Drawing.Color.White;
            this.tbxSemi_Major_Axis.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxSemi_Major_Axis.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxSemi_Major_Axis.ForeColor = System.Drawing.Color.Black;
            this.tbxSemi_Major_Axis.HideSelection = false;
            this.tbxSemi_Major_Axis.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxSemi_Major_Axis.Location = new System.Drawing.Point(130, 45);
            this.tbxSemi_Major_Axis.Name = "tbxSemi_Major_Axis";
            this.tbxSemi_Major_Axis.PasswordChar = '\0';
            this.tbxSemi_Major_Axis.ReadOnly = true;
            this.tbxSemi_Major_Axis.Size = new System.Drawing.Size(169, 23);
            this.tbxSemi_Major_Axis.TabIndex = 20;
            // 
            // metroLabel11
            // 
            this.metroLabel11.AutoSize = true;
            this.metroLabel11.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroLabel11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.metroLabel11.Location = new System.Drawing.Point(2, 16);
            this.metroLabel11.Name = "metroLabel11";
            this.metroLabel11.Size = new System.Drawing.Size(89, 15);
            this.metroLabel11.TabIndex = 19;
            this.metroLabel11.Text = "Ellipsoid Name:";
            // 
            // tbxEllipsoidName
            // 
            this.tbxEllipsoidName.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxEllipsoidName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxEllipsoidName.DefaultColor = System.Drawing.Color.White;
            this.tbxEllipsoidName.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxEllipsoidName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxEllipsoidName.ForeColor = System.Drawing.Color.Black;
            this.tbxEllipsoidName.HideSelection = false;
            this.tbxEllipsoidName.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxEllipsoidName.Location = new System.Drawing.Point(91, 12);
            this.tbxEllipsoidName.Name = "tbxEllipsoidName";
            this.tbxEllipsoidName.PasswordChar = '\0';
            this.tbxEllipsoidName.ReadOnly = true;
            this.tbxEllipsoidName.Size = new System.Drawing.Size(209, 23);
            this.tbxEllipsoidName.TabIndex = 21;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(304, 2);
            this.panel4.TabIndex = 17;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panel2.Controls.Add(this.panel12);
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnOK);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(1, 362);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(304, 43);
            this.panel2.TabIndex = 8;
            // 
            // panel12
            // 
            this.panel12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.panel12.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel12.Location = new System.Drawing.Point(0, 0);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(304, 2);
            this.panel12.TabIndex = 16;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnCancel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnCancel.DefaultColor = System.Drawing.Color.White;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnCancel.Location = new System.Drawing.Point(205, 9);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnCancel.Size = new System.Drawing.Size(85, 25);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnOK.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnOK.DefaultColor = System.Drawing.Color.White;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOK.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnOK.Location = new System.Drawing.Point(114, 9);
            this.btnOK.Name = "btnOK";
            this.btnOK.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnOK.Size = new System.Drawing.Size(85, 25);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(1, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(304, 32);
            this.label2.TabIndex = 7;
            this.label2.Text = "Ellipsoids";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(1, 33);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(304, 2);
            this.panel5.TabIndex = 17;
            // 
            // TheEllipsoids
            // 
            this.AllowResize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(306, 406);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TheEllipsoids";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.State = MetroSuite.MetroForm.FormState.Normal;
            this.Style = MetroSuite.Design.Style.Light;
            this.Text = "Ellipsoid";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvPointListTree)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel12;
        private MetroSuite.MetroButton btnCancel;
        private MetroSuite.MetroButton btnOK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private MetroSuite.MetroLabel metroLabel8;
        private MetroSuite.MetroLabel metroLabel10;
        public MetroSuite.MetroTextbox tbxInverseFlattening;
        public MetroSuite.MetroTextbox tbxSemi_Major_Axis;
        private MetroSuite.MetroLabel metroLabel11;
        public MetroSuite.MetroTextbox tbxEllipsoidName;
        public BrightIdeasSoftware.ObjectListView olvPointListTree;
        private BrightIdeasSoftware.OLVColumn clnEllipsoidName;
        private BrightIdeasSoftware.OLVColumn clnSemiMajorAxis;
        private BrightIdeasSoftware.OLVColumn clnInverseFlattening;
        private System.Windows.Forms.Panel panel5;
    }
}