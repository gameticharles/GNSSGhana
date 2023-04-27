using DotSpatial.Projections;
using ghGPS.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ghGPS.Classes.GNSSLib;
using libRTK;
using ReinGametiMatrixLib.Matricies;
using static ghGPS.Classes.SRIDReader;


namespace ghGPS.Forms
{
    public partial class Tester : Form
    {
        
        public Tester()
        {
            InitializeComponent();
        }

        IEnumerable<WKTstring> SRIDs = MainScreen.AllSRID as IEnumerable<WKTstring>;
        //public string ReprojectPoint(int fromEPSG, int toEPSG, string x, string y)
        //{
        //    //// Replace this with your appropriate proj4 expression..
        //    //string esri_102697_proj4 = @"+proj=tmerc +lat_0=35.83333333333334 +lon_0=-92.5 +k=0.9999333333333333 +x_0=500000.0000000002 +y_0=0 +ellps=GRS80 +datum=NAD83 +to_meter=0.3048006096012192 +no_defs";
        //    OSGeo.OSR.SpatialReference src = new OSGeo.OSR.SpatialReference("");
        //    src.ImportFromEPSG(fromEPSG);
            
        //    //// Replace this with your appropriate proj4 expression..
        //    //string epsg_wgs1984_proj4 = @"+proj=latlong +datum=WGS84 +no_defs";
        //    OSGeo.OSR.SpatialReference dst = new OSGeo.OSR.SpatialReference("");
        //    dst.ImportFromEPSG(toEPSG);
            
        //    // Init the transformer object.
        //    OSGeo.OSR.CoordinateTransformation ct = new OSGeo.OSR.CoordinateTransformation(src, dst);
            
        //    double[] p = new double[3];
        //    p[0] = Convert.ToDouble(x);
        //    p[1] = Convert.ToDouble(y);
        //    p[2] = 0; // I don't remember if "Z" is required, but I didn't use it.

        //    ct.TransformPoint(p); // Mutate the point coordinates here..
            
        //    return "x:" + Convert.ToString(p[0]) + " y:" + Convert.ToString(p[1]) + " z:" + Convert.ToString(p[2]);
        //}

