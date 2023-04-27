using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GNSSUpdate
{
    /// <summary>
    /// Form to show details about the update
    /// </summary>
    internal partial class GNSSUpdateInfoForm : MetroSuite.MetroForm
    {
        /// <summary>
        /// Create a new SharpUpdateInfoForm
        /// </summary>
        /// <param name="applicationInfo"></param>
        /// <param name="updateInfo"></param>
        internal GNSSUpdateInfoForm(IGNSSUpdatable applicationInfo, GNSSUpdateXml updateInfo)
        {
            InitializeComponent();

            //Set the icon if it's not null
            if (applicationInfo.ApplicationIcon != null)
            {
                this.Icon = applicationInfo.ApplicationIcon;
            }

            //Fill in the UI
            this.Text = applicationInfo.ApplicationName + "Update Info";
            this.lblVersion.Text = String.Format("Current Version: {0}\nUpdate Version: {1}", applicationInfo.ApplicationAssembly.GetName().Version.ToString(), updateInfo.Version.ToString());
            this.txtDescription.Text = updateInfo.Description;
        }
        

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            //If not crtl+v to copy ignore
            if (!(e.Control && e.KeyCode == Keys.C))
            {
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// Close the info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
