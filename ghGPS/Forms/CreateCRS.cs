using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ghGPS.Classes;
using ghGPS.Classes.CoordinateSystems;
using ghGPS.Forms;
using ghGPS.User_Forms;


namespace ghGPS.Forms
{
    public partial class CreateCRS : MetroSuite.MetroForm
    {
        public LinearUnit linearUnit = new LinearUnit(0, null, "EPSG", 10000, null, null, null);
        public IEllipsoid ellipsoid;
        public int UserCount = 0;

        public CreateCRS()
        {
            InitializeComponent();

            
            foreach (var item in Enum.GetNames(typeof(DatumType)))
            {
                cbxDatumType.Items.Add(item);
            }
            cbxDatumType.SelectedIndex = 3;
                      
            cbxAngularUnit.SelectedIndex = 0;

            var UserDefinedCRSTable = MainScreen._SQLiteHelper.GetEntries("UserDefinedCRSTable");

            //Count the number of user defined CRT
            UserCount = UserDefinedCRSTable.Count;
        }

        /// <summary>
        /// Resize the control to fit into flow panel
        /// </summary>
        public void ResizeItems()
        {
            foreach (var item in flpParameterList.Controls)
            {
                if (item is ProjectionParameterItem)
                {
                    var theItem = item as ProjectionParameterItem;
                    theItem.Width = (flpParameterList.VerticalScroll.Visible) ? 394 : 411;
                }
                else if (item is WGS84_ConversionInfoItem)
                {
                    var theItem = item as WGS84_ConversionInfoItem;
                    theItem.Width = (flpParameterList.VerticalScroll.Visible) ? 394 : 411;
                }
            }
        }

        private void flpParameterList_ControlAdded(object sender, ControlEventArgs e)
        {
            // Resize controls
            ResizeItems();
        }

        private void flpParameterList_ControlRemoved(object sender, ControlEventArgs e)
        {
            // Resize controls
            ResizeItems();
        }

        private void cbxProjectionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            flpParameterList.Controls.Clear();

            if (cbxProjectionType.SelectedIndex == 13)
            {
                for (int i = 0; i < 5; i++)
                {
                    
                }

                flpParameterList.Controls.Add(new ProjectionParameterItem());

                //flpParameterList.Controls.AddRange(new ProjectionParameterItem[5]);
            }

            flpParameterList.Controls.Add(new WGS84_ConversionInfoItem());
        }



        string GetProjectionClassName(string ClassPresentation)
        {
            return ClassPresentation.Replace(' ','_');
        }