        private void metroButton1_Click(object sender, EventArgs e)
        {
                    
            //result.Text = ReprojectPoint(4326, 32630, (34.590864037).ToString(), (135.512912303).ToString());


            //var _wgs84 = ProjectionInfo.FromEpsgCode(32630);
            //var ghGridProj = ProjectionInfo.FromEpsgCode(2136); //Projected Coordinates
            //var ghGridGeo = ProjectionInfo.FromEpsgCode(4168); //Projected Coordinates

            
            //double[] x = { double.Parse(metroTextbox1.Text)};
            //double[] y = { double.Parse(metroTextbox2.Text)};
            //double[] z = { double.Parse(metroTextbox3.Text)};


            ////rewrite xy array for input into Proj4
            //double[] xy = new double[2 * x.Length];
            //int ixy = 0;
            //for (int i = 0; i <= x.Length - 1; i++)
            //{
            //    xy[ixy] = x[i];
            //    xy[ixy + 1] = y[i];
            //    z[i] = 0;
            //    ixy += 2;
            //}

            //Reproject.ReprojectPoints(xy, z, ghGridProj, ghGridGeo, 0, x.Length);
            //CoordinateSystemConversion cor = new CoordinateSystemConversion();
            //ixy = 0;
            //for (int i = 0; i <= x.Length - 1; i++)
            //{
            //    result.Text = "X = " + cor.ConvertToDMSString(xy[ixy], false, true) + "       " + "Y = " + cor.ConvertToDMSString(xy[ixy+1], true, true) + "       " + "Z = " + z[i].ToString();

            //    ixy += 2;
            //}


            //xy = new double[2 * x.Length];
            //ixy = 0;
            //for (int i = 0; i <= x.Length - 1; i++)
            //{
            //    xy[ixy] = x[i];
            //    xy[ixy + 1] = y[i];
            //    z[i] = 0;
            //    ixy += 2;
            //}

            //Reproject.ReprojectPoints(xy, z, ghGridProj, _wgs84, 0, x.Length);

            //ixy = 0;
            //for (int i = 0; i <= x.Length - 1; i++)
            //{
            //    result.Text += "X = " + xy[ixy].ToString() + "       " + "Y = " + xy[ixy + 1].ToString() + "       " + "Z = " + z[i].ToString();

            //    ixy += 2;
            //}

            //Console.Write("Press any key to continue . . . ");

            //Console.WriteLine();



            //Console.WriteLine(ghGridGeo.ToProj4String());

            //Console.WriteLine(ghGridProj.ToProj4String());

            


            //================TEST Coordinate System=======================
            CoordinateSystemConversion cor = new CoordinateSystemConversion();

            //Getting Default or built in Grid system for WGS 84
            var wgs84 = new GridSystem().WGS84();
            //Create new coordinate system from built functions
            var ghGrid = new GridSystem().GhanaNationalGrid(new TransParams().GH7TransParams());
            result.Text = "";

            //////================Projection=====================
            ////var LatLong = cor.ENU2LatLonH(wgs84, 738036.67594, 658581.61084, 292.56);
            //var LatLonH = cor.XYZ2LatLonH(wgs84, new double[] { double.Parse(metroTextbox1.Text), double.Parse(metroTextbox2.Text), double.Parse(metroTextbox3.Text) });
            

            //================Geodetic To Cartesian=====================
            var LatLonH = new double[] { double.Parse(metroTextbox1.Text), double.Parse(metroTextbox2.Text), double.Parse(metroTextbox3.Text) };// cor.LatLonH2XYZ(wgs84, LatLonH);
            result.Text += "\nLat = " + cor.ConvertToDMSString(LatLonH[0], true, true) + "  " + "Lon = " + cor.ConvertToDMSString(LatLonH[1], false, true) + "       " + "Z = " + LatLonH[2].ToString();


            var XYZ = cor.LatLonH2XYZ(ghGrid, LatLonH);
            result.Text += "\nX = " + XYZ[0].ToString() + "  " + "\nY = " + XYZ[1].ToString() + "  " + "\nZ = " + XYZ[2].ToString();


            //================Transformation==================
            //Use inbuilt transformation parameters for Ghana
           
            var X1Y1Z1 = cor.XYZTransformation(ghGrid.TransParam, XYZ, true);
            result.Text += "\nX1 = " + X1Y1Z1[0].ToString() + "  " + "\nY1 = " + X1Y1Z1[1].ToString() + "   " + "\nZ1 = " + X1Y1Z1[2].ToString();
            

            //================Cartesian To Geodetic=====================
            var LatLonH1 = cor.XYZ2LatLonH(wgs84, new double[] { double.Parse(metroTextbox1.Text), double.Parse(metroTextbox2.Text), double.Parse(metroTextbox3.Text) });
            //result.Text += "\nLat = " + cor.ConvertToDMSString(LatLonH1[0], true, true) + "  " + "Lon = " + cor.ConvertToDMSString(LatLonH1[1], false, true) + "       " + "Z = " + LatLonH1[2].ToString();
            

            //================Projection=====================
            //var Test = new double[] { double.Parse(metroTextbox1.Text), double.Parse(metroTextbox2.Text), double.Parse(metroTextbox3.Text) };// cor.LatLonH2XYZ(wgs84, LatLonH);
            var ENU = cor.LatLonH2ENU(wgs84, LatLonH1);
            result.Text += "E = " + ENU[0] + "       " + "N = " + ENU[1] + "       " + "Z = " + ENU[2] + "       " + "conv: = " + cor.ConvertToDMSString(double.Parse(ENU[3]), false, false)
                    + "       " + "Grid Scale: " + ENU[4] + "       " + "Zone: " + ENU[5] + " " + ENU[6];
            
            //================Reprojection===================
            //var LatLon = cor.ENU2LatLonH(ghGrid, new double[] { double.Parse(ENU[0]), double.Parse(ENU[1]) });
            //var LatLon = cor.ENU2LatLonH(ghGrid.a, ghGrid.invF, double.Parse(ENU[1]) * 0.304799710181509, double.Parse(ENU[0]) * 0.304799710181509, double.Parse(ENU[2]), ghGrid.ko, ghGrid.FN, ghGrid.FE, ghGrid.φ0, ghGrid.λ0);
            //result.Text += "\nLat = " + cor.ConvertToDMSString(LatLon[0], false, true) + "       " + "Lon = " + cor.ConvertToDMSString(LatLon[1], true, true) + "       " + "Z = " + LatLon[2].ToString();

            //var LatLon = cor.ProjectToLatLon(new double[] { double.Parse(ENU[0]), double.Parse(ENU[1]) }, new double[] { double.Parse(ENU[2]) } ,2136, 4168 );
            //result.Text += "\nLon = " + cor.ConvertToDMSString(LatLon[0], false, true) + "       " + "Lat = " + cor.ConvertToDMSString(LatLon[1], true, true) + "       " + "Z = " + LatLon[2].ToString();


            
            //const string wkt4326 = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]]";
            //const string wkt3857 = "PROJCS[\"Popular Visualisation CRS / Mercator\", GEOGCS[\"Popular Visualisation CRS\", DATUM[\"Popular Visualisation Datum\", SPHEROID[\"Popular Visualisation Sphere\", 6378137, 0, AUTHORITY[\"EPSG\",\"7059\"]], TOWGS84[0, 0, 0, 0, 0, 0, 0], AUTHORITY[\"EPSG\",\"6055\"] ], PRIMEM[\"Greenwich\", 0, AUTHORITY[\"EPSG\", \"8901\"]], UNIT[\"degree\", 0.0174532925199433, AUTHORITY[\"EPSG\", \"9102\"]], AXIS[\"E\", EAST], AXIS[\"N\", NORTH], AUTHORITY[\"EPSG\",\"4055\"] ], PROJECTION[\"Mercator\"], PARAMETER[\"False_Easting\", 0], PARAMETER[\"False_Northing\", 0], PARAMETER[\"Central_Meridian\", 0], PARAMETER[\"Latitude_of_origin\", 0], UNIT[\"metre\", 1, AUTHORITY[\"EPSG\", \"9001\"]], AXIS[\"East\", EAST], AXIS[\"North\", NORTH], AUTHORITY[\"EPSG\",\"3785\"]]";
            //const string wkt3395 = "PROJCS[\"WGS 84 / World Mercator\",GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],PROJECTION[\"Mercator_1SP\"],PARAMETER[\"central_meridian\",0],PARAMETER[\"scale_factor\",1],PARAMETER[\"false_easting\",0],PARAMETER[\"false_northing\",0],AXIS[\"Easting\",EAST],AXIS[\"Northing\",NORTH],AUTHORITY[\"EPSG\",\"3395\"]]";
            //var cf = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
            //var f = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();
            //var sys4326 = cf.CreateFromWkt(GetCSbyID(SRIDs,4326));
            //var sys3857 = cf.CreateFromWkt(GetCSbyID(SRIDs, 32630));
            //var sys3395 = cf.CreateFromWkt(wkt3395);
            //var transformTo3875 = f.CreateFromCoordinateSystems(sys4326, sys3857);
            //var transformTo3395 = f.CreateFromCoordinateSystems(sys4326, sys3395);

            //var re = transformTo3875.MathTransform.Transform(new double[]{ 34.590864037, 135.512912303,0 });
            //double[] r = transformTo3875.MathTransform.Inverse().Transform(re);
            //result.Text  = "x:" + Convert.ToString(r[0]) + " y:" + Convert.ToString(re[1]) + " z:" + Convert.ToString(r[2]);
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            //Matrix a = new Double[,] { { 6F, 5F },
            //                           { 2F, 3F } };
            //Matrix b = new Double[,] { { 3F }, { 2F } };

            //result.Text = a.Solve(b).ToString();

            //Console.WriteLine("A = \n" + a);
            //Console.WriteLine("mod B = \n" + a.Solve(b));

            //Console.WriteLine("B = \n" + b);
            //Console.WriteLine("A x B = \n" + (a * b));
            //Console.WriteLine("mod A x B = \n" + (a.Multiply(b)));
            //Console.WriteLine("A.Rows[0] = \n" + a.Rows[0]);
            //Console.WriteLine("A.Rows[1] = \n" + a.Rows[1]);
            //Console.WriteLine("A.Column[1] = B.Column[0]");
            //a.Columns[1] = b.Columns[0];
            //Console.WriteLine("A = \n" + a);

            //Matrix c = new Double[,] { {  4F,  2F,  2F  },
            //                           {  4F,  6F,  -8F },
            //                           { -2F,  2F,  4F  } };

            //Console.WriteLine("C = \n" + c);
            //Console.WriteLine("C.Inverse = \n" + c.Inverse());
            //Console.WriteLine("C x C.Inverse = \n" + c * c.Inverse());
            //Console.WriteLine("C diag as maxtrix= \n" + c.DiagAsMatrix());
            //Console.WriteLine("C diag as Vector= \n" + c.DiagAsVector());

            //Double[] d = new Double[] { 6F, 5F , 2F, 3F , 4F, 2F};
            //Matrix f = new Matrix(d, 3, 2);
            //Console.WriteLine("F = \n" + f);
            //Matrix z = new Double[,] { { 3F }, { 2F } , { 7F } };
            //Console.WriteLine("C = \n" + c.ElementAbsSum().ToString() + " " + c.ElementSum().ToString() + " " + c.NumElements);
            //Console.WriteLine("C = \n" + c.MaximumValue + " " + c.ElementSum().ToString() + " " + c.ElementMaxAbs());

            //c.SetMatrix(1, c.RowCount, new int[] { 0, 1, }, c);
            //Console.WriteLine("insert = \n" + c);


            //Console.WriteLine("insert = \n" + c.Reshape(4, 4, false));
            //Console.WriteLine("insert = \n" + c);


            //Matrix gg = new Matrix(new double[][] { new double[] { 1, 2, 3 }, new double[] { 4, 5, 6 } });
            //Console.WriteLine("gg = \n" + gg);

            //Console.WriteLine("Resharped = \n" + c.Reshape(3, 2, false).MergeWith(gg) + "rank = \n" + c.Reshape(3, 2, false).MergeWith(gg).Rank());

            //Console.WriteLine("JoinVertical = \n" + (z = Matrix.JoinVertical(c.Reshape(3, 2, false),gg)) + "rank = \n" + z.Rank());
            //Console.WriteLine("JoinHorizontal = \n" + (z = Matrix.JoinHorizontal(c.Reshape(3, 2, false), gg)) + "rank = \n" + z.Rank());

            ////Simplfied
            //Console.WriteLine("MergeWith = Vertically \n" + (z = c.Reshape(2, 3, false).MergeWith(gg.Transpose(),true)) + "rank = \n" + z.Rank());
            //Console.WriteLine("MergeWith = Horizontally \n" + (z = c.Reshape(2, 3, false).MergeWith(gg.Transpose(), false)) + "rank = \n" + z.Rank());

            //Console.WriteLine("MergeWith = Horizontally \n" + (z = c.Reshape(2, 3, false).MergeWith(gg.Transpose(), false)) + "rank = \n" + z.Rank());

            //Console.WriteLine("Eigen Values = \n" + c.EigenValues() + "Eigen Vector = \n" + c.EigenVectors());

            //Matrix A = new Double[,] { {  1,  2,  3, 4  },
            //                           {  5,  6,  7, 8  },
            //                           {  9, 10,  1, 12 },
            //                           { 13,-14, 15, 16 }};

            //Matrix B = new Double[,] { {  -501.3742,  1.4432,  1.6317,  0.0025 },
            //                           {  1.4368,  1.4073,  0.9964,  1.8380 },
            //                           {  0.1729,  1.7129,  0.8692,  0.8079 },
            //                           {  0.1779,  1.4320,  1.0598,  110.6329 }};

            //Matrix V = new Double[,] { {2, 6, 6, 2},
            //                           {2, 7, 3, 6},
            //                           {1, 5, 0, 1},
            //                           {3, 7, 0, 7}};

            //result.Text += V.EigenValues().ToString() + " " + V.EigenVectors().ToString() + "\n\n";
            //A.SetRow(0, B[":", 3]);
            //result.Text += A + "\n\n";
            //result.Text += A.Multiply(A.GetPseudoInverse).Multiply(A).ToString() + "\n\n";
            //result.Text += "A.Inverse = \n" + A.Inverse().ToString() + "\n\n";

            //result.Text += "B.Tranpose = \n" + B.Transpose().ToString() + "\n\n";

            //Console.WriteLine("A.Inverse = \n" + A.Inverse());
            //Console.WriteLine("A.Inverse = \n" + A);

            //Console.WriteLine("A.Determinant = \n" + A.Determinant());

            //Console.WriteLine("A*Inv(A) = \n" + A*A.Inverse());
            //Console.WriteLine("A*Inv(A) = \n" + A.Multiply(A.Inverse()));
            //Console.WriteLine("A*Tranpose(B)*Inv(A) = \n" + A * B.Transpose() * A.Inverse());
            //Console.WriteLine("A*Tranpose(B)*Inv(A) = \n" + A.Multiply(B.Transpose()).Multiply(A.Inverse()));

            //Console.WriteLine("A*PINV(A)*A = \n" + A.Multiply(A.GetPseudoInverse).Multiply(A));


            //var rtk = MainScreen.rtk;
                        
            //rtk.observ( new string[] { "C:/Users/Reindroid/Desktop/DATA/Sat Geodesy/Base.18O" });
            //rtk.navidata( new string[] { "C:/Users/Reindroid/Desktop/DATA/Sat Geodesy/Base.18N" });

            //rtk.solution( new string[] { "C:/Users/Reindroid/Desktop/DATA/Sat Geodesy/Solution.pos" });
            
            //result.Text = " ";
            //rtk.start();
            
            //result.Text += "Solution: " + rtk.solution() + "\n\n";

            //result.Text += "Base Lat: " + rtk.BaseLatitude.ToString() + "\n\n";
            //result.Text += "Base Lon: " + rtk.BaseLongitude.ToString() + "\n\n";
            //result.Text += "Base Elip H.: " + rtk.BaseHeight.ToString() + "\n\n";

            //result.Text += "\n\n";
            //result.Text += "Rover Lat: " + rtk.RoverLatitude.ToString() + "\n\n";
            //result.Text += "Rover Lon: " + rtk.RoverLongitude.ToString() + "\n\n";
            //result.Text += "Rover Elip H.: " + rtk.RoverHeight.ToString() + "\n\n";

            
            //rtk.Dispose();
            
        }

        private void Tester_Load(object sender, EventArgs e)
        {
            
            
        }
    }
}
