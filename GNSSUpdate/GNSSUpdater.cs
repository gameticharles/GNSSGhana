using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GNSSUpdate
{
    /// <summary>
    /// Provides application update support in C#
    /// </summary>
    public class GNSSUpdater
    {
        /// <summary>
        /// Holds the program-to-update's info
        /// </summary>
        private IGNSSUpdatable applicationInfo;

        /// <summary>
        /// Thread to find update
        /// </summary>
        private BackgroundWorker bgWorker;

        /// <summary>
        /// Creates a new SharpUpdater object
        /// </summary>
        /// <param name="applicationInfo">The name of the application so it can be displayed on dialog boxes to user</param>
        public GNSSUpdater(IGNSSUpdatable applicationInfo)
        {
            this.applicationInfo = applicationInfo;

            this.bgWorker = new BackgroundWorker();
            this.bgWorker.DoWork += new DoWorkEventHandler(BgWorker_DoWork);
            this.bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BgWorker_RunWorkerCompleted);

        }

        /// <summary>
        /// Check for internet conection
        /// </summary>
        /// <returns></returns>
        public bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead("http://clients3.google.com/generate_204"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks for an update for the program passed
        /// If there is an update, a dialog asking to download will appear
        /// </summary>
        public void DoUpdate()
        {
            if (!this.bgWorker.IsBusy)
            {
                this.bgWorker.RunWorkerAsync(this.applicationInfo);
            }
        }

        /// <summary>
        /// Check for/Parse update.xml on server 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            IGNSSUpdatable application = (IGNSSUpdatable)e.Argument;

            if (!GNSSUpdateXml.ExistOnServer(application.UpdateXmlLocation))
            {
                e.Cancel = true;
            }
            else
            {
                e.Result = GNSSUpdateXml.Parse(application.UpdateXmlLocation, application.ApplicationID);
            }

        }

        /// <summary>
        /// Check if there is a newer version
        /// Download it and run it by replacing the older version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                GNSSUpdateXml update = (GNSSUpdateXml)e.Result;

                if (update != null && update.IsNewerThan(this.applicationInfo.ApplicationAssembly.GetName().Version))
                {

                    if (new GNSSUpdateAcceptForm(this.applicationInfo, update).ShowDialog(this.applicationInfo.Context) == DialogResult.Yes)
                    {

                        this.DownloadUpdate(update);
                    }
                }
                else if (!update.IsNewerThan(this.applicationInfo.ApplicationAssembly.GetName().Version))
                {
                    MessageBox.Show("You are already running the new version");

                }
            }

        }

        /// <summary>
        /// Download the update that is available
        /// </summary>
        /// <param name="update">The update to download</param>
        private void DownloadUpdate(GNSSUpdateXml update)
        {
            GNSSUpdateDownloadForm form = new GNSSUpdateDownloadForm(update.Uri, update.MD5, this.applicationInfo.ApplicationIcon);

            DialogResult result = form.ShowDialog(this.applicationInfo.Context);

            if (result == DialogResult.OK)
            {
                string currentPath = this.applicationInfo.ApplicationAssembly.Location;
                string newPath = Path.GetDirectoryName(currentPath) + "\\" + update.FileName;

                UpdateApplication(form.TempFilePath, currentPath, newPath, update.LaunchArgs);
                Application.Exit();
            }
            else if (result == DialogResult.Abort)
            {
                MessageBox.Show("The update download was cancelled.\nThis program has not been modified.", "Update Download Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("There was a problem downloading the update.\nPlease try again later.", "Update Download Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// This download the update delete and replace it with the new one 
        /// then starts the application
        /// </summary>
        /// <param name="tempFilePath"></param>
        /// <param name="currentPath"></param>
        /// <param name="newPath"></param>
        /// <param name="launchArgs"></param>
        private void UpdateApplication(string tempFilePath, string currentPath, string newPath, string launchArgs)
        {
            string argument = "/C Choice /C Y /N /D Y /T 4 & Del /F /Q \"{0}\" & Choice /C Y /N /D Y /T 2 & Move /Y \"{1}\" \"{2}\" & Start \"\" /D \"{3}\" \"{4}\" {5}";

            ProcessStartInfo info = new ProcessStartInfo();
            info.Arguments = string.Format(argument, currentPath, tempFilePath, newPath, Path.GetDirectoryName(newPath), Path.GetFileName(newPath), launchArgs);

            //Hide the command prompt from the user
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.CreateNoWindow = true;

            //Set the name of the app or process to start
            info.FileName = "cmd.exe";

            //Call the program to start the process
            Process.Start(info);
        }
    }
}
