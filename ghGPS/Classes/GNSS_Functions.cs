using GongSolutions.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using ghGPS.Classes;
using static ghGPS.Classes.GNSSLib;
using System.Text.RegularExpressions;
using ghGPS.Forms;
using MetroSuite;
using System.Drawing;
using ghGPS.Classes.CoordinateSystems;
using ghGPS.Classes.CoordinateSystems.Transformations;
using MapWinGIS;

namespace ghGPS
{
    public class GNSS_Functions
    {
        #region Label Shapefile
        //Label Shapefile and Symbol
        public static void labelShapefile(Shapefile shpfile, string fieldName, int FontSize, tkDefaultPointSymbol symbol)
        {
            shpfile.Labels.Generate("[" + fieldName + "]", tkLabelPositioning.lpCentroid, true);
            shpfile.Labels.AvoidCollisions = true;
            shpfile.Labels.AutoOffset = true;
            shpfile.Labels.InboxAlignment = tkLabelAlignment.laCenter;
            shpfile.Labels.FrameVisible = false;
            shpfile.Labels.FrameType = tkLabelFrameType.lfPointedRectangle;
            //shpfile.Labels.FrameOutlineColor = LabelColor
            //shpfile.Labels.FrameBackColor2 = LabelColor1
            //shpfile.Labels.FrameTransparency = 100
            shpfile.Labels.FontBold = true;
            
            shpfile.Labels.FontSize = FontSize;

            shpfile.DefaultDrawingOptions.SetDefaultPointSymbol(symbol);
        }
        #endregion

        #region Create Shapefile

        #endregion

        #region Create New GH local CRS
        public static IProjectedCoordinateSystem CreateTMCoordSys(string Name, long AuthorityCode, double[] toWGS84, string AreaOfUse)
        {
            CoordinateSystemFactory cFac = new CoordinateSystemFactory();

            IEllipsoid ellipsoid = cFac.CreateFlattenedSphere("War Office", 6378299.996, 296, Classes.CoordinateSystems.LinearUnit.GoldCoastFoot);

            IHorizontalDatum datum = cFac.CreateHorizontalDatum("Accra", Classes.CoordinateSystems.DatumType.HD_Geocentric, ellipsoid,
                new Wgs84ConversionInfo(toWGS84[0], toWGS84[1], toWGS84[2], toWGS84[3], toWGS84[4], toWGS84[5], toWGS84[6], toWGS84[7], toWGS84[8], toWGS84[9], AreaOfUse));

            IGeographicCoordinateSystem gcs = cFac.CreateGeographicCoordinateSystem("Accra", Classes.CoordinateSystems.AngularUnit.Degrees, datum,
                PrimeMeridian.Greenwich, new AxisInfo("Lon", AxisOrientationEnum.East), new AxisInfo("Lat", AxisOrientationEnum.North));

            List<ProjectionParameter> parameters = new List<ProjectionParameter>(5);
            parameters.Add(new ProjectionParameter("latitude_of_origin", 4.666666667));
            parameters.Add(new ProjectionParameter("central_meridian", -1));
            parameters.Add(new ProjectionParameter("scale_factor", 0.99975));
            parameters.Add(new ProjectionParameter("false_easting", 900000));
            parameters.Add(new ProjectionParameter("false_northing", 0));

            IProjection projection = cFac.CreateProjection("Transverse Mercator", "Transverse_Mercator", parameters);

            IProjectedCoordinateSystem coordsys = cFac.CreateProjectedCoordinateSystem(Name, gcs, projection, Classes.CoordinateSystems.LinearUnit.GoldCoastFoot, new AxisInfo("East", AxisOrientationEnum.East), new AxisInfo("North", AxisOrientationEnum.North));

            return coordsys;
        }
        #endregion

        #region DECLARATIONS
        /// <summary>
        /// Filter Text, Video, or all files
        /// </summary>
        public static string NormalFilter = "Text Files(*.txt)|*.txt|Video Files|*.avi, *.wmv|All Files(*.*)|*.*";

        /// <summary>
        /// Filter RINEX OBSERVATION FILES .*O, .*D, .*OBS
        /// </summary>
        public static string FilterProject = "GNSS Ghana Project file|*.ggp|All Files(*.*)|*.*";
        
        /// <summary>
        /// Filter RINEX OBSERVATION FILES .*O, .*D, .*OBS
        /// </summary>
        public static string FilterObservaton = "RINEX O-Files|*.*O|RINEX D-Files|*.*D|RINEX OBS Files(*.obs)|*.obs";

        /// <summary>
        /// Filter All Navigation message files
        /// </summary>
        public static string FilterNavigation = "RINEX NAV Files|*.nav|RINEX N-Files|*.*N|RINEX P-Files|*.*P|RINEX G-Files|*.*G|RINEX H-Files|*.*H|RINEX Q-Files|*.*Q";

        /// <summary>
        /// Filter Precise Ephemeris Files an
        /// </summary>
        public static string FilterPreciseEphemeris = "Precise Ephemeris Files|*.eph|SP3 Files|*.sp3|RINEX Clock files|*.clk";

        /// <summary>
        /// Filter Ionospheric Correction Files
        /// </summary>
        public static string FilterIonosphere = "I-Files|*.*i|IONEX Files|*.ionex";

        private static ShellItem[] selectedItems;

        public static ShellItem[] SelectedItems { get => selectedItems; set => selectedItems = value; }

        public static bool IsCadastralIncluded = true;

        /// <summary>
        /// Get Output Units
        /// </summary>
        public static string UnitsTYPE { get; set; }

        public static double CovertFactor { get; set; }

        public static string OFDheaderText = "Open: ";

        public static string ALLPointlist = "";
        public static string PointsLL = "";
        public static string PointsWGS84 = "";
        public static string PointsLatLon = "";

        public static string PointList = "";
        public static string PointSummary = "";

        public static string CrsSelectdParameters = "";
        public static string CrsSelectdName = "";

        public static string Beacon = "";
        public static string DistanceBearing = "";
        public static string PlanData = "";
        public static string AreaComputaion = "";
        public static string MapData = "";

        public static string ProjectPath = "";
        public static string ProjectName = "";

        public static string UseAprroximatePos = "";

        public static MetroForm MainFrm = new MetroForm();
               
        /// <summary>
        /// Get Zone of a point coordinate
        /// </summary>
        public static string GetZone { get; set; }

        public static string GetGridScale { get; set; }

        public static string GetConvergence { get; set; }

        /// <summary>
        /// Get project solution path
        /// </summary>
        public static string ProjectSolutionPath { get => ProjectPath + "\\" + ProjectName + ".ggp"; }

