using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ghGPS.Forms;
using ghGPS.Classes;
using System.Reflection;

namespace ghGPS.User_Forms
{
    public partial class RecentStartScreen : UserControl
    {
        int senderIndex = 0;
        public RecentStartScreen()
        {
            InitializeComponent();
        }

        private void lblDocumentaion_MouseEnter(object sender, EventArgs e)
        {
            ((Label)sender).ForeColor = Color.FromArgb(0, 122, 204);
        }

        private void lblDocumentaion_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).ForeColor = Color.DimGray;
        }

        private void btnCreateProject_Click(object sender, EventArgs e)
        {
           MainScreen.createProject.BringToFront();

            //Hide Settings button
            MainScreen.btnSettings.Hide();

            //Show the title 
            MainScreen.lblHeaderTitle.Show();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //Delete the item from the list
            flpRecentProject.Controls.RemoveAt(senderIndex);
        }

        private void lblAbout_Click(object sender, EventArgs e)
        {
            //MainScreen.btnSettings.Visible = false;
            //MainScreen.MainScreenTitle.Text = "";
            //MainScreen.MainScreenTitle.ShowIcon = false;
            //MainScreen.MainScreenTitle.Refresh();

            ////Open the about form
            //MainScreen.AboutPage.BringToFront();

            //Open the about form
            using (var _about = new About())
            {
                _about.ShowDialog();
            }
        }

        private void btnOpenProject_Click(object sender, EventArgs e)
        {

            using (var ofd = new openFileBrowser())
            {
                ofd.fileFilterComboBox1.FilterItems = GNSS_Functions.FilterProject;
                ofd.fileFilterComboBox1.SelectedIndex = 0;
                ofd.shellView.MultiSelect = false;
                GNSS_Functions.OFDheaderText = "Select GGS project file: ";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (GNSS_Functions.SelectedItems.Count() > 0)
                        {

                            string FilePath = GNSS_Functions.SelectedItems[0].FileSystemPath;                                                      
                            
                           
                        }

                    }
                    catch (Exception ex)
                    {
                       
                        Console.WriteLine(ex.Message + "\nCaused by:\n" + ex.Source + ex.TargetSite);

                    }
                }
                else
                {
                   
                }
            };

            GC.Collect();

            //var ters = new Tester();
            //ters.Show();

            //ExeProgram.Start();           

            //using (var loadFile = new WaitingForm(@"C:\Users\Reindroid\Desktop\DATA\Sat Geodesy\Close.18O"))
            //{
            //    loadFile.ShowDialog();

            //    MessageBox.Show(loadFile.result[0].ToString()); 
            //}
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            MainScreen.CoordinateConversion.BringToFront();

            //Hide Settings button
            MainScreen.btnSettings.Hide();            
        }

        /// <summary>
        /// Resize the control to fit into flow panel
        /// </summary>
        public void ResizeItems()
        {
            foreach (var item in flpRecentProject.Controls)
            {
                if (item is projectItem)
                {
                    var theItem = item as projectItem;
                    theItem.Width = (flpRecentProject.Controls.Count > 10) ? 310 : 327;                   
                }               
            }
        }

        private void flpRecentProject_ControlAdded(object sender, ControlEventArgs e)
        {
            // Resize controls
            ResizeItems();
        }

        private void flpRecentProject_ControlRemoved(object sender, ControlEventArgs e)
        {
            // Resize controls
            ResizeItems();
        }

        private void btnRINEX_TEQC_Click(object sender, EventArgs e)
        {
            //Open the RINEX TEQC
            MainScreen.RINEXTEQC.BringToFront();
        }

        private void lblTutorials_Click(object sender, EventArgs e)
        {
            MainScreen.btnSettings.Visible = false;
            MainScreen.MainScreenTitle.Text = "Video Tutorials";
            MainScreen.MainScreenTitle.ShowIcon = false;
            MainScreen.MainScreenTitle.Refresh();

            //Open the about form
            MainScreen.TutorialsPage.BringToFront();
        }
    }
}
