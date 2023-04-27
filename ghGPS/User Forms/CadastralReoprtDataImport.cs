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
using AxMapWinGIS;
using MapWinGIS;

namespace ghGPS.User_Forms
{
    public partial class CadastralReoprtDataImport : UserControl
    {
        public CadastralReoprtDataImport()
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //Import points
            GNSS_Functions.ImportPoints(this, "EXCEL_CSV_TXT");
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in dgvPoints.SelectedRows)
            {
                dgvPoints.Rows.Remove(item);
            }

            this.Refresh();
        }

        private void dgvPoints_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            lblRoverCounts.Text = (dgvPoints.RowCount - 1).ToString("00");

            btnNext.Enabled = (dgvPoints.RowCount - 1 > 5) ? true : false;            
        }

        private void dgvPoints_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            lblRoverCounts.Text = (dgvPoints.RowCount - 1).ToString("00");

            btnNext.Enabled = (dgvPoints.RowCount - 1 > 5) ? true : false;           
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            //Reload all point to BaseLine Chart form
            ReloadPointList();

            MainScreen.baseLineChart.olvPointListTree1.Visible = true;
            MainScreen.baseLineChart.olvtree = MainScreen.baseLineChart.olvPointListTree1;

            //Prepare a chart for processing
            MainScreen.baseLineChart.BringToFront();
        }

        public static List<Points> Pointlist = new List<Points>();

        public void ReloadPointList(bool fromImport = true)
        {
            if (fromImport)
            {
                CreatePointShapefile(MainScreen.baseLineChart.axMap1);
            }
            else
            {

                dgvPoints.Rows.Clear();

                int RowIndex = 0;
                foreach (var item in Pointlist)
                {
                    RowIndex = dgvPoints.RowCount - 1;
                    dgvPoints.Rows.Add();

                    dgvPoints.Rows[RowIndex].Cells[0].Value = item.SiteID;

                    dgvPoints.Rows[RowIndex].Cells[1].Value = item.Eastings;  //E

                    dgvPoints.Rows[RowIndex].Cells[2].Value = item.Northings;  //N                    

                }                

                ReloadPointList();

            }

        }

        public void CreatePointShapefile(AxMap axMap1)
        {
            //Clear all nodes
            MainScreen.baseLineChart.olvPointListTree1.Items.Clear();
            MainScreen.baseLineChart.axMap1.RemoveAllLayers();
            MainScreen.baseLineChart.axMap1.Refresh();
            bool Result = true;

            //Create Temporal Shapefiles
            var sfPoints = new Shapefile();
            var connectingLineSp = new Shapefile();
            
            //Declare shapefile types
            // MWShapeId field will be added to attribute table            
            Result = sfPoints.CreateNewWithShapeID("", ShpfileType.SHP_POINT);

            // MWShapeId field will be added to attribute table
            Result = connectingLineSp.CreateNewWithShapeID("", ShpfileType.SHP_POLYLINE);

            var f = new Field();
            f.Type = MapWinGIS.FieldType.STRING_FIELD;
            f.Name = "SiteID";
            f.Width = 20;

            Result = sfPoints.EditInsertField(f, 0);       

            
            Shape PTshp = new Shape();
            PTshp.Create(ShpfileType.SHP_POINT);

            int index = 0;   

            Pointlist = new List<Points>();

            StringBuilder ALLPointlist = new StringBuilder();

            var rtbx = new RichTextBox();
            rtbx.Font = new Font("Courier New", float.Parse("9.5"));

            //Add Item
            for (int i = 0; i < dgvPoints.Rows.Count-1; i++)
            {
                var pnt = new MapWinGIS.Point();
                pnt.x = double.Parse(dgvPoints.Rows[i].Cells[1].Value.ToString());
                pnt.y = double.Parse(dgvPoints.Rows[i].Cells[2].Value.ToString());

                ALLPointlist.AppendLine(dgvPoints.Rows[i].Cells[0].Value.ToString() + "," + dgvPoints.Rows[i].Cells[1].Value.ToString() + "," + dgvPoints.Rows[i].Cells[2].Value.ToString());

                //Base Location
                var Nextpnt = new MapWinGIS.Point();
                int x = 0;
                if (i == dgvPoints.Rows.Count - 3)
                {
                    x = 1;
                }
                else if(i == dgvPoints.Rows.Count - 2)
                {
                    x = dgvPoints.Rows.Count - 3;
                }
                else
                {
                    x = 1 + i;
                }

                Nextpnt.x = double.Parse(dgvPoints.Rows[x].Cells[1].Value.ToString());
                Nextpnt.y = double.Parse(dgvPoints.Rows[x].Cells[2].Value.ToString());

                PTshp = new Shape();
                PTshp.Create(ShpfileType.SHP_POINT);

                index = 0;
                PTshp.InsertPoint(pnt, ref index);
                sfPoints.EditInsertShape(PTshp, ref i);

                //insert some integer value
                Result = sfPoints.EditCellValue(0, i, dgvPoints.Rows[i].Cells[0].Value.ToString());

                //=============================Connecting Lines ========================
                Shape LineShp = new Shape();
                LineShp.Create(ShpfileType.SHP_POLYLINE);

                LineShp.InsertPoint(pnt, index); //The base point
                LineShp.InsertPoint(Nextpnt, index); //The Rover point

                //Add line to shapefiles
                connectingLineSp.EditInsertShape(LineShp, ref i);

                Pointlist.Add(new Points(dgvPoints.Rows[i].Cells[0].Value.ToString(), dgvPoints.Rows[i].Cells[1].Value.ToString(), dgvPoints.Rows[i].Cells[2].Value.ToString()));
            }

            rtbx.Text = ALLPointlist.ToString();
            GNSS_Functions.ALLPointlist = rtbx.Rtf;

            //Apply labels  and symbol to the shapefile
            labelShapefile(sfPoints, f.Name, 10, tkDefaultPointSymbol.dpsCircle);            

            MainScreen.baseLineChart.shpfile = sfPoints;

            MainScreen.baseLineChart.olvPointListTree1.SetObjects(AllPointList);

            // adds shapefile to the map  
            MainScreen.baseLineChart.ShapefileHandler = axMap1.AddLayer(sfPoints, true);
            axMap1.AddLayer(connectingLineSp, true);

            axMap1.ZoomToMaxExtents();

            // save if needed
            //sf.SaveAs(@"c:\points.shp", null);
        }


        //Label Shapefile and Symbol
        private void labelShapefile(Shapefile shpfile, string fieldName, int FontSize, tkDefaultPointSymbol symbol)
        {
            shpfile.Labels.Generate("[" + fieldName + "]", tkLabelPositioning.lpCentroid, true);
            shpfile.Labels.AvoidCollisions = true;
            shpfile.Labels.AutoOffset = true;
            shpfile.Labels.InboxAlignment = tkLabelAlignment.laCenter;
            shpfile.Labels.FrameVisible = false;
            //shpfile.Labels.FrameType = tkLabelFrameType.lfPointedRectangle
            //shpfile.Labels.FrameOutlineColor = LabelColor
            //shpfile.Labels.FrameBackColor2 = LabelColor1
            //shpfile.Labels.FrameTransparency = 100
            shpfile.Labels.FontSize = FontSize;

            shpfile.DefaultDrawingOptions.SetDefaultPointSymbol(symbol);
        }

        private static List<Points> AllPointList
        {
            get { return Pointlist ?? (Pointlist = LoadPoints()); }
        }        

        private static List<Points> LoadPoints()
        {
            return Pointlist;
        }

        private void btnSetTraverse_Click(object sender, EventArgs e)
        {
            
        }
    }
}
