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
using ghGPS.Classes.CoordinateSystems;
using ghGPS.Classes.CoordinateSystems.IO;
using ghGPS.Classes.CoordinateSystems.Projections;
using ghGPS.Classes.CoordinateSystems.Transformations;
using System.Globalization;
using MapWinGIS;
using System.Text.RegularExpressions;
using AxMapWinGIS;
using Point = MapWinGIS.Point;

namespace ghGPS.User_Forms
{
    public partial class CoordinateConversion : UserControl
    {
        public CoordinateConversion()
        {
            InitializeComponent();

            //TileProviders providers = axMap1.Tiles.Providers; ;
            //int providerId = (int)tkTileProvider.ProviderCustom + 1;
            //providers.Add(providerId, "MyProvider", "http:/localhost/maps/{zoom}/{x}/{y}.png", tkTileProjection.SphericalMercator, 1, 18);

            //axMap1.Projection = tkMapProjection.PROJECTION_GOOGLE_MERCATOR;
            //axMap1.TileProvider = tkTileProvider.ProviderCustom;
            //axMap1.Tiles.ProviderId = providerId;

            //axMap1.Latitude = 39;
            //axMap1.Longitude = 140;
            //axMap1.CurrentZoom = 8;

            //Populate Tiles providers
            foreach (var tile in Enum.GetNames(typeof(tkTileProvider)))
            {
                cbxTileProvider.Items.Add(tile.SplitCamelCase());
            }

            axMap1.KnownExtents = tkKnownExtents.keGhana;
                       
        }


        CoordinateSystemFactory cFac = new CoordinateSystemFactory();
        private bool TargetRowAddition = false;
        public int FormatSelect = 0;
        private void btnInputCRS_Click(object sender, EventArgs e)
        {
            using (var crs = new Forms.CoordinateSystem())
            {
                crs.Isprojection = Isprojection;
                if (crs.ShowDialog() == DialogResult.OK)
                {                    
                    if (((ToolStripButton)sender).Name == nameof(btnInputCRS))
                    {
                        tbxFromCRS.Text = GNSS_Functions.CrsSelectdName;
                        tbxFromCRS.Tag = GNSS_Functions.CrsSelectdParameters;

                        Source_CRS = CoordinateSystemWktReader.Parse(GNSS_Functions.CrsSelectdParameters) as ICoordinateSystem; //cFac.CreateFromWkt(GNSS_Functions.CrsSelectdParameters);
                       
                    }                    
                    else
                    {
                        tbxToCRS.Text = GNSS_Functions.CrsSelectdName;
                        tbxToCRS.Tag = GNSS_Functions.CrsSelectdParameters;

                        Target_CRS = CoordinateSystemWktReader.Parse(GNSS_Functions.CrsSelectdParameters) as ICoordinateSystem;
                    }                      
                }
            }
        }

        /// <summary>
        /// Get the parameter from wkt
        /// </summary>
        /// <param name="wkt"></param>
        /// <returns></returns>
        List<ProjectionParameter> getParame(string wkt)
        {
            var pcs = CoordinateSystemWktReader.Parse(wkt);
            List<ProjectionParameter> _Parameters = new List<ProjectionParameter>((pcs as ProjectedCoordinateSystem).Projection.NumParameters);
            for (int i = 0; i < (pcs as ProjectedCoordinateSystem).Projection.NumParameters; i++)
            {
                _Parameters.Add((pcs as ProjectedCoordinateSystem).Projection.GetParameter(i));
            }

            _Parameters.Add(new ProjectionParameter("semi_major", (pcs as ProjectedCoordinateSystem).HorizontalDatum.Ellipsoid.SemiMajorAxis));
            _Parameters.Add(new ProjectionParameter("semi_minor", (pcs as ProjectedCoordinateSystem).HorizontalDatum.Ellipsoid.SemiMinorAxis));
            _Parameters.Add(new ProjectionParameter("unit", (pcs as ProjectedCoordinateSystem).LinearUnit.MetersPerUnit));
            return _Parameters;
        }       


        private void btnCancel_Click(object sender, EventArgs e)
        {
            //Clear, reset fields and go to home page
            GNSS_Functions.ClearCreateNewProject();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            MainScreen.recentStartScreen.BringToFront();

            //Show Settings button
            MainScreen.btnSettings.Show();
        }

        private void OpenToolStripButton_Click(object sender, EventArgs e)
        {
            if (cbxType.SelectedIndex > -1)
            {
                //Import points
                GNSS_Functions.ImportPoints(this, "EXCEL_CSV_TXT");
            }
            
        }

        private void pnlContainer_SizeChanged(object sender, EventArgs e)
        {
            pnlInput.Height = pnlContainer.Height / 2;
            pnlOutput.Height = pnlInput.Height;
        }

        private void dgvSourceGeog_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            try
            {
                dgvTargetProjected.Rows.RemoveAt(0);
            }
            catch (Exception ex)
            {

            }

        }

        private void dgvSourceProjected_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {

            if (cbxType.SelectedIndex == 1)
            {
                dgvTargetGeog.Rows.Clear();
            }
            else
            {
                dgvTargetProjected.Rows.Clear();
            }

        }

