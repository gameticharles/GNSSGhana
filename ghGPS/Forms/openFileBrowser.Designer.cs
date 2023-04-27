namespace ghGPS.Forms
{
    partial class openFileBrowser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(openFileBrowser));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlCustomFolder = new System.Windows.Forms.Panel();
            this.btnMenuEdit = new System.Windows.Forms.Button();
            this.btnCustomFolder = new System.Windows.Forms.Button();
            this.btnAddCustomFolder = new System.Windows.Forms.Button();
            this.pnlCloud = new System.Windows.Forms.Panel();
            this.btnPersonalSpace = new System.Windows.Forms.Button();
            this.btnCloud = new System.Windows.Forms.Button();
            this.pnlLocal = new System.Windows.Forms.Panel();
            this.btnNetwork = new System.Windows.Forms.Button();
            this.btnComputer = new System.Windows.Forms.Button();
            this.btnDownloads = new System.Windows.Forms.Button();
            this.btnMyDocument = new System.Windows.Forms.Button();
            this.btnDesktop = new System.Windows.Forms.Button();
            this.btnRecentFolder = new System.Windows.Forms.Button();
            this.btnLocal = new System.Windows.Forms.Button();
            this.lblItems = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.shellView = new GongSolutions.Shell.ShellView();
            this.filterCombo = new GongSolutions.Shell.FileFilterComboBox();
            this.fileNameCombo = new GongSolutions.Shell.FileNameComboBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.fileFilterComboBox1 = new GongSolutions.Shell.FileFilterComboBox();
            this.fileNameComboBox1 = new GongSolutions.Shell.FileNameComboBox();
            this.panel8 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.tbxWebURL = new System.Windows.Forms.TextBox();
            this.btnViewStyle = new System.Windows.Forms.Button();
            this.btnNewFolder = new System.Windows.Forms.Button();
            this.btnUPFolder = new MetroSuite.MetroNavigationButton();
            this.btnForward = new MetroSuite.MetroNavigationButton();
            this.btnBack = new MetroSuite.MetroNavigationButton();
            this.lblHeaderText = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.DetailsIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ListIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smallIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.largeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.thumnailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.thumbstripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.pnlCustomFolder.SuspendLayout();
            this.pnlCloud.SuspendLayout();
            this.pnlLocal.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.panel1.Controls.Add(this.pnlCustomFolder);
            this.panel1.Controls.Add(this.btnAddCustomFolder);
            this.panel1.Controls.Add(this.pnlCloud);
            this.panel1.Controls.Add(this.pnlLocal);
            this.panel1.Controls.Add(this.lblItems);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(194, 508);
            this.panel1.TabIndex = 0;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainScreen_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainScreen_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainScreen_MouseUp);
            // 
            // pnlCustomFolder
            // 
            this.pnlCustomFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.pnlCustomFolder.Controls.Add(this.btnMenuEdit);
            this.pnlCustomFolder.Controls.Add(this.btnCustomFolder);
            this.pnlCustomFolder.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCustomFolder.Location = new System.Drawing.Point(0, 92);
            this.pnlCustomFolder.Name = "pnlCustomFolder";
            this.pnlCustomFolder.Size = new System.Drawing.Size(194, 46);
            this.pnlCustomFolder.TabIndex = 5;
            this.pnlCustomFolder.Visible = false;
            // 
            // btnMenuEdit
            // 
            this.btnMenuEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnMenuEdit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnMenuEdit.FlatAppearance.BorderSize = 0;
            this.btnMenuEdit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnMenuEdit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnMenuEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMenuEdit.Image = ((System.Drawing.Image)(resources.GetObject("btnMenuEdit.Image")));
            this.btnMenuEdit.Location = new System.Drawing.Point(171, 11);
            this.btnMenuEdit.Name = "btnMenuEdit";
            this.btnMenuEdit.Size = new System.Drawing.Size(17, 23);
            this.btnMenuEdit.TabIndex = 6;
            this.btnMenuEdit.UseVisualStyleBackColor = false;
            this.btnMenuEdit.Click += new System.EventHandler(this.btnMenuEdit_Click);
            this.btnMenuEdit.MouseEnter += new System.EventHandler(this.btnMenuEdit_MouseEnter);
            this.btnMenuEdit.MouseLeave += new System.EventHandler(this.btnMenuEdit_MouseLeave);
            // 
            // btnCustomFolder
            // 
            this.btnCustomFolder.AutoEllipsis = true;
            this.btnCustomFolder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCustomFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnCustomFolder.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCustomFolder.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnCustomFolder.FlatAppearance.BorderSize = 0;
            this.btnCustomFolder.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnCustomFolder.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnCustomFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCustomFolder.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCustomFolder.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnCustomFolder.Image = ((System.Drawing.Image)(resources.GetObject("btnCustomFolder.Image")));
            this.btnCustomFolder.Location = new System.Drawing.Point(0, 0);
            this.btnCustomFolder.Name = "btnCustomFolder";
            this.btnCustomFolder.Size = new System.Drawing.Size(194, 45);
            this.btnCustomFolder.TabIndex = 6;
            this.btnCustomFolder.Text = "    User Location      ";
            this.btnCustomFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCustomFolder.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCustomFolder.UseVisualStyleBackColor = false;
            this.btnCustomFolder.Click += new System.EventHandler(this.btnDesktop_Click);
            this.btnCustomFolder.MouseEnter += new System.EventHandler(this.btnMenuEdit_MouseEnter);
            this.btnCustomFolder.MouseLeave += new System.EventHandler(this.btnMenuEdit_MouseLeave);
            // 
            // btnAddCustomFolder
            // 
            this.btnAddCustomFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(156)))), ((int)(((byte)(222)))));
            this.btnAddCustomFolder.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnAddCustomFolder.FlatAppearance.BorderSize = 0;
            this.btnAddCustomFolder.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnAddCustomFolder.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnAddCustomFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddCustomFolder.ForeColor = System.Drawing.Color.LightGray;
            this.btnAddCustomFolder.Location = new System.Drawing.Point(17, 429);
            this.btnAddCustomFolder.Name = "btnAddCustomFolder";
            this.btnAddCustomFolder.Size = new System.Drawing.Size(160, 32);
            this.btnAddCustomFolder.TabIndex = 3;
            this.btnAddCustomFolder.Text = "Custom Location";
            this.btnAddCustomFolder.UseVisualStyleBackColor = false;
            this.btnAddCustomFolder.Click += new System.EventHandler(this.btnAddCustomFolder_Click);
            // 
            // pnlCloud
            // 
            this.pnlCloud.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.pnlCloud.Controls.Add(this.btnPersonalSpace);
            this.pnlCloud.Controls.Add(this.btnCloud);
            this.pnlCloud.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCloud.Location = new System.Drawing.Point(0, 46);
            this.pnlCloud.Name = "pnlCloud";
            this.pnlCloud.Size = new System.Drawing.Size(194, 46);
            this.pnlCloud.TabIndex = 2;
            // 
            // btnPersonalSpace
            // 
            this.btnPersonalSpace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPersonalSpace.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnPersonalSpace.FlatAppearance.BorderSize = 0;
            this.btnPersonalSpace.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnPersonalSpace.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnPersonalSpace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPersonalSpace.ForeColor = System.Drawing.Color.LightGray;
            this.btnPersonalSpace.Image = ((System.Drawing.Image)(resources.GetObject("btnPersonalSpace.Image")));
            this.btnPersonalSpace.Location = new System.Drawing.Point(0, 47);
            this.btnPersonalSpace.Name = "btnPersonalSpace";
            this.btnPersonalSpace.Size = new System.Drawing.Size(194, 38);
            this.btnPersonalSpace.TabIndex = 5;
            this.btnPersonalSpace.Text = "       Personal Space";
            this.btnPersonalSpace.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPersonalSpace.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPersonalSpace.UseVisualStyleBackColor = true;
            this.btnPersonalSpace.Click += new System.EventHandler(this.btnDesktop_Click);
            // 
            // btnCloud
            // 
            this.btnCloud.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnCloud.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCloud.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnCloud.FlatAppearance.BorderSize = 0;
            this.btnCloud.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnCloud.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnCloud.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCloud.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCloud.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnCloud.Image = ((System.Drawing.Image)(resources.GetObject("btnCloud.Image")));
            this.btnCloud.Location = new System.Drawing.Point(0, 0);
            this.btnCloud.Name = "btnCloud";
            this.btnCloud.Size = new System.Drawing.Size(194, 45);
            this.btnCloud.TabIndex = 0;
            this.btnCloud.Text = "    Cloud                    ";
            this.btnCloud.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCloud.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCloud.UseVisualStyleBackColor = false;
            this.btnCloud.Click += new System.EventHandler(this.btnCloud_Click);
            // 
            // pnlLocal
            // 
            this.pnlLocal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.pnlLocal.Controls.Add(this.btnNetwork);
            this.pnlLocal.Controls.Add(this.btnComputer);
            this.pnlLocal.Controls.Add(this.btnDownloads);
            this.pnlLocal.Controls.Add(this.btnMyDocument);
            this.pnlLocal.Controls.Add(this.btnDesktop);
            this.pnlLocal.Controls.Add(this.btnRecentFolder);
            this.pnlLocal.Controls.Add(this.btnLocal);
            this.pnlLocal.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLocal.Location = new System.Drawing.Point(0, 0);
            this.pnlLocal.Name = "pnlLocal";
            this.pnlLocal.Size = new System.Drawing.Size(194, 46);
            this.pnlLocal.TabIndex = 1;
            // 
            // btnNetwork
            // 
            this.btnNetwork.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNetwork.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnNetwork.FlatAppearance.BorderSize = 0;
            this.btnNetwork.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnNetwork.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnNetwork.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNetwork.ForeColor = System.Drawing.Color.LightGray;
            this.btnNetwork.Image = ((System.Drawing.Image)(resources.GetObject("btnNetwork.Image")));
            this.btnNetwork.Location = new System.Drawing.Point(0, 247);
            this.btnNetwork.Name = "btnNetwork";
            this.btnNetwork.Size = new System.Drawing.Size(194, 38);
            this.btnNetwork.TabIndex = 4;
            this.btnNetwork.Text = "       Network          ";
            this.btnNetwork.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNetwork.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnNetwork.UseVisualStyleBackColor = true;
            this.btnNetwork.Click += new System.EventHandler(this.btnDesktop_Click);
            // 
            // btnComputer
            // 
            this.btnComputer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnComputer.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnComputer.FlatAppearance.BorderSize = 0;
            this.btnComputer.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnComputer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnComputer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnComputer.ForeColor = System.Drawing.Color.LightGray;
            this.btnComputer.Image = ((System.Drawing.Image)(resources.GetObject("btnComputer.Image")));
            this.btnComputer.Location = new System.Drawing.Point(0, 87);
            this.btnComputer.Name = "btnComputer";
            this.btnComputer.Size = new System.Drawing.Size(194, 38);
            this.btnComputer.TabIndex = 4;
            this.btnComputer.Text = "       Computer       ";
            this.btnComputer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnComputer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnComputer.UseVisualStyleBackColor = true;
            this.btnComputer.Click += new System.EventHandler(this.btnDesktop_Click);
            // 
            // btnDownloads
            // 
            this.btnDownloads.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownloads.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnDownloads.FlatAppearance.BorderSize = 0;
            this.btnDownloads.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnDownloads.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnDownloads.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownloads.ForeColor = System.Drawing.Color.LightGray;
            this.btnDownloads.Image = ((System.Drawing.Image)(resources.GetObject("btnDownloads.Image")));
            this.btnDownloads.Location = new System.Drawing.Point(0, 207);
            this.btnDownloads.Name = "btnDownloads";
            this.btnDownloads.Size = new System.Drawing.Size(194, 38);
            this.btnDownloads.TabIndex = 3;
            this.btnDownloads.Text = "       Downloads     ";
            this.btnDownloads.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDownloads.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDownloads.UseVisualStyleBackColor = true;
            this.btnDownloads.Click += new System.EventHandler(this.btnDesktop_Click);
            // 
            // btnMyDocument
            // 
            this.btnMyDocument.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMyDocument.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnMyDocument.FlatAppearance.BorderSize = 0;
            this.btnMyDocument.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnMyDocument.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnMyDocument.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMyDocument.ForeColor = System.Drawing.Color.LightGray;
            this.btnMyDocument.Image = ((System.Drawing.Image)(resources.GetObject("btnMyDocument.Image")));
            this.btnMyDocument.Location = new System.Drawing.Point(0, 167);
            this.btnMyDocument.Name = "btnMyDocument";
            this.btnMyDocument.Size = new System.Drawing.Size(194, 38);
            this.btnMyDocument.TabIndex = 3;
            this.btnMyDocument.Text = "       Documents    ";
            this.btnMyDocument.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMyDocument.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnMyDocument.UseVisualStyleBackColor = true;
            this.btnMyDocument.Click += new System.EventHandler(this.btnDesktop_Click);
            // 
            // btnDesktop
            // 
            this.btnDesktop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDesktop.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnDesktop.FlatAppearance.BorderSize = 0;
            this.btnDesktop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnDesktop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnDesktop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDesktop.ForeColor = System.Drawing.Color.LightGray;
            this.btnDesktop.Image = ((System.Drawing.Image)(resources.GetObject("btnDesktop.Image")));
            this.btnDesktop.Location = new System.Drawing.Point(0, 127);
            this.btnDesktop.Name = "btnDesktop";
            this.btnDesktop.Size = new System.Drawing.Size(194, 38);
            this.btnDesktop.TabIndex = 2;
            this.btnDesktop.Text = "       Desktop            ";
            this.btnDesktop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDesktop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDesktop.UseVisualStyleBackColor = true;
            this.btnDesktop.Click += new System.EventHandler(this.btnDesktop_Click);
            // 
            // btnRecentFolder
            // 
            this.btnRecentFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRecentFolder.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnRecentFolder.FlatAppearance.BorderSize = 0;
            this.btnRecentFolder.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnRecentFolder.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnRecentFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRecentFolder.ForeColor = System.Drawing.Color.LightGray;
            this.btnRecentFolder.Image = ((System.Drawing.Image)(resources.GetObject("btnRecentFolder.Image")));
            this.btnRecentFolder.Location = new System.Drawing.Point(0, 47);
            this.btnRecentFolder.Name = "btnRecentFolder";
            this.btnRecentFolder.Size = new System.Drawing.Size(194, 38);
            this.btnRecentFolder.TabIndex = 1;
            this.btnRecentFolder.Text = "       Recent Folder  ";
            this.btnRecentFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRecentFolder.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRecentFolder.UseVisualStyleBackColor = true;
            this.btnRecentFolder.Click += new System.EventHandler(this.btnDesktop_Click);
            // 
            // btnLocal
            // 
            this.btnLocal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnLocal.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnLocal.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnLocal.FlatAppearance.BorderSize = 0;
            this.btnLocal.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnLocal.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnLocal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLocal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnLocal.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnLocal.Image = ((System.Drawing.Image)(resources.GetObject("btnLocal.Image")));
            this.btnLocal.Location = new System.Drawing.Point(0, 0);
            this.btnLocal.Name = "btnLocal";
            this.btnLocal.Size = new System.Drawing.Size(194, 45);
            this.btnLocal.TabIndex = 0;
            this.btnLocal.Text = "    Local Documents";
            this.btnLocal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLocal.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLocal.UseVisualStyleBackColor = false;
            this.btnLocal.Click += new System.EventHandler(this.btnLocal_Click);
            // 
            // lblItems
            // 
            this.lblItems.AutoSize = true;
            this.lblItems.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblItems.Location = new System.Drawing.Point(7, 487);
            this.lblItems.Name = "lblItems";
            this.lblItems.Size = new System.Drawing.Size(36, 15);
            this.lblItems.TabIndex = 0;
            this.lblItems.Text = "items";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel7);
            this.panel2.Controls.Add(this.panel6);
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(195, 1);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(617, 508);
            this.panel2.TabIndex = 1;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.shellView);
            this.panel7.Controls.Add(this.filterCombo);
            this.panel7.Controls.Add(this.fileNameCombo);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(0, 61);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(617, 367);
            this.panel7.TabIndex = 2;
            // 
            // shellView
            // 
            this.shellView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shellView.Location = new System.Drawing.Point(0, 0);
            this.shellView.Name = "shellView";
            this.shellView.Size = new System.Drawing.Size(617, 367);
            this.shellView.StatusBar = null;
            this.shellView.TabIndex = 9;
            this.shellView.Text = "shellView1";
            this.shellView.View = GongSolutions.Shell.ShellViewStyle.Tile;
            this.shellView.Navigated += new System.EventHandler(this.shellView_Navigated);
            this.shellView.SelectionChanged += new System.EventHandler(this.shellView_SelectionChanged);
            this.shellView.DoubleClick += new System.EventHandler(this.shellView_DoubleClick);
            // 
            // filterCombo
            // 
            this.filterCombo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterCombo.FilterItems = "Text Files (*.txt)|*.txt|Video Files|*.avi, *.wmv|All Files (*.*)|*.*";
            this.filterCombo.FormattingEnabled = true;
            this.filterCombo.Location = new System.Drawing.Point(0, 0);
            this.filterCombo.Name = "filterCombo";
            this.filterCombo.ShellView = this.shellView;
            this.filterCombo.Size = new System.Drawing.Size(617, 23);
            this.filterCombo.TabIndex = 8;
            // 
            // fileNameCombo
            // 
            this.fileNameCombo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileNameCombo.FilterControl = this.filterCombo;
            this.fileNameCombo.FormattingEnabled = true;
            this.fileNameCombo.Location = new System.Drawing.Point(0, 0);
            this.fileNameCombo.Name = "fileNameCombo";
            this.fileNameCombo.ShellView = this.shellView;
            this.fileNameCombo.Size = new System.Drawing.Size(617, 23);
            this.fileNameCombo.TabIndex = 7;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panel6.Controls.Add(this.fileFilterComboBox1);
            this.panel6.Controls.Add(this.fileNameComboBox1);
            this.panel6.Controls.Add(this.panel8);
            this.panel6.Controls.Add(this.btnCancel);
            this.panel6.Controls.Add(this.btnOpen);
            this.panel6.Controls.Add(this.label2);
            this.panel6.Controls.Add(this.label1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 428);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(617, 80);
            this.panel6.TabIndex = 1;
            this.panel6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainScreen_MouseDown);
            this.panel6.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainScreen_MouseMove);
            this.panel6.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainScreen_MouseUp);
            // 
            // fileFilterComboBox1
            // 
            this.fileFilterComboBox1.FilterItems = "";
            this.fileFilterComboBox1.FormattingEnabled = true;
            this.fileFilterComboBox1.Location = new System.Drawing.Point(95, 46);
            this.fileFilterComboBox1.MaxDropDownItems = 15;
            this.fileFilterComboBox1.Name = "fileFilterComboBox1";
            this.fileFilterComboBox1.ShellView = this.shellView;
            this.fileFilterComboBox1.Size = new System.Drawing.Size(415, 23);
            this.fileFilterComboBox1.TabIndex = 5;
            this.fileFilterComboBox1.SelectedIndexChanged += new System.EventHandler(this.fileFilterComboBox1_SelectedIndexChanged);
            // 
            // fileNameComboBox1
            // 
            this.fileNameComboBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.fileNameComboBox1.FilterControl = this.filterCombo;
            this.fileNameComboBox1.FormattingEnabled = true;
            this.fileNameComboBox1.Location = new System.Drawing.Point(95, 14);
            this.fileNameComboBox1.MaxDropDownItems = 15;
            this.fileNameComboBox1.Name = "fileNameComboBox1";
            this.fileNameComboBox1.ShellView = this.shellView;
            this.fileNameComboBox1.Size = new System.Drawing.Size(415, 23);
            this.fileNameComboBox1.TabIndex = 4;
            this.fileNameComboBox1.FileNameEntered += new System.EventHandler(this.fileNameCombo_FilenameEntered);
            this.fileNameComboBox1.TextChanged += new System.EventHandler(this.fileNameCombo_TextChanged);
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.Gainsboro;
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(617, 1);
            this.panel8.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Location = new System.Drawing.Point(516, 45);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Enabled = false;
            this.btnOpen.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnOpen.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btnOpen.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpen.ForeColor = System.Drawing.Color.Black;
            this.btnOpen.Location = new System.Drawing.Point(516, 12);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 25);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "&Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.Open_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(26, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "File Type:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(26, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "File Name:";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panel5.Controls.Add(this.tbxWebURL);
            this.panel5.Controls.Add(this.btnViewStyle);
            this.panel5.Controls.Add(this.btnNewFolder);
            this.panel5.Controls.Add(this.btnUPFolder);
            this.panel5.Controls.Add(this.btnForward);
            this.panel5.Controls.Add(this.btnBack);
            this.panel5.Controls.Add(this.lblHeaderText);
            this.panel5.Controls.Add(this.label4);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(617, 61);
            this.panel5.TabIndex = 0;
            this.panel5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainScreen_MouseDown);
            this.panel5.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainScreen_MouseMove);
            this.panel5.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainScreen_MouseUp);
            // 
            // tbxWebURL
            // 
            this.tbxWebURL.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.tbxWebURL.Location = new System.Drawing.Point(113, 32);
            this.tbxWebURL.Name = "tbxWebURL";
            this.tbxWebURL.ReadOnly = true;
            this.tbxWebURL.Size = new System.Drawing.Size(390, 23);
            this.tbxWebURL.TabIndex = 3;
            // 
            // btnViewStyle
            // 
            this.btnViewStyle.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.btnViewStyle.FlatAppearance.BorderSize = 0;
            this.btnViewStyle.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.btnViewStyle.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btnViewStyle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewStyle.Image = ((System.Drawing.Image)(resources.GetObject("btnViewStyle.Image")));
            this.btnViewStyle.Location = new System.Drawing.Point(566, 32);
            this.btnViewStyle.Name = "btnViewStyle";
            this.btnViewStyle.Size = new System.Drawing.Size(34, 23);
            this.btnViewStyle.TabIndex = 2;
            this.btnViewStyle.UseVisualStyleBackColor = true;
            this.btnViewStyle.Click += new System.EventHandler(this.btnViewStyle_Click);
            // 
            // btnNewFolder
            // 
            this.btnNewFolder.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.btnNewFolder.FlatAppearance.BorderSize = 0;
            this.btnNewFolder.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.btnNewFolder.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btnNewFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewFolder.Image = ((System.Drawing.Image)(resources.GetObject("btnNewFolder.Image")));
            this.btnNewFolder.Location = new System.Drawing.Point(534, 32);
            this.btnNewFolder.Name = "btnNewFolder";
            this.btnNewFolder.Size = new System.Drawing.Size(34, 23);
            this.btnNewFolder.TabIndex = 2;
            this.btnNewFolder.UseVisualStyleBackColor = true;
            this.btnNewFolder.Click += new System.EventHandler(this.btnNewFolder_Click);
            // 
            // btnUPFolder
            // 
            this.btnUPFolder.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnUPFolder.ArrowDirection = System.Windows.Forms.ArrowDirection.Up;
            this.btnUPFolder.ArrowHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnUPFolder.ArrowPressedColor = System.Drawing.Color.White;
            this.btnUPFolder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnUPFolder.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnUPFolder.BorderHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnUPFolder.BorderPressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnUPFolder.DefaultColor = System.Drawing.Color.White;
            this.btnUPFolder.DisabledArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnUPFolder.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnUPFolder.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnUPFolder.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnUPFolder.Location = new System.Drawing.Point(507, 33);
            this.btnUPFolder.Name = "btnUPFolder";
            this.btnUPFolder.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnUPFolder.Size = new System.Drawing.Size(24, 24);
            this.btnUPFolder.TabIndex = 1;
            this.btnUPFolder.Text = "metroNavigationButton1";
            this.btnUPFolder.Click += new System.EventHandler(this.btnUPFolder_Click);
            // 
            // btnForward
            // 
            this.btnForward.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnForward.ArrowDirection = System.Windows.Forms.ArrowDirection.Right;
            this.btnForward.ArrowHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnForward.ArrowPressedColor = System.Drawing.Color.White;
            this.btnForward.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnForward.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnForward.BorderHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnForward.BorderPressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnForward.DefaultColor = System.Drawing.Color.White;
            this.btnForward.DisabledArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnForward.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnForward.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnForward.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnForward.Location = new System.Drawing.Point(32, 33);
            this.btnForward.Name = "btnForward";
            this.btnForward.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnForward.Size = new System.Drawing.Size(24, 24);
            this.btnForward.TabIndex = 1;
            this.btnForward.Text = "metroNavigationButton1";
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            // 
            // btnBack
            // 
            this.btnBack.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnBack.ArrowHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnBack.ArrowPressedColor = System.Drawing.Color.White;
            this.btnBack.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnBack.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnBack.BorderHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnBack.BorderPressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnBack.DefaultColor = System.Drawing.Color.White;
            this.btnBack.DisabledArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnBack.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnBack.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnBack.Location = new System.Drawing.Point(4, 33);
            this.btnBack.Name = "btnBack";
            this.btnBack.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnBack.Size = new System.Drawing.Size(24, 24);
            this.btnBack.TabIndex = 1;
            this.btnBack.Text = "metroNavigationButton1";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // lblHeaderText
            // 
            this.lblHeaderText.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeaderText.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblHeaderText.ForeColor = System.Drawing.Color.Black;
            this.lblHeaderText.Location = new System.Drawing.Point(0, 0);
            this.lblHeaderText.Name = "lblHeaderText";
            this.lblHeaderText.Size = new System.Drawing.Size(617, 29);
            this.lblHeaderText.TabIndex = 0;
            this.lblHeaderText.Text = "Open";
            this.lblHeaderText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHeaderText.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainScreen_MouseDown);
            this.lblHeaderText.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainScreen_MouseMove);
            this.lblHeaderText.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainScreen_MouseUp);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(58, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "Look In:";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(126, 52);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.renameToolStripMenuItem.ForeColor = System.Drawing.Color.DimGray;
            this.renameToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("renameToolStripMenuItem.Image")));
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(125, 24);
            this.renameToolStripMenuItem.Text = "Rename";
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.deleteToolStripMenuItem.ForeColor = System.Drawing.Color.DimGray;
            this.deleteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("deleteToolStripMenuItem.Image")));
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(125, 24);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DetailsIconsToolStripMenuItem,
            this.ListIconsToolStripMenuItem,
            this.smallIconsToolStripMenuItem,
            this.largeIconsToolStripMenuItem,
            this.tileToolStripMenuItem,
            this.thumnailToolStripMenuItem,
            this.thumbstripToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip1";
            this.contextMenuStrip2.Size = new System.Drawing.Size(152, 172);
            // 
            // DetailsIconsToolStripMenuItem
            // 
            this.DetailsIconsToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.DetailsIconsToolStripMenuItem.ForeColor = System.Drawing.Color.DimGray;
            this.DetailsIconsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("DetailsIconsToolStripMenuItem.Image")));
            this.DetailsIconsToolStripMenuItem.Name = "DetailsIconsToolStripMenuItem";
            this.DetailsIconsToolStripMenuItem.Size = new System.Drawing.Size(151, 24);
            this.DetailsIconsToolStripMenuItem.Text = "VIew Details";
            this.DetailsIconsToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // ListIconsToolStripMenuItem
            // 
            this.ListIconsToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ListIconsToolStripMenuItem.ForeColor = System.Drawing.Color.DimGray;
            this.ListIconsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("ListIconsToolStripMenuItem.Image")));
            this.ListIconsToolStripMenuItem.Name = "ListIconsToolStripMenuItem";
            this.ListIconsToolStripMenuItem.Size = new System.Drawing.Size(151, 24);
            this.ListIconsToolStripMenuItem.Text = "List";
            this.ListIconsToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // smallIconsToolStripMenuItem
            // 
            this.smallIconsToolStripMenuItem.ForeColor = System.Drawing.Color.DimGray;
            this.smallIconsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("smallIconsToolStripMenuItem.Image")));
            this.smallIconsToolStripMenuItem.Name = "smallIconsToolStripMenuItem";
            this.smallIconsToolStripMenuItem.Size = new System.Drawing.Size(151, 24);
            this.smallIconsToolStripMenuItem.Text = "Small icons";
            this.smallIconsToolStripMenuItem.Click += new System.EventHandler(this.smallIconsToolStripMenuItem_Click);
            // 
            // largeIconsToolStripMenuItem
            // 
            this.largeIconsToolStripMenuItem.ForeColor = System.Drawing.Color.DimGray;
            this.largeIconsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("largeIconsToolStripMenuItem.Image")));
            this.largeIconsToolStripMenuItem.Name = "largeIconsToolStripMenuItem";
            this.largeIconsToolStripMenuItem.Size = new System.Drawing.Size(151, 24);
            this.largeIconsToolStripMenuItem.Text = "Large icons";
            this.largeIconsToolStripMenuItem.Click += new System.EventHandler(this.largeIconsToolStripMenuItem_Click);
            // 
            // tileToolStripMenuItem
            // 
            this.tileToolStripMenuItem.ForeColor = System.Drawing.Color.DimGray;
            this.tileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("tileToolStripMenuItem.Image")));
            this.tileToolStripMenuItem.Name = "tileToolStripMenuItem";
            this.tileToolStripMenuItem.Size = new System.Drawing.Size(151, 24);
            this.tileToolStripMenuItem.Text = "Tile";
            this.tileToolStripMenuItem.Click += new System.EventHandler(this.tileToolStripMenuItem_Click);
            // 
            // thumnailToolStripMenuItem
            // 
            this.thumnailToolStripMenuItem.ForeColor = System.Drawing.Color.DimGray;
            this.thumnailToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("thumnailToolStripMenuItem.Image")));
            this.thumnailToolStripMenuItem.Name = "thumnailToolStripMenuItem";
            this.thumnailToolStripMenuItem.Size = new System.Drawing.Size(151, 24);
            this.thumnailToolStripMenuItem.Text = "Thumbnail";
            this.thumnailToolStripMenuItem.Click += new System.EventHandler(this.thumnailToolStripMenuItem_Click);
            // 
            // thumbstripToolStripMenuItem
            // 
            this.thumbstripToolStripMenuItem.ForeColor = System.Drawing.Color.DimGray;
            this.thumbstripToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("thumbstripToolStripMenuItem.Image")));
            this.thumbstripToolStripMenuItem.Name = "thumbstripToolStripMenuItem";
            this.thumbstripToolStripMenuItem.Size = new System.Drawing.Size(151, 24);
            this.thumbstripToolStripMenuItem.Text = "Thumbstrip";
            this.thumbstripToolStripMenuItem.Click += new System.EventHandler(this.thumbstripToolStripMenuItem_Click);
            // 
            // openFileBrowser
            // 
            this.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.AllowResize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(813, 510);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "openFileBrowser";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.State = MetroSuite.MetroForm.FormState.Custom;
            this.Style = MetroSuite.Design.Style.Light;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FileDialog_FormClosed);
            this.Load += new System.EventHandler(this.openFileBrowser_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlCustomFolder.ResumeLayout(false);
            this.pnlCloud.ResumeLayout(false);
            this.pnlLocal.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnLocal;
        private System.Windows.Forms.Panel pnlLocal;
        private System.Windows.Forms.Panel pnlCloud;
        private System.Windows.Forms.Button btnCloud;
        private System.Windows.Forms.Button btnRecentFolder;
        private System.Windows.Forms.Button btnComputer;
        private System.Windows.Forms.Button btnMyDocument;
        private System.Windows.Forms.Button btnDesktop;
        private System.Windows.Forms.Button btnNetwork;
        private System.Windows.Forms.Button btnPersonalSpace;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Panel panel8;
        private MetroSuite.MetroNavigationButton btnForward;
        private MetroSuite.MetroNavigationButton btnBack;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnAddCustomFolder;
        private System.Windows.Forms.Panel pnlCustomFolder;
        private System.Windows.Forms.Button btnMenuEdit;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private MetroSuite.MetroNavigationButton btnUPFolder;
        private System.Windows.Forms.Button btnNewFolder;
        private System.Windows.Forms.Button btnViewStyle;
        private System.Windows.Forms.TextBox tbxWebURL;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem DetailsIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ListIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smallIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem largeIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem thumnailToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem thumbstripToolStripMenuItem;
        public System.Windows.Forms.Button btnCustomFolder;
        private GongSolutions.Shell.FileFilterComboBox filterCombo;
        private GongSolutions.Shell.FileNameComboBox fileNameCombo;
        private GongSolutions.Shell.FileNameComboBox fileNameComboBox1;
        public GongSolutions.Shell.ShellView shellView;
        private System.Windows.Forms.Button btnDownloads;
        private System.Windows.Forms.Label lblItems;
        public GongSolutions.Shell.FileFilterComboBox fileFilterComboBox1;
        public System.Windows.Forms.Label lblHeaderText;
    }
}