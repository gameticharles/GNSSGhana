using ghGPS.Properties;
using ghGPS.User_Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ghGPS.Forms;
using ghGPS.Classes;
using ghGPS.Classes.CoordinateSystems.IO;
using ghGPS.Classes.CoordinateSystems;
using ghGPS.Classes.CoordinateSystems.Transformations;
using libRTK;
using static ghGPS.Classes.SRIDReader;
using GNSSUpdate;
using System.Reflection;
using System.IO;
using System.Windows.Forms.VisualStyles;

namespace ghGPS
{
    public partial class MainScreen : MetroSuite.MetroForm, IGNSSUpdatable
    {

        #region "Declarations"
        public static IntPtr MainHandler = new IntPtr();
        public static RecentStartScreen recentStartScreen = new RecentStartScreen();
        public static CreateProject createProject = new CreateProject();
        public static GlobalSettings globalSettings = new GlobalSettings();
        public static GNSSDataImport GNSSDataImport = new GNSSDataImport();
        public static CadastralReoprtDataImport manualDataImport = new CadastralReoprtDataImport();
        //public static openFileBrowser openFileBrowser = new openFileBrowser();
        public static BaseLineChart baseLineChart = new BaseLineChart();
        public static ProcessResults ProcessResults = new ProcessResults();
        public static CoordinateConversion CoordinateConversion = new CoordinateConversion();
        public static RINEXTEQC RINEXTEQC = new RINEXTEQC();

        
        public static Tutorials TutorialsPage = new Tutorials();
        public static Button btnSettings = new Button();
        public static Label lblHeaderTitle = new Label();
        public static SQLiteHelper _SQLiteHelper = new SQLiteHelper();
        //public static RTK rtk = new RTK();
        public static object AllSRID = GetSRIDs();
        public static ICoordinateSystem GHCRS = CoordinateSystemWktReader.Parse(GetCSbyID((IEnumerable<WKTstring>)AllSRID, 2136)) as ICoordinateSystem; //Ghana
        public static ICoordinateSystem WGS84 = CoordinateSystemWktReader.Parse(GetCSbyID((IEnumerable<WKTstring>)AllSRID, 32630)) as ICoordinateSystem; //WGS 84
        public static ICoordinateTransformation trans;

        public static ICoordinateSystem GH3Params = GNSS_Functions.CreateTMCoordSys("Ghana National Grid 3 Params", 100000, new TransParams().GH3TransParams().getValues(), "Ghana");
        public static ICoordinateSystem GH7Params = GNSS_Functions.CreateTMCoordSys("Ghana National Grid 7 Params", 100001, new TransParams().GH7TransParams().getValues(), "Ghana");
        public static ICoordinateSystem GH10Params = GNSS_Functions.CreateTMCoordSys("Ghana National Grid 10 Params", 100002, new TransParams().GH10TransParams().getValues(), "Ghana");

        public static RunResults GNSSResult = new RunResults();
       
        public static GNSSUpdater updater;
        public static MetroSuite.MetroForm MainScreenTitle; 
        #endregion

