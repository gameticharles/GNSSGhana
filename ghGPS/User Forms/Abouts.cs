using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace ghGPS.User_Forms
{
    public partial class Abouts : UserControl
    {
        public Abouts()
        {
            InitializeComponent();

            lblVersion.Text = this.ProductVersion.ToString();
            lblBuildDate.Text = "June 22, 2019";
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            //Title  
            MainScreen.MainScreenTitle.Text = "GNSS Ghana";
            MainScreen.MainScreenTitle.ShowIcon = true;

            MainScreen.MainScreenTitle.Refresh();

            //Go back to home page
            MainScreen.recentStartScreen.BringToFront();

            //Hide the status
            lblStatus.Text = "";
            lblStatus.Visible = false;

            MainScreen.btnSettings.Visible = true;
        }

        private void btnCheckUpdate_Click(object sender, EventArgs e)
        {
            lblStatus.Visible = true;
            if (MainScreen.updater.CheckForInternetConnection())
            {
                lblStatus.Text = "Internet connection Successful!. Fetching for the update information...";

                //Perform update
                MainScreen.updater.DoUpdate();

            }
            else
            {
                lblStatus.Text = "No connection to the Internet!. Check network connection and try again.";
            }
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
