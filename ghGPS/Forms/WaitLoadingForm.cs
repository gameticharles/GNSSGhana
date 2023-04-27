using ghGPS.Classes;
using ghGPS.User_Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ghGPS.Forms
{
    public partial class WaitLoadingForm : MetroSuite.MetroForm
    {
        public WaitLoadingForm()
        {
            InitializeComponent();

            timer1.Start();
        }

        public static string ExtraStaus { get; set; }

        int DotNumber = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (DotNumber == 6)
            {
                DotNumber = 0;
            }
            DotNumber++;

            lblStatus.Text = "Processing" + new string('.', DotNumber) + "\n\n" + ExtraStaus;
        }

        private string ObsExt = "";
        private string GPSNavExt = "";
        private string GLONavExt = "";
        private static string OutObsExt = "";
        private static string OutGPSNavExt = "";
        private static string OutGLONavExt = "";
        private static string OutMixedExt = "";

        /// <summary>
        /// 
        /// </summary>
        private async void QualityCheck()
        {
            var QC = RINEXTEQC.QualityCheck;

            string fileDirectory = RINEXTEQC.FileDirectory;
            string checkFormat = "";

            //Copy teqc to the folder
            if (!File.Exists(Path.Combine(fileDirectory, "teqc.exe")))
            {
                File.Copy(Path.Combine(Environment.CurrentDirectory, "teqc.exe"), Path.Combine(fileDirectory, "teqc.exe"));
            }

            checkFormat = @"teqc " + ((QC.cbxReportType.SelectedItem.ToString() == "Full") ? "+qc " : "+qcq ");

            if (QC.cbxReportType.SelectedItem.ToString() == "Full")
            {
                checkFormat += (QC.chbxGPS.Checked) ? "-G " : "";
                checkFormat += (QC.chbxGALILEO.Checked) ? "-E " : "";
                checkFormat += (QC.chbxGLONASS.Checked) ? "-R " : "";
                checkFormat += (QC.chbxQZSS.Checked) ? "-J " : "";
                checkFormat += (QC.chbxSBAS.Checked) ? "-S " : "";
                checkFormat += (QC.chbxBEIDOU.Checked) ? "-C " : "";
            }

            checkFormat += "+sym ";

            //input file name
            checkFormat += RINEXTEQC.Filename + RINEXTEQC.ext;

            //if (cbxReportType.SelectedItem.ToString() != "Full")
            //{
            //    //Include QC Symbols
            //    checkFormat += " 2 > / dev / null";
            //}

            //Out to 
            checkFormat += " > ";

            checkFormat += "Output.txt";

            checkFormat += " 2>&1";
            //if (cbxReportType.SelectedItem.ToString() != "Full")
            //{
            //    //Include QC Symbols
            //    checkFormat += " 2>&1";
            //}

            var result = await ProcessEx.RunCMD(new string[] { @"cd " + fileDirectory, checkFormat, "exit" });

            //Delete the file "teqc.exe"
            File.Delete(Path.Combine(fileDirectory, "teqc.exe"));

            string file = "";

            WaitLoadingForm.ExtraStaus = "Checking result";

            //Make sure the solution file is deleted
            if (File.Exists(Path.Combine(fileDirectory, RINEXTEQC.Filename + RINEXTEQC.ext.Substring(0, 3) + "S")))
            {
                WaitLoadingForm.ExtraStaus = "Reading result";

                file = File.ReadAllText(Path.Combine(fileDirectory, RINEXTEQC.Filename + RINEXTEQC.ext.Substring(0, 3) + "S"));
                File.Delete(Path.Combine(fileDirectory, RINEXTEQC.Filename + RINEXTEQC.ext.Substring(0, 3) + "S"));
                File.Delete(Path.Combine(fileDirectory, "Output.txt"));
            }
            else
            {
                WaitLoadingForm.ExtraStaus = "Reading result";

                file = File.ReadAllText(Path.Combine(fileDirectory, "Output.txt"));
                File.Delete(Path.Combine(fileDirectory, "Output.txt"));
            }

            using (RichTextBox rtbx = new RichTextBox())
            {
                rtbx.Font = new Font("Courier New", float.Parse("8"));

                rtbx.Text = file;

                QC.rtbxQualityCheck.RtfText = rtbx.Rtf;
            }

            this.Close();

        }

        /// <summary>
        /// 
        /// </summary>
        private async void MergeSplit()
        {
            var MST = RINEXTEQC.Merge_Split;

            string fileDirectory = MST.tbxFilePath.Text;

            //Copy teqc to the folder
            if (!File.Exists(Path.Combine(fileDirectory, "teqc.exe")))
            {
                File.Copy(Path.Combine(Environment.CurrentDirectory, "teqc.exe"), Path.Combine(fileDirectory, "teqc.exe"));
            }

            try
            {
                if (MST.cbxMerge_Split.SelectedItem.ToString() == "Merge Data")
                {
                    var item = MST.dgvFiles[2, 0].Value.ToString().Substring(0, 3);

                    var ext = MST.cbxOBSFileType.SelectedItem.ToString();
                    if (ext.ToUpper() == ".YYO")
                    {
                        OutObsExt = item + ((ext == ".YYO") ? "O" : "o");
                        OutGPSNavExt = item + ((ext == ".YYO") ? "N" : "n");
                        OutGLONavExt = item + ((ext == ".YYO") ? "G" : "g");
                        OutMixedExt = item + ((ext == ".YYO") ? "P" : "p");
                    }
                    else
                    {
                        OutObsExt = ((ext == ".OBS") ? ".OBS" : ".obs");
                        OutGPSNavExt = ((ext == ".OBS") ? ".NAV" : ".nav");
                        OutGLONavExt = ((ext == ".OBS") ? ".GLO" : ".glo");
                    }
                    //minutes
                    ExtraStaus = "Merging Started";

                    string ObservationItems = "";
                    string GPSNAVItems = "";
                    string GLONAVItems = "";
                    string MIXNAVItems = "";
                    int CountGPSNav = 0;
                    int CountGLONav = 0;
                    int CountMIXNav = 0;

                    IList<string> ProcessItems = new List<string>();

                    ProcessItems.Add(@"cd " + fileDirectory);

                    for (int i = 0; i < MST.dgvFiles.RowCount; i++)
                    {

                        if ((bool)MST.dgvFiles[0, i].Value == true)
                        {

                            ObservationItems += MST.dgvFiles[1, i].Value.ToString() + ((MST.dgvFiles[2, i].Value.ToString().ToUpper().EndsWith("O")) ? MST.dgvFiles[2, i].Value.ToString().Substring(0, 3) + "O" : ".OBS") + " ";

                            if (File.Exists(Path.Combine(fileDirectory, MST.dgvFiles[1, i].Value.ToString() + ((MST.dgvFiles[2, i].Value.ToString().ToUpper().EndsWith("O")) ? MST.dgvFiles[2, i].Value.ToString().Substring(0, 3) + "N" : ".NAV"))))
                            {
                                GPSNAVItems += MST.dgvFiles[1, i].Value.ToString() + ((MST.dgvFiles[2, i].Value.ToString().ToUpper().EndsWith("O")) ? MST.dgvFiles[2, i].Value.ToString().Substring(0, 3) + "N" : ".NAV") + " ";
                                CountGPSNav++;
                            }

                            if (File.Exists(Path.Combine(fileDirectory, MST.dgvFiles[1, i].Value.ToString() + ((MST.dgvFiles[2, i].Value.ToString().ToUpper().EndsWith("O")) ? MST.dgvFiles[2, i].Value.ToString().Substring(0, 3) + "G" : ".GLO"))))
                            {
                                GLONAVItems += MST.dgvFiles[1, i].Value.ToString() + ((MST.dgvFiles[2, i].Value.ToString().ToUpper().EndsWith("O")) ? MST.dgvFiles[2, i].Value.ToString().Substring(0, 3) + "G" : ".GLO") + " ";
                                CountGLONav++;
                            }

                            if (File.Exists(Path.Combine(fileDirectory, MST.dgvFiles[1, i].Value.ToString() + MST.dgvFiles[2, i].Value.ToString().Substring(0, 3) + "P")))
                            {
                                MIXNAVItems += MST.dgvFiles[1, i].Value.ToString() + MST.dgvFiles[2, i].Value.ToString().Substring(0, 3) + "P" + " ";
                                CountMIXNav++;
                            }

                        }
                    }

                    var obsQuery = "";

                    obsQuery += @"teqc ";


                    //Add options
                    if (!MST.chbxStartTime.Checked) //Checked
                    {
                        obsQuery += "-st " + MST.StartDateTimePicker.Text.Replace(" ", "T");
                    }

                    if (!MST.chbxStopTime.Checked)
                    {
                        if (MST.cbxEndTimeFormat.SelectedIndex == 0)
                        {
                            obsQuery += " -e " + MST.StopDateTimePicker.Text.Replace(" ", "T") + " ";
                        }
                        else if (MST.cbxEndTimeFormat.SelectedIndex == 1)
                        {
                            obsQuery += " " + MST.cbxHourAdd_Subtract.SelectedItem.ToString() + " " + MST.tbxHours.Text + " ";
                        }
                        else
                        {
                            obsQuery += " " + MST.cbxMinutesAdd_Subtract.SelectedItem.ToString() + " " + MST.tbxMunites.Text + " ";
                        }
                    }

                    obsQuery += ObservationItems + " > ";

                    obsQuery += StringExtension.RemoveSpecialCharacters(MST.tbxObservationFileName.Text) + OutObsExt;

                    ProcessItems.Add(obsQuery);

                    if (CountGPSNav > 0) //GPS NAV 
                    {
                        ProcessItems.Add(@"teqc " + GPSNAVItems + " > " + StringExtension.RemoveSpecialCharacters(MST.tbxObservationFileName.Text) + OutGPSNavExt);
                    }

                    if (CountGLONav > 0) //GONASS NAV
                    {
                        ProcessItems.Add(@"teqc " + GLONAVItems + " > " + StringExtension.RemoveSpecialCharacters(MST.tbxObservationFileName.Text) + OutGLONavExt);
                    }

                    if (CountMIXNav > 0) //MIXED NAV
                    {
                        ProcessItems.Add(@"teqc " + MIXNAVItems + " > " + StringExtension.RemoveSpecialCharacters(MST.tbxObservationFileName.Text) + OutMixedExt);
                    }

                    ProcessItems.Add("exit");

                    await ProcessEx.RunCMD(ProcessItems.ToArray());
                   
                }
                else if (MST.cbxMerge_Split.SelectedItem.ToString() == "Decimate Data")
                {

                    int CountSelected = 0;
                    IList<string> strings = new List<string>();
                    IList<string> ObservationItems = new List<string>();

                    ObservationItems.Add(@"cd " + fileDirectory);

                    for (int i = 0; i < MST.dgvFiles.RowCount; i++)
                    {
                        if ((bool)MST.dgvFiles[0, i].Value == true)
                        {
                            CountSelected++;
                            strings.Add(MST.dgvFiles[1, i].Value.ToString());

                            var filename = MST.dgvFiles[1, i].Value.ToString();
                            var ext = ((MST.dgvFiles[2, i].Value.ToString().ToUpper().EndsWith("O")) ? MST.dgvFiles[2, i].Value.ToString().Substring(0, 3) + "O" : ".OBS");

                            //teqc -O.int 0.1 -O.dec 3600 SURSFEB01.17O > SURSFEB01_10.17O
                            ObservationItems.Add("teqc -O.int 0.1 -O.dec " + MST.cbxTimeSeconds.SelectedItem.ToString() + " " + filename + ext + " > " + filename + "_" + MST.cbxTimeSeconds.SelectedItem.ToString() + ext);

                        }
                    }

                    //Get the prefix of the items
                    var prefixData = StringExtension.GetPrefix(strings) + "*";

                    ObservationItems.Add("exit");

                    //var result = await ProcessEx.RunCMD(new string[] { @"cd " + fileDirectory, @"teqc " + prefixData + ObsExt + " > " + tbxObservationFileName.Text + OutObsExt, @"teqc " + prefixData + GPSNavExt + " > " + tbxObservationFileName.Text + OutGPSNavExt, @"teqc " + prefixData + GLONavExt + " > " + tbxObservationFileName.Text + OutGLONavExt, "exit" });
                    var result = await ProcessEx.RunCMD(ObservationItems.ToArray());
                }
                else
                {

                    IList<string> ObservationItems = new List<string>();

                    ObservationItems.Add(@"cd " + fileDirectory);

                    for (int i = 0; i < MST.dgvFiles.RowCount; i++)
                    {
                        if ((bool)MST.dgvFiles[0, i].Value == true)
                        {

                            var filename = MST.dgvFiles[1, i].Value.ToString();
                            var ext = ((MST.dgvFiles[2, i].Value.ToString().ToUpper().EndsWith("O")) ? MST.dgvFiles[2, i].Value.ToString().Substring(0, 3) + "O" : ".OBS");
                            var splitInterval = "";

                            if (true)
                            {
                                //teqc -O.int 0.1 -O.dec 3600 SURSFEB01.17O > SURSFEB01_10.17O
                                ObservationItems.Add("teqc -tbin " + "1h" + " " + filename + ext);
                            }                            

                        }
                    }

                    ObservationItems.Add("exit");

                    var result = await ProcessEx.RunCMD(ObservationItems.ToArray());

                }

                //Delete the file "teqc.exe"
                File.Delete(Path.Combine(fileDirectory, "teqc.exe"));

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async void RunRINEXBase()
        {
            //Read the RINEX File          
            await GNSS_Functions.RinexReadsObsFile(RoverFilePath);

            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        private async void ProcessRINEXDATA()
        {
            
            MainScreen.MainScreenTitle.TopMost = true;
            TopMost = true;

            ProcessedSolution.IsBaseNotDone = true;
            //=========================Enter rover results=============================            
            for (int i = 0; i < RoversFiles.Length; i++)
            {
                
                //Precise Point Positioned
                var approxPosAll = await ProcessEx.RunRINEX(RoversFiles[i], GNSS_Functions.MainFrm);

                GNSS_Functions.approxPos[0] = approxPosAll[0];
                GNSS_Functions.approxPos[1] = approxPosAll[1];
                GNSS_Functions.approxPos[2] = approxPosAll[2];

                if (approxPosAll[3] == 1)
                {
                    GNSS_Functions.SolutionFix = "Fixed";
                }
                else
                {
                    GNSS_Functions.SolutionFix = "Float";
                }

                if (ProcessedSolution.IsBaseNotDone)
                {
                    ProcessedSolution.CreateListItem(sbPointList, GNSS_Functions.BaseSiteID, new double[] { double.Parse(GNSS_Functions.BaseX), double.Parse(GNSS_Functions.BaseY), double.Parse(GNSS_Functions.BaseZ) });
                    ProcessedSolution.IsBaseNotDone = false;
                }

                ProcessedSolution.CreateListItem(sbPointList, RoverIDs[i], GNSS_Functions.approxPos);
            }
           

            Close();

            MainScreen.MainScreenTitle.TopMost = false;

        }

        /// <summary>
        /// 
        /// </summary>
        private async void RunRINEXRover()
        {
            
            if (GNSS_Functions.SelectedItems.Count() > 0)
            {
                int RowIndex = 0;

                GNSS_Functions.IsBase = false;

                //Set the text label to the file name and path
                for (int i = 0; i < GNSS_Functions.SelectedItems.Count(); i++)
                {
                    RowIndex = MainScreen.GNSSDataImport.dgvRovers.RowCount;
                    MainScreen.GNSSDataImport.dgvRovers.Rows.Add();

                    string FilePath = GNSS_Functions.SelectedItems[i].FileSystemPath;

                    //Read the RINEX File
                    await GNSS_Functions.RinexReadsObsFile(FilePath);

                    MainScreen.GNSSDataImport.dgvRovers.Rows[RowIndex].Cells[0].Value = FilePath;

                    MainScreen.GNSSDataImport.dgvRovers.Rows[RowIndex].Cells[1].Value = GNSS_Functions.GetUntilOrEmpty(FilePath.Substring(FilePath.LastIndexOf(@"\") + 1).ToString(), ".");

                    MainScreen.GNSSDataImport.dgvRovers.Rows[RowIndex].Cells[2].Value = GNSS_Functions.FirtObsString;

                    MainScreen.GNSSDataImport.dgvRovers.Rows[RowIndex].Cells[3].Value = GNSS_Functions.lastObsString;

                    MainScreen.GNSSDataImport.dgvRovers.Rows[RowIndex].Cells[4].Value = GNSS_Functions.Epoch.ToString();

                    MainScreen.GNSSDataImport.dgvRovers.Rows[RowIndex].Cells[5].Value = GNSS_Functions.tInterval.ToString();

                    MainScreen.GNSSDataImport.dgvRovers.Rows[RowIndex].Cells[6].Value = GNSS_Functions.GetFileSizeInBytes(FilePath);


                    MainScreen.GNSSDataImport.dgvRovers.Rows[RowIndex].Cells[7].Value = Math.Round(GNSS_Functions.approxPos[0], 3).ToString();  //E

                    MainScreen.GNSSDataImport.dgvRovers.Rows[RowIndex].Cells[8].Value = Math.Round(GNSS_Functions.approxPos[1], 3).ToString();  //N

                    MainScreen.GNSSDataImport.dgvRovers.Rows[RowIndex].Cells[9].Value = Math.Round(GNSS_Functions.approxPos[2], 3).ToString(); //Z

                    MainScreen.GNSSDataImport.dgvRovers.Rows[RowIndex].Cells[10].Value = "0";

                    MainScreen.GNSSDataImport.dgvRovers.Rows[RowIndex].Cells[11].Value = "Vertical";

                }
            }

            this.Close();
        }

        public string[] RoversFiles { get; set; } = new string[] { };
        public string[] RoverIDs { get; set; } = new string[] { };

        public StringBuilder sbPointList { get; set; } = new StringBuilder();

        public string RoverFilePath { get; set; } = "";
        public string OpenedForm { get; set; } = "";
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WaitLoadingForm_Load(object sender, EventArgs e)
        {
            if (OpenedForm == "Quality Check")
            {
                QualityCheck();
            }
            else if(OpenedForm == "Merge Split")
            {               
                MergeSplit();
            }
            else if (OpenedForm == "Read RINEX Base")
            {
                RunRINEXBase();
            }
            else if(OpenedForm == "Read RINEX Rovers")
            {
                RunRINEXRover();
            }
            else if (OpenedForm == "Process Data")
            {
                ProcessRINEXDATA();
            }

        }
    }
}
