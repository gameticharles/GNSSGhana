using ghGPS.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ghGPS.Classes;
using ghGPS.Classes.CoordinateSystems;
using ghGPS.Classes.CoordinateSystems.Transformations;

namespace ghGPS.Classes
{
    /// <summary>
    /// Process data report factory
    /// Contains functions, methods and global variables for processing the report output
    /// </summary>
    public class ProcessedSolution
    {
        /// <summary>
        /// Initialize Processed Solution
        /// </summary>
        public ProcessedSolution()
        {

        }

        //Declare variables 
        static StringBuilder sbPointList = new StringBuilder();
        static StringBuilder sbPointsLL = new StringBuilder();
        static StringBuilder sbPointsWGS84 = new StringBuilder();
        static StringBuilder sbPointsLatLon = new StringBuilder();
        static StringBuilder ALLPointlist = new StringBuilder();
        static StringBuilder SummaryList = new StringBuilder();

        static string baseSiteID = "";        

        /// <summary>
        /// Get solution from base and rover data
        /// </summary>
        /// <param name="baseCoorFile">Path to base file</param>
        /// <param name="baseEditedID">Base site ID </param>
        /// <param name="baseSiteHeight">Base Antenna Height</param>
        /// <param name="roversFiles">List of path to rover files</param>
        /// <param name="roversIDs">List of rover site IDs</param>
        /// <param name="roverAntenaHeight">List of rover antenna height</param>
        public void solutions(string baseCoorFile, string baseEditedID, string baseSiteHeight, string[] roversFiles, string[] roversIDs, string[] roverAntenaHeight)
        {
            sbPointList = new StringBuilder();
            sbPointsLL = new StringBuilder();
            sbPointsWGS84 = new StringBuilder();
            sbPointsLatLon = new StringBuilder();
            ALLPointlist = new StringBuilder();
            SummaryList = new StringBuilder();

            var rtbx = new RichTextBox();
            rtbx.Font = new System.Drawing.Font("Courier New", float.Parse("9.5"));

            #region POINT LIST HEADER
            //header info
            sbPointList.AppendLine("GHANA GNSS" + new String(' ', 48) + "PROCESSED POINT LIST");
            sbPointList.AppendLine(new String('-', 78));


            //Full project path formating
            string fullPath = GNSS_Functions.ProjectSolutionPath;
            string remPath = "";
            if (fullPath.Length > 59)
            {
                remPath = fullPath.Substring(59);
                fullPath = fullPath.Substring(0, 59);
            }

            //Add project path
            sbPointList.AppendLine("Project:" + new String(' ', 11) + fullPath);
            if (remPath != "") sbPointList.AppendLine(remPath);

            sbPointList.AppendLine();

            string coordSys = (MainScreen.createProject.cbxUnits.SelectedItem.ToString() == "Gold Cost Feet" || MainScreen.createProject.cbxUnits.SelectedItem.ToString() == "INT Feet" ? "Ghana National Grid" : "Ghana Meter Grid");
            sbPointList.AppendLine("Coordinate System: " + coordSys + " [Transverse Mercator]");
            sbPointList.AppendLine("Time Format:       UTC");
            sbPointList.AppendLine();

            sbPointList.AppendLine("Total Points:      " + (1 + roversFiles.Length).ToString("000") + new String(' ', 29) + "Units: " + new String(' ', 20 - UnitsTYPE.Length) + UnitsTYPE);

            //sbPointList.AppendLine(new String('-', 78));   //Count number of fix and float 

            sbPointList.AppendLine();

            sbPointList.AppendLine("POINTS" + new String(' ', 13) + "PROCESS DATE" + new String(' ', 16) + "SOURCE" + new String(' ', 18) + "CONTROL");
            sbPointList.AppendLine(new String('-', 78));

            string newBaseID = baseEditedID.Length > 19 ? baseEditedID.Remove(0, 18) : baseEditedID;

            //---------------------------POINTS HEADER-----------------------------------------
            string PointsLL = new String(' ', 19 - "POINT ID,".Length) + "POINT ID," + new String(' ', 14 - "EASTINGS,".Length) + "EASTINGS," + new String(' ', 14 - "NORTHINGS,".Length) + "NORTHINGS," + new String(' ', 11 - "HEIGHT,".Length) + "HEIGHT";
            string PointsLatLon = new String(' ', 19 - "POINT ID,".Length) + "POINT ID," + new String(' ', 21 - "LATIITUDE,".Length) + "LATIITUDE," + new String(' ', 21 - "LONGITUDE,".Length) + "LONGITUDE," + new String(' ', 11 - "HEIGHT,".Length) + "HEIGHT";

            //sbPointsLL.AppendLine(PointsLL);
            //sbPointsLL.AppendLine(new String('-', 57));
            //sbPointsWGS84.AppendLine(PointsLL);
            //sbPointsWGS84.AppendLine(new String('-', 57));
            //sbPointsLatLon.AppendLine(PointsLatLon);
            //sbPointsLatLon.AppendLine(new String('-', 71));

            CreatePointsHeader(sbPointsLL, coordSys + " [Transverse Mercator]", UnitsTYPE, roversFiles.Length, PointsLL);
            CreatePointsHeader(sbPointsWGS84, "WGS 84" + " [Universal Transverse Mercator]", "Degree/Meter", roversFiles.Length, PointsLL);
            CreatePointsHeader(sbPointsLatLon, coordSys + " [Geographic]", "Meter", roversFiles.Length, PointsLatLon);
            #endregion POINT LIST HEADER

            #region POINT SUMMARY HEADER
            //header info
            SummaryList.AppendLine("GHANA GNSS" + new String(' ', 45) + "PROCESSED POINT SUMMARY");
            SummaryList.AppendLine(new String('-', 78));


            //Add project path
            SummaryList.AppendLine("Project:" + new String(' ', 11) + fullPath);
            if (remPath != "") SummaryList.AppendLine(remPath);

            SummaryList.AppendLine();

            SummaryList.AppendLine("Coordinate System: " + (MainScreen.createProject.cbxUnits.SelectedItem.ToString() == "Gold Cost Feet" || MainScreen.createProject.cbxUnits.SelectedItem.ToString() == "INT Feet" ? "Ghana National Grid" : "Ghana Meter Grid") + " [Transverse Mercator]");
            SummaryList.AppendLine("Date Processed:    " + GNSS_Functions.currentdateTime() + " (UTC)");
            SummaryList.AppendLine();

            SummaryList.AppendLine("Total Points:      " + (1 + roversFiles.Length).ToString("000") + new String(' ', 29) + "Units: " + new String(' ', 20 - UnitsTYPE.Length) + UnitsTYPE);

            //sbPointList.AppendLine(new String('-', 78));   //Count number of fix and float 

            SummaryList.AppendLine();

            SummaryList.AppendLine("VECTOR/OCC." + new String(' ', 8) + "SOLUTION" + new String(' ', 16) + "LENGTH" + new String(' ', 7) + "USED" + new String(' ', 5) + "RATIO" + new String(' ', 5) + "DOP");
            SummaryList.AppendLine(new String('-', 78));
            
            #endregion POINT SUMMARY HEADER

            baseSiteID = baseEditedID;
                      
            ////Read the RINEX File
            using (var waiting = new WaitLoadingForm())
            {
                waiting.OpenedForm = "Process Data";
                waiting.RoversFiles = roversFiles;
                waiting.RoverIDs = roversIDs;
                waiting.sbPointList = sbPointList;
                waiting.ShowDialog();
            }

            rtbx.Text = sbPointsLL.ToString();
            GNSS_Functions.PointsLL = rtbx.Rtf;
            rtbx.Text = sbPointsWGS84.ToString();
            GNSS_Functions.PointsWGS84 = rtbx.Rtf;
            rtbx.Text = sbPointsLatLon.ToString();
            GNSS_Functions.PointsLatLon = rtbx.Rtf;
            rtbx.Text = sbPointList.ToString();
            GNSS_Functions.PointList = rtbx.Rtf;
            rtbx.Text = ALLPointlist.ToString();
            GNSS_Functions.ALLPointlist = rtbx.Rtf;
            rtbx.Text = SummaryList.ToString();
            GNSS_Functions.PointSummary = rtbx.Rtf;

            rtbx.Dispose();
            
        }

