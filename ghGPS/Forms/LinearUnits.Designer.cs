namespace ghGPS.Forms
{
    partial class LinearUnits
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinearUnits));
            this.panel1 = new System.Windows.Forms.Panel();
            this.olvPointListTree = new BrightIdeasSoftware.ObjectListView();
            this.clnUnitName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnMeterPerUnit = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnAutority = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnAuthorityCode = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnAlias = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnAbbreviation = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnRemark = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.panel3 = new System.Windows.Forms.Panel();
            this.metroLabel2 = new MetroSuite.MetroLabel();
            this.tbxLinearUnitName = new MetroSuite.MetroTextbox();
            this.metroLabel6 = new MetroSuite.MetroLabel();
            this.tbxToMeterFactor = new MetroSuite.MetroTextbox();
            this.metroLabel4 = new MetroSuite.MetroLabel();
            this.metroLabel1 = new MetroSuite.MetroLabel();
            this.metroLabel3 = new MetroSuite.MetroLabel();
            this.tbxRemarks = new MetroSuite.MetroTextbox();
            this.tbxAlias = new MetroSuite.MetroTextbox();
            this.tbxLinearUnitAbb = new MetroSuite.MetroTextbox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCancel = new MetroSuite.MetroButton();
            this.btnOK = new MetroSuite.MetroButton();
            this.panel12 = new System.Windows.Forms.Panel();
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
            this.panel1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.panel1.Location = new System.Drawing.Point(1, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(304, 388);
            this.panel1.TabIndex = 3;
            // 
            // olvPointListTree
            // 
            this.olvPointListTree.AllColumns.Add(this.clnUnitName);
            this.olvPointListTree.AllColumns.Add(this.clnMeterPerUnit);
            this.olvPointListTree.AllColumns.Add(this.clnAutority);
            this.olvPointListTree.AllColumns.Add(this.clnAuthorityCode);
            this.olvPointListTree.AllColumns.Add(this.clnAlias);
            this.olvPointListTree.AllColumns.Add(this.clnAbbreviation);
            this.olvPointListTree.AllColumns.Add(this.clnRemark);
            this.olvPointListTree.AllowColumnReorder = true;
            this.olvPointListTree.AllowDrop = true;
            this.olvPointListTree.BackColor = System.Drawing.Color.WhiteSmoke;
            this.olvPointListTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.olvPointListTree.CellEditUseWholeCell = false;
            this.olvPointListTree.CheckedAspectName = "";
            this.olvPointListTree.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clnUnitName,
            this.clnMeterPerUnit,
            this.clnAutority,
            this.clnAuthorityCode,
            this.clnAlias,
            this.clnAbbreviation,
            this.clnRemark});
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
            this.olvPointListTree.Size = new System.Drawing.Size(304, 181);
            this.olvPointListTree.TabIndex = 18;
            this.olvPointListTree.UseCompatibleStateImageBehavior = false;
            this.olvPointListTree.UseHotItem = true;
            this.olvPointListTree.View = System.Windows.Forms.View.Details;
            this.olvPointListTree.SelectedIndexChanged += new System.EventHandler(this.olvPointListTree_SelectedIndexChanged);
            // 
            // clnUnitName
            // 
            this.clnUnitName.Text = "Other Stations";
            this.clnUnitName.Width = 285;
            // 
            // clnMeterPerUnit
            // 
            this.clnMeterPerUnit.Width = 0;
            // 
            // clnAutority
            // 
            this.clnAutority.Width = 0;
            // 
            // clnAuthorityCode
            // 
            this.clnAuthorityCode.Width = 0;
            // 
            // clnAlias
            // 
            this.clnAlias.Width = 0;
            // 
            // clnAbbreviation
            // 
            this.clnAbbreviation.Width = 0;
            // 
            // clnRemark
            // 
            this.clnRemark.Width = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.metroLabel2);
            this.panel3.Controls.Add(this.tbxLinearUnitName);
            this.panel3.Controls.Add(this.metroLabel6);
            this.panel3.Controls.Add(this.tbxToMeterFactor);
            this.panel3.Controls.Add(this.metroLabel4);
            this.panel3.Controls.Add(this.metroLabel1);
            this.panel3.Controls.Add(this.metroLabel3);
            this.panel3.Controls.Add(this.tbxRemarks);
            this.panel3.Controls.Add(this.tbxAlias);
            this.panel3.Controls.Add(this.tbxLinearUnitAbb);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 181);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(304, 207);
            this.panel3.TabIndex = 0;
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.metroLabel2.Location = new System.Drawing.Point(9, 13);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(67, 15);
            this.metroLabel2.TabIndex = 22;
            this.metroLabel2.Text = "Unit Name:";
            // 
            // tbxLinearUnitName
            // 
            this.tbxLinearUnitName.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxLinearUnitName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxLinearUnitName.DefaultColor = System.Drawing.Color.White;
            this.tbxLinearUnitName.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxLinearUnitName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxLinearUnitName.ForeColor = System.Drawing.Color.Black;
            this.tbxLinearUnitName.HideSelection = false;
            this.tbxLinearUnitName.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxLinearUnitName.Location = new System.Drawing.Point(78, 9);
            this.tbxLinearUnitName.Name = "tbxLinearUnitName";
            this.tbxLinearUnitName.PasswordChar = '\0';
            this.tbxLinearUnitName.ReadOnly = true;
            this.tbxLinearUnitName.Size = new System.Drawing.Size(221, 23);
            this.tbxLinearUnitName.TabIndex = 23;
            // 
            // metroLabel6
            // 
            this.metroLabel6.AutoSize = true;
            this.metroLabel6.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroLabel6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.metroLabel6.Location = new System.Drawing.Point(9, 41);
            this.metroLabel6.Name = "metroLabel6";
            this.metroLabel6.Size = new System.Drawing.Size(95, 15);
            this.metroLabel6.TabIndex = 18;
            this.metroLabel6.Text = "To meters factor:";
            // 
            // tbxToMeterFactor
            // 
            this.tbxToMeterFactor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxToMeterFactor.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxToMeterFactor.DefaultColor = System.Drawing.Color.White;
            this.tbxToMeterFactor.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxToMeterFactor.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxToMeterFactor.ForeColor = System.Drawing.Color.Black;
            this.tbxToMeterFactor.HideSelection = false;
            this.tbxToMeterFactor.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxToMeterFactor.Location = new System.Drawing.Point(107, 37);
            this.tbxToMeterFactor.Name = "tbxToMeterFactor";
            this.tbxToMeterFactor.PasswordChar = '\0';
            this.tbxToMeterFactor.ReadOnly = true;
            this.tbxToMeterFactor.Size = new System.Drawing.Size(192, 23);
            this.tbxToMeterFactor.TabIndex = 20;
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.metroLabel4.Location = new System.Drawing.Point(9, 117);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(55, 15);
            this.metroLabel4.TabIndex = 19;
            this.metroLabel4.Text = "Remarks:";
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.metroLabel1.Location = new System.Drawing.Point(70, 99);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(35, 15);
            this.metroLabel1.TabIndex = 19;
            this.metroLabel1.Text = "Alias:";
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.metroLabel3.Location = new System.Drawing.Point(27, 70);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(78, 15);
            this.metroLabel3.TabIndex = 19;
            this.metroLabel3.Text = "Abbreviation:";
            // 
            // tbxRemarks
            // 
            this.tbxRemarks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxRemarks.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxRemarks.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxRemarks.DefaultColor = System.Drawing.Color.White;
            this.tbxRemarks.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxRemarks.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxRemarks.ForeColor = System.Drawing.Color.Black;
            this.tbxRemarks.HideSelection = false;
            this.tbxRemarks.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxRemarks.Location = new System.Drawing.Point(5, 135);
            this.tbxRemarks.Multiline = true;
            this.tbxRemarks.Name = "tbxRemarks";
            this.tbxRemarks.PasswordChar = '\0';
            this.tbxRemarks.ReadOnly = true;
            this.tbxRemarks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxRemarks.Size = new System.Drawing.Size(294, 66);
            this.tbxRemarks.TabIndex = 21;
            // 
            // tbxAlias
            // 
            this.tbxAlias.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxAlias.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxAlias.DefaultColor = System.Drawing.Color.White;
            this.tbxAlias.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxAlias.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxAlias.ForeColor = System.Drawing.Color.Black;
            this.tbxAlias.HideSelection = false;
            this.tbxAlias.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxAlias.Location = new System.Drawing.Point(107, 95);
            this.tbxAlias.Name = "tbxAlias";
            this.tbxAlias.PasswordChar = '\0';
            this.tbxAlias.ReadOnly = true;
            this.tbxAlias.Size = new System.Drawing.Size(192, 23);
            this.tbxAlias.TabIndex = 21;
            // 
            // tbxLinearUnitAbb
            // 
            this.tbxLinearUnitAbb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxLinearUnitAbb.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxLinearUnitAbb.DefaultColor = System.Drawing.Color.White;
            this.tbxLinearUnitAbb.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxLinearUnitAbb.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxLinearUnitAbb.ForeColor = System.Drawing.Color.Black;
            this.tbxLinearUnitAbb.HideSelection = false;
            this.tbxLinearUnitAbb.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxLinearUnitAbb.Location = new System.Drawing.Point(107, 66);
            this.tbxLinearUnitAbb.Name = "tbxLinearUnitAbb";
            this.tbxLinearUnitAbb.PasswordChar = '\0';
            this.tbxLinearUnitAbb.ReadOnly = true;
            this.tbxLinearUnitAbb.Size = new System.Drawing.Size(192, 23);
            this.tbxLinearUnitAbb.TabIndex = 21;
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
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(1, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(304, 32);
            this.label2.TabIndex = 4;
            this.label2.Text = "Linear Units";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnOK);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(1, 421);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(304, 43);
            this.panel2.TabIndex = 5;
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
            this.panel12.TabIndex = 17;
            // 
            // LinearUnits
            // 
            this.AllowResize = false;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(306, 465);
            this.Controls.Add(this.panel12);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LinearUnits";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.State = MetroSuite.MetroForm.FormState.Normal;
            this.Style = MetroSuite.Design.Style.Light;
            this.Text = "Linear Unit";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvPointListTree)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private MetroSuite.MetroButton btnCancel;
        private MetroSuite.MetroButton btnOK;
        private System.Windows.Forms.Panel panel12;
        public BrightIdeasSoftware.ObjectListView olvPointListTree;
        private BrightIdeasSoftware.OLVColumn clnUnitName;
        private BrightIdeasSoftware.OLVColumn clnMeterPerUnit;
        private BrightIdeasSoftware.OLVColumn clnAutority;
        private BrightIdeasSoftware.OLVColumn clnAuthorityCode;
        private BrightIdeasSoftware.OLVColumn clnAlias;
        private BrightIdeasSoftware.OLVColumn clnAbbreviation;
        private BrightIdeasSoftware.OLVColumn clnRemark;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private MetroSuite.MetroLabel metroLabel6;
        private MetroSuite.MetroLabel metroLabel3;
        public MetroSuite.MetroTextbox tbxToMeterFactor;
        public MetroSuite.MetroTextbox tbxLinearUnitAbb;
        private MetroSuite.MetroLabel metroLabel2;
        public MetroSuite.MetroTextbox tbxLinearUnitName;
        private MetroSuite.MetroLabel metroLabel1;
        public MetroSuite.MetroTextbox tbxAlias;
        public MetroSuite.MetroTextbox tbxRemarks;
        private MetroSuite.MetroLabel metroLabel4;
    }
}