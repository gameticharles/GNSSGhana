namespace ghGPS.User_Forms
{
    partial class rnxDataImport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(rnxDataImport));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblAbout = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tbxFilePath = new MetroSuite.MetroTextbox();
            this.btnODB = new MetroSuite.MetroButton();
            this.cbxFileType = new MetroSuite.MetroComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dgvFiles = new MetroFramework.Controls.MetroGrid();
            this.lblRoverCounts = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.obsTable = new System.Windows.Forms.TableLayoutPanel();
            this.lblObsSBAS = new System.Windows.Forms.Label();
            this.lblObsGAL = new System.Windows.Forms.Label();
            this.lblObsGPS = new System.Windows.Forms.Label();
            this.lblObsGLO = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.lblEphSBAS = new System.Windows.Forms.Label();
            this.lblEphGAL = new System.Windows.Forms.Label();
            this.lblEphGPS = new System.Windows.Forms.Label();
            this.lblEphGLO = new System.Windows.Forms.Label();
            this.File_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn19 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GPSNav = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GLONav = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.file_Paths = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFiles)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.obsTable.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblAbout
            // 
            this.lblAbout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblAbout.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblAbout.ForeColor = System.Drawing.Color.DimGray;
            this.lblAbout.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblAbout.Location = new System.Drawing.Point(25, 5);
            this.lblAbout.Name = "lblAbout";
            this.lblAbout.Size = new System.Drawing.Size(124, 21);
            this.lblAbout.TabIndex = 5;
            this.lblAbout.Text = "File/Folder Path:";
            this.lblAbout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 94F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel1.Controls.Add(this.tbxFilePath, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnODB, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbxFileType, 1, 0);
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(29, 26);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(602, 34);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // tbxFilePath
            // 
            this.tbxFilePath.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxFilePath.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxFilePath.DefaultColor = System.Drawing.Color.White;
            this.tbxFilePath.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxFilePath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxFilePath.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxFilePath.ForeColor = System.Drawing.Color.Black;
            this.tbxFilePath.HideSelection = false;
            this.tbxFilePath.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxFilePath.Location = new System.Drawing.Point(3, 3);
            this.tbxFilePath.Name = "tbxFilePath";
            this.tbxFilePath.PasswordChar = '\0';
            this.tbxFilePath.ReadOnly = true;
            this.tbxFilePath.Size = new System.Drawing.Size(465, 28);
            this.tbxFilePath.TabIndex = 2;
            // 
            // btnODB
            // 
            this.btnODB.AutoStyle = false;
            this.btnODB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnODB.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnODB.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnODB.DefaultColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.btnODB.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnODB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnODB.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnODB.ForeColor = System.Drawing.Color.DimGray;
            this.btnODB.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnODB.Icon = ((System.Drawing.Image)(resources.GetObject("btnODB.Icon")));
            this.btnODB.Location = new System.Drawing.Point(568, 3);
            this.btnODB.Name = "btnODB";
            this.btnODB.PressedColor = System.Drawing.Color.Silver;
            this.btnODB.Size = new System.Drawing.Size(31, 28);
            this.btnODB.Style = MetroSuite.Design.Style.Custom;
            this.btnODB.TabIndex = 4;
            this.btnODB.Click += new System.EventHandler(this.btnODB_Click);
            // 
            // cbxFileType
            // 
            this.cbxFileType.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxFileType.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxFileType.AutoStyle = false;
            this.cbxFileType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxFileType.DefaultColor = System.Drawing.Color.White;
            this.cbxFileType.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.cbxFileType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxFileType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxFileType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxFileType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbxFileType.ForeColor = System.Drawing.Color.Black;
            this.cbxFileType.FormattingEnabled = true;
            this.cbxFileType.ItemHeight = 22;
            this.cbxFileType.Items.AddRange(new object[] {
            "File",
            "Folder"});
            this.cbxFileType.Location = new System.Drawing.Point(474, 3);
            this.cbxFileType.Name = "cbxFileType";
            this.cbxFileType.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxFileType.Size = new System.Drawing.Size(88, 28);
            this.cbxFileType.Style = MetroSuite.Design.Style.Custom;
            this.cbxFileType.TabIndex = 7;
            this.cbxFileType.SelectedIndexChanged += new System.EventHandler(this.cbxFileType_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel1.Controls.Add(this.dgvFiles);
            this.panel1.Location = new System.Drawing.Point(29, 66);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(1);
            this.panel1.Size = new System.Drawing.Size(602, 186);
            this.panel1.TabIndex = 12;
            // 
            // dgvFiles
            // 
            this.dgvFiles.AllowUserToAddRows = false;
            this.dgvFiles.AllowUserToResizeColumns = false;
            this.dgvFiles.AllowUserToResizeRows = false;
            dataGridViewCellStyle19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.dgvFiles.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle19;
            this.dgvFiles.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.dgvFiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvFiles.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvFiles.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            dataGridViewCellStyle20.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle20.ForeColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle20.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle20.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle20.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFiles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle20;
            this.dgvFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.File_Name,
            this.dataGridViewTextBoxColumn14,
            this.dataGridViewTextBoxColumn19,
            this.GPSNav,
            this.GLONav,
            this.file_Paths});
            dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle23.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle23.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle23.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle23.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            dataGridViewCellStyle23.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle23.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFiles.DefaultCellStyle = dataGridViewCellStyle23;
            this.dgvFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFiles.EnableHeadersVisualStyles = false;
            this.dgvFiles.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.dgvFiles.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.dgvFiles.Location = new System.Drawing.Point(1, 1);
            this.dgvFiles.Name = "dgvFiles";
            this.dgvFiles.ReadOnly = true;
            this.dgvFiles.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle24.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle24.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle24.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle24.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle24.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle24.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFiles.RowHeadersDefaultCellStyle = dataGridViewCellStyle24;
            this.dgvFiles.RowHeadersVisible = false;
            this.dgvFiles.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFiles.Size = new System.Drawing.Size(600, 184);
            this.dgvFiles.TabIndex = 8;
            this.dgvFiles.UseCustomBackColor = true;
            this.dgvFiles.UseCustomForeColor = true;
            this.dgvFiles.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvFiles_RowsAdded);
            this.dgvFiles.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgvFiles_RowsRemoved);
            this.dgvFiles.SelectionChanged += new System.EventHandler(this.dgvFiles_SelectionChanged);
            // 
            // lblRoverCounts
            // 
            this.lblRoverCounts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRoverCounts.AutoEllipsis = true;
            this.lblRoverCounts.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblRoverCounts.ForeColor = System.Drawing.Color.DimGray;
            this.lblRoverCounts.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblRoverCounts.Location = new System.Drawing.Point(144, 350);
            this.lblRoverCounts.Name = "lblRoverCounts";
            this.lblRoverCounts.Size = new System.Drawing.Size(65, 31);
            this.lblRoverCounts.TabIndex = 14;
            this.lblRoverCounts.Text = "00";
            this.lblRoverCounts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label14.AutoEllipsis = true;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label14.ForeColor = System.Drawing.Color.DimGray;
            this.label14.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label14.Location = new System.Drawing.Point(26, 350);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(123, 31);
            this.label14.TabIndex = 15;
            this.label14.Text = "RINEX Files Count:";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.Controls.Add(this.label19, 4, 2);
            this.tableLayoutPanel2.Controls.Add(this.label18, 3, 2);
            this.tableLayoutPanel2.Controls.Add(this.label17, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.label16, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label15, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label13, 4, 1);
            this.tableLayoutPanel2.Controls.Add(this.label12, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.label11, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.label10, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label9, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label8, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.label7, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.label6, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.label5, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(69, 275);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.97959F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.02041F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(270, 74);
            this.tableLayoutPanel2.TabIndex = 16;
            // 
            // label19
            // 
            this.label19.Cursor = System.Windows.Forms.Cursors.Default;
            this.label19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label19.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label19.ForeColor = System.Drawing.Color.DimGray;
            this.label19.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label19.Location = new System.Drawing.Point(216, 49);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(50, 24);
            this.label19.TabIndex = 32;
            this.label19.Text = "S5";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label18
            // 
            this.label18.Cursor = System.Windows.Forms.Cursors.Default;
            this.label18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label18.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label18.ForeColor = System.Drawing.Color.DimGray;
            this.label18.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label18.Location = new System.Drawing.Point(163, 49);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(46, 24);
            this.label18.TabIndex = 31;
            this.label18.Text = "D5";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label17
            // 
            this.label17.Cursor = System.Windows.Forms.Cursors.Default;
            this.label17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label17.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label17.ForeColor = System.Drawing.Color.DimGray;
            this.label17.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label17.Location = new System.Drawing.Point(110, 49);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(46, 24);
            this.label17.TabIndex = 30;
            this.label17.Text = "L3";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            this.label16.Cursor = System.Windows.Forms.Cursors.Default;
            this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label16.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label16.ForeColor = System.Drawing.Color.DimGray;
            this.label16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label16.Location = new System.Drawing.Point(57, 49);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(46, 24);
            this.label16.TabIndex = 29;
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.Cursor = System.Windows.Forms.Cursors.Default;
            this.label15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label15.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label15.ForeColor = System.Drawing.Color.DimGray;
            this.label15.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label15.Location = new System.Drawing.Point(4, 49);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(46, 24);
            this.label15.TabIndex = 28;
            this.label15.Text = "C5";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.Cursor = System.Windows.Forms.Cursors.Default;
            this.label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label13.ForeColor = System.Drawing.Color.DimGray;
            this.label13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label13.Location = new System.Drawing.Point(216, 25);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(50, 23);
            this.label13.TabIndex = 27;
            this.label13.Text = "S2";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.Cursor = System.Windows.Forms.Cursors.Default;
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label12.ForeColor = System.Drawing.Color.DimGray;
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(163, 25);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(46, 23);
            this.label12.TabIndex = 26;
            this.label12.Text = "D2";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.Cursor = System.Windows.Forms.Cursors.Default;
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label11.ForeColor = System.Drawing.Color.DimGray;
            this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label11.Location = new System.Drawing.Point(110, 25);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(46, 23);
            this.label11.TabIndex = 25;
            this.label11.Text = "L2";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.Cursor = System.Windows.Forms.Cursors.Default;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label10.ForeColor = System.Drawing.Color.DimGray;
            this.label10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label10.Location = new System.Drawing.Point(57, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 23);
            this.label10.TabIndex = 24;
            this.label10.Text = "P2";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.Cursor = System.Windows.Forms.Cursors.Default;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label9.ForeColor = System.Drawing.Color.DimGray;
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(4, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(46, 23);
            this.label9.TabIndex = 23;
            this.label9.Text = "C2";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.Cursor = System.Windows.Forms.Cursors.Default;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label8.ForeColor = System.Drawing.Color.DimGray;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(216, 1);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 23);
            this.label8.TabIndex = 22;
            this.label8.Text = "S1";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.Cursor = System.Windows.Forms.Cursors.Default;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label7.ForeColor = System.Drawing.Color.DimGray;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(163, 1);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 23);
            this.label7.TabIndex = 21;
            this.label7.Text = "D1";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.Cursor = System.Windows.Forms.Cursors.Default;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label6.ForeColor = System.Drawing.Color.DimGray;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(110, 1);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 23);
            this.label6.TabIndex = 20;
            this.label6.Text = "L1";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Cursor = System.Windows.Forms.Cursors.Default;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label5.ForeColor = System.Drawing.Color.DimGray;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(57, 1);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 23);
            this.label5.TabIndex = 19;
            this.label5.Text = "P1";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Cursor = System.Windows.Forms.Cursors.Default;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label4.ForeColor = System.Drawing.Color.DimGray;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(4, 1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 23);
            this.label4.TabIndex = 18;
            this.label4.Text = "C1";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // obsTable
            // 
            this.obsTable.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.obsTable.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.obsTable.ColumnCount = 4;
            this.obsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.obsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.obsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.obsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.obsTable.Controls.Add(this.lblObsSBAS, 0, 0);
            this.obsTable.Controls.Add(this.lblObsGAL, 0, 0);
            this.obsTable.Controls.Add(this.lblObsGPS, 0, 0);
            this.obsTable.Controls.Add(this.lblObsGLO, 0, 0);
            this.obsTable.Location = new System.Drawing.Point(363, 275);
            this.obsTable.Name = "obsTable";
            this.obsTable.RowCount = 1;
            this.obsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.4375F));
            this.obsTable.Size = new System.Drawing.Size(210, 26);
            this.obsTable.TabIndex = 17;
            // 
            // lblObsSBAS
            // 
            this.lblObsSBAS.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblObsSBAS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblObsSBAS.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblObsSBAS.ForeColor = System.Drawing.Color.DimGray;
            this.lblObsSBAS.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblObsSBAS.Location = new System.Drawing.Point(108, 1);
            this.lblObsSBAS.Name = "lblObsSBAS";
            this.lblObsSBAS.Size = new System.Drawing.Size(45, 24);
            this.lblObsSBAS.TabIndex = 22;
            this.lblObsSBAS.Text = "SBAS";
            this.lblObsSBAS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblObsGAL
            // 
            this.lblObsGAL.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblObsGAL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblObsGAL.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblObsGAL.ForeColor = System.Drawing.Color.DimGray;
            this.lblObsGAL.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblObsGAL.Location = new System.Drawing.Point(160, 1);
            this.lblObsGAL.Name = "lblObsGAL";
            this.lblObsGAL.Size = new System.Drawing.Size(46, 24);
            this.lblObsGAL.TabIndex = 21;
            this.lblObsGAL.Text = "GAL";
            this.lblObsGAL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblObsGPS
            // 
            this.lblObsGPS.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblObsGPS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblObsGPS.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblObsGPS.ForeColor = System.Drawing.Color.DimGray;
            this.lblObsGPS.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblObsGPS.Location = new System.Drawing.Point(4, 1);
            this.lblObsGPS.Name = "lblObsGPS";
            this.lblObsGPS.Size = new System.Drawing.Size(45, 24);
            this.lblObsGPS.TabIndex = 20;
            this.lblObsGPS.Text = "GPS";
            this.lblObsGPS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblObsGLO
            // 
            this.lblObsGLO.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblObsGLO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblObsGLO.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblObsGLO.ForeColor = System.Drawing.Color.DimGray;
            this.lblObsGLO.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblObsGLO.Location = new System.Drawing.Point(56, 1);
            this.lblObsGLO.Name = "lblObsGLO";
            this.lblObsGLO.Size = new System.Drawing.Size(45, 24);
            this.lblObsGLO.TabIndex = 19;
            this.lblObsGLO.Text = "GLO";
            this.lblObsGLO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 7.8F);
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(360, 309);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Ephemerides";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 7.8F);
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(360, 259);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Navigation Files";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label3.Cursor = System.Windows.Forms.Cursors.Default;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 7.8F);
            this.label3.ForeColor = System.Drawing.Color.DimGray;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(66, 259);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Observation Types";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.tableLayoutPanel4.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel4.ColumnCount = 4;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.Controls.Add(this.lblEphSBAS, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.lblEphGAL, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.lblEphGPS, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.lblEphGLO, 0, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(363, 323);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.4375F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(210, 26);
            this.tableLayoutPanel4.TabIndex = 17;
            // 
            // lblEphSBAS
            // 
            this.lblEphSBAS.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblEphSBAS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEphSBAS.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblEphSBAS.ForeColor = System.Drawing.Color.DimGray;
            this.lblEphSBAS.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblEphSBAS.Location = new System.Drawing.Point(108, 1);
            this.lblEphSBAS.Name = "lblEphSBAS";
            this.lblEphSBAS.Size = new System.Drawing.Size(45, 24);
            this.lblEphSBAS.TabIndex = 22;
            this.lblEphSBAS.Text = "SBAS";
            this.lblEphSBAS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEphGAL
            // 
            this.lblEphGAL.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblEphGAL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEphGAL.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblEphGAL.ForeColor = System.Drawing.Color.DimGray;
            this.lblEphGAL.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblEphGAL.Location = new System.Drawing.Point(160, 1);
            this.lblEphGAL.Name = "lblEphGAL";
            this.lblEphGAL.Size = new System.Drawing.Size(46, 24);
            this.lblEphGAL.TabIndex = 21;
            this.lblEphGAL.Text = "GAL";
            this.lblEphGAL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEphGPS
            // 
            this.lblEphGPS.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblEphGPS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEphGPS.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblEphGPS.ForeColor = System.Drawing.Color.DimGray;
            this.lblEphGPS.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblEphGPS.Location = new System.Drawing.Point(4, 1);
            this.lblEphGPS.Name = "lblEphGPS";
            this.lblEphGPS.Size = new System.Drawing.Size(45, 24);
            this.lblEphGPS.TabIndex = 20;
            this.lblEphGPS.Text = "GPS";
            this.lblEphGPS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEphGLO
            // 
            this.lblEphGLO.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblEphGLO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEphGLO.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblEphGLO.ForeColor = System.Drawing.Color.DimGray;
            this.lblEphGLO.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblEphGLO.Location = new System.Drawing.Point(56, 1);
            this.lblEphGLO.Name = "lblEphGLO";
            this.lblEphGLO.Size = new System.Drawing.Size(45, 24);
            this.lblEphGLO.TabIndex = 19;
            this.lblEphGLO.Text = "GLO";
            this.lblEphGLO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // File_Name
            // 
            this.File_Name.HeaderText = "File Name";
            this.File_Name.Name = "File_Name";
            this.File_Name.ReadOnly = true;
            this.File_Name.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.File_Name.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.File_Name.Width = 200;
            // 
            // dataGridViewTextBoxColumn14
            // 
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn14.DefaultCellStyle = dataGridViewCellStyle21;
            this.dataGridViewTextBoxColumn14.HeaderText = "Type";
            this.dataGridViewTextBoxColumn14.Name = "dataGridViewTextBoxColumn14";
            this.dataGridViewTextBoxColumn14.ReadOnly = true;
            this.dataGridViewTextBoxColumn14.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn14.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn14.Width = 65;
            // 
            // dataGridViewTextBoxColumn19
            // 
            dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn19.DefaultCellStyle = dataGridViewCellStyle22;
            this.dataGridViewTextBoxColumn19.HeaderText = "Size";
            this.dataGridViewTextBoxColumn19.Name = "dataGridViewTextBoxColumn19";
            this.dataGridViewTextBoxColumn19.ReadOnly = true;
            this.dataGridViewTextBoxColumn19.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn19.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn19.Width = 87;
            // 
            // GPSNav
            // 
            this.GPSNav.HeaderText = "GPS Nav";
            this.GPSNav.MinimumWidth = 2;
            this.GPSNav.Name = "GPSNav";
            this.GPSNav.ReadOnly = true;
            this.GPSNav.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.GPSNav.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.GPSNav.Visible = false;
            this.GPSNav.Width = 2;
            // 
            // GLONav
            // 
            this.GLONav.HeaderText = "GLO Nav";
            this.GLONav.MinimumWidth = 2;
            this.GLONav.Name = "GLONav";
            this.GLONav.ReadOnly = true;
            this.GLONav.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.GLONav.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.GLONav.Visible = false;
            this.GLONav.Width = 2;
            // 
            // file_Paths
            // 
            this.file_Paths.HeaderText = "File Path";
            this.file_Paths.MinimumWidth = 2;
            this.file_Paths.Name = "file_Paths";
            this.file_Paths.ReadOnly = true;
            this.file_Paths.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.file_Paths.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.file_Paths.Visible = false;
            this.file_Paths.Width = 2;
            // 
            // rnxDataImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tableLayoutPanel4);
            this.Controls.Add(this.obsTable);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.lblRoverCounts);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblAbout);
            this.Name = "rnxDataImport";
            this.Size = new System.Drawing.Size(657, 384);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFiles)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.obsTable.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblAbout;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public MetroSuite.MetroComboBox cbxFileType;
        public MetroSuite.MetroButton btnODB;
        private System.Windows.Forms.Panel panel1;
        public MetroFramework.Controls.MetroGrid dgvFiles;
        public System.Windows.Forms.Label lblRoverCounts;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel obsTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblObsSBAS;
        private System.Windows.Forms.Label lblObsGAL;
        private System.Windows.Forms.Label lblObsGPS;
        private System.Windows.Forms.Label lblObsGLO;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label lblEphSBAS;
        private System.Windows.Forms.Label lblEphGAL;
        private System.Windows.Forms.Label lblEphGPS;
        private System.Windows.Forms.Label lblEphGLO;
        private System.Windows.Forms.DataGridViewTextBoxColumn File_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn14;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn19;
        private System.Windows.Forms.DataGridViewTextBoxColumn GPSNav;
        private System.Windows.Forms.DataGridViewTextBoxColumn GLONav;
        private System.Windows.Forms.DataGridViewTextBoxColumn file_Paths;
        public MetroSuite.MetroTextbox tbxFilePath;
    }
}
