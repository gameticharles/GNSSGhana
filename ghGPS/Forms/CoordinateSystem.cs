using BrightIdeasSoftware;
using DotSpatial.Projections;
using ghGPS.Classes.CoordinateSystems;
using ghGPS.Classes.CoordinateSystems.IO;
using ghGPS.Classes;
using MetroSuite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ghGPS.Classes.SRIDReader;


namespace ghGPS.Forms
{
    public partial class CoordinateSystem : MetroForm
    {

        private ProjectionInfo _selectedCoordinateSystem;

        public bool Isprojection = false;

        public CoordinateSystem()
        {
            InitializeComponent();
   
            chbxEsri.Checked = false;

            Load += delegate
            {
                if (DesignMode) return;
                
                if (SelectedCoordinateSystem == null)
                {                    
                    //pnlProjectedCoord.Visible = true;
                    pnlGeographicCoord.Visible = !Isprojection;
                    
                    if (pnlGeographicCoord.Visible)
                    {
                        btnGeographicCoord.PerformClick();
                    }
                    else
                    {
                        btnProjectedCoord.PerformClick();
                    }
                    
                }
            };

            cbxHotItem.SelectedIndex = 3; // Translucent
            cbxExpander.SelectedIndex = 2; // triangles
            
            treeCRS.CanExpandGetter = model => ((CRSCategory)model).CRSCategories.Count > 0;
            treeCRS.ChildrenGetter = delegate (object model)
            {
                return ((CRSCategory)model).CRSCategories;
            };

            // Show the system description for this object
            this.clnCRS.AspectGetter = delegate (object x) {
                return ((CRSCategory)x).CategoryName;
            };

            // Show the system description for this object
            this.cnlCSRCodeName.AspectGetter = delegate (object x) {
                return ((CRSCategory)x).CRSCodeName;
            };

            // Show the system description for this object
            this.clnAutorityCode.AspectGetter = delegate (object x) {
                return ((CRSCategory)x).AutorityCode;
            };

            //treeCRS.SetObjects(model);
        }

        private static ProjectionCategory GetProjectionCategory(ProjectionInfo projectionInfo)
        {
            var holder = projectionInfo.IsLatLon
                  ? (ICoordinateSystemCategoryHolder)KnownCoordinateSystems.Geographic
                  : KnownCoordinateSystems.Projected;
            var selectedAsStr = projectionInfo.ToString();
            var selectedEsri = projectionInfo.ToEsriString();
            var selectedEPSRICode = projectionInfo.AuthorityCode.ToString();
            foreach (var name in holder.Names)
            {
                var cat = holder.GetCategory(name);
                foreach (var projName in cat.Names)
                {
                    var proj = cat.GetProjection(projName);
                    if (proj.ToString() == selectedAsStr && proj.ToEsriString() == selectedEsri)
                    {
                        return new ProjectionCategory { CategoryName = name, ProjectionFieldName = projName , EPSGCode = selectedEPSRICode };
                    }
                }
            }
            return null;
        }

        private class ProjectionCategory
        {
            public string CategoryName { get; set; }
            public string ProjectionFieldName { get; set; }
            public string EPSGCode { get; set; }
        }

        /// <summary>
        /// Gets or sets the currently chosen coordinate system
        /// </summary>

        public ProjectionInfo SelectedCoordinateSystem
        {
            get { return _selectedCoordinateSystem; }
            set
            {
                if (_selectedCoordinateSystem == value) return;

                _selectedCoordinateSystem = value;
                if (_selectedCoordinateSystem == null)
                {
                    nudEpsgCode.Text = "";
                    rtbxSelectedCRSDetails.Text = null;
                    return;
                }                
                
                rtbxSelectedCRSDetails.Text = !chbxEsri.Checked
                    ? treeCRS.SelectedItem.GetSubItem(2).Text
                    : _selectedCoordinateSystem.ToProj4String();

            }
        }

        #region Button Effect
        /// <summary>
        /// Add check effect to the button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void buttonEffect(object sender, EventArgs e)
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                {
                    if (control is Button)
                    {
                        if ((control as Button) == (Button)sender)
                        {
                            if (!(control as Button).Name.StartsWith("btnSub") && (control as Button).Parent.Parent == pnlButtons)
                            {
                                (control as Button).BackColor = Color.FromArgb(100, 160, 199);
                            }                            

                        }
                        else
                        {
                            if (!(control as Button).Name.StartsWith("btnSub") && (control as Button).Parent.Parent == pnlButtons)
                            {
                                (control as Button).BackColor = Color.FromArgb(90, 139, 196);
                            }
                            
                        }
                    }
                    else
                    {
                        func(control.Controls);
                    }

                }
            };