        /// <summary>
        /// Get Processed GNSS report path
        /// </summary>
        public static string ProcessedReportPath { get => ProjectPath + "\\Processed Report"; }

        /// <summary>
        /// Get Cadastral Report path
        /// </summary>
        public static string CadastralReportPath { get => ProjectPath + "\\Cadastral Report"; }

        /// <summary>
        /// Get data folder path
        /// </summary>
        public static string DataPath { get => ProjectPath + "\\Data"; }

        public static double baseAntennaHeight = 0;

        public static string fileToConvert;
        public static string File2Convert
        {
            get
            {
                return fileToConvert;
            }
            set
            {
                fileToConvert = value;
            }
        }

        public static string _delimiter;
        public static string Delimiter
        {
            get
            {
                return _delimiter;
            }
            set
            {
                _delimiter = value;
            }
        }

        public static string Filter_Type;
        public static UserControl openedForm;
        public static double cellValue;

        public static string BaseSiteID { get; set; } = "";
        public static string BaseOBSFilePath { get; set; } = "";
        public static string BaseNAVFilePath { get; set; } = "";
        public static string BaseAntHeight { get; set; } = "";
        public static string BaseX { get; set; } = "0";
        public static string BaseY { get; set; } = "0";
        public static string BaseZ { get; set; } = "0";

        public static GridSystem GHCRS = new GridSystem();
        #endregion

        #region FILE FILTER
        public static string myFilter(string filter)
        {
            if (filter == "CSV")
            {
                filter = "XLS files (*.xls, *.xlt)|*.xls;*.xlt|XLSX files (*.xlsx, *.xlsm, *.xltx, *.xltm)|*.xlsx;*.xlsm;*.xltx;*.xltm|ODS files (*.ods, *.ots)|*.ods;*.ots|CSV files (*.csv, *.tsv)|*.csv";
            }
            else if (filter == "TXT")
            {
                filter = "TEXT files (*.txt)|*.txt";
            }
            else if (filter == "CSV_TXT")
            {
                filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All Files (*.*)|*.*";
            }
            else if (filter == "ALL FILES")
            {
                filter = "XLS files (*.xls)|*.xls;*.xlt|XLT files (*.xlt)|*.xlt|XLSX files (*.xlsx, *.xlsm, *.xltx, *.xltm)|*.xlsx;*.xlsm;*.xltx;*.xltm|ODS files (*.ods, *.ots)|*.ods;*.ots|CSV files (*.csv, *.tsv)|*.csv;*.tsv|HTML files (*.html, *.htm)|*.html;*.htm|Text files (*.txt)|*.txt|All Files (*.*)|*.*";
            }
            else
            {
                filter = "XLS files (*.xls)|*.xls|XLT files (*.xlt)|*.xlt|XLSX files (*.xlsx)|*.xlsx|ODS files (*.ods)|*.ods|CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All Files (*.*)|*.*";
            }

            return filter;
        }
        #endregion

        #region IMPORT TO TABLE
        public static void ImportPoints(UserControl frm, string theFilter)
        {
            //This function calls importation wizard
            FileToTable frmFile2Table = new FileToTable();

            openedForm = frm;
            Filter_Type = myFilter(theFilter);

            frmFile2Table.ShowDialog();
        }
        #endregion

        #region Count Rows
        public static object RowCount(DataGridView dgv)
        {
            //Return the number of rows in the dataGridView
            return dgv.RowCount;
        }
        #endregion

        #region CONVERT DMS TO DECIMAL DEGREE
        public static double DMS2DecDeg(string Deg, double Min, double Sec)
        {
            double DegDec = 0;

            DegDec = (Deg == "-0" || Deg == "-00" || Deg == "-000" || Convert.ToDouble(Deg) < 0) ? (Math.Abs(Convert.ToDouble(Deg)) + Min / 60 + Sec / 3600) * -1 : Convert.ToDouble(Deg) + Min / 60 + Sec / 3600;

            return DegDec;
        }

        #endregion

        #region CONVERT DMM TO DECIMAL DEGREE
        public static double DMM2DecDeg(double Deg, double Min)
        {
            double DegDec = 0;

            DegDec = (Deg < 0) ? (Math.Abs(Deg) + Min / 60) * -1 : Deg + Min / 60;

            return DegDec;
        }

        #endregion

        #region CONVERT DECIMAL DEGREE TO DD MM.MMM
        public static Array DegDEC2DMM(double DegDEC)
        {
            var absDegDec = Math.Abs(DegDEC);
            //DEG
            double Deg = Convert.ToDouble(Math.Truncate(absDegDec));
            //MIN
            double Min = (absDegDec - Deg) * 60;

            if (Min >= 60)
            {
                Min = 0;
                Deg = Deg + 1;
            }

            if (DegDEC < 0)
            {
                Deg = Deg * -1;
            }

            double[] DegDMM = new double[2];
            DegDMM[0] = Deg;
            DegDMM[1] = Min;

            return DegDMM;
        }

        #endregion

        #region CONVERT DECIMAL DEGREE TO DMS
        public static double[] DegDec2DMS(double DegDEC)
        {
            var absDegDec = Math.Abs(DegDEC);

            //DEG
            double Deg = Convert.ToDouble(Math.Truncate(absDegDec));
            //MIN
            double Min = Convert.ToDouble(Math.Truncate((absDegDec - Deg) * 60));
            //SEC
            double Sec = Math.Round((((absDegDec - Deg) * 60) - Min) * 60, 10);

            for (var i = 0; i <= 5; i++)
            {
                if (Sec > 59.995)
                {
                    Sec = 0;
                    Min = Min + 1;
                }
                if (Min > 59.995)
                {
                    Min = 0;
                    Deg = Deg + 1;
                }
            }

            if (DegDEC < 0)
            {
                Deg = Deg * -1;
            }

            double[] DegDMS = new double[3];
            DegDMS[0] = Deg;
            DegDMS[1] = Min;
            DegDMS[2] = Sec;

            return DegDMS;
        }

        #endregion

