namespace ghGPS.Forms
{
    partial class CoordinateSystem
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CoordinateSystem));
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.treeCRS = new BrightIdeasSoftware.TreeListView();
            this.clnCRS = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnAutorityCode = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.cnlCSRCodeName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.panel9 = new System.Windows.Forms.Panel();
            this.tbxFilter = new MetroSuite.MetroTextbox();
            this.metroLabel2 = new MetroSuite.MetroLabel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rtbxSelectedCRSDetails = new System.Windows.Forms.RichTextBox();
            this.panel8 = new System.Windows.Forms.Panel();
            this.tbxSelectedCRSName = new MetroSuite.MetroTextbox();
            this.metroLabel1 = new MetroSuite.MetroLabel();
            this.lblStatus = new MetroSuite.MetroLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new MetroSuite.MetroButton();
            this.btnOK = new MetroSuite.MetroButton();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.pnlExport = new System.Windows.Forms.Panel();
            this.btnUserDefinedCoord = new System.Windows.Forms.Button();
            this.pnlProjectedCoord = new System.Windows.Forms.Panel();
            this.btnProjectedCoord = new System.Windows.Forms.Button();
            this.pnlGeographicCoord = new System.Windows.Forms.Panel();
            this.btnGeographicCoord = new System.Windows.Forms.Button();
            this.btnCreateGrisSystem = new System.Windows.Forms.Button();
            this.chbxEsri = new MetroSuite.MetroSwitch();
            this.chbxReadable = new MetroSuite.MetroSwitch();
            this.editSwitch = new MetroSuite.MetroSwitch();
            this.cbxHotItem = new MetroSuite.MetroComboBox();
            this.cbxExpander = new MetroSuite.MetroComboBox();
            this.nudEpsgCode = new MetroSuite.MetroLabel();
            this.metroLabel7 = new MetroSuite.MetroLabel();
            this.metroLabel8 = new MetroSuite.MetroLabel();
            this.metroLabel6 = new MetroSuite.MetroLabel();
            this.metroLabel5 = new MetroSuite.MetroLabel();
            this.metroLabel4 = new MetroSuite.MetroLabel();
            this.metroLabel3 = new MetroSuite.MetroLabel();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeCRS)).BeginInit();
            this.panel9.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.pnlExport.SuspendLayout();
            this.pnlProjectedCoord.SuspendLayout();
            this.pnlGeographicCoord.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.panel6);
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(1, 1);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(622, 536);
            this.panel3.TabIndex = 7;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.DarkGray;
            this.panel4.Controls.Add(this.panel7);
            this.panel4.Controls.Add(this.panel2);
            this.panel4.Controls.Add(this.panel1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(194, 50);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(1);
            this.panel4.Size = new System.Drawing.Size(428, 486);
            this.panel4.TabIndex = 4;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.treeCRS);
            this.panel7.Controls.Add(this.panel9);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(1, 1);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(426, 287);
            this.panel7.TabIndex = 2;
            // 
            // treeCRS
            // 
            this.treeCRS.AllColumns.Add(this.clnCRS);
            this.treeCRS.AllColumns.Add(this.clnAutorityCode);
            this.treeCRS.AllColumns.Add(this.cnlCSRCodeName);
            this.treeCRS.CellEditUseWholeCell = false;
            this.treeCRS.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clnCRS,
            this.clnAutorityCode,
            this.cnlCSRCodeName});
            this.treeCRS.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeCRS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeCRS.EmptyListMsg = "No match found";
            this.treeCRS.EmptyListMsgFont = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeCRS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.treeCRS.FullRowSelect = true;
            this.treeCRS.HideSelection = false;
            this.treeCRS.Location = new System.Drawing.Point(0, 29);
            this.treeCRS.MultiSelect = false;
            this.treeCRS.Name = "treeCRS";
            this.treeCRS.ShowGroups = false;
            this.treeCRS.Size = new System.Drawing.Size(426, 258);
            this.treeCRS.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.treeCRS.TabIndex = 9;
            this.treeCRS.UseCompatibleStateImageBehavior = false;
            this.treeCRS.UseFilterIndicator = true;
            this.treeCRS.UseFiltering = true;
            this.treeCRS.UseHotItem = true;
            this.treeCRS.View = System.Windows.Forms.View.Details;
            this.treeCRS.VirtualMode = true;
            this.treeCRS.SelectionChanged += new System.EventHandler(this.treeCRS_SelectionChanged);
            // 
            // clnCRS
            // 
            this.clnCRS.Text = "Coordinate Reference System";
            this.clnCRS.Width = 300;
            // 
            // clnAutorityCode
            // 
            this.clnAutorityCode.Text = "Authority ID";
            this.clnAutorityCode.Width = 100;
            // 
            // cnlCSRCodeName
            // 
            this.cnlCSRCodeName.Width = 0;
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panel9.Controls.Add(this.tbxFilter);
            this.panel9.Controls.Add(this.metroLabel2);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(426, 29);
            this.panel9.TabIndex = 6;
            // 
            // tbxFilter
            // 
            this.tbxFilter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxFilter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxFilter.DefaultColor = System.Drawing.Color.White;
            this.tbxFilter.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxFilter.HideSelection = false;
            this.tbxFilter.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.tbxFilter.Location = new System.Drawing.Point(47, 3);
            this.tbxFilter.Name = "tbxFilter";
            this.tbxFilter.PasswordChar = '\0';
            this.tbxFilter.Size = new System.Drawing.Size(365, 23);
            this.tbxFilter.TabIndex = 3;
            this.tbxFilter.TextChanged += new System.EventHandler(this.tbxFilter_TextChanged);
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.metroLabel2.Location = new System.Drawing.Point(5, 7);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(36, 15);
            this.metroLabel2.TabIndex = 2;
            this.metroLabel2.Text = "Filter:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rtbxSelectedCRSDetails);
            this.panel2.Controls.Add(this.panel8);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(1, 288);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(426, 154);
            this.panel2.TabIndex = 1;
            // 
            // rtbxSelectedCRSDetails
            // 
            this.rtbxSelectedCRSDetails.BackColor = System.Drawing.Color.White;
            this.rtbxSelectedCRSDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbxSelectedCRSDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxSelectedCRSDetails.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbxSelectedCRSDetails.Location = new System.Drawing.Point(0, 69);
            this.rtbxSelectedCRSDetails.Name = "rtbxSelectedCRSDetails";
            this.rtbxSelectedCRSDetails.ReadOnly = true;
            this.rtbxSelectedCRSDetails.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.rtbxSelectedCRSDetails.Size = new System.Drawing.Size(426, 85);
            this.rtbxSelectedCRSDetails.TabIndex = 4;
            this.rtbxSelectedCRSDetails.Text = "";
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panel8.Controls.Add(this.tbxSelectedCRSName);
            this.panel8.Controls.Add(this.metroLabel1);
            this.panel8.Controls.Add(this.lblStatus);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(426, 69);
            this.panel8.TabIndex = 5;
            // 
            // tbxSelectedCRSName
            // 
            this.tbxSelectedCRSName.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxSelectedCRSName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxSelectedCRSName.DefaultColor = System.Drawing.Color.White;
            this.tbxSelectedCRSName.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxSelectedCRSName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxSelectedCRSName.HideSelection = false;
            this.tbxSelectedCRSName.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.tbxSelectedCRSName.Location = new System.Drawing.Point(85, 40);
            this.tbxSelectedCRSName.Name = "tbxSelectedCRSName";
            this.tbxSelectedCRSName.PasswordChar = '\0';
            this.tbxSelectedCRSName.ReadOnly = true;
            this.tbxSelectedCRSName.Size = new System.Drawing.Size(327, 23);
            this.tbxSelectedCRSName.TabIndex = 3;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.metroLabel1.Location = new System.Drawing.Point(5, 44);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(78, 15);
            this.metroLabel1.TabIndex = 2;
            this.metroLabel1.Text = "Selected CRS:";
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblStatus.Location = new System.Drawing.Point(0, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(426, 35);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(1, 442);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(426, 43);
            this.panel1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnCancel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnCancel.DefaultColor = System.Drawing.Color.White;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnCancel.Location = new System.Drawing.Point(318, 9);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnCancel.Size = new System.Drawing.Size(85, 25);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnOK.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnOK.DefaultColor = System.Drawing.Color.White;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOK.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnOK.Location = new System.Drawing.Point(227, 9);
            this.btnOK.Name = "btnOK";
            this.btnOK.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnOK.Size = new System.Drawing.Size(85, 25);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panel6.Controls.Add(this.label2);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(194, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(428, 50);
            this.panel6.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(428, 50);
            this.label2.TabIndex = 2;
            this.label2.Text = "Coordinate Reference System(CRS)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.panel5.Controls.Add(this.pnlButtons);
            this.panel5.Controls.Add(this.btnCreateGrisSystem);
            this.panel5.Controls.Add(this.chbxEsri);
            this.panel5.Controls.Add(this.chbxReadable);
            this.panel5.Controls.Add(this.editSwitch);
            this.panel5.Controls.Add(this.cbxHotItem);
            this.panel5.Controls.Add(this.cbxExpander);
            this.panel5.Controls.Add(this.nudEpsgCode);
            this.panel5.Controls.Add(this.metroLabel7);
            this.panel5.Controls.Add(this.metroLabel8);
            this.panel5.Controls.Add(this.metroLabel6);
            this.panel5.Controls.Add(this.metroLabel5);
            this.panel5.Controls.Add(this.metroLabel4);
            this.panel5.Controls.Add(this.metroLabel3);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(194, 536);
            this.panel5.TabIndex = 5;
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.pnlExport);
            this.pnlButtons.Controls.Add(this.pnlProjectedCoord);
            this.pnlButtons.Controls.Add(this.pnlGeographicCoord);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButtons.Location = new System.Drawing.Point(0, 0);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(194, 142);
            this.pnlButtons.TabIndex = 15;
            // 
            // pnlExport
            // 
            this.pnlExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.pnlExport.Controls.Add(this.btnUserDefinedCoord);
            this.pnlExport.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlExport.Location = new System.Drawing.Point(0, 92);
            this.pnlExport.Name = "pnlExport";
            this.pnlExport.Size = new System.Drawing.Size(194, 46);
            this.pnlExport.TabIndex = 7;
            // 
            // btnUserDefinedCoord
            // 
            this.btnUserDefinedCoord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnUserDefinedCoord.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnUserDefinedCoord.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnUserDefinedCoord.FlatAppearance.BorderSize = 0;
            this.btnUserDefinedCoord.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnUserDefinedCoord.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnUserDefinedCoord.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnUserDefinedCoord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUserDefinedCoord.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnUserDefinedCoord.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnUserDefinedCoord.Image = global::ghGPS.Properties.Resources.icons8_User_26px_1;
            this.btnUserDefinedCoord.Location = new System.Drawing.Point(0, 0);
            this.btnUserDefinedCoord.Name = "btnUserDefinedCoord";
            this.btnUserDefinedCoord.Size = new System.Drawing.Size(194, 45);
            this.btnUserDefinedCoord.TabIndex = 0;
            this.btnUserDefinedCoord.Text = "    User Defined       ";
            this.btnUserDefinedCoord.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUserDefinedCoord.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUserDefinedCoord.UseVisualStyleBackColor = false;
            this.btnUserDefinedCoord.Click += new System.EventHandler(this.btnGeographicCoord_Click);
            // 
            // pnlProjectedCoord
            // 
            this.pnlProjectedCoord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.pnlProjectedCoord.Controls.Add(this.btnProjectedCoord);
            this.pnlProjectedCoord.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlProjectedCoord.Location = new System.Drawing.Point(0, 46);
            this.pnlProjectedCoord.Name = "pnlProjectedCoord";
            this.pnlProjectedCoord.Size = new System.Drawing.Size(194, 46);
            this.pnlProjectedCoord.TabIndex = 1;
            // 
            // btnProjectedCoord
            // 
            this.btnProjectedCoord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnProjectedCoord.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnProjectedCoord.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnProjectedCoord.FlatAppearance.BorderSize = 0;
            this.btnProjectedCoord.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnProjectedCoord.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnProjectedCoord.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnProjectedCoord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProjectedCoord.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnProjectedCoord.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnProjectedCoord.Image = global::ghGPS.Properties.Resources.icons8_Grid_26px;
            this.btnProjectedCoord.Location = new System.Drawing.Point(0, 0);
            this.btnProjectedCoord.Name = "btnProjectedCoord";
            this.btnProjectedCoord.Size = new System.Drawing.Size(194, 45);
            this.btnProjectedCoord.TabIndex = 0;
            this.btnProjectedCoord.Text = "    Projected             ";
            this.btnProjectedCoord.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnProjectedCoord.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnProjectedCoord.UseVisualStyleBackColor = false;
            this.btnProjectedCoord.Click += new System.EventHandler(this.btnGeographicCoord_Click);
            // 
            // pnlGeographicCoord
            // 
            this.pnlGeographicCoord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.pnlGeographicCoord.Controls.Add(this.btnGeographicCoord);
            this.pnlGeographicCoord.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGeographicCoord.Location = new System.Drawing.Point(0, 0);
            this.pnlGeographicCoord.Name = "pnlGeographicCoord";
            this.pnlGeographicCoord.Size = new System.Drawing.Size(194, 46);
            this.pnlGeographicCoord.TabIndex = 6;
            // 
            // btnGeographicCoord
            // 
            this.btnGeographicCoord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnGeographicCoord.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnGeographicCoord.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnGeographicCoord.FlatAppearance.BorderSize = 0;
            this.btnGeographicCoord.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnGeographicCoord.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnGeographicCoord.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnGeographicCoord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGeographicCoord.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnGeographicCoord.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnGeographicCoord.Image = global::ghGPS.Properties.Resources.icons8_Globe_26px;
            this.btnGeographicCoord.Location = new System.Drawing.Point(0, 0);
            this.btnGeographicCoord.Name = "btnGeographicCoord";
            this.btnGeographicCoord.Size = new System.Drawing.Size(194, 45);
            this.btnGeographicCoord.TabIndex = 0;
            this.btnGeographicCoord.Text = "    Geographic         ";
            this.btnGeographicCoord.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGeographicCoord.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGeographicCoord.UseVisualStyleBackColor = false;
            this.btnGeographicCoord.Click += new System.EventHandler(this.btnGeographicCoord_Click);
            // 
            // btnCreateGrisSystem
            // 
            this.btnCreateGrisSystem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(156)))), ((int)(((byte)(222)))));
            this.btnCreateGrisSystem.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnCreateGrisSystem.FlatAppearance.BorderSize = 0;
            this.btnCreateGrisSystem.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnCreateGrisSystem.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnCreateGrisSystem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateGrisSystem.ForeColor = System.Drawing.Color.LightGray;
            this.btnCreateGrisSystem.Location = new System.Drawing.Point(17, 233);
            this.btnCreateGrisSystem.Name = "btnCreateGrisSystem";
            this.btnCreateGrisSystem.Size = new System.Drawing.Size(160, 32);
            this.btnCreateGrisSystem.TabIndex = 14;
            this.btnCreateGrisSystem.Text = "Create Custom System";
            this.btnCreateGrisSystem.UseVisualStyleBackColor = false;
            this.btnCreateGrisSystem.Click += new System.EventHandler(this.btnCreateGrisSystem_Click);
            // 
            // chbxEsri
            // 
            this.chbxEsri.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chbxEsri.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.chbxEsri.CheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.chbxEsri.DefaultColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.chbxEsri.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chbxEsri.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.chbxEsri.Location = new System.Drawing.Point(132, 372);
            this.chbxEsri.Name = "chbxEsri";
            this.chbxEsri.Size = new System.Drawing.Size(30, 19);
            this.chbxEsri.SwitchColor = System.Drawing.Color.White;
            this.chbxEsri.TabIndex = 12;
            this.chbxEsri.Text = "metroSwitch1";
            this.chbxEsri.CheckedChanged += new MetroSuite.MetroSwitch.CheckedChangedEventHandler(this.chbEsri_CheckedChanged);
            // 
            // chbxReadable
            // 
            this.chbxReadable.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chbxReadable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.chbxReadable.CheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.chbxReadable.Checked = true;
            this.chbxReadable.DefaultColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.chbxReadable.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chbxReadable.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.chbxReadable.Location = new System.Drawing.Point(131, 342);
            this.chbxReadable.Name = "chbxReadable";
            this.chbxReadable.Size = new System.Drawing.Size(36, 19);
            this.chbxReadable.SwitchColor = System.Drawing.Color.White;
            this.chbxReadable.TabIndex = 12;
            this.chbxReadable.Text = "metroSwitch1";
            this.chbxReadable.CheckedChanged += new MetroSuite.MetroSwitch.CheckedChangedEventHandler(this.chbxReadable_CheckedChanged);
            // 
            // editSwitch
            // 
            this.editSwitch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.editSwitch.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.editSwitch.CheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.editSwitch.Checked = true;
            this.editSwitch.DefaultColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.editSwitch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.editSwitch.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.editSwitch.Location = new System.Drawing.Point(131, 311);
            this.editSwitch.Name = "editSwitch";
            this.editSwitch.Size = new System.Drawing.Size(36, 19);
            this.editSwitch.SwitchColor = System.Drawing.Color.White;
            this.editSwitch.TabIndex = 12;
            this.editSwitch.Text = "metroSwitch1";
            this.editSwitch.CheckedChanged += new MetroSuite.MetroSwitch.CheckedChangedEventHandler(this.GridLinesSwitch_CheckedChanged);
            // 
            // cbxHotItem
            // 
            this.cbxHotItem.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxHotItem.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxHotItem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxHotItem.DefaultColor = System.Drawing.Color.White;
            this.cbxHotItem.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.cbxHotItem.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxHotItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxHotItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxHotItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cbxHotItem.FormattingEnabled = true;
            this.cbxHotItem.Items.AddRange(new object[] {
            "None",
            "Text Color",
            "Border",
            "Translucent",
            "Lightbox"});
            this.cbxHotItem.Location = new System.Drawing.Point(66, 407);
            this.cbxHotItem.Name = "cbxHotItem";
            this.cbxHotItem.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxHotItem.Size = new System.Drawing.Size(121, 24);
            this.cbxHotItem.TabIndex = 9;
            this.cbxHotItem.SelectedIndexChanged += new System.EventHandler(this.cbxHotItem_SelectedIndexChanged);
            // 
            // cbxExpander
            // 
            this.cbxExpander.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxExpander.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxExpander.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxExpander.DefaultColor = System.Drawing.Color.White;
            this.cbxExpander.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.cbxExpander.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxExpander.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxExpander.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxExpander.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cbxExpander.FormattingEnabled = true;
            this.cbxExpander.Items.AddRange(new object[] {
            "None",
            "Plus/Minus",
            "Triangles"});
            this.cbxExpander.Location = new System.Drawing.Point(66, 448);
            this.cbxExpander.Name = "cbxExpander";
            this.cbxExpander.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxExpander.Size = new System.Drawing.Size(121, 24);
            this.cbxExpander.TabIndex = 9;
            this.cbxExpander.SelectedIndexChanged += new System.EventHandler(this.cbxExpander_SelectedIndexChanged);
            // 
            // nudEpsgCode
            // 
            this.nudEpsgCode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.nudEpsgCode.ForeColor = System.Drawing.Color.White;
            this.nudEpsgCode.Location = new System.Drawing.Point(72, 508);
            this.nudEpsgCode.Name = "nudEpsgCode";
            this.nudEpsgCode.Size = new System.Drawing.Size(116, 15);
            this.nudEpsgCode.TabIndex = 2;
            // 
            // metroLabel7
            // 
            this.metroLabel7.AutoSize = true;
            this.metroLabel7.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroLabel7.ForeColor = System.Drawing.Color.White;
            this.metroLabel7.Location = new System.Drawing.Point(5, 374);
            this.metroLabel7.Name = "metroLabel7";
            this.metroLabel7.Size = new System.Drawing.Size(124, 15);
            this.metroLabel7.TabIndex = 2;
            this.metroLabel7.Text = "ESRI (otherwise Proj4):";
            // 
            // metroLabel8
            // 
            this.metroLabel8.AutoSize = true;
            this.metroLabel8.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroLabel8.ForeColor = System.Drawing.Color.White;
            this.metroLabel8.Location = new System.Drawing.Point(38, 344);
            this.metroLabel8.Name = "metroLabel8";
            this.metroLabel8.Size = new System.Drawing.Size(91, 15);
            this.metroLabel8.TabIndex = 2;
            this.metroLabel8.Text = "Readable string:";
            // 
            // metroLabel6
            // 
            this.metroLabel6.AutoSize = true;
            this.metroLabel6.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroLabel6.ForeColor = System.Drawing.Color.White;
            this.metroLabel6.Location = new System.Drawing.Point(70, 313);
            this.metroLabel6.Name = "metroLabel6";
            this.metroLabel6.Size = new System.Drawing.Size(59, 15);
            this.metroLabel6.TabIndex = 2;
            this.metroLabel6.Text = "Grid lines:";
            // 
            // metroLabel5
            // 
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroLabel5.ForeColor = System.Drawing.Color.White;
            this.metroLabel5.Location = new System.Drawing.Point(5, 412);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(57, 15);
            this.metroLabel5.TabIndex = 2;
            this.metroLabel5.Text = "Hot Item:";
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroLabel4.ForeColor = System.Drawing.Color.White;
            this.metroLabel4.Location = new System.Drawing.Point(5, 453);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(58, 15);
            this.metroLabel4.TabIndex = 2;
            this.metroLabel4.Text = "Expander:";
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroLabel3.ForeColor = System.Drawing.Color.White;
            this.metroLabel3.Location = new System.Drawing.Point(5, 508);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(68, 15);
            this.metroLabel3.TabIndex = 2;
            this.metroLabel3.Text = "EPGS Code:";
            // 
            // CoordinateSystem
            // 
            this.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.AllowResize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(624, 538);
            this.Controls.Add(this.panel3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CoordinateSystem";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.State = MetroSuite.MetroForm.FormState.Custom;
            this.Style = MetroSuite.Design.Style.Light;
            this.Text = "CoordinateSystem";
            this.Load += new System.EventHandler(this.CoordinateSystem_Load);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeCRS)).EndInit();
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlExport.ResumeLayout(false);
            this.pnlProjectedCoord.ResumeLayout(false);
            this.pnlGeographicCoord.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel pnlExport;
        private System.Windows.Forms.Button btnUserDefinedCoord;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel7;
        public System.Windows.Forms.RichTextBox rtbxSelectedCRSDetails;
        private System.Windows.Forms.Panel panel8;
        private MetroSuite.MetroLabel metroLabel1;
        private MetroSuite.MetroButton btnCancel;
        private MetroSuite.MetroButton btnOK;
        private System.Windows.Forms.Panel panel9;
        private MetroSuite.MetroTextbox tbxFilter;
        private MetroSuite.MetroLabel metroLabel2;
        private MetroSuite.MetroLabel nudEpsgCode;
        private MetroSuite.MetroLabel metroLabel3;
        public BrightIdeasSoftware.TreeListView treeCRS;
        private BrightIdeasSoftware.OLVColumn clnCRS;
        private BrightIdeasSoftware.OLVColumn clnAutorityCode;
        private MetroSuite.MetroComboBox cbxExpander;
        private MetroSuite.MetroLabel metroLabel4;
        private MetroSuite.MetroComboBox cbxHotItem;
        private MetroSuite.MetroLabel metroLabel5;
        private MetroSuite.MetroLabel lblStatus;
        private BrightIdeasSoftware.OLVColumn cnlCSRCodeName;
        public MetroSuite.MetroTextbox tbxSelectedCRSName;
        private System.Windows.Forms.Panel pnlProjectedCoord;
        private System.Windows.Forms.Button btnProjectedCoord;
        private System.Windows.Forms.Panel pnlGeographicCoord;
        private System.Windows.Forms.Button btnGeographicCoord;
        public MetroSuite.MetroSwitch editSwitch;
        private MetroSuite.MetroLabel metroLabel6;
        private System.Windows.Forms.Button btnCreateGrisSystem;
        public MetroSuite.MetroSwitch chbxEsri;
        private MetroSuite.MetroLabel metroLabel7;
        public MetroSuite.MetroSwitch chbxReadable;
        private MetroSuite.MetroLabel metroLabel8;
        private System.Windows.Forms.Panel pnlButtons;
    }
}