        private void CreatePointsHeader(StringBuilder sb, string coordinateSys, string _units, int totalCount, string theHeader)
        {
            //Header info
            sb.AppendLine("GHANA GNSS" + new String(' ', 52) + "PROCESSED POINTS");
            sb.AppendLine(new String('-', 78));
            
            //Full project path formating
            string fullPath = GNSS_Functions.ProjectSolutionPath;
            string remPath = "";
            if (fullPath.Length > 59)
            {
                remPath = fullPath.Substring(59);
                fullPath = fullPath.Substring(0, 59);
            }

            //Add project path
            sb.AppendLine("Project:" + new String(' ', 11) + fullPath);
            if (remPath != "") sb.AppendLine(remPath);

            sb.AppendLine();

            sb.AppendLine("Coordinate System: " + coordinateSys);
            sb.AppendLine("Time Format:       UTC");
            sb.AppendLine();

            sb.AppendLine("Total Points:      " + (1 + totalCount).ToString("000") + new String(' ', 29) + "Units: " + new String(' ', 20 - _units.Length) + _units);
                        
            sb.AppendLine();

            sb.AppendLine(theHeader);
            sb.AppendLine(new String('-', 78));

        }

        static string UnitsTYPE = GNSS_Functions.UnitsTYPE;
        
