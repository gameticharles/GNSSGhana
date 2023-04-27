using GenericParsing;
using ghGPS.Classes;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ghGPS.Forms
{
    public partial class FileToTable : MetroSuite.MetroForm
    {
        private bool IsForConversion = false;
        public FileToTable()
        {
            InitializeComponent();            
        }

        private string myDelimeter;
        private string FileName;
        //Function to recreate the Table of Attributes of the File
        public static DataTable dt = new DataTable("Data");

        private void editSwitch_CheckedChanged(object sender, bool isChecked)
        {
            if (!editSwitch.Checked)
            {
                rbtnHasHeader.Checked = false;
                rbtnNoHeader.Checked = false;
            }
        }

        private void rbtnHasHeader_CheckedChanged(object sender, EventArgs e)
        {
            editSwitch.Checked = true;
        }

        private void rbtn_Delimiter_CheckedChanged(object sender, EventArgs e)
        {
            myDelimeter = ((RadioButton)sender).Tag.ToString();

            tbxOther.Enabled = rbtnOther.Checked ? true : false;
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {

            //--------------------- Validating ------------------ 
            if (string.IsNullOrEmpty(tbxFilePath.Text) && string.IsNullOrEmpty(FileName))
            {
                MessageBox.Show("File does not exist. Please select a file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (myDelimeter == "Other" && string.IsNullOrEmpty(tbxOther.Text))
            {
                MessageBox.Show("Enter a delimiter or a separator", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbxOther.Focus();
                return;
            }

            //------------------- Checks ------------------
            if (string.IsNullOrEmpty(tbxFilePath.Text) && !string.IsNullOrEmpty(FileName))
            {
                tbxFilePath.Text = FileName;
            }

            if (!string.IsNullOrEmpty(tbxFilePath.Text) && !string.IsNullOrEmpty(FileName) && tbxFilePath.Text != FileName)
            {
                FileName = tbxFilePath.Text;
            }

            if (!string.IsNullOrEmpty(tbxFilePath.Text) && string.IsNullOrEmpty(FileName))
            {
                FileName = tbxFilePath.Text;

                if (System.IO.File.Exists(FileName) == false)
                {
                    MessageBox.Show("File does not exits. Please select a file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (myDelimeter == "Other")
            {
                myDelimeter = tbxOther.Text;
            }

            GNSS_Functions.File2Convert = FileName;
            GNSS_Functions.Delimiter = myDelimeter;
                       

            if (Path.GetExtension(GNSS_Functions.File2Convert).ToLower().StartsWith(".xl"))
            {
                dt = ReadFileToDataTable(GNSS_Functions.File2Convert, int.Parse(tbxToStartReadingFrom.Text));                
            }
            else
            {                
                //Try to get header
                string headerData = File.ReadAllLines(GNSS_Functions.File2Convert)[int.Parse(tbxToStartReadingFrom.Text)];                
                HasHeader(headerData.Split(new string[] { GNSS_Functions.Delimiter }, StringSplitOptions.None));

                dt = ReadFileToDataTable(GNSS_Functions.File2Convert, GNSS_Functions.Delimiter.ToCharArray()[0], rbtnHasHeader.Checked ? true : false, int.Parse(tbxToStartReadingFrom.Text));
            }

            dgvTables.DataSource = dt;

            //Populated values or items into the cbx
            populateColumns();

            rbtnLatDMS.Checked = true;
            rbtnLonDMS.Checked = true;

            btnApply.Enabled = true;

            GC.Collect();

        }

        void HasHeader(string[] fieldsInfo)
        {
            
            int countNumericHeader = 0;
            int countTextHeader = 0;

            foreach (var item in fieldsInfo)
            {

                if (Simulate.IsNumeric(item))
                {
                    countNumericHeader += 1;
                }

                if (Simulate.IsNumeric(item) == false)
                {
                    countTextHeader += 1;
                }
            }

            if (countTextHeader > 1)
            {
                rbtnHasHeader.Checked = true;
            }

            if (countNumericHeader > 0)
            {
                rbtnNoHeader.Checked = true;
            }

            if (countTextHeader < 2 && countNumericHeader < 1)
            {
                rbtnNoHeader.Checked = true;
            }
        }
        
        private void tbxFilePath_ButtonClick(object sender, EventArgs e)
        {

            //Import from file to the table
            ImportToTable();

        }

        public DataTable ReadFileToDataTable(string inputData, char Delimiter, bool HasHeader, int LinesToSkip = 0)
        {
            DataTable dtTable = new DataTable("Data");
            using (GenericParserAdapter parser = new GenericParserAdapter(inputData))
            {
                
                parser.ColumnDelimiter = Delimiter;
                parser.SkipStartingDataRows = LinesToSkip;
                parser.FirstRowHasHeader = HasHeader;
                //parser.IncludeFileLineNumber = true;
                parser.MaxBufferSize = 16384;
                parser.TextQualifier = '\"';
                //parser.ExpectedColumnCount = 5; 
                

                try
                {
                    dtTable = parser.GetDataTable();                   
                    
                }
                catch (ParsingException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            return dtTable;
        }

        public DataTable ReadFileToDataTable(string inputData, int LinesToSkip = 0)
        {
            //Open the workbook (or create it if it doesn't exist)
            var fi = new FileInfo(inputData);

            ExcelPackage excelPackage = new ExcelPackage(fi);
            DataTable dt = new DataTable();
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[1];

            //check if the worksheet is completely empty
            if (worksheet.Dimension == null)
            {
                return dt;
            }

            //create a list to hold the column names
            List<string> columnNames = new List<string>();

            //needed to keep track of empty column headers
            int currentColumn = 1;
            List<string> headerFeilds = new List<string>();
            
            foreach (var cell in worksheet.Cells[LinesToSkip + 1, 1, 1, worksheet.Dimension.End.Column])
            {
                headerFeilds.Add(cell.Text.Trim());                
            }

            HasHeader(headerFeilds.ToArray());

            //loop all columns in the sheet and add them to the datatable
            foreach (var cell in worksheet.Cells[LinesToSkip + 1, 1, 1, worksheet.Dimension.End.Column])
            {
                string columnName = cell.Text.Trim();                               

                //check if the previous header was empty and add it if it was
                if (cell.Start.Column != currentColumn)
                {
                    columnNames.Add("Column_" + currentColumn);
                    dt.Columns.Add("Column_" + currentColumn);
                    currentColumn++;
                }

                //add the column name to the list to count the duplicates
                columnNames.Add(columnName);

                //count the duplicate column names and make them unique to avoid the exception
                //A column named 'Name' already belongs to this DataTable
                int occurrences = columnNames.Count(x => x.Equals(columnName));
                if (occurrences > 1)
                {
                    columnName = columnName + "_" + occurrences;
                }

                //add the column to the datatable
                dt.Columns.Add(columnName);

                currentColumn++;
            }

            //start adding the contents of the excel file to the datatable
            for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
            {
                var row = worksheet.Cells[i, 1, i, worksheet.Dimension.End.Column];
                DataRow newRow = dt.NewRow();

                //loop all cells in the row
                foreach (var cell in row)
                {
                    newRow[cell.Start.Column - 1] = cell.Text;
                }

                dt.Rows.Add(newRow);
            }

            return dt;
        }                       

        #region IMPORT FROM FILE TO DATAGRID VIEW

        private openFileBrowser ofd;
        public object ImportToTable()
        {

            ofd = new openFileBrowser();

           
            ofd.fileFilterComboBox1.FilterItems = GNSS_Functions.Filter_Type;
            ofd.fileFilterComboBox1.SelectedIndex = 0;
            ofd.shellView.MultiSelect = false;

            GNSS_Functions.OFDheaderText = "Select Data file: ";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                dgvTables.DataSource = null;

                FileName = GNSS_Functions.SelectedItems[0].FileSystemPath;
                tbxFilePath.Text = FileName;

                //Enable button
                btnLoadFile.Enabled = true;

                btnApply.Enabled = false;

                //Clear all items in cbx
                clearCbx(false);

                //Count number of Columns
                lblColumnCount.Text = 0.ToString();

                //Count number of records or rows in the table
                lblRowCount.Text = 0.ToString();

            }


            //INSTANT C# NOTE: Inserted the following 'return' since all code paths must return a value in C#:
            return null;
        }
        #endregion

        public void populateColumns()
        {

            //Clear all items in cbx
            clearCbx(true);

            //Add new items to the cbx  dt.Columns
            foreach (DataColumn col in dt.Columns)
            {
                foreach (Control pnl in GroupBox1.Controls)
                {
                    if (pnl.Name.Contains("pnl"))
                    {

                        foreach (Control cbx in pnl.Controls)
                        {
                            if (cbx.Name.Contains("cbx"))
                            {
                                ((ComboBox)cbx).Items.Add(col.ColumnName);
                                ((ComboBox)cbx).DisplayMember = col.ColumnName;


                                //Check for sub panel
                            }
                            else if (cbx.Name.Contains("pnl"))
                            {
                                foreach (Control Ang_pnl in cbx.Controls)
                                {
                                    if (Ang_pnl.Name.Contains("cbx"))
                                    {
                                        ((ComboBox)Ang_pnl).Items.Add(col.ColumnName);
                                        ((ComboBox)Ang_pnl).DisplayMember = col.ColumnName;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Check for Cartesian
            if (MainScreen.CoordinateConversion.cbxType.SelectedIndex == 0 ||
                MainScreen.CoordinateConversion.cbxType.SelectedIndex == 1 ||
                MainScreen.CoordinateConversion.cbxType.SelectedIndex == 2)
            {

                chbxHeight.Enabled = false;

            }

        }

        private void clearCbx(bool Enable_Disable)
        {

            //Clear all items and disable cbx 

            foreach (Control pnl in GroupBox1.Controls)
            {
                if (pnl.Name.Contains("pnl"))
                {

                    foreach (Control cbx in pnl.Controls)
                    {
                        if (cbx.Name.Contains("cbx") || cbx.Name.Contains("rbtn") || cbx.Name.Contains("chbx"))
                        {

                            //Clear content if cbx
                            if (cbx.Name.Contains("cbx"))
                            {
                                
                                ((ComboBox)cbx).Items.Clear();
                            }

                            //Disable
                            if (Enable_Disable == false)
                            {
                                cbx.Enabled = false;
                            }
                            else
                            {
                                cbx.Enabled = true;
                            }

                            //Check for sub panel
                        }
                        else if (cbx.Name.Contains("pnl"))
                        {
                            foreach (Control cbxx in cbx.Controls)
                            {
                                if (cbxx.Name.Contains("cbx") || cbxx.Name.Contains("rbtn") || cbxx.Name.Contains("chbx"))
                                {

                                    //Clear content if cbx
                                    if (cbxx.Name.Contains("cbx"))
                                    {
                                        ((ComboBox)cbxx).Items.Clear();
                                    }

                                    //Disable
                                    if (Enable_Disable == false)
                                    {
                                        cbxx.Enabled = false;
                                    }
                                    else
                                    {
                                        cbxx.Enabled = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }


        }

        private void dgvTables_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            //Count number of Columns
            lblColumnCount.Text = dgvTables.Columns.Count.ToString();

            //Count number of records or rows in the table
            lblRowCount.Text = (dgvTables.Rows.Count).ToString();
        }

        private void dgvTables_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            //Count number of Columns
            lblColumnCount.Text = dgvTables.Columns.Count.ToString();

            //Count number of records or rows in the table
            lblRowCount.Text = (dgvTables.Rows.Count).ToString();
        }

        private void Lat_SelectionChange(object sender, EventArgs e)
        {

            int degIndex = cbxLatDeg.SelectedIndex;

            try
            {
                if (rbtnLatDMS.Checked)
                {
                    cbxLatMin.SelectedIndex = degIndex + 1;
                    cbxLatSec.SelectedIndex = degIndex + 2;
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void Lon_SelectionChange(object sender, EventArgs e)
        {

            int degIndex = cbxLonDeg.SelectedIndex;

            try
            {
                if (rbtnLonDMS.Checked)
                {
                    cbxLonMin.SelectedIndex = degIndex + 1;
                    cbxLonSec.SelectedIndex = degIndex + 2;
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void Azm_SelectionChange(object sender, EventArgs e)
        {

            int degIndex = cbxAzmDeg.SelectedIndex;

            try
            {
                if (chbxAzmMin.Checked)
                {
                    cbxAzmMin.SelectedIndex = degIndex + 1;
                }

                if (chbxAzmSec.Checked)
                {
                    cbxAzmSec.SelectedIndex = degIndex + 2;
                }

            }
            catch (Exception ex)
            {

            }

        }

        private void allChbx_CheckedChanged(object sender, EventArgs e)
        {
            var cbxSender = (MetroFramework.Controls.MetroCheckBox)sender;
            try
            {
                //Search the control or cbx associated with the lable
                foreach (Control pnl in GroupBox1.Controls)
                {
                    if (pnl.Name.Contains("pnl"))
                    {

                        foreach (Control cbx in pnl.Controls)
                        {
                            if (cbx.Name.Contains(cbxSender.Tag.ToString()))
                            {

                                //Set control
                                cbx.Enabled = (cbxSender.Checked == true) ? true : false;

                                //Check for sub panel
                            }
                            else if (cbx.Name.Contains("pnl"))
                            {
                                foreach (Control cbxx in cbx.Controls)
                                {
                                    if (cbxx.Name.Contains(cbxSender.Tag.ToString()))
                                    {
                                        //Set control
                                        cbxx.Enabled = (cbxSender.Checked == true) ? true : false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void chbxLatDeg_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnLatDMS.Checked == true)
            {

            }
            else
            {
                chbxLatDeg.Checked = true;
            }
        }

        private void chbxLonDeg_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnLonDMS.Checked == true)
            {

            }
            else
            {
                chbxLonDeg.Checked = true;
            }
        }

        private void rbtnLatDMS_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnLatDMS.Checked == true)
            {
                chbxLatMIn.Checked = true;
                chbxLatSec.Checked = true;

                chbxLatMIn.Enabled = true;
                chbxLatSec.Enabled = true;
            }
            else
            {
                chbxLatMIn.Checked = false;
                chbxLatSec.Checked = false;

                chbxLatMIn.Enabled = false;
                chbxLatSec.Enabled = false;

            }

        }

        private void rbtnLonDMS_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnLonDMS.Checked == true)
            {
                chbxLonMin.Checked = true;
                chbxLonSec.Checked = true;

                chbxLonMin.Enabled = true;
                chbxLonSec.Enabled = true;
            }
            else
            {
                chbxLonMin.Checked = false;
                chbxLonSec.Checked = false;

                chbxLonMin.Enabled = false;
                chbxLonSec.Enabled = false;
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (string.IsNullOrEmpty(tbxFilePath.Text))
                {
                    return;
                }

                if (chbxHeight.Visible == true)
                {
                    if (chbxHeight.Checked && cbxHeight.SelectedIndex < 0)
                    {
                        MessageBox.Show("Select the Height field and try again" + Environment.NewLine + "Or check it off!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        if (MainScreen.CoordinateConversion.cbxType.SelectedIndex == 0 ||
                            MainScreen.CoordinateConversion.cbxType.SelectedIndex == 1 ||
                            MainScreen.CoordinateConversion.cbxType.SelectedIndex == 2)
                        {
                            if (cbxHeight.SelectedIndex < 0)
                            {
                                MessageBox.Show("Select the Height field and try again" + Environment.NewLine + "Or check it off!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }
                }

                if (chbxName.Checked && cbxName.SelectedIndex < 0)
                {
                    MessageBox.Show("Select the Name field and try again" + Environment.NewLine + "Or check it off!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
                if (pnlProjectValues.Visible == false)
                {

                    if (chbxLatDeg.Checked && cbxLatDeg.SelectedIndex < 0)
                    {
                        MessageBox.Show("Select the field for Degrees of the Latitude and try again" + Environment.NewLine + "Or check it off!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (chbxLatMIn.Checked && cbxLatMin.SelectedIndex < 0)
                    {
                        MessageBox.Show("Select the field for Minutes of the Latitude and try again" + Environment.NewLine + "Or check it off!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (chbxLatSec.Checked && cbxLatSec.SelectedIndex < 0)
                    {
                        MessageBox.Show("Select the field for Seconds of the Latitude and try again" + Environment.NewLine + "Or check it off!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (chbxLonDeg.Checked && cbxLonDeg.SelectedIndex < 0)
                    {
                        MessageBox.Show("Select the field for Degrees of the Longitude and try again" + Environment.NewLine + "Or check it off!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (chbxLonMin.Checked && cbxLonMin.SelectedIndex < 0)
                    {
                        MessageBox.Show("Select the field for Minutes of the Longitude and try again" + Environment.NewLine + "Or check it off!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (chbxLonSec.Checked && cbxLonSec.SelectedIndex < 0)
                    {
                        MessageBox.Show("Select the field for Seconds of the Longitude and try again" + Environment.NewLine + "Or check it off!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                }
                else
                {

                    if (cbxEasting.SelectedIndex < 0)
                    {
                        MessageBox.Show("Select the Easting field and try again" + Environment.NewLine + "Or check it off!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (cbxNorthing.SelectedIndex < 0)
                    {
                        MessageBox.Show("Select the Northing field and try again" + Environment.NewLine + "Or check it off!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    
                }

                
                //Do the computation
                InsertRow(GNSS_Functions.openedForm);

                this.Close();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //clsConvertion.firstControlPt = true;
            }
        }

        #region Insert Row
        public void InsertRow(UserControl frmOpen)
        {         

            if (IsForConversion)
            {
                if (MainScreen.CoordinateConversion.dgvSourceProjected.Visible == true)
                {
                    int ptName = 0;
                    int Eastings = 0;
                    int Northings = 0;
                    int ht = 0;

                    var myImportSourceDATA = MainScreen.CoordinateConversion.dgvSourceProjected;

                    for (var i = 0; i <= dgvTables.Rows.Count - 1; i++)
                    {

                        myImportSourceDATA.Rows.Add();

                        if (chbxName.Checked)
                        {
                            ptName = cbxName.SelectedIndex;

                            myImportSourceDATA[0, i].Value = dgvTables[ptName, i].Value;
                        }
                        else
                        {
                            myImportSourceDATA[0, i].Value = "PT " + i;
                        }

                        Eastings = cbxEasting.SelectedIndex;
                        myImportSourceDATA[1, i].Value = dgvTables[Eastings, i].Value;

                        Northings = cbxNorthing.SelectedIndex;
                        myImportSourceDATA[2, i].Value = dgvTables[Northings, i].Value;

                        
                        if (chbxHeight.Checked)
                        {
                            ht = cbxHeight.SelectedIndex;

                            myImportSourceDATA[3, i].Value = dgvTables[ht, i].Value;
                        }
                        else
                        {
                            //Check for Cartesian
                            if (MainScreen.CoordinateConversion.cbxType.SelectedIndex == 0 ||
                                MainScreen.CoordinateConversion.cbxType.SelectedIndex == 1 ||
                                MainScreen.CoordinateConversion.cbxType.SelectedIndex == 2)
                            {
                                ht = cbxHeight.SelectedIndex;

                                myImportSourceDATA[3, i].Value = dgvTables[ht, i].Value;
                            }
                            else
                            {
                                myImportSourceDATA[3, i].Value = 0;
                            }
                            
                        }

                        //height(Ellipsoid)
                        GNSS_Functions.cellValue = Convert.ToDouble(myImportSourceDATA[1, i].Value);
                        myImportSourceDATA[1, i].Value = GNSS_Functions.cellValue.ToString("#.0000");

                        //height(Ellipsoid)
                        GNSS_Functions.cellValue = Convert.ToDouble(myImportSourceDATA[2, i].Value);
                        myImportSourceDATA[2, i].Value = GNSS_Functions.cellValue.ToString("#.0000");

                        //height(Ellipsoid)
                        GNSS_Functions.cellValue = Convert.ToDouble(myImportSourceDATA[3, i].Value);
                        myImportSourceDATA[3, i].Value = GNSS_Functions.cellValue.ToString("#.0000");

                    }

                    //Count number of rows
                    MainScreen.CoordinateConversion.lblPointCounts.Text = GNSS_Functions.RowCount(myImportSourceDATA).ToString();
                }
                else
                {

                    int ptName = 0;
                    int latDeg = 0;
                    int latMin = 0;
                    int latSec = 0;
                    int lonDeg = 0;
                    int lonMin = 0;
                    int lonSec = 0;
                    int ht = 0;

                    var myImportSourceDATA = MainScreen.CoordinateConversion.dgvSourceGeog;

                    for (var i = 0; i <= dgvTables.Rows.Count - 1; i++)
                    {

                        myImportSourceDATA.Rows.Add();

                        if (chbxName.Checked)
                        {
                            ptName = cbxName.SelectedIndex;

                            myImportSourceDATA[0, i].Value = dgvTables[ptName, i].Value;
                        }
                        else
                        {
                            myImportSourceDATA[0, i].Value = "PT " + i;
                        }

                        if (chbxLatDeg.Checked)
                        {
                            latDeg = cbxLatDeg.SelectedIndex;

                            myImportSourceDATA[1, i].Value = dgvTables[latDeg, i].Value;
                        }
                        else
                        {
                            myImportSourceDATA[1, i].Value = 0;
                        }

                        if (chbxLatMIn.Checked)
                        {
                            latMin = cbxLatMin.SelectedIndex;

                            myImportSourceDATA[2, i].Value = dgvTables[latMin, i].Value;
                        }
                        else
                        {
                            myImportSourceDATA[2, i].Value = 0;
                        }

                        if (chbxLatSec.Checked)
                        {
                            latSec = cbxLatSec.SelectedIndex;

                            myImportSourceDATA[3, i].Value = dgvTables[latSec, i].Value;
                        }
                        else
                        {
                            myImportSourceDATA[3, i].Value = 0;
                        }

                        if (chbxLonDeg.Checked)
                        {
                            lonDeg = cbxLonDeg.SelectedIndex;

                            myImportSourceDATA[4, i].Value = dgvTables[lonDeg, i].Value;
                        }
                        else
                        {
                            myImportSourceDATA[4, i].Value = 0;
                        }

                        if (chbxLonMin.Checked)
                        {
                            lonMin = cbxLonMin.SelectedIndex;

                            myImportSourceDATA[5, i].Value = dgvTables[lonMin, i].Value;
                        }
                        else
                        {
                            myImportSourceDATA[5, i].Value = 0;
                        }

                        if (chbxLonSec.Checked)
                        {
                            lonSec = cbxLonSec.SelectedIndex;

                            myImportSourceDATA[6, i].Value = dgvTables[lonSec, i].Value;
                        }
                        else
                        {
                            myImportSourceDATA[6, i].Value = 0;
                        }

                        if (chbxHeight.Checked)
                        {
                            ht = cbxHeight.SelectedIndex;

                            myImportSourceDATA[7, i].Value = dgvTables[ht, i].Value;
                        }
                        else
                        {
                            myImportSourceDATA[7, i].Value = 0;
                        }

                        //deg(latitude)
                        if (myImportSourceDATA[1, i].Value.ToString() == "-0" || myImportSourceDATA[1, i].Value.ToString() == "-00" || myImportSourceDATA[1, i].Value.ToString() == "-000")
                        {
                            myImportSourceDATA[1, i].Value = "-000";
                        }
                        else
                        {
                            GNSS_Functions.cellValue = Convert.ToDouble(myImportSourceDATA[1, i].Value);
                            if (GNSS_Functions.cellValue.ToString().Contains("."))
                            {
                                var dms = GNSS_Functions.DegDec2DMS(GNSS_Functions.cellValue);
                                myImportSourceDATA[1, i].Value = dms[0].ToString("000");

                                //min(latitude)
                                myImportSourceDATA[2, i].Value = dms[1].ToString("00");
                                //sec(latitude)
                                myImportSourceDATA[3, i].Value = dms[2];

                            }
                            else
                            {
                                myImportSourceDATA[1, i].Value = GNSS_Functions.cellValue.ToString("000");

                                //min(latitude)
                                GNSS_Functions.cellValue = Convert.ToDouble(myImportSourceDATA[2, i].Value);
                                myImportSourceDATA[2, i].Value = GNSS_Functions.cellValue.ToString("00");
                                //sec(latitude)
                                GNSS_Functions.cellValue = Convert.ToDouble(myImportSourceDATA[3, i].Value);
                                myImportSourceDATA[3, i].Value = GNSS_Functions.cellValue.ToString("00.0000");
                            }
                        }

                        //deg(longitude)
                        if (myImportSourceDATA[4, i].Value.ToString() == "-0" || myImportSourceDATA[4, i].Value.ToString() == "-00" || myImportSourceDATA[4, i].Value.ToString() == "-000")
                        {
                            myImportSourceDATA[4, i].Value = "-000";
                        }
                        else
                        {
                            GNSS_Functions.cellValue = Convert.ToDouble(myImportSourceDATA[4, i].Value);
                            if (GNSS_Functions.cellValue.ToString().Contains("."))
                            {
                                var dms = GNSS_Functions.DegDec2DMS(GNSS_Functions.cellValue);
                                myImportSourceDATA[4, i].Value = dms[0].ToString("000");

                                //min(longitude)
                                myImportSourceDATA[5, i].Value = dms[1].ToString("00");
                                //sec(longitude)
                                myImportSourceDATA[6, i].Value = dms[2];

                            }
                            else
                            {
                                myImportSourceDATA[4, i].Value = GNSS_Functions.cellValue.ToString("000");
                                //min(longitude)
                                GNSS_Functions.cellValue = Convert.ToDouble(myImportSourceDATA[5, i].Value);
                                myImportSourceDATA[5, i].Value = GNSS_Functions.cellValue.ToString("00");
                                //sec(longitude)
                                GNSS_Functions.cellValue = Convert.ToDouble(myImportSourceDATA[6, i].Value);
                                myImportSourceDATA[6, i].Value = GNSS_Functions.cellValue.ToString("00.0000");

                            }
                        }

                        //height(Ellipsoid)
                        GNSS_Functions.cellValue = Convert.ToDouble(myImportSourceDATA[7, i].Value);
                        myImportSourceDATA[7, i].Value = GNSS_Functions.cellValue.ToString("#.0000");

                    }

                    //Count number of rows
                    MainScreen.CoordinateConversion.lblPointCounts.Text = GNSS_Functions.RowCount(myImportSourceDATA).ToString();

                }
            }
            else
            {
                int ptName = 0;
                int Eastings = 0;
                int Northings = 0;                

                var myImportSourceDATA = MainScreen.manualDataImport.dgvPoints;

                for (var i = 0; i <= dgvTables.Rows.Count - 1; i++)
                {

                    myImportSourceDATA.Rows.Add();

                    if (chbxName.Checked)
                    {
                        ptName = cbxName.SelectedIndex;

                        myImportSourceDATA[0, i].Value = dgvTables[ptName, i].Value;
                    }
                    else
                    {
                        myImportSourceDATA[0, i].Value = "PT " + i;
                    }

                    Eastings = cbxEasting.SelectedIndex;
                    myImportSourceDATA[1, i].Value = dgvTables[Eastings, i].Value;

                    Northings = cbxNorthing.SelectedIndex;
                    myImportSourceDATA[2, i].Value = dgvTables[Northings, i].Value;
                                        
                    //height(Ellipsoid)
                    GNSS_Functions.cellValue = Convert.ToDouble(myImportSourceDATA[1, i].Value);
                    myImportSourceDATA[1, i].Value = GNSS_Functions.cellValue.ToString("#.000");

                    //height(Ellipsoid)
                    GNSS_Functions.cellValue = Convert.ToDouble(myImportSourceDATA[2, i].Value);
                    myImportSourceDATA[2, i].Value = GNSS_Functions.cellValue.ToString("#.000");

                    
                }
                
            }

            //Dump memories
            dt.Dispose();

        }
        
        #endregion

        public void HideAzm()
        {
            pnlAzimuth.Visible = true;
            cbxAzmDeg.Visible = false;
            cbxAzmMin.Visible = false;
            cbxAzmSec.Visible = false;

            chbxAzmDeg.Visible = false;
            chbxAzmMin.Visible = false;
            chbxAzmSec.Visible = false;
        }

        private void FileToTable_Load(object sender, EventArgs e)
        {
            
            IsForConversion = (GNSS_Functions.openedForm.Name == nameof(MainScreen.CoordinateConversion)) ? true : false;
            chbxHeight.Visible = IsForConversion;
            cbxHeight.Visible = IsForConversion;

            myDelimeter = ",";
            
            //Clear all items in cbx
            clearCbx(false);

            //Setting Parameters
            //if (GNSS_Functions.openedForm.Name == nameof(frmGeodeticCalc))
            //{
            //    pnlProjectValues.Visible = false;
            //    if (GNSS_Functions.openedForm.pnlReverseHeight.Visible == true)
            //    {
            //        pnlAzimuth.Visible = false;

            //    }
            //    else if (GNSS_Functions.openedForm.pnlDirect.Visible == true)
            //    {
            //        pnlAzimuth.Visible = true;

            //    }
            //    else
            //    {
            //        HideAzm();

            //        chbxHeight.Visible = false;
            //        cbxHeight.Visible = false;
            //    }

            //}
            if (IsForConversion) //Conversion
            {
                HideAzm();

                //Check for Cartesian
                if (MainScreen.CoordinateConversion.cbxType.SelectedIndex == 0 ||
                    MainScreen.CoordinateConversion.cbxType.SelectedIndex == 1 ||
                    MainScreen.CoordinateConversion.cbxType.SelectedIndex == 2)
                {
                    lblEasting.Text = "X:";
                    lblNorthing.Text = "Y:";
                    chbxHeight.Text = "Z:";

                    chbxHeight.Enabled = false;
                   
                }

                pnlProjectValues.Visible = (MainScreen.CoordinateConversion.dgvSourceProjected.Visible == true) ? true : false;

            }
            else if (!IsForConversion) 
            {
                HideAzm();
                pnlProjectValues.Visible = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            GC.Collect();
            this.Close();
        }

        private void metroTextbox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            GNSS_Functions.ValidateTbx(tbxToStartReadingFrom, e);
        }
    }
}
