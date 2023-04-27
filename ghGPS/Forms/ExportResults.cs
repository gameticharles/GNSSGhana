using DevExpress.XtraRichEdit;
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
    
    public partial class ExportResults : MetroSuite.MetroForm
    {
        public ExportResults()
        {
            InitializeComponent();
        }

        private RichEditControl rtbx = new RichEditControl();
        private void btnExport_Click(object sender, EventArgs e)
        {
            
            
            if (chbxArea.Checked)
            {
                rtbx.RtfText = GNSS_Functions.AreaComputaion;
                rtbx.ExportToPdf(GNSS_Functions.CadastralReportPath + "\\Area Computaion.pdf");
            }

            if (chbxBeacon.Checked)
            {
                rtbx.RtfText = GNSS_Functions.Beacon;
                rtbx.ExportToPdf(GNSS_Functions.CadastralReportPath + "\\Beacon.pdf");
            }

            if (chbxDistance_Bearing.Checked)
            {
                rtbx.RtfText = GNSS_Functions.DistanceBearing;
                rtbx.ExportToPdf(GNSS_Functions.CadastralReportPath + "\\Distance & Bearing.pdf");
            }

            if (chbxPlanData.Checked)
            {
                rtbx.RtfText = GNSS_Functions.PlanData;
                rtbx.ExportToPdf(GNSS_Functions.CadastralReportPath + "\\Plan Data.pdf");
            }

            if (chbxMapData.Checked)
            {
                rtbx.RtfText = GNSS_Functions.MapData;
                rtbx.ExportToPdf(GNSS_Functions.CadastralReportPath + "\\Map Data.pdf");
            }

            if (chbxPointList.Checked)
            {
                rtbx.RtfText = GNSS_Functions.PointList;
                rtbx.ExportToPdf(GNSS_Functions.ProcessedReportPath + "\\Point List.pdf");
            }

            if (chbxSummaryList.Checked)
            {
                rtbx.RtfText = GNSS_Functions.PointSummary;
                rtbx.ExportToPdf(GNSS_Functions.ProcessedReportPath + "\\Summary List.pdf");
            }

            var rtbx1 = new RichTextBox();

            var swUTM = new StreamWriter(GNSS_Functions.ProcessedReportPath + "\\WGS 84 (UTM).csv");
            var swGEO = new StreamWriter(GNSS_Functions.ProcessedReportPath + "\\WGS 84 (Geo).csv");
            var swLocal = new StreamWriter(GNSS_Functions.ProcessedReportPath + "\\Local Points.csv");
            rtbx1.Rtf = GNSS_Functions.PointsWGS84;
            var wg = rtbx1.Text.Split('\n');
            rtbx1.Rtf = GNSS_Functions.PointsLatLon;
            var latlon = rtbx1.Text.Split('\n');
            rtbx1.Rtf = GNSS_Functions.PointsLL;
            var ll = rtbx1.Text.Split('\n');
            

            for (int i = 0; i < wg.Length; i++)
            {
                if (i != 1)
                {
                    swUTM.WriteLine(wg[i]);
                    swGEO.WriteLine(latlon[i]);
                    swLocal.WriteLine(ll[i]);
                }
            }

            swUTM.FlushAsync();
            swGEO.FlushAsync();
            swLocal.FlushAsync();

            //swUTM.Close();
            //swGEO.Close();
            //swLocal.Close();
                    
        }
    }
}
