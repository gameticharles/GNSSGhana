namespace ghGPS.User_Forms
{
    partial class BaseLineChart
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseLineChart));
            this.panel1 = new System.Windows.Forms.Panel();
            this.metroLabel4 = new MetroSuite.MetroLabel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.lblProjected = new System.Windows.Forms.Label();
            this.btnProcessData = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnArrow = new System.Windows.Forms.ToolStripButton();
            this.btnZoomIn = new System.Windows.Forms.ToolStripButton();
            this.btnZoomOut = new System.Windows.Forms.ToolStripButton();
            this.btnPan = new System.Windows.Forms.ToolStripButton();
            this.btnSelect = new System.Windows.Forms.ToolStripButton();
            this.btnZoomToPoint = new System.Windows.Forms.ToolStripButton();
            this.btnZoomToExtent = new System.Windows.Forms.ToolStripButton();
            this.axMap1 = new AxMapWinGIS.AxMap();
            this.panel11 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel13 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.olvPointListTree1 = new BrightIdeasSoftware.ObjectListView();
            this.clnSiteName1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnEastings1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnNorthings1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvPointListTree = new BrightIdeasSoftware.ObjectListView();
            this.clnFile = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnSiteName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnStartTime = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnStopTime = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnEpoch = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnDataRate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnSize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnEastings = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnNorthings = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnHeight = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnAntennaHeight = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnAntennType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.panel8 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.pnlAntenna = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxAntennaType = new MetroSuite.MetroComboBox();
            this.tbxAntennaHeight = new MetroSuite.MetroTextbox();
            this.label4 = new System.Windows.Forms.Label();
            this.editSwitch = new MetroSuite.MetroSwitch();
            this.btnDiscard = new MetroSuite.MetroButton();
            this.btnApply = new MetroSuite.MetroButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.tbxSiteID = new MetroSuite.MetroTextbox();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel12.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).BeginInit();
            this.panel4.SuspendLayout();
            this.panel13.SuspendLayout();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvPointListTree1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.olvPointListTree)).BeginInit();
            this.panel8.SuspendLayout();
            this.panel6.SuspendLayout();
            this.pnlAntenna.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panel1.Controls.Add(this.metroLabel4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 459);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(851, 45);
            this.panel1.TabIndex = 3;
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.metroLabel4.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.metroLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.metroLabel4.Location = new System.Drawing.Point(245, 12);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(361, 20);
            this.metroLabel4.TabIndex = 13;
            this.metroLabel4.Text = "Note: Drag points in the list to rearrange traverse line";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnBack);
            this.panel2.Controls.Add(this.lblProjected);
            this.panel2.Controls.Add(this.btnProcessData);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 428);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(851, 31);
            this.panel2.TabIndex = 4;
            // 
            // btnCancel
            // 
            this.btnCancel.AutoEllipsis = true;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.DimGray;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(575, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(92, 31);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel ";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnBack
            // 
            this.btnBack.AutoEllipsis = true;
            this.btnBack.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnBack.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnBack.FlatAppearance.BorderSize = 0;
            this.btnBack.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btnBack.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.ForeColor = System.Drawing.Color.DimGray;
            this.btnBack.Image = ((System.Drawing.Image)(resources.GetObject("btnBack.Image")));
            this.btnBack.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBack.Location = new System.Drawing.Point(667, 0);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(92, 31);
            this.btnBack.TabIndex = 1;
            this.btnBack.Text = " Back   ";
            this.btnBack.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // lblProjected
            // 
            this.lblProjected.AutoEllipsis = true;
            this.lblProjected.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblProjected.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblProjected.ForeColor = System.Drawing.Color.DimGray;
            this.lblProjected.Location = new System.Drawing.Point(0, 0);
            this.lblProjected.Name = "lblProjected";
            this.lblProjected.Size = new System.Drawing.Size(229, 31);
            this.lblProjected.TabIndex = 1;
            this.lblProjected.Text = "  E: 654754.123 N: 745875.456";
            this.lblProjected.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnProcessData
            // 
            this.btnProcessData.AutoEllipsis = true;
            this.btnProcessData.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnProcessData.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnProcessData.FlatAppearance.BorderSize = 0;
            this.btnProcessData.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btnProcessData.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnProcessData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProcessData.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProcessData.ForeColor = System.Drawing.Color.DimGray;
            this.btnProcessData.Image = ((System.Drawing.Image)(resources.GetObject("btnProcessData.Image")));
            this.btnProcessData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnProcessData.Location = new System.Drawing.Point(759, 0);
            this.btnProcessData.Name = "btnProcessData";
            this.btnProcessData.Size = new System.Drawing.Size(92, 31);
            this.btnProcessData.TabIndex = 0;
            this.btnProcessData.Text = "Process";
            this.btnProcessData.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnProcessData.UseVisualStyleBackColor = true;
            this.btnProcessData.Click += new System.EventHandler(this.btnProcessData_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(851, 428);
            this.panel3.TabIndex = 5;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.panel12);
            this.panel5.Controls.Add(this.panel11);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(575, 428);
            this.panel5.TabIndex = 1;
            // 
            // panel12
            // 
            this.panel12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel12.Controls.Add(this.toolStrip1);
            this.panel12.Controls.Add(this.axMap1);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel12.Location = new System.Drawing.Point(10, 0);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(565, 428);
            this.panel12.TabIndex = 5;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Snow;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnArrow,
            this.btnZoomIn,
            this.btnZoomOut,
            this.btnPan,
            this.btnSelect,
            this.btnZoomToPoint,
            this.btnZoomToExtent});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(12, 10);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(29, 228);
            this.toolStrip1.TabIndex = 10;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // btnArrow
            // 
            this.btnArrow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnArrow.Image = ((System.Drawing.Image)(resources.GetObject("btnArrow.Image")));
            this.btnArrow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnArrow.Name = "btnArrow";
            this.btnArrow.Size = new System.Drawing.Size(27, 28);
            this.btnArrow.Text = "Arrow";
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomIn.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomIn.Image")));
            this.btnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(27, 28);
            this.btnZoomIn.Text = "Zoom In";
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomOut.Image")));
            this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(27, 28);
            this.btnZoomOut.Text = "Zoom Out";
            // 
            // btnPan
            // 
            this.btnPan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPan.Image = ((System.Drawing.Image)(resources.GetObject("btnPan.Image")));
            this.btnPan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPan.Name = "btnPan";
            this.btnPan.Size = new System.Drawing.Size(27, 28);
            this.btnPan.Text = "Pan";
            // 
            // btnSelect
            // 
            this.btnSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSelect.Image = ((System.Drawing.Image)(resources.GetObject("btnSelect.Image")));
            this.btnSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(27, 28);
            this.btnSelect.Text = "Select";
            // 
            // btnZoomToPoint
            // 
            this.btnZoomToPoint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomToPoint.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomToPoint.Image")));
            this.btnZoomToPoint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomToPoint.Name = "btnZoomToPoint";
            this.btnZoomToPoint.Size = new System.Drawing.Size(27, 28);
            this.btnZoomToPoint.Text = "Zoom to Point";
            // 
            // btnZoomToExtent
            // 
            this.btnZoomToExtent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomToExtent.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomToExtent.Image")));
            this.btnZoomToExtent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomToExtent.Name = "btnZoomToExtent";
            this.btnZoomToExtent.Size = new System.Drawing.Size(27, 28);
            this.btnZoomToExtent.Text = "Zoom Extent";
            // 
            // axMap1
            // 
            this.axMap1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axMap1.Enabled = true;
            this.axMap1.Location = new System.Drawing.Point(0, 0);
            this.axMap1.Name = "axMap1";
            this.axMap1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMap1.OcxState")));
            this.axMap1.Size = new System.Drawing.Size(563, 426);
            this.axMap1.TabIndex = 1;
            this.axMap1.MouseUpEvent += new AxMapWinGIS._DMapEvents_MouseUpEventHandler(this.axMap1_MouseUpEvent);
            this.axMap1.MouseMoveEvent += new AxMapWinGIS._DMapEvents_MouseMoveEventHandler(this.axMap1_MouseMoveEvent);
            this.axMap1.ChooseLayer += new AxMapWinGIS._DMapEvents_ChooseLayerEventHandler(this.axMap1_ChooseLayer);
            this.axMap1.SelectionChanged += new AxMapWinGIS._DMapEvents_SelectionChangedEventHandler(this.axMap1_SelectionChanged);
            // 
            // panel11
            // 
            this.panel11.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel11.Location = new System.Drawing.Point(0, 0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(10, 428);
            this.panel11.TabIndex = 4;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.panel13);
            this.panel4.Controls.Add(this.panel9);
            this.panel4.Controls.Add(this.panel10);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(575, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(276, 428);
            this.panel4.TabIndex = 0;
            // 
            // panel13
            // 
            this.panel13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel13.Controls.Add(this.panel7);
            this.panel13.Controls.Add(this.panel6);
            this.panel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel13.Location = new System.Drawing.Point(10, 0);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(256, 428);
            this.panel13.TabIndex = 5;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.olvPointListTree1);
            this.panel7.Controls.Add(this.olvPointListTree);
            this.panel7.Controls.Add(this.panel8);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(0, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(254, 210);
            this.panel7.TabIndex = 1;
            // 
            // olvPointListTree1
            // 
            this.olvPointListTree1.AllColumns.Add(this.clnSiteName1);
            this.olvPointListTree1.AllColumns.Add(this.clnEastings1);
            this.olvPointListTree1.AllColumns.Add(this.clnNorthings1);
            this.olvPointListTree1.AllowColumnReorder = true;
            this.olvPointListTree1.AllowDrop = true;
            this.olvPointListTree1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.olvPointListTree1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.olvPointListTree1.CellEditUseWholeCell = false;
            this.olvPointListTree1.CheckedAspectName = "";
            this.olvPointListTree1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clnSiteName1,
            this.clnEastings1,
            this.clnNorthings1});
            this.olvPointListTree1.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvPointListTree1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvPointListTree1.EmptyListMsg = "";
            this.olvPointListTree1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.olvPointListTree1.FullRowSelect = true;
            this.olvPointListTree1.GroupWithItemCountFormat = "{0} ({1} SiteID)";
            this.olvPointListTree1.GroupWithItemCountSingularFormat = "{0} ({1} person)";
            this.olvPointListTree1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.olvPointListTree1.HeaderUsesThemes = true;
            this.olvPointListTree1.HideSelection = false;
            this.olvPointListTree1.Location = new System.Drawing.Point(0, 41);
            this.olvPointListTree1.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.olvPointListTree1.Name = "olvPointListTree1";
            this.olvPointListTree1.OverlayText.Alignment = System.Drawing.ContentAlignment.BottomLeft;
            this.olvPointListTree1.OverlayText.Text = "";
            this.olvPointListTree1.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvPointListTree1.ShowCommandMenuOnRightClick = true;
            this.olvPointListTree1.ShowGroups = false;
            this.olvPointListTree1.ShowHeaderInAllViews = false;
            this.olvPointListTree1.ShowItemToolTips = true;
            this.olvPointListTree1.Size = new System.Drawing.Size(254, 169);
            this.olvPointListTree1.TabIndex = 16;
            this.olvPointListTree1.UseCompatibleStateImageBehavior = false;
            this.olvPointListTree1.UseHotItem = true;
            this.olvPointListTree1.View = System.Windows.Forms.View.Details;
            this.olvPointListTree1.Dropped += new System.EventHandler<BrightIdeasSoftware.OlvDropEventArgs>(this.olvPointListTree_Dropped);
            this.olvPointListTree1.SelectedIndexChanged += new System.EventHandler(this.olvPointListTree_SelectedIndexChanged);
            this.olvPointListTree1.Click += new System.EventHandler(this.olvPointListTree_Click);
            this.olvPointListTree1.DoubleClick += new System.EventHandler(this.olvPointListTree_DoubleClick);
            // 
            // clnSiteName1
            // 
            this.clnSiteName1.Text = "Other Stations";
            this.clnSiteName1.Width = 235;
            // 
            // clnEastings1
            // 
            this.clnEastings1.Width = 0;
            // 
            // clnNorthings1
            // 
            this.clnNorthings1.Width = 0;
            // 
            // olvPointListTree
            // 
            this.olvPointListTree.AllColumns.Add(this.clnFile);
            this.olvPointListTree.AllColumns.Add(this.clnSiteName);
            this.olvPointListTree.AllColumns.Add(this.clnStartTime);
            this.olvPointListTree.AllColumns.Add(this.clnStopTime);
            this.olvPointListTree.AllColumns.Add(this.clnEpoch);
            this.olvPointListTree.AllColumns.Add(this.clnDataRate);
            this.olvPointListTree.AllColumns.Add(this.clnSize);
            this.olvPointListTree.AllColumns.Add(this.clnEastings);
            this.olvPointListTree.AllColumns.Add(this.clnNorthings);
            this.olvPointListTree.AllColumns.Add(this.clnHeight);
            this.olvPointListTree.AllColumns.Add(this.clnAntennaHeight);
            this.olvPointListTree.AllColumns.Add(this.clnAntennType);
            this.olvPointListTree.AllowColumnReorder = true;
            this.olvPointListTree.AllowDrop = true;
            this.olvPointListTree.BackColor = System.Drawing.Color.WhiteSmoke;
            this.olvPointListTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.olvPointListTree.CellEditUseWholeCell = false;
            this.olvPointListTree.CheckedAspectName = "";
            this.olvPointListTree.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clnFile,
            this.clnSiteName,
            this.clnStartTime,
            this.clnStopTime,
            this.clnEpoch,
            this.clnDataRate,
            this.clnSize,
            this.clnEastings,
            this.clnNorthings,
            this.clnHeight,
            this.clnAntennaHeight,
            this.clnAntennType});
            this.olvPointListTree.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvPointListTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvPointListTree.EmptyListMsg = "";
            this.olvPointListTree.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.olvPointListTree.FullRowSelect = true;
            this.olvPointListTree.GroupWithItemCountFormat = "{0} ({1} site ID)";
            this.olvPointListTree.GroupWithItemCountSingularFormat = "{0} ({1} person)";
            this.olvPointListTree.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.olvPointListTree.HeaderUsesThemes = true;
            this.olvPointListTree.HideSelection = false;
            this.olvPointListTree.Location = new System.Drawing.Point(0, 41);
            this.olvPointListTree.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.olvPointListTree.Name = "olvPointListTree";
            this.olvPointListTree.OverlayText.Alignment = System.Drawing.ContentAlignment.BottomLeft;
            this.olvPointListTree.OverlayText.Text = "";
            this.olvPointListTree.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvPointListTree.ShowCommandMenuOnRightClick = true;
            this.olvPointListTree.ShowGroups = false;
            this.olvPointListTree.ShowHeaderInAllViews = false;
            this.olvPointListTree.ShowItemToolTips = true;
            this.olvPointListTree.Size = new System.Drawing.Size(254, 169);
            this.olvPointListTree.TabIndex = 9;
            this.olvPointListTree.UseCompatibleStateImageBehavior = false;
            this.olvPointListTree.UseHotItem = true;
            this.olvPointListTree.View = System.Windows.Forms.View.Details;
            this.olvPointListTree.Dropped += new System.EventHandler<BrightIdeasSoftware.OlvDropEventArgs>(this.olvPointListTree_Dropped);
            this.olvPointListTree.SelectedIndexChanged += new System.EventHandler(this.olvPointListTree_SelectedIndexChanged);
            this.olvPointListTree.Click += new System.EventHandler(this.olvPointListTree_Click);
            this.olvPointListTree.DoubleClick += new System.EventHandler(this.olvPointListTree_DoubleClick);
            // 
            // clnFile
            // 
            this.clnFile.Text = "File Path";
            this.clnFile.Width = 0;
            // 
            // clnSiteName
            // 
            this.clnSiteName.Text = "Site Name";
            this.clnSiteName.Width = 235;
            // 
            // clnStartTime
            // 
            this.clnStartTime.Width = 0;
            // 
            // clnStopTime
            // 
            this.clnStopTime.Width = 0;
            // 
            // clnEpoch
            // 
            this.clnEpoch.Width = 0;
            // 
            // clnDataRate
            // 
            this.clnDataRate.Width = 0;
            // 
            // clnSize
            // 
            this.clnSize.Width = 0;
            // 
            // clnEastings
            // 
            this.clnEastings.Text = "Eastings";
            this.clnEastings.Width = 0;
            // 
            // clnNorthings
            // 
            this.clnNorthings.Text = "Northings";
            this.clnNorthings.Width = 0;
            // 
            // clnHeight
            // 
            this.clnHeight.Width = 0;
            // 
            // clnAntennaHeight
            // 
            this.clnAntennaHeight.Text = "Antenna Height";
            this.clnAntennaHeight.Width = 0;
            // 
            // clnAntennType
            // 
            this.clnAntennType.Text = "Antenna Type";
            this.clnAntennType.Width = 0;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.label3);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(254, 41);
            this.panel8.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoEllipsis = true;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.label3.ForeColor = System.Drawing.Color.DimGray;
            this.label3.Location = new System.Drawing.Point(87, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 21);
            this.label3.TabIndex = 9;
            this.label3.Text = "Point List";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.pnlAntenna);
            this.panel6.Controls.Add(this.editSwitch);
            this.panel6.Controls.Add(this.btnDiscard);
            this.panel6.Controls.Add(this.btnApply);
            this.panel6.Controls.Add(this.label2);
            this.panel6.Controls.Add(this.label16);
            this.panel6.Controls.Add(this.tbxSiteID);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 210);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(254, 216);
            this.panel6.TabIndex = 0;
            // 
            // pnlAntenna
            // 
            this.pnlAntenna.Controls.Add(this.label1);
            this.pnlAntenna.Controls.Add(this.cbxAntennaType);
            this.pnlAntenna.Controls.Add(this.tbxAntennaHeight);
            this.pnlAntenna.Controls.Add(this.label4);
            this.pnlAntenna.Location = new System.Drawing.Point(2, 107);
            this.pnlAntenna.Name = "pnlAntenna";
            this.pnlAntenna.Size = new System.Drawing.Size(250, 55);
            this.pnlAntenna.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoEllipsis = true;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(0, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 19);
            this.label1.TabIndex = 8;
            this.label1.Text = "Antenna Height:";
            // 
            // cbxAntennaType
            // 
            this.cbxAntennaType.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxAntennaType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbxAntennaType.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxAntennaType.AutoStyle = false;
            this.cbxAntennaType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxAntennaType.DefaultColor = System.Drawing.Color.White;
            this.cbxAntennaType.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.cbxAntennaType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxAntennaType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxAntennaType.Enabled = false;
            this.cbxAntennaType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxAntennaType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbxAntennaType.ForeColor = System.Drawing.Color.Black;
            this.cbxAntennaType.FormattingEnabled = true;
            this.cbxAntennaType.ItemHeight = 22;
            this.cbxAntennaType.Items.AddRange(new object[] {
            "Pole",
            "Slant",
            "Vertical"});
            this.cbxAntennaType.Location = new System.Drawing.Point(131, 25);
            this.cbxAntennaType.Name = "cbxAntennaType";
            this.cbxAntennaType.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxAntennaType.Size = new System.Drawing.Size(117, 28);
            this.cbxAntennaType.Style = MetroSuite.Design.Style.Custom;
            this.cbxAntennaType.TabIndex = 4;
            // 
            // tbxAntennaHeight
            // 
            this.tbxAntennaHeight.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tbxAntennaHeight.AutoStyle = false;
            this.tbxAntennaHeight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxAntennaHeight.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxAntennaHeight.DefaultColor = System.Drawing.Color.White;
            this.tbxAntennaHeight.DisabledColor = System.Drawing.Color.WhiteSmoke;
            this.tbxAntennaHeight.Enabled = false;
            this.tbxAntennaHeight.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tbxAntennaHeight.ForeColor = System.Drawing.Color.DimGray;
            this.tbxAntennaHeight.HideSelection = false;
            this.tbxAntennaHeight.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxAntennaHeight.Location = new System.Drawing.Point(4, 25);
            this.tbxAntennaHeight.Name = "tbxAntennaHeight";
            this.tbxAntennaHeight.PasswordChar = '\0';
            this.tbxAntennaHeight.Size = new System.Drawing.Size(119, 28);
            this.tbxAntennaHeight.Style = MetroSuite.Design.Style.Custom;
            this.tbxAntennaHeight.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoEllipsis = true;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label4.ForeColor = System.Drawing.Color.DimGray;
            this.label4.Location = new System.Drawing.Point(127, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 19);
            this.label4.TabIndex = 8;
            this.label4.Text = "Antenna Type:";
            // 
            // editSwitch
            // 
            this.editSwitch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.editSwitch.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.editSwitch.CheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.editSwitch.Checked = true;
            this.editSwitch.DefaultColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.editSwitch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.editSwitch.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.editSwitch.Location = new System.Drawing.Point(210, 44);
            this.editSwitch.Name = "editSwitch";
            this.editSwitch.Size = new System.Drawing.Size(36, 19);
            this.editSwitch.SwitchColor = System.Drawing.Color.White;
            this.editSwitch.TabIndex = 11;
            this.editSwitch.Text = "metroSwitch1";
            this.editSwitch.CheckedChanged += new MetroSuite.MetroSwitch.CheckedChangedEventHandler(this.editSwitch_CheckedChanged);
            // 
            // btnDiscard
            // 
            this.btnDiscard.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDiscard.AutoStyle = false;
            this.btnDiscard.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnDiscard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnDiscard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDiscard.DefaultColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.btnDiscard.DisabledColor = System.Drawing.Color.LightGray;
            this.btnDiscard.Enabled = false;
            this.btnDiscard.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnDiscard.ForeColor = System.Drawing.Color.DimGray;
            this.btnDiscard.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnDiscard.Location = new System.Drawing.Point(130, 169);
            this.btnDiscard.Name = "btnDiscard";
            this.btnDiscard.PressedColor = System.Drawing.Color.Silver;
            this.btnDiscard.Size = new System.Drawing.Size(83, 29);
            this.btnDiscard.Style = MetroSuite.Design.Style.Custom;
            this.btnDiscard.TabIndex = 10;
            this.btnDiscard.Text = "Cancel";
            this.btnDiscard.Click += new System.EventHandler(this.btnDiscard_Click);
            this.btnDiscard.MouseEnter += new System.EventHandler(this.btnDiscard_MouseEnter);
            this.btnDiscard.MouseLeave += new System.EventHandler(this.btnDiscard_MouseLeave);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnApply.AutoStyle = false;
            this.btnApply.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnApply.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnApply.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApply.DefaultColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.btnApply.DisabledColor = System.Drawing.Color.LightGray;
            this.btnApply.Enabled = false;
            this.btnApply.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnApply.ForeColor = System.Drawing.Color.DimGray;
            this.btnApply.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnApply.Location = new System.Drawing.Point(41, 169);
            this.btnApply.Name = "btnApply";
            this.btnApply.PressedColor = System.Drawing.Color.Silver;
            this.btnApply.Size = new System.Drawing.Size(83, 29);
            this.btnApply.Style = MetroSuite.Design.Style.Custom;
            this.btnApply.TabIndex = 9;
            this.btnApply.Text = "Apply";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            this.btnApply.MouseEnter += new System.EventHandler(this.btnApply_MouseEnter);
            this.btnApply.MouseLeave += new System.EventHandler(this.btnApply_MouseLeave);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoEllipsis = true;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(85, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 21);
            this.label2.TabIndex = 8;
            this.label2.Text = "Edit Point";
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label16.AutoEllipsis = true;
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label16.ForeColor = System.Drawing.Color.DimGray;
            this.label16.Location = new System.Drawing.Point(1, 51);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(52, 19);
            this.label16.TabIndex = 8;
            this.label16.Text = "Site ID:";
            // 
            // tbxSiteID
            // 
            this.tbxSiteID.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tbxSiteID.AutoStyle = false;
            this.tbxSiteID.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxSiteID.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxSiteID.DefaultColor = System.Drawing.Color.White;
            this.tbxSiteID.DisabledColor = System.Drawing.Color.WhiteSmoke;
            this.tbxSiteID.Enabled = false;
            this.tbxSiteID.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tbxSiteID.ForeColor = System.Drawing.Color.DimGray;
            this.tbxSiteID.HideSelection = false;
            this.tbxSiteID.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxSiteID.Location = new System.Drawing.Point(5, 73);
            this.tbxSiteID.Name = "tbxSiteID";
            this.tbxSiteID.PasswordChar = '\0';
            this.tbxSiteID.Size = new System.Drawing.Size(244, 28);
            this.tbxSiteID.Style = MetroSuite.Design.Style.Custom;
            this.tbxSiteID.TabIndex = 7;
            // 
            // panel9
            // 
            this.panel9.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(10, 428);
            this.panel9.TabIndex = 3;
            // 
            // panel10
            // 
            this.panel10.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel10.Location = new System.Drawing.Point(266, 0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(10, 428);
            this.panel10.TabIndex = 4;
            // 
            // BaseLineChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "BaseLineChart";
            this.Size = new System.Drawing.Size(851, 504);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.panel12.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel13.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvPointListTree1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.olvPointListTree)).EndInit();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.pnlAntenna.ResumeLayout(false);
            this.pnlAntenna.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label lblProjected;
        private System.Windows.Forms.Button btnProcessData;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel6;
        public MetroSuite.MetroTextbox tbxSiteID;
        private System.Windows.Forms.Label label1;
        public MetroSuite.MetroTextbox tbxAntennaHeight;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label2;
        public MetroSuite.MetroButton btnDiscard;
        public MetroSuite.MetroButton btnApply;
        public AxMapWinGIS.AxMap axMap1;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Label label3;
        public MetroSuite.MetroSwitch editSwitch;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.Label label4;
        public MetroSuite.MetroComboBox cbxAntennaType;
        private BrightIdeasSoftware.OLVColumn clnSiteName;
        private BrightIdeasSoftware.OLVColumn clnEastings;
        private BrightIdeasSoftware.OLVColumn clnNorthings;
        private BrightIdeasSoftware.OLVColumn clnAntennaHeight;
        private BrightIdeasSoftware.OLVColumn clnAntennType;
        public BrightIdeasSoftware.ObjectListView olvPointListTree;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnArrow;
        private System.Windows.Forms.ToolStripButton btnZoomIn;
        private System.Windows.Forms.ToolStripButton btnZoomOut;
        private System.Windows.Forms.ToolStripButton btnPan;
        private System.Windows.Forms.ToolStripButton btnSelect;
        private System.Windows.Forms.ToolStripButton btnZoomToPoint;
        private System.Windows.Forms.ToolStripButton btnZoomToExtent;
        private BrightIdeasSoftware.OLVColumn clnFile;
        private BrightIdeasSoftware.OLVColumn clnStartTime;
        private BrightIdeasSoftware.OLVColumn clnStopTime;
        private BrightIdeasSoftware.OLVColumn clnEpoch;
        private BrightIdeasSoftware.OLVColumn clnDataRate;
        private BrightIdeasSoftware.OLVColumn clnSize;
        private BrightIdeasSoftware.OLVColumn clnHeight;
        private MetroSuite.MetroLabel metroLabel4;
        public BrightIdeasSoftware.ObjectListView olvPointListTree1;
        private BrightIdeasSoftware.OLVColumn clnSiteName1;
        private BrightIdeasSoftware.OLVColumn clnEastings1;
        private BrightIdeasSoftware.OLVColumn clnNorthings1;
        public System.Windows.Forms.Panel pnlAntenna;
    }
}
