namespace ghGPS.Forms
{
    partial class TransParamLists
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransParamLists));
            this.clnScale = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.olvPointListTree = new BrightIdeasSoftware.ObjectListView();
            this.clnTransName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnDx = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnDy = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnDz = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnRx = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnRy = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnRz = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnXm = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnYm = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnZm = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.panel3 = new System.Windows.Forms.Panel();
            this.tbxZm = new MetroSuite.MetroTextbox();
            this.tbxYm = new MetroSuite.MetroTextbox();
            this.tbxScale = new MetroSuite.MetroTextbox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbxRZ = new MetroSuite.MetroTextbox();
            this.tbxRX = new MetroSuite.MetroTextbox();
            this.tbxXm = new MetroSuite.MetroTextbox();
            this.tbxRY = new MetroSuite.MetroTextbox();
            this.tbxDX = new MetroSuite.MetroTextbox();
            this.tbxDZ = new MetroSuite.MetroTextbox();
            this.tbxDY = new MetroSuite.MetroTextbox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.metroLabel2 = new MetroSuite.MetroLabel();
            this.tbxTransParamName = new MetroSuite.MetroTextbox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCancel = new MetroSuite.MetroButton();
            this.btnOK = new MetroSuite.MetroButton();
            this.panel12 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvPointListTree)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // clnScale
            // 
            this.clnScale.Width = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.olvPointListTree);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.panel1.Location = new System.Drawing.Point(1, 35);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(304, 370);
            this.panel1.TabIndex = 22;
            // 
            // olvPointListTree
            // 
            this.olvPointListTree.AllColumns.Add(this.clnTransName);
            this.olvPointListTree.AllColumns.Add(this.clnDx);
            this.olvPointListTree.AllColumns.Add(this.clnDy);
            this.olvPointListTree.AllColumns.Add(this.clnDz);
            this.olvPointListTree.AllColumns.Add(this.clnRx);
            this.olvPointListTree.AllColumns.Add(this.clnRy);
            this.olvPointListTree.AllColumns.Add(this.clnRz);
            this.olvPointListTree.AllColumns.Add(this.clnScale);
            this.olvPointListTree.AllColumns.Add(this.clnXm);
            this.olvPointListTree.AllColumns.Add(this.clnYm);
            this.olvPointListTree.AllColumns.Add(this.clnZm);
            this.olvPointListTree.AllowColumnReorder = true;
            this.olvPointListTree.AllowDrop = true;
            this.olvPointListTree.BackColor = System.Drawing.Color.WhiteSmoke;
            this.olvPointListTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.olvPointListTree.CellEditUseWholeCell = false;
            this.olvPointListTree.CheckedAspectName = "";
            this.olvPointListTree.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clnTransName,
            this.clnDx,
            this.clnDy,
            this.clnDz,
            this.clnRx,
            this.clnRy,
            this.clnRz,
            this.clnScale,
            this.clnXm,
            this.clnYm,
            this.clnZm});
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
            this.olvPointListTree.Size = new System.Drawing.Size(304, 189);
            this.olvPointListTree.TabIndex = 18;
            this.olvPointListTree.UseCompatibleStateImageBehavior = false;
            this.olvPointListTree.UseHotItem = true;
            this.olvPointListTree.View = System.Windows.Forms.View.Details;
            this.olvPointListTree.SelectedIndexChanged += new System.EventHandler(this.olvPointListTree_SelectedIndexChanged);
            // 
            // clnTransName
            // 
            this.clnTransName.Width = 285;
            // 
            // clnDx
            // 
            this.clnDx.Text = "Other Stations";
            this.clnDx.Width = 0;
            // 
            // clnDy
            // 
            this.clnDy.Width = 0;
            // 
            // clnDz
            // 
            this.clnDz.Width = 0;
            // 
            // clnRx
            // 
            this.clnRx.Width = 0;
            // 
            // clnRy
            // 
            this.clnRy.Width = 0;
            // 
            // clnRz
            // 
            this.clnRz.Width = 0;
            // 
            // clnXm
            // 
            this.clnXm.Width = 0;
            // 
            // clnYm
            // 
            this.clnYm.Width = 0;
            // 
            // clnZm
            // 
            this.clnZm.Width = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.tbxZm);
            this.panel3.Controls.Add(this.tbxYm);
            this.panel3.Controls.Add(this.tbxScale);
            this.panel3.Controls.Add(this.label11);
            this.panel3.Controls.Add(this.tbxRZ);
            this.panel3.Controls.Add(this.tbxRX);
            this.panel3.Controls.Add(this.tbxXm);
            this.panel3.Controls.Add(this.tbxRY);
            this.panel3.Controls.Add(this.tbxDX);
            this.panel3.Controls.Add(this.tbxDZ);
            this.panel3.Controls.Add(this.tbxDY);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Controls.Add(this.metroLabel2);
            this.panel3.Controls.Add(this.tbxTransParamName);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 189);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(304, 181);
            this.panel3.TabIndex = 0;
            // 
            // tbxZm
            // 
            this.tbxZm.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tbxZm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxZm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxZm.DefaultColor = System.Drawing.Color.White;
            this.tbxZm.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxZm.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxZm.ForeColor = System.Drawing.Color.Black;
            this.tbxZm.HideSelection = false;
            this.tbxZm.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxZm.Location = new System.Drawing.Point(207, 153);
            this.tbxZm.Name = "tbxZm";
            this.tbxZm.PasswordChar = '\0';
            this.tbxZm.ReadOnly = true;
            this.tbxZm.Size = new System.Drawing.Size(82, 23);
            this.tbxZm.TabIndex = 43;
            this.tbxZm.Text = "0";
            this.tbxZm.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbxYm
            // 
            this.tbxYm.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tbxYm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxYm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxYm.DefaultColor = System.Drawing.Color.White;
            this.tbxYm.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxYm.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxYm.ForeColor = System.Drawing.Color.Black;
            this.tbxYm.HideSelection = false;
            this.tbxYm.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxYm.Location = new System.Drawing.Point(207, 127);
            this.tbxYm.Name = "tbxYm";
            this.tbxYm.PasswordChar = '\0';
            this.tbxYm.ReadOnly = true;
            this.tbxYm.Size = new System.Drawing.Size(82, 23);
            this.tbxYm.TabIndex = 41;
            this.tbxYm.Text = "0";
            this.tbxYm.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbxScale
            // 
            this.tbxScale.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tbxScale.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxScale.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxScale.DefaultColor = System.Drawing.Color.White;
            this.tbxScale.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxScale.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxScale.ForeColor = System.Drawing.Color.Black;
            this.tbxScale.HideSelection = false;
            this.tbxScale.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxScale.Location = new System.Drawing.Point(207, 75);
            this.tbxScale.Name = "tbxScale";
            this.tbxScale.PasswordChar = '\0';
            this.tbxScale.ReadOnly = true;
            this.tbxScale.Size = new System.Drawing.Size(82, 23);
            this.tbxScale.TabIndex = 39;
            this.tbxScale.Text = "0";
            this.tbxScale.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label11.ForeColor = System.Drawing.Color.Gray;
            this.label11.Location = new System.Drawing.Point(132, 79);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(73, 15);
            this.label11.TabIndex = 29;
            this.label11.Text = "Scale (ppm):";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbxRZ
            // 
            this.tbxRZ.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tbxRZ.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxRZ.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxRZ.DefaultColor = System.Drawing.Color.White;
            this.tbxRZ.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxRZ.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxRZ.ForeColor = System.Drawing.Color.Black;
            this.tbxRZ.HideSelection = false;
            this.tbxRZ.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxRZ.Location = new System.Drawing.Point(207, 49);
            this.tbxRZ.Name = "tbxRZ";
            this.tbxRZ.PasswordChar = '\0';
            this.tbxRZ.ReadOnly = true;
            this.tbxRZ.Size = new System.Drawing.Size(82, 23);
            this.tbxRZ.TabIndex = 40;
            this.tbxRZ.Text = "0";
            this.tbxRZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbxRX
            // 
            this.tbxRX.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tbxRX.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxRX.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxRX.DefaultColor = System.Drawing.Color.White;
            this.tbxRX.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxRX.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxRX.ForeColor = System.Drawing.Color.Black;
            this.tbxRX.HideSelection = false;
            this.tbxRX.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxRX.Location = new System.Drawing.Point(39, 127);
            this.tbxRX.Name = "tbxRX";
            this.tbxRX.PasswordChar = '\0';
            this.tbxRX.ReadOnly = true;
            this.tbxRX.Size = new System.Drawing.Size(82, 23);
            this.tbxRX.TabIndex = 38;
            this.tbxRX.Text = "0";
            this.tbxRX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbxXm
            // 
            this.tbxXm.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tbxXm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxXm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxXm.DefaultColor = System.Drawing.Color.White;
            this.tbxXm.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxXm.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxXm.ForeColor = System.Drawing.Color.Black;
            this.tbxXm.HideSelection = false;
            this.tbxXm.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxXm.Location = new System.Drawing.Point(207, 101);
            this.tbxXm.Name = "tbxXm";
            this.tbxXm.PasswordChar = '\0';
            this.tbxXm.ReadOnly = true;
            this.tbxXm.Size = new System.Drawing.Size(82, 23);
            this.tbxXm.TabIndex = 37;
            this.tbxXm.Text = "0";
            this.tbxXm.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbxRY
            // 
            this.tbxRY.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tbxRY.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxRY.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxRY.DefaultColor = System.Drawing.Color.White;
            this.tbxRY.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxRY.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxRY.ForeColor = System.Drawing.Color.Black;
            this.tbxRY.HideSelection = false;
            this.tbxRY.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxRY.Location = new System.Drawing.Point(39, 153);
            this.tbxRY.Name = "tbxRY";
            this.tbxRY.PasswordChar = '\0';
            this.tbxRY.ReadOnly = true;
            this.tbxRY.Size = new System.Drawing.Size(82, 23);
            this.tbxRY.TabIndex = 36;
            this.tbxRY.Text = "0";
            this.tbxRY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbxDX
            // 
            this.tbxDX.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tbxDX.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxDX.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxDX.DefaultColor = System.Drawing.Color.White;
            this.tbxDX.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxDX.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxDX.ForeColor = System.Drawing.Color.Black;
            this.tbxDX.HideSelection = false;
            this.tbxDX.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxDX.Location = new System.Drawing.Point(39, 49);
            this.tbxDX.Name = "tbxDX";
            this.tbxDX.PasswordChar = '\0';
            this.tbxDX.ReadOnly = true;
            this.tbxDX.Size = new System.Drawing.Size(82, 23);
            this.tbxDX.TabIndex = 35;
            this.tbxDX.Text = "0";
            this.tbxDX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbxDZ
            // 
            this.tbxDZ.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tbxDZ.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxDZ.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxDZ.DefaultColor = System.Drawing.Color.White;
            this.tbxDZ.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxDZ.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxDZ.ForeColor = System.Drawing.Color.Black;
            this.tbxDZ.HideSelection = false;
            this.tbxDZ.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxDZ.Location = new System.Drawing.Point(39, 101);
            this.tbxDZ.Name = "tbxDZ";
            this.tbxDZ.PasswordChar = '\0';
            this.tbxDZ.ReadOnly = true;
            this.tbxDZ.Size = new System.Drawing.Size(82, 23);
            this.tbxDZ.TabIndex = 42;
            this.tbxDZ.Text = "0";
            this.tbxDZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbxDY
            // 
            this.tbxDY.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tbxDY.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxDY.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxDY.DefaultColor = System.Drawing.Color.White;
            this.tbxDY.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxDY.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxDY.ForeColor = System.Drawing.Color.Black;
            this.tbxDY.HideSelection = false;
            this.tbxDY.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxDY.Location = new System.Drawing.Point(39, 75);
            this.tbxDY.Name = "tbxDY";
            this.tbxDY.PasswordChar = '\0';
            this.tbxDY.ReadOnly = true;
            this.tbxDY.Size = new System.Drawing.Size(82, 23);
            this.tbxDY.TabIndex = 34;
            this.tbxDY.Text = "0";
            this.tbxDY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Location = new System.Drawing.Point(16, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 15);
            this.label1.TabIndex = 32;
            this.label1.Text = "Dx:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label4.ForeColor = System.Drawing.Color.Gray;
            this.label4.Location = new System.Drawing.Point(16, 131);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(22, 15);
            this.label4.TabIndex = 31;
            this.label4.Text = "Rx:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label9.ForeColor = System.Drawing.Color.Gray;
            this.label9.Location = new System.Drawing.Point(179, 157);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 15);
            this.label9.TabIndex = 30;
            this.label9.Text = "Zm:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label7.ForeColor = System.Drawing.Color.Gray;
            this.label7.Location = new System.Drawing.Point(179, 105);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 15);
            this.label7.TabIndex = 28;
            this.label7.Text = "Xm:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label6.ForeColor = System.Drawing.Color.Gray;
            this.label6.Location = new System.Drawing.Point(184, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(22, 15);
            this.label6.TabIndex = 27;
            this.label6.Text = "Rz:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label3.ForeColor = System.Drawing.Color.Gray;
            this.label3.Location = new System.Drawing.Point(16, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 15);
            this.label3.TabIndex = 26;
            this.label3.Text = "Dz:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label5.ForeColor = System.Drawing.Color.Gray;
            this.label5.Location = new System.Drawing.Point(16, 79);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 15);
            this.label5.TabIndex = 25;
            this.label5.Text = "Dy:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label8.ForeColor = System.Drawing.Color.Gray;
            this.label8.Location = new System.Drawing.Point(16, 157);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(23, 15);
            this.label8.TabIndex = 33;
            this.label8.Text = "Ry:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label10.ForeColor = System.Drawing.Color.Gray;
            this.label10.Location = new System.Drawing.Point(179, 131);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(27, 15);
            this.label10.TabIndex = 24;
            this.label10.Text = "Ym:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.metroLabel2.Location = new System.Drawing.Point(11, 4);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(45, 15);
            this.metroLabel2.TabIndex = 22;
            this.metroLabel2.Text = " Name:";
            // 
            // tbxTransParamName
            // 
            this.tbxTransParamName.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxTransParamName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxTransParamName.DefaultColor = System.Drawing.Color.White;
            this.tbxTransParamName.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxTransParamName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxTransParamName.ForeColor = System.Drawing.Color.Black;
            this.tbxTransParamName.HideSelection = false;
            this.tbxTransParamName.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxTransParamName.Location = new System.Drawing.Point(16, 21);
            this.tbxTransParamName.Name = "tbxTransParamName";
            this.tbxTransParamName.PasswordChar = '\0';
            this.tbxTransParamName.ReadOnly = true;
            this.tbxTransParamName.Size = new System.Drawing.Size(279, 23);
            this.tbxTransParamName.TabIndex = 23;
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
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnOK);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(1, 405);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(304, 43);
            this.panel2.TabIndex = 24;
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
            // panel12
            // 
            this.panel12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.panel12.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel12.Location = new System.Drawing.Point(1, 33);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(304, 2);
            this.panel12.TabIndex = 25;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(1, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(304, 32);
            this.label2.TabIndex = 23;
            this.label2.Text = "WGS Conversion Info";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TransParamLists
            // 
            this.AllowResize = false;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(306, 449);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel12);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TransParamLists";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.State = MetroSuite.MetroForm.FormState.Normal;
            this.Style = MetroSuite.Design.Style.Light;
            this.Text = "TransParamLists";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvPointListTree)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.OLVColumn clnScale;
        private System.Windows.Forms.Panel panel1;
        public BrightIdeasSoftware.ObjectListView olvPointListTree;
        private BrightIdeasSoftware.OLVColumn clnTransName;
        private BrightIdeasSoftware.OLVColumn clnDx;
        private BrightIdeasSoftware.OLVColumn clnDy;
        private BrightIdeasSoftware.OLVColumn clnDz;
        private BrightIdeasSoftware.OLVColumn clnRx;
        private BrightIdeasSoftware.OLVColumn clnRy;
        private BrightIdeasSoftware.OLVColumn clnRz;
        private BrightIdeasSoftware.OLVColumn clnXm;
        private BrightIdeasSoftware.OLVColumn clnYm;
        private BrightIdeasSoftware.OLVColumn clnZm;
        private System.Windows.Forms.Panel panel3;
        public MetroSuite.MetroTextbox tbxZm;
        public MetroSuite.MetroTextbox tbxYm;
        public MetroSuite.MetroTextbox tbxScale;
        public System.Windows.Forms.Label label11;
        public MetroSuite.MetroTextbox tbxRZ;
        public MetroSuite.MetroTextbox tbxRX;
        public MetroSuite.MetroTextbox tbxXm;
        public MetroSuite.MetroTextbox tbxRY;
        public MetroSuite.MetroTextbox tbxDX;
        public MetroSuite.MetroTextbox tbxDZ;
        public MetroSuite.MetroTextbox tbxDY;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label9;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label label8;
        public System.Windows.Forms.Label label10;
        private MetroSuite.MetroLabel metroLabel2;
        public MetroSuite.MetroTextbox tbxTransParamName;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel2;
        private MetroSuite.MetroButton btnCancel;
        private MetroSuite.MetroButton btnOK;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Label label2;
    }
}