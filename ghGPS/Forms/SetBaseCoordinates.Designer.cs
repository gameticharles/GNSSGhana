namespace ghGPS.Forms
{
    partial class SetBaseCoordinates
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetBaseCoordinates));
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbxAntennaType = new MetroSuite.MetroComboBox();
            this.btnOK = new MetroSuite.MetroButton();
            this.btnCancel = new MetroSuite.MetroButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblAproxHeight = new System.Windows.Forms.Label();
            this.lblAproxNorthings = new System.Windows.Forms.Label();
            this.lblAproxEastings = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.metroTextbox1 = new MetroSuite.MetroTextbox();
            this.tbxHeight = new MetroSuite.MetroTextbox();
            this.tbxNorthings = new MetroSuite.MetroTextbox();
            this.tbxStationEditedID = new MetroSuite.MetroTextbox();
            this.tbxEastings = new MetroSuite.MetroTextbox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbxAntennaType);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.lblAproxHeight);
            this.panel1.Controls.Add(this.lblAproxNorthings);
            this.panel1.Controls.Add(this.lblAproxEastings);
            this.panel1.Controls.Add(this.label19);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.label15);
            this.panel1.Controls.Add(this.metroTextbox1);
            this.panel1.Controls.Add(this.tbxHeight);
            this.panel1.Controls.Add(this.tbxNorthings);
            this.panel1.Controls.Add(this.tbxStationEditedID);
            this.panel1.Controls.Add(this.tbxEastings);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(1, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(322, 315);
            this.panel1.TabIndex = 0;
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
            this.cbxAntennaType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxAntennaType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbxAntennaType.ForeColor = System.Drawing.Color.Black;
            this.cbxAntennaType.FormattingEnabled = true;
            this.cbxAntennaType.ItemHeight = 22;
            this.cbxAntennaType.Items.AddRange(new object[] {
            "Pole",
            "Slant",
            "Vertical"});
            this.cbxAntennaType.Location = new System.Drawing.Point(189, 238);
            this.cbxAntennaType.Name = "cbxAntennaType";
            this.cbxAntennaType.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxAntennaType.Size = new System.Drawing.Size(117, 28);
            this.cbxAntennaType.Style = MetroSuite.Design.Style.Custom;
            this.cbxAntennaType.TabIndex = 16;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOK.AutoStyle = false;
            this.btnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnOK.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOK.DefaultColor = System.Drawing.Color.White;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnOK.ForeColor = System.Drawing.Color.Black;
            this.btnOK.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnOK.Location = new System.Drawing.Point(164, 277);
            this.btnOK.Name = "btnOK";
            this.btnOK.PressedColor = System.Drawing.Color.Silver;
            this.btnOK.Size = new System.Drawing.Size(67, 27);
            this.btnOK.Style = MetroSuite.Design.Style.Custom;
            this.btnOK.TabIndex = 15;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.AutoStyle = false;
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnCancel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.DefaultColor = System.Drawing.Color.White;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnCancel.Location = new System.Drawing.Point(238, 277);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.PressedColor = System.Drawing.Color.Silver;
            this.btnCancel.Size = new System.Drawing.Size(67, 27);
            this.btnCancel.Style = MetroSuite.Design.Style.Custom;
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            // 
            // label4
            // 
            this.label4.AutoEllipsis = true;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.DimGray;
            this.label4.Location = new System.Drawing.Point(7, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "Input Location:";
            // 
            // label6
            // 
            this.label6.AutoEllipsis = true;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.DimGray;
            this.label6.Location = new System.Drawing.Point(8, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(133, 15);
            this.label6.TabIndex = 8;
            this.label6.Text = "Approximate Location:";
            // 
            // lblAproxHeight
            // 
            this.lblAproxHeight.AutoEllipsis = true;
            this.lblAproxHeight.AutoSize = true;
            this.lblAproxHeight.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAproxHeight.ForeColor = System.Drawing.Color.DimGray;
            this.lblAproxHeight.Location = new System.Drawing.Point(87, 66);
            this.lblAproxHeight.Name = "lblAproxHeight";
            this.lblAproxHeight.Size = new System.Drawing.Size(0, 15);
            this.lblAproxHeight.TabIndex = 9;
            // 
            // lblAproxNorthings
            // 
            this.lblAproxNorthings.AutoEllipsis = true;
            this.lblAproxNorthings.AutoSize = true;
            this.lblAproxNorthings.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAproxNorthings.ForeColor = System.Drawing.Color.DimGray;
            this.lblAproxNorthings.Location = new System.Drawing.Point(87, 49);
            this.lblAproxNorthings.Name = "lblAproxNorthings";
            this.lblAproxNorthings.Size = new System.Drawing.Size(0, 15);
            this.lblAproxNorthings.TabIndex = 10;
            // 
            // lblAproxEastings
            // 
            this.lblAproxEastings.AutoEllipsis = true;
            this.lblAproxEastings.AutoSize = true;
            this.lblAproxEastings.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAproxEastings.ForeColor = System.Drawing.Color.DimGray;
            this.lblAproxEastings.Location = new System.Drawing.Point(87, 32);
            this.lblAproxEastings.Name = "lblAproxEastings";
            this.lblAproxEastings.Size = new System.Drawing.Size(0, 15);
            this.lblAproxEastings.TabIndex = 11;
            // 
            // label19
            // 
            this.label19.AutoEllipsis = true;
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ForeColor = System.Drawing.Color.DimGray;
            this.label19.Location = new System.Drawing.Point(72, 66);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(17, 15);
            this.label19.TabIndex = 12;
            this.label19.Text = "Z:";
            // 
            // label17
            // 
            this.label17.AutoEllipsis = true;
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.Color.DimGray;
            this.label17.Location = new System.Drawing.Point(72, 49);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(19, 15);
            this.label17.TabIndex = 13;
            this.label17.Text = "N:";
            // 
            // label15
            // 
            this.label15.AutoEllipsis = true;
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.Color.DimGray;
            this.label15.Location = new System.Drawing.Point(72, 32);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(16, 15);
            this.label15.TabIndex = 14;
            this.label15.Text = "E:";
            // 
            // metroTextbox1
            // 
            this.metroTextbox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.metroTextbox1.AutoStyle = false;
            this.metroTextbox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.metroTextbox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.metroTextbox1.DefaultColor = System.Drawing.Color.White;
            this.metroTextbox1.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.metroTextbox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metroTextbox1.ForeColor = System.Drawing.Color.Black;
            this.metroTextbox1.HideSelection = false;
            this.metroTextbox1.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.metroTextbox1.Location = new System.Drawing.Point(103, 239);
            this.metroTextbox1.Name = "metroTextbox1";
            this.metroTextbox1.PasswordChar = '\0';
            this.metroTextbox1.Size = new System.Drawing.Size(80, 28);
            this.metroTextbox1.Style = MetroSuite.Design.Style.Custom;
            this.metroTextbox1.TabIndex = 7;
            // 
            // tbxHeight
            // 
            this.tbxHeight.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tbxHeight.AutoStyle = false;
            this.tbxHeight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxHeight.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxHeight.DefaultColor = System.Drawing.Color.White;
            this.tbxHeight.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxHeight.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxHeight.ForeColor = System.Drawing.Color.Black;
            this.tbxHeight.HideSelection = false;
            this.tbxHeight.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxHeight.Location = new System.Drawing.Point(103, 206);
            this.tbxHeight.Name = "tbxHeight";
            this.tbxHeight.PasswordChar = '\0';
            this.tbxHeight.Size = new System.Drawing.Size(203, 28);
            this.tbxHeight.Style = MetroSuite.Design.Style.Custom;
            this.tbxHeight.TabIndex = 7;
            // 
            // tbxNorthings
            // 
            this.tbxNorthings.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tbxNorthings.AutoStyle = false;
            this.tbxNorthings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxNorthings.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxNorthings.DefaultColor = System.Drawing.Color.White;
            this.tbxNorthings.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxNorthings.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxNorthings.ForeColor = System.Drawing.Color.Black;
            this.tbxNorthings.HideSelection = false;
            this.tbxNorthings.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxNorthings.Location = new System.Drawing.Point(103, 174);
            this.tbxNorthings.Name = "tbxNorthings";
            this.tbxNorthings.PasswordChar = '\0';
            this.tbxNorthings.Size = new System.Drawing.Size(203, 28);
            this.tbxNorthings.Style = MetroSuite.Design.Style.Custom;
            this.tbxNorthings.TabIndex = 7;
            // 
            // tbxStationEditedID
            // 
            this.tbxStationEditedID.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tbxStationEditedID.AutoStyle = false;
            this.tbxStationEditedID.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxStationEditedID.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxStationEditedID.DefaultColor = System.Drawing.Color.White;
            this.tbxStationEditedID.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxStationEditedID.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxStationEditedID.ForeColor = System.Drawing.Color.Black;
            this.tbxStationEditedID.HideSelection = false;
            this.tbxStationEditedID.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxStationEditedID.Location = new System.Drawing.Point(103, 109);
            this.tbxStationEditedID.Name = "tbxStationEditedID";
            this.tbxStationEditedID.PasswordChar = '\0';
            this.tbxStationEditedID.Size = new System.Drawing.Size(203, 28);
            this.tbxStationEditedID.Style = MetroSuite.Design.Style.Custom;
            this.tbxStationEditedID.TabIndex = 7;
            // 
            // tbxEastings
            // 
            this.tbxEastings.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tbxEastings.AutoStyle = false;
            this.tbxEastings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxEastings.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxEastings.DefaultColor = System.Drawing.Color.White;
            this.tbxEastings.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxEastings.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxEastings.ForeColor = System.Drawing.Color.Black;
            this.tbxEastings.HideSelection = false;
            this.tbxEastings.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxEastings.Location = new System.Drawing.Point(103, 142);
            this.tbxEastings.Name = "tbxEastings";
            this.tbxEastings.PasswordChar = '\0';
            this.tbxEastings.Size = new System.Drawing.Size(203, 28);
            this.tbxEastings.Style = MetroSuite.Design.Style.Custom;
            this.tbxEastings.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoEllipsis = true;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label5.ForeColor = System.Drawing.Color.DimGray;
            this.label5.Location = new System.Drawing.Point(9, 246);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 15);
            this.label5.TabIndex = 6;
            this.label5.Text = "Antenna Height:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoEllipsis = true;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label3.ForeColor = System.Drawing.Color.DimGray;
            this.label3.Location = new System.Drawing.Point(55, 212);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Height:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoEllipsis = true;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(38, 180);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Northings:";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoEllipsis = true;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label7.ForeColor = System.Drawing.Color.DimGray;
            this.label7.Location = new System.Drawing.Point(40, 115);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 15);
            this.label7.TabIndex = 6;
            this.label7.Text = "Station ID:";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoEllipsis = true;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(48, 148);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Eastings:";
            // 
            // SetBaseCoordinates
            // 
            this.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(139)))), ((int)(((byte)(196)))));
            this.AllowResize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 349);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SetBaseCoordinates";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.State = MetroSuite.MetroForm.FormState.Custom;
            this.Style = MetroSuite.Design.Style.Light;
            this.Text = "Set Base Coordinates";
            this.Load += new System.EventHandler(this.SetBaseCoordinates_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        public MetroSuite.MetroTextbox tbxEastings;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label15;
        public MetroSuite.MetroTextbox tbxHeight;
        public MetroSuite.MetroTextbox tbxNorthings;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        public MetroSuite.MetroButton btnCancel;
        public MetroSuite.MetroButton btnOK;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Label lblAproxHeight;
        public System.Windows.Forms.Label lblAproxNorthings;
        public System.Windows.Forms.Label lblAproxEastings;
        public MetroSuite.MetroTextbox metroTextbox1;
        private System.Windows.Forms.Label label5;
        public MetroSuite.MetroTextbox tbxStationEditedID;
        private System.Windows.Forms.Label label7;
        public MetroSuite.MetroComboBox cbxAntennaType;
    }
}