        static double BaseE;
        static double BaseN;

        private void SetSummary()
        {

        }

        public static bool IsBaseNotDone { get; set; } = false;
     
        public static void CreateListItem(StringBuilder sbPointList, string SiteID, double[] WGS84_XYZ)
        {
            if (WGS84_XYZ[0] == 0 || WGS84_XYZ[0] == 0 || WGS84_XYZ[0] == 0)
            {
                return;
            }

            //Create new coordinate factory
            CoordinateSystemFactory cFac = new CoordinateSystemFactory();

            var IGeoncenTarget = cFac.CreateGeocentricCoordinateSystem(" ", (MainScreen.GHCRS as IProjectedCoordinateSystem).HorizontalDatum, (MainScreen.GHCRS as IProjectedCoordinateSystem).LinearUnit, (MainScreen.GHCRS as IProjectedCoordinateSystem).GeographicCoordinateSystem.PrimeMeridian);
            var IGeoncenSource = cFac.CreateGeocentricCoordinateSystem(" ", (MainScreen.WGS84 as IProjectedCoordinateSystem).HorizontalDatum, (MainScreen.WGS84 as IProjectedCoordinateSystem).LinearUnit, (MainScreen.WGS84 as IProjectedCoordinateSystem).GeographicCoordinateSystem.PrimeMeridian);

            //==============TO UTM===============
            MainScreen.trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems(IGeoncenSource, (MainScreen.WGS84 as IProjectedCoordinateSystem).GeographicCoordinateSystem);
            var utmLonLatH = MainScreen.trans.MathTransform.Transform(WGS84_XYZ);
            MainScreen.trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((MainScreen.WGS84 as IProjectedCoordinateSystem).GeographicCoordinateSystem, (MainScreen.WGS84 as IProjectedCoordinateSystem));
            var utm = MainScreen.trans.MathTransform.Transform(utmLonLatH);
            //===================================

            //================TO LOCAL===========
            MainScreen.trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((MainScreen.WGS84 as IProjectedCoordinateSystem), (MainScreen.GHCRS as IProjectedCoordinateSystem));

            var _ENU = MainScreen.trans.MathTransform.Transform(new double[] { utm[0], utm[1], utm[2] / GNSS_Functions.CovertFactor });
                       

            MainScreen.trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((MainScreen.GHCRS as IProjectedCoordinateSystem), (MainScreen.GHCRS as IProjectedCoordinateSystem).GeographicCoordinateSystem);
            var LonLatH = MainScreen.trans.MathTransform.Transform(_ENU);

            //Must be removed????????????????????????????????????????????????????????????????
            if (IsBaseNotDone)
            {
                _ENU = new double[] { double.Parse(MainScreen.GNSSDataImport.lblInputEastings.Text), double.Parse(MainScreen.GNSSDataImport.lblInputNorthings.Text), double.Parse(MainScreen.GNSSDataImport.lblInputHeight.Text) };
            }
            //===================================

            var wE = " "; var tmE = " "; var tmLat = " ";
            var wN = " "; var tmN = " "; var tmLon = " ";
            var wZ = " "; var tmZE = " "; var Conv = " ";
            var wZone = " "; var tmZO = " "; var Scale = " ";

            wE = utm[0].ToString("#.000");
            wN = utm[1].ToString("#.000");
            wZ = utm[2].ToString("#.000");
            wZone = GNSS_Functions.GetZone;

            //var tm = GNSS_Functions.approxPos;
            var _tmE = _ENU[0];
            var _tmN = _ENU[1];
            var _tmZE = _ENU[2];

            //Check Units
            tmE = _tmE.ToString("#.000");
            tmN = _tmN.ToString("#.000");
            tmZE = _tmZE.ToString("#.000");
            tmZO = _tmZE.ToString("#.000");

            tmLat = GNSS_Functions.ConvertToDMSString(LonLatH[1], true, true);
            tmLon = GNSS_Functions.ConvertToDMSString(LonLatH[0], false, true);

            Conv = GNSS_Functions.GetConvergence;
            Scale = double.Parse(GNSS_Functions.GetGridScale).ToString("0.00000000");

            sbPointsLL.AppendLine(new String(' ', 18 - SiteID.Length) + SiteID + "," + new String(' ', 13 - tmE.Length) + tmE + "," + new String(' ', 13 - tmN.Length) + tmN + "," + new String(' ', 10 - tmZO.Length) + tmZO);
            sbPointsWGS84.AppendLine(new String(' ', 18 - SiteID.Length) + SiteID + "," + new String(' ', 13 - wE.Length) + wE + "," + new String(' ', 13 - wN.Length) + wN + "," + new String(' ', 10 - wZ.Length) + wZ);
            sbPointsLatLon.AppendLine(new String(' ', 18 - SiteID.Length) + SiteID + "," + new String(' ', 20 - tmLon.Length) + tmLon + "," + new String(' ', 20 - tmLat.Length) + tmLat + "," + new String(' ', 10 - wZ.Length) + wZ);
            ALLPointlist.AppendLine(SiteID + "," + tmE + "," + tmN);

            if (UnitsTYPE == "Gold Cost Feet")
            {
                UnitsTYPE = "GH Feet";
            }

            string ProcessedVector = "";
            string SummaryBands = "";
            string baseLineDistance = "";

            if (IsBaseNotDone)
            {
                
                if (GNSS_Functions.UseAprroximatePos == "Input")
                {
                    ProcessedVector = "User Input                 CTRL";
                }
                else if (GNSS_Functions.UseAprroximatePos == "Yes") //Use RINEX header position
                {
                    ProcessedVector = "RINEX header Position      CTRL";
                }
                else if (GNSS_Functions.UseAprroximatePos == "No") //Use average single position
                {
                    ProcessedVector = "Average Single Position    CTRL";
                }

                //Get Base Coordinate 
                BaseE = double.Parse(tmE);
                BaseN = double.Parse(tmN);

            }
            else
            {
                var FreqBands = "";
                if (GNSS_Functions.FreqBand == 0) //0 = L1, 1 = L1 + L2, 2 = L1 + L2 + L5, 3 = L1 + L5
                {
                    FreqBands = "L1";
                }
                else if (GNSS_Functions.FreqBand == 1)
                {
                    FreqBands = "L1 + L2";
                }
                else if (GNSS_Functions.FreqBand == 2)
                {
                    FreqBands = "L1 + L2 + L5";
                }
                else if (GNSS_Functions.FreqBand == 3)
                {
                    FreqBands = "L1 + L5";
                }

                ProcessedVector = "Vector (" + FreqBands + " " + GNSS_Functions.SolutionFix + ")";
                SummaryBands = GNSS_Functions.SolutionFix + " (" + FreqBands + ")";

                //====Configuring Summary list==== 
                //Compute Distance
                baseLineDistance = (Math.Sqrt(Math.Pow((BaseE - double.Parse(tmE)), 2) + Math.Pow((BaseN - double.Parse(tmN)), 2))).ToString("#.000");
                var Ratio = "4.0";    // Ratio
                var Used = "68.98%";  // Obs. used
                var DOP = "0.017";    // Dilution of Precision

                SummaryList.AppendLine(GNSS_Functions.BaseSiteID + " - " + SiteID);
                SummaryList.AppendLine(new String(' ', 5) + "01" + new String(' ', 12) + SummaryBands + new String(' ', 30 - (baseLineDistance.Length + SummaryBands.Length)) + baseLineDistance + new String(' ', 5) + Used + new String(' ', 10 - Ratio.Length) + Ratio);
            }

            //Enter base Results
            sbPointList.AppendLine(SiteID + new String(' ', 19 - SiteID.Length) + GNSS_Functions.currentdateTime() + new String(' ', 6) + ProcessedVector);
            sbPointList.AppendLine(new String(' ', 7) + "WGS84 (Meters) " + new String(' ', 8) + "GH  Geographic(Degree)" + new String(' ', 9) + "GH TM" + "(" + UnitsTYPE + ")");
            sbPointList.AppendLine(new String(' ', 7) + "E:" + new String(' ', 13 - wE.Length) + wE + new String(' ', 8) + "Lat: " + tmLat + new String(' ', 8) + "E:" + new String(' ', 13 - tmE.Length) + tmE);
            sbPointList.AppendLine(new String(' ', 7) + "N:" + new String(' ', 13 - wN.Length) + wN + new String(' ', 8) + "Lon: " + tmLon + new String(' ', 8) + "N:" + new String(' ', 13 - tmN.Length) + tmN);
            sbPointList.AppendLine(new String(' ', 7) + "E.Hgt: " + new String(' ', 8 - wZ.Length) + wZ + new String(' ', 8) + "Conv:" + new String(' ', 18 - Conv.Length) + Conv + new String(' ', 8) + "E.Hgt: " + new String(' ', 8 - tmZE.Length) + tmZE);
            sbPointList.AppendLine(new String(' ', 7) + "Zone: " + wZone + new String(' ', 9 - wZone.Length) + new String(' ', 8) + "Grid Scale:" + new String(' ', 12 - Scale.Length) + Scale + new String(' ', 8) + "Orth:" + new String(' ', 10 - tmZO.Length) + tmZO);
            sbPointList.AppendLine();
        }
        
    }
}
