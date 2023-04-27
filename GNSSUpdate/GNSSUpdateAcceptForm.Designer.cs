namespace GNSSUpdate
{
    partial class GNSSUpdateAcceptForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GNSSUpdateAcceptForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlUpdateAvailable = new System.Windows.Forms.Panel();
            this.btnDetails = new MetroSuite.MetroButton();
            this.btnYes = new MetroSuite.MetroButton();
            this.lblNewVersion = new System.Windows.Forms.Label();
            this.lblUpdateAvail = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.pnlUpdateAvailable.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.pnlUpdateAvailable);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(329, 162);
            this.panel1.TabIndex = 2;
            // 
            // pnlUpdateAvailable
            // 
            this.pnlUpdateAvailable.Controls.Add(this.btnDetails);
            this.pnlUpdateAvailable.Controls.Add(this.btnYes);
            this.pnlUpdateAvailable.Controls.Add(this.lblNewVersion);
            this.pnlUpdateAvailable.Controls.Add(this.lblUpdateAvail);
            this.pnlUpdateAvailable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlUpdateAvailable.Location = new System.Drawing.Point(0, 24);
            this.pnlUpdateAvailable.Name = "pnlUpdateAvailable";
            this.pnlUpdateAvailable.Size = new System.Drawing.Size(329, 138);
            this.pnlUpdateAvailable.TabIndex = 3;
            // 
            // btnDetails
            // 
            this.btnDetails.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDetails.AutoStyle = false;
            this.btnDetails.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnDetails.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnDetails.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDetails.DefaultColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.btnDetails.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnDetails.DisabledColor = System.Drawing.Color.LightGray;
            this.btnDetails.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnDetails.ForeColor = System.Drawing.Color.Black;
            this.btnDetails.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnDetails.Location = new System.Drawing.Point(168, 99);
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.PressedColor = System.Drawing.Color.Silver;
            this.btnDetails.Size = new System.Drawing.Size(83, 26);
            this.btnDetails.Style = MetroSuite.Design.Style.Custom;
            this.btnDetails.TabIndex = 19;
            this.btnDetails.Text = "Details";
            this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
            // 
            // btnYes
            // 
            this.btnYes.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnYes.AutoStyle = false;
            this.btnYes.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnYes.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.btnYes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnYes.DefaultColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.btnYes.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnYes.DisabledColor = System.Drawing.Color.LightGray;
            this.btnYes.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnYes.ForeColor = System.Drawing.Color.Black;
            this.btnYes.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnYes.Location = new System.Drawing.Point(78, 99);
            this.btnYes.Name = "btnYes";
            this.btnYes.PressedColor = System.Drawing.Color.Silver;
            this.btnYes.Size = new System.Drawing.Size(83, 26);
            this.btnYes.Style = MetroSuite.Design.Style.Custom;
            this.btnYes.TabIndex = 19;
            this.btnYes.Text = "Download";
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // lblNewVersion
            // 
            this.lblNewVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewVersion.Location = new System.Drawing.Point(86, 62);
            this.lblNewVersion.Name = "lblNewVersion";
            this.lblNewVersion.Size = new System.Drawing.Size(154, 19);
            this.lblNewVersion.TabIndex = 3;
            this.lblNewVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblUpdateAvail
            // 
            this.lblUpdateAvail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpdateAvail.Location = new System.Drawing.Point(50, 19);
            this.lblUpdateAvail.Name = "lblUpdateAvail";
            this.lblUpdateAvail.Size = new System.Drawing.Size(228, 40);
            this.lblUpdateAvail.TabIndex = 2;
            this.lblUpdateAvail.Text = "An update is available!\r\nClick download to update.";
            this.lblUpdateAvail.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(329, 24);
            this.panel2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "GNSS GH - Update Available";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.LightGray;
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 23);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(329, 1);
            this.panel4.TabIndex = 4;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.Control;
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Location = new System.Drawing.Point(304, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(25, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // GNSSUpdateAcceptForm
            // 
            this.AllowResize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 164);
            this.Controls.Add(this.panel1);
            this.Name = "GNSSUpdateAcceptForm";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.State = MetroSuite.MetroForm.FormState.Normal;
            this.Style = MetroSuite.Design.Style.Light;
            this.Text = "GNSSUpdateAcceptForm";
            this.panel1.ResumeLayout(false);
            this.pnlUpdateAvailable.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlUpdateAvailable;
        private System.Windows.Forms.Label lblNewVersion;
        private System.Windows.Forms.Label lblUpdateAvail;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel4;
        public MetroSuite.MetroButton btnDetails;
        public MetroSuite.MetroButton btnYes;
        private System.Windows.Forms.Button btnClose;
    }
}