        private void dgvSourceGeog_RowsAdded(object sender, System.Windows.Forms.DataGridViewRowsAddedEventArgs e)
        {
            try
            {
                if (TargetRowAddition == true)
                {
                    dgvTargetProjected.Rows.Add();
                }
                else
                {
                    TargetRowAddition = true;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void dgvSourceGeog_CellEndEdit(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            try
            {
                double CellValue = 0;

                var r = dgvSourceGeog.CurrentRow.Index;
                var c = dgvSourceGeog.CurrentCell.ColumnIndex;
                var curtext = dgvSourceGeog.CurrentCell.Value;

                if (dgvSourceGeog.CurrentCell.ColumnIndex == 0)
                {


                }
                else
                {
                    if (Simulate.IsNumeric(curtext) == true || curtext.ToString() == "-000" || curtext.ToString() == "-00" || curtext.ToString() == "-0")
                    {

                        switch (dgvSourceGeog.CurrentCell.ColumnIndex)
                        {

                            case 1:
                                {
                                    if (dgvSourceGeog.CurrentCell.Value.ToString() == "-000" || dgvSourceGeog.CurrentCell.Value.ToString() == "-0000" || dgvSourceGeog.CurrentCell.Value.ToString() == "-00" || dgvSourceGeog.CurrentCell.Value.ToString() == "-0")
                                    {
                                        dgvSourceGeog.CurrentCell.Value = "-000";
                                    }
                                    else
                                    {
                                        CellValue = Simulate.Val(dgvSourceGeog.CurrentCell.Value);
                                        if (CellValue.ToString().Contains("."))
                                        {

                                            var dms = GNSS_Functions.DegDec2DMS(CellValue);
                                            dgvSourceGeog.CurrentCell.Value = dms[0].ToString("000");

                                            //min(latitude)
                                            dgvSourceGeog[c + 1, r].Value = dms[1].ToString("00");
                                            //sec(latitude)
                                            dgvSourceGeog[c + 2, r].Value = dms[2];

                                        }
                                        else
                                        {
                                            dgvSourceGeog.CurrentCell.Value = CellValue.ToString("000");

                                        }
                                    }

                                    break;
                                }
                            case 2:
                                {
                                    CellValue = Simulate.Val(dgvSourceGeog.CurrentCell.Value);
                                    dgvSourceGeog.CurrentCell.Value = CellValue.ToString("00");
                                    break;
                                }
                            case 3:
                                {
                                    CellValue = Simulate.Val(dgvSourceGeog.CurrentCell.Value);
                                    dgvSourceGeog.CurrentCell.Value = CellValue.ToString("00.00000");
                                    break;
                                }
                            case 4:
                                {
                                    if (dgvSourceGeog.CurrentCell.Value.ToString() == "-000" || dgvSourceGeog.CurrentCell.Value.ToString() == "-0000" || dgvSourceGeog.CurrentCell.Value.ToString() == "-00" || dgvSourceGeog.CurrentCell.Value.ToString() == "-0")
                                    {
                                        dgvSourceGeog.CurrentCell.Value = "-000";
                                    }
                                    else
                                    {
                                        CellValue = Simulate.Val(dgvSourceGeog.CurrentCell.Value);
                                        if (CellValue.ToString().Contains("."))
                                        {

                                            var dms = GNSS_Functions.DegDec2DMS(CellValue);
                                            dgvSourceGeog.CurrentCell.Value = dms[0].ToString("000");

                                            //min(latitude)
                                            dgvSourceGeog[c + 1, r].Value = dms[1].ToString("00");
                                            //sec(latitude)
                                            dgvSourceGeog[c + 2, r].Value = dms[2];

                                        }
                                        else
                                        {
                                            dgvSourceGeog.CurrentCell.Value = CellValue.ToString("000");

                                        }
                                    }
                                    break;
                                }
                            case 5:
                                {
                                    CellValue = Simulate.Val(dgvSourceGeog.CurrentCell.Value);
                                    dgvSourceGeog.CurrentCell.Value = CellValue.ToString("00");
                                    break;
                                }
                            case 6:
                                {
                                    CellValue = Simulate.Val(dgvSourceGeog.CurrentCell.Value);
                                    dgvSourceGeog.CurrentCell.Value = CellValue.ToString("00.00000");
                                    break;
                                }
                            case 7:
                                {
                                    if (dgvSourceGeog.CurrentCell.Value == null)
                                    {
                                        dgvSourceGeog.CurrentCell.Value = "0.0000";
                                    }
                                    else
                                    {
                                        CellValue = Simulate.Val(dgvSourceGeog.CurrentCell.Value);
                                        dgvSourceGeog.CurrentCell.Value = CellValue.ToString("0.0000");
                                    }
                                   

                                    break;
                                }
                        }

                    }
                    else
                    {
                        MessageBox.Show("Only Numeric Entry Allowed ", "Check Current Cell", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.dgvSourceGeog.CurrentCell.Value = "";
                    }

                }

                //Count Number of rows
                lblPointCounts.Text = (dgvSourceGeog.RowCount - 1).ToString();
            }
            catch (Exception)
            {

            }            

        }

        private void dgvSourceProjected_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double CellValue = 0;

                var curText = dgvSourceProjected.CurrentCell.Value;


                if (dgvSourceProjected.CurrentCell.ColumnIndex == 0)
                {

                }
                else
                {
                    if (Simulate.IsNumeric(curText) == true || curText.ToString() == "-0" || curText.ToString() == "-00" || curText.ToString() == "-000")
                    {

                        switch (dgvSourceProjected.CurrentCell.ColumnIndex)
                        {

                            case 1:

                                CellValue = Simulate.Val(this.dgvSourceProjected.CurrentCell.Value);
                                this.dgvSourceProjected.CurrentCell.Value = CellValue.ToString("0.0000");
                                break;
                            case 2:
                                CellValue = Simulate.Val(this.dgvSourceProjected.CurrentCell.Value);
                                this.dgvSourceProjected.CurrentCell.Value = CellValue.ToString("0.0000");
                                break;
                            case 3:
                                if (this.dgvSourceProjected.CurrentCell.Value == null)
                                {
                                    this.dgvSourceProjected.CurrentCell.Value = "0.0000";
                                }
                                else
                                {
                                    CellValue = Simulate.Val(this.dgvSourceProjected.CurrentCell.Value);
                                    this.dgvSourceProjected.CurrentCell.Value = CellValue.ToString("0.0000");
                                }


                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Only Numeric Entry Allowed ", "Check Current Cell", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.dgvSourceProjected.CurrentCell.Value = "";

                    }
                }

                //Count Number of rows
                lblPointCounts.Text = (dgvSourceProjected.RowCount - 1).ToString();
            }
            catch (Exception)
            {

            }

        }

        private void dgvSourceProjected_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            try
            {             

                //count number of rows
                lblPointCounts.Text = (dgvTargetGeog.Visible == true) ? dgvTargetGeog.RowCount.ToString() : dgvTargetProjected.RowCount.ToString();


            }
            catch (Exception ex)
            {

            }
        }

        private void NewToolStripButton_Click(object sender, EventArgs e)
        {
            //Clear the dataGridviews
            this.dgvSourceGeog.Rows.Clear();
            this.dgvTargetGeog.Rows.Clear();
            this.dgvSourceProjected.Rows.Clear();
            this.dgvTargetProjected.Rows.Clear();

            lblPointCounts.Text = 0.ToString();
        }

        /// <summary>
        /// Rename Column Header
        /// </summary>
        /// <param name="gdv"></param>
        /// <param name="IsCartesian"></param>
        void dgvRenameHeader(DataGridView gdv, bool IsCartesian)
        {
            //Rename Headers
            gdv.Columns[1].HeaderText = (IsCartesian) ? "  X" : "EASTING";
            gdv.Columns[2].HeaderText = (IsCartesian) ? "  Y" : "NORTHING";
            gdv.Columns[3].HeaderText = (IsCartesian) ? "  Z" : "HEIGHT";

            if (gdv.Name == nameof(dgvTargetProjected))
            {
                gdv.Columns[4].Visible = (IsCartesian) ? false : true;
                gdv.Columns[5].Visible = (IsCartesian) ? false : true;
                gdv.Columns[6].Visible = (IsCartesian) ? false : true;
            }
           
        }

        /// <summary>
        /// End Edit of the dgv
        /// </summary>
        void EndEdit()
        {
            //Ending all Editing before computations
            dgvSourceGeog.EndEdit();
            //dgvTargetGeog.EndEdit();
            //dgvTargetProjected.EndEdit();
            dgvSourceProjected.EndEdit();
        }

        DataGridView InputDGV = new DataGridView();
        DataGridView OutputDGV = new DataGridView();

        private void cbxFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxFormat.SelectedItem.ToString() == "Projection")
            {
                
                if (cbxType.SelectedIndex == 0) //Cartesian To Geographic
                {
                    dgvRenameHeader(dgvSourceProjected, true);
                    dgvSourceProjected.Visible = true;
                    dgvTargetGeog.Visible = true;

                    InputDGV = dgvSourceProjected;
                    OutputDGV = dgvTargetGeog;

                    dgvSourceGeog.Visible = false;
                    dgvTargetProjected.Visible = false;

                    //Show or hide labels
                    lblInputLat.Visible = false;
                    lblInputLon.Visible = false;
                    lblOutputLat.Visible = true;
                    lblOutputLon.Visible = true;

                    //Show convert to meters
                    pnlConvrtToMeters.Visible = false;

                    //Set to combo box selected value
                    FormatSelect = 0;

                }
                else if (cbxType.SelectedIndex == 1) //Cartesian To Projected
                {
                    dgvRenameHeader(dgvSourceProjected, true);
                    dgvRenameHeader(dgvTargetProjected, false);
                    dgvSourceProjected.Visible = true;
                    dgvTargetProjected.Visible = true;

                    InputDGV = dgvSourceProjected;
                    OutputDGV = dgvTargetProjected;

                    dgvTargetGeog.Visible = false;
                    dgvSourceGeog.Visible = false;


                    //Show or hide labels
                    lblInputLat.Visible = false;
                    lblInputLon.Visible = false;
                    lblOutputLat.Visible = false;
                    lblOutputLon.Visible = false;

                    //Show convert to meters
                    pnlConvrtToMeters.Visible = true;

                    //Set to combo box selected value
                    FormatSelect = 1;
                }
                else if (cbxType.SelectedIndex == 2) //Geographic To Cartesian
                {
                    dgvRenameHeader(dgvTargetProjected, true);
                    dgvSourceGeog.Visible = true;
                    dgvTargetProjected.Visible = true;

                    InputDGV = dgvSourceGeog;
                    OutputDGV = dgvTargetProjected;

                    dgvSourceProjected.Visible = false;
                    dgvTargetGeog.Visible = false;

                    //Show or hide labels
                    lblInputLat.Visible = true;
                    lblInputLon.Visible = true;
                    lblOutputLat.Visible = false;
                    lblOutputLon.Visible = false;

                    //Show convert to meters
                    pnlConvrtToMeters.Visible = false;

                    //Set to combo box selected value
                    FormatSelect = 2;

                }                
                else if (cbxType.SelectedIndex == 3) //Geographic To Projected
                {

                    dgvRenameHeader(dgvTargetProjected, false);
                    dgvSourceGeog.Visible = true;
                    dgvTargetProjected.Visible = true;

                    InputDGV = dgvSourceGeog;
                    OutputDGV = dgvTargetProjected;

                    dgvSourceProjected.Visible = false;
                    dgvTargetGeog.Visible = false;

                    //Show or hide labels
                    lblInputLat.Visible = true;
                    lblInputLon.Visible = true;
                    lblOutputLat.Visible = false;
                    lblOutputLon.Visible = false;

                    //Show convert to meters
                    pnlConvrtToMeters.Visible = true;

                    //Set to combo box selected value
                    FormatSelect = 3;

                }
                else if (cbxType.SelectedIndex == 4) //Projected To Cartesian
                {
                    dgvRenameHeader(dgvSourceProjected, false);
                    dgvRenameHeader(dgvTargetProjected, true);
                    dgvSourceProjected.Visible = true;
                    dgvTargetGeog.Visible = false;

                    dgvSourceGeog.Visible = false;
                    dgvTargetProjected.Visible = true;

                    InputDGV = dgvSourceProjected;
                    OutputDGV = dgvTargetProjected;

                    //Show or hide labels
                    lblInputLat.Visible = false;
                    lblInputLon.Visible = false;
                    lblOutputLat.Visible = false;
                    lblOutputLon.Visible = false;

                    //Show convert to meters
                    pnlConvrtToMeters.Visible = false;

                    //Set to combo box selected value
                    FormatSelect = 4;
                }
                else if (cbxType.SelectedIndex == 5) //Projected To Geograhic
                {
                    dgvRenameHeader(dgvSourceProjected, false);
                    dgvSourceProjected.Visible = true;
                    dgvTargetGeog.Visible = true;

                    InputDGV = dgvSourceProjected;
                    OutputDGV = dgvTargetGeog;

                    dgvSourceGeog.Visible = false;
                    dgvTargetProjected.Visible = false;

                    //Show or hide labels
                    lblInputLat.Visible = false;
                    lblInputLon.Visible = false;
                    lblOutputLat.Visible = true;
                    lblOutputLon.Visible = true;

                    //Show convert to meters
                    pnlConvrtToMeters.Visible = false;

                    //Set to combo box selected value
                    FormatSelect = 5;

                }
            }
            else
            {
                if (cbxType.SelectedIndex == 0) //Cartesian To Cartesian
                {
                    dgvRenameHeader(dgvSourceProjected, true);
                    dgvRenameHeader(dgvTargetProjected, true);
                    dgvSourceProjected.Visible = true;
                    dgvTargetProjected.Visible = true;

                    InputDGV = dgvSourceProjected;
                    OutputDGV = dgvTargetProjected;

                    dgvTargetGeog.Visible = false;
                    dgvSourceGeog.Visible = false;

                    //Show or hide labels
                    lblInputLat.Visible = false;
                    lblInputLon.Visible = false;
                    lblOutputLat.Visible = false;
                    lblOutputLon.Visible = false;

                    //Show convert to meters
                    pnlConvrtToMeters.Visible = false;

                    //Set to combo box selected value
                    FormatSelect = 0;
                }
                else if(cbxType.SelectedIndex == 1) //Cartesian To Geographic
                {
                    dgvRenameHeader(dgvSourceProjected, true);
                    dgvSourceProjected.Visible = true;
                    dgvTargetGeog.Visible = true;

                    InputDGV = dgvSourceProjected;
                    OutputDGV = dgvTargetGeog;

                    dgvSourceGeog.Visible = false;
                    dgvTargetProjected.Visible = false;

                    //Show or hide labels
                    lblInputLat.Visible = false;
                    lblInputLon.Visible = false;
                    lblOutputLat.Visible = true;
                    lblOutputLon.Visible = true;

                    //Show convert to meters
                    pnlConvrtToMeters.Visible = false;

                    //Set to combo box selected value
                    FormatSelect = 1;

                }
                else if (cbxType.SelectedIndex == 2) //Cartesian To Projected
                {
                    dgvRenameHeader(dgvSourceProjected, true);
                    dgvRenameHeader(dgvTargetProjected, false);
                    dgvSourceProjected.Visible = true;
                    dgvTargetProjected.Visible = true;

                    InputDGV = dgvSourceProjected;
                    OutputDGV = dgvTargetProjected;

                    dgvTargetGeog.Visible = false;
                    dgvSourceGeog.Visible = false;


                    //Show or hide labels
                    lblInputLat.Visible = false;
                    lblInputLon.Visible = false;
                    lblOutputLat.Visible = false;
                    lblOutputLon.Visible = false;

                    //Show convert to meters
                    pnlConvrtToMeters.Visible = true;

                    //Set to combo box selected value
                    FormatSelect = 2;
                }
                else if (cbxType.SelectedIndex == 3) //Geographic To Cartesian
                {
                    dgvRenameHeader(dgvTargetProjected, true);
                    dgvSourceGeog.Visible = true;
                    dgvTargetProjected.Visible = true;

                    InputDGV = dgvSourceGeog;
                    OutputDGV = dgvTargetProjected;

                    dgvSourceProjected.Visible = false;
                    dgvTargetGeog.Visible = false;

                    //Show or hide labels
                    lblInputLat.Visible = true;
                    lblInputLon.Visible = true;
                    lblOutputLat.Visible = false;
                    lblOutputLon.Visible = false;

                    //Show convert to meters
                    pnlConvrtToMeters.Visible = false;

                    //Set to combo box selected value
                    FormatSelect = 3;

                }
                else if (cbxType.SelectedIndex == 4) //Geographic To Geographic
                {

                    dgvTargetGeog.Visible = true;
                    dgvSourceGeog.Visible = true;

                    InputDGV = dgvSourceGeog;
                    OutputDGV = dgvTargetGeog;

                    dgvSourceProjected.Visible = false;
                    dgvTargetProjected.Visible = false;

                    //Show or hide labels
                    lblInputLat.Visible = true;
                    lblInputLon.Visible = true;
                    lblOutputLat.Visible = true;
                    lblOutputLon.Visible = true;

                    //Show convert to meters
                    pnlConvrtToMeters.Visible = false;



                    //Set to combo box selected value
                    FormatSelect = 4;
                }
                else if (cbxType.SelectedIndex == 5) //Geographic To Projected
                {

                    dgvRenameHeader(dgvTargetProjected, false);
                    dgvSourceGeog.Visible = true;
                    dgvTargetProjected.Visible = true;

                    InputDGV = dgvSourceGeog;
                    OutputDGV = dgvTargetProjected;

                    dgvSourceProjected.Visible = false;
                    dgvTargetGeog.Visible = false;

                    //Show or hide labels
                    lblInputLat.Visible = true;
                    lblInputLon.Visible = true;
                    lblOutputLat.Visible = false;
                    lblOutputLon.Visible = false;

                    //Show convert to meters
                    pnlConvrtToMeters.Visible = true;

                    //Set to combo box selected value
                    FormatSelect = 5;

                }
                else if (cbxType.SelectedIndex == 6) //Projected To Cartesian
                {
                    dgvRenameHeader(dgvSourceProjected, false);
                    dgvRenameHeader(dgvTargetProjected, true);
                    dgvSourceProjected.Visible = true;
                    dgvTargetGeog.Visible = false;

                    InputDGV = dgvSourceProjected;
                    OutputDGV = dgvTargetProjected;

                    dgvSourceGeog.Visible = false;
                    dgvTargetProjected.Visible = true;

                    //Show or hide labels
                    lblInputLat.Visible = false;
                    lblInputLon.Visible = false;
                    lblOutputLat.Visible = false;
                    lblOutputLon.Visible = false;

                    //Show convert to meters
                    pnlConvrtToMeters.Visible = false;



                    //Set to combo box selected value
                    FormatSelect = 6;
                }
                else if (cbxType.SelectedIndex == 7) //Projected To Geographic
                {
                    dgvRenameHeader(dgvSourceProjected, false);
                    dgvSourceProjected.Visible = true;
                    dgvTargetGeog.Visible = true;

                    InputDGV = dgvSourceProjected;
                    OutputDGV = dgvTargetGeog;

                    dgvSourceGeog.Visible = false;
                    dgvTargetProjected.Visible = false;

                    //Show or hide labels
                    lblInputLat.Visible = false;
                    lblInputLon.Visible = false;
                    lblOutputLat.Visible = true;
                    lblOutputLon.Visible = true;

                    //Show convert to meters
                    pnlConvrtToMeters.Visible = false;

                    //Set to combo box selected value
                    FormatSelect = 7;

                }
                else if (cbxType.SelectedIndex == 8) //Projected To Projected
                {
                    dgvRenameHeader(dgvSourceProjected, false);
                    dgvRenameHeader(dgvTargetProjected, false);
                    dgvSourceProjected.Visible = true;
                    dgvTargetGeog.Visible = false;

                    InputDGV = dgvSourceProjected;
                    OutputDGV = dgvTargetProjected;

                    dgvSourceGeog.Visible = false;
                    dgvTargetProjected.Visible = true;

                    //Show or hide labels
                    lblInputLat.Visible = false;
                    lblInputLon.Visible = false;
                    lblOutputLat.Visible = false;
                    lblOutputLon.Visible = false;

                    //Show convert to meters
                    pnlConvrtToMeters.Visible = true;

                    //Set to combo box selected value
                    FormatSelect = 8;
                }
            }

            OutputDGV.Rows.Clear();            
            
        }

