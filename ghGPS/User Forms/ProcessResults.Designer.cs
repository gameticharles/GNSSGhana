namespace ghGPS.User_Forms
{
    partial class ProcessResults
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessResults));
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblRoverCounts = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnDone = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.rtbxResults = new DevExpress.XtraRichEdit.RichEditControl();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.pnlCustomFolder = new System.Windows.Forms.Panel();
            this.btnPrint = new System.Windows.Forms.Button();
            this.pnlExport = new System.Windows.Forms.Panel();
            this.btnExportResult = new System.Windows.Forms.Button();
            this.pnlCadastralReport = new System.Windows.Forms.Panel();
            this.btnTraversePath = new System.Windows.Forms.Button();
            this.btnDistBear = new System.Windows.Forms.Button();
            this.btnMapData = new System.Windows.Forms.Button();
            this.btnAreaComp = new System.Windows.Forms.Button();
            this.btnPlanData = new System.Windows.Forms.Button();
            this.btnBeacon = new System.Windows.Forms.Button();
            this.btnCadastral = new System.Windows.Forms.Button();
            this.pnlPoints = new System.Windows.Forms.Panel();
            this.btnLocalGeo = new System.Windows.Forms.Button();
            this.btnLocalTM = new System.Windows.Forms.Button();
            this.btnWGS84 = new System.Windows.Forms.Button();
            this.btnPoints = new System.Windows.Forms.Button();
            this.pnlSummary = new System.Windows.Forms.Panel();
            this.btnSummaryList = new System.Windows.Forms.Button();
            this.pnlPointList = new System.Windows.Forms.Panel();
            this.btnPointList = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.pnlCustomFolder.SuspendLayout();
            this.pnlExport.SuspendLayout();
            this.pnlCadastralReport.SuspendLayout();
            this.pnlPoints.SuspendLayout();
            this.pnlSummary.SuspendLayout();
            this.pnlPointList.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblRoverCounts);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.btnBack);
            this.panel2.Controls.Add(this.btnDone);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 473);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(851, 31);
            this.panel2.TabIndex = 5;
            // 
            // lblRoverCounts
            // 
            this.lblRoverCounts.AutoEllipsis = true;
            this.lblRoverCounts.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblRoverCounts.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblRoverCounts.ForeColor = System.Drawing.Color.DimGray;
            this.lblRoverCounts.Location = new System.Drawing.Point(194, 0);
            this.lblRoverCounts.Name = "lblRoverCounts";
            this.lblRoverCounts.Size = new System.Drawing.Size(86, 31);
            this.lblRoverCounts.TabIndex = 3;
            this.lblRoverCounts.Text = "00";
            this.lblRoverCounts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label14
            // 
            this.label14.AutoEllipsis = true;
            this.label14.Dock = System.Windows.Forms.DockStyle.Left;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label14.ForeColor = System.Drawing.Color.DimGray;
            this.label14.Location = new System.Drawing.Point(0, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(194, 31);
            this.label14.TabIndex = 4;
            this.label14.Text = "Number of Pages:";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // btnDone
            // 
            this.btnDone.AutoEllipsis = true;
            this.btnDone.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnDone.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnDone.FlatAppearance.BorderSize = 0;
            this.btnDone.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btnDone.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnDone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDone.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDone.ForeColor = System.Drawing.Color.Green;
            this.btnDone.Image = ((System.Drawing.Image)(resources.GetObject("btnDone.Image")));
            this.btnDone.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDone.Location = new System.Drawing.Point(759, 0);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(92, 31);
            this.btnDone.TabIndex = 0;
            this.btnDone.Text = "  Done   ";
            this.btnDone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.panel6);
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(851, 473);
            this.panel3.TabIndex = 6;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.DarkGray;
            this.panel4.Controls.Add(this.rtbxResults);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(194, 55);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(1);
            this.panel4.Size = new System.Drawing.Size(657, 418);
            this.panel4.TabIndex = 4;
            // 
            // rtbxResults
            // 
            this.rtbxResults.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.rtbxResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbxResults.Location = new System.Drawing.Point(1, 1);
            this.rtbxResults.LookAndFeel.SkinName = "Office 2016 Colorful";
            this.rtbxResults.LookAndFeel.UseDefaultLookAndFeel = false;
            this.rtbxResults.Name = "rtbxResults";
            this.rtbxResults.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
            this.rtbxResults.Options.VerticalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
            this.rtbxResults.ReadOnly = true;
            this.rtbxResults.Size = new System.Drawing.Size(655, 416);
            this.rtbxResults.TabIndex = 0;
            this.rtbxResults.RtfTextChanged += new System.EventHandler(this.rtbxResults_RtfTextChanged);
            this.rtbxResults.ZoomChanged += new System.EventHandler(this.rtbxResults_ZoomChanged);
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panel6.Controls.Add(this.label2);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(194, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(657, 55);
            this.panel6.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(657, 55);
            this.label2.TabIndex = 2;
            this.label2.Text = "Processed Result";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.panel5.Controls.Add(this.pnlCustomFolder);
            this.panel5.Controls.Add(this.pnlExport);
            this.panel5.Controls.Add(this.pnlCadastralReport);
            this.panel5.Controls.Add(this.pnlPoints);
            this.panel5.Controls.Add(this.pnlSummary);
            this.panel5.Controls.Add(this.pnlPointList);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(194, 473);
            this.panel5.TabIndex = 5;
            // 
            // pnlCustomFolder
            // 
            this.pnlCustomFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.pnlCustomFolder.Controls.Add(this.btnPrint);
            this.pnlCustomFolder.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCustomFolder.Location = new System.Drawing.Point(0, 230);
            this.pnlCustomFolder.Name = "pnlCustomFolder";
            this.pnlCustomFolder.Size = new System.Drawing.Size(194, 46);
            this.pnlCustomFolder.TabIndex = 5;
            // 
            // btnPrint
            // 
            this.btnPrint.AutoEllipsis = true;
            this.btnPrint.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnPrint.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPrint.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnPrint.FlatAppearance.BorderSize = 0;
            this.btnPrint.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnPrint.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnPrint.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnPrint.Image = global::ghGPS.Properties.Resources.icons8_Print_26px;
            this.btnPrint.Location = new System.Drawing.Point(0, 0);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(194, 45);
            this.btnPrint.TabIndex = 6;
            this.btnPrint.Text = "   Print                      ";
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // pnlExport
            // 
            this.pnlExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.pnlExport.Controls.Add(this.btnExportResult);
            this.pnlExport.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlExport.Location = new System.Drawing.Point(0, 184);
            this.pnlExport.Name = "pnlExport";
            this.pnlExport.Size = new System.Drawing.Size(194, 46);
            this.pnlExport.TabIndex = 7;
            // 
            // btnExportResult
            // 
            this.btnExportResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnExportResult.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnExportResult.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnExportResult.FlatAppearance.BorderSize = 0;
            this.btnExportResult.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnExportResult.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnExportResult.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnExportResult.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportResult.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnExportResult.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnExportResult.Image = global::ghGPS.Properties.Resources.icons8_Export_26px_2;
            this.btnExportResult.Location = new System.Drawing.Point(0, 0);
            this.btnExportResult.Name = "btnExportResult";
            this.btnExportResult.Size = new System.Drawing.Size(194, 45);
            this.btnExportResult.TabIndex = 0;
            this.btnExportResult.Text = "   Export                   ";
            this.btnExportResult.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExportResult.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExportResult.UseVisualStyleBackColor = false;
            this.btnExportResult.Click += new System.EventHandler(this.btnExportResult_Click);
            // 
            // pnlCadastralReport
            // 
            this.pnlCadastralReport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.pnlCadastralReport.Controls.Add(this.btnTraversePath);
            this.pnlCadastralReport.Controls.Add(this.btnDistBear);
            this.pnlCadastralReport.Controls.Add(this.btnMapData);
            this.pnlCadastralReport.Controls.Add(this.btnAreaComp);
            this.pnlCadastralReport.Controls.Add(this.btnPlanData);
            this.pnlCadastralReport.Controls.Add(this.btnBeacon);
            this.pnlCadastralReport.Controls.Add(this.btnCadastral);
            this.pnlCadastralReport.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCadastralReport.Location = new System.Drawing.Point(0, 138);
            this.pnlCadastralReport.Name = "pnlCadastralReport";
            this.pnlCadastralReport.Size = new System.Drawing.Size(194, 46);
            this.pnlCadastralReport.TabIndex = 2;
            // 
            // btnTraversePath
            // 
            this.btnTraversePath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnTraversePath.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTraversePath.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnTraversePath.FlatAppearance.BorderSize = 0;
            this.btnTraversePath.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnTraversePath.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnTraversePath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTraversePath.Image = ((System.Drawing.Image)(resources.GetObject("btnTraversePath.Image")));
            this.btnTraversePath.Location = new System.Drawing.Point(181, 11);
            this.btnTraversePath.Name = "btnTraversePath";
            this.btnTraversePath.Size = new System.Drawing.Size(12, 23);
            this.btnTraversePath.TabIndex = 9;
            this.btnTraversePath.UseVisualStyleBackColor = false;
            this.btnTraversePath.Visible = false;
            this.btnTraversePath.Click += new System.EventHandler(this.btnTraversePath_Click);
            this.btnTraversePath.MouseEnter += new System.EventHandler(this.btnMenuEdit_MouseEnter);
            this.btnTraversePath.MouseLeave += new System.EventHandler(this.btnMenuEdit_MouseLeave);
            // 
            // btnDistBear
            // 
            this.btnDistBear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDistBear.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnDistBear.FlatAppearance.BorderSize = 0;
            this.btnDistBear.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnDistBear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnDistBear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDistBear.ForeColor = System.Drawing.Color.LightGray;
            this.btnDistBear.Image = ((System.Drawing.Image)(resources.GetObject("btnDistBear.Image")));
            this.btnDistBear.Location = new System.Drawing.Point(0, 88);
            this.btnDistBear.Name = "btnDistBear";
            this.btnDistBear.Size = new System.Drawing.Size(194, 38);
            this.btnDistBear.TabIndex = 10;
            this.btnDistBear.Text = "       Distance && Bearing";
            this.btnDistBear.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDistBear.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDistBear.UseVisualStyleBackColor = true;
            this.btnDistBear.Click += new System.EventHandler(this.btnDistBear_Click);
            // 
            // btnMapData
            // 
            this.btnMapData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMapData.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnMapData.FlatAppearance.BorderSize = 0;
            this.btnMapData.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnMapData.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnMapData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMapData.ForeColor = System.Drawing.Color.LightGray;
            this.btnMapData.Image = ((System.Drawing.Image)(resources.GetObject("btnMapData.Image")));
            this.btnMapData.Location = new System.Drawing.Point(0, 208);
            this.btnMapData.Name = "btnMapData";
            this.btnMapData.Size = new System.Drawing.Size(194, 38);
            this.btnMapData.TabIndex = 7;
            this.btnMapData.Text = "       Map Data            ";
            this.btnMapData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMapData.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnMapData.UseVisualStyleBackColor = true;
            this.btnMapData.Click += new System.EventHandler(this.btnMapData_Click);
            // 
            // btnAreaComp
            // 
            this.btnAreaComp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAreaComp.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnAreaComp.FlatAppearance.BorderSize = 0;
            this.btnAreaComp.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnAreaComp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnAreaComp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAreaComp.ForeColor = System.Drawing.Color.LightGray;
            this.btnAreaComp.Image = ((System.Drawing.Image)(resources.GetObject("btnAreaComp.Image")));
            this.btnAreaComp.Location = new System.Drawing.Point(0, 168);
            this.btnAreaComp.Name = "btnAreaComp";
            this.btnAreaComp.Size = new System.Drawing.Size(194, 38);
            this.btnAreaComp.TabIndex = 8;
            this.btnAreaComp.Text = "       Area Computaion";
            this.btnAreaComp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAreaComp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAreaComp.UseVisualStyleBackColor = true;
            this.btnAreaComp.Click += new System.EventHandler(this.btnAreaComp_Click);
            // 
            // btnPlanData
            // 
            this.btnPlanData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlanData.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnPlanData.FlatAppearance.BorderSize = 0;
            this.btnPlanData.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnPlanData.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnPlanData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlanData.ForeColor = System.Drawing.Color.LightGray;
            this.btnPlanData.Image = ((System.Drawing.Image)(resources.GetObject("btnPlanData.Image")));
            this.btnPlanData.Location = new System.Drawing.Point(0, 128);
            this.btnPlanData.Name = "btnPlanData";
            this.btnPlanData.Size = new System.Drawing.Size(194, 38);
            this.btnPlanData.TabIndex = 6;
            this.btnPlanData.Text = "       Plan Data             ";
            this.btnPlanData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPlanData.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPlanData.UseVisualStyleBackColor = true;
            this.btnPlanData.Click += new System.EventHandler(this.btnPlanData_Click);
            // 
            // btnBeacon
            // 
            this.btnBeacon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBeacon.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnBeacon.FlatAppearance.BorderSize = 0;
            this.btnBeacon.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnBeacon.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnBeacon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBeacon.ForeColor = System.Drawing.Color.LightGray;
            this.btnBeacon.Image = ((System.Drawing.Image)(resources.GetObject("btnBeacon.Image")));
            this.btnBeacon.Location = new System.Drawing.Point(0, 48);
            this.btnBeacon.Name = "btnBeacon";
            this.btnBeacon.Size = new System.Drawing.Size(194, 38);
            this.btnBeacon.TabIndex = 5;
            this.btnBeacon.Text = "       Beacon                    ";
            this.btnBeacon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBeacon.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBeacon.UseVisualStyleBackColor = true;
            this.btnBeacon.Click += new System.EventHandler(this.btnBeacon_Click);
            // 
            // btnCadastral
            // 
            this.btnCadastral.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnCadastral.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCadastral.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnCadastral.FlatAppearance.BorderSize = 0;
            this.btnCadastral.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnCadastral.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnCadastral.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnCadastral.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCadastral.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCadastral.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnCadastral.Image = global::ghGPS.Properties.Resources.icons8_Documents_26px;
            this.btnCadastral.Location = new System.Drawing.Point(0, 0);
            this.btnCadastral.Name = "btnCadastral";
            this.btnCadastral.Size = new System.Drawing.Size(194, 45);
            this.btnCadastral.TabIndex = 0;
            this.btnCadastral.Text = "   Cadastral Reports";
            this.btnCadastral.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCadastral.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCadastral.UseVisualStyleBackColor = false;
            this.btnCadastral.Click += new System.EventHandler(this.btnCadastral_Click);
            this.btnCadastral.MouseEnter += new System.EventHandler(this.btnMenuEdit_MouseEnter);
            this.btnCadastral.MouseLeave += new System.EventHandler(this.btnMenuEdit_MouseLeave);
            // 
            // pnlPoints
            // 
            this.pnlPoints.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.pnlPoints.Controls.Add(this.btnLocalGeo);
            this.pnlPoints.Controls.Add(this.btnLocalTM);
            this.pnlPoints.Controls.Add(this.btnWGS84);
            this.pnlPoints.Controls.Add(this.btnPoints);
            this.pnlPoints.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPoints.Location = new System.Drawing.Point(0, 92);
            this.pnlPoints.Name = "pnlPoints";
            this.pnlPoints.Size = new System.Drawing.Size(194, 46);
            this.pnlPoints.TabIndex = 8;
            // 
            // btnLocalGeo
            // 
            this.btnLocalGeo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLocalGeo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnLocalGeo.FlatAppearance.BorderSize = 0;
            this.btnLocalGeo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnLocalGeo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnLocalGeo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLocalGeo.ForeColor = System.Drawing.Color.LightGray;
            this.btnLocalGeo.Image = ((System.Drawing.Image)(resources.GetObject("btnLocalGeo.Image")));
            this.btnLocalGeo.Location = new System.Drawing.Point(0, 92);
            this.btnLocalGeo.Name = "btnLocalGeo";
            this.btnLocalGeo.Size = new System.Drawing.Size(194, 38);
            this.btnLocalGeo.TabIndex = 11;
            this.btnLocalGeo.Text = "       Local Geographic         ";
            this.btnLocalGeo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLocalGeo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLocalGeo.UseVisualStyleBackColor = true;
            this.btnLocalGeo.Click += new System.EventHandler(this.btnLocalGeo_Click);
            // 
            // btnLocalTM
            // 
            this.btnLocalTM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLocalTM.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnLocalTM.FlatAppearance.BorderSize = 0;
            this.btnLocalTM.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnLocalTM.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnLocalTM.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLocalTM.ForeColor = System.Drawing.Color.LightGray;
            this.btnLocalTM.Image = ((System.Drawing.Image)(resources.GetObject("btnLocalTM.Image")));
            this.btnLocalTM.Location = new System.Drawing.Point(0, 136);
            this.btnLocalTM.Name = "btnLocalTM";
            this.btnLocalTM.Size = new System.Drawing.Size(194, 38);
            this.btnLocalTM.TabIndex = 11;
            this.btnLocalTM.Text = "       Local Projected  (TM)";
            this.btnLocalTM.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLocalTM.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLocalTM.UseVisualStyleBackColor = true;
            this.btnLocalTM.Click += new System.EventHandler(this.btnLocalTM_Click);
            // 
            // btnWGS84
            // 
            this.btnWGS84.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWGS84.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnWGS84.FlatAppearance.BorderSize = 0;
            this.btnWGS84.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnWGS84.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnWGS84.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWGS84.ForeColor = System.Drawing.Color.LightGray;
            this.btnWGS84.Image = ((System.Drawing.Image)(resources.GetObject("btnWGS84.Image")));
            this.btnWGS84.Location = new System.Drawing.Point(0, 48);
            this.btnWGS84.Name = "btnWGS84";
            this.btnWGS84.Size = new System.Drawing.Size(194, 38);
            this.btnWGS84.TabIndex = 11;
            this.btnWGS84.Text = "       WGS 84 (UTM)         ";
            this.btnWGS84.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnWGS84.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnWGS84.UseVisualStyleBackColor = true;
            this.btnWGS84.Click += new System.EventHandler(this.btnWGS84_Click);
            // 
            // btnPoints
            // 
            this.btnPoints.AutoEllipsis = true;
            this.btnPoints.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnPoints.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnPoints.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPoints.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnPoints.FlatAppearance.BorderSize = 0;
            this.btnPoints.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnPoints.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnPoints.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnPoints.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPoints.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnPoints.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnPoints.Image = ((System.Drawing.Image)(resources.GetObject("btnPoints.Image")));
            this.btnPoints.Location = new System.Drawing.Point(0, 0);
            this.btnPoints.Name = "btnPoints";
            this.btnPoints.Size = new System.Drawing.Size(194, 45);
            this.btnPoints.TabIndex = 6;
            this.btnPoints.Text = "   Points                    ";
            this.btnPoints.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPoints.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPoints.UseVisualStyleBackColor = false;
            this.btnPoints.Click += new System.EventHandler(this.btnPoints_Click_1);
            // 
            // pnlSummary
            // 
            this.pnlSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.pnlSummary.Controls.Add(this.btnSummaryList);
            this.pnlSummary.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSummary.Location = new System.Drawing.Point(0, 46);
            this.pnlSummary.Name = "pnlSummary";
            this.pnlSummary.Size = new System.Drawing.Size(194, 46);
            this.pnlSummary.TabIndex = 1;
            // 
            // btnSummaryList
            // 
            this.btnSummaryList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnSummaryList.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSummaryList.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnSummaryList.FlatAppearance.BorderSize = 0;
            this.btnSummaryList.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnSummaryList.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnSummaryList.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnSummaryList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSummaryList.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSummaryList.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnSummaryList.Image = global::ghGPS.Properties.Resources.icons8_Transaction_List_26px;
            this.btnSummaryList.Location = new System.Drawing.Point(0, 0);
            this.btnSummaryList.Name = "btnSummaryList";
            this.btnSummaryList.Size = new System.Drawing.Size(194, 45);
            this.btnSummaryList.TabIndex = 0;
            this.btnSummaryList.Text = "   Summary List       ";
            this.btnSummaryList.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSummaryList.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSummaryList.UseVisualStyleBackColor = false;
            this.btnSummaryList.Click += new System.EventHandler(this.btnSummaryList_Click);
            // 
            // pnlPointList
            // 
            this.pnlPointList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.pnlPointList.Controls.Add(this.btnPointList);
            this.pnlPointList.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPointList.Location = new System.Drawing.Point(0, 0);
            this.pnlPointList.Name = "pnlPointList";
            this.pnlPointList.Size = new System.Drawing.Size(194, 46);
            this.pnlPointList.TabIndex = 6;
            // 
            // btnPointList
            // 
            this.btnPointList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnPointList.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPointList.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnPointList.FlatAppearance.BorderSize = 0;
            this.btnPointList.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnPointList.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnPointList.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnPointList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPointList.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnPointList.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnPointList.Image = global::ghGPS.Properties.Resources.icons8_List_26px;
            this.btnPointList.Location = new System.Drawing.Point(0, 0);
            this.btnPointList.Name = "btnPointList";
            this.btnPointList.Size = new System.Drawing.Size(194, 45);
            this.btnPointList.TabIndex = 0;
            this.btnPointList.Text = "   Point List              ";
            this.btnPointList.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPointList.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPointList.UseVisualStyleBackColor = false;
            this.btnPointList.Click += new System.EventHandler(this.btnPointList_Click);
            // 
            // ProcessResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Name = "ProcessResults";
            this.Size = new System.Drawing.Size(851, 504);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.pnlCustomFolder.ResumeLayout(false);
            this.pnlExport.ResumeLayout(false);
            this.pnlCadastralReport.ResumeLayout(false);
            this.pnlPoints.ResumeLayout(false);
            this.pnlSummary.ResumeLayout(false);
            this.pnlPointList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel pnlCustomFolder;
        public System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnCadastral;
        private System.Windows.Forms.Button btnSummaryList;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button btnPointList;
        private System.Windows.Forms.Panel pnlExport;
        private System.Windows.Forms.Button btnExportResult;
        private System.Windows.Forms.Button btnDistBear;
        private System.Windows.Forms.Button btnMapData;
        private System.Windows.Forms.Button btnAreaComp;
        private System.Windows.Forms.Button btnPlanData;
        private System.Windows.Forms.Button btnBeacon;
        public System.Windows.Forms.Button btnPoints;
        public System.Windows.Forms.Button btnTraversePath;
        private System.Windows.Forms.Button btnWGS84;
        private System.Windows.Forms.Button btnLocalGeo;
        private System.Windows.Forms.Button btnLocalTM;
        public System.Windows.Forms.Panel pnlCadastralReport;
        public DevExpress.XtraRichEdit.RichEditControl rtbxResults;
        public System.Windows.Forms.Label lblRoverCounts;
        private System.Windows.Forms.Label label14;
        public System.Windows.Forms.Panel pnlSummary;
        public System.Windows.Forms.Panel pnlPointList;
        public System.Windows.Forms.Panel pnlPoints;
    }
}