            func(Controls);

        }
        #endregion

        private ICoordinateSystemCategoryHolder CurrentCategoryHolder
        {
            get
            {
                return IsProjected //Select Projected Btn
                ? (ICoordinateSystemCategoryHolder)KnownCoordinateSystems.Projected
                : KnownCoordinateSystems.Geographic;
            }
        }

        private bool IsProjected = true;
        private bool IsAllCRS = true;
        private void btnGeographicCoord_Click(object sender, EventArgs e)
        {
            buttonEffect(sender, e);
            lblStatus.Text = "";

            
            //IsAllCRS = (((Button)sender).Name != nameof(btnAllCRS)) ? false : true;
            
            if (((Button)sender).Name == nameof(btnProjectedCoord)) //Projected
            {                
                crsCategories = AllProjected;
                itemCount = AllProjectedCount;
            }
            else if (((Button)sender).Name == nameof(btnGeographicCoord)) //Geographic
            {                               
                crsCategories = AllGeog;
                itemCount = AllGeogCount;
            }           
            else                                //User defined coordinate system
            {
                crsCategories = UserDefined;
                itemCount = AllGeogCount;
            }

            // Get all Categories
            treeCRS.SetObjects(crsCategories);
            treeCRS.Refresh();

            //Filter if demanded
            if (tbxFilter.Text.Length > 1)
            {
                tbxFilter_TextChanged(sender, e);
            }

            if (_disableEvents) return;
            
        }

        static IEnumerable<WKTstring> SRIDs = MainScreen.AllSRID as IEnumerable<WKTstring>;
        static List<CRSCategory> crsCategories = new List<CRSCategory>();
        static List<CRSCategory> AllProjected = new List<CRSCategory>();
        static List<CRSCategory> AllGeog = new List<CRSCategory>();
        static List<CRSCategory> UserDefined = new List<CRSCategory>();
        static int AllProjectedCount = 0;
        static int AllGeogCount = 0;
        public static int AllUserDefinedCount { get; set; } = 0;

        int itemCount=0;

        /// <summary>
        /// Load and populate all CRS
        /// </summary>
        public static void ReloadCRS()
        {
            
            //LoadAllProjected();
            LoadAllCategories();
            //LoadAllGeograhic();
            LoadAllUserDefine();
        }
               
        private static void LoadAllCategories()
        {             

            AllGeogCount += 18;
           
            //Get all Categories
            foreach (WKTstring SRID in SRIDs)
            {
                if (SRID.WKType.Contains("GEOGCS"))
                {
                    AllGeogCount++;

                    //Get All sub item
                    AllGeog.Add(new CRSCategory(SRID.WKName, SRID.WKID.ToString(), SRID.WKT));
                }
            }  
            
            var Albers_Conic_Equal_Area = new CRSCategory("Albers Conic Equal Area", "", "");
            var Cassini_Soldner = new CRSCategory("Cassini Soldner", "", "");
            var Hotine_Oblique_Mercator = new CRSCategory("Hotine Oblique Mercator", "", "");
            var Krovak = new CRSCategory("Krovak", "", "");
            var Lambert_Azimuthal_Equal_Area = new CRSCategory("Lambert Azimuthal Equal Area", "", "");
            var Lambert_Conformal_Conic_1SP = new CRSCategory("Lambert Conformal Conic 1SP", "", "");
            var Lambert_Conformal_Conic_2SP = new CRSCategory("Lambert Conformal Conic 2SP", "", "");
            var Lambert_Conformal_Conic_2SP_Belgium = new CRSCategory("Lambert Conformal Conic 2SP Belgium", "", "");
            var Mercator_1SP = new CRSCategory("Mercator 1SP", "", "");
            var Oblique_Stereographic = new CRSCategory("Oblique Stereographic", "", "");
            var Polar_Stereographic = new CRSCategory("Polar Stereographic", "", "");
            var Polyconic = new CRSCategory("Polyconic", "", "");
            var Transverse_Mercator = new CRSCategory("Transverse Mercator", "", "");
            var Transverse_Mercator_South_Orientated = new CRSCategory("Transverse Mercator South_Orientated", "", "");
            var Tunisia_Mining_Grid = new CRSCategory("Tunisia Mining Grid", "", "");
            var Undefined = new CRSCategory("Undefined: Others", "", "");


            //Get all Categories
            foreach (WKTstring SRID in SRIDs)
            {                
                if (SRID.WKType.Contains("PROJCS"))
                {
                    AllProjectedCount++;

                    if (SRID.WKProjType.Contains("Albers_Conic_Equal_Area"))
                    {
                        Albers_Conic_Equal_Area.CRSCategories.Add(new CRSCategory(SRID.WKName, SRID.WKID.ToString(), SRID.WKT));
                    }
                    else if (SRID.WKProjType.Contains("Cassini_Soldner"))
                    {
                        Cassini_Soldner.CRSCategories.Add(new CRSCategory(SRID.WKName, SRID.WKID.ToString(), SRID.WKT));
                    }
                    else if (SRID.WKProjType.Contains("Hotine_Oblique_Mercator"))
                    {
                        Hotine_Oblique_Mercator.CRSCategories.Add(new CRSCategory(SRID.WKName, SRID.WKID.ToString(), SRID.WKT));
                    }
                    else if (SRID.WKProjType.Contains("Krovak"))
                    {
                        Krovak.CRSCategories.Add(new CRSCategory(SRID.WKName, SRID.WKID.ToString(), SRID.WKT));
                    }
                    else if (SRID.WKProjType.Contains("Lambert_Azimuthal_Equal_Area"))
                    {
                        Lambert_Azimuthal_Equal_Area.CRSCategories.Add(new CRSCategory(SRID.WKName, SRID.WKID.ToString(), SRID.WKT));
                    }
                    else if (SRID.WKProjType.Contains("Lambert_Conformal_Conic_1SP"))
                    {
                        Lambert_Conformal_Conic_1SP.CRSCategories.Add(new CRSCategory(SRID.WKName, SRID.WKID.ToString(), SRID.WKT));
                    }
                    else if (SRID.WKProjType.Contains("Lambert_Conformal_Conic_2SP_Belgium"))
                    {
                        Lambert_Conformal_Conic_2SP_Belgium.CRSCategories.Add(new CRSCategory(SRID.WKName, SRID.WKID.ToString(), SRID.WKT));
                    }
                    else if (SRID.WKProjType.Contains("Lambert_Conformal_Conic_2SP"))
                    {
                        Lambert_Conformal_Conic_2SP.CRSCategories.Add(new CRSCategory(SRID.WKName, SRID.WKID.ToString(), SRID.WKT));
                    }
                    else if (SRID.WKProjType.Contains("Mercator_1SP"))
                    {
                        Mercator_1SP.CRSCategories.Add(new CRSCategory(SRID.WKName, SRID.WKID.ToString(), SRID.WKT));
                    }
                    else if (SRID.WKProjType.Contains("Oblique_Stereographic"))
                    {
                        Oblique_Stereographic.CRSCategories.Add(new CRSCategory(SRID.WKName, SRID.WKID.ToString(), SRID.WKT));
                    }
                    else if (SRID.WKProjType.Contains("Polar_Stereographic"))
                    {
                        Polar_Stereographic.CRSCategories.Add(new CRSCategory(SRID.WKName, SRID.WKID.ToString(), SRID.WKT));
                    }
                    else if (SRID.WKProjType.Contains("Polyconic"))
                    {
                        Polyconic.CRSCategories.Add(new CRSCategory(SRID.WKName, SRID.WKID.ToString(), SRID.WKT));
                    }
                    else if (SRID.WKProjType.Contains("Transverse_Mercator_South_Orientated"))
                    {
                        Transverse_Mercator_South_Orientated.CRSCategories.Add(new CRSCategory(SRID.WKName, SRID.WKID.ToString(), SRID.WKT));
                    }
                    else if (SRID.WKProjType.Contains("Transverse_Mercator"))
                    {
                        Transverse_Mercator.CRSCategories.Add(new CRSCategory(SRID.WKName, SRID.WKID.ToString(), SRID.WKT));
                    }
                    else if (SRID.WKProjType.Contains("Tunisia_Mining_Grid"))
                    {
                        Tunisia_Mining_Grid.CRSCategories.Add(new CRSCategory(SRID.WKName, SRID.WKID.ToString(), SRID.WKT));
                    }
                    else
                    {
                        Undefined.CRSCategories.Add(new CRSCategory(SRID.WKProjType, SRID.WKID.ToString(), SRID.WKT));
                    }
                }
            }

            //Get All sub item
            AllProjected.Add(Albers_Conic_Equal_Area);
            AllProjected.Add(Cassini_Soldner);
            AllProjected.Add(Hotine_Oblique_Mercator);
            AllProjected.Add(Krovak);
            AllProjected.Add(Lambert_Azimuthal_Equal_Area);
            AllProjected.Add(Lambert_Conformal_Conic_1SP);
            AllProjected.Add(Lambert_Conformal_Conic_2SP);
            AllProjected.Add(Lambert_Conformal_Conic_2SP_Belgium);
            AllProjected.Add(Mercator_1SP);
            AllProjected.Add(Oblique_Stereographic);
            AllProjected.Add(Polar_Stereographic);
            AllProjected.Add(Polyconic);
            AllProjected.Add(Transverse_Mercator);
            AllProjected.Add(Transverse_Mercator_South_Orientated);
            AllProjected.Add(Tunisia_Mining_Grid);
            AllProjected.Add(Undefined);
           // AllProjected.Add(Categories);

            GC.Collect();
   
        }

        private void LoadAllProjected()
        {
            treeCRS.Items.Clear();

            AllProjected = new List<CRSCategory>();

            IsProjected = true;

            //Get all Categories
            foreach (var name in CurrentCategoryHolder.Names)
            {
                AllProjectedCount++;
                var Categories = new CRSCategory(name.SplitCamelCase(), "", "");

                //Get All sub item
                var c = CurrentCategoryHolder.GetCategory(name);
                if (c == null) return;

                foreach (var subname in c.Names)
                {
                    AllProjectedCount++;
                    Categories.CRSCategories.Add(new CRSCategory(subname.SplitCamelCase(), c.GetProjection(subname).ToEsriString(), c.GetProjection(subname).ToEsriString()));

                }

                AllProjected.Add(Categories);
            }
                       
            GC.Collect();

        }

        private void LoadAllGeograhic()
        {
            treeCRS.Items.Clear();

            AllGeog = new List<CRSCategory>();

            IsProjected = false;

            //Get all Categories
            foreach (var name in CurrentCategoryHolder.Names)
            {
                AllGeogCount++;
                var Categories = new CRSCategory(name.SplitCamelCase(), "", "");

                //Get All sub item
                var c = CurrentCategoryHolder.GetCategory(name);
                if (c == null) return;

                foreach (var subname in c.Names)
                {
                    AllGeogCount++;
                    Categories.CRSCategories.Add(new CRSCategory(subname.SplitCamelCase(), c.GetProjection(subname).ToEsriString(), c.GetProjection(subname).ToEsriString()));

                }

                AllGeog.Add(Categories);
            }

    
            GC.Collect();

        }

        private static void LoadAllUserDefine()
        {
            
            UserDefined = new List<CRSCategory>();

            //Add Items to the respective page
            var UserDefinedCRSTable = MainScreen._SQLiteHelper.GetEntries("UserDefinedCRSTable");
                       
            foreach (var newCRS in UserDefinedCRSTable)
            {
                AllUserDefinedCount +=1;
                UserDefined.Add(new CRSCategory(newCRS.Text, newCRS.SubItems[0].ToString(), newCRS.SubItems[1].ToString()));
            }
            
        }
               

        class CRSCategory
        {
            public string CategoryName { get; set; }
            public string AutorityCode { get; set; }
            public string CRSCodeName { get; set; }

            public List<CRSCategory> CRSCategories { get; set; }

            public CRSCategory(string CategoryName, string AutorityCode, string CRSCodeName)
            {
                this.CategoryName = CategoryName;
                this.AutorityCode = AutorityCode;
                this.CRSCodeName = CRSCodeName;
                CRSCategories = new List<CRSCategory>();
            }
        }

        private void treeCRS_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //if (_disableEvents) return;

            //var c = CurrentCategoryHolder.GetCategory(treeCRS.SelectedItem.Name);
            //if (c == null) return;
            //_userChanges = true;
            //SelectedCoordinateSystem = c.GetProjection(treeCRS.SelectedItem.Name);
            //_userChanges = false;
        }

        private void tbxFilter_TextChanged(object sender, EventArgs e)
        {
            treeCRS.ExpandAll();
            rtbxSelectedCRSDetails.Text = "";
            tbxSelectedCRSName.Text = "";
            try
            {
                int EPSGCode = 0;

                if (int.TryParse(tbxFilter.Text,out EPSGCode) && EPSGCode >= 2000)
                {

                   
                    SelectedCoordinateSystem = ProjectionInfo.FromEsriString(treeCRS.SelectedItem.GetSubItem(2).Text);
                    
                    TimedFilter(treeCRS, SelectedCoordinateSystem.ToProj4String());

                    if (rtbxSelectedCRSDetails.Text.Length>5)
                    {
                        treeCRS.EmptyListMsg = "An undefined CRS \nBut found with EPSG: " + EPSGCode.ToString() + "\nCRS Type: " + (SelectedCoordinateSystem.IsLatLon ? "Geographic CS" : "Projected CS");
                        GNSS_Functions.CrsSelectdName = "An undefined CRS";
                    }
                    else
                    {
                        treeCRS.EmptyListMsg = "No match found";
                    }
                    
                    
                }
                else
                {
                    TimedFilter(treeCRS, tbxFilter.Text);
                }                

                treeCRS.Refresh();

            }
            catch (Exception)
            {
                TimedFilter(treeCRS, tbxFilter.Text);
            }

        }

        private void cbxExpander_SelectedIndexChanged(object sender, EventArgs e)
        {
            var treeColumnRenderer = treeCRS.TreeColumnRenderer;

            ComboBox cb = (ComboBox)sender;
            switch (cb.SelectedIndex)
            {
                case 0:
                    treeColumnRenderer.IsShowGlyphs = false;
                    break;
                case 1:
                    treeColumnRenderer.IsShowGlyphs = true;
                    treeColumnRenderer.UseTriangles = false;
                    break;
                case 2:
                    treeColumnRenderer.IsShowGlyphs = true;
                    treeColumnRenderer.UseTriangles = true;
                    break;
            }

            // Cause a redraw so that the changes to the renderer take effect
            treeCRS.Refresh();
        }        

        public void TimedFilter(ObjectListView olv, string txt)
        {
            TimedFilter(olv, txt, 0); //Contains
        }

        private IList FilteredObjects;
        private bool _disableEvents;

        public void TimedFilter(ObjectListView olv, string txt, int matchKind)
        {
            TextMatchFilter filter = null;
            var allcount = itemCount;

            if (!String.IsNullOrEmpty(txt))
            {
                switch (matchKind)
                {
                    case 0:
                    default:
                        filter = TextMatchFilter.Contains(olv, txt);
                        break;
                    case 1:
                        filter = TextMatchFilter.Prefix(olv, txt);
                        break;
                    case 2:
                        filter = TextMatchFilter.Regex(olv, txt);
                        break;
                }
            }

            // Text highlighting requires at least a default renderer
            if (olv.DefaultRenderer == null)
                olv.DefaultRenderer = new HighlightTextRenderer(filter);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();            

            olv.AdditionalFilter = filter;
            //olv.Invalidate();
            stopWatch.Stop();

            FilteredObjects = olv.Objects as IList;
            
            if (FilteredObjects == null)
                this.lblStatus.Text = String.Format("Filtered in {0}ms", stopWatch.ElapsedMilliseconds);
            else
                this.lblStatus.Text = String.Format("Filtered {0} Categories with {1} items down to {2} items in {3}ms", FilteredObjects.Count, allcount, olv.Items.Count, stopWatch.ElapsedMilliseconds);
        }

        public void ChangeHotItemStyle(ObjectListView olv, ComboBox cb)
        {
            olv.UseTranslucentHotItem = false;
            olv.UseHotItem = true;
            olv.UseExplorerTheme = false;

            switch (cb.SelectedIndex)
            {
                case 0:
                    olv.UseHotItem = true;                    
                    break;
                case 1:
                    HotItemStyle hotItemStyle = new HotItemStyle();
                    hotItemStyle.ForeColor = Color.AliceBlue;
                    hotItemStyle.BackColor = Color.FromArgb(255, 64, 64, 64);
                    olv.HotItemStyle = hotItemStyle;
                    break;
                case 2:
                    RowBorderDecoration rbd = new RowBorderDecoration();
                    rbd.BorderPen = new Pen(Color.SeaGreen, 2);
                    rbd.FillBrush = null;
                    rbd.CornerRounding = 4.0f;
                    HotItemStyle hotItemStyle2 = new HotItemStyle();
                    hotItemStyle2.Decoration = rbd;
                    olv.HotItemStyle = hotItemStyle2;
                    break;
                case 3:
                    olv.UseTranslucentHotItem = true;
                    break;
                case 4:
                    HotItemStyle hotItemStyle3 = new HotItemStyle();
                    hotItemStyle3.Decoration = new LightBoxDecoration();
                    olv.HotItemStyle = hotItemStyle3;
                    break;
                case 5:
                    olv.FullRowSelect = true;
                    olv.UseHotItem = false;
                    olv.UseExplorerTheme = true;
                    break;
            }
            olv.Invalidate();
        }

        public void HandleHotItemChanged(object sender, HotItemChangedEventArgs e)
        {
            if (sender == null)
            {
                //this.Form.toolStripStatusLabel3.Text = "";
                return;
            }

            switch (e.HotCellHitLocation)
            {
                case HitTestLocation.Nothing:
                    //this.Form.toolStripStatusLabel3.Text = @"Over nothing";
                    break;
                case HitTestLocation.Header:
                case HitTestLocation.HeaderCheckBox:
                case HitTestLocation.HeaderDivider:
                    //this.Form.toolStripStatusLabel3.Text = String.Format("Over {0} of column #{1}", e.HotCellHitLocation, e.HotColumnIndex);
                    break;
                case HitTestLocation.Group:
                    //this.Form.toolStripStatusLabel3.Text = String.Format("Over group '{0}', {1}", e.HotGroup.Header, e.HotCellHitLocationEx);
                    break;
                case HitTestLocation.GroupExpander:
                    //this.Form.toolStripStatusLabel3.Text = String.Format("Over group expander of '{0}'", e.HotGroup.Header);
                    break;
                default:
                    //this.Form.toolStripStatusLabel3.Text = String.Format("Over {0} of ({1}, {2})", e.HotCellHitLocation, e.HotRowIndex, e.HotColumnIndex);
                    break;
            }
        }

        private void cbxHotItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeHotItemStyle(treeCRS, (MetroComboBox)sender);
        }
            
        private void treeCRS_SelectionChanged(object sender, EventArgs e)
        {
            tbxSelectedCRSName.Text = "";
            //ProjectionInfo proj;
            try
            {
                if (treeCRS.SelectedItem.GetSubItem(1).Text != "" && treeCRS.SelectedItem.GetSubItem(2).Text != "" )
                {
                    tbxSelectedCRSName.Text = treeCRS.SelectedItem.GetSubItem(0).Text;

                    if (IsAllCRS)
                    {
                        SelectedCoordinateSystem = ProjectionInfo.FromEsriString(treeCRS.SelectedItem.GetSubItem(2).Text);
                        var EPSGCode = treeCRS.SelectedItem.GetSubItem(1).Text.Substring(5);
                        nudEpsgCode.Text = EPSGCode;
                    }
                    else
                    {
                        SelectedCoordinateSystem = ProjectionInfo.FromEsriString(treeCRS.SelectedItem.GetSubItem(1).Text);
                    }
                   
                    //var c = CurrentCategoryHolder.GetCategory(treeCRS.SelectedItem.GetSubItem(2).Text);
                    //SelectedCoordinateSystem = c.GetProjection(treeCRS.SelectedItem.GetSubItem(1).Text);
                    
                                     
                    //rtbxSelectedCRSDetails.Text = treeCRS.SelectedItem.GetSubItem(2).Text;
                }
            }
            catch (Exception)
            {
                                
            }
            
        }

        private void chbEsri_CheckedChanged(object sender, bool isChecked)
        {
            if (SelectedCoordinateSystem == null) return;

            rtbxSelectedCRSDetails.Text = !chbxEsri.Checked
                    ? SelectedCoordinateSystem.ToEsriString()
                    : SelectedCoordinateSystem.ToProj4String();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            
            GNSS_Functions.CrsSelectdParameters = rtbxSelectedCRSDetails.Text;
            GNSS_Functions.CrsSelectdName = tbxSelectedCRSName.Text;
            var pcs = CoordinateSystemWktReader.Parse(GNSS_Functions.CrsSelectdParameters) as Projection;
           
        }

        private void CoordinateSystem_Load(object sender, EventArgs e)
        {
            treeCRS.Items.Clear();

            ReloadCRS();
        }

        private void GridLinesSwitch_CheckedChanged(object sender, bool isChecked)
        {
            if (editSwitch.Checked)
            {
                treeCRS.GridLines = false;
            }
            else
            {
                treeCRS.GridLines = true;
            }
        }

        private void chbxReadable_CheckedChanged(object sender, bool isChecked)
        {

        }

        private void btnCreateGrisSystem_Click(object sender, EventArgs e)
        {
            using (CreateCRS createCRS = new CreateCRS())
            {
                createCRS.ShowDialog();
            }
        }
    }
}