        CoordinateSystemFactory cFac = new CoordinateSystemFactory();
        private void btnOK_Click(object sender, EventArgs e)
        {      
            //TO DO
            //Validate Entries 
            var TheAngular = AngularUnit.Degrees;

            switch (cbxAngularUnit.SelectedItem.ToString())
            {
                case "Degree":
                    TheAngular = AngularUnit.Degrees;
                    break;
                case "Radian":
                    TheAngular = AngularUnit.Radian;
                    break;
                case "Gon":
                    TheAngular = AngularUnit.Gon;
                    break;
                case "Grad":
                    TheAngular = AngularUnit.Grad;
                    break;
                default:
                    break;
            }

            var _WGS84_Conversion = new Wgs84ConversionInfo();

            List<ProjectionParameter> parameters = new List<ProjectionParameter>(5);
            foreach (Control ctrl in flpParameterList.Controls)
            {
                if (ctrl is ProjectionParameterItem)
                {
                    ProjectionParameterItem param = ctrl as ProjectionParameterItem;
                    parameters.Add(new ProjectionParameter(param.cbxParamName.SelectedItem.ToString(), double.Parse(param.tbxParamValue.Text)));
                }
                else if (ctrl is WGS84_ConversionInfoItem)
                {
                    WGS84_ConversionInfoItem _ToWGS84 = ctrl as WGS84_ConversionInfoItem;

                    if (_ToWGS84.chbxAllZeroValues.Checked)
                    {
                        _WGS84_Conversion = new Wgs84ConversionInfo();
                    }
                    else
                    {
                        _WGS84_Conversion = new Wgs84ConversionInfo(double.Parse(_ToWGS84.tbxDX.Text), double.Parse(_ToWGS84.tbxDY.Text), double.Parse(_ToWGS84.tbxDZ.Text), double.Parse(_ToWGS84.tbxRX.Text),
                       double.Parse(_ToWGS84.tbxRY.Text), double.Parse(_ToWGS84.tbxRZ.Text), double.Parse(_ToWGS84.tbxScale.Text), double.Parse(_ToWGS84.tbxXm.Text), double.Parse(_ToWGS84.tbxYm.Text), double.Parse(_ToWGS84.tbxZm.Text));
                    }
                   
                }
            }

            IHorizontalDatum datum = cFac.CreateHorizontalDatum(tbxDatumName.Text, (DatumType)Enum.Parse(typeof(DatumType), cbxDatumType.SelectedItem.ToString()),
               ellipsoid, _WGS84_Conversion);

            IGeographicCoordinateSystem gcs = cFac.CreateGeographicCoordinateSystem(tbxGeeograhicName.Text, TheAngular, datum,
                PrimeMeridian.Greenwich, new AxisInfo("Lon", AxisOrientationEnum.East), new AxisInfo("Lat", AxisOrientationEnum.North));

            IProjection projection = cFac.CreateProjection(cbxProjectionType.SelectedItem.ToString(), GetProjectionClassName(cbxProjectionType.SelectedItem.ToString()), parameters);

            IProjectedCoordinateSystem coordsys = cFac.CreateProjectedCoordinateSystem(tbxCRSName.Text, gcs, projection, linearUnit, new AxisInfo("East", AxisOrientationEnum.East), new AxisInfo("North", AxisOrientationEnum.North));

            MainScreen.InsertNewCRSToDatabase(coordsys.Name, (100000 + UserCount).ToString(), coordsys.WKT);

            //Reload from database            
            CoordinateSystem.ReloadCRS();
        }

        private void btnLinearUnits_Click(object sender, EventArgs e)
        {
            using (var mylinear = new LinearUnits())
            {
                var AllLinearUnit = MainScreen._SQLiteHelper.GetEntries("LinearUnitsTable");
                var LinearUnitlist = new List<LinearUnitListItem>();
                foreach (var item in AllLinearUnit)
                {
                   
                    LinearUnitlist.Add(new LinearUnitListItem(item.Text, item.SubItems[0].ToString(), item.SubItems[1].ToString(),
                        item.SubItems[2].ToString(), item.SubItems[3].ToString(), item.SubItems[4].ToString(), item.SubItems[5].ToString()));
                }

                mylinear.olvPointListTree.SetObjects(LinearUnitlist);

                if (mylinear.ShowDialog() == DialogResult.OK)
                {
                    linearUnit = mylinear.linearUnit;

                    tbxToMeterFactor.Text = linearUnit.MetersPerUnit.ToString();
                    tbxLinearUnitName.Text = linearUnit.Name;
                    tbxLinearUnitAbb.Text = linearUnit.Abbreviation;

                }
            }
        }

        private void btnEllipsoids_Click(object sender, EventArgs e)
        {
            using (var myEllipsoid = new TheEllipsoids())
            {
                var AllEllipsoid = MainScreen._SQLiteHelper.GetEntries("EllipsoidsTable");
                var Ellipsoidlist = new List<EllipsoidListItem>();
                foreach (var item in AllEllipsoid)
                {
                    Ellipsoidlist.Add(new EllipsoidListItem(item.Text, item.SubItems[0].ToString(), item.SubItems[1].ToString()));
                }

                myEllipsoid.olvPointListTree.SetObjects(Ellipsoidlist);

                if (myEllipsoid.ShowDialog() == DialogResult.OK)
                {                   
                    tbxEllipsoidName.Text = myEllipsoid.tbxEllipsoidName.Text;
                    tbxSemi_Major_Axis.Text = myEllipsoid.tbxSemi_Major_Axis.Text;
                    tbxInverseFlattening.Text = myEllipsoid.tbxInverseFlattening.Text;                   
                    
                    ellipsoid = cFac.CreateFlattenedSphere(tbxEllipsoidName.Text, double.Parse(tbxSemi_Major_Axis.Text), double.Parse(tbxInverseFlattening.Text), linearUnit);

                }
            }
        }
    }
}
