namespace ghGPS.User_Forms
{
    partial class ProjectionParameterItem
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbxParamValue = new MetroSuite.MetroTextbox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblProjectType = new System.Windows.Forms.Label();
            this.cbxParamName = new MetroSuite.MetroComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tbxParamValue);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblProjectType);
            this.panel1.Controls.Add(this.cbxParamName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(393, 50);
            this.panel1.TabIndex = 0;
            // 
            // tbxParamValue
            // 
            this.tbxParamValue.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.tbxParamValue.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbxParamValue.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.tbxParamValue.DefaultColor = System.Drawing.Color.White;
            this.tbxParamValue.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.tbxParamValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbxParamValue.ForeColor = System.Drawing.Color.Black;
            this.tbxParamValue.HideSelection = false;
            this.tbxParamValue.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tbxParamValue.Location = new System.Drawing.Point(224, 22);
            this.tbxParamValue.Name = "tbxParamValue";
            this.tbxParamValue.PasswordChar = '\0';
            this.tbxParamValue.ReadOnly = true;
            this.tbxParamValue.Size = new System.Drawing.Size(156, 23);
            this.tbxParamValue.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Location = new System.Drawing.Point(221, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 15);
            this.label1.TabIndex = 10;
            this.label1.Text = "Value:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblProjectType
            // 
            this.lblProjectType.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblProjectType.AutoSize = true;
            this.lblProjectType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblProjectType.ForeColor = System.Drawing.Color.Gray;
            this.lblProjectType.Location = new System.Drawing.Point(12, 4);
            this.lblProjectType.Name = "lblProjectType";
            this.lblProjectType.Size = new System.Drawing.Size(64, 15);
            this.lblProjectType.TabIndex = 10;
            this.lblProjectType.Text = "Parameter:";
            this.lblProjectType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbxParamName
            // 
            this.cbxParamName.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxParamName.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cbxParamName.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxParamName.AutoStyle = false;
            this.cbxParamName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.cbxParamName.DefaultColor = System.Drawing.Color.White;
            this.cbxParamName.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.cbxParamName.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxParamName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxParamName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxParamName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbxParamName.ForeColor = System.Drawing.Color.Black;
            this.cbxParamName.FormattingEnabled = true;
            this.cbxParamName.ItemHeight = 17;
            this.cbxParamName.Items.AddRange(new object[] {
            "Central Meridian",
            "False Easting",
            "False Northing",
            "Latitude of Origin",
            "Latitude of Center",
            "Longitude of Center",
            "Scale Factor",
            "Standard Parallel 1",
            "Standard Parallel 2",
            "Rectified Grid Angle",
            "Azimuth",
            "Pseudo Standard Parallel 1"});
            this.cbxParamName.Location = new System.Drawing.Point(15, 22);
            this.cbxParamName.Name = "cbxParamName";
            this.cbxParamName.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cbxParamName.Size = new System.Drawing.Size(206, 23);
            this.cbxParamName.Style = MetroSuite.Design.Style.Custom;
            this.cbxParamName.TabIndex = 9;
            this.cbxParamName.SelectedIndexChanged += new System.EventHandler(this.CbxParamName_SelectedIndexChanged);
            // 
            // ProjectionParameterItem
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "ProjectionParameterItem";
            this.Size = new System.Drawing.Size(393, 50);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        public MetroSuite.MetroComboBox cbxParamName;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label lblProjectType;
        public MetroSuite.MetroTextbox tbxParamValue;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
