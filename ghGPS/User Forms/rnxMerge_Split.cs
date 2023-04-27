using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ghGPS.Classes;
using ghGPS.Forms;

namespace ghGPS.User_Forms
{
    public partial class rnxMerge_Split : UserControl
    {
        public rnxMerge_Split()
        {
            InitializeComponent();

            var timeInterval = 120;
            while (timeInterval <= 86400)
            {
                cbxTimeSeconds.Items.Add(timeInterval);
                timeInterval += 60;
            }

            cbxTimeSeconds.SelectedIndex = 0;
            cbxFileType.SelectedIndex = 0;
            cbxMerge_Split.SelectedIndex = 0;
            cbxOBSFileType.SelectedIndex = 0;

            cbxEndTimeFormat.SelectedIndex = 0;
            cbxMinutesAdd_Subtract.SelectedIndex = 0;
            cbxHourAdd_Subtract.SelectedIndex = 0;

            //Resize dgv
            dgvRoverResize();
        }

        private string filePath = string.Empty;
        private string file_folder = string.Empty;

        private int SelectedIndex = 0;


        int selectedCount = 0;
        private void dgvRoverResize()
        {
            //Check the row count and resize
            if (dgvFiles.RowCount > 15)
            {
                dgvFiles.Columns[1].Width = dgvFiles.Width - 220;
            }
            else
            {
                dgvFiles.Columns[1].Width = dgvFiles.Width - 203;
            }

            UpdateButtonExecute();

            lblRoverCounts.Text = dgvFiles.RowCount.ToString("00");
            
        }

        private void dgvFiles_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
           
            //Check the row count and resize
            dgvRoverResize();
            //Get prefix
            getPrefix();
        }


        void UpdateButtonExecute()
        {
            if (dgvFiles.RowCount > 0)
            {
                btnExecuteMerge_Split.Enabled = true;
            }
            else
            {
                btnExecuteMerge_Split.Enabled = false;
            }
            
        }

        private void dgvFiles_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            

            //Check the row count and resize
            dgvRoverResize();

