namespace ghGPS.Forms
{
    partial class CreateTraversePath
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateTraversePath));
            this.clnEastings1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.clnNorthings1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.cbxClosingStation = new MetroSuite.MetroComboBox();
            this.cbxStartingStation = new MetroSuite.MetroComboBox();
            this.olvPointListTree1 = new BrightIdeasSoftware.ObjectListView();
            this.clnSiteName1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.metroLabel3 = new MetroSuite.MetroLabel();
            this.metroLabel2 = new MetroSuite.MetroLabel();
            this.metroLabel1 = new MetroSuite.MetroLabel();
            this.btnDiscard = new MetroSuite.MetroButton();
            this.btnApply = new MetroSuite.MetroButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.metroLabel4 = new MetroSuite.MetroLabel();
            ((System.ComponentModel.ISupportInitialize)(this.olvPointListTree1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // clnEastings1
            // 
            this.clnEastings1.Width = 0;
            // 
            // clnNorthings1
            // 
            this.clnNorthings1.Width = 0;
            // 
            // cbxClosingStation
            // 
            this.cbxClosingStation.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxClosingStation.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxClosingStation.AutoStyle = false;
            this.cbxClosingStation.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxClosingStation.DefaultColor = System.Drawing.Color.White;
            this.cbxClosingStation.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.cbxClosingStation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxClosingStation.DropDownHeight = 300;
            this.cbxClosingStation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxClosingStation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxClosingStation.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbxClosingStation.ForeColor = System.Drawing.Color.Black;
            this.cbxClosingStation.FormattingEnabled = true;
            this.cbxClosingStation.IntegralHeight = false;
            this.cbxClosingStation.ItemHeight = 22;
            this.cbxClosingStation.Location = new System.Drawing.Point(27, 133);
            this.cbxClosingStation.Name = "cbxClosingStation";
            this.cbxClosingStation.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxClosingStation.Size = new System.Drawing.Size(292, 28);
            this.cbxClosingStation.Style = MetroSuite.Design.Style.Custom;
            this.cbxClosingStation.TabIndex = 16;
            this.cbxClosingStation.SelectedIndexChanged += new System.EventHandler(this.cbxStartingStation_SelectedIndexChanged);
            this.cbxClosingStation.Click += new System.EventHandler(this.cbxStartingStation_Click);
            // 
            // cbxStartingStation
            // 
            this.cbxStartingStation.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxStartingStation.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxStartingStation.AutoStyle = false;
            this.cbxStartingStation.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxStartingStation.DefaultColor = System.Drawing.Color.White;
            this.cbxStartingStation.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.cbxStartingStation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxStartingStation.DropDownHeight = 300;
            this.cbxStartingStation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxStartingStation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxStartingStation.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbxStartingStation.ForeColor = System.Drawing.Color.Black;
            this.cbxStartingStation.FormattingEnabled = true;
            this.cbxStartingStation.IntegralHeight = false;
            this.cbxStartingStation.ItemHeight = 22;
            this.cbxStartingStation.Location = new System.Drawing.Point(27, 78);
            this.cbxStartingStation.Name = "cbxStartingStation";
            this.cbxStartingStation.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxStartingStation.Size = new System.Drawing.Size(292, 28);
            this.cbxStartingStation.Style = MetroSuite.Design.Style.Custom;
            this.cbxStartingStation.TabIndex = 17;
            this.cbxStartingStation.SelectedIndexChanged += new System.EventHandler(this.cbxStartingStation_SelectedIndexChanged);
            this.cbxStartingStation.Click += new System.EventHandler(this.cbxStartingStation_Click);
            // 
            // olvPointListTree1
            // 
            this.olvPointListTree1.AllColumns.Add(this.clnSiteName1);
            this.olvPointListTree1.AllColumns.Add(this.clnEastings1);
            this.olvPointListTree1.AllColumns.Add(this.clnNorthings1);
            this.olvPointListTree1.AllowColumnReorder = true;
            this.olvPointListTree1.AllowDrop = true;
            this.olvPointListTree1.CellEditUseWholeCell = false;
            this.olvPointListTree1.CheckedAspectName = "";
            this.olvPointListTree1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clnSiteName1,
            this.clnEastings1,
            this.clnNorthings1});
            this.olvPointListTree1.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvPointListTree1.EmptyListMsg = "";
            this.olvPointListTree1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.olvPointListTree1.FullRowSelect = true;
            this.olvPointListTree1.GroupWithItemCountFormat = "{0} ({1} SiteID)";
            this.olvPointListTree1.GroupWithItemCountSingularFormat = "{0} ({1} person)";
            this.olvPointListTree1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.olvPointListTree1.HeaderUsesThemes = true;
            this.olvPointListTree1.HideSelection = false;
            this.olvPointListTree1.Location = new System.Drawing.Point(27, 195);
            this.olvPointListTree1.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.olvPointListTree1.Name = "olvPointListTree1";
            this.olvPointListTree1.OverlayText.Alignment = System.Drawing.ContentAlignment.BottomLeft;
            this.olvPointListTree1.OverlayText.Text = "";
            this.olvPointListTree1.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvPointListTree1.ShowCommandMenuOnRightClick = true;
            this.olvPointListTree1.ShowGroups = false;
            this.olvPointListTree1.ShowHeaderInAllViews = false;
            this.olvPointListTree1.ShowImagesOnSubItems = true;
            this.olvPointListTree1.ShowItemToolTips = true;
            this.olvPointListTree1.Size = new System.Drawing.Size(292, 235);
            this.olvPointListTree1.TabIndex = 15;
            this.olvPointListTree1.UseCompatibleStateImageBehavior = false;
            this.olvPointListTree1.UseHotItem = true;
            this.olvPointListTree1.View = System.Windows.Forms.View.Details;
            this.olvPointListTree1.SelectedIndexChanged += new System.EventHandler(this.olvPointListTree_SelectedIndexChanged);
            // 
            // clnSiteName1
            // 
            this.clnSiteName1.Text = "Other Stations";
            this.clnSiteName1.Width = 265;
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.metroLabel3.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.metroLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.metroLabel3.Location = new System.Drawing.Point(23, 108);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(117, 21);
            this.metroLabel3.TabIndex = 12;
            this.metroLabel3.Text = "Closing Station:";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.metroLabel2.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.metroLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.metroLabel2.Location = new System.Drawing.Point(23, 53);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(119, 21);
            this.metroLabel2.TabIndex = 13;
            this.metroLabel2.Text = "Starting Station:";
            // 
            // metroLabel1
            // 
            this.metroLabel1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.metroLabel1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Underline);
            this.metroLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.metroLabel1.Location = new System.Drawing.Point(56, 7);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(231, 32);
            this.metroLabel1.TabIndex = 14;
            this.metroLabel1.Text = "Create Traverse Path";
            // 
            // btnDiscard
            // 
            this.btnDiscard.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDiscard.AutoStyle = false;
            this.btnDiscard.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnDiscard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnDiscard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDiscard.DefaultColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.btnDiscard.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDiscard.DisabledColor = System.Drawing.Color.LightGray;
            this.btnDiscard.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnDiscard.ForeColor = System.Drawing.Color.DimGray;
            this.btnDiscard.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnDiscard.Location = new System.Drawing.Point(235, 441);
            this.btnDiscard.Name = "btnDiscard";
            this.btnDiscard.PressedColor = System.Drawing.Color.Silver;
            this.btnDiscard.Size = new System.Drawing.Size(83, 29);
            this.btnDiscard.Style = MetroSuite.Design.Style.Custom;
            this.btnDiscard.TabIndex = 19;
            this.btnDiscard.Text = "Cancel";
            this.btnDiscard.Click += new System.EventHandler(this.btnDiscard_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnApply.AutoStyle = false;
            this.btnApply.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnApply.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnApply.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApply.DefaultColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.DisabledColor = System.Drawing.Color.LightGray;
            this.btnApply.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnApply.ForeColor = System.Drawing.Color.DimGray;
            this.btnApply.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnApply.Location = new System.Drawing.Point(146, 441);
            this.btnApply.Name = "btnApply";
            this.btnApply.PressedColor = System.Drawing.Color.Silver;
            this.btnApply.Size = new System.Drawing.Size(83, 29);
            this.btnApply.Style = MetroSuite.Design.Style.Custom;
            this.btnApply.TabIndex = 18;
            this.btnApply.Text = "Apply";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnDiscard);
            this.panel1.Controls.Add(this.metroLabel1);
            this.panel1.Controls.Add(this.btnApply);
            this.panel1.Controls.Add(this.cbxClosingStation);
            this.panel1.Controls.Add(this.cbxStartingStation);
            this.panel1.Controls.Add(this.metroLabel4);
            this.panel1.Controls.Add(this.metroLabel3);
            this.panel1.Controls.Add(this.metroLabel2);
            this.panel1.Controls.Add(this.olvPointListTree1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(343, 478);
            this.panel1.TabIndex = 20;
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.metroLabel4.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.metroLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.metroLabel4.Location = new System.Drawing.Point(23, 171);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(311, 21);
            this.metroLabel4.TabIndex = 12;
            this.metroLabel4.Text = "Note: Drag points to rearrange traverse line";
            // 
            // CreateTraversePath
            // 
            this.AllowResize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.ClientSize = new System.Drawing.Size(345, 480);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CreateTraversePath";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.State = MetroSuite.MetroForm.FormState.Normal;
            this.Style = MetroSuite.Design.Style.Custom;
            this.Load += new System.EventHandler(this.CreateTraversePath_Load);
            ((System.ComponentModel.ISupportInitialize)(this.olvPointListTree1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private BrightIdeasSoftware.OLVColumn clnEastings1;
        private BrightIdeasSoftware.OLVColumn clnNorthings1;
        public MetroSuite.MetroComboBox cbxClosingStation;
        public MetroSuite.MetroComboBox cbxStartingStation;
        public BrightIdeasSoftware.ObjectListView olvPointListTree1;
        private BrightIdeasSoftware.OLVColumn clnSiteName1;
        private MetroSuite.MetroLabel metroLabel3;
        private MetroSuite.MetroLabel metroLabel2;
        private MetroSuite.MetroLabel metroLabel1;
        public MetroSuite.MetroButton btnDiscard;
        public MetroSuite.MetroButton btnApply;
        private System.Windows.Forms.Panel panel1;
        private MetroSuite.MetroLabel metroLabel4;
    }
}