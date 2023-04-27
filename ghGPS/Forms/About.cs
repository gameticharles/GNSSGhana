using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ghGPS.Forms
{
    public partial class About : MetroSuite.MetroForm
    {
        public About()
        {
            InitializeComponent();
            lblVersion.Text = "Version: " + this.ProductVersion;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCheckUpdate_Click(object sender, EventArgs e)
        {
            //lblStatus.Visible = true;
            //if (MainScreen.updater.CheckForInternetConnection())
            //{
            //    lblStatus.Text = "Internet connection Successful!. Fetching for the update information...";

            //    //Perform update
            //    MainScreen.updater.DoUpdate();

            //}
            //else
            //{
            //    lblStatus.Text = "No connection to the Internet!. Check network connection and try again.";
            //}
        }

        string getAcknowlegment()
        {
            return "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                "";
        }

        string getChange_Log()
        {
            return "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                "";
        }
    }
}
