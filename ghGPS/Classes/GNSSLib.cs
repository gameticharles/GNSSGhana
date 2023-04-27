using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using System.Collections;
using static ghGPS.Forms.CreateTraversePath;

//using Sichem; //https://github.com/rds1983/Sichem

namespace ghGPS.Classes
{
    public static class GNSSLib
    {
        

        #region ASINH
        public static double asinh(double x)
        {
            return Math.Log(x + Math.Sqrt((x * x) + 1.0));
        }
        #endregion

        #region ACOSH
        public static double acosh(double x)
        {
            return Math.Log(x + Math.Sqrt((x * x) - 1.0));
        }
        #endregion

        #region ATANH
        public static double atanh(double x)
        {
            return 0.5 * Math.Log((1 + x) / (1 - x));
        }

        #endregion

        #region ACSCH
        public static object acsch(double x)
        {
            return asinh(1 / x);
        }
        #endregion

        #region ASECH
        public static object asech(double x)
        {
            return acosh(1 / x);
        }
        #endregion

        #region ACOTH
        public static double acoth(double x)
        {
            return atanh(1 / x);
        }

        #endregion

        #region Deg
        public static bool validateTBXDeg(int tbx)
        {
            if (tbx > 90 || tbx < -90)
            {
                MessageBox.Show("Please Degree value can't exceed 90", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Semi-Minor Axis
        public static double bb(double a, double invf)
        {
            var Semi_minor_axix = a - (a / invf);

            return Semi_minor_axix;
        }
        #endregion               

        #region EXPORT FROM DATAGRID VIEW TO FILE
        public static object ImportFromTable(DataGridView dgv)
        {
            var saveFileDialog = new SaveFileDialog();
            string FileName = null;

            saveFileDialog.Filter = "XLS files (*.xls, *.xlt)|*.xls;*.xlt|XLSX files (*.xlsx, *.xlsm, *.xltx, *.xltm)|*.xlsx;*.xlsm;*.xltx;*.xltm|ODS files (*.ods, *.ots)|*.ods;*.ots|CSV files (*.csv, *.tsv)|*.csv;*.tsv|HTML files (*.html, *.htm)|*.html;*.htm|Text (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog.FilterIndex = 3;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileName = saveFileDialog.FileName;

            }


            //INSTANT C# NOTE: Inserted the following 'return' since all code paths must return a value in C#:
            return null;
        }
        #endregion

        #region Convert from degrees to radians
        public static double DegreesToRadians(double degrees)
        {
            return (degrees * (Math.PI / 180));
        }

        #endregion

        #region Convert from radians to degrees
        public static double RadiansToDegrees(double radians)
        {
            return (float)((radians * 180) / Math.PI);
        }
        #endregion

        #region Convert latLonElipsoidH to XYZ
        public static double[] LatLonH2XYZ(double lat, double lon, double Height, double a, double invF)
        {
            //Second eccentricity

            var e2 = (2 * (1 / invF)) - Math.Pow((1 / invF), 2);

            var v = a / Math.Sqrt((1 - (e2 * Math.Pow(Math.Sin(DegreesToRadians((float)lat)), 2))));

            var X = (v + Height) * Math.Cos(DegreesToRadians((float)lat)) * Math.Cos(DegreesToRadians((float)lon));

            var Y = (v + Height) * Math.Cos(DegreesToRadians((float)lat)) * Math.Sin(DegreesToRadians((float)lon));

            var Z = (v * (1 - e2) + Height) * Math.Sin(DegreesToRadians((float)lat));

            double[] XYZ = new double[3];

            XYZ[0] = X;
            XYZ[1] = Y;
            XYZ[2] = Z;

            return XYZ;
        }

        #endregion

        #region Convert XYZ to LatLonH
        public static double[] XYZ2LatLonH(double X, double Y, double Z, double a, double invF)
        {
            //Second eccentricity
            var e2 = (2 * (1 / invF)) - Math.Pow((1 / invF), 2);

            var b = a - (a / invF);

            double lon = 0;
            //tailor the output to fit the equatorial quadrant as determined by the signs of X and Y

            lon = RadiansToDegrees((float)Math.Atan(Y / X));

            var RootXYSqr = Math.Sqrt((System.Math.Pow(X, 2)) + (System.Math.Pow(Y, 2)));


            var lat1 = Math.Atan(Z / (RootXYSqr * (1 - e2)));

            double V = a / Math.Sqrt(1 - (e2 * Math.Pow(Math.Sin(DegreesToRadians((float)lat1)), 2)));

            var lat2 = Math.Atan((Z + (e2 * V * (Math.Sin(lat1)))) / RootXYSqr);

            while (Math.Abs(lat1 - lat2) > 0.000000001)
            {
                lat1 = lat2;
                V = a / (Math.Sqrt(1 - (e2 * (System.Math.Pow((Math.Sin(lat1)), 2)))));
                lat2 = Math.Atan((Z + (e2 * V * (Math.Sin(lat1)))) / RootXYSqr);
            }

            //Latitude
            var lat = RadiansToDegrees((float)lat2);

            //Redefining V
            V = a / (Math.Sqrt(1 - (e2 * (System.Math.Pow((Math.Sin(DegreesToRadians(lat))), 2)))));

            var Height = (RootXYSqr / Math.Cos(DegreesToRadians(lat))) - V;

            double[] latLonH = new double[3];
            latLonH[0] = lat;
            latLonH[1] = lon;
            latLonH[2] = Height;

            return latLonH;
        }
        #endregion

        #region Reverse Multiply Regression Equation
        public static double[] TransParameters = new double[20];
        public static object ReverseMultiplyRegression(double lat, double lon, double elipsoid_H, double lat_origin, double lon_origin)
        {
            ////The 1,2,3 determines the degree of V and U

            //var A00 = TransParameters(0);
            //var B00 = TransParameters(10);
            //var A10 = TransParameters(1);
            //var B10 = TransParameters(11);
            //var A01 = TransParameters(2);
            //var B01 = TransParameters(12);
            //var A20 = TransParameters(3);
            //var B20 = TransParameters(13);
            //var A11 = TransParameters(4);
            //var B11 = TransParameters(14);
            //var A02 = TransParameters(5);
            //var B02 = TransParameters(15);
            //var A30 = TransParameters(6);
            //var B30 = TransParameters(16);
            //var A21 = TransParameters(7);
            //var B21 = TransParameters(17);
            //var A12 = TransParameters(8);
            //var B12 = TransParameters(18);
            //var A03 = TransParameters(9);
            //var B03 = TransParameters(19);

            //var k = 0.05235988;

            //var U = k * (lat - lat_origin);
            //var V = k * (lon - lon_origin);

            //var dlat = A00 + (A10 * U) + (A01 * V) + (System.Math.Pow(A20 * U, 2)) + (A11 * U * V) + (System.Math.Pow(A02 * V, 2)) + (System.Math.Pow(A30 * U, 3)) + (System.Math.Pow(A21 * U, 2 * V)) + (System.Math.Pow(A12 * U * V, 2)) + (System.Math.Pow(A03 * V, 3));
            //var dlon = B00 + (B10 * U) + (B01 * V) + (System.Math.Pow(B20 * U, 2)) + (B11 * U * V) + (System.Math.Pow(B02 * V, 2)) + (System.Math.Pow(B30 * U, 3)) + (System.Math.Pow(B21 * U, 2 * V)) + (System.Math.Pow(B12 * U * V, 2)) + (System.Math.Pow(B03 * V, 3));

            //double[] MRELatLonH = new double[3];
            //MRELatLonH[0] = dlat + lat;
            //MRELatLonH[1] = dlon + lon;
            //MRELatLonH[2] = elipsoid_H;

            return null;// MRELatLonH;

        }
        #endregion

        #region Forward Multiply Regression Equation
        public static object ForwardMultiplyRegression(double lat, double lon, double elipsoid_H, double lat_origin, double lon_origin)
        {
            ////The 1,2,3 determines the degree of V and U
            //var A00 = TransParameters(0);
            //var B00 = TransParameters(10);
            //var A10 = TransParameters(1);
            //var B10 = TransParameters(11);
            //var A01 = TransParameters(2);
            //var B01 = TransParameters(12);
            //var A20 = TransParameters(3);
            //var B20 = TransParameters(13);
            //var A11 = TransParameters(4);
            //var B11 = TransParameters(14);
            //var A02 = TransParameters(5);
            //var B02 = TransParameters(15);
            //var A30 = TransParameters(6);
            //var B30 = TransParameters(16);
            //var A21 = TransParameters(7);
            //var B21 = TransParameters(17);
            //var A12 = TransParameters(8);
            //var B12 = TransParameters(18);
            //var A03 = TransParameters(9);
            //var B03 = TransParameters(19);

            //var k = 0.05235988;

            //var U = k * (lat - lat_origin);
            //var V = k * (lon - lon_origin);

            //var dlat = A00 + (A10 * U) + (A01 * V) + (System.Math.Pow(A20 * U, 2)) + (A11 * U * V) + (System.Math.Pow(A02 * V, 2)) + (System.Math.Pow(A30 * U, 3)) + (System.Math.Pow(A21 * U, 2 * V)) + (System.Math.Pow(A12 * U * V, 2)) + (System.Math.Pow(A03 * V, 3));
            //var dlon = B00 + (B10 * U) + (B01 * V) + (System.Math.Pow(B20 * U, 2)) + (B11 * U * V) + (System.Math.Pow(B02 * V, 2)) + (System.Math.Pow(B30 * U, 3)) + (System.Math.Pow(B21 * U, 2 * V)) + (System.Math.Pow(B12 * U * V, 2)) + (System.Math.Pow(B03 * V, 3));

            //double[] MRELatLonH = new double[3];
            //MRELatLonH[0] = -dlat + lat;
            //MRELatLonH[1] = -dlon + lon;
            //MRELatLonH[2] = elipsoid_H;

            return null; //MRELatLonH;

        }
        #endregion

        #region Forward Transformation
        public static double[] ForwardTransformation(double Xs, double Ys, double Zs)
        {

            double Xt = 0;
            double Yt = 0;
            double Zt = 0;
            double X = 0;
            double Y = 0;
            double Z = 0;
            double Xm = 0;
            double Ym = 0;
            double Zm = 0;
            double dS = 0;
            double Rx = 0;
            double Ry = 0;
            double Rz = 0;
            double dX = 0;
            double dY = 0;
            double dZ = 0;

            dX = 158.635;
            dY = -32.174;
            dZ = -326.783;
            Rx = -0.0368 / 206264.81; //Convert arcsecond to radian
            Ry = 0.00798 / 206264.81;
            Rz = 0.0119 / 206264.81;
            dS = Math.Pow(7.6 * 10, -6); //convert ppm
            Xm = 0;
            Ym = 0;
            Zm = 0;

            //APPLYING SCALE , ROTATION AND SHIFT
            X = Xs - Xm;
            Y = Ys - Ym;
            Z = Zs - Zm;

            Xt = Xs + (dS * X) + (Rz * Y) - (Ry * Z) + dX;
            Yt = Ys - (Rz * X) + (dS * Y) + (Rx * Z) + dY;
            Zt = Zs + (Ry * X) - (Rx * Y) + (dS * Z) + dZ;

            double[] NewXYZ = new double[3];
            NewXYZ[0] = Xt;
            NewXYZ[1] = Yt;
            NewXYZ[2] = Zt;

            return NewXYZ;
        }

        #endregion

        #region Reverse Transformation
        public static double[] ReverseTransformation(double Xs, double Ys, double Zs)
        {

            double Xt = 0;
            double Yt = 0;
            double Zt = 0;
            double X = 0;
            double Y = 0;
            double Z = 0;
            double Xm = 0;
            double Ym = 0;
            double Zm = 0;
            double dS = 0;
            double Rx = 0;
            double Ry = 0;
            double Rz = 0;
            double dX = 0;
            double dY = 0;
            double dZ = 0;

            dX = -1 * 158.635;
            dY = -1 * -32.174;
            dZ = -1 * -326.783;
            Rx = -1 * (-0.0368 / 206264.81); //Convert arcsecond to radian
            Ry = -1 * (0.00798 / 206264.81);
            Rz = -1 * (0.0119 / 206264.81);
            dS = -1 * Math.Pow(7.6 * 10, -6); //convert ppm
            Xm = -1 * 0;
            Ym = -1 * 0;
            Zm = -1 * 0;

            //APPLYING SCALE , ROTATION AND SHIFT
            X = Xs - Xm;
            Y = Ys - Ym;
            Z = Zs - Zm;

            Xt = Xs + (dS * X) + (Rz * Y) - (Ry * Z) + dX;
            Yt = Ys - (Rz * X) + (dS * Y) + (Rx * Z) + dY;
            Zt = Zs + (Ry * X) - (Rx * Y) + (dS * Z) + dZ;

            double[] NewXYZ = new double[3];
            NewXYZ[0] = Xt;
            NewXYZ[1] = Yt;
            NewXYZ[2] = Zt;
            
            return NewXYZ;
        }

        #endregion

        #region LatLonH TO UTM/TM
        public static double[] LatLongH2UTM(double a, double InvF, double lat, double lon, double altitude, double ko, double FalseNorthing, double FalseEasting, double latOfOrigin, double zoneCM)
        {

            int N_S = 0;
            N_S = (lat >= 0) ? 1 : -1;

            var DlatAbs = Math.Abs(lat);
            var latR = DlatAbs;

            var Dlon = Math.Abs(lon - zoneCM);
            var DlonAbs = Math.Abs(lon);
            var lonR = Dlon;


            var b = bb(a, InvF);

            double f = (a - b) / a;
            double n = (a - b) / (a + b);
            double e = Math.Sqrt(1 - Math.Pow((b / a), 2));

            double AA = (a / (1 + n)) * (1 + ((1 / 4.0) * Math.Pow(n, 2)) + ((1 / 64.0) * Math.Pow(n, 4)) + ((1 / 256.0) * Math.Pow(n, 6)) + ((25 / 16384.0) * Math.Pow(n, 8)) + ((49 / 65536.0) * Math.Pow(n, 10)));

            double tau = Math.Sin(latR) / Math.Cos(latR);

            double ConfLat = Math.Atan(Math.Sinh(Convert.ToDouble(asinh(Math.Tan(latR)) - (e * atanh(e * Math.Sin(latR))))));

            double Sigma = Math.Sinh(e * atanh(e * Math.Tan(latR) / Math.Sqrt(1 + Math.Pow(Math.Tan(latR), 2))));

            double ConfLat1 = Math.Atan((Math.Tan(latR) * Math.Sqrt(1 + Sigma * Sigma)) - Sigma * Math.Sqrt(1 + Math.Pow(Math.Tan(latR), 2)));

            double tau_prime = Math.Tan(ConfLat1);

            var xi_prime = Math.Atan(tau_prime / Math.Cos(lonR));

            var eta_Prime = asinh(Math.Sin(lonR) / Math.Sqrt((tau_prime * tau_prime) + (Math.Pow(Math.Cos(lonR), 2))));

            var alpha1 = ((1 / 2.0) * n) - ((2 / 3.0) * Math.Pow(n, 2)) + ((5 / 16.0) * Math.Pow(n, 3)) + ((41 / 180.0) * Math.Pow(n, 4)) - ((127 / 288.0) * Math.Pow(n, 5)) + ((7891 / 37800.0) * Math.Pow(n, 6)) + ((72161 / 387072.0) * Math.Pow(n, 7)) - ((18975107 / 50803200.0) * Math.Pow(n, 8)) + ((60193001 / 290304000.0) * Math.Pow(n, 9)) + ((134592031 / 1026432000.0) * Math.Pow(n, 10));
            var alpha2 = ((13 / 48.0) * Math.Pow(n, 2) - (3 / 5.0) * Math.Pow(n, 3) + (557 / 1440.0) * Math.Pow(n, 4) + (281 / 630.0) * Math.Pow(n, 5) - (1983433 / 1935360.0) * Math.Pow(n, 6) + (13769 / 28800.0) * Math.Pow(n, 7) + (148003883 / 174182400.0) * Math.Pow(n, 8) - (705286231 / 465696000.0) * Math.Pow(n, 9) + (1703267974087 / 3218890752000.0) * Math.Pow(n, 10));
            var alpha3 = (61 / 240.0) * Math.Pow(n, 3) - (103 / 140.0) * Math.Pow(n, 4) + (15061 / 26880.0) * Math.Pow(n, 5) + (167603 / 181440.0) * Math.Pow(n, 6) - (67102379 / 29030400.0) * Math.Pow(n, 7) + (79682431 / 79833600.0) * Math.Pow(n, 8) + (6304945039 / 2128896000.0) * Math.Pow(n, 9) - (6601904925257 / 1307674368000.0) * Math.Pow(n, 10);
            var alpha4 = (49561 / 161280.0) * Math.Pow(n, 4) - (179 / 168.0) * Math.Pow(n, 5) + (6601661 / 7257600.0) * Math.Pow(n, 6) + (97445 / 49896.0) * Math.Pow(n, 7) - (40176129013 / 7664025600.0) * Math.Pow(n, 8) + (138471097 / 66528000.0) * Math.Pow(n, 9) + (48087451385201 / 5230697472000.0) * Math.Pow(n, 10);
            var alpha5 = (34729 / 80640.0) * Math.Pow(n, 5) - (3418889 / 1995840.0) * Math.Pow(n, 6) + (14644087 / 9123840.0) * Math.Pow(n, 7) + (2605413599 / 622702080.0) * Math.Pow(n, 8) - (31015475399 / 2583060480.0) * Math.Pow(n, 9) + (5820486440369 / 1307674368000.0) * Math.Pow(n, 10);
            var alpha6 = (212378941 / 319334400.0) * Math.Pow(n, 6) - (30705481 / 10378368.0) * Math.Pow(n, 7) + (175214326799 / 58118860800.0) * Math.Pow(n, 8) + (870492877 / 96096000.0) * Math.Pow(n, 9) - (1328004581729000 / 47823519744000.0) * Math.Pow(n, 10);
            var alpha7 = (1522256789 / 1383782400.0) * Math.Pow(n, 7) - (16759934899 / 3113510400.0) * Math.Pow(n, 8) + (1315149374443 / 221405184000.0) * Math.Pow(n, 9) + (71809987837451 / 3629463552000.0) * Math.Pow(n, 10);
            var alpha8 = (1424729850961 / 743921418240.0) * Math.Pow(n, 8) - (256783708069 / 25204608000.0) * Math.Pow(n, 9) + (2468749292989890 / 203249958912000.0) * Math.Pow(n, 10);
            var alpha9 = (21091646195357 / 6080126976000.0) * Math.Pow(n, 9) - (67196182138355800 / 3379030566912000.0) * Math.Pow(n, 10);
            var alpha10 = (77911515623232800 / 12014330904576000.0) * Math.Pow(n, 10);

            //north
            var xi = xi_prime + (alpha1 * Math.Sin(2 * xi_prime) * Math.Cosh(2 * eta_Prime) + alpha2 * Math.Sin(4 * xi_prime) * Math.Cosh(4 * eta_Prime) + alpha3 * Math.Sin(6 * xi_prime) * Math.Cosh(6 * eta_Prime) + alpha4 * Math.Sin(8 * xi_prime) * Math.Cosh(8 * eta_Prime) + alpha5 * Math.Sin(10 * xi_prime) * Math.Cosh(10 * eta_Prime) + alpha6 * Math.Sin(12 * xi_prime) * Math.Cosh(12 * eta_Prime) + alpha7 * Math.Sin(14 * xi_prime) * Math.Cosh(14 * eta_Prime) + alpha8 * Math.Sin(16 * xi_prime) * Math.Cosh(16 * eta_Prime) + alpha9 * Math.Sin(18 * xi_prime) * Math.Cosh(18 * eta_Prime) + alpha10 * Math.Sin(20 * xi_prime) * Math.Cosh(20 * eta_Prime));

            //east
            var eta = eta_Prime + (alpha1 * Math.Cos(2 * xi_prime) * Math.Sinh(2 * eta_Prime) + alpha2 * Math.Cos(4 * xi_prime) * Math.Sinh(4 * eta_Prime) + alpha3 * Math.Cos(6 * xi_prime) * Math.Sinh(6 * eta_Prime) + alpha4 * Math.Cos(8 * xi_prime) * Math.Sinh(8 * eta_Prime) + alpha5 * Math.Cos(10 * xi_prime) * Math.Sinh(10 * eta_Prime) + alpha6 * Math.Cos(12 * xi_prime) * Math.Sinh(12 * eta_Prime) + alpha7 * Math.Cos(14 * xi_prime) * Math.Sinh(14 * eta_Prime) + alpha8 * Math.Cos(16 * xi_prime) * Math.Sinh(16 * eta_Prime) + alpha9 * Math.Cos(18 * xi_prime) * Math.Sinh(18 * eta_Prime) + alpha10 * Math.Cos(20 * xi_prime) * Math.Sinh(20 * eta_Prime));

            var northing = ko * AA * xi;
            //Rel to CM
            var easting = ko * AA * eta;

            //COORDINATES
            double NORTHINGS = 0;
            double EASTINGS = 0;

            if (N_S == 1)
            {
                NORTHINGS = northing - FalseNorthing;
            }
            else
            {
                NORTHINGS = 10000000 + northing;

                //Check if its the northern or southern hermispher
                if (NORTHINGS >= 10000000)
                {
                    NORTHINGS = NORTHINGS - 10000000;
                }
            }

            //East of CM
            int EastCM = 0;

            EastCM = (lon > zoneCM) ? 1 : -1;

            //East of CM
            EASTINGS = EastCM * easting + FalseEasting;


            //Compute Height
            var e2 = System.Math.Pow(e, 2);
            var V = a / (Math.Sqrt(1 - (e2 * (System.Math.Pow((Math.Sin(latR)), 2)))));
            var Height = ((V * (1 - e2)) + altitude) * (Math.Sin(lat));

            double[] UTM = new double[3];
            UTM[0] = NORTHINGS;
            UTM[1] = EASTINGS;
            UTM[2] = altitude;

            return UTM;

        }
        #endregion

        #region UTM To LatLonH
        public static object UTM2LatLongH(double a, double InvF, double Northing, double Easting, double Altitude, double ko, double FalseNorthing, double FalseEasting, double LatOfOrigin, double zoneCM)
        {
            //Compute Semi-minor axis
            var b = bb(a, InvF);

            double f = (a - b) / a;
            var n = (a - b) / (a + b);
            var e = Math.Sqrt(1 - Math.Pow((b / a), 2));
            double ee = Math.Pow(e, 2);
            var n2 = n * n;
            var n3 = n * n2;
            var n4 = n * n3;
            var n5 = n * n4;
            var n6 = n * n5;
            var n7 = n * n6;
            var n8 = n * n7;
            var n9 = n * n8;
            var n10 = n * n9;

            double AA = (a / (1 + n)) * (1 + ((1 / 4.0) * n2) + ((1 / 64.0) * n4) + ((1 / 256.0) * n6) + ((25 / 16384.0) * n8) + ((49 / 65536.0) * n10));

            //xi(north)
            var xi = Northing / (ko * AA);

            //eta (east)
            var eta = (Easting - FalseEasting) / (ko * AA);

            var beta1 = (1 / 2.0) * n - (2 / 3.0) * n2 + (37 / 96.0) * n3 - (1 / 360.0) * n4 - (81 / 512.0) * n5 + (96199 / 604800.0) * n6 - (5406467 / 38707200.0) * n7 + (7944359 / 67737600.0) * n8 - (7378753979 / 97542144000.0) * n9 + (25123531261 / 804722688000.0) * n10;
            var beta2 = (1 / 48.0) * n2 + (1 / 15.0) * n3 - (437 / 1440.0) * n4 + (46 / 105.0) * n5 - (1118711 / 3870720.0) * n6 + (51841 / 1209600.0) * n7 + (24749483 / 348364800.0) * n8 - (115295683 / 1397088000.0) * n9 + (5487737251099 / 51502252032000.0) * n10;
            var beta3 = (17 / 480.0) * n3 - (37 / 840.0) * n4 - (209 / 4480.0) * n5 + (5569 / 90720.0) * n6 + (9261899 / 58060800.0) * n7 - (6457463 / 17740800.0) * n8 + (2473691167 / 9289728000.0) * n9 - (852549456029 / 20922789888000.0) * n10;
            var beta4 = (4397 / 161280.0) * n4 - (11 / 504.0) * n5 - (830251 / 7257600.0) * n6 + (466511 / 2494800.0) * n7 + (324154477 / 7664025600.0) * n8 - (937932223 / 3891888000.0) * n9 - (89112264211 / 5230697472000.0) * n10;
            var beta5 = (4583 / 161280.0) * n5 - (108847 / 3991680.0) * n6 - (8005831 / 63866880.0) * n7 + (22894433 / 124540416.0) * n8 + (112731569449 / 557941063680.0) * n9 - (5391039814733 / 10461394944000.0) * n10;
            var beta6 = (20648693 / 638668800.0) * n6 - (16363163 / 518918400.0) * n7 - (2204645983 / 12915302400.0) * n8 + (4543317553 / 18162144000.0) * n9 + (54894890298749 / 167382319104000.0) * n10;
            var beta7 = (219941297 / 5535129600.0) * n7 - (497323811 / 12454041600.0) * n8 - (79431132943 / 332107776000.0) * n9 + (4346429528407 / 12703122432000.0) * n10;
            var beta8 = (191773887257 / 3719607091200.0) * n8 - (17822319343 / 336825216000.0) * n9 - (497155444501631 / 1422749712384000.0) * n10;
            var beta9 = (11025641854267 / 158083301376000.0) * n9 - (492293158444691 / 6758061133824000.0) * n10;
            var beta10 = (7028504530429621 / 72085985427456000.0) * n10;

            //xi prime
            var xi_prime = xi - ((beta1 * Math.Sin(2 * xi) * Math.Cosh(2 * eta) + beta2 * Math.Sin(4 * xi) * Math.Cosh(4 * eta) + beta3 * Math.Sin(6 * xi) * Math.Cosh(6 * eta) + beta4 * Math.Sin(8 * xi) * Math.Cosh(8 * eta) + beta5 * Math.Sin(10 * xi) * Math.Cosh(10 * eta) + beta6 * Math.Sin(12 * xi) * Math.Cosh(12 * eta) + beta7 * Math.Sin(14 * xi) * Math.Cosh(14 * eta) + beta8 * Math.Sin(16 * xi) * Math.Cosh(16 * eta) + beta9 * Math.Sin(18 * xi) * Math.Cosh(18 * eta) + beta10 * Math.Sin(20 * xi) * Math.Cosh(20 * eta)));
            //eta prime
            var eta_prime = eta - ((beta1 * Math.Cos(2 * xi) * Math.Sinh(2 * eta) + beta2 * Math.Cos(4 * xi) * Math.Sinh(4 * eta) + beta3 * Math.Cos(6 * xi) * Math.Sinh(6 * eta) + beta4 * Math.Cos(8 * xi) * Math.Sinh(8 * eta) + beta5 * Math.Cos(10 * xi) * Math.Sinh(10 * eta) + beta6 * Math.Cos(12 * xi) * Math.Sinh(12 * eta) + beta7 * Math.Cos(14 * xi) * Math.Sinh(14 * eta) + beta8 * Math.Cos(16 * xi) * Math.Sinh(16 * eta) + beta9 * Math.Cos(18 * xi) * Math.Sinh(18 * eta) + beta10 * Math.Cos(20 * xi) * Math.Sinh(20 * eta)));
            //tau prime
            var tau_prime = Math.Sin(xi_prime) / Math.Sqrt(Math.Pow(Math.Sinh(eta_prime), 2) + Math.Pow(Math.Cos(xi_prime), 2));
            //Longitude in radians
            var lonR = Math.Atan(Math.Sinh(eta_prime) / Math.Cos(xi_prime));

            //Longitude in degrees
            var lonD = RadiansToDegrees((float)lonR);

            double[] sigma = new double[6];
            double[] f_tau = new double[6];

            double[] df_tau_dtau = new double[6];
            double[] Tau = new double[6];

            Tau[0] = tau_prime;

            //Determine the to the fifth approximation
            for (int i = 0; i <= 5; i++)
            {
                sigma[i] = Math.Sinh(e * atanh(e * Tau[i] / Math.Sqrt(System.Math.Pow(1 + Tau[i], 2))));
                f_tau[i] = Tau[i] * Math.Sqrt(System.Math.Pow(1 + sigma[i], 2)) - sigma[i] * Math.Sqrt(System.Math.Pow(1 + Tau[i], 2)) - tau_prime;
                //df(tau)/dtau
                df_tau_dtau[i] = (Math.Sqrt((System.Math.Pow(1 + sigma[i], 2)) * (System.Math.Pow(1 + Tau[i], 2))) - sigma[i] * Tau[i]) * (System.Math.Pow(1 - e, 2)) * Math.Sqrt(System.Math.Pow(1 + Tau[i], 2)) / (1 + (System.Math.Pow((System.Math.Pow(1 - e, 2)) * Tau[i], 2)));

                Tau[i] = Tau[i] - (f_tau[i] / df_tau_dtau[i]);
            }

            //Latitude in degrees
            var latD = RadiansToDegrees((float)Math.Atan(Tau[5]));

            var Latitude = latD + LatOfOrigin;
            double Longitude = 0;

            Longitude = (zoneCM == 0) ? lonD - zoneCM : lonD + zoneCM;

            double[] Lat_Lon = new double[3];
            Lat_Lon[0] = Latitude;
            Lat_Lon[1] = Longitude;
            Lat_Lon[2] = Altitude;
            
            return Lat_Lon;
        }
        #endregion

        #region (U)TM To LatLonH
        public static double[] UTM2LatLong(double a, double InvF, double Northing, double Easting, double Altitude, double ko, double FalseNorthing, double FalseEasting, double LatOfOrigin, double zoneCM)
        {
            //Compute Semi-minor axis
            var b = bb(a, InvF);

            //φ λ α ξ ηʹ β
            //3rd Flattening
            var n = (a - b) / (a + b);

            //Powers of n
            var n2 = n * n;
            var n3 = n * n2;
            var n4 = n * n3;
            var n5 = n * n4;
            var n6 = n * n5;
            var n7 = n * n6;
            var n8 = n * n7;

            //Eccentricity square
            var e2 = (4 * n) / Math.Pow((1 + n), 2);
            var e = Math.Sqrt(e2);

            //(Rectifying) sphere of radius 
            double AA = (a / (1 + n)) * (1 + ((1 / 4.0) * n2) + ((1 / 64.0) * n4) + ((1 / 256.0) * n6) + ((25 / 16384.0) * n8));

            //Co-efficients
            double[] β = new double[9];

            β[0] = 0;
            β[1] = -((1 / 2.0) * n) + ((2 / 3.0) * n2) - ((37 / 96.0) * n3) + ((1 / 360.0) * n4) + ((81 / 512.0) * n5) - ((96199 / 604800.0) * n6) + ((5406467 / 38707200.0) * n7) - ((7944359 / 67737600.0) * n8);
            β[2] = -((1 / 48.0) * n2) - ((1 / 15.0) * n3) + ((437 / 1440.0) * n4) - ((46 / 105.0) * n5) + ((1118711 / 3870720.0) * n6) - ((51841 / 1209600.0) * n7) - ((24749483 / 348364800.0) * n8);
            β[3] = -((17 / 480.0) * n3) + ((37 / 840.0) * n4) + ((209 / 4480.0) * n5) - ((5569 / 90720.0) * n6) - ((9261899 / 58060800.0) * n7) + ((6457463 / 17740800.0) * n8);
            β[4] = -((4397 / 161280.0) * n4) + ((11 / 504.0) * n5) + ((830251 / 7257600.0) * n6) - ((466511 / 2494800.0) * n7) - ((324154477 / 7664025600.0) * n8);
            β[5] = -((4583 / 161280.0) * n5) + ((108847 / 3991680.0) * n6) + ((8005831 / 63866880.0) * n7) - ((22894433 / 124540416.0) * n8);
            β[6] = -((20648693 / 638668800.0) * n6) + ((16363163 / 518918400.0) * n7) + ((2204645983 / 12915302400.0) * n8);
            β[7] = -((219941297 / 5535129600.0) * n7) + ((497323811 / 12454041600.0) * n8);
            β[8] = -((191773887257 / 3719607091200.0) * n8);

            //Computing ratios u/a and v/a
            //Let u/a =ηʹ and v/a = ξʹ
            var ξ = (Easting - FalseEasting) / (AA * ko);
            var η = Northing / (AA * ko);

            var ξʹ = ξ;
            var ηʹ = η;

            for (var r = 1; r <= 8; r++)
            {
                ηʹ += β[r] * Math.Sin(2 * r * η) * Math.Cosh(2 * r * ξ);
                ξʹ += β[r] * Math.Cos(2 * r * η) * Math.Sinh(2 * r * ξ);
            }

            //----------Computing conformal latitude φ' and longitude difference ω 
            var sinηʹ = Math.Sin(ηʹ);
            var cosηʹ = Math.Cos(ηʹ);
            var sinhξʹ = Math.Sinh(ξʹ);

            var φʹ = sinηʹ / Math.Sqrt((sinhξʹ * sinhξʹ) + (cosηʹ * cosηʹ));

            var ω = RadiansToDegrees((float)Math.Atan(sinhξʹ / cosηʹ));

            double[] σ = new double[6];
            double[] f = new double[6];

            double[] fʹ = new double[6];
            double[] Tau = new double[6];

            Tau[0] = φʹ;

            //Determine the to the fifth approximation
            for (int i = 0; i <= 5; i++)
            {
                σ[i] = Math.Sinh(e * atanh(e * Tau[i] / Math.Sqrt(System.Math.Pow(1 + Tau[i], 2))));
                f[i] = Tau[i] * Math.Sqrt(System.Math.Pow(1 + σ[i], 2)) - σ[i] * Math.Sqrt(System.Math.Pow(1 + Tau[i], 2)) - φʹ;
                //df(tau)/dtau
                fʹ[i] = (Math.Sqrt((System.Math.Pow(1 + σ[i], 2)) * (System.Math.Pow(1 + Tau[i], 2))) - (σ[i] * Tau[i])) * (System.Math.Pow(1 - e, 2)) * Math.Sqrt(System.Math.Pow(1 + Tau[i], 2)) / (1 + (System.Math.Pow((System.Math.Pow(1 - e, 2)) * Tau[i], 2)));

                Tau[i] = Tau[i] - (f[i] / fʹ[i]);
            }

            //Latitude in degrees
            var φ_ = RadiansToDegrees((float)Math.Atan(Tau[5]));

            var φ = φ_ + LatOfOrigin;
            double λ = 0;

            λ = (zoneCM == 0) ? ω - zoneCM : ω + zoneCM;

            double[] Lat_Lon = new double[3];
            Lat_Lon[0] = φ;
            Lat_Lon[1] = λ;
            Lat_Lon[2] = Altitude;

            return Lat_Lon;
        }
        #endregion

        #region (U)TM TO LAT_LONG_H
        public static double[] UTM_LatLonH(double a, double InvF, double Northing, double Easting, double Altitude, double ko, double FalseNorthing, double FalseEasting, double LatOfOrigin, double zoneCM)
        {

            double R1 = 0;
            double D = 0;
            double RN1 = 0;
            double C1 = 0;
            double T1 = 0;
            double Y_pe = 0;
            double X_pe = 0;
            double Ht_pe = 0;
            double ee2 = 0;
            double LONGo = 0;
            double Of1 = 0;
            double Of2 = 0;
            double Of3 = 0;
            double Of4 = 0;
            double Of5 = 0;
            double Ofo = 0;
            double e1 = 0;
            double M1o = 0;
            double M2o = 0;
            double M3o = 0;
            double M4o = 0;
            double Mo = 0;
            double M = 0;
            double Miu = 0;
            double Latitude_pe = 0;
            double Lt1 = 0;
            double Lt2 = 0;
            double Lt3 = 0;
            double Lt4 = 0;
            double Lt5 = 0;
            double Lg1 = 0;
            double Lg2 = 0;
            double Lg3 = 0;
            double Lg4 = 0;
            double Longitude_pe = 0;
            double Lg5 = 0;

            //Obtaining the values of Latitude,Longitude and Height from the Text Entries in Geographic

            Y_pe = Northing;

            X_pe = Easting;

            Ht_pe = Altitude;

            Ht_pe *= 0.304799710181509;

            LONGo = DegreesToRadians((float)zoneCM);

            var ff = 1 / InvF;

            //Calculating Eccentricity squared from System Ellipsoid
            var e2 = System.Math.Pow(2 * ff - ff, 2);

            //Second Eccentricity
            ee2 = e2 / (1 - e2);

            //3rd Flattening
            var n = ff / (2 - ff);

            //Powers of n
            var n2 = n * n;
            var n3 = n * n2;
            var n4 = n * n3;
            var n5 = n * n4;
            var n6 = n * n5;
            var n7 = n * n6;
            var n8 = n * n7;


            //(Rectifying) sphere of radius 
            double AA = (a / (1 + n)) * (1 + ((1 / 4.0) * n2) + ((1 / 64.0) * n4) + ((1 / 256.0) * n6) + ((25 / 16384.0) * n8));

            //Terms for Mo
            M1o = (System.Math.Pow(1 - e2 / 4 - 3 / 64 * e2, System.Math.Pow(2 - 5 / 256 * e2, 3))) * DegreesToRadians((float)LatOfOrigin);

            M2o = (System.Math.Pow(3 / 8 * e2 + 3 / 32 * e2, System.Math.Pow(2 + 45 / 1024 * e2, 3))) * Math.Sin(2 * DegreesToRadians((float)LatOfOrigin));

            M3o = (System.Math.Pow(15 / 256 * e2, System.Math.Pow(2 + 45 / 1024 * e2, 3))) * Math.Sin(4 * DegreesToRadians((float)LatOfOrigin));

            M4o = (System.Math.Pow(35 / 3072 * e2, 3)) * Math.Sin(6 * DegreesToRadians((float)LatOfOrigin));

            //calculating the value of M
            Mo = a * (M1o - M2o + M3o - M4o);

            //VARIABLE NOTATIONS
            //______________________________________________________________________________

            M = Mo + Y_pe / (ko);

            Miu = M * 1 / a * 1 / (1 - e2 / 4 - (System.Math.Pow((3 / 64.0) * e2, 2 - (System.Math.Pow((5 / 256.0) * e2, 3)))));

            //where miu is in radians
            e1 = (1 - Math.Sqrt(1 - e2)) / (1 + Math.Sqrt(1 - e2));

            //Terms for calculating the foot print latitude
            Of1 = Miu;

            Of2 = (System.Math.Pow(3 / 2 * e1 - 27 / 32 * e1, 3)) * Math.Sin(2 * Miu);

            Of3 = (System.Math.Pow(21 / 16 * e1, System.Math.Pow(2 - 55 / 32 * e1, 4))) * Math.Sin(4 * Miu);

            Of4 = (System.Math.Pow(151 / 96 * e1, 3)) * Math.Sin(6 * Miu);

            Of5 = (System.Math.Pow(1097 / 512 * e1, 4)) * Math.Sin(8 * Miu);

            Ofo = Of1 + Of2 + Of3 + Of4 + Of5;

            //********************************************************************************

            T1 = Math.Tan(Ofo) * Math.Tan(Ofo);

            C1 = ee2 * Math.Cos(Ofo) * Math.Cos(Ofo);

            RN1 = (a) / (System.Math.Pow((1 - e2 * Math.Sin(Ofo) * Math.Sin(Ofo)), 0.5));

            R1 = (a * (1 - e2)) / (System.Math.Pow((1 - e2 * Math.Sin(Ofo) * Math.Sin(Ofo)), 1.5));

            D = (X_pe - FalseEasting) / (RN1 * ko);

            //TERMS FOR CALCULATING LATITUDE
            Lt1 = Ofo;

            Lt2 = (RN1 * Math.Tan(Ofo) / R1);

            Lt3 = (System.Math.Pow(D, 2)) / 2;

            Lt4 = (System.Math.Pow(5 + 3 * T1 + 10 * C1 - 4 * C1, 2 - 9 * ee2)) * ((System.Math.Pow(D, 4)) / 24);

            Lt5 = (System.Math.Pow(61 + 90 * T1 + 298 * C1 + 45 * T1, 2 - 252 * ee2 - 3 * (System.Math.Pow(C1, 2)))) * ((System.Math.Pow(D, 6)) / 720);

            Latitude_pe = Lt1 - Lt2 * (Lt3 - Lt4 + Lt5);

            Latitude_pe = RadiansToDegrees((float)Latitude_pe); //* (1 / Deg2Rad) 'Finale Value of Value of Latitude

            //Terms of Longitude

            Lg1 = LONGo;

            Lg2 = D;

            Lg3 = (1 + 2 * T1 + C1) * (System.Math.Pow(D, 3 / 6.0));

            Lg4 = (System.Math.Pow(5 - 2 * C1 + 28 * T1 - 3 * C1, System.Math.Pow(2 + 8 * ee2 + 24 * T1, 2))) * (System.Math.Pow(D, 5 / 120.0));

            Lg5 = Math.Cos(Ofo);

            Longitude_pe = Lg1 + (Lg2 - Lg3 + Lg4) / (Lg5);


            Longitude_pe = RadiansToDegrees((float)Longitude_pe);


            double[] Lat_Lon = new double[3];
            Lat_Lon[0] = Latitude_pe;
            Lat_Lon[1] = Longitude_pe;
            Lat_Lon[2] = Altitude;

            return Lat_Lon;

        }
        #endregion
        
        #region Ellipsoid to WGS 84
        public static double[] Elipsoid_2_WGS84(double lat, double lon, double Altitude, double a, double invf)
        {
            //----------------- Convert Lat, Lon, and Elip. Height To X,Y and Z -----------------
            var LatLongH2XYZ = LatLonH2XYZ(lat, lon, Altitude, a, invf);

            //----------------- Perform Transformation ---------------
            var X2Y2Z2 = ForwardTransformation(LatLongH2XYZ[0], LatLongH2XYZ[1], LatLongH2XYZ[2]);

            //Get the WGS 84 Ellipsoid
            var WGS84Elipsoid = retrieveElipsoid(0);

            //----------------- Convert WGG To Lat, Long and Ellipsoid Height  ---------------
            var LatLongH = XYZ2LatLonH(X2Y2Z2[0], X2Y2Z2[1], X2Y2Z2[2], WGS84Elipsoid[0], WGS84Elipsoid[1]);

            return LatLongH;

        }
        #endregion

        #region WGS 84 to Ellipsoid
        public static double[] WGS84_2_Elipsoid(double lat, double lon, double Altitude, double a, double invf)
        {
          
            //----------------- Convert Lat, Lon, and Elip. Height To X,Y and Z -----------------
            var LatLongH2XYZ = LatLonH2XYZ(lat, lon, Altitude, RE_WGS84, 298.257222101);
            
            //----------------- Perform Transformation ---------------
            var X2Y2Z2 = ReverseTransformation(LatLongH2XYZ[0], LatLongH2XYZ[1], LatLongH2XYZ[2]);
            
            //----------------- Convert WGG To Lat, Long and Ellipsoid Height  ---------------
            var LatLongH = XYZ2LatLonH(X2Y2Z2[0], X2Y2Z2[1], X2Y2Z2[2], a, invf);
           
            return LatLongH;

        }
        #endregion

        #region RETRIEVE ELIPSOID VALUES
        public static double[] retrieveElipsoid(int selectedElipsoidIndex)
        {
            ////Open connection
            //Connect();

            //OleDbDataAdapter da = null;
            //DataTable dt = new DataTable();

            //da = new OleDbDataAdapter("SELECT * FROM Elipsoid", connection);
            ////Using dataTable to fill data from database
            //da.Fill(dt);
            //DataRow DataRow = dt.Rows(selectedElipsoidIndex);

            //object[] Ellipsoid = new object[3];
            //Ellipsoid[0] = DataRow.Item(2); //Semi-Major axis
            //Ellipsoid[1] = DataRow.Item(3); //inverse Flattening
            //Ellipsoid[2] = DataRow.Item(1); //Name

            return null;// Ellipsoid;
        }
        #endregion


        public const double RE_WGS84 = 6378137.0;
        public const double INVF_WGS84 = 298.257222101;


        #region Check if directory exist
        /// <summary>
        /// Create a directory if not exists
        /// </summary>
        /// <param name="strLogPath"></param>
        /// <returns></returns>
        private static bool CheckDirectory(string strLogPath)
        {
            try
            {
                int nFindSlashPos = strLogPath.Trim().LastIndexOf("\\");
                string strDirectoryname = strLogPath.Trim().Substring(0, nFindSlashPos);

                if (Directory.Exists(strDirectoryname) == false)
                {
                    //LogInfo("Creating log directory :" + strDirectoryname);
                    Directory.CreateDirectory(strDirectoryname);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Check if directory exist


        public class Logger
        {
            private string strPathName = string.Empty;
            private StreamWriter sw = null;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="prefix"></param>
            public Logger(string prefix)
            {
                DateTime datet = DateTime.Now;

                // Format string
                if (string.IsNullOrEmpty(prefix))
                {
                    prefix += "_";
                }
                else
                {
                    prefix = "";
                }

                strPathName = "Log_" + prefix + datet.ToString("MM_dd_hhmmss") + ".log";
                if (File.Exists(strPathName) == true)
                {
                    FileStream fs = new FileStream(strPathName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs.Close();
                }
            }

            /// <summary>
            /// Create a directory if not exists
            /// </summary>
            /// <param name="strLogPath"></param>
            /// <returns></returns>
            private bool CheckDirectory(string strLogPath)
            {
                try
                {
                    int nFindSlashPos = strLogPath.Trim().LastIndexOf("\\");
                    string strDirectoryname = strLogPath.Trim().Substring(0, nFindSlashPos);

                    if (Directory.Exists(strDirectoryname) == false)
                    {
                        //LogInfo("Creating log directory :" + strDirectoryname);
                        Directory.CreateDirectory(strDirectoryname);
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public void Log(String message, params object[] args)
            {
                DateTime datet = DateTime.Now;
                if (sw == null)
                {
                    sw = new StreamWriter(strPathName, true);
                }
                sw.WriteLine(String.Format(message, args));
                sw.Flush();
            }

            /// <summary>
            /// Close stream
            /// </summary>
            public void Close()
            {
                if (sw != null)
                {
                    sw.Close();
                    sw = null;                    
                }
            }
                        
        }               

    }

}