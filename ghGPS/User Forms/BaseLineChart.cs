using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MapWinGIS;
using BrightIdeasSoftware;
using ghGPS.Classes;

using ghGPS.Forms;

namespace ghGPS.User_Forms
{
    public partial class BaseLineChart : UserControl
    {
        public BaseLineChart()
        {
            InitializeComponent();

            SetupColumns();
            SetupDragAndDrop();
            
        }

        public int ShapefileHandler { get; set; }
        public int[] ShapeIDs = new int[] { };

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
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

        private void btnDiscard_MouseEnter(object sender, EventArgs e)
        {
            btnDiscard.BorderColor = Color.Red;
        }

        private void btnDiscard_MouseLeave(object sender, EventArgs e)
        {
            btnDiscard.BorderColor = Color.FromArgb(98, 98, 98);
        }

        private void btnApply_MouseEnter(object sender, EventArgs e)
        {
            btnApply.BorderColor = Color.LightGreen;
        }

        private void btnApply_MouseLeave(object sender, EventArgs e)
        {
            btnApply.BorderColor = Color.FromArgb(98, 98, 98);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (GNSS_Functions.IsProcessed)
            {
                //Show GNSS Data Import 
                MainScreen.GNSSDataImport.BringToFront();
            }
            else
            {
                //Show Manual Data Import
                MainScreen.manualDataImport.BringToFront();
            }
           
        }

        private void axMap1_MouseMoveEvent(object sender, AxMapWinGIS._DMapEvents_MouseMoveEvent e)
        {
            //Projected Y
            double X = 0, Y = 0;
            axMap1.PixelToProj(e.x, e.y, ref X, ref Y);
            lblProjected.Text = "  E: " + Math.Round(X, 3).ToString() + " N: " + Math.Round(Y, 3).ToString();

        }

        private void editSwitch_CheckedChanged(object sender, bool isChecked)
        {
            if (editSwitch.Checked == false)
            {
                tbxSiteID.Enabled = true;
                tbxAntennaHeight.Enabled = true;
                btnApply.Enabled = true;
                btnDiscard.Enabled = true;
                cbxAntennaType.Enabled = true;
            }
            else
            {
                tbxSiteID.Enabled = false;
                tbxAntennaHeight.Enabled = false;
                btnApply.Enabled = false;
                btnDiscard.Enabled = false;
                cbxAntennaType.Enabled = false;
            }
        }

        private void btnDiscard_Click(object sender, EventArgs e)
        {
            tbxAntennaHeight.Text = "";
            tbxSiteID.Text = "";
            tbxSiteID.Enabled = false;
            tbxAntennaHeight.Enabled = false;
            btnApply.Enabled = false;
            btnDiscard.Enabled = false;
            cbxAntennaType.Enabled = false;
            editSwitch.Checked = true;

        }

