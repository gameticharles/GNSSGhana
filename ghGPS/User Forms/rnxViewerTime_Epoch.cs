using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace ghGPS.User_Forms
{
    public partial class rnxViewerTime_Epoch : UserControl
    {
        public rnxViewerTime_Epoch()
        {
            InitializeComponent();
        }

        private void rnxViewerTime_Epoch_Load(object sender, EventArgs e)
        {

            LineItem myPoint;

            // Plot Sun Moon Altitude path
            var Time_Epoch = Time_EpochChart.GraphPane;
            Time_Epoch.CurveList.Clear();

            // Set the titles and axis labels
            Time_Epoch.Title.Text = "Altitude of Sun and Moon";
            Time_Epoch.XAxis.Title.Text = "Hour";
            Time_Epoch.YAxis.Title.Text = "Altitude";


            // Make up some data points based 
            var MyPoints = new PointPairList();

            // Add Points to plot
            for (int i = 0; i < 10; i++)
            {
                MyPoints.Add(i,Math.Sin(60*(i/10)));
            }

            myPoint = Time_Epoch.AddCurve("Moon Path", MyPoints, Color.DarkGray, SymbolType.Circle);
            // Fill the symbols with DarkGray
            myPoint.Symbol.Fill = new Fill(Color.DarkGray);
            myPoint.Symbol.Size = 12;

            //Disable Legend
            Time_Epoch.Legend.IsVisible = false;

            // Display the Y zero line
            Time_Epoch.XAxis.MajorGrid.IsZeroLine = false;
            Time_Epoch.YAxis.MajorGrid.IsZeroLine = false;

            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            Time_EpochChart.AxisChange();

            // Make sure the Graph gets redrawn
            Time_EpochChart.Invalidate();
        }

        private string Time_EpochChart_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            //// Get the PointPair that is under the mouse
            //PointPair pt = curve(iPt);

            //return "Moon at Distance of " + pt.Y.ToString("f2") + " Earth Radii" + "\n" + " on Day " + pt.X.ToString();
            return null;
        }
    }
}
