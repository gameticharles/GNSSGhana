using BrightIdeasSoftware;
using ghGPS.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ghGPS.Forms
{
    public partial class CreateTraversePath : MetroSuite.MetroForm
    {
        public CreateTraversePath()
        {
            InitializeComponent();

            SetupColumns();
            SetupDragAndDrop();

            olvPointListTree1.SetObjects(AllRoverList);
        }

        public List<Points> AllRoverList
        {
            get { return loadPoints(); }
        }

        List<Points> cbxPointlist = new List<Points>();
        List<Points> traversPathPoints = new List<Points>();
        private static List<Points> Pointlist = new List<Points>();
        private List<Points> loadPoints()
        {
            Pointlist = new List<Points>();
            cbxPointlist = new List<Points>();
            var rtbx = new RichTextBox
            {
                Rtf = GNSS_Functions.ALLPointlist
            };
            var PtList = rtbx.Text.Split('\n');
            rtbx.Dispose();

            for (int i = 0; i < PtList.Count() - 1; i++)
            {

                cbxPointlist.Add(new Points(PtList[i].Split(',')));             

            }

            for (int i = 0; i < PtList.Count()-1; i++)
            {
                try
                {
                    if ((cbxStartingStation.SelectedItem.ToString() != (PtList[i].Split(','))[0]) && (cbxClosingStation.SelectedItem.ToString() != (PtList[i].Split(','))[0]))
                    {
                       
                        Pointlist.Add(new Points(PtList[i].Split(',')));
                                               
                    }
                    //Pointlist.Add(new Points(PtList[i].Split(',')));
                }
                catch (Exception)
                {
                    
                }                
                
            }

            return Pointlist;
        }
                        
        private void SetupColumns()
        {           
            this.clnSiteName1.AspectGetter = delegate (object x) { return ((Points)x).SiteID; ; };
            this.clnEastings1.AspectGetter = delegate (object x) { return ((Points)x).Eastings; ; };
            this.clnNorthings1.AspectGetter = delegate (object x) { return ((Points)x).Northings; ; };           
        }

        private void SetupDragAndDrop()
        {

            // Make each listview capable of dragging rows out
            this.olvPointListTree1.DragSource = new SimpleDragSource();


            // Make each listview capable of accepting drops.
            // More than that, make it so it's items can be rearranged
            this.olvPointListTree1.DropSink = new RearrangingDropSink(true);

            // For a normal drag and drop situation, you will need to create a SimpleDropSink
            // and then listen for ModelCanDrop and ModelDropped events
        }

        private void btnDiscard_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void olvPointListTree_SelectedIndexChanged(object sender, EventArgs e)
        {

        }        

        private void CreateTraversePath_Load(object sender, EventArgs e)
        {
            foreach (var item in cbxPointlist)
            {
                cbxStartingStation.Items.Add(item.SiteID);
                cbxClosingStation.Items.Add(item.SiteID);
            }
            try{cbxStartingStation.SelectedIndex = 0;cbxClosingStation.SelectedIndex = 1;}catch (Exception){}
            
        }

        void cbxSelectionChanges(MetroSuite.MetroComboBox cbx)
        {
            MetroSuite.MetroComboBox cbxToChange;
                       

            cbxToChange = (cbx.Name == nameof(cbxStartingStation)) ? cbxClosingStation : cbxStartingStation;
            
            cbxSelected = cbx.SelectedItem.ToString();
            cbxToChange.Items.Clear();

            if (clicked)
            {
                foreach (var item in cbxPointlist)
                {
                    if (item.SiteID != cbxSelected)
                    {
                        cbxToChange.Items.Add(item.SiteID);
                    }
                }
            }

            clicked = false;

            try{cbxToChange.SelectedIndex = 0;}catch (Exception){}
            
        }
        string cbxSelected = "";
        private void cbxStartingStation_SelectedIndexChanged(object sender, EventArgs e)
        {

            //cbxSelectionChanges((MetroSuite.MetroComboBox)sender);
            olvPointListTree1.Items.Clear();
            olvPointListTree1.SetObjects(AllRoverList);
        }

        bool clicked = false;
        private void cbxStartingStation_Click(object sender, EventArgs e)
        {
            clicked = true;
        }

        
        private void btnApply_Click(object sender, EventArgs e)
        {
            traversPathPoints = new List<Points>();

            //Get the Starting Station
            foreach (var item in cbxPointlist)
            {
                if (cbxStartingStation.SelectedItem.ToString() == item.SiteID)
                {
                    traversPathPoints.Add(new Points(item));
                    break;
                }
            }
            
            //Get all other points
            foreach (var item in cbxPointlist)
            {
                for (int i = 0; i < olvPointListTree1.Items.Count; i++)
                {                    
                    if (olvPointListTree1.Items[i].SubItems[0].Text == item.SiteID)
                    {
                        traversPathPoints.Add(new Points(item));
                        break;
                    }
                }
            }
            
            //Get the Closing Station 
            foreach (var item in cbxPointlist)
            {
                if (cbxClosingStation.SelectedItem.ToString() == item.SiteID)
                {
                    traversPathPoints.Add(new Points(item));
                    break;
                }
            }
            
            var cadas = new CadastralSolution();
            cadas.solution(traversPathPoints);

            GC.Collect();
            MainScreen.ProcessResults.IsCadastralDone = true;
            MainScreen.ProcessResults.btnTraversePath.Visible = true;

            //Show results
            MainScreen.ProcessResults.rtbxResults.RtfText = GNSS_Functions.Beacon;
            
        }
    }
}