        #region Validate textbox
        public static void ValidateTbx(TextBox tbx, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) == false)
            {
                if (!char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;

                    tbx.Focus();
                    Microsoft.VisualBasic.Interaction.Beep();
                }
            }
        }

        public static void ValidateTbx(MetroSuite.MetroTextbox tbx, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) == false)
            {
                if (!char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;

                    tbx.Focus();
                    Microsoft.VisualBasic.Interaction.Beep();
                }
            }
        }
        #endregion

        #region Validate Min/Sec
        public static bool validateMinSec(int tbx)
        {
            if (tbx > 59 || tbx < 0)
            {
                MessageBox.Show("Please min/sec value can't exceed 59", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region ConvertToDMSString
        /// <summary>
        /// Get the DMS reading in the decimal angle to a string presentation
        /// </summary>
        /// <param name="DegDEC">The angle in decimal degree</param>
        /// <param name="lat">Indicate if it is the latitude value provided</param>
        /// <param name="IsLatLon">Is the reading provided part of LAT-LONG </param>
        /// <returns>Returns the a string representation of the reading</returns>
        public static string ConvertToDMSString(double DegDEC, bool lat, bool IsLatLon)
        {
            var absDegDec = Math.Abs(DegDEC);

            //DEG
            double Deg = Convert.ToDouble(Math.Truncate(absDegDec));
            //MIN
            double Min = Convert.ToDouble(Math.Truncate((absDegDec - Deg) * 60));
            //SEC
            double Sec = Math.Round((((absDegDec - Deg) * 60) - Min) * 60, 10);

            for (var i = 1; i <= 3; i++)
            {
                if (Sec >= 59.999)
                {
                    Sec = 0;
                    Min = Min + 1;
                }
                if (Min >= 59.999)
                {
                    Min = 0;
                    Deg = Deg + 1;
                }
            }

            var NSWE = "";

            if (IsLatLon)
            {
                if (DegDEC < 0)
                {
                    if (lat)
                    {
                        NSWE = "S  ";
                    }
                    else
                    {
                        NSWE = "W  ";
                    }

                }
                else
                {
                    if (lat)
                    {
                        NSWE = "N  ";
                    }
                    else
                    {
                        NSWE = "E  ";
                    }
                }
            }
            else
            {
                NSWE = "+ ";
                if (DegDEC < 0)
                {
                    NSWE = "- ";
                }
            }

            return NSWE + Deg.ToString("000") + " " + Min.ToString("00") + " " + Sec.ToString("00.00000");
        }

        #endregion ConvertToDMSString

        #region Processed Result View

        public static bool IsProcessed;
        public static void ShowResults()
        {
            
            MainScreen.ProcessResults.pnlPointList.Visible = IsProcessed;
            MainScreen.ProcessResults.pnlPoints.Visible = IsProcessed;
            MainScreen.ProcessResults.pnlSummary.Visible = IsProcessed;

            
            MainScreen.ProcessResults.btnTraversePath.Visible = !IsProcessed;
            MainScreen.ProcessResults.pnlCadastralReport.Height = 46;
            MainScreen.ProcessResults.IsCadastralDone = !IsProcessed;

            MainScreen.ProcessResults.BringToFront();
        }
        #endregion Processed Result View

        #region Clear Project
        /// <summary>
        /// Clear Create new project entries and go back to home page
        /// </summary>
        public static void ClearCreateNewProject()
        {
            // Go back to the recent or start menu
            MainScreen.recentStartScreen.BringToFront();

            //Reset global variables
            MainScreen.createProject.tbxProjectName.Text = "";
            MainScreen.createProject.tbxFolderPath.Text = "";
            MainScreen.createProject.cbxProjectType.SelectedIndex = -1;

            MainScreen.createProject.chbxIncludeReport.Checked = false;
            //preset settings
            MainScreen.createProject.cbxUnits.SelectedIndex = 1;
            MainScreen.createProject.cbxProjectType.SelectedIndex = 0;

            //Hide the title 
            MainScreen.MainScreenTitle.Text = "GNSS Ghana";
            MainScreen.MainScreenTitle.Refresh();

            //Show Settings button
            MainScreen.btnSettings.Show();


            //===============GNSS IMPORT CONTROLS===================
            //Clear Import Tables
            MainScreen.GNSSDataImport.dgvRovers.Rows.Clear();
            MainScreen.GNSSDataImport.lblRoverCounts.ResetText();
            MainScreen.GNSSDataImport.tbxBasePath.ResetText();
            MainScreen.manualDataImport.dgvPoints.Rows.Clear();

            //Load All Data
            MainScreen.LoadData();

        }

        #endregion

        #region Radian
        public static double Radian(double degree)
        {
            return degree * Math.PI / 180;
        }
        #endregion Radian

        #region Degree
        public static double Degree(double radian)
        {
            return radian * 180 / Math.PI;
        }
        #endregion Degree

        #region Expand Panel
        /// <summary>
        /// Resize the panel to a specific height
        /// </summary>
        /// <param name="pnl">Name of the panel</param>
        /// <param name="StopAt">Maximum height of th panel</param>
        public static void expandEffect(Panel pnl, int StopAt)
        {

            if (pnl.Height > 48)
            {
                StopAt = 46;
            }

            pnl.Height = StopAt;

        }
        #endregion Expand Panel

        #region Format Current DateTime 
        /// <summary>
        /// Get the current date and time in a formatted string
        /// </summary>
        /// <returns></returns>
        public static string currentdateTime()
        {
            var theDateTime = DateTime.Now;

            //2016/06/10 06:44:25.93 
            return theDateTime.Year.ToString() + "/" + theDateTime.Month.ToString("00") + "/" + theDateTime.Day.ToString("00") + " "
                + theDateTime.Hour.ToString("00") + ":" + theDateTime.Minute.ToString("00") + ":" + theDateTime.Second.ToString("00") + "." + (theDateTime.Millisecond / 10).ToString("00");

        }
        #endregion Format Current DateTime 

        #region String Format

        /// <summary>
        /// Function to capitalize Words
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FirstCharToUpper(string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": return null;
                default:
                    var arr = input.ToLower().ToCharArray();
                    arr[0] = Char.ToUpperInvariant(arr[0]);
                    return new String(arr);
            }
        }

        /// <summary>
        /// Function to capitalize Words
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CapitalizeSentence(string input)
        {
            string myCustomFolder = null;

            var split = input.Split(' ');
            int wordIndex = 0;
            foreach (var item in split)
            {
                if (wordIndex == 0)
                {
                    myCustomFolder = FirstCharToUpper(item);
                }
                else
                {
                    myCustomFolder += " " + FirstCharToUpper(item);
                }
                wordIndex += 1;
            }

            return myCustomFolder;
        }
        

        /// <summary>
        /// Get all string before a string or character
        /// </summary>
        /// <param name="text"> The text/string to seacrh in</param>
        /// <param name="stopAt">the work or character to be searched</param>
        /// <returns></returns>
        public static string GetUntilOrEmpty(string text, string stopAt = "-")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.LastIndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
        }

        #endregion String Format

        #region Format Size 
        /// <summary>
        /// Get the file size in bytes
        /// </summary>
        /// <param name="FilePath">Path to the file</param>
        /// <returns></returns>
        public static string GetFileSizeInBytes(string FilePath)
        {
            // Create new FileInfo object and get the Length.
            FileInfo fInfo = new FileInfo(FilePath);
            long TotalBytes = fInfo.Length;

            if (TotalBytes >= 1073741824) //Giga Bytes
            {
                Decimal FileSize = Decimal.Divide(TotalBytes, 1073741824);
                return String.Format("{0:##.##} GB", FileSize);
            }
            else if (TotalBytes >= 1048576) //Mega Bytes
            {
                Decimal FileSize = Decimal.Divide(TotalBytes, 1048576);
                return String.Format("{0:##.##} MB", FileSize);
            }
            else if (TotalBytes >= 1024) //Kilo Bytes
            {
                Decimal FileSize = Decimal.Divide(TotalBytes, 1024);
                return String.Format("{0:##.##} KB", FileSize);
            }
            else if (TotalBytes >= 0 & TotalBytes <1024)
            {            
                Decimal FileSize = TotalBytes;
                return String.Format("{0:##.##} Bytes", FileSize);
            }
            else
            {
                return "0 Bytes";
            }
        }
        #endregion Format Size 

        #region RINEX Reader
        public static int eof = 0;
        public static int success = 1;
        public static int warnings = 0;
        public static int got_info = 0;
        public static int numHeaderLines = 0;
        public static int clockOffsetsON = 0;
        public static int numLinTypObs = 0;
        public static string tInterval = string.Empty;
        public static int numOfObsTypes = 0;
        public static double leapSec = 0;
        public static string[] antDelta = new string[] { };
        public static string[] rinexHeader = new string[] { };
        public static string timeSystem = string.Empty;
        public static string[] tFirstObs = new string[6];
        public static string[] tLastObs = new string[6];
        public static string[] typesOfObs = new string[] { };
        public static string rinexVersion = string.Empty;
        public static string rinexType = string.Empty;
        public static string gnssType = string.Empty;
        public static string rinexProgr = string.Empty;
        public static string rinexDate = string.Empty;
        public static string markerName = string.Empty;
        public static double[] approxPos = new double[5];
        public static string aux = string.Empty;
        private static string[] extraTypesOfObs = new string[] { };
        public static string FirtObsString = string.Empty;
        public static string lastObsString = string.Empty;
        public static string[] utm = new string[] { };
        public static double nextObsdouble = 0;
        public static double FirstObsdouble = 0;
        private static bool HasReachedHeaderEnd = false;
        private static bool FistTimeReadingDat = false;
        public static double Epoch = 0;
        private static string yyyy = "", mm = "", dd = "", hh = "", mnt = "", ss = "";
        private static string EpochfullTime = string.Empty;

        public static string SolutionFix = string.Empty;
        public static int FreqBand = 1;   //0 = L1, 1 = L1 + L2, 2 = L1 + L2 + L5, 3 = L1 + L5
        public static int ProcessingMode = 3; //0 = Single  1 = DGPS/DGNSS  2 = Kinematic  3 = Static DGNSS  4 = Moving Base  5 = Fixed  6 = PPP Kinematic  7 = PPP Static  8 = PPP Fixed 
        public static bool IsBase = false;

        public static void resetRINEXVariables()
        {
            eof = 0;
            success = 1;
            warnings = 0;
            got_info = 0;
            numHeaderLines = 0;
            clockOffsetsON = 0;
            numLinTypObs = 0;
            tInterval = string.Empty;
            numOfObsTypes = 0;
            leapSec = 0;
            antDelta = new string[] { };
            rinexHeader = new string[] { };
            timeSystem = string.Empty;
            tFirstObs = new string[6];
            tLastObs = new string[6];
            typesOfObs = new string[] { };
            rinexVersion = string.Empty;
            rinexType = string.Empty;
            gnssType = string.Empty;
            rinexProgr = string.Empty;
            rinexDate = string.Empty;
            markerName = string.Empty;
            approxPos = new double[5]; ;
            aux = string.Empty;
            extraTypesOfObs = new string[] { };
            FirtObsString = string.Empty;
            lastObsString = string.Empty;
            FirstObsdouble = 0;
            nextObsdouble = 0;
            HasReachedHeaderEnd = false;
            EpochfullTime = string.Empty;
            FistTimeReadingDat = false;
            Epoch = 0;

            SolutionFix = string.Empty;

        }

        /// <summary>
        /// Extracts relevant data from the header of a RINEX GNSS observations file.
        /// Analyzes the header of a RINEX GNSS observation file and extracts relevant data.
        /// </summary>
        /// <param name="file">Path to the RINEX file</param>
        /// <returns></returns>
        public static async Task<Array[]> RinexReadsObsFile(string filePath)
        {
            /*
              Input:
                    file: RINEX observation file


             Outputs:
                     success: 1 if the reading of the RINEX observations file seems to be successful, 0 otherwise
                     
                     warnings: 1 if the reading of the RINEX observations was done with warnings, 0 otherwise
                     
                     rHeader: cell column-vector containing the following data:
                     
                     rinexVersion: RINEX version number; string: '' if not specified
                     
                     rinexType: RINEX file type; char
                     
                     gnssType: GNSS system of the satellites observed; can be 'G', 'R', 'S', 'E' or 'M' that stand for GPS, GLONASS, Geostationary
                     signal payload, GALILEO or Mixed (satellites from various of the previous systems); char
                     
                     rinexProgr: name of the software used to produce de RINEX GPS nav file; '' if not specified
                     
                     rinexDate: date/time of the RINEX file creation; '' if not specified
                     
                     markerName: name of the antenna marker; '' if not specified
                     
                     antDelta: column vector ot the three components of the distance from the marker to the antenna, in the following order - up, east and
                     north; reals; null vector by default
                     
                     numOfObsTypes: number of different observation types stored in the RINEX file; THIS IS CRITICAL DATA!
                     
                     typesOfObs: cell column-vector containing the observation types; eachB2 observation type is a two-character string, the first one (a
                     capital letter) is an observation code and the second one (a digit) is a frequency code. THIS IS CRITICAL DATA!
                    
                     According to RINEX 2.11 these codes are:
                    
                     C: Pseudorange GPS: C/A, L2C
                     Glonass: C/A
                     Galileo: All
                     P: Pseudorange GPS and Glonass: P code
                     L: Carrier phase
                     D: Doppler frequency
                     S: Raw signal strengths or SNR values
                     as given by the receiver for the
                     respective phase observations (see comments of
                     function rinexReadsObsBlock211)
                    
                     Frequency code
                     GPS Glonass Galileo SBAS
                     1: L1 G1 E2-L1-E1 L1
                     2: L2 G2 -- --
                     5: L5 -- E5a L5
                     6: -- -- E6 --
                     7: -- -- E5b --
                     8: -- -- E5a+b --
                    
                     Observations collected under Antispoofing are converted to "L2" or "P2" and flagged with bit 2 of loss of lock indicator (LLI);
                     read comments of function rinexReadsObsBlock211
                    
                     tFirstObs: time stamp of the first observation record in the RINEX observations file; column vector of reals [YYYY; MM; DD; hh; mm;
                     ss.sssssss]; THIS IS CRITICAL DATA
                     
                     tLastObs: time stamp of the last observation record in the RINEX observations file; column vector of reals [YYYY; MM; DD; hh; mm;
                     ss.sssssss]. NaN by default. THIS IS RINEX 2.11 OPTIONAL DATA
                     
                     tInterval: observations interval; seconds. NaN by default. THIS IS RINEX 2.11 OPTIONAL DATA
                     
                     timeSystem: three-character code string of the time system used for expressing tfirstObs; can be GPS, GLO or GAL; THIS IS CRITICAL DATA
                     
                     numHeaderLines: total number of lines of the headerB3
                     
                     clockOffsetsON: receiver clock offsets flag. O if no realtimederived receiver clock offset was applied to epoch, code and phase data (in
                     other words, if the file only has raw data), 1 otherwise. 0 by default. THIS IS RINEX 2.11 OPTIONAL DATA
                     
                     leapSec: number of leap seconds since 6-Jan-1980. UTC=GPSTleapSec. NaN by default. THIS IS RINEX 2.11 OPTIONAL DATA
                     
                     eof: end-of-file flag; 1 if end-of-file was reached, 0 otherwise
                    
                     Based in the work of Kai Borre and António Pestana, March 2015
             */


            String line;            

            try
            {
                // Open the text file using a stream reader.
                StreamReader sr = new StreamReader(filePath);

                resetRINEXVariables(); // Reset all variables
                
                //Continue to read until you reach end of file
                while (true) // Gobbling the header
                {

                    numHeaderLines = numHeaderLines + 1;

                    //Read the first line of text
                    line = sr.ReadLine();

                    if (line == null)
                    {
                        sr.Close();
                        break;
                    }

                    if (numHeaderLines == 1)
                    {
                        rinexVersion = line.Remove(9).Trim();
                        rinexType = line.Substring(20, 1).Trim();
                                                       
                        if (!rinexType.Contains("O"))
                        {
                            Console.WriteLine("ERROR: the file is not a RINEX observations data file!");
                            success = 0;

                            sr.Close();
                            break;
                        }

                        gnssType = line.Substring(40, 1); // reads the GNSS system type
                        string[] stringArray = { " ", "G", "R", "S", "E", "M", "C", "J" };
                            
                        if (!stringArray.Any(gnssType.Contains))
                        {
                            Console.WriteLine("ERROR: " + gnssType + " is an unrecognized satellite system type.");
                            success = 0;

                            sr.Close();
                            break;
                        }

                        if (gnssType == " ")
                        {
                            gnssType = "G";
                        }
                        got_info = got_info + 1;
                    }
                        
                    else if (line.Contains("PGM / RUN BY / DATE"))
                    {
                        rinexProgr = line.Substring(0, 20).Trim();
                        rinexDate = line.Substring(40, 20).Trim();
                        got_info = got_info + 1;
                        
                    }                        

                    else if (line.Contains("MARKER NAME"))
                    {
                        markerName = GetUntilOrEmpty(line, "MARKER NAME").Trim();
                        got_info = got_info + 1;
                        
                    }

                    else if (line.Contains("ANTENNA: DELTA H/E/N"))
                    {
                        antDelta = GetUntilOrEmpty(line, "ANTENNA: DELTA H/E/N").Trim().Split(new string[] { "        " }, StringSplitOptions.None);

                        got_info = got_info + 1;                            
                    }
                    
                    else if (line.Contains("APPROX POSITION XYZ")) // Receiver aprox position
                    {
                        var approxPosAll = GetUntilOrEmpty(line, "APPROX POSITION XYZ");

                        approxPos[0] = double.Parse(approxPosAll.Substring(0, 14).Trim());
                        approxPos[1] = double.Parse(approxPosAll.Substring(14, 14).Trim());
                        approxPos[2] = double.Parse(approxPosAll.Substring(28, 14).Trim());
                        SolutionFix = "Fixed";


                        got_info = got_info + 1;                            
                    }                    
 
                    else if (line.Contains("TIME OF FIRST OBS"))
                    {
                        line = GetUntilOrEmpty(line, "TIME OF FIRST OBS").Trim();
                        
                        for (int i = 0; i < 6; i++)
                        {
                                
                            //the components of the time of the
                            // first observation(YYYY; MM; DD; hh; mm; ss.sssssss) and specifies
                            // the Time System used in the
                            // observations file(GPST, GLOT or GALT

                            switch (i)
                            {
                                case 0:                                        
                                    yyyy = line.Substring(0,5).Trim();
                                    line = line.Substring(5).Trim();
                                    break;
                                case 1:
                                    mm = int.Parse(line.Substring(0, 3).Trim()).ToString("00");
                                    line = line.Substring(3).Trim();
                                    break;
                                case 2:
                                    dd = int.Parse(line.Substring(0, 3).Trim()).ToString("00");
                                    line = line.Substring(3).Trim();
                                    break;
                                case 3:
                                    hh = int.Parse(line.Substring(0, 3).Trim()).ToString("00");
                                    line = line.Substring(3).Trim();
                                    break;
                                case 4:
                                    mnt = int.Parse(line.Substring(0, 3).Trim()).ToString("00");
                                    line = line.Substring(3).Trim();
                                    break;
                                case 5:                                    
                                    ss = double.Parse(line.Substring(0, 8).Trim()).ToString("00.0000");
                                    line = line.Substring(8).Trim();
                                    break;
                                default:                                        
                                    line = line.Trim();                                        
                                    break;
                            }                                             

                        }
                                                      
                        tFirstObs = new string[] { yyyy, mm, dd, hh, mnt, ss };
                       
                        got_info = got_info + 1;

                        aux = line.Trim();

                        if (aux != null)
                        {
                            switch (aux) //
                            {
                                case "GPS":
                                    timeSystem = "GPST";
                                    break;
                                case "GLO":
                                    timeSystem = "GLOT";
                                    break;
                                case "GAL":
                                    timeSystem = "GALT";
                                    break;
                                default:
                                    switch (gnssType)
                                    {
                                        case "G":
                                            timeSystem = "GPST";
                                            break;
                                        case "R":
                                            timeSystem = "GLOT";
                                            break;
                                        case "E":
                                            timeSystem = "GALT";
                                            break;
                                        default:
                                            Console.WriteLine("CRITICAL ERROR (rinexReadsObsHeader):\n The Time System of the RINEX observations file 'isn''t correctly specified!\n");                                            
                                            success = 0;                                            
                                            break;
                                    }

                                    break;
                            }
                        }
                            
                    }
                                     
                    else if (line.Contains("LEAP SECONDS"))
                    {
                        leapSec = double.Parse(GetUntilOrEmpty(line, "LEAP SECONDS").Trim());
                        got_info = got_info + 1;
                    }
                    
                    else if (line.Contains("END OF HEADER")) // the end of the header was found
                    {
                        if (got_info > 3)
                        {
                            HasReachedHeaderEnd = true;
                            FistTimeReadingDat = true;
                            success = 1;                            
                        }
                        else
                        {
                            success = 0;
                            warnings = 1;
                            break;
                        }
                       
                    }

                    else
                    {
                        // Done with Header section
                        // Get all observation data
                        string newEpochLine = "";
                        
                        if (success == 1 && HasReachedHeaderEnd == true)
                        {
                           
                            if (FistTimeReadingDat == true)
                            {
                                Epoch += 1;

                                //Get the date line to work with
                                if (double.Parse(rinexVersion) >= 3)
                                {
                                    newEpochLine = line.Substring(1, 9).Trim();
                                }
                                else
                                {
                                    newEpochLine = line.Substring(0, 6).Trim();  //Year and Month will not Use 9 for daily data
                                }
                               
                                FistTimeReadingDat = false;
                                
                            }

                            while (true)
                            {
                                //Read next line
                                line = sr.ReadLine();

                                if (line == null)
                                {
                                    sr.Close();
                                    break;
                                }

                                //Read for first time for each new Epoch Information
                                if (line.Contains(newEpochLine))
                                {
                                    Epoch += 1;

                                    //New epoch
                                    if (double.Parse(rinexVersion)>=3)
                                    {
                                        EpochfullTime = line.Substring(1, 29).Trim();
                                    }
                                    else
                                    {
                                        EpochfullTime = line.Substring(0, 26).Trim();
                                    }
                                   
                                    for (int i = 0; i < 6; i++)
                                    {

                                        //the components of the time of the
                                        // first observation(YYYY; MM; DD; hh; mm; ss.sssssss) and specifies
                                        // the Time System used in the
                                        // observations file(GPST, GLOT or GALT
                                        
                                        switch (i)
                                        {
                                            case 0:
                                                if (double.Parse(rinexVersion) >= 3)
                                                {
                                                    yyyy = EpochfullTime.Substring(0, 4).Trim();
                                                    EpochfullTime = EpochfullTime.Substring(4).Trim();
                                                }
                                                else
                                                {
                                                    yyyy = EpochfullTime.Substring(0, 2).Trim();
                                                    EpochfullTime = EpochfullTime.Substring(2).Trim();
                                                }                                                
                                                break;
                                            case 1:
                                                mm = int.Parse(EpochfullTime.Substring(0, 2).Trim()).ToString("00");
                                                EpochfullTime = EpochfullTime.Substring(2).Trim();
                                                break;
                                            case 2:
                                                dd = int.Parse(EpochfullTime.Substring(0, 2).Trim()).ToString("00");
                                                EpochfullTime = EpochfullTime.Substring(2).Trim();
                                                break;
                                            case 3:
                                                hh = int.Parse(EpochfullTime.Substring(0, 2).Trim()).ToString("00");
                                                EpochfullTime = EpochfullTime.Substring(2).Trim();
                                                break;
                                            case 4:
                                                mnt = int.Parse(EpochfullTime.Substring(0, 2).Trim()).ToString("00");
                                                EpochfullTime = EpochfullTime.Substring(2).Trim();
                                                break;
                                            case 5:
                                                ss = double.Parse(EpochfullTime.Substring(0, 9).Trim()).ToString("00.0000");
                                                EpochfullTime = EpochfullTime.Substring(9).Trim();
                                                break;
                                            default:
                                                EpochfullTime = EpochfullTime.Trim();
                                                break;
                                        }

                                    }

                                    tLastObs = new string[] { yyyy, mm, dd, hh, mnt, ss };

                                    if (Epoch == 2)
                                    {

                                        nextObsdouble = double.Parse(tLastObs[3])*3600  + double.Parse(tLastObs[4])*60 + double.Parse(tLastObs[5]);

                                    }
                                }                                
                            }
                            break;
                        }
                                                
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
                Console.WriteLine("The file could not be read:\n" + ex.Message +"\nCaused by:\n" + ex.Source + "On line: "+ Atline);
                MessageBox.Show(ex.Message);
            }


            FirtObsString = tFirstObs[2] + "/" + tFirstObs[1] + "/" + tFirstObs[0] + " " + tFirstObs[3] + ":" + tFirstObs[4] + ":" + tFirstObs[5];
            lastObsString = tFirstObs[2] + "/" + tFirstObs[1] + "/" + tFirstObs[0] + " " + tLastObs[3] + ":" + tLastObs[4] + ":" + tLastObs[5];

            //DateTime startedTime = DateTime.Parse(FirtObsString, new System.Globalization.CultureInfo("en-US", true), System.Globalization.DateTimeStyles.AssumeLocal); ;
            //DateTime nextEpochTime = DateTime.Parse(nextObsString, new System.Globalization.CultureInfo("en-US", true), System.Globalization.DateTimeStyles.AssumeLocal); ;
            //tInterval = nextEpochTime.Subtract(startedTime).ToString();

            FirstObsdouble = double.Parse(tFirstObs[3])*3600 + double.Parse(tFirstObs[4])*60 + double.Parse(tFirstObs[5]);
            
            tInterval = TimeSpan.FromSeconds(nextObsdouble-FirstObsdouble).ToString();
          
            //Create new coordinate factory
            CoordinateSystemConversion cor = new CoordinateSystemConversion();


            //Create new coordinate system from built functions
            var wgs84 = new GridSystem().WGS84();
                       
            var ENU = new string[] { };
                        

            ////Convert to the new XYZ
            //var XYZ = cor.XYZTransformation(GHCRS.TransParam, approxPos, false);
            
            //var LatLonH = cor.XYZ2LatLonH(GHCRS, XYZ);

            //ENU = cor.LatLonH2ENU(GHCRS, LatLonH); 

            //approxPos = cor.XYZ2LatLonH(wgs84, approxPos);
                       
            //utm = cor.WGS84latLon2UTM(approxPos);

            CoordinateSystemFactory cFac = new CoordinateSystemFactory();

            var IGeoncenTarget = cFac.CreateGeocentricCoordinateSystem(" ", (MainScreen.GHCRS as IProjectedCoordinateSystem).HorizontalDatum, (MainScreen.GHCRS as IProjectedCoordinateSystem).LinearUnit, (MainScreen.GHCRS as IProjectedCoordinateSystem).GeographicCoordinateSystem.PrimeMeridian);
            var IGeoncenSource = cFac.CreateGeocentricCoordinateSystem(" ", (MainScreen.WGS84 as IProjectedCoordinateSystem).HorizontalDatum, (MainScreen.WGS84 as IProjectedCoordinateSystem).LinearUnit, (MainScreen.WGS84 as IProjectedCoordinateSystem).GeographicCoordinateSystem.PrimeMeridian);

            //==============TO UTM===============
            MainScreen.trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems(IGeoncenSource, (MainScreen.WGS84 as IProjectedCoordinateSystem).GeographicCoordinateSystem);
            var utmLonLatH = MainScreen.trans.MathTransform.Transform(approxPos);
            MainScreen.trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((MainScreen.WGS84 as IProjectedCoordinateSystem).GeographicCoordinateSystem, (MainScreen.WGS84 as IProjectedCoordinateSystem));
            var utm = MainScreen.trans.MathTransform.Transform(utmLonLatH);
            //===================================

            //================TO LOCAL===========
            MainScreen.trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems((MainScreen.WGS84 as IProjectedCoordinateSystem), (MainScreen.GHCRS as IProjectedCoordinateSystem));
            var _ENU = MainScreen.trans.MathTransform.Transform(utm);

            approxPos[0] = _ENU[0];//(tm[0] * 0.304799706846) / CovertFactor;
            approxPos[1] = _ENU[1];//(tm[1] * 0.304799706846) / CovertFactor;
            approxPos[2] = _ENU[2] / CovertFactor;//tm[2] / CovertFactor;

            //approxPos = gpsLib.LatLongH2UTM(DefineConstants.RE_WGS84, 298.257222101, pos[0], pos[1], pos[2], 0.9996, 0, 500000, 0, 0);


            //rinexHeader = new string[] { rinexVersion, rinexType, gnssType, rinexProgr , rinexDate , leapSec.ToString(), tInterval.ToString()};

            Array[] OutputHeader = new Array[] { rinexHeader, antDelta, typesOfObs, approxPos, tFirstObs, tLastObs };

            return OutputHeader;
        }

        public static async Task<Array[]> RinexReadsObsNavFile(string filePath)
        {
            /*
              Input:
                    file: RINEX observation file


             Outputs:
                     success: 1 if the reading of the RINEX observations file seems to be successful, 0 otherwise
                     
                     warnings: 1 if the reading of the RINEX observations was done with warnings, 0 otherwise
                     
                     rHeader: cell column-vector containing the following data:
                     
                     rinexVersion: RINEX version number; string: '' if not specified
                     
                     rinexType: RINEX file type; char
                     
                     gnssType: GNSS system of the satellites observed; can be 'G', 'R', 'S', 'E' or 'M' that stand for GPS, GLONASS, Geostationary
                     signal payload, GALILEO or Mixed (satellites from various of the previous systems); char
                     
                     rinexProgr: name of the software used to produce de RINEX GPS nav file; '' if not specified
                     
                     rinexDate: date/time of the RINEX file creation; '' if not specified
                     
                     markerName: name of the antenna marker; '' if not specified
                     
                     antDelta: column vector ot the three components of the distance from the marker to the antenna, in the following order - up, east and
                     north; reals; null vector by default
                     
                     numOfObsTypes: number of different observation types stored in the RINEX file; THIS IS CRITICAL DATA!
                     
                     typesOfObs: cell column-vector containing the observation types; eachB2 observation type is a two-character string, the first one (a
                     capital letter) is an observation code and the second one (a digit) is a frequency code. THIS IS CRITICAL DATA!
                    
                     According to RINEX 2.11 these codes are:
                    
                     C: Pseudorange GPS: C/A, L2C
                     Glonass: C/A
                     Galileo: All
                     P: Pseudorange GPS and Glonass: P code
                     L: Carrier phase
                     D: Doppler frequency
                     S: Raw signal strengths or SNR values
                     as given by the receiver for the
                     respective phase observations (see comments of
                     function rinexReadsObsBlock211)
                    
                     Frequency code
                     GPS Glonass Galileo SBAS
                     1: L1 G1 E2-L1-E1 L1
                     2: L2 G2 -- --
                     5: L5 -- E5a L5
                     6: -- -- E6 --
                     7: -- -- E5b --
                     8: -- -- E5a+b --
                    
                     Observations collected under Antispoofing are converted to "L2" or "P2" and flagged with bit 2 of loss of lock indicator (LLI);
                     read comments of function rinexReadsObsBlock211
                    
                     tFirstObs: time stamp of the first observation record in the RINEX observations file; column vector of reals [YYYY; MM; DD; hh; mm;
                     ss.sssssss]; THIS IS CRITICAL DATA
                     
                     tLastObs: time stamp of the last observation record in the RINEX observations file; column vector of reals [YYYY; MM; DD; hh; mm;
                     ss.sssssss]. NaN by default. THIS IS RINEX 2.11 OPTIONAL DATA
                     
                     tInterval: observations interval; seconds. NaN by default. THIS IS RINEX 2.11 OPTIONAL DATA
                     
                     timeSystem: three-character code string of the time system used for expressing tfirstObs; can be GPS, GLO or GAL; THIS IS CRITICAL DATA
                     
                     numHeaderLines: total number of lines of the headerB3
                     
                     clockOffsetsON: receiver clock offsets flag. O if no realtimederived receiver clock offset was applied to epoch, code and phase data (in
                     other words, if the file only has raw data), 1 otherwise. 0 by default. THIS IS RINEX 2.11 OPTIONAL DATA
                     
                     leapSec: number of leap seconds since 6-Jan-1980. UTC=GPSTleapSec. NaN by default. THIS IS RINEX 2.11 OPTIONAL DATA
                     
                     eof: end-of-file flag; 1 if end-of-file was reached, 0 otherwise
                    
                     Based in the work of Kai Borre and António Pestana, March 2015
             */

            String line;

            try
            {
                // Open the text file using a stream reader.
                StreamReader sr = new StreamReader(filePath);

                string FileDirectory = filePath.Substring(0, filePath.LastIndexOf('\\'));
                string Filename = Path.GetFileNameWithoutExtension(filePath);
                string ext = Path.GetExtension(filePath);
                
                string checkFormat = "";

                //Copy teqc to the folder
                if (!File.Exists(Path.Combine(FileDirectory, "teqc.exe")))
                {
                    File.Copy(Path.Combine(Environment.CurrentDirectory, "teqc.exe"), Path.Combine(FileDirectory, "teqc.exe"));
                }

                checkFormat = @"teqc +qcq +sym ";                              

                //input file name
                checkFormat += Filename + ext;

                //Out to 
                checkFormat += " > ";

                checkFormat += "Output.txt";

                checkFormat += " 2>&1";
                //if (cbxReportType.SelectedItem.ToString() != "Full")
                //{
                //    //Include QC Symbols
                //    checkFormat += " 2>&1";
                //}

                var result = await ProcessEx.RunCMD(new string[] { @"cd " + FileDirectory, checkFormat, "exit" });

                //Delete the file "teqc.exe"
                File.Delete(Path.Combine(FileDirectory, "teqc.exe"));

                string file = "";

                WaitLoadingForm.ExtraStaus = "Checking result";

                //Make sure the solution file is deleted
                if (File.Exists(Path.Combine(FileDirectory, Filename + ext.Substring(0, 3) + "S")))
                {
                    WaitLoadingForm.ExtraStaus = "Reading result";

                    file = Path.Combine(FileDirectory, Filename + ext.Substring(0, 3) + "S");
                    
                }
                else
                {
                    WaitLoadingForm.ExtraStaus = "Reading result";

                    file = Path.Combine(FileDirectory, "Output.txt");
                   
                }

                resetRINEXVariables(); // Reset all variables

                using (StreamReader procFile = new StreamReader(file))
                {

                    line = await procFile.ReadLineAsync();
                   
                    while (true)
                    {                        
                        line = await procFile.ReadLineAsync();

                        if (line == null)
                        {
                            continue;
                        }
                        else if (line.Contains("Time of start of window :"))
                        {
                            
                            FirtObsString = line.Substring(25).Trim();                            
                            continue;
                        }
                        else if (line.Contains("Time of  end  of window :"))
                        {
                            lastObsString = line.Substring(25).Trim();
                                                       
                            continue;
                        }
                        else if (line.Contains("antenna WGS 84 (xyz)  :"))
                        {
                            var approxPosAll = line.Substring(25).Trim().Split(' ');

                            approxPos[0] = double.Parse(approxPosAll[0]);
                            approxPos[1] = double.Parse(approxPosAll[1]);
                            approxPos[2] = double.Parse(approxPosAll[2]);

                            continue;
                        }
                        else if (line.Contains("Observation interval    :"))
                        {                         
                            
                            tInterval = TimeSpan.FromSeconds(double.Parse(GetUntilOrEmpty(line.Substring(25).Trim(), "seconds").Trim())).ToString(); 
                            continue;
                        }
                        else if (line.Contains("Epochs w/ observations  :"))
                        {
                            Epoch = double.Parse(line.Substring(25).Trim());
                            
                            break;
                        }
                       
                    }                                                          

                }
                
                //Make sure the solution file is deleted
                if (File.Exists(Path.Combine(FileDirectory, Filename + ext.Substring(0, 3) + "S")))
                {
                    File.Delete(Path.Combine(FileDirectory, Filename + ext.Substring(0, 3) + "S"));
                    File.Delete(Path.Combine(FileDirectory, "Output.txt"));
                }
                else
                {
                    File.Delete(Path.Combine(FileDirectory, "Output.txt"));
                }

                CoordinateSystemConversion cor = new CoordinateSystemConversion();
                var wgs84 = new GridSystem().WGS84();
                //Create new coordinate system from built functions
               

                var ENU = new string[] { };

                //Convert to the new XYZ
                var XYZ = cor.XYZTransformation(GHCRS.TransParam, approxPos, false);

                var LatLonH = cor.XYZ2LatLonH(GHCRS, XYZ);

                ENU = cor.LatLonH2ENU(GHCRS, LatLonH);

                approxPos = cor.XYZ2LatLonH(wgs84, approxPos);

                utm = cor.WGS84latLon2UTM(approxPos);
                
                approxPos[0] = double.Parse(ENU[0]);//(tm[0] * 0.304799706846) / CovertFactor;
                approxPos[1] = double.Parse(ENU[1]);//(tm[1] * 0.304799706846) / CovertFactor;
                approxPos[2] = double.Parse(ENU[2]);//tm[2] / CovertFactor;

                Array[] OutputHeader = new Array[] { rinexHeader, antDelta, typesOfObs, approxPos, tFirstObs, tLastObs };

                return OutputHeader;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            
        }
                        
        #endregion RINEX Reader

    }

    public static class StringExtension
    {
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string GetPrefix(string first, string second)
        {
            int prefixLength = 0;

            for (int i = 0; i < Math.Min(first.Length, second.Length); i++)
            {
                if (first[i] != second[i])
                    break;

                prefixLength++;
            }

            return first.Substring(0, prefixLength);
        }

        public static string GetPrefix(IList<string> strings)
        {
            return strings.Aggregate(GetPrefix);
        }

        public static IEnumerable<string> RemovePrefix(IList<string> strings)
        {
            var prefix = GetPrefix(strings);

            return strings.Select(s => s.Substring(prefix.Length, s.Length - prefix.Length));
        }

        public static string CapitalizeFirst(this string s)
        {
            bool IsNewSentense = true;
            var result = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                if (IsNewSentense && char.IsLetter(s[i]))
                {
                    result.Append(char.ToUpper(s[i]));
                    IsNewSentense = false;
                }
                else
                    result.Append(s[i]);

                if (s[i] == '!' || s[i] == '?' || s[i] == '.' || s[i] == '.')
                {
                    IsNewSentense = true;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Split Camel case String
        /// </summary>
        /// <param name="input"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string SplitCamelCase(this string input, string delimiter = " ")
        {
            try
            {
                return input.Any(char.IsUpper) ? string.Join(delimiter, Regex.Split(input, @"(?<!^)(?=[A-Z])")) : input;
            }
            catch (Exception)
            {

                return input;
            }

        }

        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        }
    }

    public static class RandomExtensions
    {
        public static double NextDouble( this Random random, double minValue, double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }
    }

}
