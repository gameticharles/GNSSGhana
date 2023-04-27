using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ghGPS.Properties;

namespace ghGPS.User_Forms
{
    public partial class projectItem : UserControl
    {
        public projectItem()
        {
            InitializeComponent();                        
        }
              
        private void lblProjectDate_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.Gainsboro;

            pnlDelete.Visible = true;            
        }

        private void lblProjectDate_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(243, 243, 243);

            checkIn(this);
        }

        private void lblProjectDate_MouseDown(object sender, MouseEventArgs e)
        {
            this.BackColor = Color.WhiteSmoke;
        }

        private void lblProjectDate_MouseUp(object sender, MouseEventArgs e)
        {
            this.BackColor = Color.Gainsboro;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            MainScreen.btnDeleteHandler(this);
        }

        private void projectItem_Load(object sender, EventArgs e)
        {
            //Set tooltip to show fullpath
            toolTip1.SetToolTip(lblProjectName, this.Tag.ToString());
            toolTip1.SetToolTip(lblProjectType, this.Tag.ToString());
            toolTip1.SetToolTip(lblProjectDate, this.Tag.ToString());
            toolTip1.SetToolTip(picBxIcon, this.Tag.ToString());
            //toolTip1.SetToolTip(lblDelete, this.Tag.ToString());
            toolTip1.SetToolTip(btnDelete, "Delete Project: " + lblProjectName.Text + "\n" + this.Tag.ToString());
        }
              

        private void btnExpandCollapse_Click(object sender, EventArgs e)
        {
            // Exapand the size to fit the control
            ExpandCollapse();

        }

        /// <summary>
        /// Exapand the size to fit the control
        /// </summary>
        void ExpandCollapse()
        {
            if (pnlDelete.Width == this.Width)
            {
                pnlDelete.Width = 19;
                btnExpandCollapse.Image = Resources.Left_25px;
            }
            else
            {
                pnlDelete.Width = this.Width;
                btnExpandCollapse.Image = Resources.Right_25px;
            }
        }
        
        /// <summary>
        /// Check if the Mouse Position is within the bound of the control
        /// </summary>
        /// <param name="ctrl">The Control to check the bound</param>
        void checkIn(Control ctrl)
        {
            if (!ctrl.ClientRectangle.Contains(PointToClient(Control.MousePosition)))
            {
                pnlDelete.Width = 45;
                btnExpandCollapse.Image = Resources.Left_25px;
                pnlDelete.Visible = false;
            }
        }
    }
}
