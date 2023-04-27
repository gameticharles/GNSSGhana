namespace ghGPS.User_Forms
{
    partial class CreateProject
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateProject));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.chbxIncludeReport = new MetroSuite.MetroChecker();
            this.tbxProjectName = new MetroSuite.MetroTextbox();
            this.tbxFolderPath = new MetroSuite.MetroTextbox();
            this.btnOpenFolderPath = new System.Windows.Forms.Button();
            this.cbxUnits = new MetroSuite.MetroComboBox();
            this.cbxTransformationType = new MetroSuite.MetroComboBox();
            this.cbxProjectType = new MetroSuite.MetroComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblTrans = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ToolTip = new MetroFramework.Components.MetroToolTip();
            this.label6 = new System.Windows.Forms.Label();
            this.cbxBandFreq = new MetroSuite.MetroComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cbxProcessingMode = new MetroSuite.MetroComboBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panel1.Controls.Add(this.panel8);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 425);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(851, 79);
            this.panel1.TabIndex = 1;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.Gainsboro;
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(851, 1);
            this.panel8.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnBack);
            this.panel2.Controls.Add(this.btnNext);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 394);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(851, 31);
            this.panel2.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.AutoEllipsis = true;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
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
            this.btnBack.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnBack.FlatAppearance.BorderSize = 0;
            this.btnBack.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.btnBack.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
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
            // btnNext
            // 
            this.btnNext.AutoEllipsis = true;
            this.btnNext.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnNext.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.btnNext.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNext.ForeColor = System.Drawing.Color.DimGray;
            this.btnNext.Image = ((System.Drawing.Image)(resources.GetObject("btnNext.Image")));
            this.btnNext.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNext.Location = new System.Drawing.Point(759, 0);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(92, 31);
            this.btnNext.TabIndex = 0;
            this.btnNext.Text = " Next   ";
            this.btnNext.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.chbxIncludeReport);
            this.panel3.Controls.Add(this.tbxProjectName);
            this.panel3.Controls.Add(this.tbxFolderPath);
            this.panel3.Controls.Add(this.btnOpenFolderPath);
            this.panel3.Controls.Add(this.cbxUnits);
            this.panel3.Controls.Add(this.cbxTransformationType);
            this.panel3.Controls.Add(this.cbxProcessingMode);
            this.panel3.Controls.Add(this.cbxBandFreq);
            this.panel3.Controls.Add(this.cbxProjectType);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Controls.Add(this.lblTrans);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(851, 394);
            this.panel3.TabIndex = 3;
            // 
            // chbxIncludeReport
            // 
            this.chbxIncludeReport.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chbxIncludeReport.BackColor = System.Drawing.Color.White;
            this.chbxIncludeReport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chbxIncludeReport.CheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.chbxIncludeReport.Checked = true;
            this.chbxIncludeReport.CheckedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.chbxIncludeReport.DefaultColor = System.Drawing.Color.Gray;
            this.chbxIncludeReport.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chbxIncludeReport.ForeColor = System.Drawing.Color.Black;
            this.chbxIncludeReport.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.chbxIncludeReport.Location = new System.Drawing.Point(182, 368);
            this.chbxIncludeReport.Name = "chbxIncludeReport";
            this.chbxIncludeReport.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(165)))));
            this.chbxIncludeReport.Size = new System.Drawing.Size(245, 14);
            this.chbxIncludeReport.Style = MetroSuite.Design.Style.Custom;
            this.chbxIncludeReport.TabIndex = 6;
            this.chbxIncludeReport.Text = "Include cadastral computation report     ";
            this.ToolTip.SetToolTip(this.chbxIncludeReport, "Select this option to include all ");
            this.chbxIncludeReport.Visible = false;
            this.chbxIncludeReport.CheckedChanged += new MetroSuite.MetroChecker.CheckedChangedEventHandler(this.chbxIncludeReport_CheckedChanged);
            // 
            // tbxProjectName
            // 
            this.tbxProjectName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tbxProjectName.AutoStyle = false;
            this.tbxProjectName.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxProjectName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxProjectName.DefaultColor = System.Drawing.Color.White;
            this.tbxProjectName.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxProjectName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxProjectName.ForeColor = System.Drawing.Color.Black;
            this.tbxProjectName.HideSelection = false;
            this.tbxProjectName.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxProjectName.Location = new System.Drawing.Point(182, 108);
            this.tbxProjectName.Name = "tbxProjectName";
            this.tbxProjectName.PasswordChar = '\0';
            this.tbxProjectName.Size = new System.Drawing.Size(298, 23);
            this.tbxProjectName.Style = MetroSuite.Design.Style.Custom;
            this.tbxProjectName.TabIndex = 5;
            this.ToolTip.SetToolTip(this.tbxProjectName, "Enter name of the  project to start");
            // 
            // tbxFolderPath
            // 
            this.tbxFolderPath.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tbxFolderPath.AutoStyle = false;
            this.tbxFolderPath.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxFolderPath.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxFolderPath.DefaultColor = System.Drawing.Color.White;
            this.tbxFolderPath.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxFolderPath.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxFolderPath.ForeColor = System.Drawing.Color.Black;
            this.tbxFolderPath.HideSelection = false;
            this.tbxFolderPath.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxFolderPath.Location = new System.Drawing.Point(182, 161);
            this.tbxFolderPath.Name = "tbxFolderPath";
            this.tbxFolderPath.PasswordChar = '\0';
            this.tbxFolderPath.Size = new System.Drawing.Size(462, 23);
            this.tbxFolderPath.Style = MetroSuite.Design.Style.Custom;
            this.tbxFolderPath.TabIndex = 5;
            this.ToolTip.SetToolTip(this.tbxFolderPath, "Should be your working folder or directory");
            // 
            // btnOpenFolderPath
            // 
            this.btnOpenFolderPath.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOpenFolderPath.AutoEllipsis = true;
            this.btnOpenFolderPath.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenFolderPath.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnOpenFolderPath.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btnOpenFolderPath.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnOpenFolderPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenFolderPath.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenFolderPath.ForeColor = System.Drawing.Color.DimGray;
            this.btnOpenFolderPath.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenFolderPath.Image")));
            this.btnOpenFolderPath.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenFolderPath.Location = new System.Drawing.Point(646, 161);
            this.btnOpenFolderPath.Name = "btnOpenFolderPath";
            this.btnOpenFolderPath.Size = new System.Drawing.Size(26, 23);
            this.btnOpenFolderPath.TabIndex = 4;
            this.btnOpenFolderPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOpenFolderPath.UseVisualStyleBackColor = true;
            this.btnOpenFolderPath.Click += new System.EventHandler(this.btnOpenFolderPath_Click);
            // 
            // cbxUnits
            // 
            this.cbxUnits.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxUnits.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbxUnits.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxUnits.AutoStyle = false;
            this.cbxUnits.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxUnits.DefaultColor = System.Drawing.Color.White;
            this.cbxUnits.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.cbxUnits.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxUnits.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxUnits.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbxUnits.ForeColor = System.Drawing.Color.Black;
            this.cbxUnits.FormattingEnabled = true;
            this.cbxUnits.ItemHeight = 22;
            this.cbxUnits.Items.AddRange(new object[] {
            "INT Feet",
            "Gold Cost Feet",
            "Meters"});
            this.cbxUnits.Location = new System.Drawing.Point(182, 272);
            this.cbxUnits.Name = "cbxUnits";
            this.cbxUnits.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxUnits.Size = new System.Drawing.Size(227, 28);
            this.cbxUnits.Style = MetroSuite.Design.Style.Custom;
            this.cbxUnits.TabIndex = 3;
            this.cbxUnits.SelectedIndexChanged += new System.EventHandler(this.cbxUnits_SelectedIndexChanged);
            // 
            // cbxTransformationType
            // 
            this.cbxTransformationType.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxTransformationType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbxTransformationType.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxTransformationType.AutoStyle = false;
            this.cbxTransformationType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxTransformationType.DefaultColor = System.Drawing.Color.White;
            this.cbxTransformationType.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.cbxTransformationType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxTransformationType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTransformationType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxTransformationType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbxTransformationType.ForeColor = System.Drawing.Color.Black;
            this.cbxTransformationType.FormattingEnabled = true;
            this.cbxTransformationType.ItemHeight = 22;
            this.cbxTransformationType.Items.AddRange(new object[] {
            "GH 3 - Parameters",
            "GH 7 - Parameters",
            "GH 10 - Parameters",
            "GH MRE"});
            this.cbxTransformationType.Location = new System.Drawing.Point(182, 330);
            this.cbxTransformationType.Name = "cbxTransformationType";
            this.cbxTransformationType.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxTransformationType.Size = new System.Drawing.Size(227, 28);
            this.cbxTransformationType.Style = MetroSuite.Design.Style.Custom;
            this.cbxTransformationType.TabIndex = 3;
            this.cbxTransformationType.SelectedIndexChanged += new System.EventHandler(this.cbxTransformationType_SelectedIndexChanged);
            // 
            // cbxProjectType
            // 
            this.cbxProjectType.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxProjectType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbxProjectType.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxProjectType.AutoStyle = false;
            this.cbxProjectType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxProjectType.DefaultColor = System.Drawing.Color.White;
            this.cbxProjectType.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.cbxProjectType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxProjectType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxProjectType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxProjectType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbxProjectType.ForeColor = System.Drawing.Color.Black;
            this.cbxProjectType.FormattingEnabled = true;
            this.cbxProjectType.ItemHeight = 22;
            this.cbxProjectType.Items.AddRange(new object[] {
            "GNSS Processing",
            "Cadastral Report"});
            this.cbxProjectType.Location = new System.Drawing.Point(182, 214);
            this.cbxProjectType.Name = "cbxProjectType";
            this.cbxProjectType.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxProjectType.Size = new System.Drawing.Size(227, 28);
            this.cbxProjectType.Style = MetroSuite.Design.Style.Custom;
            this.cbxProjectType.TabIndex = 3;
            this.cbxProjectType.SelectedIndexChanged += new System.EventHandler(this.cbxProjectType_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoEllipsis = true;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label5.ForeColor = System.Drawing.Color.DimGray;
            this.label5.Location = new System.Drawing.Point(178, 249);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 19);
            this.label5.TabIndex = 1;
            this.label5.Text = "Units:";
            // 
            // lblTrans
            // 
            this.lblTrans.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblTrans.AutoEllipsis = true;
            this.lblTrans.AutoSize = true;
            this.lblTrans.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTrans.ForeColor = System.Drawing.Color.DimGray;
            this.lblTrans.Location = new System.Drawing.Point(178, 307);
            this.lblTrans.Name = "lblTrans";
            this.lblTrans.Size = new System.Drawing.Size(202, 19);
            this.lblTrans.TabIndex = 1;
            this.lblTrans.Text = "Transformation Parameter type:";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoEllipsis = true;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label4.ForeColor = System.Drawing.Color.DimGray;
            this.label4.Location = new System.Drawing.Point(178, 191);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 19);
            this.label4.TabIndex = 1;
            this.label4.Text = "Project type:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoEllipsis = true;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label3.ForeColor = System.Drawing.Color.DimGray;
            this.label3.Location = new System.Drawing.Point(178, 138);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 19);
            this.label3.TabIndex = 1;
            this.label3.Text = "Project directory:";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoEllipsis = true;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(178, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "Project name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(27, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(244, 37);
            this.label2.TabIndex = 1;
            this.label2.Text = "Create New Project";
            // 
            // ToolTip
            // 
            this.ToolTip.Style = MetroFramework.MetroColorStyle.Blue;
            this.ToolTip.StyleManager = null;
            this.ToolTip.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoEllipsis = true;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label6.ForeColor = System.Drawing.Color.DimGray;
            this.label6.Location = new System.Drawing.Point(413, 191);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(110, 19);
            this.label6.TabIndex = 1;
            this.label6.Text = "Band Frequency:";
            // 
            // cbxBandFreq
            // 
            this.cbxBandFreq.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxBandFreq.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbxBandFreq.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxBandFreq.AutoStyle = false;
            this.cbxBandFreq.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxBandFreq.DefaultColor = System.Drawing.Color.White;
            this.cbxBandFreq.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.cbxBandFreq.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxBandFreq.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBandFreq.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxBandFreq.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbxBandFreq.ForeColor = System.Drawing.Color.Black;
            this.cbxBandFreq.FormattingEnabled = true;
            this.cbxBandFreq.ItemHeight = 22;
            this.cbxBandFreq.Items.AddRange(new object[] {
            "L1",
            "L1 + L2",
            "L1 + L2 + L5",
            "L1 + L5"});
            this.cbxBandFreq.Location = new System.Drawing.Point(417, 214);
            this.cbxBandFreq.Name = "cbxBandFreq";
            this.cbxBandFreq.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxBandFreq.Size = new System.Drawing.Size(227, 28);
            this.cbxBandFreq.Style = MetroSuite.Design.Style.Custom;
            this.cbxBandFreq.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoEllipsis = true;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label7.ForeColor = System.Drawing.Color.DimGray;
            this.label7.Location = new System.Drawing.Point(413, 249);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(117, 19);
            this.label7.TabIndex = 1;
            this.label7.Text = "Processing Mode:";
            // 
            // cbxProcessingMode
            // 
            this.cbxProcessingMode.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxProcessingMode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbxProcessingMode.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxProcessingMode.AutoStyle = false;
            this.cbxProcessingMode.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxProcessingMode.DefaultColor = System.Drawing.Color.White;
            this.cbxProcessingMode.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.cbxProcessingMode.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxProcessingMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxProcessingMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxProcessingMode.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbxProcessingMode.ForeColor = System.Drawing.Color.Black;
            this.cbxProcessingMode.FormattingEnabled = true;
            this.cbxProcessingMode.ItemHeight = 22;
            this.cbxProcessingMode.Items.AddRange(new object[] {
            "Single",
            "DGPS/DGNSS",
            "Kinematic",
            "Static DGNSS",
            "Moving Base",
            "Fixed",
            "PPP Kinematic",
            "PPP Static",
            "PPP Fixed "});
            this.cbxProcessingMode.Location = new System.Drawing.Point(417, 272);
            this.cbxProcessingMode.Name = "cbxProcessingMode";
            this.cbxProcessingMode.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxProcessingMode.Size = new System.Drawing.Size(227, 28);
            this.cbxProcessingMode.Style = MetroSuite.Design.Style.Custom;
            this.cbxProcessingMode.TabIndex = 3;
            // 
            // CreateProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "CreateProject";
            this.Size = new System.Drawing.Size(851, 504);
            this.Load += new System.EventHandler(this.CreateProject_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnOpenFolderPath;
        public MetroSuite.MetroComboBox cbxProjectType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public MetroSuite.MetroTextbox tbxFolderPath;
        public MetroSuite.MetroTextbox tbxProjectName;
        public MetroSuite.MetroChecker chbxIncludeReport;
        internal MetroFramework.Components.MetroToolTip ToolTip;
        private System.Windows.Forms.Panel panel8;
        public MetroSuite.MetroComboBox cbxUnits;
        private System.Windows.Forms.Label label5;
        public MetroSuite.MetroComboBox cbxTransformationType;
        private System.Windows.Forms.Label lblTrans;
        public MetroSuite.MetroComboBox cbxBandFreq;
        private System.Windows.Forms.Label label6;
        public MetroSuite.MetroComboBox cbxProcessingMode;
        private System.Windows.Forms.Label label7;
    }
}
