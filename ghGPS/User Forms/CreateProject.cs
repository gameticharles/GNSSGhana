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

namespace ghGPS.User_Forms
{
    public partial class CreateProject : UserControl
    {
        public CreateProject()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            MainScreen.recentStartScreen.BringToFront();

            //Show Settings button
            MainScreen.btnSettings.Show();
                        
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

            //Clear, reset fields and go to home page
            GNSS_Functions.ClearCreateNewProject();
        }

        private void btnOpenFolderPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog() { Description = "Select your project path" })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    tbxFolderPath.Text = fbd.SelectedPath;

                    GNSS_Functions.ProjectPath = fbd.SelectedPath.ToString();
                    
                }
            }
        }

        private void cbxProjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Show the option to include cadastral computation report
            if (cbxProjectType.SelectedIndex > -1)
            {
                if (cbxProjectType.SelectedItem.ToString().Contains("GNSS"))
                {                    
                    GNSS_Functions.IsProcessed = true;
                }
                else
                {                    
                    GNSS_Functions.IsProcessed = false;
                }

                chbxIncludeReport.Visible = GNSS_Functions.IsProcessed;
                lblTrans.Visible = GNSS_Functions.IsProcessed;
                cbxTransformationType.Visible = GNSS_Functions.IsProcessed;
                MainScreen.baseLineChart.pnlAntenna.Visible = GNSS_Functions.IsProcessed;
            }                        
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            //=========================== Begin Validation =====================================
            if (tbxProjectName.Text.Length == 0)
            {
                MessageBox.Show("Enter project or job name and continue", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbxProjectName.Focus();
                return;
            }

            //Validate to check if path field is empty
            if (tbxFolderPath.Text.Length == 0 || tbxFolderPath.Text == null)
            {
                MessageBox.Show("Enter or select the working directory and continue", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbxFolderPath.Focus();
                return;
            }

            //Validate to see if the path in the text field provided exist or not
            if (!System.IO.Directory.Exists(tbxFolderPath.Text))
            {
                MessageBox.Show("Folder path does not exist. \nPlease try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbxFolderPath.Focus();
                return;
            }

            //Check if project type is select or not
            if (cbxProjectType.SelectedIndex == -1)
            {
                MessageBox.Show("Select the type of processing you want to do. \nClick next to continue", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //Check if project type is select or not
            if (cbxUnits.SelectedIndex == -1)
            {
                MessageBox.Show("Select the unit for processing. \nClick next to continue", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //=========================== End Validation =====================================

            GNSS_Functions.ProjectName = tbxProjectName.Text;

            GNSS_Functions.FreqBand = cbxBandFreq.SelectedIndex;

            GNSS_Functions.ProcessingMode = cbxProcessingMode.SelectedIndex;

            //Create a new project item and insert into database            
            using (var pItem = new projectItem())
            {

                //Create Item
                pItem.lblProjectName.Text = GNSS_Functions.ProjectName;
                pItem.lblProjectDate.Text = GNSS_Functions.currentdateTime();
                pItem.lblProjectType.Text = "Type:" + ((cbxProjectType.SelectedItem.ToString().Contains("GNSS")) ? "Processed GNSS" : "Cadastral Report");
                pItem.Tag = GNSS_Functions.ProjectSolutionPath;

                //Insert item into database
                MainScreen.InserProjectToDatabase(pItem);
            }

            if (cbxProjectType.SelectedItem.ToString().Contains("GNSS"))
            {
                //Go to GPS data management
                MainScreen.GNSSDataImport.BringToFront();
            }
            else
            {
                //Go to Cadastral computation
                MainScreen.manualDataImport.BringToFront();
            }

            //Rename MainScreen Form Title
            MainScreen.RenameTitle(GNSS_Functions.ProjectName);

            //Create the solution file .ggp
            System.IO.File.Create(GNSS_Functions.ProjectSolutionPath);
                       
            //Create Data  Directory
            System.IO.Directory.CreateDirectory(GNSS_Functions.DataPath);

            //Create Processed  Directory
            System.IO.Directory.CreateDirectory(GNSS_Functions.ProcessedReportPath);

            //Create Cadastral  Directory
            System.IO.Directory.CreateDirectory(GNSS_Functions.CadastralReportPath);

        }

        private void CreateProject_Load(object sender, EventArgs e)
        {
            //preset settings
            cbxUnits.SelectedIndex = 1;
            cbxProjectType.SelectedIndex = 0;
            cbxTransformationType.SelectedIndex = 1;
            cbxBandFreq.SelectedIndex = 0;
            cbxProcessingMode.SelectedIndex = 3;
        }

        private void cbxUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            GNSS_Functions.UnitsTYPE = cbxUnits.SelectedItem.ToString();
            
            switch (cbxUnits.SelectedItem.ToString())
            {
                case "INT Feet":
                    GNSS_Functions.CovertFactor = 0.3048;
                    break;
                case "Gold Cost Feet":
                    GNSS_Functions.CovertFactor = 0.3047997101815088; 
                    break;
                case "Meters":
                    GNSS_Functions.CovertFactor = 1;
                    break;
                default:
                    break;
            }
        }

        private void chbxIncludeReport_CheckedChanged(object sender, bool isChecked)
        {
            GNSS_Functions.IsCadastralIncluded = (chbxIncludeReport.Checked == true) ? true : false;            
        }

        private void cbxTransformationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            switch (cbxTransformationType.SelectedItem.ToString())
            {
                //
                //case "GH 3 - Parameters":                    
                //    GNSS_Functions.GHCRS = new GridSystem().GhanaNationalGrid(new TransParams().GH3TransParams());
                //    break;
                //case "GH 7 - Parameters":
                //    GNSS_Functions.GHCRS = new GridSystem().GhanaNationalGrid(new TransParams().GH7TransParams());
                //    break;
                //case "GH 10 - Parameters":
                //    GNSS_Functions.GHCRS = new GridSystem().GhanaNationalGrid(new TransParams().GH10TransParams());
                //    break;
                //case "GH MRE":                    
                //    GNSS_Functions.GHCRS.MREParam = new MultipleRegressionEquationParams().GHMREParams();
                //    break;
                //default:
                //    break;

                case "GH 3 - Parameters":
                    MainScreen.GHCRS = MainScreen.GH3Params;
                    break;
                case "GH 7 - Parameters":
                    MainScreen.GHCRS = MainScreen.GH7Params;
                    break;
                case "GH 10 - Parameters":
                    MainScreen.GHCRS = MainScreen.GH10Params;
                    break;
                case "GH MRE":
                    MainScreen.GHCRS = MainScreen.GH7Params;
                    break;
                default:
                    break;
            }
        }
        
    }
}