            //Get prefix
            getPrefix();
        }

        private void dgvFiles_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                SelectedIndex = dgvFiles.CurrentRow.Index;   
            }
            catch (Exception ex)
            {

                //MessageBox.Show(ex.Message);
            }

        }

        private void btnODB_Click(object sender, EventArgs e)
        {
            //Declare open file and browser variable
            var ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.FilterIndex = 0;
            ofd.Multiselect = true;
            ofd.Title = "Select File(s)";
            ofd.Filter = "RINEX OBS (*.obs,*.*O)|*.obs;*.*O"; // "RINEX OBS (*.obs,*.*O)|*.obs;*.*O|RINEX NAV (*.nav,*.*N,*.*P)|*.nav;*.*N;*.*P|All Files (*.*)|*.*";

            var fbd = new FolderBrowserDialog();

            //Count the items
            var iRow = dgvFiles.Rows.Count - 1;

            if (file_folder == "File")
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    //Set the file path
                    tbxFilePath.Text = Path.GetDirectoryName(ofd.FileNames[0]);

                    
                    //Add items to the grid view
                    for (int i = 0; i < ofd.FileNames.Length; i++)
                    {
                        dgvFiles.Rows.Add();
                        iRow++;

                        dgvFiles[0, iRow].Value = true;
                        dgvFiles[1, iRow].Value = Path.GetFileNameWithoutExtension(ofd.FileNames[i]);
                        dgvFiles[2, iRow].Value = Path.GetExtension(ofd.FileNames[i]).ToUpper();
                        dgvFiles[3, iRow].Value = GNSS_Functions.GetFileSizeInBytes(ofd.FileNames[i]);

                    }
                }
            }
            else
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    //Set the file path
                    tbxFilePath.Text = fbd.SelectedPath;

                    dgvFiles.Rows.Clear();
                    MainScreen.RINEXTEQC.cbxSelectedRINEX_File.Items.Clear();
                    iRow = dgvFiles.Rows.Count - 1;

                    foreach (var item in Directory.GetFiles(fbd.SelectedPath))
                    {
                        if (Path.GetExtension(item).ToUpper().Equals(".OBS") || Path.GetExtension(item).ToUpper().EndsWith("O"))
                        {
                            dgvFiles.Rows.Add();
                            iRow++;

                            iRow = dgvFiles.Rows.Count - 1;
                            dgvFiles[0, iRow].Value = true;
                            dgvFiles[1, iRow].Value = Path.GetFileNameWithoutExtension(item);
                            dgvFiles[2, iRow].Value = Path.GetExtension(item).ToUpper();
                            dgvFiles[3, iRow].Value = GNSS_Functions.GetFileSizeInBytes(item);
                            dgvFiles[6, iRow].Value = item;

                            
                            if (Path.GetExtension(item).EndsWith("o"))
                            {

                                if (File.Exists(Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + Path.GetExtension(item).Substring(0, 3) + "n")))
                                {
                                    dgvFiles[4, iRow].Value = 1;
                                }
                                else
                                {
                                    dgvFiles[4, iRow].Value = 0;
                                }

                                if (File.Exists(Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + Path.GetExtension(item).Substring(0, 3) + "g")))
                                {
                                    dgvFiles[5, iRow].Value = 1;
                                }
                                else
                                {
                                    dgvFiles[5, iRow].Value = 0;
                                }
                            }
                            else if (Path.GetExtension(item).EndsWith("O"))
                            {

                                if (File.Exists(Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + Path.GetExtension(item).Substring(0, 3) + "N")))
                                {
                                    dgvFiles[4, iRow].Value = 1;
                                }
                                else
                                {
                                    dgvFiles[4, iRow].Value = 0;
                                }

                                if (File.Exists(Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + Path.GetExtension(item).Substring(0, 3) + "G")))
                                {
                                    dgvFiles[5, iRow].Value = 1;
                                }
                                else
                                {
                                    dgvFiles[5, iRow].Value = 0;
                                }
                            }
                            else if (Path.GetExtension(item).Contains("obs"))
                            {

                                if (File.Exists(Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + ".nav")))
                                {
                                    dgvFiles[4, iRow].Value = 1;
                                }
                                else
                                {
                                    dgvFiles[4, iRow].Value = 0;
                                }

                                if (File.Exists(Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + ".glo")))
                                {
                                    dgvFiles[5, iRow].Value = 1;
                                }
                                else
                                {
                                    dgvFiles[5, iRow].Value = 0;
                                }
                            }
                            else if (Path.GetExtension(item).Contains("OBS"))
                            {

                                if (File.Exists(Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + ".NAV")))
                                {
                                    dgvFiles[4, iRow].Value = 1;
                                }
                                else
                                {
                                    dgvFiles[4, iRow].Value = 0;
                                }

                                if (File.Exists(Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + ".GLO")))
                                {
                                    dgvFiles[5, iRow].Value = 1;
                                }
                                else
                                {
                                    dgvFiles[5, iRow].Value = 0;
                                }
                            }

                        }

                    }
                }
            }

            ////AutoFIll
            //MergeForm.tbxNavigationFileName.Text = "rinex_merged_nav"; 
            //MergeForm.tbxObservationFileName.Text = "rinex_merged_obs"; 
        }

        private void cbxFileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            file_folder = cbxFileType.SelectedItem.ToString();
        }


        IList<string> strings = new List<string>();
        void getPrefix()
        {
            try
            {
                for (int i = 0; i < dgvFiles.RowCount; i++)
                {
                    if ((bool)dgvFiles[0, i].Value == true)
                    {

                        strings.Add(dgvFiles[1, i].Value.ToString());
                    }
                }

                //Get the prefix of the items
                var prefixData = StringExtension.GetPrefix(strings) + "*";

                lblPrefix.Text = prefixData;

                tbxObservationFileName.Text = prefixData;
            }
            catch (Exception)
            {

            }
            
        }

        private async void btnExecuteMerge_Split_Click(object sender, EventArgs e)
        {
            btnExecuteMerge_Split.Enabled = false;
            if (cbxMerge_Split.SelectedItem.ToString() == "Merge Data" & tbxObservationFileName.Text == "")
            {
                MessageBox.Show("Enter Output file name","Error",MessageBoxButtons.OK , MessageBoxIcon.Error);

                btnExecuteMerge_Split.Enabled = true;
                return;
            }

            using (var waiting = new WaitLoadingForm())
            {
                waiting.OpenedForm = "Merge Split";
                waiting.ShowDialog();
            }

            btnExecuteMerge_Split.Enabled = true;

        }

        private void chbxCheckAll_CheckedChanged(object sender, bool isChecked)
        {

                for (int i = 0; i < dgvFiles.RowCount ; i++)
                {
                    dgvFiles[0, i].Value = (chbxCheckAll.Checked) ? true: false;
                }
                           
        }

        private void dgvFiles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvFiles.CurrentCell.ColumnIndex == 0)
            {
                dgvRoverResize();
            }            
        }

        private void cbxMerge_Split_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblPrefixLabel.Visible = false;
            if (cbxMerge_Split.SelectedItem.ToString() == "Merge Data")
            {
                pnlMerge.BringToFront();
                lblPrefixLabel.Visible = true;
            }
            else if (cbxMerge_Split.SelectedItem.ToString() == "Decimate Data")
            {
                pnlDecimate.BringToFront();
            }
            else
            {
                pnlSplit.BringToFront();
            }

            if (dgvFiles.RowCount > 0)
            {
                getPrefix();
            }
        }

        private void chbxStartTime_CheckedChanged(object sender, bool isChecked)
        {           
            StartDateTimePicker.Enabled = (!chbxStartTime.Checked) ? true : false;           
        }

        private void chbxStopTime_CheckedChanged(object sender, bool isChecked)
        {
            enable_disable_Controls();
        }

        private void cbxEndTimeFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxEndTimeFormat.SelectedIndex == 0) //End Time
            {
                pnlEndTime.BringToFront();               
            }
            else if (cbxEndTimeFormat.SelectedIndex == 1) //Add hour(s) from start or hours from end
            {
                pnlHours.BringToFront();
            }
            else
            {
                pnlMinutes.BringToFront();
            }

            enable_disable_Controls();
        }

        /// <summary>
        /// Check and enable/disable controls
        /// </summary>
        void enable_disable_Controls()
        {
            StopDateTimePicker.Enabled = (!chbxStopTime.Checked) ? true : false;
           
            cbxHourAdd_Subtract.Enabled = (!chbxStopTime.Checked) ? true : false;
            cbxMinutesAdd_Subtract.Enabled = (!chbxStopTime.Checked) ? true : false;
            tbxMunites.Enabled = (!chbxStopTime.Checked) ? true : false;
            tbxHours.Enabled = (!chbxStopTime.Checked) ? true : false;
        }
    }
}
