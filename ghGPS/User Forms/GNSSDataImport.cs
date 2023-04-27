using System;
using System.Linq;
using System.Windows.Forms;
using ghGPS.Properties;
using ghGPS.Forms;
using System.Diagnostics;
using ghGPS.Classes;
using AxMapWinGIS;
using MapWinGIS;
using System.Collections.Generic;
using static ghGPS.User_Forms.BaseLineChart;
using ghGPS.Classes.CoordinateSystems;
using ghGPS.Classes.CoordinateSystems.Transformations;


namespace ghGPS.User_Forms
{
    public partial class GNSSDataImport : UserControl
    {
        public GNSSDataImport()
        {
            InitializeComponent();
        }
             

        private void btnBack_Click(object sender, EventArgs e)
        {
            //Go back to create new project
            MainScreen.createProject.BringToFront();            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //Clear, reset fields and go to home page
            GNSS_Functions.ClearCreateNewProject();
        }

        private void dgvRoverResize()
        {
            //Check the row count and resize
            if (dgvRovers.RowCount > 8)
            {
                dgvRovers.Columns["File_Path"].Width = 153;
            }
            else
            {
                dgvRovers.Columns["File_Path"].Width = 170;
            }            
        }

        private void dgvRovers_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            lblRoverCounts.Text = dgvRovers.RowCount.ToString("00");

            //Check the row count and resize
            dgvRoverResize();
        }

        private void dgvRovers_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            lblRoverCounts.Text = dgvRovers.RowCount.ToString("00");

