namespace ghGPS.User_Forms
{
    partial class projectItem
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(projectItem));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lblProjectName = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblProjectDate = new System.Windows.Forms.Label();
            this.lblProjectType = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.picBxIcon = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pnlDelete = new System.Windows.Forms.Panel();
            this.btnDelete = new System.Windows.Forms.Button();
            this.pnlSeparator = new System.Windows.Forms.Panel();
            this.btnExpandCollapse = new System.Windows.Forms.Button();
            this.pnlMain.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBxIcon)).BeginInit();
            this.pnlDelete.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.panel5);
            this.pnlMain.Controls.Add(this.panel3);
            this.pnlMain.Controls.Add(this.panel2);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(309, 43);
            this.pnlMain.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.lblProjectName);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(41, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(268, 20);
            this.panel5.TabIndex = 3;
            // 
            // lblProjectName
            // 
            this.lblProjectName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProjectName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblProjectName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblProjectName.Location = new System.Drawing.Point(0, 0);
            this.lblProjectName.Name = "lblProjectName";
            this.lblProjectName.Size = new System.Drawing.Size(268, 20);
            this.lblProjectName.TabIndex = 1;
            this.lblProjectName.Text = "Project 1";
            this.lblProjectName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblProjectName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblProjectDate_MouseDown);
            this.lblProjectName.MouseEnter += new System.EventHandler(this.lblProjectDate_MouseEnter);
            this.lblProjectName.MouseLeave += new System.EventHandler(this.lblProjectDate_MouseLeave);
            this.lblProjectName.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblProjectDate_MouseUp);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblProjectDate);
            this.panel3.Controls.Add(this.lblProjectType);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(41, 20);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(268, 23);
            this.panel3.TabIndex = 1;
            // 
            // lblProjectDate
            // 
            this.lblProjectDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProjectDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblProjectDate.ForeColor = System.Drawing.Color.Gray;
            this.lblProjectDate.Location = new System.Drawing.Point(130, 0);
            this.lblProjectDate.Name = "lblProjectDate";
            this.lblProjectDate.Size = new System.Drawing.Size(138, 23);
            this.lblProjectDate.TabIndex = 0;
            this.lblProjectDate.Text = "2018/08/22 10:55:48";
            this.lblProjectDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblProjectDate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblProjectDate_MouseDown);
            this.lblProjectDate.MouseEnter += new System.EventHandler(this.lblProjectDate_MouseEnter);
            this.lblProjectDate.MouseLeave += new System.EventHandler(this.lblProjectDate_MouseLeave);
            this.lblProjectDate.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblProjectDate_MouseUp);
            // 
            // lblProjectType
            // 
            this.lblProjectType.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblProjectType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblProjectType.ForeColor = System.Drawing.Color.Gray;
            this.lblProjectType.Location = new System.Drawing.Point(0, 0);
            this.lblProjectType.Name = "lblProjectType";
            this.lblProjectType.Size = new System.Drawing.Size(130, 23);
            this.lblProjectType.TabIndex = 1;
            this.lblProjectType.Text = "Type: Processed GNSS";
            this.lblProjectType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblProjectType.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblProjectDate_MouseDown);
            this.lblProjectType.MouseEnter += new System.EventHandler(this.lblProjectDate_MouseEnter);
            this.lblProjectType.MouseLeave += new System.EventHandler(this.lblProjectDate_MouseLeave);
            this.lblProjectType.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblProjectDate_MouseUp);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.picBxIcon);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(41, 43);
            this.panel2.TabIndex = 0;
            this.panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblProjectDate_MouseDown);
            this.panel2.MouseEnter += new System.EventHandler(this.lblProjectDate_MouseEnter);
            this.panel2.MouseLeave += new System.EventHandler(this.lblProjectDate_MouseLeave);
            this.panel2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblProjectDate_MouseUp);
            // 
            // picBxIcon
            // 
            this.picBxIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picBxIcon.Location = new System.Drawing.Point(0, 0);
            this.picBxIcon.Name = "picBxIcon";
            this.picBxIcon.Size = new System.Drawing.Size(41, 43);
            this.picBxIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBxIcon.TabIndex = 0;
            this.picBxIcon.TabStop = false;
            this.picBxIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblProjectDate_MouseDown);
            this.picBxIcon.MouseEnter += new System.EventHandler(this.lblProjectDate_MouseEnter);
            this.picBxIcon.MouseLeave += new System.EventHandler(this.lblProjectDate_MouseLeave);
            this.picBxIcon.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblProjectDate_MouseUp);
            // 
            // pnlDelete
            // 
            this.pnlDelete.Controls.Add(this.btnDelete);
            this.pnlDelete.Controls.Add(this.pnlSeparator);
            this.pnlDelete.Controls.Add(this.btnExpandCollapse);
            this.pnlDelete.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlDelete.Location = new System.Drawing.Point(264, 0);
            this.pnlDelete.Name = "pnlDelete";
            this.pnlDelete.Size = new System.Drawing.Size(45, 43);
            this.pnlDelete.TabIndex = 2;
            this.pnlDelete.Visible = false;
            this.pnlDelete.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblProjectDate_MouseDown);
            this.pnlDelete.MouseEnter += new System.EventHandler(this.lblProjectDate_MouseEnter);
            this.pnlDelete.MouseLeave += new System.EventHandler(this.lblProjectDate_MouseLeave);
            this.pnlDelete.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblProjectDate_MouseUp);
            // 
            // btnDelete
            // 
            this.btnDelete.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnDelete.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btnDelete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnDelete.ForeColor = System.Drawing.Color.Gray;
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDelete.Location = new System.Drawing.Point(-111, 0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(93, 43);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "Delete";
            this.btnDelete.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.btnDelete.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblProjectDate_MouseDown);
            this.btnDelete.MouseEnter += new System.EventHandler(this.lblProjectDate_MouseEnter);
            this.btnDelete.MouseLeave += new System.EventHandler(this.lblProjectDate_MouseLeave);
            this.btnDelete.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblProjectDate_MouseUp);
            // 
            // pnlSeparator
            // 
            this.pnlSeparator.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlSeparator.Location = new System.Drawing.Point(-18, 0);
            this.pnlSeparator.Name = "pnlSeparator";
            this.pnlSeparator.Size = new System.Drawing.Size(44, 43);
            this.pnlSeparator.TabIndex = 3;
            this.pnlSeparator.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblProjectDate_MouseDown);
            this.pnlSeparator.MouseEnter += new System.EventHandler(this.lblProjectDate_MouseEnter);
            this.pnlSeparator.MouseLeave += new System.EventHandler(this.lblProjectDate_MouseLeave);
            this.pnlSeparator.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblProjectDate_MouseUp);
            // 
            // btnExpandCollapse
            // 
            this.btnExpandCollapse.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnExpandCollapse.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.btnExpandCollapse.FlatAppearance.BorderSize = 0;
            this.btnExpandCollapse.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btnExpandCollapse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnExpandCollapse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExpandCollapse.Image = global::ghGPS.Properties.Resources.Left_25px;
            this.btnExpandCollapse.Location = new System.Drawing.Point(26, 0);
            this.btnExpandCollapse.Name = "btnExpandCollapse";
            this.btnExpandCollapse.Size = new System.Drawing.Size(19, 43);
            this.btnExpandCollapse.TabIndex = 2;
            this.btnExpandCollapse.UseVisualStyleBackColor = true;
            this.btnExpandCollapse.Click += new System.EventHandler(this.btnExpandCollapse_Click);
            this.btnExpandCollapse.MouseEnter += new System.EventHandler(this.lblProjectDate_MouseEnter);
            this.btnExpandCollapse.MouseLeave += new System.EventHandler(this.lblProjectDate_MouseLeave);
            // 
            // projectItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.Controls.Add(this.pnlDelete);
            this.Controls.Add(this.pnlMain);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "projectItem";
            this.Size = new System.Drawing.Size(309, 43);
            this.Load += new System.EventHandler(this.projectItem_Load);
            this.pnlMain.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBxIcon)).EndInit();
            this.pnlDelete.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel5;
        public System.Windows.Forms.Label lblProjectDate;
        public System.Windows.Forms.Label lblProjectType;
        public System.Windows.Forms.Label lblProjectName;
        public System.Windows.Forms.PictureBox picBxIcon;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel pnlDelete;
        private System.Windows.Forms.Button btnExpandCollapse;
        private System.Windows.Forms.Panel pnlSeparator;
        public System.Windows.Forms.Button btnDelete;
    }
}
