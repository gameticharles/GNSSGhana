namespace ghGPS.Forms
{
    partial class ExportResults
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportResults));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.btnCancel = new MetroSuite.MetroButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.chbxPointList = new MetroSuite.MetroChecker();
            this.chbxSummaryList = new MetroSuite.MetroChecker();
            this.chbxLocalGeo = new MetroSuite.MetroChecker();
            this.chbxWGS84_UTM = new MetroSuite.MetroChecker();
            this.chbxTM = new MetroSuite.MetroChecker();
            this.chbxBeacon = new MetroSuite.MetroChecker();
            this.chbxDistance_Bearing = new MetroSuite.MetroChecker();
            this.chbxMapData = new MetroSuite.MetroChecker();
            this.chbxArea = new MetroSuite.MetroChecker();
            this.chbxPlanData = new MetroSuite.MetroChecker();
            this.btnExport = new MetroSuite.MetroButton();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.btnExport);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(219, 340);
            this.panel1.TabIndex = 0;
            // 
            // label14
            // 
            this.label14.AutoEllipsis = true;
            this.label14.Dock = System.Windows.Forms.DockStyle.Top;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 17F);
            this.label14.ForeColor = System.Drawing.Color.DimGray;
            this.label14.Location = new System.Drawing.Point(0, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(219, 51);
            this.label14.TabIndex = 74;
            this.label14.Text = "Export Result";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.HoverColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(21, 308);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnCancel.Size = new System.Drawing.Size(85, 25);
            this.btnCancel.TabIndex = 72;
            this.btnCancel.Text = "Cancel";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(8, 43);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(202, 259);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.chbxPointList);
            this.flowLayoutPanel1.Controls.Add(this.chbxSummaryList);
            this.flowLayoutPanel1.Controls.Add(this.chbxWGS84_UTM);
            this.flowLayoutPanel1.Controls.Add(this.chbxLocalGeo);
            this.flowLayoutPanel1.Controls.Add(this.chbxTM);
            this.flowLayoutPanel1.Controls.Add(this.chbxBeacon);
            this.flowLayoutPanel1.Controls.Add(this.chbxDistance_Bearing);
            this.flowLayoutPanel1.Controls.Add(this.chbxMapData);
            this.flowLayoutPanel1.Controls.Add(this.chbxArea);
            this.flowLayoutPanel1.Controls.Add(this.chbxPlanData);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 19);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(10);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(196, 237);
            this.flowLayoutPanel1.TabIndex = 13;
            // 
            // chbxPointList
            // 
            this.chbxPointList.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chbxPointList.BackColor = System.Drawing.Color.White;
            this.chbxPointList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chbxPointList.CheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.chbxPointList.Checked = true;
            this.chbxPointList.CheckedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.chbxPointList.DefaultColor = System.Drawing.Color.Gray;
            this.chbxPointList.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chbxPointList.ForeColor = System.Drawing.Color.Black;
            this.chbxPointList.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.chbxPointList.Location = new System.Drawing.Point(14, 14);
            this.chbxPointList.Margin = new System.Windows.Forms.Padding(4);
            this.chbxPointList.Name = "chbxPointList";
            this.chbxPointList.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(165)))));
            this.chbxPointList.Size = new System.Drawing.Size(76, 14);
            this.chbxPointList.Style = MetroSuite.Design.Style.Custom;
            this.chbxPointList.TabIndex = 7;
            this.chbxPointList.Text = "Point List";
            // 
            // chbxSummaryList
            // 
            this.chbxSummaryList.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chbxSummaryList.BackColor = System.Drawing.Color.White;
            this.chbxSummaryList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chbxSummaryList.CheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.chbxSummaryList.Checked = true;
            this.chbxSummaryList.CheckedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.chbxSummaryList.DefaultColor = System.Drawing.Color.Gray;
            this.chbxSummaryList.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chbxSummaryList.ForeColor = System.Drawing.Color.Black;
            this.chbxSummaryList.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.chbxSummaryList.Location = new System.Drawing.Point(14, 36);
            this.chbxSummaryList.Margin = new System.Windows.Forms.Padding(4);
            this.chbxSummaryList.Name = "chbxSummaryList";
            this.chbxSummaryList.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(165)))));
            this.chbxSummaryList.Size = new System.Drawing.Size(102, 14);
            this.chbxSummaryList.Style = MetroSuite.Design.Style.Custom;
            this.chbxSummaryList.TabIndex = 8;
            this.chbxSummaryList.Text = "Summary List";
            // 
            // chbxLocalGeo
            // 
            this.chbxLocalGeo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chbxLocalGeo.BackColor = System.Drawing.Color.White;
            this.chbxLocalGeo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chbxLocalGeo.CheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.chbxLocalGeo.Checked = true;
            this.chbxLocalGeo.CheckedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.chbxLocalGeo.DefaultColor = System.Drawing.Color.Gray;
            this.chbxLocalGeo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chbxLocalGeo.ForeColor = System.Drawing.Color.Black;
            this.chbxLocalGeo.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.chbxLocalGeo.Location = new System.Drawing.Point(14, 80);
            this.chbxLocalGeo.Margin = new System.Windows.Forms.Padding(4);
            this.chbxLocalGeo.Name = "chbxLocalGeo";
            this.chbxLocalGeo.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(165)))));
            this.chbxLocalGeo.Size = new System.Drawing.Size(125, 14);
            this.chbxLocalGeo.Style = MetroSuite.Design.Style.Custom;
            this.chbxLocalGeo.TabIndex = 10;
            this.chbxLocalGeo.Text = "Local Geographic";
            // 
            // chbxWGS84_UTM
            // 
            this.chbxWGS84_UTM.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chbxWGS84_UTM.BackColor = System.Drawing.Color.White;
            this.chbxWGS84_UTM.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chbxWGS84_UTM.CheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.chbxWGS84_UTM.Checked = true;
            this.chbxWGS84_UTM.CheckedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.chbxWGS84_UTM.DefaultColor = System.Drawing.Color.Gray;
            this.chbxWGS84_UTM.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chbxWGS84_UTM.ForeColor = System.Drawing.Color.Black;
            this.chbxWGS84_UTM.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.chbxWGS84_UTM.Location = new System.Drawing.Point(14, 58);
            this.chbxWGS84_UTM.Margin = new System.Windows.Forms.Padding(4);
            this.chbxWGS84_UTM.Name = "chbxWGS84_UTM";
            this.chbxWGS84_UTM.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(165)))));
            this.chbxWGS84_UTM.Size = new System.Drawing.Size(110, 14);
            this.chbxWGS84_UTM.Style = MetroSuite.Design.Style.Custom;
            this.chbxWGS84_UTM.TabIndex = 9;
            this.chbxWGS84_UTM.Text = "WGS 84 (UTM)";
            // 
            // chbxTM
            // 
            this.chbxTM.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chbxTM.BackColor = System.Drawing.Color.White;
            this.chbxTM.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chbxTM.CheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.chbxTM.Checked = true;
            this.chbxTM.CheckedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.chbxTM.DefaultColor = System.Drawing.Color.Gray;
            this.chbxTM.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chbxTM.ForeColor = System.Drawing.Color.Black;
            this.chbxTM.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.chbxTM.Location = new System.Drawing.Point(14, 102);
            this.chbxTM.Margin = new System.Windows.Forms.Padding(4);
            this.chbxTM.Name = "chbxTM";
            this.chbxTM.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(165)))));
            this.chbxTM.Size = new System.Drawing.Size(148, 14);
            this.chbxTM.Style = MetroSuite.Design.Style.Custom;
            this.chbxTM.TabIndex = 11;
            this.chbxTM.Text = "Local Projected  (TM)";
            // 
            // chbxBeacon
            // 
            this.chbxBeacon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chbxBeacon.BackColor = System.Drawing.Color.White;
            this.chbxBeacon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chbxBeacon.CheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.chbxBeacon.Checked = true;
            this.chbxBeacon.CheckedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.chbxBeacon.DefaultColor = System.Drawing.Color.Gray;
            this.chbxBeacon.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chbxBeacon.ForeColor = System.Drawing.Color.Black;
            this.chbxBeacon.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.chbxBeacon.Location = new System.Drawing.Point(14, 124);
            this.chbxBeacon.Margin = new System.Windows.Forms.Padding(4);
            this.chbxBeacon.Name = "chbxBeacon";
            this.chbxBeacon.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(165)))));
            this.chbxBeacon.Size = new System.Drawing.Size(65, 14);
            this.chbxBeacon.Style = MetroSuite.Design.Style.Custom;
            this.chbxBeacon.TabIndex = 12;
            this.chbxBeacon.Text = "Beacon";
            // 
            // chbxDistance_Bearing
            // 
            this.chbxDistance_Bearing.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chbxDistance_Bearing.BackColor = System.Drawing.Color.White;
            this.chbxDistance_Bearing.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chbxDistance_Bearing.CheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.chbxDistance_Bearing.Checked = true;
            this.chbxDistance_Bearing.CheckedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.chbxDistance_Bearing.DefaultColor = System.Drawing.Color.Gray;
            this.chbxDistance_Bearing.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chbxDistance_Bearing.ForeColor = System.Drawing.Color.Black;
            this.chbxDistance_Bearing.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.chbxDistance_Bearing.Location = new System.Drawing.Point(14, 146);
            this.chbxDistance_Bearing.Margin = new System.Windows.Forms.Padding(4);
            this.chbxDistance_Bearing.Name = "chbxDistance_Bearing";
            this.chbxDistance_Bearing.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(165)))));
            this.chbxDistance_Bearing.Size = new System.Drawing.Size(137, 14);
            this.chbxDistance_Bearing.Style = MetroSuite.Design.Style.Custom;
            this.chbxDistance_Bearing.TabIndex = 12;
            this.chbxDistance_Bearing.Text = "Distance & Bearing";
            // 
            // chbxMapData
            // 
            this.chbxMapData.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chbxMapData.BackColor = System.Drawing.Color.White;
            this.chbxMapData.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chbxMapData.CheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.chbxMapData.Checked = true;
            this.chbxMapData.CheckedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.chbxMapData.DefaultColor = System.Drawing.Color.Gray;
            this.chbxMapData.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chbxMapData.ForeColor = System.Drawing.Color.Black;
            this.chbxMapData.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.chbxMapData.Location = new System.Drawing.Point(14, 168);
            this.chbxMapData.Margin = new System.Windows.Forms.Padding(4);
            this.chbxMapData.Name = "chbxMapData";
            this.chbxMapData.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(165)))));
            this.chbxMapData.Size = new System.Drawing.Size(80, 14);
            this.chbxMapData.Style = MetroSuite.Design.Style.Custom;
            this.chbxMapData.TabIndex = 12;
            this.chbxMapData.Text = "Map Data";
            // 
            // chbxArea
            // 
            this.chbxArea.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chbxArea.BackColor = System.Drawing.Color.White;
            this.chbxArea.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chbxArea.CheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.chbxArea.Checked = true;
            this.chbxArea.CheckedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.chbxArea.DefaultColor = System.Drawing.Color.Gray;
            this.chbxArea.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chbxArea.ForeColor = System.Drawing.Color.Black;
            this.chbxArea.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.chbxArea.Location = new System.Drawing.Point(14, 190);
            this.chbxArea.Margin = new System.Windows.Forms.Padding(4);
            this.chbxArea.Name = "chbxArea";
            this.chbxArea.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(165)))));
            this.chbxArea.Size = new System.Drawing.Size(127, 14);
            this.chbxArea.Style = MetroSuite.Design.Style.Custom;
            this.chbxArea.TabIndex = 12;
            this.chbxArea.Text = "Area Computaion";
            // 
            // chbxPlanData
            // 
            this.chbxPlanData.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chbxPlanData.BackColor = System.Drawing.Color.White;
            this.chbxPlanData.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chbxPlanData.CheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.chbxPlanData.Checked = true;
            this.chbxPlanData.CheckedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.chbxPlanData.DefaultColor = System.Drawing.Color.Gray;
            this.chbxPlanData.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chbxPlanData.ForeColor = System.Drawing.Color.Black;
            this.chbxPlanData.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.chbxPlanData.Location = new System.Drawing.Point(14, 212);
            this.chbxPlanData.Margin = new System.Windows.Forms.Padding(4);
            this.chbxPlanData.Name = "chbxPlanData";
            this.chbxPlanData.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(165)))));
            this.chbxPlanData.Size = new System.Drawing.Size(78, 14);
            this.chbxPlanData.Style = MetroSuite.Design.Style.Custom;
            this.chbxPlanData.TabIndex = 12;
            this.chbxPlanData.Text = "Plan Data";
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.AutoStyle = false;
            this.btnExport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnExport.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnExport.DefaultColor = System.Drawing.Color.White;
            this.btnExport.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnExport.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnExport.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnExport.ForeColor = System.Drawing.Color.Black;
            this.btnExport.HoverColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(112, 308);
            this.btnExport.Name = "btnExport";
            this.btnExport.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnExport.Size = new System.Drawing.Size(85, 25);
            this.btnExport.TabIndex = 73;
            this.btnExport.Text = "Export";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // ExportResults
            // 
            this.AllowResize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(221, 342);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExportResults";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.State = MetroSuite.MetroForm.FormState.Normal;
            this.Style = MetroSuite.Design.Style.Light;
            this.Text = "Export Results";
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel panel1;
        public MetroSuite.MetroChecker chbxPointList;
        public MetroSuite.MetroChecker chbxDistance_Bearing;
        public MetroSuite.MetroChecker chbxTM;
        public MetroSuite.MetroChecker chbxLocalGeo;
        public MetroSuite.MetroChecker chbxWGS84_UTM;
        public MetroSuite.MetroChecker chbxSummaryList;
        private System.Windows.Forms.GroupBox groupBox1;
        public MetroSuite.MetroChecker chbxMapData;
        public MetroSuite.MetroChecker chbxPlanData;
        public MetroSuite.MetroChecker chbxArea;
        public MetroSuite.MetroChecker chbxBeacon;
        private MetroSuite.MetroButton btnCancel;
        private MetroSuite.MetroButton btnExport;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label14;
    }
}