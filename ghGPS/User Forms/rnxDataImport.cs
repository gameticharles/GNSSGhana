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
using System.Diagnostics;
using ghGPS.Classes;
using System.Threading;

namespace ghGPS.User_Forms
{
    public partial class rnxDataImport : UserControl
    {
        public rnxDataImport()
        {
            InitializeComponent();

            cbxFileType.SelectedIndex = 0;            

            //Resize dgv
            dgvRoverResize();
        }

        private string filePath = string.Empty;
        private string file_folder = string.Empty;

        public int SelectedIndex = 0;
        public string ObsExt = "";
        public string GPSNavExt = "";
        public string GLONavExt = "";

        private void dgvRoverResize()
        {
            //Check the row count and resize
            if (dgvFiles.RowCount > 15)
            {
                dgvFiles.Columns[0].Width = dgvFiles.Width - 170;
            }
            else
            {
                dgvFiles.Columns[0].Width = dgvFiles.Width - 153;
            }
        }

        private void dgvFiles_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            lblRoverCounts.Text = dgvFiles.RowCount.ToString("00");

            //Check the row count and resize
            dgvRoverResize();

            UpdateCBX();
        }
    
        void UpdateCBX()
        {

            try
            {
                MainScreen.RINEXTEQC.cbxSelectedRINEX_File.Items.Clear();
                for (int i = 0; i < dgvFiles.RowCount; i++)
                {
                    MainScreen.RINEXTEQC.cbxSelectedRINEX_File.Items.Add(dgvFiles[5, i].Value.ToString());
                }

            }
            catch (Exception)
            {

               
            }
           
        }

        private void dgvFiles_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            lblRoverCounts.Text = dgvFiles.RowCount.ToString("00");

            //Check the row count and resize
            dgvRoverResize();

            UpdateCBX();
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
                    MainScreen.RINEXTEQC.cbxSelectedRINEX_File.Items.Clear();

                    //Set the file path
                    tbxFilePath.Text = Path.GetDirectoryName(ofd.FileNames[0]);

                    //Add items to the grid view
                    for (int i = 0; i < ofd.FileNames.Length; i++)
                    {
                        
                        dgvFiles.Rows.Add();
                        iRow++;

                        dgvFiles[0, iRow].Value = Path.GetFileNameWithoutExtension(ofd.FileNames[i]);
                        dgvFiles[1, iRow].Value = Path.GetExtension(ofd.FileNames[i]).ToUpper();
                        dgvFiles[2, iRow].Value = GNSS_Functions.GetFileSizeInBytes(ofd.FileNames[i]);

                        var item = ofd.FileNames[i];
                        dgvFiles[5, iRow].Value = item;
                        if (Path.GetExtension(item).ToUpper().EndsWith("O"))
                        {

                            if (File.Exists(Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + Path.GetExtension(item).Substring(0, 3) + "n")))
                            {
                                dgvFiles[3, iRow].Value = 1;
                            }
                            else
                            {
                                dgvFiles[3, iRow].Value = 0;
                            }

                            if (File.Exists(Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + Path.GetExtension(item).Substring(0, 3) + "g")))
                            {
                                dgvFiles[4, iRow].Value = 1;
                            }
                            else
                            {
                                dgvFiles[4, iRow].Value = 0;
                            }
                        }
                        else if (Path.GetExtension(item).ToUpper().Contains("OBS"))
                        {

                            if (File.Exists(Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + ".NAV")))
                            {
                                dgvFiles[3, iRow].Value = 1;
                            }
                            else
                            {
                                dgvFiles[3, iRow].Value = 0;
                            }

                            if (File.Exists(Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + ".GLO")))
                            {
                                dgvFiles[4, iRow].Value = 1;
                            }
                            else
                            {
                                dgvFiles[4, iRow].Value = 0;
                            }
                        }
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

                            iRow = dgvFiles.Rows.Count - 1;dgvFiles[0, iRow].Value = Path.GetFileNameWithoutExtension(item);
                            dgvFiles[1, iRow].Value = Path.GetExtension(item).ToUpper();
                            dgvFiles[2, iRow].Value = GNSS_Functions.GetFileSizeInBytes(item);
                            dgvFiles[5, iRow].Value = item;
                            
                            if (Path.GetExtension(item).ToUpper().EndsWith("O"))
                            {
                                
                                if (File.Exists(Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + Path.GetExtension(item).Substring(0,3) + "n")))
                                {
                                    dgvFiles[3, iRow].Value = 1;
                                }
                                else
                                {
                                    dgvFiles[3, iRow].Value = 0;
                                }

                                if (File.Exists(Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + Path.GetExtension(item).Substring(0, 3) + "g")))
                                {
                                    dgvFiles[4, iRow].Value = 1;
                                }
                                else
                                {
                                    dgvFiles[4, iRow].Value = 0;
                                }
                            }
                            else if (Path.GetExtension(item).ToUpper().Contains("OBS"))
                            {
                               
                                if (File.Exists(Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + ".NAV")))
                                {
                                    dgvFiles[3, iRow].Value = 1;
                                }
                                else
                                {
                                    dgvFiles[3, iRow].Value = 0;
                                }

                                if (File.Exists(Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + ".GLO")))
                                {
                                    dgvFiles[4, iRow].Value = 1;
                                }
                                else
                                {
                                    dgvFiles[4, iRow].Value = 0;
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

       
        private void dgvFiles_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                SelectedIndex = dgvFiles.CurrentRow.Index;

                if ((int)dgvFiles[3, SelectedIndex].Value == 1)
                {
                    lblObsGPS.BackColor = Color.GreenYellow;
                    lblEphGPS.BackColor = Color.GreenYellow;
                }
                else
                {
                    lblObsGPS.BackColor = Color.White;
                    lblEphGPS.BackColor = Color.White;
                }

                if ((int)dgvFiles[4, SelectedIndex].Value == 1)
                {
                    lblObsGLO.BackColor = Color.GreenYellow;
                    lblEphGLO.BackColor = Color.GreenYellow;
                }
                else
                {
                    lblObsGLO.BackColor = Color.White;
                    lblEphGLO.BackColor = Color.White;
                }

                //Select the item in cbx
                MainScreen.RINEXTEQC.cbxSelectedRINEX_File.SelectedItem = dgvFiles[5, SelectedIndex].Value;
            }
            catch (Exception ex)
            {

                //MessageBox.Show(ex.Message);
            }
           
        }

        //private void ExtractResource(string resName)
        //{
        //    object ob = Properties.Resources.ResourceManager.GetObject(resName, originalCulture);
        //    byte[] myResBytes = (byte[])ob;
        //    using (FileStream fsDst = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "A3E5.exe"), FileMode.CreateNew, FileAccess.Write))
        //    {
        //        byte[] bytes = myResBytes;
        //        fsDst.Write(bytes, 0, bytes.Length);

        //        fsDst.Close();
        //        fsDst.Dispose();
        //    }
        //}

    }
}
