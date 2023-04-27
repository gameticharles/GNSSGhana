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
    /// Form to prompt the user to accept the update
    /// </summary>
    internal partial class GNSSUpdateAcceptForm : MetroSuite.MetroForm
    {
        /// <summary>
        /// The program to update's info
        /// </summary>
        private IGNSSUpdatable applicationInfo;

        /// <summary>
        /// The update info from the update.xml
        /// </summary>
        private GNSSUpdateXml updateInfo;

        /// <summary>
        /// The update info display form
        /// </summary>
        private GNSSUpdateInfoForm updateInfoForm;

        /// <summary>
        /// Create a new SharpUpdateAcceptForm
        /// </summary>
        /// <param name="applicationInfo"></param>
        /// <param name="updateInfo"></param>
        internal GNSSUpdateAcceptForm(IGNSSUpdatable applicationInfo, GNSSUpdateXml updateInfo)
        {
            InitializeComponent();

            this.applicationInfo = applicationInfo;
            this.updateInfo = updateInfo;

            this.Text = this.applicationInfo.ApplicationName + " - Update Available";

            //Set the icon if it's not null
            if (applicationInfo.ApplicationIcon != null)
            {
                this.Icon = applicationInfo.ApplicationIcon;
            }

            //Add the update version to the form
            this.lblNewVersion.Text = string.Format("New Version: {0}", this.updateInfo.Version.ToString());

        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            if (this.updateInfoForm == null)
            {
                this.updateInfoForm = new GNSSUpdateInfoForm(this.applicationInfo, this.updateInfo);
            }

            this.updateInfoForm.ShowDialog(this);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }
    }
}