            //Check the row count and resize
            dgvRoverResize();
        }

        private void dgvRovers_MouseLeave(object sender, EventArgs e)
        {
            //dgvRovers.ClearSelection();
        }

        public void hideShowDetails()
        {

            //Check the height
            if (pnlDetailsContainer.Height == 77)
            {
                pnlDetailsContainer.Height = 148;

                btnShowMoreDetails.Image = Resources.up;
            }
            else
            {
                pnlDetailsContainer.Height = 77;

                btnShowMoreDetails.Image = Resources.down;
            }
        }

        private void btnShowMoreDetails_Click(object sender, EventArgs e)
        {
            //Control Details
            hideShowDetails();
        }

        private async void btnOpenFolderPath_Click(object sender, EventArgs e)
        {
            using (var ofd = new openFileBrowser())
            {
                ofd.fileFilterComboBox1.FilterItems = GNSS_Functions.FilterObservaton;
                ofd.fileFilterComboBox1.SelectedIndex = 0;
                ofd.shellView.MultiSelect = false;
                GNSS_Functions.OFDheaderText = "Select Base observation file: ";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (GNSS_Functions.SelectedItems.Count() > 0)
                        {

                            string FilePath = GNSS_Functions.SelectedItems[0].FileSystemPath;
                            
                            //Set the text label to the file name and path
                            tbxBasePath.Text = FilePath;

                            using (var waiting = new WaitLoadingForm())
                            {
                                waiting.OpenedForm = "Read RINEX Base";
                                waiting.RoverFilePath = FilePath;

                                GNSS_Functions.IsBase = true;

                                waiting.ShowDialog();
                            }                        
                           

                            if (GNSS_Functions.success == 1)
                            {
                                lblFileSize.Text = GNSS_Functions.GetFileSizeInBytes(FilePath);

                                lblTimeInteerval.Text = GNSS_Functions.tInterval.ToString();

                                lblEpoch.Text = GNSS_Functions.Epoch.ToString();

                                lblStartObservationTime.Text = GNSS_Functions.FirtObsString;

                                lblStopObservationTime.Text = GNSS_Functions.lastObsString;

                                lblSiteID.Text = GNSS_Functions.GetUntilOrEmpty(FilePath.Substring(FilePath.LastIndexOf(@"\") + 1).ToString(), ".");

                                lblAproxEastings.Text = Math.Round(GNSS_Functions.approxPos[0], 3).ToString();   //E

                                lblAproxNorthings.Text = Math.Round(GNSS_Functions.approxPos[1], 3).ToString();  //N

                                lblAproxHeight.Text = Math.Round(GNSS_Functions.approxPos[2], 3).ToString();     //Z

                                pnlDetailsContainer.Height = 77;
                            }

                        }                                            

                    }
                    catch (Exception ex)
                    {
                        // Get stack trace for the exception with source file information
                        var st = new StackTrace(ex, true);
                        // Get the top stack frame
                        var frame = st.GetFrame(0);
                        // Get the line number from the stack frame
                        var Atline = frame.GetFileLineNumber();
                        Console.WriteLine(ex.Message + "\nCaused by:\n" + ex.Source + ex.TargetSite + "\nOn line: " + Atline);

                    }
                }
                else
                {
                    if (tbxBasePath.Text == "")
                    {
                        pnlDetailsContainer.Height = 0;
                    }                                      

                }
            };

            GC.Collect();

            //Hide the Input Coordinates
            pnlInput.Visible = false;
        }

        private async void btnSelectRovers_Click(object sender, EventArgs e)
        {
            using (var ofd = new openFileBrowser())
            {
                ofd.fileFilterComboBox1.FilterItems = GNSS_Functions.FilterObservaton;
                ofd.fileFilterComboBox1.SelectedIndex = 0;
                ofd.shellView.MultiSelect = true;
                GNSS_Functions.OFDheaderText = "Select Rover observation file(s): ";
                
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var waiting = new WaitLoadingForm())
                        {
                            waiting.OpenedForm = "Read RINEX Rovers";
                            
                            GNSS_Functions.IsBase = false;

                            waiting.ShowDialog();
                        }

                    }
                    catch (Exception)
                    {


                    }
                }
            };

            GC.Collect();
        }

        private void btnSelectNavigationFile_Click(object sender, EventArgs e)
        {
            using (var ofd = new openFileBrowser())
            {
                ofd.fileFilterComboBox1.FilterItems = GNSS_Functions.FilterNavigation;
                ofd.shellView.MultiSelect = false;
                ofd.fileFilterComboBox1.SelectedIndex = 1;
                GNSS_Functions.OFDheaderText = "Select Navigation file: ";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (GNSS_Functions.SelectedItems.Count() > 0)
                        {
                            //Set the text label to the file name and path
                            tbxNavigationfilePath.Text = GNSS_Functions.SelectedItems[0].FileSystemPath;
                            tbxNavigationfilePath.Tag = GNSS_Functions.SelectedItems[0].FileSystemPath;
                        }

                    }
                    catch (Exception)
                    {


                    }
                }
                
            };

            GC.Collect();

        }

        private string optionalFilter = "";
        private void btnSelectPreciseEphmerisClock_Click(object sender, EventArgs e)
        {
            using (var ofd = new openFileBrowser())
            {
                ofd.fileFilterComboBox1.FilterItems = optionalFilter;
                ofd.fileFilterComboBox1.SelectedIndex = 0;
                ofd.shellView.MultiSelect = false;
                GNSS_Functions.OFDheaderText = "Select Clock File: ";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (GNSS_Functions.SelectedItems.Count() > 0)
                        {
                            //Set the text label to the file name and path
                            tbxPreciseEphemeris_ClockPath.Text = GNSS_Functions.SelectedItems[0].FileSystemPath;
                            tbxPreciseEphemeris_ClockPath.Tag = GNSS_Functions.SelectedItems[0].FileSystemPath;
                        }

                    }
                    catch (Exception)
                    {


                    }
                }
                
            };

            GC.Collect();
        }

        /// <summary>
        /// Occurs when the index of the optional data is changed
        /// Therefore change the filter 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRINEXOptionalFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRINEXOptionalFile.SelectedIndex == 0 || cbxRINEXOptionalFile.SelectedIndex == 1)
            {
                //Enable the file browser
                btnSelectPreciseEphmerisClock.Enabled = true;

                if (cbxRINEXOptionalFile.SelectedIndex == 0)
                {
                    GNSS_Functions.OFDheaderText = "Select Precise Ephemeris or Clock file: ";
                    optionalFilter = GNSS_Functions.FilterPreciseEphemeris;
                }
                else
                {
                    GNSS_Functions.OFDheaderText = "Select RINEX Ionosphere file: ";
                    optionalFilter = GNSS_Functions.FilterIonosphere;
                }
            }
            else
            {
                //Disable the file browser
                btnSelectPreciseEphmerisClock.Enabled = false;
            }            
            
        }

        private void btnSetBase_Click(object sender, EventArgs e)
        {
            using (SetBaseCoordinates SetBaseCoordinates = new SetBaseCoordinates())
            {

                //Set the Approximate Coordinate
                SetBaseCoordinates.lblAproxEastings.Text = this.lblAproxEastings.Text;
                SetBaseCoordinates.lblAproxNorthings.Text = this.lblAproxNorthings.Text;
                SetBaseCoordinates.lblAproxHeight.Text = this.lblAproxHeight.Text;
                SetBaseCoordinates.tbxStationEditedID.Text = this.lblSiteID.Text;

                if (SetBaseCoordinates.ShowDialog() == DialogResult.OK)
                {
                    this.lblInputEastings.Text = string.Format(SetBaseCoordinates.tbxEastings.Text, "00000.000"); 
                    this.lblInputNorthings.Text = string.Format(SetBaseCoordinates.tbxNorthings.Text, "00000.000");
                    this.lblInputHeight.Text = string.Format(SetBaseCoordinates.tbxHeight.Text, "00000.000");
                    this.lblSiteID.Text = SetBaseCoordinates.tbxStationEditedID.Text;

                   

                    //CoordinateSystemConversion cor = new CoordinateSystemConversion();
                    ////Create new coordinate system from built functions
                    //var GHCRS = new GridSystem().GhanaNationalGrid(new TransParams().GH7TransParams());
                    //var LatLonH = cor.ENU2LatLonH(GHCRS, new double[] { double.Parse(MainScreen.GNSSDataImport.lblInputEastings.Text), double.Parse(MainScreen.GNSSDataImport.lblInputNorthings.Text), double.Parse(MainScreen.GNSSDataImport.lblInputHeight.Text) });
                    //var XYZ = cor.LatLonH2XYZ(GHCRS, new double[] { LatLonH[0], LatLonH[1], LatLonH[2] * GHCRS.ToMetersFactor });
                    //var X1Y1Z1 = cor.XYZTransformation(GHCRS.TransParam, XYZ, true);

                    CoordinateSystemFactory cFac = new CoordinateSystemFactory();

                    var IGeoncenTarget = cFac.CreateGeocentricCoordinateSystem(" ", (MainScreen.WGS84 as IProjectedCoordinateSystem).HorizontalDatum, (MainScreen.WGS84 as IProjectedCoordinateSystem).LinearUnit, (MainScreen.WGS84 as IProjectedCoordinateSystem).GeographicCoordinateSystem.PrimeMeridian);

                    MainScreen.trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((MainScreen.GHCRS as IProjectedCoordinateSystem), (MainScreen.WGS84 as IProjectedCoordinateSystem));
                    var ENU = MainScreen.trans.MathTransform.Transform(new double[] { double.Parse(MainScreen.GNSSDataImport.lblInputEastings.Text), double.Parse(MainScreen.GNSSDataImport.lblInputNorthings.Text), double.Parse(MainScreen.GNSSDataImport.lblInputHeight.Text)  });//
                    
                    MainScreen.trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((MainScreen.WGS84 as IProjectedCoordinateSystem), (MainScreen.WGS84 as IProjectedCoordinateSystem).GeographicCoordinateSystem);
                    var LonLatH = MainScreen.trans.MathTransform.Transform(new double[] { ENU[0], ENU[1], ENU[2] * GNSS_Functions.CovertFactor });
                    MainScreen.trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((MainScreen.WGS84 as IProjectedCoordinateSystem).GeographicCoordinateSystem, IGeoncenTarget);
                    var XYZ = MainScreen.trans.MathTransform.Transform(new double[] { LonLatH[0], LonLatH[1], LonLatH[2] });
                                        
                    GNSS_Functions.BaseX = XYZ[0].ToString();
                    GNSS_Functions.BaseY = XYZ[1].ToString();
                    GNSS_Functions.BaseZ = XYZ[2].ToString();

                    //Show the Input Coordinates
                    pnlInput.Visible = true;
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in dgvRovers.SelectedRows)
            {
                dgvRovers.Rows.Remove(item);
            }

            this.Refresh();
        }

        private void GNSSDataImport_Load(object sender, EventArgs e)
        {
            pnlDetailsContainer.Height = 0;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            //Do validation
            if (tbxBasePath.Text == "")
            {
                MessageBox.Show("Select Base file and continue", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbxBasePath.Focus();
                return;
            }

            if (tbxNavigationfilePath.Text == "")
            {
                MessageBox.Show("Select Navigation file and continue", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbxNavigationfilePath.Focus();
                return;
            }

            if (dgvRovers.RowCount == 0)
            {
                MessageBox.Show("Add one or more rover observation files", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);                
                return;
            }

            if (!pnlInput.Visible)
            {
                var msg = MessageBox.Show("Base station has no been set.\n\nPress 'Yes' to use approximate coordinate, \nPress 'No' to use average single Position \nCancel to return and enter ", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                                
                if(msg == DialogResult.Yes)
                {
                    GNSS_Functions.UseAprroximatePos = "Yes";
                }
                else if (msg == DialogResult.No)
                {
                    GNSS_Functions.UseAprroximatePos = "No";
                }
                else
                {
                    return;
                }
            }
            else
            {
                GNSS_Functions.UseAprroximatePos = "Input";
            }

            //Reload all point to BaseLine Chart form
            ReloadPointList();

            MainScreen.baseLineChart.olvPointListTree1.Visible = false;
            MainScreen.baseLineChart.olvtree = MainScreen.baseLineChart.olvPointListTree;
            
            //Prepare a chart for processing
            MainScreen.baseLineChart.BringToFront();
        }
        
        public void ReloadPointList(bool fromImport = true)
        {
            if (fromImport)
            {
                //Clear all nodes
                MainScreen.baseLineChart.olvPointListTree.Items.Clear();

                CreatePointShapefile(MainScreen.baseLineChart.axMap1);
            }
            else
            {
                
                dgvRovers.Rows.Clear();

                int RowIndex = 0;
                foreach (var item in Roverlist)
                {
                    RowIndex = dgvRovers.RowCount;
                    dgvRovers.Rows.Add();

                    dgvRovers.Rows[RowIndex].Cells[0].Value = item.FilePath;

                    dgvRovers.Rows[RowIndex].Cells[1].Value = item.SiteID;

                    dgvRovers.Rows[RowIndex].Cells[2].Value = item.StartTime;

                    dgvRovers.Rows[RowIndex].Cells[3].Value = item.StopTime;

                    dgvRovers.Rows[RowIndex].Cells[4].Value = item.Epoch;

                    dgvRovers.Rows[RowIndex].Cells[5].Value = item.DataRate;

                    dgvRovers.Rows[RowIndex].Cells[6].Value = item.Size;


                    dgvRovers.Rows[RowIndex].Cells[7].Value = item.Eastings;  //E

                    dgvRovers.Rows[RowIndex].Cells[8].Value = item.Northings;  //N

                    dgvRovers.Rows[RowIndex].Cells[9].Value = item.Heights; //Z

                    dgvRovers.Rows[RowIndex].Cells[10].Value = item.AntennaHeight;

                    dgvRovers.Rows[RowIndex].Cells[11].Value = item.AntennaType;
                }
            }
            
        }
               

        // <summary>
        // Creates a point shapefile 
        // </summary>
        public void CreatePointShapefile(AxMap axMap1)
        {            
            //MainScreen.baseLineChart.olvPointListTree.Items.Clear();
            axMap1.RemoveAllLayers();
            axMap1.Refresh();
            bool Result = true;
            

            //Create Temporal Shapefiles
            var sfRovers = new Shapefile();
            var connectingLineSp = new Shapefile();
            var sfBase = new Shapefile();


            //Declare shapefile types
            // MWShapeId field will be added to attribute table
            Result = sfBase.CreateNewWithShapeID("", ShpfileType.SHP_POINT);
            Result = sfRovers.CreateNewWithShapeID("", ShpfileType.SHP_POINT);

            // MWShapeId field will be added to attribute table
            Result = connectingLineSp.CreateNewWithShapeID("", ShpfileType.SHP_POLYLINE);

            var f = new Field();
            f.Type = MapWinGIS.FieldType.STRING_FIELD;
            f.Name = "SiteID";
            f.Width = 20;

            Result = sfRovers.EditInsertField(f, 0);
            Result = sfBase.EditInsertField(f, 0);

            //Base Location
            var Basepnt = new Point();
            Basepnt.x = double.Parse(lblAproxEastings.Text);
            Basepnt.y = double.Parse(lblAproxNorthings.Text );

            Shape PTshp = new Shape();
            PTshp.Create(ShpfileType.SHP_POINT);

            int index = 0;
            PTshp.InsertPoint(Basepnt, ref index);
            sfBase.EditInsertShape(PTshp, ref index);

            Result = sfBase.EditCellValue(0, 0, lblSiteID.Text);

            Roverlist = new List<Rover>();
            //Add Item
            for (int i = 0; i < dgvRovers.Rows.Count; i++)
            {
                     

                var pnt = new Point();
                pnt.x = double.Parse(dgvRovers.Rows[i].Cells[7].Value.ToString());
                pnt.y = double.Parse(dgvRovers.Rows[i].Cells[8].Value.ToString());

                PTshp = new Shape();
                PTshp.Create(ShpfileType.SHP_POINT);

                index = 0;
                PTshp.InsertPoint(pnt, ref index);
                sfRovers.EditInsertShape(PTshp, ref i);

                //insert some integer value
                Result = sfRovers.EditCellValue(0, i, dgvRovers.Rows[i].Cells[1].Value.ToString());

                //=============================Connecting Lines ========================
                Shape LineShp = new Shape();
                LineShp.Create(ShpfileType.SHP_POLYLINE);

                LineShp.InsertPoint(Basepnt, index); //The base point
                LineShp.InsertPoint(pnt, index); //The Rover point

                //Add line to shapefiles
                connectingLineSp.EditInsertShape(LineShp, ref i);

                Roverlist.Add(new Rover(dgvRovers.Rows[i].Cells[0].Value.ToString(), dgvRovers.Rows[i].Cells[1].Value.ToString(), dgvRovers.Rows[i].Cells[2].Value.ToString(),
                    dgvRovers.Rows[i].Cells[3].Value.ToString(), dgvRovers.Rows[i].Cells[4].Value.ToString(), dgvRovers.Rows[i].Cells[5].Value.ToString(), 
                    dgvRovers.Rows[i].Cells[6].Value.ToString(), dgvRovers.Rows[i].Cells[7].Value.ToString(), dgvRovers.Rows[i].Cells[8].Value.ToString(), 
                    dgvRovers.Rows[i].Cells[9].Value.ToString(), dgvRovers.Rows[i].Cells[10].Value.ToString(), dgvRovers.Rows[i].Cells[11].Value.ToString()));
            }


            //Apply labels  and symbol to the shapefile
            GNSS_Functions.labelShapefile(sfRovers, f.Name, 10, tkDefaultPointSymbol.dpsCircle);
            GNSS_Functions.labelShapefile(sfBase, f.Name, 10, tkDefaultPointSymbol.dpsTriangleDown);

            MainScreen.baseLineChart.shpfile = sfRovers;

            MainScreen.baseLineChart.olvPointListTree.SetObjects(AllRoverList);

            // adds shapefile to the map      
            axMap1.AddLayer(sfBase, true);
            MainScreen.baseLineChart.ShapefileHandler = axMap1.AddLayer(sfRovers, true);
            axMap1.AddLayer(connectingLineSp, true);
            
            axMap1.ZoomToMaxExtents();

            // save if needed
            //sf.SaveAs(@"c:\points.shp", null);
        }


        private static List<Rover> AllRoverList
        {
            get { return Roverlist ?? (Roverlist = LoadRovers()); }
        }
        public static List<Rover> Roverlist;

        private static List<Rover> LoadRovers()
        {                        
            return Roverlist;
        }

        

    }
}