        public MainScreen(string args = null)
        {
           
            InitializeComponent();
            
            //AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs arg) =>
            //{
            //    string resourceName = new AssemblyName(arg.Name).Name + ".dll";
            //    string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

            //    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
            //    {
            //        byte[] assemblData = new byte[stream.Length];
            //        stream.Read(assemblData, 0, assemblData.Length);

            //        return Assembly.Load(assemblData);
            //    }

            //};


            //// create bridge, with default setup
            //// it will lookup jni4net.j.jar next to jni4net.n.dll
            //Bridge.CreateJVM(new BridgeSetup() { Verbose = true });

            #region User Controls
            //Add Components and Controls to the form
            pnlContainer.Controls.Add(recentStartScreen);
            recentStartScreen.Dock = DockStyle.Fill;

            pnlContainer.Controls.Add(createProject);
            createProject.Dock = DockStyle.Fill;

            pnlContainer.Controls.Add(globalSettings);
            globalSettings.Dock = DockStyle.Fill;

            pnlContainer.Controls.Add(GNSSDataImport);
            GNSSDataImport.Dock = DockStyle.Fill;

            pnlContainer.Controls.Add(manualDataImport);
            manualDataImport.Dock = DockStyle.Fill;

            pnlContainer.Controls.Add(baseLineChart);
            baseLineChart.Dock = DockStyle.Fill;
                        
            pnlContainer.Controls.Add(TutorialsPage);
            TutorialsPage.Dock = DockStyle.Fill;

            pnlContainer.Controls.Add(ProcessResults);
            ProcessResults.Dock = DockStyle.Fill;

            pnlContainer.Controls.Add(CoordinateConversion);
            CoordinateConversion.Dock = DockStyle.Fill;

            pnlContainer.Controls.Add(RINEXTEQC);
            RINEXTEQC.Dock = DockStyle.Fill;

            // 
            // btnSettings
            // 
            btnSettings.AutoEllipsis = true;
            btnSettings.Dock = DockStyle.Right;
            btnSettings.FlatAppearance.BorderColor = Color.FromArgb(243, 243, 243);
            btnSettings.FlatAppearance.BorderSize = 0;
            btnSettings.FlatAppearance.MouseDownBackColor = Color.FromArgb(243, 243, 243);
            btnSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(243, 243, 243);
            btnSettings.FlatStyle = FlatStyle.Flat;
            btnSettings.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btnSettings.ForeColor = Color.DimGray;
            btnSettings.Image = Resources.Settings;
            btnSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnSettings.Location = new Point(660, 0);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(85, 27);
            btnSettings.TabIndex = 1;
            btnSettings.Text = "Settings";
            btnSettings.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            btnSettings.UseVisualStyleBackColor = true;
            btnSettings.Click += btnSettings_Click;
            this.ToolTip.SetToolTip(btnSettings, "Go to global settings");

            //Create and add
            //var area = new MetroSuite.MetroControlBoxArea(MetroSuite.MetroControlBoxArea.ControlBoxAreaType.Custom, MetroSuite.Design.Style.Light, "custom1");

            //area.AreaImage = Resources.Settings;
            //frmControlBox.Areas.Add(area);

            pnlHeader.Controls.Add(btnSettings);
            btnSettings.BringToFront();

            #endregion User Controls
            
            MainScreenTitle = this;

            MainHandler = this.Handle;

            //Implements the updater
            updater = new GNSSUpdater(this);

            //Check data source
            if (args != null)
            {
                //Set Project name
                string fileWithExt = args.Substring(args.LastIndexOf(@"\") + 1);
                GNSS_Functions.ProjectName = fileWithExt.Remove(fileWithExt.LastIndexOf(@"."));

                //Set Project Path
                GNSS_Functions.ProjectPath = args.Substring(0, args.LastIndexOf(@"\"));
               
            }
            else
            {
                //Load Recent Project
                LoadData();
                                
            }

            //ExeProgram.Run(@"C:\Users\Reindroid\Desktop\POST vs\rtkpost.exe");
            
            //MessageBox.Show(GNSSResult.RunException.ToString());
            // + "\n\nOutput\n" + GNSSResult.Output.ToString() + "\n\nErr\n" + GNSSResult.Error.ToString());            

        }

        #region GNSSUpdate
        public string ApplicationName
        {
            get { return "GNSS Ghana"; }
        }

        public string ApplicationID
        {
            get { return "GNSS Ghana"; }
        }

        public Assembly ApplicationAssembly
        {
            get { return Assembly.GetExecutingAssembly(); }
        }

        public Icon ApplicationIcon
        {
            get { return this.Icon; }
        }

        //Url to the link in the dropBox
        public Uri UpdateXmlLocation
        {

            get { return new Uri("https://dl.dropbox.com/sh/ms380yp2vrzpde8/AAAqtPWzdbIbGsZHPb1SEHg8a?dl=0"); }
            
        }

        public Form Context
        {
            get { return this; }
        }
        #endregion

        #region Resize Form
        /// <summary>
        /// Allow to resize form
        /// </summary>
        private const int cGrip = 16;
        private const int cCaption = 12;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {
                Point pos = new Point(m.LParam.ToInt32());
                pos = this.PointToClient(pos);

                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2;
                    return;
                }

                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17;
                    return;
                }
            }
            base.WndProc(ref m);
        }

       
        #endregion Resize Form

        /// <summary>
        /// Set Database info
        /// </summary>
        private static void DatabaseInfo()
        {
            //Set the name of the Database
            _SQLiteHelper.DatabaseFile = "GNSSGhanaDB.db3";

            //Set a password
            _SQLiteHelper.Password = null;// "ReClipPassword";
        }

        /// <summary>
        /// Create New GNSSProcessedDB
        /// </summary>
        private static void CreateNewGNSSGhanaDB()
        {
            //Call Database Info
            DatabaseInfo();

            //Create the database File
            _SQLiteHelper.CreateDatabase();

            //Create Table
            Table GNSSProcessedItemsTable = new Table();
            GNSSProcessedItemsTable.Name = "GNSSProcessedItemsTable";

            //Add Columns 
            GNSSProcessedItemsTable.Columns.Add("ProjectName", ColType.Text, true);
            GNSSProcessedItemsTable.Columns.Add("ProjectType", ColType.Text, true);
            GNSSProcessedItemsTable.Columns.Add("ProjectDate", ColType.Text, true); 
            GNSSProcessedItemsTable.Columns.Add("ProjectPath", ColType.Text, true);
            GNSSProcessedItemsTable.Columns.Add("ProjectID", ColType.Text, true);
            GNSSProcessedItemsTable.Columns.Add("SummaryList", ColType.Text, false);
            GNSSProcessedItemsTable.Columns.Add("PointList", ColType.Text, false);
            GNSSProcessedItemsTable.Columns.Add("Beacon", ColType.Text, false);
            GNSSProcessedItemsTable.Columns.Add("DistanceBearing", ColType.Text, false);
            GNSSProcessedItemsTable.Columns.Add("PlanData", ColType.Text, false);
            GNSSProcessedItemsTable.Columns.Add("AreaComputaion", ColType.Text, false);
            GNSSProcessedItemsTable.Columns.Add("MapData", ColType.Text, false);
            GNSSProcessedItemsTable.Columns.Add("PointsLocal", ColType.Text, false);
            GNSSProcessedItemsTable.Columns.Add("PointsUTM", ColType.Text, false);
            GNSSProcessedItemsTable.Columns.Add("PointsGEO", ColType.Text, false);

            //Create Table CRS user-defined Units
            Table UserDefinedCRSTable = new Table();
            UserDefinedCRSTable.Name = "UserDefinedCRSTable";

            // Add Columns
            UserDefinedCRSTable.Columns.Add("CRSName", ColType.Text, true);
            UserDefinedCRSTable.Columns.Add("WKTID", ColType.Text, true);
            UserDefinedCRSTable.Columns.Add("WKTString", ColType.Text, true);
            

            //Create Table Linear Units
            Table LinearUnitsTable = new Table();
            LinearUnitsTable.Name = "LinearUnitsTable";

            // Add Columns
            //LinearUnitsTable.Columns.Add("LinearUnitPriKey", ColType.Integer, true, true, true);
            LinearUnitsTable.Columns.Add("UnitName", ColType.Text, true);
            LinearUnitsTable.Columns.Add("MetersPerUnit", ColType.Text, true);            
            LinearUnitsTable.Columns.Add("Authority", ColType.Text, true);
            LinearUnitsTable.Columns.Add("AuthorityCode", ColType.Text, true);
            LinearUnitsTable.Columns.Add("Alias", ColType.Text, true);
            LinearUnitsTable.Columns.Add("Abbreviation", ColType.Text, false);
            LinearUnitsTable.Columns.Add("Remarks", ColType.Text, false);
            

            //Create Table Linear Units
            Table EllipsoidsTable = new Table();
            EllipsoidsTable.Name = "EllipsoidsTable";

            // Add Columns
            EllipsoidsTable.Columns.Add("EllipsoidName", ColType.Text, true);
            EllipsoidsTable.Columns.Add("SemiMajorAxis", ColType.Text, true);
            EllipsoidsTable.Columns.Add("InverseFlattening", ColType.Text, true);



            //Create Table
            Table TransParamsTable = new Table();
            TransParamsTable.Name = "TransParamsTable";

            //Add Columns             
            TransParamsTable.Columns.Add("TransName", ColType.Text, true);
            TransParamsTable.Columns.Add("Dx", ColType.Text, true);
            TransParamsTable.Columns.Add("Dy", ColType.Text, true);
            TransParamsTable.Columns.Add("Dz", ColType.Text, true);
            TransParamsTable.Columns.Add("Rx", ColType.Text, true);
            TransParamsTable.Columns.Add("Ry", ColType.Text, true);
            TransParamsTable.Columns.Add("Rz", ColType.Text, true);
            TransParamsTable.Columns.Add("Scale", ColType.Text, true);
            TransParamsTable.Columns.Add("Xm", ColType.Text, true);
            TransParamsTable.Columns.Add("Ym", ColType.Text, true);
            TransParamsTable.Columns.Add("Zm", ColType.Text, true);  

            //Create tables
            _SQLiteHelper.CreateTable(GNSSProcessedItemsTable);
            _SQLiteHelper.CreateTable(UserDefinedCRSTable);
            _SQLiteHelper.CreateTable(LinearUnitsTable);
            _SQLiteHelper.CreateTable(EllipsoidsTable);
            _SQLiteHelper.CreateTable(TransParamsTable);

            
            //Add default GH Local CRS
            InsertNewCRSToDatabase(GH3Params.Name, "100000", GH3Params.WKT);
            InsertNewCRSToDatabase(GH7Params.Name, "100001", GH7Params.WKT);
            InsertNewCRSToDatabase(GH10Params.Name, "100002", GH10Params.WKT);

            //Add default Linear units
            insertLinearUnit(1.0, "Meter", "EPSG", 9001, "m", String.Empty, "Also known as International metre. SI standard unit.");
            insertLinearUnit(0.3048, "Foot", "EPSG", 9002, "ft", String.Empty, String.Empty);
            insertLinearUnit(0.3047997101815088, "Gold Coast foot", "EPSG", 9004, "ft", "Ghana foot", "Used in Ghana");
            insertLinearUnit(0.304800609601219, "US survey foot", "EPSG", 9003, "American foot", "ftUS", "Used in USA.");
            insertLinearUnit(1852, "Nautical mile", "EPSG", 9030, "NM", String.Empty, String.Empty);
            insertLinearUnit(0.3047972654, "Clarke's foot", "EPSG", 9005, "Clarke's foot", String.Empty, "Assumes Clarke's 1865 ratio of 1 British foot = 0.3047972654 French legal metres applies to the international metre. Used in older Australian, southern African & British West Indian mapping.");


            //Add Default Trans Params values to it
            string TransParamsfilename = @"..\..\Classes\TransParameter.csv";
            using (StreamReader sr = File.OpenText(TransParamsfilename))
            {
                sr.ReadLine().Split(',');
                while (!sr.EndOfStream)
                {
                    string[] line = sr.ReadLine().Split(',');
                    InsertTransParamToDatabase(line);
                }
                sr.Close();
            }

            //Add Default Ellipsoid values to it
            string Ellipsoidfilename = @"..\..\Classes\Elipsoid.csv";
            using (StreamReader sr = File.OpenText(Ellipsoidfilename))
            {
                sr.ReadLine().Split(',');
                while (!sr.EndOfStream)
                {
                    string[] line = sr.ReadLine().Split(',');

                    InsertEllipsoid(line[0], line[1], line[2]);

                }
                sr.Close();
            }

            //Reload from database
            LoadData();
        }

        /// <summary>
        /// Insert a linear unit  into database
        /// </summary>
        /// <param name="MetersPerUnit"></param>
        /// <param name="UnitName"></param>
        /// <param name="Authority"></param>
        /// <param name="AuthorityCode"></param>
        /// <param name="Alias"></param>
        /// <param name="Abbreviation"></param>
        /// <param name="Remarks"></param>
        public static void insertLinearUnit(double MetersPerUnit, string UnitName, string Authority, long AuthorityCode, string Alias, string Abbreviation, string Remarks)
        {
            EntryList LinearUnitItem = new EntryList();
            
            LinearUnitItem.Add("UnitName", UnitName);
            LinearUnitItem.Add("MetersPerUnit", MetersPerUnit);
            LinearUnitItem.Add("Authority", Authority);
            LinearUnitItem.Add("AuthorityCode", AuthorityCode);
            LinearUnitItem.Add("Alias", Alias);
            LinearUnitItem.Add("Abbreviation", Abbreviation);
            LinearUnitItem.Add("Remarks", Remarks);

            _SQLiteHelper.CreateEntry("LinearUnitsTable", LinearUnitItem);
        }

        /// <summary>
        /// Insert ellipsoid into database
        /// </summary>
        /// <param name="EllipsoidName"></param>
        /// <param name="SemiMajorAxis"></param>
        /// <param name="InverseFlattening"></param>
        public static void InsertEllipsoid(string EllipsoidName,string SemiMajorAxis, string InverseFlattening)
        {
            EntryList EllipsoidItem = new EntryList();

            EllipsoidItem.Add("EllipsoidName", EllipsoidName);
            EllipsoidItem.Add("SemiMajorAxis", SemiMajorAxis);
            EllipsoidItem.Add("InverseFlattening", InverseFlattening);
            
            _SQLiteHelper.CreateEntry("EllipsoidsTable", EllipsoidItem);
        }

        /// <summary>
        /// Insert Trans-params into database
        /// </summary>
        /// <param name="line"></param>
        public static void InsertTransParamToDatabase(string[] line)
        {
            EntryList TransParamItem = new EntryList();
            TransParamItem.Add("TransName", line[0]);
            TransParamItem.Add("Dx", line[1]);
            TransParamItem.Add("Dy", line[2]);
            TransParamItem.Add("Dz", line[3]);
            TransParamItem.Add("Rx", line[4]);
            TransParamItem.Add("Ry", line[5]);
            TransParamItem.Add("Rz", line[6]);
            TransParamItem.Add("Scale", line[7]);
            TransParamItem.Add("Xm", line[8]);
            TransParamItem.Add("Ym", line[9]);
            TransParamItem.Add("Zm", line[10]);

            _SQLiteHelper.CreateEntry("TransParamsTable", TransParamItem);
        }


        /// <summary>
        /// Insert a new CRS into the database
        /// </summary>
        /// <param name="CRSName"></param>
        /// <param name="WKTID"></param>
        /// <param name="WKTString"></param>
        public static void InsertNewCRSToDatabase(string CRSName, string WKTID, string WKTString)
        {
            EntryList NewCRSItem = new EntryList();
            NewCRSItem.Add("CRSName", CRSName);
            NewCRSItem.Add("WKTID", WKTID);
            NewCRSItem.Add("WKTString", WKTString);

            _SQLiteHelper.CreateEntry("UserDefinedCRSTable", NewCRSItem);
        }

        /// <summary>
        /// Insert GNSS project Data into GNSSProcessedDB
        /// </summary>
        /// <param name="clipboardItem"></param>
        public static void InserProjectToDatabase(projectItem ProjectItem)
        {
            EntryList RecentProjectItem = new EntryList();            
            RecentProjectItem.Add("ProjectName", ProjectItem.lblProjectName.Text);
            RecentProjectItem.Add("ProjectType", ProjectItem.lblProjectType.Text);
            RecentProjectItem.Add("ProjectDate", ProjectItem.lblProjectDate.Text);
            RecentProjectItem.Add("ProjectPath", ProjectItem.Tag.ToString());
            RecentProjectItem.Add("ProjectID", _SQLiteHelper.RemoveSpecialCharacters(ProjectItem.lblProjectDate.Text));

            //RecentProjectItem.Add("ProjectPath", DbType.String, ProjectItem.Tag.ToString());

            //RecentProjectItem.Add("SummaryList");
            //RecentProjectItem.Add("PointList");
            //RecentProjectItem.Add("Beacon");
            //RecentProjectItem.Add("DistanceBearing");
            //RecentProjectItem.Add("PlanData");
            //RecentProjectItem.Add("AreaComputaion");
            //RecentProjectItem.Add("MapData");
            //RecentProjectItem.Add("PointsLocal");
            //RecentProjectItem.Add("PointsUTM");
            //RecentProjectItem.Add("PointsGEO");

            ProjectItem.Dispose();

            _SQLiteHelper.CreateEntry("GNSSProcessedItemsTable", RecentProjectItem);

            //Reload from database
            LoadData();

            //Release Memory
            GC.Collect();
        }


        private static FlowLayoutPanel flp = recentStartScreen.flpRecentProject;

        /// <summary>
        /// Load all items or processed database
        /// </summary>
        public static void LoadData()
        {
            //Check if exist or not
            //Create new if not
            if (!System.IO.File.Exists("./GNSSGhanaDB.db3"))
            {
                CreateNewGNSSGhanaDB();
            }
            else
            {
                //Call Database Info
                DatabaseInfo();

                //Add Items to the respective page
                var ProjectList = _SQLiteHelper.GetEntries("GNSSProcessedItemsTable");

                //Remove all items
                flp.Controls.Clear();

                //Populate from the database
                //Add Recent items
                var recentItem = new projectItem();
                foreach (var item in ProjectList)
                {
                    recentItem = GetProjectItem(item);
                    //recentItem = await GetProjectItem(item);
                    
                    flp.Controls.Add(recentItem);
                    //order the item in the current clipboard                  

                    flp.Controls.SetChildIndex(recentItem, 0);

                }

                //Resize the items 
                recentStartScreen.ResizeItems();

                //Count items
                countItems();

            }


            //Release Memory
            GC.Collect();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// //public static async Task<projectItem> GetProjectItem(ListWithName item)
        public static projectItem GetProjectItem(ListWithName item)
        {
            projectItem RecentItem = new projectItem();
           
            RecentItem.lblProjectName.Text = item.Text;            
            RecentItem.lblProjectType.Text = item.SubItems[0].ToString();
            RecentItem.lblProjectDate.Text = item.SubItems[1].ToString();

            Image img = Resources.GG_Conversion_png;
            switch (item.SubItems[0].ToString())
            {
                
                case "Type:Cadastral Report":
                    img = Resources.GG_Cadastral_png;
                    break;
                case "Type:Processed GNSS":
                    img = Resources.GG_GNSS_Process_png;
                    break;               
                default:
                    break;
            }
            RecentItem.picBxIcon.Image = img;
            
            RecentItem.Tag = item.SubItems[2].ToString();
            

            ////Add tooltip text
            //toolTip2.SetToolTip(RecentItem.lblFileDetail, text);
            //toolTip2.SetToolTip(RecentItem.lblTime, text);
            //toolTip2.SetToolTip(RecentItem.picImageIcon, text);
            //toolTip2.SetToolTip(RecentItem.lblNumFiles, text);

            ////Add Event handler
            //RecentItem.btnDelete.Click += new EventHandler(btnDeleteHandler);

            //RecentItem.Tag = clipboardItem.itemAddedTime;

            return RecentItem;
        }
                
        public static void btnDeleteHandler(projectItem ItemValue)
        {            
            try
            {
                
                //Find the active flp
                foreach (projectItem item in flp.Controls)
                {
                    if (item.lblProjectName.Text == ItemValue.lblProjectName.Text)
                    {
                        
                        //Delete from the Database
                        _SQLiteHelper.DeleteEntry("GNSSProcessedItemsTable", "ProjectID", item.lblProjectDate.Text);
           
                        if (_SQLiteHelper.HasError)
                        {

                            //Display Message of success
                            MessageBox.Show("Project delete unsuccessfully", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                        else
                        {
                            flp.Controls.Remove(item);
                            flp.Refresh();

                            //Reload from database
                            LoadData();
                        }

                        break;
                    }
                }
                              

            }
            catch (System.Exception ex)
            {
                
                MessageBox.Show(ex.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }

            GC.Collect();

        }

        /// <summary>
        /// Count the items in the database
        /// </summary>
        public static void countItems()
        {
            string flpCount = "";

            flpCount = flp.Controls.Count.ToString();

            recentStartScreen.lblStatus.Text = flpCount + " project(s) on the history board";
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            globalSettings.BringToFront();

            //Hide Settings button
            btnSettings.Hide();

            lblHeaderTitle.Show();
        }

        private void MainScreen_Load(object sender, EventArgs e)
        {
            #region======== GNSS Setting Options  ==========

            #region============== Setting 1 =================
            //rtk.pos1_posmode = posmode.single;                      //Positioning Mode
            //rtk.pos1_frequency = frequency.l1_l2;                   //Frequencies
            //rtk.pos1_soltype = soltype.forward;                     //Filter/Solution Type
            //rtk.pos1_elmask = 15;                                   //Elevation Mask
            //rtk.pos1_navsys = 63;
            //rtk.pos1_snrmask = 0;                                   //SNR Mask (dbHz)
            //rtk.pos1_dynamics = false;                              //Rec Dynamics                          
            //rtk.pos1_tidecorr = false;                              //Earth Tide Correction
            //rtk.pos1_ionoopt = ionoopt.brdc;                        //Ionosphere Correction
            //rtk.pos1_tropopt = tropopt.saas;                        //Troposphere Correction
            //rtk.pos1_sateph = eph.brdc;                             //Satellite Ephemeris/Clock
            //rtk.pos1_exclsats = "";                                 //Exclude Satellites (+PRN: Included)
            #endregion============== Setting 1 =================

            #region============== Setting 2 ===============
            //rtk.pos2_armode = armode.continuous;                    //Integer Ambiguity Res (GPS/GLO/BDS)
            //rtk.pos2_gloarmode = gloarmode.on;                      //GLONASS mode ON/OFF
            //rtk.pos2_arminfix = 10;                                 //Min Ratio to Fix Ambiguity
            //rtk.pos2_arlockcnt = 0;                                 //Min Lock to Fix Ambiguity
            //rtk.pos2_arelmask = 0;                                  //Elevation mask to Fix and Hold                    
            //rtk.pos2_aroutcnt = 5;                                  //Outage to Reset Ambiguity  
            //rtk.pos2_slipthres = 0.050;                             //Slip Thres (m)
            //rtk.pos2_maxage = 30;                                   //Max Age of Diff (s) 
            //rtk.pos2_arthres = 30;                                  //Threshold of GDOP
            //rtk.pos2_rejionno = 30;                                 //Reject Innov (m)
            //rtk.pos2_niter = 1;                                     //Max # of AR Iter/# of Filter Iter
            //rtk.pos2_baselen = 0;                                   //Baseline Length Constraint (m) 1
            //rtk.pos2_basesig = 0;                                   //Baseline Length Constraint (m) 2
            #endregion============== Setting 2 ===============

            #region=============== Output =================
            //rtk.out_solformat = solformat.llh;                      //Solution Format
            //rtk.out_outhead = true;                                 //Output Header
            //rtk.out_outopt = true;                                  //Output Processing Option
            //rtk.out_timesys = timesys.gpst;                         //Time System
            //rtk.out_timeform = timeform.hms;                        //Time Format
            //rtk.out_timendec = 3;                                   //Number of Decimal
            //rtk.out_degform = degform.deg;                          //Latitude and Longitude Format
            //rtk.out_fieldsep = "";                                  //Field Separator            
            //rtk.out_height = height.ellipsoidal;                    //Height System
            //rtk.out_geoid = geoid.intern;                           //Geoid Model
            //rtk.out_solstatic = solstatic.single;                   //Solution for Static Mode
            //rtk.out_nmeaintv1 = 0;                                  //NMEA Interval (s) RMC/GGA, GSA/GSV 1
            //rtk.out_nmeaintv2 = 0;                                  //NMEA Interval (s) RMC/GGA, GSA/GSV 2
            //rtk.out_outstat = stat.off;                             //Output Solution Status
            #endregion=============== Output =================

            #region================ Statistics ================
            //rtk.stats_errratio = 100;                               //Carrier-Phase Error Ratio L1/L2
            //rtk.stats_errphase = 100.0;                             //Code            
            //rtk.stats_errphaseel = 0.003;                           //Carrier-Phase Error a+b/sinEL
            //rtk.stats_errphasebl = 0.000;                           //Carrier-Phase Error/Baseline (m/10km)
            //rtk.stats_errdoppler = 1.000;                           //Doppler Frequency (Hz)
            //rtk.stats_prnaccelh = 1.00E+01;                         //Receiver Accel Horiz (m/S2)
            //rtk.stats_prnaccelv = 1.00E+01;                         //Receiver Accel Vertical (m/S2)
            //rtk.stats_prnbias = 1.00E-04;                           //Carrier-Phase Bias (cycle)
            //rtk.stats_prniono = 1.00E-03;                           //Vertical Ionospheric Delay (m/10km)
            //rtk.stats_prntrop = 1.00E-04;                           //Zenith Tropospheric Delay (m)
            //rtk.stats_stdbias = 1.00E-04;
            //rtk.stats_stdiono = 1.00E-03;
            //rtk.stats_stdtrop = 1.00E-04;
            //rtk.stats_clkstab = 5.00E-12;                           //Satellite Clock Stability (s/s)
            #endregion================ Statistics ================

            #region================ Position =================
            //rtk.ant1_postype = postype.rinexhead;
            //rtk.ant1_anttype = "*";                                 //Rover Antenna Type (*: Auto)
            //rtk.ant1_antdele = 0.0000;                              //Rover Delta Eastings
            //rtk.ant1_antdeln = 0.0000;                              //Rover Delta Northings
            //rtk.ant1_antdelu = 0.0000;                              //Rover Delta Height
            //rtk.ant1_pos1 = 0.0000;                                 //Rover Latitude/X
            //rtk.ant1_pos2 = 0.0000;                                 //Rover Longitude/Y
            //rtk.ant1_pos3 = 0.0000;                                 //Rover Geo Height/Z
            //rtk.ant2_postype = postype.rinexhead;
            //rtk.ant2_anttype = "*";                                 //Base Antenna Type (*: Auto)
            //rtk.ant2_antdele = 0.0000;                              //Base Delta Eastings
            //rtk.ant2_antdeln = 0.0000;                              //Base Delta Northings
            //rtk.ant2_antdelu = 0.0000;                              //Base Delta Height
            //rtk.ant2_pos1 = 0.0000;                                 //Base Latitude/X
            //rtk.ant2_pos2 = 0.0000;                                 //Base Longitude/Y
            //rtk.ant2_pos3 = 0.0000;                                 //Base Geo Height/Z
            #endregion================ Position =================

            #region============== File  ================
            //rtk.file_cmdfile = "";
            //rtk.file_cmdfile2 = "";
            //rtk.file_cmdfile3 = "";
            //rtk.file_geoidfile = "";
            //rtk.file_dcbfile = "";
            //rtk.file_rcvantfile = "";
            //rtk.file_satantfile = "";
            //rtk.file_staposfile = "";
            //rtk.file_tempdir = "C:\\Temp";
            #endregion============== File  ================

            #region================ Misc ==================
            //rtk.misc_svrcycle = 10;                                 //Process Cycle (ms)
            //rtk.misc_buffsize = 32768;                              //Buffer Size (bytes)
            //rtk.misc_timeout = 10000;                               //Timeout Interval (ms)
            //rtk.misc_reconnect = 10000;                             //Reconnect Interval (ms)
            //rtk.misc_nmeacycle = 5000;                              //NMEA Cycle (ms)
            //rtk.misc_navmsgsel = peer.all;                          //Navigation Message Selection
            //rtk.misc_sbasatsel = 0;                                 //SBAS Sat Selection (0: all)
            //rtk.misc_timeinterp = false;
            //rtk.misc_startcmd = "";
            //rtk.misc_stopcmd = "";
            #endregion================ Misc ==================

            #region============  Output Streams  ==============
            ////Solution 1
            //rtk.outstr1_type = strtype.off;                         //Type
            //rtk.outstr1_format = ostrfmt.llh;                       //Format
            //rtk.outstr1_path = "";                                  //Path 

            ////Solution 2
            //rtk.outstr2_type = strtype.off;                         //Type
            //rtk.outstr2_format = ostrfmt.llh;                       //Format
            //rtk.outstr2_path = "";                                  //Path 
            #endregion============  Output Streams  ==============

            #region============ Input Strams  ===============
            //// (1) Rover
            //rtk.inpstr1_type = strtype.off;                         //Type
            //rtk.inpstr1_format = strfmt.skytraq;                    //Format
            //rtk.inpstr1_path = "";                                  //Path

            //// (2) Base
            //rtk.inpstr1_type = strtype.off;                         //Type
            //rtk.inpstr1_format = strfmt.skytraq;                    //Format
            //rtk.inpstr1_path = "";                                  //Path            
            //rtk.inpstr2_nmealat = 0;                                //Transmit NMEA GPGGA to Base Station Latitude
            //rtk.inpstr2_nmealon = 0;                                //Transmit NMEA GPGGA to Base Station Longitude
            //rtk.inpstr2_nmeareq = 0;                                //Transmit NMEA GPGGA to Base Station Longitude

            //// (3) Correction
            //rtk.inpstr3_type = strtype.off;                         //Type
            //rtk.inpstr3_format = strfmt.skytraq;                    //Format
            //rtk.inpstr3_path = "";                                  //Path
            #endregion============ Input Strams  ===============

            #region=========== Log Stream ===============
            //// (1) Rover
            //rtk.logstr1_type = strtype.off;                         //Rover File Type
            //rtk.logstr1_path = "";                                  //Rover Log File Path

            //// (2) Base
            //rtk.logstr2_type = strtype.off;                         //Base File Type
            //rtk.logstr2_path = "";                                  //Base Log File Path

            //// (3) Correction
            //rtk.logstr3_type = strtype.off;                         //Correction File Type
            //rtk.logstr3_path = "";                                  //Correction Log File Path
            #endregion=========== Log Stream ===============

            //rtk.restart();

            #endregion ======== GNSS Setting Options  ==========

            //rtk.load(new string[] { "./rtk.conf" });
            //rtk.restart();
            //rtk.save(new string[] { "./rtk.conf" });
            //rtk.help();

            GNSS_Functions.MainFrm = this;
            
        }

        public static void RenameTitle(string title)
        {
            MainScreenTitle.Text = title + " - GNSS Ghana";
            MainScreenTitle.Refresh();
        }

       
    }
}