        int selecteIndex = 0;
        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                var olvPointList = (GNSS_Functions.IsProcessed) ? olvPointListTree : olvPointListTree1;
                int SiteIDIndex = (GNSS_Functions.IsProcessed) ? 1 : 0;
                if (olvPointList.SelectedItems.Count > 0)
                {
                    var selectedItems = new int[olvPointList.SelectedItems.Count];

                    if (olvPointList.SelectedItems.Count > 1)
                    {
                        for (int i = 0; i < olvPointList.SelectedItems.Count; i++)
                        {
                            for (int j = 0; j < olvPointList.Items.Count; j++)
                            {
                                if (olvPointList.Items[j].ToString().Contains(olvPointList.SelectedItems[i].ToString()))
                                {
                                    

                                    if (GNSS_Functions.IsProcessed)
                                    {
                                        MainScreen.GNSSDataImport.dgvRovers.Rows[j].Cells[1].Value = tbxSiteID.Text + (i + 1).ToString();        //Set site ID 
                                        MainScreen.GNSSDataImport.dgvRovers.Rows[j].Cells[10].Value = tbxAntennaHeight.Text + (i + 1).ToString();        //Set the antenna Height
                                        MainScreen.GNSSDataImport.dgvRovers.Rows[j].Cells[11].Value = cbxAntennaType.SelectedItem.ToString(); //Set the antenna Type
                                    }
                                    else
                                    {
                                        MainScreen.manualDataImport.dgvPoints.Rows[j].Cells[0].Value = tbxSiteID.Text + (i + 1).ToString();        //Set site ID 
                                    }

                                    //Gather all selected items
                                    selectedItems[i] = j;
                                    break;
                                }
                            }
                        }

                    }
                    else
                    {                        
                        if (GNSS_Functions.IsProcessed)
                        {
                            MainScreen.GNSSDataImport.dgvRovers.Rows[olvPointList.SelectedIndex].Cells[1].Value = tbxSiteID.Text;        //Set site ID 
                            MainScreen.GNSSDataImport.dgvRovers.Rows[olvPointList.SelectedIndex].Cells[10].Value = tbxAntennaHeight.Text;        //Set the antenna Height
                            MainScreen.GNSSDataImport.dgvRovers.Rows[olvPointList.SelectedIndex].Cells[11].Value = cbxAntennaType.SelectedItem.ToString(); //Set the antenna Type                           
                        }
                        else
                        {
                            MainScreen.manualDataImport.dgvPoints.Rows[olvPointList.SelectedIndex].Cells[0].Value = tbxSiteID.Text;        //Set site ID 
                        }  
                    }

                    //Reload data
                    if (GNSS_Functions.IsProcessed)
                    {
                        MainScreen.GNSSDataImport.ReloadPointList();
                    }
                    else
                    {
                        MainScreen.manualDataImport.ReloadPointList();
                    }
                    

                    //Select and zoom to the selected items
                    for (int i = 0; i < selectedItems.Count(); i++)
                    {
                        shpfile.ShapeSelected[selectedItems[i]] = true;
                    }

                    axMap1.ZoomToSelected(ShapefileHandler); //Zoom to the selected

                    tbxAntennaHeight.Text = "";
                    tbxSiteID.Text = "";
                    cbxAntennaType.SelectedIndex = -1;
                }
            }
            catch (Exception)
            {


            }

        }

 
        private void axMap1_ChooseLayer(object sender, AxMapWinGIS._DMapEvents_ChooseLayerEvent e)
        {
            //choose layer according to the logic of your application ' for example, legend1.SelectedLayer if your app uses legend control
            e.layerHandle = ShapefileHandler;
        }

        bool SelectionFromPointList = false;

        /// <summary>
        /// Clear Selection
        /// </summary>
        void ClearSelection()
        {
            for (int i = 0; i < olvtree.Items.Count; i++)
            {
                olvtree.Items[i].Selected = false;
            }
        }

        public ObjectListView olvtree = new ObjectListView();
        private void axMap1_SelectionChanged(object sender, AxMapWinGIS._DMapEvents_SelectionChangedEvent e)
        {
            
            //PointListTree.ClearSelected();
            SelectionFromPointList = false;
            if (shpfile.NumSelected == 0)
            {
                olvtree.ClearHotItem();
                ClearSelection();
            }
            else
            {

                for (int i = 0; i < shpfile.NumShapes; i++)
                {                    
                    if (shpfile.ShapeSelected[i] == true)
                    {
                        var theSiteID = shpfile.CellValue[0, i].ToString().Trim();
                        
                        for (int j = 0; j < olvtree.Items.Count; j++)
                        {
                            if (olvtree.Items[j].Text.ToString().Equals(theSiteID))
                            {                                
                                olvtree.Items[j].Selected = true;                                
                                break;
                            }
                        }
                        //break;
                    }
                }
            }

            axMap1.Redraw();
            olvtree.Focus();
        }

        public Shapefile shpfile = new Shapefile();
        private void axMap1_MouseUpEvent(object sender, AxMapWinGIS._DMapEvents_MouseUpEvent e)
        {

            //if (shpfile.NumSelected > 1) return;

            //for (int i = 0; i < shpfile.NumShapes; i++)
            //{
            //    if (shpfile.ShapeSelected[i])
            //    {

            //        var theSiteID = shpfile.CellValue[0, i].ToString();

            //        for (int j = 0; i < PointListTree.Items.Count; i++)
            //        {
            //            if (PointListTree.Items[i].ToString().Contains(theSiteID))
            //            {
            //                PointListTree.SelectedIndex = i;                            
            //                //break;
            //            }
            //        }
            //        //break;
            //    }
            //}

        }
                
        private void btnProcessData_Click(object sender, EventArgs e)
        {
            if (GNSS_Functions.IsProcessed)
            {
                //Process the results
                GNSS_Functions.BaseOBSFilePath = MainScreen.GNSSDataImport.tbxBasePath.Text;
                GNSS_Functions.BaseSiteID = MainScreen.GNSSDataImport.lblSiteID.Text;
                GNSS_Functions.BaseAntHeight = (!MainScreen.GNSSDataImport.pnlInput.Visible) ? "0" : "0"; //Change to the antenna height
                GNSS_Functions.BaseNAVFilePath = MainScreen.GNSSDataImport.tbxNavigationfilePath.Text;


                var roverfiles = new string[MainScreen.GNSSDataImport.dgvRovers.RowCount];
                var roverIDs = new string[MainScreen.GNSSDataImport.dgvRovers.RowCount];
                var roverAntenaHeight = new string[MainScreen.GNSSDataImport.dgvRovers.RowCount];

                for (int i = 0; i < MainScreen.GNSSDataImport.dgvRovers.RowCount; i++)
                {
                    roverfiles[i] = MainScreen.GNSSDataImport.dgvRovers.Rows[i].Cells[0].Value.ToString();
                    roverIDs[i] = MainScreen.GNSSDataImport.dgvRovers.Rows[i].Cells[1].Value.ToString();
                    roverAntenaHeight[i] = (MainScreen.GNSSDataImport.dgvRovers.Rows[i].Cells[1].Value.ToString() == "") ? "0" : MainScreen.GNSSDataImport.dgvRovers.Rows[i].Cells[1].Value.ToString();
                }

                //Check if Cadastral Report is to be included
                MainScreen.ProcessResults.pnlCadastralReport.Visible = (GNSS_Functions.IsCadastralIncluded == true) ? true : false;

                //Get all Results
                var results = new ProcessedSolution();                               

                results.solutions(GNSS_Functions.BaseOBSFilePath, GNSS_Functions.BaseSiteID, GNSS_Functions.BaseAntHeight, roverfiles, roverIDs, roverAntenaHeight);

                MainScreen.ProcessResults.rtbxResults.RtfText = GNSS_Functions.PointList;

                GNSS_Functions.ShowResults();
                               
            }
            else
            {
                using (var crTravers = new CreateTraversePath())
                {
                    if (crTravers.ShowDialog() == DialogResult.OK)
                    {
                        //Show results
                        MainScreen.ProcessResults.rtbxResults.RtfText = GNSS_Functions.Beacon;
                        GNSS_Functions.ShowResults();
                    }

                }                

            }            

        }
               
        
        private void SetupColumns()
        {
            this.clnFile.AspectGetter = delegate (object x) { return ((Rover)x).FilePath; ; };
            this.clnSiteName.AspectGetter = delegate (object x) { return ((Rover)x).SiteID; ; };
            this.clnStartTime.AspectGetter = delegate (object x) { return ((Rover)x).StartTime; ; };
            this.clnStopTime.AspectGetter = delegate (object x) { return ((Rover)x).StopTime; ; };
            this.clnEpoch.AspectGetter = delegate (object x) { return ((Rover)x).Epoch; ; };
            this.clnDataRate.AspectGetter = delegate (object x) { return ((Rover)x).DataRate; ; };
            this.clnSize.AspectGetter = delegate (object x) { return ((Rover)x).Size; ; };
            this.clnEastings.AspectGetter = delegate (object x) { return ((Rover)x).Eastings; ; };
            this.clnNorthings.AspectGetter = delegate (object x) { return ((Rover)x).Northings; ; };
            this.clnHeight.AspectGetter = delegate (object x) { return ((Rover)x).Heights; ; };
            this.clnAntennaHeight.AspectGetter = delegate (object x) { return ((Rover)x).AntennaHeight; ; };
            this.clnAntennType.AspectGetter = delegate (object x) { return ((Rover)x).AntennaType; ; };

            this.clnSiteName1.AspectGetter = delegate (object x) { return ((Points)x).SiteID; ; };
            this.clnEastings1.AspectGetter = delegate (object x) { return ((Points)x).Eastings; ; };
            this.clnNorthings1.AspectGetter = delegate (object x) { return ((Points)x).Northings; ; };
        }

        private void SetupDragAndDrop()
        {

            // Make each listview capable of dragging rows out
            this.olvPointListTree.DragSource = new SimpleDragSource();
            this.olvPointListTree1.DragSource = new SimpleDragSource();


            // Make each listview capable of accepting drops.
            // More than that, make it so it's items can be rearranged
            this.olvPointListTree.DropSink = new RearrangingDropSink(true);
            this.olvPointListTree1.DropSink = new RearrangingDropSink(true);

            // For a normal drag and drop situation, you will need to create a SimpleDropSink
            // and then listen for ModelCanDrop and ModelDropped events
        }

        private void olvPointListTree_SelectedIndexChanged(object sender, EventArgs e)
        {            
            
            selecteIndex = olvtree.SelectedIndex;
            
            if (selecteIndex > -1)
            {

                if (olvtree.SelectedItems.Count > 1)
                {
                    if (!SelectionFromPointList)
                    {
                        shpfile.SelectNone();

                        for (int i = 0; i < olvtree.SelectedItems.Count; i++)
                        {
                            for (int j = 0; j < olvtree.Items.Count; j++)
                            {
                                if (olvtree.Items[j].Text.Equals(olvtree.SelectedItems[i].ToString()))
                                {
                                    shpfile.ShapeSelected[j] = true;

                                    break;
                                }
                            }
                        }
                        
                    }

                    axMap1.ZoomToSelected(ShapefileHandler); //Zoom to the selected
                    axMap1.Redraw();

                }
                else
                {
                    if (GNSS_Functions.IsProcessed)
                    {
                        tbxSiteID.Text = olvtree.SelectedItem.GetSubItem(1).Text;        //Get site ID 
                        tbxAntennaHeight.Text = olvtree.SelectedItem.GetSubItem(10).Text; //Get the antenna Height
                        cbxAntennaType.SelectedItem = olvtree.SelectedItem.GetSubItem(11).Text; //Get the antenna Height
                    }
                    else
                    {
                        tbxSiteID.Text = olvtree.SelectedItem.GetSubItem(0).Text;        //Get site ID 
                    }                    

                }

                SelectionFromPointList = true;

            }
            else
            {

                olvPointListTree.ClearHotItem();
                olvPointListTree1.ClearHotItem();
                //ClearSelection();
                tbxAntennaHeight.Text = "";
                tbxSiteID.Text = "";
                cbxAntennaType.SelectedIndex = 0;
 
            }
        }

        private void olvPointListTree_DoubleClick(object sender, EventArgs e)
        {
            shpfile.SelectNone();
            shpfile.ShapeSelected[(GNSS_Functions.IsProcessed)? olvPointListTree.SelectedIndex : olvPointListTree1.SelectedIndex] = true;
            axMap1.ZoomToSelected(ShapefileHandler);
        }

        private void olvPointListTree_DragDrop(object sender, DragEventArgs e)
        {
            
        }

        private void olvPointListTree_Click(object sender, EventArgs e)
        {
            SelectionFromPointList = true;
        }

        private void olvPointListTree_Dropped(object sender, OlvDropEventArgs e)
        {
            if (GNSS_Functions.IsProcessed)
            {
                GNSSDataImport.Roverlist = new List<Rover>();

                for (int i = 0; i < olvPointListTree.Items.Count; i++)
                {
                    GNSSDataImport.Roverlist.Add(new Rover(olvPointListTree.Items[i].SubItems[0].Text, olvPointListTree.Items[i].SubItems[1].Text, olvPointListTree.Items[i].SubItems[2].Text,
                        olvPointListTree.Items[i].SubItems[3].Text, olvPointListTree.Items[i].SubItems[4].Text, olvPointListTree.Items[i].SubItems[5].Text,
                        olvPointListTree.Items[i].SubItems[6].Text, olvPointListTree.Items[i].SubItems[7].Text, olvPointListTree.Items[i].SubItems[8].Text,
                        olvPointListTree.Items[i].SubItems[9].Text, olvPointListTree.Items[i].SubItems[10].Text, olvPointListTree.Items[i].SubItems[11].Text));
                }
                
                //Reload data
                MainScreen.GNSSDataImport.ReloadPointList(false);
            }
            else
            {
                CadastralReoprtDataImport.Pointlist = new List<Points>();

                for (int i = 0; i < olvPointListTree1.Items.Count; i++)
                {
                    CadastralReoprtDataImport.Pointlist.Add(new Points(olvPointListTree1.Items[i].SubItems[0].Text, olvPointListTree1.Items[i].SubItems[1].Text, olvPointListTree1.Items[i].SubItems[2].Text));
                }

                //Reload data
                MainScreen.manualDataImport.ReloadPointList(false);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //Clear, reset fields and go to home page
            GNSS_Functions.ClearCreateNewProject();
        }
    }
}
