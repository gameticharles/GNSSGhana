using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ghGPS.Classes;
using System.IO;

namespace ghGPS.User_Forms
{
    public partial class RINEXTEQC : UserControl
    {
        public static rnxDataImport DataImport = new rnxDataImport();
        public static rnxEditHeader EditHeader = new rnxEditHeader();
        public static rnxMerge_Split Merge_Split = new rnxMerge_Split();
        public static rnxRINEX_Viewer RINEX_Viewer = new rnxRINEX_Viewer();
        public static rnxSummary Summary = new rnxSummary();
        public static rnxTranslation_Convert Translation_Convert = new rnxTranslation_Convert();
        public static rnxQualityCheck QualityCheck = new rnxQualityCheck();

        public static rnxViewerEphemeris ViewerEphemeris = new rnxViewerEphemeris();
        public static rnxViewerIonoParameter ViewerIonoParameter = new rnxViewerIonoParameter();
        public static rnxViewerMeasurement ViewerMeasurement = new rnxViewerMeasurement();
        public static rnxViewerSatellite ViewerSatellite = new rnxViewerSatellite();
        public static rnxViewerTime_Epoch ViewerTime_Epoch = new rnxViewerTime_Epoch();

        public static string Filename = "";
        public static string ext = "";
        public static string FileDirectory = "";

        public RINEXTEQC()
        {
            InitializeComponent();

            DataImport.Dock = DockStyle.Fill;
            pnlContainer.Controls.Add(DataImport);
                        
            EditHeader.Dock = DockStyle.Fill;
            pnlContainer.Controls.Add(EditHeader);

            Merge_Split.Dock = DockStyle.Fill;
            pnlContainer.Controls.Add(Merge_Split);

            RINEX_Viewer.Dock = DockStyle.Fill;
            pnlContainer.Controls.Add(RINEX_Viewer);

            Translation_Convert.Dock = DockStyle.Fill;
            pnlContainer.Controls.Add(Translation_Convert);

            QualityCheck.Dock = DockStyle.Fill;
            pnlContainer.Controls.Add(QualityCheck);

            Summary.Dock = DockStyle.Fill;
            pnlContainer.Controls.Add(Summary);

            ViewerEphemeris.Dock = DockStyle.Fill;
            RINEX_Viewer.Controls.Add(ViewerEphemeris);

            ViewerIonoParameter.Dock = DockStyle.Fill;
            RINEX_Viewer.Controls.Add(ViewerIonoParameter);

            ViewerMeasurement.Dock = DockStyle.Fill;
            RINEX_Viewer.Controls.Add(ViewerMeasurement);

            ViewerSatellite.Dock = DockStyle.Fill;
            RINEX_Viewer.Controls.Add(ViewerSatellite);

            ViewerTime_Epoch.Dock = DockStyle.Fill;
            RINEX_Viewer.Controls.Add(ViewerTime_Epoch);

            this.Refresh();
        }
        

        private void btnBack_Click(object sender, EventArgs e)
        {
            MainScreen.recentStartScreen.BringToFront();

            //Show Settings button
            MainScreen.btnSettings.Show();

            //Refresh Recent list
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

            //Clear, reset fields and go to home page
            GNSS_Functions.ClearCreateNewProject();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            buttonEffect(sender, e);
            GNSS_Functions.expandEffect(pnlReport, 250);
            ViewerTime_Epoch.BringToFront();
            RINEX_Viewer.BringToFront();            
            //pnlCadastralReport.Height = 46;
        }

        private void btnRINEXData_Click(object sender, EventArgs e)
        {
            DataImport.BringToFront();

            buttonEffect(sender, e);
        }

        private void btnSummary_Click(object sender, EventArgs e)
        {
            Summary.BringToFront();
            buttonEffect(sender, e);
        }

        private void btnEditHeader_Click(object sender, EventArgs e)
        {
            EditHeader.BringToFront();

            buttonEffect(sender, e);
        }

        private void btnMerge_SplitData_Click(object sender, EventArgs e)
        {
            Merge_Split.BringToFront();

            buttonEffect(sender, e);
        }              

        private void btnTranslation_Click(object sender, EventArgs e)
        {
            Translation_Convert.BringToFront();
            buttonEffect(sender, e);
        }

        private void btnIonoParameters_Click(object sender, EventArgs e)
        {
            ViewerIonoParameter.BringToFront();
            buttonEffect(sender, e);
        }

        private void btnSatellites_Click(object sender, EventArgs e)
        {
            ViewerSatellite.BringToFront();
            buttonEffect(sender, e);
        }

        private void btnEphemeris_Click(object sender, EventArgs e)
        {
            ViewerEphemeris.BringToFront();
            buttonEffect(sender, e);
        }

        private void btnMeasurements_Click(object sender, EventArgs e)
        {
            ViewerMeasurement.BringToFront();
            buttonEffect(sender, e);
        }

        private void btnTime_Epoch_Click(object sender, EventArgs e)
        {
            ViewerTime_Epoch.BringToFront();
            buttonEffect(sender, e);
        }

        private async void cbxSelectedRINEX_File_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Set the current fill and run process again
            
            FileDirectory = cbxSelectedRINEX_File.SelectedItem.ToString().Substring(0, cbxSelectedRINEX_File.SelectedItem.ToString().LastIndexOf('\\'));
            Filename = Path.GetFileNameWithoutExtension(cbxSelectedRINEX_File.SelectedItem.ToString());
            ext = Path.GetExtension(cbxSelectedRINEX_File.SelectedItem.ToString());
                        
        }

        private void btnQualityCheck_Click(object sender, EventArgs e)
        {
            QualityCheck.BringToFront();
            buttonEffect(sender, e);
        }

        /// <summary>
        /// Add check effect to the button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void buttonEffect(object sender, EventArgs e)
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                {
                    if (control is Button)
                    {
                        if ((control as Button) == (Button)sender)
                        {
                            if (!(control as Button).Name.StartsWith("btnSub") && (control as Button).Parent.Parent == pnlButtons)
                            {
                                (control as Button).BackColor = Color.FromArgb(100, 160, 199);
                            }
                            else if ((control as Button).Name.StartsWith("btnSub") && (control as Button).Parent == pnlReport)
                            {
                                btnReport.BackColor = Color.FromArgb(100, 160, 199);
                                (control as Button).BackColor = Color.FromArgb(83, 127, 183); 
                            }
                            
                        }
                        else
                        {                            
                            if (!(control as Button).Name.StartsWith("btnSub") && (control as Button).Parent.Parent == pnlButtons)
                            {
                                (control as Button).BackColor = Color.FromArgb(90, 139, 196);
                            }
                            else if((control as Button).Name.StartsWith("btnSub") && (control as Button).Parent == pnlReport)
                            {
                                
                                (control as Button).BackColor = Color.FromArgb(72, 115, 164);
                            }
                            
                        }                        
                    }
                    else
                    {
                        func(control.Controls);
                    }

                }
            };

            func(Controls);

        }
    }
}
