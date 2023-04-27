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
using System.Drawing.Printing;

namespace ghGPS.User_Forms
{
    public partial class ProcessResults : UserControl
    {
        public ProcessResults()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (GNSS_Functions.IsProcessed)
            {
                MainScreen.baseLineChart.BringToFront();
            }
            else
            {
                MainScreen.manualDataImport.BringToFront();               
            }
            
        }

        public bool IsCadastralDone = false;
        private void btnCadastral_Click(object sender, EventArgs e)
        {            
            if (IsCadastralDone)
            {
                GNSS_Functions.expandEffect(pnlCadastralReport, 250);
                pnlPoints.Height = 46;
            }
            else
            {
                using (var crTravers = new CreateTraversePath())
                {
                    if (crTravers.ShowDialog() == DialogResult.OK)
                    {

                    }

                }
            }
            
        }



        private void btnPointList_Click(object sender, EventArgs e)
        {
            rtbxResults.RtfText = GNSS_Functions.PointList;
        }
                

        private void btnSummaryList_Click(object sender, EventArgs e)
        {
            rtbxResults.RtfText = GNSS_Functions.PointSummary;
        }

        private void btnPoints_Click_1(object sender, EventArgs e)
        {
            GNSS_Functions.expandEffect(pnlPoints, 178);
            pnlCadastralReport.Height = 46;
        }

        private void btnMenuEdit_MouseEnter(object sender, EventArgs e)
        {
            btnTraversePath.BackColor = Color.FromArgb(100, 160, 199);
            btnCadastral.BackColor = Color.FromArgb(100, 160, 199);
        }

        private void btnMenuEdit_MouseLeave(object sender, EventArgs e)
        {
            btnTraversePath.BackColor = Color.FromArgb(90, 139, 196);
            btnCadastral.BackColor = Color.FromArgb(90, 139, 196);
        }

        private void btnTraversePath_Click(object sender, EventArgs e)
        {
            using (var crTravers = new CreateTraversePath())
            {
                if (crTravers.ShowDialog() == DialogResult.OK)
                {

                }

            }
        }

        private void btnLocalTM_Click(object sender, EventArgs e)
        {
            rtbxResults.RtfText = GNSS_Functions.PointsLL;
        }

        private void btnLocalGeo_Click(object sender, EventArgs e)
        {
            rtbxResults.RtfText = GNSS_Functions.PointsLatLon;
        }

        private void btnWGS84_Click(object sender, EventArgs e)
        {
            rtbxResults.RtfText = GNSS_Functions.PointsWGS84;
        }

        private void btnBeacon_Click(object sender, EventArgs e)
        {
            rtbxResults.RtfText = GNSS_Functions.Beacon;
        }

        private void btnDistBear_Click(object sender, EventArgs e)
        {
            rtbxResults.RtfText = GNSS_Functions.DistanceBearing;
        }

        private void btnMapData_Click(object sender, EventArgs e)
        {
            rtbxResults.RtfText = GNSS_Functions.MapData;
        }

        private void btnAreaComp_Click(object sender, EventArgs e)
        {
            rtbxResults.RtfText = GNSS_Functions.AreaComputaion;
        }

        private void btnPlanData_Click(object sender, EventArgs e)
        {
            rtbxResults.RtfText = GNSS_Functions.PlanData;
        }

        private void rtbxResults_ZoomChanged(object sender, EventArgs e)
        {
            
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            //Save the results before exit
            GNSS_Functions.ClearCreateNewProject();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {            
            rtbxResults.ShowPrintDialog();            
        }

        private void btnExportResult_Click(object sender, EventArgs e)
        {
            //Export Result
            using (var ep = new ExportResults())
            {
                ep.ShowDialog();
            }
        }

        private void rtbxResults_RtfTextChanged(object sender, EventArgs e)
        {
            lblRoverCounts.Text = rtbxResults.DocumentLayout.GetPageCount().ToString("00");
        }
    }
}