        //Getting Default or built in Grid system for WGS 84
        private GridSystem SourceCRS = new GridSystem().WGS84();

        //Create new coordinate system from built functions
        private GridSystem TargetCRS = new GridSystem().GhanaNationalGrid(new TransParams().GH7TransParams());


        ICoordinateSystem Source_CRS;
        ICoordinateSystem Target_CRS;
        ICoordinateTransformation trans;
        CoordinateSystemConversion cor = new CoordinateSystemConversion();
        private void btnCompute_Click(object sender, EventArgs e)
        {
            EndEdit();

            //--------------------------------------- Validating Process --------------------------------------------
            if (cbxType.SelectedIndex == -1)
            {
                //Show an error message
                MessageBox.Show("Please select an input format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //Terminate the process
                return;
            }

            if (tbxFromCRS.Text == "")
            {
                //Show an error message
                MessageBox.Show("Please select an input Grid or Datum", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //Terminate the process
                return;
            }

            //================TEST Coordinate System=======================
           
                                 

            bool ToWGS84 = false;

            OutputDGV.Rows.Clear();
            
            for (var index = 0; index <= InputDGV.Rows.Count - 2; index++)
            {
                OutputDGV.Rows.Add();               
            }

            string[] ENU = new string[] { };            

            var IGeoncenSource = cFac.CreateGeocentricCoordinateSystem(" ", (Source_CRS as IProjectedCoordinateSystem).HorizontalDatum, (Source_CRS as IProjectedCoordinateSystem).LinearUnit, (Source_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem.PrimeMeridian);
              
            try
            {
                if (cbxFormat.SelectedItem.ToString() == "Projection")
                {

                    Target_CRS = Source_CRS;
                    //var _SourceParams = getParame(Source_CRS.WKT);
                    
                    //pcs.Name;
                    //pcs.GeographicCoordinateSystem.Name;
                    //pcs.GeographicCoordinateSystem.HorizontalDatum.Name;
                    //pcs.GeographicCoordinateSystem.HorizontalDatum.Ellipsoid.Name;
                    //pcs.GeographicCoordinateSystem.HorizontalDatum.Ellipsoid.SemiMajorAxis;
                    //pcs.GeographicCoordinateSystem.HorizontalDatum.Ellipsoid.InverseFlattening;
                    //pcs.GeographicCoordinateSystem.HorizontalDatum.Ellipsoid.Authority;
                    //pcs.GeographicCoordinateSystem.HorizontalDatum.Ellipsoid.AuthorityCode;
                    //pcs.GeographicCoordinateSystem.HorizontalDatum.Authority;
                    //pcs.GeographicCoordinateSystem.HorizontalDatum.AuthorityCode;
                    //pcs.GeographicCoordinateSystem.HorizontalDatum.Wgs84Parameters;
                    //pcs.GeographicCoordinateSystem.PrimeMeridian.Name;
                    //pcs.GeographicCoordinateSystem.PrimeMeridian.Longitude;
                    //pcs.GeographicCoordinateSystem.PrimeMeridian.Authority;
                    //pcs.GeographicCoordinateSystem.PrimeMeridian.AuthorityCode;
                    //pcs.GeographicCoordinateSystem.AngularUnit.Name;
                    //pcs.GeographicCoordinateSystem.AngularUnit.RadiansPerUnit;
                    //pcs.GeographicCoordinateSystem.AngularUnit.Authority;
                    //pcs.GeographicCoordinateSystem.AngularUnit.AuthorityCode;
                    //pcs.GeographicCoordinateSystem.Authority;
                    //pcs.GeographicCoordinateSystem.AuthorityCode;

                    //pcs.Projection.ClassName;
                    //pcs.Projection.GetParameter("latitude_of_origin");            
                    //pcs.Projection.GetParameter("central_meridian");            
                    //pcs.Projection.GetParameter("standard_parallel_1");            
                    //pcs.Projection.GetParameter("standard_parallel_2");            
                    //pcs.Projection.GetParameter("false_easting");            
                    //pcs.Projection.GetParameter("false_northing");            
                    //pcs.LinearUnit.Name;
                    //pcs.LinearUnit.MetersPerUnit;
                    //pcs.LinearUnit.Authority;
                    //pcs.LinearUnit.AuthorityCode;
                    //pcs.Authority;
                    //pcs.AuthorityCode;
                    //pcs.WKT;

                    if (cbxType.SelectedIndex == 0) //Cartesian To Geographic
                    {
                        trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems(IGeoncenSource, (Source_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem);

                        for (int i = 0; i < dgvSourceProjected.RowCount - 1; i++)
                        {
                            //var LonLatH = cor.XYZ2LonLatH(SourceCRS, new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) });
                           
                            var LonLatH = trans.MathTransform.Transform(new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) });

                            dgvTargetGeog[0, i].Value = dgvSourceProjected[0, i].Value;

                            var Lat = cor.DegDec2DMS(LonLatH[1]); var Lon = cor.DegDec2DMS(LonLatH[0]);
                            dgvTargetGeog[1, i].Value = (LonLatH[1] < 0 && LonLatH[1] > -1) ? "-000" : Lat[0].ToString("000");
                            dgvTargetGeog[2, i].Value = Lat[1].ToString("00");
                            dgvTargetGeog[3, i].Value = Lat[2].ToString("00.00000");

                            dgvTargetGeog[4, i].Value = (LonLatH[0] < 0 && LonLatH[0] > -1) ? "-000" : Lon[0].ToString("000");
                            dgvTargetGeog[5, i].Value = Lon[1].ToString("00");
                            dgvTargetGeog[6, i].Value = Lon[2].ToString("00.00000");

                            dgvTargetGeog[7, i].Value = (LonLatH[2] * (Source_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit).ToString("00.0000");
                            
                        }

                    }
                    else if (cbxType.SelectedIndex == 1) //Cartesian To Projected
                    {

                        for (int i = 0; i < dgvSourceProjected.RowCount - 1; i++)
                        {

                            trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems(IGeoncenSource, (Source_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem);
                            var LonLatH = trans.MathTransform.Transform(new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) });
                            trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((Source_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem, (Source_CRS as IProjectedCoordinateSystem));
                            var _ENU = trans.MathTransform.Transform(LonLatH);

                            dgvTargetProjected[0, i].Value = dgvSourceProjected[0, i].Value;

                            dgvTargetProjected[1, i].Value = (!chbxConvertToMeters.Checked) ? (_ENU[0] * (Source_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit).ToString("00.0000") : _ENU[0].ToString("00.0000");
                            dgvTargetProjected[2, i].Value = (!chbxConvertToMeters.Checked) ? (_ENU[1] * (Source_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit).ToString("00.0000") : _ENU[1].ToString("00.0000");
                            var oriHgt = _ENU[2] / (Source_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit;
                            dgvTargetProjected[3, i].Value = (!chbxConvertToMeters.Checked) ? (oriHgt * (Source_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit).ToString("00.0000") : oriHgt.ToString("00.0000");


                            dgvTargetProjected[4, i].Value = GNSS_Functions.GetZone;
                            dgvTargetProjected[5, i].Value = GNSS_Functions.GetGridScale;
                            dgvTargetProjected[6, i].Value = GNSS_Functions.GetConvergence;

                        }

                    }
                    else if (cbxType.SelectedIndex == 2) //Geographic To Cartesian
                    {
                        trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((Source_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem, IGeoncenSource);

                        for (int i = 0; i < dgvSourceGeog.RowCount - 1; i++)
                        {
                            var Lat = cor.DMS2DecDeg(dgvSourceGeog[1, i].Value.ToString(), double.Parse(dgvSourceGeog[2, i].Value.ToString()), double.Parse(dgvSourceGeog[3, i].Value.ToString()));
                            var Lon = cor.DMS2DecDeg(dgvSourceGeog[4, i].Value.ToString(), double.Parse(dgvSourceGeog[5, i].Value.ToString()), double.Parse(dgvSourceGeog[6, i].Value.ToString()));
                            var hgt = (dgvSourceGeog[7, i].Value == null) ? 0 : double.Parse(dgvSourceGeog[7, i].Value.ToString());
                                                     
                            var XYZ = trans.MathTransform.Transform(new double[] { Lon, Lat, double.Parse(dgvSourceGeog[7, i].Value.ToString()) });
                            
                            dgvTargetProjected[0, i].Value = dgvSourceGeog[0, i].Value;

                            dgvTargetProjected[1, i].Value = XYZ[0].ToString("00.0000");
                            dgvTargetProjected[2, i].Value = XYZ[1].ToString("00.0000");
                            dgvTargetProjected[3, i].Value = XYZ[2].ToString("00.0000");

                        }

                    }
                    else if (cbxType.SelectedIndex == 3) //Geographic To Projected
                    {
                        trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((Source_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem, (Source_CRS as IProjectedCoordinateSystem));

                        for (int i = 0; i < dgvSourceGeog.RowCount - 1; i++)
                        {
                            var Lat = cor.DMS2DecDeg(dgvSourceGeog[1, i].Value.ToString(), double.Parse(dgvSourceGeog[2, i].Value.ToString()), double.Parse(dgvSourceGeog[3, i].Value.ToString()));
                            var Lon = cor.DMS2DecDeg(dgvSourceGeog[4, i].Value.ToString(), double.Parse(dgvSourceGeog[5, i].Value.ToString()), double.Parse(dgvSourceGeog[6, i].Value.ToString()));

                            //ENU = cor.LonLatH2ENU(SourceCRS, new double[] { Lat, Lon, double.Parse(dgvSourceGeog[7, i].Value.ToString()) });
                           
                            var _ENU = trans.MathTransform.Transform(new double[] { Lon, Lat, double.Parse(dgvSourceGeog[7, i].Value.ToString()) });

                            dgvTargetProjected[0, i].Value = dgvSourceGeog[0, i].Value;

                            dgvTargetProjected[1, i].Value = (!chbxConvertToMeters.Checked) ? (_ENU[0] * (Source_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit).ToString("00.0000") : _ENU[0].ToString("00.0000");
                            dgvTargetProjected[2, i].Value = (!chbxConvertToMeters.Checked) ? (_ENU[1] * (Source_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit).ToString("00.0000") : _ENU[1].ToString("00.0000");
                            var oriHgt = _ENU[2] / (Source_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit;
                            dgvTargetProjected[3, i].Value = (!chbxConvertToMeters.Checked) ? (oriHgt * (Source_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit).ToString("00.0000") : oriHgt.ToString("00.0000");

                            dgvTargetProjected[4, i].Value = GNSS_Functions.GetZone;
                            dgvTargetProjected[5, i].Value = GNSS_Functions.GetGridScale;
                            dgvTargetProjected[6, i].Value = GNSS_Functions.GetConvergence;
                        }

                    }
                    else if (cbxType.SelectedIndex == 4) //Projected To Cartesian
                    {
                        for (int i = 0; i < dgvSourceProjected.RowCount - 1; i++)
                        {

                            trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((Source_CRS as IProjectedCoordinateSystem), (Source_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem);
                            var LonLatH = trans.MathTransform.Transform(new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) });
                            trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((Source_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem, IGeoncenSource);
                            var XYZ = trans.MathTransform.Transform(LonLatH);

                            dgvTargetProjected[0, i].Value = dgvSourceProjected[0, i].Value;

                            dgvTargetProjected[1, i].Value = XYZ[0].ToString("00.0000");
                            dgvTargetProjected[2, i].Value = XYZ[1].ToString("00.0000");
                            dgvTargetProjected[3, i].Value = XYZ[2].ToString("00.0000");

                        }

                    }
                    else if (FormatSelect == 5) //Projected To Geographic
                    {
                        //var LatLon = cor.ProjectToLatLon(new double[] { double.Parse(ENU[0]), double.Parse(ENU[1]) }, new double[] { double.Parse(ENU[2]) }, 2136, 4168);
                        trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((Source_CRS as IProjectedCoordinateSystem), (Source_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem);

                        for (int i = 0; i < dgvSourceProjected.RowCount - 1; i++)
                        {

                            var LonLatH = trans.MathTransform.Transform(new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) });

                            dgvTargetGeog[0, i].Value = dgvSourceProjected[0, i].Value;

                            var Lat = cor.DegDec2DMS(LonLatH[1]); var Lon = cor.DegDec2DMS(LonLatH[0]);
                            dgvTargetGeog[1, i].Value = (LonLatH[1] < 0 && LonLatH[1] > -1) ? "-000" : Lat[0].ToString("000");
                            dgvTargetGeog[2, i].Value = Lat[1].ToString("00");
                            dgvTargetGeog[3, i].Value = Lat[2].ToString("00.00000");

                            dgvTargetGeog[4, i].Value = (LonLatH[0] < 0 && LonLatH[0] > -1) ? "-000" : Lon[0].ToString("000");
                            dgvTargetGeog[5, i].Value = Lon[1].ToString("00");
                            dgvTargetGeog[6, i].Value = Lon[2].ToString("00.00000");

                            dgvTargetGeog[7, i].Value = (LonLatH[2] * (Source_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit).ToString("00.0000");
                           
                        }

                    }

                }
                else
                {
                    var IGeoncenTarget = cFac.CreateGeocentricCoordinateSystem(" ", (Target_CRS as IProjectedCoordinateSystem).HorizontalDatum, (Target_CRS as IProjectedCoordinateSystem).LinearUnit, (Target_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem.PrimeMeridian);
                    ICoordinateTransformation transCART = new CoordinateTransformationFactory().CreateFromCoordinateSystems(IGeoncenSource, IGeoncenTarget);
                    ICoordinateTransformation ProjectTargetCARTGEO = new CoordinateTransformationFactory().CreateFromCoordinateSystems(IGeoncenTarget, (Target_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem);
                    ICoordinateTransformation ProjectTargetGEOPRO = new CoordinateTransformationFactory().CreateFromCoordinateSystems((Target_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem, (Target_CRS as IProjectedCoordinateSystem));

                    if (cbxType.SelectedIndex == 0) //Cartesian To Cartesian
                    {
                        trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems(IGeoncenSource, IGeoncenTarget);

                        for (int i = 0; i < dgvSourceProjected.RowCount - 1; i++)
                        {
                            if (tbxToCRS.Text != "" && tbxToCRS.Text != tbxFromCRS.Text)
                            {
                                //var X1Y1Z1 = cor.XYZTransformation(TargetCRS.TransParam, new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) }, ToWGS84);

                                var X1Y1Z1 = trans.MathTransform.Transform(new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) });

                                dgvTargetProjected[0, i].Value = dgvSourceProjected[0, i].Value;
                                dgvTargetProjected[1, i].Value = X1Y1Z1[0].ToString("00.0000");
                                dgvTargetProjected[2, i].Value = X1Y1Z1[1].ToString("00.0000");
                                dgvTargetProjected[3, i].Value = X1Y1Z1[2].ToString("00.0000");
                            }
                            else
                            {
                                dgvTargetProjected[0, i].Value = dgvSourceProjected[0, i].Value;
                                dgvTargetProjected[1, i].Value = dgvSourceProjected[1, i].Value;
                                dgvTargetProjected[2, i].Value = dgvSourceProjected[2, i].Value;
                                dgvTargetProjected[3, i].Value = dgvSourceProjected[3, i].Value;
                            }

                        }

                    }
                    else if (cbxType.SelectedIndex == 1) //Cartesian To Geographic
                    {
                        
                        for (int i = 0; i < dgvSourceProjected.RowCount - 1; i++)
                        {
                            //var LonLatH = new double[] { };

                            //if (tbxToCRS.Text != "" && tbxToCRS.Text != tbxFromCRS.Text)
                            //{
                            //    //Convert to the new XYZ
                            //    var XYZ = cor.XYZTransformation(TargetCRS.TransParam, new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) }, true);

                            //    LonLatH = cor.XYZ2LonLatH(TargetCRS, XYZ);
                            //}
                            //else
                            //{

                            //    LonLatH = cor.XYZ2LonLatH(SourceCRS, new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) });

                            //}

                            var X1Y1Z1 = transCART.MathTransform.Transform(new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) });                           
                            var LonLatH = ProjectTargetCARTGEO.MathTransform.Transform(X1Y1Z1);

                            dgvTargetGeog[0, i].Value = dgvSourceProjected[0, i].Value;

                            var Lat = cor.DegDec2DMS(LonLatH[1]); var Lon = cor.DegDec2DMS(LonLatH[0]);
                            dgvTargetGeog[1, i].Value = (LonLatH[1] < 0 && LonLatH[1] > -1) ? "-000" : Lat[0].ToString("000");
                            dgvTargetGeog[2, i].Value = Lat[1].ToString("00");
                            dgvTargetGeog[3, i].Value = Lat[2].ToString("00.00000");

                            dgvTargetGeog[4, i].Value = (LonLatH[0] < 0 && LonLatH[0] > -1) ? "-000" : Lon[0].ToString("000");
                            dgvTargetGeog[5, i].Value = Lon[1].ToString("00");
                            dgvTargetGeog[6, i].Value = Lon[2].ToString("00.00000");

                            dgvTargetGeog[7, i].Value = LonLatH[2].ToString("00.0000");
                        }

                    }
                    else if (cbxType.SelectedIndex == 2) //Cartesian To Projected
                    {
                        
                        for (int i = 0; i < dgvSourceProjected.RowCount - 1; i++)
                        {

                            //var LonLatH = new double[] { };

                            //if (tbxToCRS.Text != "" && tbxToCRS.Text != tbxFromCRS.Text)
                            //{
                            //    //Convert to the new XYZ
                            //    var XYZ = cor.XYZTransformation(TargetCRS.TransParam, new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) }, true);

                            //    LonLatH = cor.XYZ2LonLatH(TargetCRS, XYZ);

                            //    ENU = cor.LonLatH2ENU(SourceCRS, LonLatH);
                            //}
                            //else
                            //{
                            //    LonLatH = cor.XYZ2LonLatH(SourceCRS, new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) });

                            //    ENU = cor.LonLatH2ENU(SourceCRS, LonLatH);
                            //}
                            var X1Y1Z1 = transCART.MathTransform.Transform(new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) });
                                                        
                            var LonLatH = ProjectTargetCARTGEO.MathTransform.Transform(X1Y1Z1);                            
                            var _ENU = ProjectTargetGEOPRO.MathTransform.Transform(LonLatH);

                            dgvTargetProjected[0, i].Value = dgvSourceProjected[0, i].Value;

                            dgvTargetProjected[1, i].Value = (!chbxConvertToMeters.Checked) ? (_ENU[0] * (Target_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit).ToString("00.0000") : _ENU[0].ToString("00.0000");
                            dgvTargetProjected[2, i].Value = (!chbxConvertToMeters.Checked) ? (_ENU[1] * (Target_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit).ToString("00.0000") : _ENU[1].ToString("00.0000");
                            var oriHgt = _ENU[2] / (Target_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit;
                            dgvTargetProjected[3, i].Value = (!chbxConvertToMeters.Checked) ? (oriHgt * (Target_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit).ToString("00.0000") : oriHgt.ToString("00.0000");

                            //dgvTargetProjected[4, i].Value = ENU[5] + " " + ENU[6];
                            //dgvTargetProjected[5, i].Value = ENU[4];
                            //dgvTargetProjected[6, i].Value = cor.ConvertToDMSString(double.Parse(ENU[3]), false, false);
                            dgvTargetProjected[4, i].Value = GNSS_Functions.GetZone;
                            dgvTargetProjected[5, i].Value = GNSS_Functions.GetGridScale;
                            dgvTargetProjected[6, i].Value = GNSS_Functions.GetConvergence;
                        }

                    }
                    else if (cbxType.SelectedIndex == 3) //Geographic To Cartesian
                    {
                        trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((Source_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem, IGeoncenTarget);

                        for (int i = 0; i < dgvSourceGeog.RowCount - 1; i++)
                        {
                            var Lat = cor.DMS2DecDeg(dgvSourceGeog[1, i].Value.ToString(), double.Parse(dgvSourceGeog[2, i].Value.ToString()), double.Parse(dgvSourceGeog[3, i].Value.ToString()));
                            var Lon = cor.DMS2DecDeg(dgvSourceGeog[4, i].Value.ToString(), double.Parse(dgvSourceGeog[5, i].Value.ToString()), double.Parse(dgvSourceGeog[6, i].Value.ToString()));

                            //if (tbxToCRS.Text != "" && tbxToCRS.Text != tbxFromCRS.Text)
                            //{

                            //    XYZ = cor.LonLatH2XYZ(SourceCRS, new double[] { Lat, Lon, double.Parse(dgvSourceGeog[7, i].Value.ToString()) });

                            //    //Convert to the new XYZ
                            //    XYZ = cor.XYZTransformation(TargetCRS.TransParam, XYZ, ToWGS84);

                            //}
                            //else
                            //{

                            //    XYZ = cor.LonLatH2XYZ(SourceCRS, new double[] { Lat, Lon, double.Parse(dgvSourceGeog[7, i].Value.ToString()) });

                            //}
                                                       
                            var XYZ = trans.MathTransform.Transform(new double[] { Lon, Lat, double.Parse(dgvSourceGeog[7, i].Value.ToString()) });
                            
                            dgvTargetProjected[0, i].Value = dgvSourceGeog[0, i].Value;

                            dgvTargetProjected[1, i].Value = XYZ[0].ToString("00.0000");
                            dgvTargetProjected[2, i].Value = XYZ[1].ToString("00.0000");
                            dgvTargetProjected[3, i].Value = XYZ[2].ToString("00.0000");
                        }

                    }
                    else if (cbxType.SelectedIndex == 4) //Geographic To Geographic
                    {
                        trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((Source_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem, (Target_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem);

                        for (int i = 0; i < dgvSourceGeog.RowCount - 1; i++)
                        {
                            if (tbxToCRS.Text != "" && tbxToCRS.Text != tbxFromCRS.Text)
                            {
                                var Lat = cor.DMS2DecDeg(dgvSourceGeog[1, i].Value.ToString(), double.Parse(dgvSourceGeog[2, i].Value.ToString()), double.Parse(dgvSourceGeog[3, i].Value.ToString()));
                                var Lon = cor.DMS2DecDeg(dgvSourceGeog[4, i].Value.ToString(), double.Parse(dgvSourceGeog[5, i].Value.ToString()), double.Parse(dgvSourceGeog[6, i].Value.ToString()));

                                //var XYZ = cor.LonLatH2XYZ(SourceCRS, new double[] { Lat, Lon, double.Parse(dgvSourceGeog[7, i].Value.ToString()) });

                                //var X1Y1Z1 = cor.XYZTransformation(TargetCRS.TransParam, XYZ, ToWGS84);

                                //var LonLatH = cor.XYZ2LonLatH(TargetCRS, X1Y1Z1);

                               var LonLatH = trans.MathTransform.Transform(new double[] { Lon, Lat, double.Parse(dgvSourceGeog[7, i].Value.ToString()) });

                                dgvTargetGeog[0, i].Value = dgvSourceGeog[0, i].Value;

                                var Lat1 = cor.DegDec2DMS(LonLatH[1]); var Lon1 = cor.DegDec2DMS(LonLatH[0]);
                                dgvTargetGeog[1, i].Value = (LonLatH[1] < 0 && LonLatH[1] > -1) ? "-000" : Lat1[0].ToString("000");
                                dgvTargetGeog[2, i].Value = Lat1[1].ToString("00");
                                dgvTargetGeog[3, i].Value = Lat1[2].ToString("00.00000");

                                dgvTargetGeog[4, i].Value = (LonLatH[0] < 0 && LonLatH[0] > -1) ? "-000" : Lon1[0].ToString("000");
                                dgvTargetGeog[5, i].Value = Lon1[1].ToString("00");
                                dgvTargetGeog[6, i].Value = Lon1[2].ToString("00.00000");

                                dgvTargetGeog[7, i].Value = LonLatH[2].ToString("00.0000");
                            }
                            else
                            {
                                dgvTargetGeog[0, i].Value = dgvSourceGeog[0, i].Value;
                                dgvTargetGeog[1, i].Value = dgvSourceGeog[1, i].Value;
                                dgvTargetGeog[2, i].Value = dgvSourceGeog[2, i].Value;
                                dgvTargetGeog[3, i].Value = dgvSourceGeog[3, i].Value;
                                dgvTargetGeog[4, i].Value = dgvSourceGeog[4, i].Value;
                                dgvTargetGeog[5, i].Value = dgvSourceGeog[5, i].Value;
                                dgvTargetGeog[6, i].Value = dgvSourceGeog[6, i].Value;
                                dgvTargetGeog[7, i].Value = dgvSourceGeog[7, i].Value;
                            }

                        }

                    }
                    else if (cbxType.SelectedIndex == 5) //Geographic To Projected
                    {
                        trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((Source_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem, (Target_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem);

                        for (int i = 0; i < dgvSourceGeog.RowCount - 1; i++)
                        {
                            var Lat = cor.DMS2DecDeg(dgvSourceGeog[1, i].Value.ToString(), double.Parse(dgvSourceGeog[2, i].Value.ToString()), double.Parse(dgvSourceGeog[3, i].Value.ToString()));
                            var Lon = cor.DMS2DecDeg(dgvSourceGeog[4, i].Value.ToString(), double.Parse(dgvSourceGeog[5, i].Value.ToString()), double.Parse(dgvSourceGeog[6, i].Value.ToString()));

                            //if (tbxToCRS.Text != "" && tbxToCRS.Text != tbxFromCRS.Text)
                            //{

                            //    var XYZ = cor.LonLatH2XYZ(SourceCRS, new double[] { Lat, Lon, double.Parse(dgvSourceGeog[7, i].Value.ToString()) });

                            //    var X1Y1Z1 = cor.XYZTransformation(TargetCRS.TransParam, XYZ, false);

                            //    var LonLatH = cor.XYZ2LonLatH(TargetCRS, X1Y1Z1);

                            //    ENU = cor.LonLatH2ENU(TargetCRS, LonLatH);

                            //}
                            //else
                            //{
                            //    //Change it to the source CRS
                            //    ENU = cor.LonLatH2ENU(SourceCRS, new double[] { Lat, Lon, double.Parse(dgvSourceGeog[7, i].Value.ToString()) });
                            //}

                            var LonLatH = trans.MathTransform.Transform(new double[] { Lon, Lat, double.Parse(dgvSourceGeog[7, i].Value.ToString()) });
                            var _ENU = ProjectTargetGEOPRO.MathTransform.Transform(LonLatH);                            


                            dgvTargetProjected[0, i].Value = dgvSourceGeog[0, i].Value;

                            dgvTargetProjected[1, i].Value = (!chbxConvertToMeters.Checked) ? (_ENU[0] * (Target_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit).ToString("00.0000") : _ENU[0].ToString("00.0000");
                            dgvTargetProjected[2, i].Value = (!chbxConvertToMeters.Checked) ? (_ENU[1] * (Target_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit).ToString("00.0000") : _ENU[1].ToString("00.0000");
                            var oriHgt = _ENU[2] / (Target_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit;
                            dgvTargetProjected[3, i].Value = (!chbxConvertToMeters.Checked) ? (oriHgt * (Target_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit).ToString("00.0000") : oriHgt.ToString("00.0000");

                            //dgvTargetProjected[4, i].Value = ENU[5] + " " + ENU[6];
                            //dgvTargetProjected[5, i].Value = ENU[4];
                            //dgvTargetProjected[6, i].Value = cor.ConvertToDMSString(double.Parse(ENU[3]), false, false);
                            dgvTargetProjected[4, i].Value = GNSS_Functions.GetZone;
                            dgvTargetProjected[5, i].Value = GNSS_Functions.GetGridScale;
                            dgvTargetProjected[6, i].Value = GNSS_Functions.GetConvergence;
                        }

                    }
                    else if (cbxType.SelectedIndex == 6) //Projected To Cartesian
                    {
                        for (int i = 0; i < dgvSourceProjected.RowCount - 1; i++)
                        {


                            trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((Source_CRS as IProjectedCoordinateSystem), (Target_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem);
                            var LonLatH = trans.MathTransform.Transform(new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) });
                            trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((Target_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem, IGeoncenTarget);
                            var XYZ = trans.MathTransform.Transform(LonLatH);

                            dgvTargetProjected[0, i].Value = dgvSourceProjected[0, i].Value;

                            dgvTargetProjected[1, i].Value = XYZ[0].ToString("00.0000");
                            dgvTargetProjected[2, i].Value = XYZ[1].ToString("00.0000");
                            dgvTargetProjected[3, i].Value = XYZ[2].ToString("00.0000");

                        }

                    }
                    else if (cbxType.SelectedIndex == 7) //Projected To Geographic
                    {
                        //var LatLon = cor.ProjectToLatLon(new double[] { double.Parse(ENU[0]), double.Parse(ENU[1]) }, new double[] { double.Parse(ENU[2]) }, 2136, 4168);

                        //double[] pGeo = new double[] { 0.5, 50.5 };
                        //double[] pUtm = trans.MathTransform.Transform(pGeo);
                        ////double[] pGeo2 = trans.MathTransform.Inverse().Transform(pUtm);
                        //MessageBox.Show(pUtm[0].ToString() + " " + pUtm[1].ToString());
                        trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((Source_CRS as IProjectedCoordinateSystem), (Target_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem);

                        for (int i = 0; i < dgvSourceProjected.RowCount - 1; i++)
                        {
                            //var LonLatH = cor.ENU2LonLatH(new GridSystem().GhanaNationalGrid(new TransParams().GH7TransParams()), new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) });
                            //var LonLatH = CoordinateTransformationFactory.CreateCoordinateOperation((Source_CRS as ProjectedCoordinateSystem).Projection.ClassName, _SourceParams, new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) });
                                                        
                            var LonLatH = trans.MathTransform.Transform(new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) });

                            dgvTargetGeog[0, i].Value = dgvSourceProjected[0, i].Value;

                            var Lat = cor.DegDec2DMS(LonLatH[1]); var Lon = cor.DegDec2DMS(LonLatH[0]);
                            dgvTargetGeog[1, i].Value = (LonLatH[1] < 0 && LonLatH[1] > -1) ? "-000" : Lat[0].ToString("000");
                            dgvTargetGeog[2, i].Value = Lat[1].ToString("00");
                            dgvTargetGeog[3, i].Value = Lat[2].ToString("00.00000");

                            dgvTargetGeog[4, i].Value = (LonLatH[0] < 0 && LonLatH[0] > -1) ? "-000" : Lon[0].ToString("000");
                            dgvTargetGeog[5, i].Value = Lon[1].ToString("00");
                            dgvTargetGeog[6, i].Value = Lon[2].ToString("00.00000");

                            dgvTargetGeog[7, i].Value = LonLatH[2].ToString("00.0000");
                        }
                    }
                    else if (cbxType.SelectedIndex == 8) //Projected To Projected
                    {
                        trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((Source_CRS as IProjectedCoordinateSystem), (Target_CRS as IProjectedCoordinateSystem));

                        for (int i = 0; i < dgvSourceProjected.RowCount - 1; i++)
                        {

                            //var LatLon = cor.ENU2LonLatH(SourceCRS, new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) });

                            //var XYZ = cor.LonLatH2XYZ(SourceCRS, LatLon);
                            //var X1Y1Z1 = cor.XYZTransformation(TargetCRS.TransParam, XYZ, true);
                            //var LonLatH = cor.XYZ2LonLatH(TargetCRS, X1Y1Z1);
                            //ENU = cor.LonLatH2ENU(TargetCRS, LonLatH);    

                            var _ENU = trans.MathTransform.Transform(new double[] { double.Parse(dgvSourceProjected[1, i].Value.ToString()), double.Parse(dgvSourceProjected[2, i].Value.ToString()), double.Parse(dgvSourceProjected[3, i].Value.ToString()) });
                            
                            dgvTargetProjected[0, i].Value = dgvSourceProjected[0, i].Value;

                            dgvTargetProjected[1, i].Value = (!chbxConvertToMeters.Checked) ? (_ENU[0] * (Target_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit).ToString("00.0000") : _ENU[0].ToString("00.0000");
                            dgvTargetProjected[2, i].Value = (!chbxConvertToMeters.Checked) ? (_ENU[1] * (Target_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit).ToString("00.0000") : _ENU[1].ToString("00.0000");
                            var oriHgt = _ENU[2] / (Target_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit;
                            dgvTargetProjected[3, i].Value = (!chbxConvertToMeters.Checked) ? (oriHgt * (Target_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit).ToString("00.0000") : oriHgt.ToString("00.0000"); 

                            //dgvTargetProjected[4, i].Value = ENU[5] + " " + ENU[6];
                            //dgvTargetProjected[5, i].Value = ENU[4];
                            //dgvTargetProjected[6, i].Value = cor.ConvertToDMSString(double.Parse(ENU[3]), false, false);
                            dgvTargetProjected[4, i].Value = GNSS_Functions.GetZone;
                            dgvTargetProjected[5, i].Value = GNSS_Functions.GetGridScale;
                            dgvTargetProjected[6, i].Value = GNSS_Functions.GetConvergence;
                        }

                    }
                }

                lblPointCounts.Text = OutputDGV.RowCount.ToString("00");

                //Plot results
                CreatePointShapefile(axMap1);

                GC.Collect();
            }
            catch (Exception)
            {

                
            }

            cbxTileProvider.SelectedIndex = 0;

            
            //================Geodetic To Cartesian=====================
            //var LonLatH = new double[] { double.Parse(metroTextbox1.Text), double.Parse(metroTextbox2.Text), double.Parse(metroTextbox3.Text) };// cor.LonLatH2XYZ(SourceCRS, LonLatH);
            //result.Text += "\nLat = " + cor.ConvertToDMSString(LonLatH[0], true, true) + "  " + "Lon = " + cor.ConvertToDMSString(LonLatH[1], false, true) + "       " + "Z = " + LonLatH[2].ToString();


            //var XYZ = cor.LonLatH2XYZ(SourceCRS, LonLatH);
            //result.Text += "\nX = " + XYZ[0].ToString() + "  " + "\nY = " + XYZ[1].ToString() + "  " + "\nZ = " + XYZ[2].ToString();


            ////================Transformation==================
            ////Use inbuilt transformation parameters for Ghana
            //var TransParam = new TransParams().GH7TransParams();
            //var X1Y1Z1 = cor.XYZTransformation(TransParam, XYZ, false);
            //result.Text += "\nX1 = " + X1Y1Z1[0].ToString() + "  " + "\nY1 = " + X1Y1Z1[1].ToString() + "   " + "\nZ1 = " + X1Y1Z1[2].ToString();



            ////================Cartesian To Geodetic=====================
            //var LonLatH1 = cor.XYZ2LonLatH(TargetCRS, X1Y1Z1);
            ////result.Text += "\nLat = " + cor.ConvertToDMSString(LonLatH1[0], true, true) + "  " + "Lon = " + cor.ConvertToDMSString(LonLatH1[1], false, true) + "       " + "Z = " + LonLatH1[2].ToString();



            ////================Projection=====================
            //var Test = new double[] { double.Parse(metroTextbox1.Text), double.Parse(metroTextbox2.Text), double.Parse(metroTextbox3.Text) };// cor.LonLatH2XYZ(SourceCRS, LonLatH);
            //var ENU = cor.LonLatH2ENU(TargetCRS, LonLatH1);
            //result.Text += "E = " + ENU[0] + "       " + "N = " + ENU[1] + "       " + "Z = " + ENU[2] + "       " + "conv: = " + cor.ConvertToDMSString(double.Parse(ENU[3]), false, false)
            //        + "       " + "Grid Scale: " + ENU[4] + "       " + "Zone: " + ENU[5] + " " + ENU[6];


            ////================Reprojection===================
            //var LatLon = cor.ENU2LonLatH(TargetCRS, new double[] { double.Parse(ENU[0]), double.Parse(ENU[1]) });
            //var LatLon = cor.ENU2LonLatH(TargetCRS.a, TargetCRS.invF, double.Parse(ENU[1]) * 0.304799710181509, double.Parse(ENU[0]) * 0.304799710181509, double.Parse(ENU[2]), TargetCRS.ko, TargetCRS.FN, TargetCRS.FE, TargetCRS.φ0, TargetCRS.λ0);
            //result.Text += "\nLat = " + cor.ConvertToDMSString(LatLon[0], false, true) + "       " + "Lon = " + cor.ConvertToDMSString(LatLon[1], true, true) + "       " + "Z = " + LatLon[2].ToString();

            //var LatLon = cor.ProjectToLatLon(new double[] { double.Parse(ENU[0]), double.Parse(ENU[1]) }, new double[] { double.Parse(ENU[2]) }, 2136, 4168);
            //result.Text += "\nLon = " + cor.ConvertToDMSString(LatLon[0], false, true) + "       " + "Lat = " + cor.ConvertToDMSString(LatLon[1], true, true) + "       " + "Z = " + LatLon[2].ToString();



            //if (cbxFromUnits.Text == "Units")
            //{

            //    string msg = null;

            //    if (cbxFormat.SelectedIndex == 2 || cbxFormat.SelectedIndex == 5 || cbxFormat.SelectedIndex == 8)
            //    {
            //        msg = "Please select an output Units";
            //    }
            //    else if (cbxFormat.SelectedIndex == 2 || cbxFormat.SelectedIndex == 5 || cbxFormat.SelectedIndex == 8)
            //    {
            //        msg = "Please select an input Units";
            //    }

            //    //Show an error message
            //    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            //    //Terminate the process
            //    return;
            //}
        }
                       
        private void CoordinateConversion_Load(object sender, EventArgs e)
        {
            cbxFormat.SelectedIndex = 0;
            cbxType.SelectedIndex = 0;
        }

        bool Isprojection = false;
        private void cbxFormat_Click(object sender, EventArgs e)
        {
            cbxType.Items.Clear();

            try
            {
                if (cbxFormat.SelectedItem.ToString() == "Projection")
                {
                    cbxType.Items.Add("C->G");
                    cbxType.Items.Add("C->P");
                    cbxType.Items.Add("G->C");
                    cbxType.Items.Add("G->P");
                    cbxType.Items.Add("P->C");
                    cbxType.Items.Add("P->G");

                    Isprojection = true;
                }
                else
                {
                    cbxType.Items.Add("C->C");
                    cbxType.Items.Add("C->G");
                    cbxType.Items.Add("C->P");
                    cbxType.Items.Add("G->C");
                    cbxType.Items.Add("G->G");
                    cbxType.Items.Add("G->P");
                    cbxType.Items.Add("P->C");
                    cbxType.Items.Add("P->G");
                    cbxType.Items.Add("P->P");

                    Isprojection = false;
                }

                btnSwapCRS.Visible = (cbxFormat.SelectedItem.ToString() == "Projection") ? false : true;
                tbxToCRS.Visible = (cbxFormat.SelectedItem.ToString() == "Projection") ? false : true;
                btnOutputCRS.Visible = (cbxFormat.SelectedItem.ToString() == "Projection") ? false : true;
                lblOutLabel.Visible = (cbxFormat.SelectedItem.ToString() == "Projection") ? false : true;
                cbxType.SelectedIndex = 0;

                dgvTargetGeog.Rows.Clear();
                dgvTargetProjected.Rows.Clear();
                
            }
            catch (Exception)
            {

            }
            

        }

        // <summary>
        // Creates a point shapefile by placing 1000 points randomly
        // </summary>
        public void CreatePointShapefile(AxMap axMap1)
        {
                        
            axMap1.RemoveAllLayers();
            axMap1.Refresh();
            bool Result = true;

            var IGeoncenTarget = cFac.CreateGeocentricCoordinateSystem(" ", (Target_CRS as IProjectedCoordinateSystem).HorizontalDatum, (Target_CRS as IProjectedCoordinateSystem).LinearUnit, (Target_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem.PrimeMeridian);
            var IGeoncenWGS84 = cFac.CreateGeocentricCoordinateSystem(" ", (MainScreen.WGS84 as IProjectedCoordinateSystem).HorizontalDatum, (MainScreen.WGS84 as IProjectedCoordinateSystem).LinearUnit, (MainScreen.WGS84 as IProjectedCoordinateSystem).GeographicCoordinateSystem.PrimeMeridian);
            ICoordinateTransformation transCART = new CoordinateTransformationFactory().CreateFromCoordinateSystems(IGeoncenTarget, IGeoncenWGS84);
            ICoordinateTransformation ProjectTargetCARTGEO = new CoordinateTransformationFactory().CreateFromCoordinateSystems(IGeoncenWGS84, (MainScreen.WGS84 as IProjectedCoordinateSystem).GeographicCoordinateSystem);
            ICoordinateTransformation ProjectTargetGEOPRO = new CoordinateTransformationFactory().CreateFromCoordinateSystems((MainScreen.WGS84 as IProjectedCoordinateSystem).GeographicCoordinateSystem, (MainScreen.WGS84 as IProjectedCoordinateSystem));

            //Create Temporal Shapefiles
            var sfRovers = new Shapefile();
            

            //Declare shapefile types
            // MWShapeId field will be added to attribute table           
            Result = sfRovers.CreateNewWithShapeID("", ShpfileType.SHP_POINT);
            sfRovers.GeoProjection.ImportFromWKT(MainScreen.WGS84.WKT);
            axMap1.GeoProjection.ImportFromWKT(MainScreen.WGS84.WKT);
           
            var f = new Field();
            f.Type = MapWinGIS.FieldType.STRING_FIELD;
            f.Name = "SiteID";
            f.Width = 20;

            Result = sfRovers.EditInsertField(f, 0);
                                   
            Shape PTshp = new Shape();
            PTshp.Create(ShpfileType.SHP_POINT);

            int index = 0;
                        
            //Add Item
            
            PlotPointList = new List<PlotPoint>();
            var pnt = new Point();
            double[] _ENU = new double[] { };

            //Compute for plotting
            if (cbxType.SelectedItem.ToString().Contains("->C"))
            {
                for (int i = 0; i < OutputDGV.RowCount; i++)
                {
                    pnt = new Point();
                    var X1Y1Z1 = transCART.MathTransform.Transform(new double[] { double.Parse(OutputDGV[1, i].Value.ToString()), double.Parse(OutputDGV[2, i].Value.ToString()), double.Parse(OutputDGV[3, i].Value.ToString()) });

                    var LonLatH = ProjectTargetCARTGEO.MathTransform.Transform(X1Y1Z1);
                    _ENU = ProjectTargetGEOPRO.MathTransform.Transform(LonLatH);

                    pnt.x = _ENU[0];
                    pnt.y = _ENU[1];

                    PTshp = new Shape();
                    PTshp.Create(ShpfileType.SHP_POINT);

                    index = 0;
                    PTshp.InsertPoint(pnt, ref index);
                    sfRovers.EditInsertShape(PTshp, ref i);

                    //insert some integer value
                    Result = sfRovers.EditCellValue(0, i, OutputDGV.Rows[i].Cells[0].Value.ToString());

                    //PlotPointList.Add(new PlotPoint(OutputDGV[0, i].Value.ToString(), _ENU[0], _ENU[1], _ENU[2] / (Target_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit));

                }
            }
            else if (cbxType.SelectedItem.ToString().Contains("->G"))
            {
                trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((Target_CRS as IProjectedCoordinateSystem).GeographicCoordinateSystem, (MainScreen.WGS84 as IProjectedCoordinateSystem).GeographicCoordinateSystem);

                for (int i = 0; i < OutputDGV.RowCount; i++)
                {
                    pnt = new Point();
                    var Lat = cor.DMS2DecDeg(OutputDGV[1, i].Value.ToString(), double.Parse(OutputDGV[2, i].Value.ToString()), double.Parse(OutputDGV[3, i].Value.ToString()));
                    var Lon = cor.DMS2DecDeg(OutputDGV[4, i].Value.ToString(), double.Parse(OutputDGV[5, i].Value.ToString()), double.Parse(OutputDGV[6, i].Value.ToString()));

                    var LonLatH = trans.MathTransform.Transform(new double[] { Lon, Lat, double.Parse(OutputDGV[7, i].Value.ToString()) });
                    _ENU = ProjectTargetGEOPRO.MathTransform.Transform(LonLatH);

                    pnt.x = _ENU[0];
                    pnt.y = _ENU[1];

                    PTshp = new Shape();
                    PTshp.Create(ShpfileType.SHP_POINT);

                    index = 0;
                    PTshp.InsertPoint(pnt, ref index);
                    sfRovers.EditInsertShape(PTshp, ref i);

                    //insert some integer value
                    Result = sfRovers.EditCellValue(0, i, OutputDGV.Rows[i].Cells[0].Value.ToString());

                    //PlotPointList.Add(new PlotPoint(OutputDGV[0, i].Value.ToString(), _ENU[0], _ENU[1], _ENU[2] / (Target_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit));

                }
            }
            else
            {
                trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((Target_CRS as IProjectedCoordinateSystem), (MainScreen.WGS84 as IProjectedCoordinateSystem));

                for (int i = 0; i < OutputDGV.RowCount; i++)
                {
                    pnt = new Point();

                    _ENU = trans.MathTransform.Transform(new double[] { double.Parse(OutputDGV[1, i].Value.ToString()), double.Parse(OutputDGV[2, i].Value.ToString()), double.Parse(OutputDGV[3, i].Value.ToString()) });

                    pnt.x = _ENU[0];
                    pnt.y = _ENU[1];

                    PTshp = new Shape();
                    PTshp.Create(ShpfileType.SHP_POINT);

                    index = 0;
                    PTshp.InsertPoint(pnt, ref index);
                    sfRovers.EditInsertShape(PTshp, ref i);

                    //insert some integer value
                    Result = sfRovers.EditCellValue(0, i, OutputDGV.Rows[i].Cells[0].Value.ToString());

                    //PlotPointList.Add(new PlotPoint(OutputDGV[0, i].Value.ToString(), _ENU[0], _ENU[1], _ENU[2] / (Target_CRS as IProjectedCoordinateSystem).LinearUnit.MetersPerUnit));

                }
            }


            
            //Apply labels  and symbol to the shapefile
            GNSS_Functions.labelShapefile(sfRovers, f.Name, 10, tkDefaultPointSymbol.dpsTriangleDown);

            // adds shapefile to the map 
            ShapefileHandler = axMap1.AddLayer(sfRovers, true);
            axMap1.ZoomToLayer(ShapefileHandler);
           

            // save if needed
            //sf.SaveAs(@"c:\points.shp", null);
        }

        static List<PlotPoint> PlotPointList;

        bool Show_HidePlots()
        {
            bool IsShown = false;
            if (OutputDGV.RowCount > 0 && btnCompute.Enabled)
            {
                pnlPlotPoints.BringToFront();
                IsShown = true;
            }
            else
            {                
                pnlPlotPoints.SendToBack();                
            }

            return IsShown;
        }

        
        private void btnPlot_Click(object sender, EventArgs e)
        {
            //btnPlot.Enabled = (OutputDGV.RowCount > 0) ? true : false;
            if (OutputDGV.RowCount > 0)
            {
                
                axMap1.Redraw();
                bool IsPlotShown = Show_HidePlots();

                btnCompute.Enabled = (IsPlotShown) ? false : true;
                btnBack.Enabled = (IsPlotShown) ? false : true;
                btnCancel.Enabled = (IsPlotShown) ? false : true;
                chbxConvertToMeters.Enabled = (IsPlotShown) ? false : true;

                NewToolStripButton.Enabled = (IsPlotShown) ? false : true;
                OpenToolStripButton.Enabled = (IsPlotShown) ? false : true;
            }
            else
            {
                MessageBox.Show("Nothing to plot!", "Plot Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        string tileEd = string.Empty;
        private void cbxTileProvider_TextChanged(object sender, EventArgs e)
        {
            try
            {
                tileEd = cbxTileProvider.SelectedItem.ToString().Replace("  ", "");
                
                axMap1.TileProvider = (tkTileProvider)Enum.Parse(typeof(tkTileProvider), Regex.Replace(tileEd, @"\s", ""));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void btnSwapCRS_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbxFromCRS.Text !="" && tbxToCRS.Text != "")
                {
                    var IntrChangeCRS = Source_CRS;
                    var CRStext = tbxFromCRS.Text;

                    Source_CRS = Target_CRS;
                    tbxFromCRS.Text = tbxToCRS.Text;

                    Target_CRS = IntrChangeCRS;
                    tbxToCRS.Text = CRStext;
                }
                else
                {
                    MessageBox.Show("Missing CRS(s) to swap with","CRS Swap Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                
            }
            catch (Exception)
            {

            }
            
        }

        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                
                DialogResult strReply = MessageBox.Show("Save input values?\n\nClick no save output values.", "Notice", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                if (strReply == DialogResult.Yes)
                {
                    SaveAs(InputDGV);
                }
                else if(strReply == DialogResult.No)
                {
                    SaveAs(OutputDGV);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not export to file\n\nMessage:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        

        #region Save Output File As
	    public void SaveAs(DataGridView dgvSave)
        {

            if (dgvSave.RowCount <= 1)
            {
                MessageBox.Show("No data to export!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
                        
            var SaveFileDialog = new SaveFileDialog();
            SaveFileDialog.Filter = "Text (*.txt)|*.txt;*.TXT|CSV files (*.csv)|*.csv;*.CSV";
            

            string ext = null;
            string FileName = null;

            if (SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileName = SaveFileDialog.FileName;
                ext = System.IO.Path.GetExtension(FileName);
            }
            else
            {
                MessageBox.Show("File save was canceled", "Operation Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                StringBuilder builder = new StringBuilder();
                var headers = (
                    from DataGridViewColumn header in dgvSave.Columns.Cast<DataGridViewColumn>()
                    select header.HeaderText).ToArray();

                var rows = from DataGridViewRow row in dgvSave.Rows.Cast<DataGridViewRow>()
                           where !row.IsNewRow
                           select Array.ConvertAll(row.Cells.Cast<DataGridViewCell>().ToArray(), (c) => ((c.Value != null) ? c.Value.ToString() : ""));

                builder.AppendLine(string.Join(",", headers));
                foreach (var R in rows)
                {
                    builder.AppendLine(string.Join(",", R));
                }

                System.IO.File.WriteAllText(FileName, builder.ToString());
                MessageBox.Show("Project Saved Successively", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while saving the project." + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        #endregion

        private void chbxConvertToMeters_CheckedChanged(object sender, bool isChecked)
        {
            if (OutputDGV.RowCount > 0)
            {
                btnCompute.PerformClick();
            }
        }

        public int ShapefileHandler { get; set; }
        private void toolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name != nameof(btnSelect))
            {
                axMap1.SendMouseUp = false;
            }

            switch (e.ClickedItem.Name)
            {
                case nameof(btnArrow):
                    axMap1.CursorMode = MapWinGIS.tkCursorMode.cmNone;
                    break;
                case nameof(btnZoomIn):
                    axMap1.CursorMode = MapWinGIS.tkCursorMode.cmZoomIn;
                    break;
                case nameof(btnZoomOut):
                    axMap1.CursorMode = MapWinGIS.tkCursorMode.cmZoomOut;
                    break;
                case nameof(btnSelect):
                    axMap1.CursorMode = MapWinGIS.tkCursorMode.cmSelection;
                    axMap1.SendMouseUp = true;
                    break;
                case nameof(btnPan):
                    axMap1.CursorMode = MapWinGIS.tkCursorMode.cmPan;
                    break;
                case nameof(btnZoomToExtent):
                    axMap1.ZoomToMaxExtents();
                    break;
                case nameof(btnZoomToPoint):
                    axMap1.ZoomToSelected(ShapefileHandler);
                    break;
                default:
                    break;
            }
        }
    }
}
