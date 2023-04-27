namespace ghGPS.User_Forms
{
    partial class GlobalSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GlobalSettings));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.pnlSide = new System.Windows.Forms.Panel();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.pnlCoordinateSystem = new System.Windows.Forms.Panel();
            this.btnSubDatumGrid = new System.Windows.Forms.Button();
            this.btnSubEphemeris = new System.Windows.Forms.Button();
            this.btnSubMeasurements = new System.Windows.Forms.Button();
            this.btnCoordinateSystem = new System.Windows.Forms.Button();
            this.btnSubEllipsoid = new System.Windows.Forms.Button();
            this.pnlGenralSettings = new System.Windows.Forms.Panel();
            this.btnGeneralSettings = new System.Windows.Forms.Button();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnBack = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ToolTip = new MetroFramework.Components.MetroToolTip();
            this.pnlGNSS_Setting = new System.Windows.Forms.Panel();
            this.btnGNSS_Settings = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCadastral = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.pnlSide.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.pnlCoordinateSystem.SuspendLayout();
            this.pnlGenralSettings.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlGNSS_Setting.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pnlContainer);
            this.panel1.Controls.Add(this.pnlSide);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(851, 504);
            this.panel1.TabIndex = 4;
            // 
            // pnlContainer
            // 
            this.pnlContainer.AutoScroll = true;
            this.pnlContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContainer.Location = new System.Drawing.Point(274, 0);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(577, 504);
            this.pnlContainer.TabIndex = 4;
            // 
            // pnlSide
            // 
            this.pnlSide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.pnlSide.Controls.Add(this.pnlButtons);
            this.pnlSide.Controls.Add(this.pnlHeader);
            this.pnlSide.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSide.Location = new System.Drawing.Point(0, 0);
            this.pnlSide.Name = "pnlSide";
            this.pnlSide.Size = new System.Drawing.Size(274, 504);
            this.pnlSide.TabIndex = 3;
            // 
            // pnlButtons
            // 
            this.pnlButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.pnlButtons.Controls.Add(this.panel2);
            this.pnlButtons.Controls.Add(this.pnlGNSS_Setting);
            this.pnlButtons.Controls.Add(this.pnlCoordinateSystem);
            this.pnlButtons.Controls.Add(this.pnlGenralSettings);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlButtons.Location = new System.Drawing.Point(0, 101);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(274, 403);
            this.pnlButtons.TabIndex = 1;
            // 
            // pnlCoordinateSystem
            // 
            this.pnlCoordinateSystem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.pnlCoordinateSystem.Controls.Add(this.btnSubDatumGrid);
            this.pnlCoordinateSystem.Controls.Add(this.btnSubEphemeris);
            this.pnlCoordinateSystem.Controls.Add(this.btnSubMeasurements);
            this.pnlCoordinateSystem.Controls.Add(this.btnCoordinateSystem);
            this.pnlCoordinateSystem.Controls.Add(this.btnSubEllipsoid);
            this.pnlCoordinateSystem.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCoordinateSystem.Location = new System.Drawing.Point(0, 46);
            this.pnlCoordinateSystem.Name = "pnlCoordinateSystem";
            this.pnlCoordinateSystem.Size = new System.Drawing.Size(274, 215);
            this.pnlCoordinateSystem.TabIndex = 19;
            // 
            // btnSubDatumGrid
            // 
            this.btnSubDatumGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubDatumGrid.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnSubDatumGrid.FlatAppearance.BorderSize = 0;
            this.btnSubDatumGrid.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnSubDatumGrid.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnSubDatumGrid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubDatumGrid.ForeColor = System.Drawing.Color.LightGray;
            this.btnSubDatumGrid.Location = new System.Drawing.Point(0, 90);
            this.btnSubDatumGrid.Name = "btnSubDatumGrid";
            this.btnSubDatumGrid.Size = new System.Drawing.Size(274, 38);
            this.btnSubDatumGrid.TabIndex = 7;
            this.btnSubDatumGrid.Text = "Ellipsoids                            ";
            this.btnSubDatumGrid.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSubDatumGrid.UseVisualStyleBackColor = true;
            // 
            // btnSubEphemeris
            // 
            this.btnSubEphemeris.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubEphemeris.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnSubEphemeris.FlatAppearance.BorderSize = 0;
            this.btnSubEphemeris.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnSubEphemeris.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnSubEphemeris.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubEphemeris.ForeColor = System.Drawing.Color.LightGray;
            this.btnSubEphemeris.Location = new System.Drawing.Point(0, 129);
            this.btnSubEphemeris.Name = "btnSubEphemeris";
            this.btnSubEphemeris.Size = new System.Drawing.Size(274, 38);
            this.btnSubEphemeris.TabIndex = 8;
            this.btnSubEphemeris.Text = "Datum Grid                        ";
            this.btnSubEphemeris.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSubEphemeris.UseVisualStyleBackColor = true;
            // 
            // btnSubMeasurements
            // 
            this.btnSubMeasurements.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubMeasurements.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnSubMeasurements.FlatAppearance.BorderSize = 0;
            this.btnSubMeasurements.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnSubMeasurements.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnSubMeasurements.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubMeasurements.ForeColor = System.Drawing.Color.LightGray;
            this.btnSubMeasurements.Location = new System.Drawing.Point(0, 168);
            this.btnSubMeasurements.Name = "btnSubMeasurements";
            this.btnSubMeasurements.Size = new System.Drawing.Size(274, 38);
            this.btnSubMeasurements.TabIndex = 6;
            this.btnSubMeasurements.Text = "Transformation Parameter ";
            this.btnSubMeasurements.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSubMeasurements.UseVisualStyleBackColor = true;
            // 
            // btnCoordinateSystem
            // 
            this.btnCoordinateSystem.AutoEllipsis = true;
            this.btnCoordinateSystem.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCoordinateSystem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnCoordinateSystem.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCoordinateSystem.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnCoordinateSystem.FlatAppearance.BorderSize = 0;
            this.btnCoordinateSystem.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnCoordinateSystem.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnCoordinateSystem.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnCoordinateSystem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCoordinateSystem.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCoordinateSystem.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnCoordinateSystem.Location = new System.Drawing.Point(0, 0);
            this.btnCoordinateSystem.Name = "btnCoordinateSystem";
            this.btnCoordinateSystem.Size = new System.Drawing.Size(274, 45);
            this.btnCoordinateSystem.TabIndex = 0;
            this.btnCoordinateSystem.Text = "Coordinate System      ";
            this.btnCoordinateSystem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCoordinateSystem.UseVisualStyleBackColor = false;
            this.btnCoordinateSystem.Click += new System.EventHandler(this.btnCoordinateSystem_Click);
            // 
            // btnSubEllipsoid
            // 
            this.btnSubEllipsoid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubEllipsoid.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.btnSubEllipsoid.FlatAppearance.BorderSize = 0;
            this.btnSubEllipsoid.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnSubEllipsoid.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(127)))), ((int)(((byte)(183)))));
            this.btnSubEllipsoid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubEllipsoid.ForeColor = System.Drawing.Color.LightGray;
            this.btnSubEllipsoid.Location = new System.Drawing.Point(0, 51);
            this.btnSubEllipsoid.Name = "btnSubEllipsoid";
            this.btnSubEllipsoid.Size = new System.Drawing.Size(274, 38);
            this.btnSubEllipsoid.TabIndex = 10;
            this.btnSubEllipsoid.Text = "Linear Units                        ";
            this.btnSubEllipsoid.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSubEllipsoid.UseVisualStyleBackColor = true;
            // 
            // pnlGenralSettings
            // 
            this.pnlGenralSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.pnlGenralSettings.Controls.Add(this.btnGeneralSettings);
            this.pnlGenralSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGenralSettings.Location = new System.Drawing.Point(0, 0);
            this.pnlGenralSettings.Name = "pnlGenralSettings";
            this.pnlGenralSettings.Size = new System.Drawing.Size(274, 46);
            this.pnlGenralSettings.TabIndex = 16;
            // 
            // btnGeneralSettings
            // 
            this.btnGeneralSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnGeneralSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnGeneralSettings.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnGeneralSettings.FlatAppearance.BorderSize = 0;
            this.btnGeneralSettings.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnGeneralSettings.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnGeneralSettings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnGeneralSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGeneralSettings.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnGeneralSettings.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnGeneralSettings.Location = new System.Drawing.Point(0, 0);
            this.btnGeneralSettings.Name = "btnGeneralSettings";
            this.btnGeneralSettings.Size = new System.Drawing.Size(274, 45);
            this.btnGeneralSettings.TabIndex = 0;
            this.btnGeneralSettings.Text = "General                         ";
            this.btnGeneralSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGeneralSettings.UseVisualStyleBackColor = false;
            this.btnGeneralSettings.Click += new System.EventHandler(this.btnGeneralSettings_Click);
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.pnlHeader.Controls.Add(this.panel5);
            this.pnlHeader.Controls.Add(this.btnBack);
            this.pnlHeader.Controls.Add(this.label2);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(274, 101);
            this.pnlHeader.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(0, 100);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(274, 1);
            this.panel5.TabIndex = 0;
            // 
            // btnBack
            // 
            this.btnBack.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnBack.FlatAppearance.BorderSize = 0;
            this.btnBack.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnBack.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.ForeColor = System.Drawing.Color.DimGray;
            this.btnBack.Image = ((System.Drawing.Image)(resources.GetObject("btnBack.Image")));
            this.btnBack.Location = new System.Drawing.Point(0, -1);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(40, 35);
            this.btnBack.TabIndex = 9;
            this.ToolTip.SetToolTip(this.btnBack, "Go back to home");
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label2.Location = new System.Drawing.Point(7, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 37);
            this.label2.TabIndex = 2;
            this.label2.Text = "Settings";
            // 
            // ToolTip
            // 
            this.ToolTip.Style = MetroFramework.MetroColorStyle.Blue;
            this.ToolTip.StyleManager = null;
            this.ToolTip.Theme = MetroFramework.MetroThemeStyle.Default;
            // 
            // pnlGNSS_Setting
            // 
            this.pnlGNSS_Setting.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.pnlGNSS_Setting.Controls.Add(this.btnGNSS_Settings);
            this.pnlGNSS_Setting.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGNSS_Setting.Location = new System.Drawing.Point(0, 261);
            this.pnlGNSS_Setting.Name = "pnlGNSS_Setting";
            this.pnlGNSS_Setting.Size = new System.Drawing.Size(274, 46);
            this.pnlGNSS_Setting.TabIndex = 20;
            // 
            // btnGNSS_Settings
            // 
            this.btnGNSS_Settings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnGNSS_Settings.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnGNSS_Settings.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnGNSS_Settings.FlatAppearance.BorderSize = 0;
            this.btnGNSS_Settings.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnGNSS_Settings.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnGNSS_Settings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnGNSS_Settings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGNSS_Settings.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnGNSS_Settings.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnGNSS_Settings.Location = new System.Drawing.Point(0, 0);
            this.btnGNSS_Settings.Name = "btnGNSS_Settings";
            this.btnGNSS_Settings.Size = new System.Drawing.Size(274, 45);
            this.btnGNSS_Settings.TabIndex = 0;
            this.btnGNSS_Settings.Text = "GNSS Settings              ";
            this.btnGNSS_Settings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGNSS_Settings.UseVisualStyleBackColor = false;
            this.btnGNSS_Settings.Click += new System.EventHandler(this.btnGNSS_Settings_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(115)))), ((int)(((byte)(164)))));
            this.panel2.Controls.Add(this.btnCadastral);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 307);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(274, 46);
            this.panel2.TabIndex = 21;
            // 
            // btnCadastral
            // 
            this.btnCadastral.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnCadastral.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCadastral.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.btnCadastral.FlatAppearance.BorderSize = 0;
            this.btnCadastral.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnCadastral.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(125)))), ((int)(((byte)(170)))));
            this.btnCadastral.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(199)))));
            this.btnCadastral.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCadastral.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCadastral.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnCadastral.Location = new System.Drawing.Point(0, 0);
            this.btnCadastral.Name = "btnCadastral";
            this.btnCadastral.Size = new System.Drawing.Size(274, 45);
            this.btnCadastral.TabIndex = 0;
            this.btnCadastral.Text = "Cadastral Settings        ";
            this.btnCadastral.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCadastral.UseVisualStyleBackColor = false;
            this.btnCadastral.Click += new System.EventHandler(this.btnCadastral_Click);
            // 
            // GlobalSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel1);
            this.Name = "GlobalSettings";
            this.Size = new System.Drawing.Size(851, 504);
            this.panel1.ResumeLayout(false);
            this.pnlSide.ResumeLayout(false);
            this.pnlButtons.ResumeLayout(false);
            this.pnlCoordinateSystem.ResumeLayout(false);
            this.pnlGenralSettings.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlGNSS_Setting.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlContainer;
        private System.Windows.Forms.Panel pnlSide;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label label2;
        internal MetroFramework.Components.MetroToolTip ToolTip;
        public System.Windows.Forms.Panel pnlCoordinateSystem;
        private System.Windows.Forms.Button btnSubDatumGrid;
        private System.Windows.Forms.Button btnSubEphemeris;
        private System.Windows.Forms.Button btnSubMeasurements;
        public System.Windows.Forms.Button btnCoordinateSystem;
        private System.Windows.Forms.Button btnSubEllipsoid;
        public System.Windows.Forms.Panel pnlGenralSettings;
        private System.Windows.Forms.Button btnGeneralSettings;
        private System.Windows.Forms.Panel panel5;
        public System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnCadastral;
        public System.Windows.Forms.Panel pnlGNSS_Setting;
        private System.Windows.Forms.Button btnGNSS_Settings;
    }
}
