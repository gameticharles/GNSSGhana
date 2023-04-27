using DotSpatial.Projections;
using ghGPS.Classes.CoordinateSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using static ghGPS.Classes.SRIDReader;

namespace ghGPS.Classes
{
    /// <summary>
    /// Contains geodetic computations
    /// and all coordinate conversion (Transformation, Projections and Re-projections)
    /// </summary>
    public class CoordinateSystemConversion
    {        
             
        /// <summary>
        /// Get all SRIDs from WKT
        /// </summary>
        IEnumerable<WKTstring> SRIDs = MainScreen.AllSRID as IEnumerable<WKTstring>;

        /// <summary>
        /// Converts coordinates in projected meters to decimal degrees.
        /// </summary>
        /// <param name="p">Point in meters</param>
        /// <returns>Transformed point in decimal degrees</returns>
        public virtual double[] ENU2LatLonH(GridSystem Grid, double[] ENU)
        {
            double con, phi;        /* temporary angles				*/
            double delta_phi;   /* difference between longitudes		*/
            long i;         /* counter variable				*/
            double sin_phi, cos_phi, tan_phi;   /* sin cos and tangent values	*/
            double c, cs, t, ts, n, r, d, ds;   /* temporary variables		*/
            long max_iter = 6;          /* maximum number of iterations	*/

            double gscale, conv;

            ko = Grid.ko;
            λ0 = ToRadians(Grid.λ0);
            φ0 = ToRadians(Grid.φ0);
            _metersPerUnit = Grid.ToMetersFactor;
            FE = Grid.FE * _metersPerUnit;
            FN = Grid.FN * _metersPerUnit;
            b = Grid.b;
            a = Grid.a;
            
            es = 1.0 - Math.Pow(this.b / this.a, 2);
            e = Math.Sqrt(es);
            e0 = e0fn(es);
            e1 = e1fn(es);
            e2 = e2fn(es);
            e3 = e3fn(es);
            ml0 = this.a * mlfn(e0, e1, e2, e3, φ0);
            esp = es / (1.0 - es);

            double x = ENU[0] * _metersPerUnit - FE;
            double y = ENU[1] * _metersPerUnit - FN;

            con = (ml0 + y / ko) / this.a;
            phi = con;
            for (i = 0; ; i++)
            {
                delta_phi = ((con + e1 * Math.Sin(2.0 * phi) - e2 * Math.Sin(4.0 * phi) + e3 * Math.Sin(6.0 * phi))
                    / e0) - phi;
                phi += delta_phi;
                if (Math.Abs(delta_phi) <= EPSLN) break;
                if (i >= max_iter)
                    throw new ArgumentException("Latitude failed to converge");
            }

            var result = new double[] { };            

            if (Math.Abs(phi) < HALF_PI)
            {
                sincos(phi, out sin_phi, out cos_phi);
                tan_phi = Math.Tan(phi);
                c = esp * Math.Pow(cos_phi, 2);
                cs = Math.Pow(c, 2);
                t = Math.Pow(tan_phi, 2);
                ts = Math.Pow(t, 2);
                con = 1.0 - es * Math.Pow(sin_phi, 2);
                n = this.a / Math.Sqrt(con);
                r = n * (1.0 - es) / con;
                d = x / (n * ko);
                ds = Math.Pow(d, 2);

                double lat = phi - (n * tan_phi * ds / r) * (0.5 - ds / 24.0 * (5.0 + 3.0 * t +
                    10.0 * c - 4.0 * cs - 9.0 * esp - ds / 30.0 * (61.0 + 90.0 * t +
                    298.0 * c + 45.0 * ts - 252.0 * esp - 3.0 * cs)));
                double lon = adjust_lon(λ0 + (d * (1.0 - ds / 6.0 * (1.0 + 2.0 * t +
                    c - ds / 20.0 * (5.0 - 2.0 * c + 28.0 * t - 3.0 * cs + 8.0 * esp +
                    24.0 * ts))) / cos_phi));

                lat = ToDegrees(lat);
                lon = ToDegrees(lon);
                gscale = GridScale(Grid.e_sq, lat, lon, Grid.λ0, Grid.ko);
                conv = Convergence(lat, lon, Grid.λ0);

                if (ENU.Length < 3)
                    result = new double[] { lat, lon, 0 , gscale, conv };
                else
                    result = new double[] { lat, lon, ENU[2], gscale, conv };
            }
            else
            {

                gscale = GridScale(Grid.e_sq, ToDegrees(HALF_PI * sign(y)), ToDegrees(λ0), Grid.λ0, Grid.ko);
                conv = Convergence(ToDegrees(HALF_PI * sign(y)), ToDegrees(λ0), Grid.λ0);

                if (ENU.Length < 3)
                    result = new double[] { ToDegrees(HALF_PI * sign(y)), ToDegrees(λ0), 0, gscale, conv };
                else
                    result = new double[] { ToDegrees(HALF_PI * sign(y)), ToDegrees(λ0), ENU[2], gscale, conv };

            }

            return result;
        }
              

        /// <summary>
        /// Convert Latitude, Longitude and height to Cartesian XYZ
        /// </summary>
        /// <param name="a"></param>
        /// <param name="invF"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="E_Hgt"></param>
        /// <returns>[X,Y,Z]</returns>
        public virtual double[] LatLonH2XYZ(GridSystem Grid, double[] latLonH)
        {
            var a = Grid.a;
            var invF = Grid.invF;

            double ϕ = ToRadians(latLonH[0]); double λ = ToRadians(latLonH[1]); double E_Hgt = latLonH[2];
            var e_sq = Grid.e_sq;
            var v = V(a, ϕ, e_sq);

            var X = (v + E_Hgt) * Math.Cos(ϕ) * Math.Cos(λ);
            var Y = (v + E_Hgt) * Math.Cos(ϕ) * Math.Sin(λ);
            var Z = ((1 - e_sq) * (v + E_Hgt)) * Math.Sin(ϕ);

            return new double[] { X, Y, Z };
        }

        /// <summary>
        /// 3 Dimensional Similarity Transformation
        /// </summary>
        /// <param name="trasPara"> 1 by 10 Matrix </param> 
        /// <param name="XYZ">Point to transform 1 by 3</param>
        /// <param name="IsToWGS84">Are the transParam to ToWGS84 </param>
        /// <returns></returns>
        public virtual double[] XYZTransformation(double[] trasPara, double[] XYZ, bool IsToWGS84)
        {
            var X = XYZ[0];
            var Y = XYZ[1];
            var Z = XYZ[2];
            
            var δx = trasPara[0];
            var δy = trasPara[1];
            var δz = trasPara[2];            
            var Rx = ToRadians(trasPara[3] / 3600);
            var Ry = ToRadians(trasPara[4] / 3600);
            var Rz = ToRadians(trasPara[5] / 3600);
            var scale =(trasPara[6] / 1000000);
            var Xm = trasPara[7];
            var Ym = trasPara[8];
            var Zm = trasPara[9];

            if (!IsToWGS84)
            {
                 δx *= -1;
                 δy *= -1;
                 δz *= -1;
                 scale *= -1;
                 Rx *= -1;
                 Ry *= -1;
                 Rz *= -1;
                 Xm *= -1;
                 Ym *= -1;
                Zm *= -1;
            }

            var X1 = X + (scale * (X - Xm)) + (Rz * (Y - Ym)) - (Ry * (Z - Zm)) + δx;
            var Y1 = Y - (Rz * (X - Xm)) + (scale * (Y - Ym)) + (Rx * (Z - Zm)) + δy;
            var Z1 = Z + (Ry * (X - Xm)) - (Rx * (Y - Ym)) + (scale * (Z - Zm)) + δz;
            
            return new double[] { X1, Y1, Z1 };
        }

        /// <summary>
        /// 3 Dimensional Similarity Transformation
        /// </summary>
        /// <param name="trasPara"> Transformation Parameter </param> 
        /// <param name="XYZ">Point to transform 1 by 3</param>
        /// <param name="IsToWGS84">Is the transParam to ToWGS84 </param>
        /// <returns></returns>
        public virtual double[] XYZTransformation(TransParams trasPara, double[] XYZ, bool IsToWGS84)
        {            
             return XYZTransformation(trasPara.getValues(),  XYZ, IsToWGS84);
        }

        /// <summary>
        /// Cartesian XYZ to Convert Latitude, Longitude and height
        /// </summary>
        /// <param name="a">Semi Major</param>
        /// <param name="invF">Inverse Flattening</param>
        /// <param name="XYZ">Cartesian XYZ = [X,Y,Z]</param>
        /// <returns>Returns [Lat,lon,H]</returns>
        public virtual double[] XYZ2LatLonH(GridSystem Grid, double[] XYZ)
        {
            var X = XYZ[0];
            var Y = XYZ[1];
            var Z = XYZ[2];

            var a = Grid.a;
            var invF = Grid.invF;

            //var e_sq = Grid.e_sq;

            //var b = a * (1 - (1 / invF));
            //var f = 1 / invF;

            //var ρ = Math.Pow(Math.Pow(X, 2) + Math.Pow(Y, 2), 0.5);
            //var υ = Math.Atan((a * Z) / (b * ρ));
            //var μ = Math.Atan((Z / ρ) * ((1 - f) + (e_sq * a) / υ));

            //var λ = ToDegrees(Math.Atan(Y / X));

            //var latTop = Z + Math.Pow(((a - b) / b), 2) * b * Math.Pow(Math.Sin(υ), 3);
            //var latButton = ρ - e_sq * a * Math.Pow(Math.Cos(υ), 3);
            //var ϕ = ToDegrees(Math.Atan(latTop / latButton));

            //var H = ρ * Math.Cos(ToRadians(ϕ)) + Z * Math.Sin(ToRadians(ϕ)) - (a * Math.Sqrt(1 - (e_sq * Math.Pow(Math.Sin(ToRadians(ϕ)), 2))));

            //Second eccentricity
            var e2 = (2 * (1 / invF)) - Math.Pow((1 / invF), 2);

            
            //tailor the output to fit the equatorial quadrant as determined by the signs of X and Y
            double λ  = Math.Atan2(Y, X);

            var RootXYSqr = Math.Sqrt((System.Math.Pow(X, 2)) + (System.Math.Pow(Y, 2)));

            var lat1 = Math.Atan(Z / (RootXYSqr * (1 - e2)));

            double V = a / Math.Sqrt(1 - (e2 * Math.Pow(Math.Sin(lat1), 2)));

            var lat2 = Math.Atan((Z+ (e2 * V * (Math.Sin(lat1)))) / RootXYSqr);

            while (Math.Abs(lat1 - lat2) > 0.000000001)
            {
                lat1 = lat2;
                V = a / (Math.Sqrt(1 - (e2 * (System.Math.Pow(Math.Sin(lat1), 2)))));
                lat2 = Math.Atan((Z + (e2 * V * (Math.Sin(lat1)))) / RootXYSqr);
            }

            //Latitude
            var ϕ = lat2;

            //Redefining V
            V = a / (Math.Sqrt(1 - (e2 * (System.Math.Pow(Math.Sin(ϕ), 2)))));

            var H = (RootXYSqr / Math.Cos(ϕ)) - V;
                       
            return new double[] { ToDegrees(ϕ), ToDegrees(λ), H };
        }
        
        public virtual double[] MRE(GridSystem Grid, double[] LatLonH, bool IsToWGS84)
        {
            double[] MRELatLonH = new double[3];
            if (Grid.MREParam == null)
            {
                return MRELatLonH;
            }

            //The 1,2,3 determines the degree of V and U
            var A00 = Grid.MREParam.A00;
            var B00 = Grid.MREParam.B00;
            var A10 = Grid.MREParam.A10;
            var B10 = Grid.MREParam.B10;
            var A01 = Grid.MREParam.A01;
            var B01 = Grid.MREParam.B01;
            var A20 = Grid.MREParam.A20;
            var B20 = Grid.MREParam.B20;
            var A11 = Grid.MREParam.A11;
            var B11 = Grid.MREParam.B11;
            var A02 = Grid.MREParam.A02;
            var B02 = Grid.MREParam.B02;
            var A30 = Grid.MREParam.A30;
            var B30 = Grid.MREParam.B30;
            var A21 = Grid.MREParam.A21;
            var B21 = Grid.MREParam.B21;
            var A12 = Grid.MREParam.A12;
            var B12 = Grid.MREParam.B12;
            var A03 = Grid.MREParam.A03;
            var B03 = Grid.MREParam.B03;

            var k = 0.05235988;

            var U = k * (LatLonH[0]- Grid.φ0);
            var V = k * (LatLonH[1] - Grid.λ0);

            var dlat = A00 + (A10 * U) + (A01 * V) + (Math.Pow(A20 * U, 2)) + 
                (A11 * U * V) + (System.Math.Pow(A02 * V, 2)) + (System.Math.Pow(A30 * U, 3)) + 
                (System.Math.Pow(A21 * U, 2 * V)) + (System.Math.Pow(A12 * U * V, 2)) + 
                (System.Math.Pow(A03 * V, 3));

            var dlon = B00 + (B10 * U) + (B01 * V) + (Math.Pow(B20 * U, 2)) + 
                (B11 * U * V) + (System.Math.Pow(B02 * V, 2)) + (System.Math.Pow(B30 * U, 3)) + 
                (System.Math.Pow(B21 * U, 2 * V)) + (System.Math.Pow(B12 * U * V, 2)) + 
                (System.Math.Pow(B03 * V, 3));

            if (IsToWGS84)
            {
                MRELatLonH[0] = -dlat + LatLonH[0];
                MRELatLonH[1] = -dlon + LatLonH[1];
                MRELatLonH[2] = LatLonH[2];
            }
            else
            {
                MRELatLonH[0] = dlat + LatLonH[0];
                MRELatLonH[1] = dlon + LatLonH[1];
                MRELatLonH[2] = LatLonH[2];
            }
            
            return MRELatLonH;
        }

        /// <summary>
        /// Converts coordinates in decimal degrees to projected meters.
        /// Results:
        /// Easting, Northing, Height , Convergence, Grid Scale, Zone Number, Lat Zone
        /// </summary>
        /// <param name="lonlat">The point in decimal degrees.</param>
        /// <returns>
        /// Point in projected meters            ///
        /// </returns>
        public virtual string[] LatLonH2ENU(GridSystem Grid, double[] latlon)
        {
            double lat = ToRadians(latlon[0]);
            double lon = ToRadians(latlon[1]);
            
            ko = Grid.ko;
            λ0 = ToRadians(Grid.λ0);
            φ0 = ToRadians(Grid.φ0);
            _metersPerUnit = Grid.ToMetersFactor;
            FE = Grid.FE * _metersPerUnit;
            FN = Grid.FN * _metersPerUnit;
            b = Grid.b;
            a = Grid.a;
            
            es = 1.0 - Math.Pow(this.b / this.a, 2);
            e = Math.Sqrt(es);
            e0 = e0fn(es);
            e1 = e1fn(es);
            e2 = e2fn(es);
            e3 = e3fn(es);
            ml0 = this.a * mlfn(e0, e1, e2, e3, φ0);
            esp = es / (1.0 - es);

            double delta_lon = 0.0; /* Delta longitude (Given longitude - center 	*/
            double sin_phi, cos_phi;/* sin and cos value				*/
            double al, als;     /* temporary values				*/
            double c, t, tq;    /* temporary values				*/
            double con, n, ml;  /* cone constant, small m			*/

            delta_lon = adjust_lon(lon - λ0);
            sincos(lat, out sin_phi, out cos_phi);

            al = cos_phi * delta_lon;
            als = Math.Pow(al, 2);
            c = esp * Math.Pow(cos_phi, 2);
            tq = Math.Tan(lat);
            t = Math.Pow(tq, 2);
            con = 1.0 - es * Math.Pow(sin_phi, 2);
            n = this.a / Math.Sqrt(con);
            ml = this.a * mlfn(e0, e1, e2, e3, lat);

            double E =
                ko * n * al * (1.0 + als / 6.0 * (1.0 - t + c + als / 20.0 *
                (5.0 - 18.0 * t + Math.Pow(t, 2) + 72.0 * c - 58.0 * esp))) + FE;

            double N = ko * (ml - ml0 + n * tq * (als * (0.5 + als / 24.0 *
                (5.0 - t + 9.0 * c + 4.0 * Math.Pow(c, 2) + als / 30.0 * (61.0 - 58.0 * t
                + Math.Pow(t, 2) + 600.0 * c - 330.0 * esp))))) + FN;


            double zone = getLongZone(latlon[1]);
            LatZones latZones = new LatZones();
            string latZone = latZones.getLatZone(latlon[0]);            
            double gscale = GridScale(Grid.e_sq, latlon[0], latlon[1], Grid.λ0, Grid.ko);
            double conv = Convergence(latlon[0], latlon[1], Grid.λ0);

            if (latlon.Length < 3)
                return new string[] { (E / _metersPerUnit).ToString(), (N / _metersPerUnit).ToString(), "0", conv.ToString(), gscale.ToString(), zone.ToString("00"), latZone };
            else
                return new string[] { (E / _metersPerUnit).ToString(), (N / _metersPerUnit).ToString(), latlon[2].ToString(), conv.ToString(), gscale.ToString(), zone.ToString("00"), latZone };

        }

        /// <summary>
        /// Converts coordinates in decimal degrees to projected meters.
        /// Results:
        /// Eastings, Northings, Height , Convergence, Grid Scale, Zone Number, Lat Zone
        /// </summary>
        /// <param name="Grid"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public virtual string[] LatLonH2ENU(GridSystem Grid, double latitude, double longitude, double height)
        {
            return LatLonH2ENU(Grid, new double[] { latitude, longitude, height });
        }
        
        /// <summary>
        /// Convert WGS84 LatLonH To UTM
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public virtual string[] WGS84latLon2UTM(double latitude, double longitude, double height)
        {            
            return convertLatLonToUTM(latitude, longitude, height);
        }

        /// <summary>
        /// Convert WGS84 LatLonH To UTM 
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public virtual string[] WGS84latLon2UTM(double[] latlon)
        {            
            return WGS84latLon2UTM(latlon[0], latlon[1], (latlon.Length == 3)? latlon[2]:0);
        }

        private string[] convertLatLonToUTM(double latitude, double longitude, double height)
        {
            validate(latitude, longitude);
            
            string[] UTM = new string[] { };
            var wgs84 = new GridSystem();            

            double var1;
            if (longitude < 0.0)
            {
                var1 = ((int)((180 + longitude) / 6.0)) + 1;
            }
            else
            {
                var1 = ((int)(longitude / 6)) + 31;
            }
            double var2 = (6 * var1) - 183;

            wgs84.setValues(6378137, 298.257223563, 0.9996, 0, var2, 500000, 0, 1, "Transverse_Mercator");
            
            return LatLonH2ENU(wgs84, latitude, longitude, height);

        }

        /// <summary>
        /// Convert UTM string array to Lat, Lon, Height, Grid Scale, Convergence
        /// </summary>
        /// <param name="UTM">UTM strin array</param>
        /// <returns></returns>
        public virtual double[] WGS84UTMToLatLon(string[] UTM)
        {
            return convertUTMToLatLon(UTM);
        }

        /// <summary>
        /// Convert UTM to Lat, Lon, Height, Grid Scale, Convergence
        /// </summary>
        /// <param name="UTM">UTM strin array</param>
        /// <returns></returns>
        public virtual double[] WGS84UTMToLatLon(double Easting, double Northing, double Height, int Zone, string ZoneLat)
        {
            return convertUTMToLatLon(new string[] { Easting.ToString(), Northing.ToString(), Height.ToString(),"0", "0", Zone.ToString(), ZoneLat });
        }

        /// <summary>
        /// Convert UTM to Lat, Lon, Height, Grid Scale, Convergence
        /// </summary>
        /// <param name="UTM"></param>
        /// <returns></returns>
        private double[] convertUTMToLatLon(string[] UTM)
        {                            
            var wgs84 = new GridSystem().WGS84();

            var easting = Convert.ToDouble(UTM[0]);
            var northing = Convert.ToDouble(UTM[1]);

            var zone = Convert.ToInt32(UTM[5]);
            string latZone = UTM[6];
            string hemisphere = getHemisphere(latZone);

            if (hemisphere.Equals("S"))
            {
                northing = 10000000 - northing;
            }

            var arc = northing / wgs84.ko;
            var mu = arc / (a * (1 - Math.Pow(e, 2) / 4.0 - 3 * Math.Pow(e, 4) / 64.0 - 5 * Math.Pow(e, 6) / 256.0));

            var ei = (1 - Math.Pow((1 - e * e), (1 / 2.0))) / (1 + Math.Pow((1 - e * e), (1 / 2.0)));

            var ca = 3 * ei / 2 - 27 * Math.Pow(ei, 3) / 32.0;

            var cb = 21 * Math.Pow(ei, 2) / 16 - 55 * Math.Pow(ei, 4) / 32;
            var cc = 151 * Math.Pow(ei, 3) / 96;
            var cd = 1097 * Math.Pow(ei, 4) / 512;
            var phi1 = mu + ca * Math.Sin(2 * mu) + cb * Math.Sin(4 * mu) + cc * Math.Sin(6 * mu) + cd * Math.Sin(8 * mu);

            var n0 = a / Math.Pow((1 - Math.Pow((e * Math.Sin(phi1)), 2)), (1 / 2.0));

            var r0 = a * (1 - e * e) / Math.Pow((1 - Math.Pow((e * Math.Sin(phi1)), 2)), (3 / 2.0));
            var fact1 = n0 * Math.Tan(phi1) / r0;

            var _a1 = FE - easting;
            var dd0 = _a1 / (n0 * wgs84.ko);
            var fact2 = dd0 * dd0 / 2;

            var t0 = Math.Pow(Math.Tan(phi1), 2);
            var Q0 = wgs84.e_sq * Math.Pow(Math.Cos(phi1), 2);
            var fact3 = (5 + 3 * t0 + 10 * Q0 - 4 * Q0 * Q0 - 9 * wgs84.e_sq) * Math.Pow(dd0, 4) / 24;

            var fact4 = (61 + 90 * t0 + 298 * Q0 + 45 * t0 * t0 - 252 * wgs84.e_sq - 3 * Q0 * Q0) * Math.Pow(dd0, 6) / 720;

            //
            var lof1 = _a1 / (n0 * wgs84.ko);
            var lof2 = (1 + 2 * t0 + Q0) * Math.Pow(dd0, 3) / 6.0;
            var lof3 = (5 - 2 * Q0 + 28 * t0 - 3 * Math.Pow(Q0, 2) + 8 * wgs84.e_sq + 24 * Math.Pow(t0, 2)) * Math.Pow(dd0, 5) / 120;
            var _a2 = (lof1 - lof2 + lof3) / Math.Cos(phi1);
            var _a3 = _a2 * 180 / Math.PI;
            
            var latitude = 180 * (phi1 - fact1 * (fact2 + fact3 + fact4)) / Math.PI;

            double zoneCM;
            if (zone > 0)
            {
                zoneCM = 6 * zone - 183.0;
            }
            else
            {
                zoneCM = 3.0;
            }

            var longitude = zoneCM - _a3;

            if (hemisphere.Equals("S"))
            {
                latitude = -latitude;
            }

            double[] latlon = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0 };

            latlon[0] = latitude;
            latlon[1] = longitude;
            latlon[2] = Convert.ToDouble(UTM[2]);

            latlon[3] = GridScale(wgs84.e_sq, latitude, longitude, wgs84.λ0, wgs84.ko);

            latlon[4] = Convergence(latitude, longitude, wgs84.λ0);

            return latlon;

        }
        
        private double[] MGRUTMToUTMdouble(string mgrutm)
        {
            double[] latlon = new double[] { 0.0, 0.0 };
            // 02CNR0634657742
            int zone = Convert.ToInt32(mgrutm.Substring(0, 2));
            string latZone = mgrutm.Substring(2, 1);

            string digraph1 = mgrutm.Substring(3, 1);
            string digraph2 = mgrutm.Substring(4, 1);
            var easting = Convert.ToDouble(mgrutm.Substring(5, 5));
            var northing = Convert.ToDouble(mgrutm.Substring(10, 5));

            LatZones lz = new LatZones();
            double latZoneDegree = lz.getLatZoneDegree(latZone);

            double a1 = latZoneDegree * 40000000 / 360.0;
            double a2 = 2000000 * Math.Floor(a1 / 2000000.0);

            Digraphs digraphs = new Digraphs();

            double digraph2Index = digraphs.getDigraph2Index(digraph2);

            double startindexEquator = 1;
            if ((1 + zone % 2) == 1)
            {
                startindexEquator = 6;
            }

            double a3 = a2 + (digraph2Index - startindexEquator) * 100000;
            if (a3 <= 0)
            {
                a3 = 10000000 + a3;
            }
            northing = a3 + northing;

            double digraph1Index = digraphs.getDigraph1Index(digraph1);
            int a5 = 1 + zone % 3;
            double[] a6 = new double[] { 16, 0, 8 };
            double a7 = 100000 * (digraph1Index - a6[a5 - 1]);
            easting = easting + a7;

            return  new double[] { easting, northing ,0};
        }

        public virtual string[] MGRUTM2UTM(string mgrutm)
        {
            var UTM = MGRUTMToLatLong(mgrutm);

            return WGS84latLon2UTM(UTM);
        }

        public virtual double[] MGRUTMToLatLong(string mgrutm)
        {
            var UTM = MGRUTMToUTMdouble(mgrutm);            
            return ENU2LatLonH(new GridSystem().WGS84(), UTM);
        }
        

        /* Variables common to all subroutines in this code file
        -----------------------------------------------------*/
        private double ko;    /* scale factor				*/
        private double λ0;    /* Center longitude (projection center) */
        private double φ0;  /* center latitude			*/
        private double e0, e1, e2, e3;  /* eccentricity constants		*/
        private double e, es, esp;      /* eccentricity constants		*/
        private double ml0;     /* small value m			*/
        private double FN;  /* y offset in meters			*/
        private double FE;   /* x offset in meters			*/
                             //static double ind;		/* spherical flag			*/
        private double a;
        private double b;
        private double _metersPerUnit;

        internal string southernHemisphere = "ACDEFGHJKLM";

        protected internal virtual string getHemisphere(string latZone)
        {
            string hemisphere = "N";
            if (southernHemisphere.IndexOf(latZone, StringComparison.Ordinal) > -1)
            {
                hemisphere = "S";
            }
            return hemisphere;
        }

        #region Helper mathematical functions

        /// <summary>
        /// Function to calculate the sine and cosine in one call.  Some computer
        /// systems have implemented this function, resulting in a faster implementation
        /// than calling each function separately.  It is provided here for those
        /// computer systems which don`t implement this function
        /// </summary>
        protected static void sincos(double val, out double sin_val, out double cos_val)

        {
            sin_val = Math.Sin(val);
            cos_val = Math.Cos(val);
        }

        // defines some usefull constants that are used in the projection routines
        /// <summary>
        /// PI
        /// </summary>
        protected const double PI = Math.PI;

        /// <summary>
        /// Half of PI
        /// </summary>
        protected const double HALF_PI = (PI * 0.5);

        /// <summary>
        /// PI * 2
        /// </summary>
        protected const double TWO_PI = (PI * 2.0);

        /// <summary>
        /// EPSLN
        /// </summary>
        protected const double EPSLN = 1.0e-10;

        /// <summary>
        /// S2R
        /// </summary>
        protected const double S2R = 4.848136811095359e-6;

        /// <summary>
        /// MAX_VAL
        /// </summary>
        protected const double MAX_VAL = 4;

        /// <summary>
        /// prjMAXLONG
        /// </summary>
        protected const double prjMAXLONG = 2147483647;

        /// <summary>
        /// DBLLONG
        /// </summary>
        protected const double DBLLONG = 4.61168601e18;

        /// <summary>
        /// Returns the cube of a number.
        /// </summary>
        /// <param name="x"> </param>
        protected static double CUBE(double x)
        {
            return Math.Pow(x, 3);   /* x^3 */
        }

        /// <summary>
        /// Returns the quad of a number.
        /// </summary>
        /// <param name="x"> </param>
        protected static double QUAD(double x)
        {
            return Math.Pow(x, 4);  /* x^4 */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        protected static double GMAX(ref double A, ref double B)
        {
            return Math.Max(A, B); /* assign maximum of a and b */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        protected static double GMIN(ref double A, ref double B)
        {
            return ((A) < (B) ? (A) : (B)); /* assign minimum of a and b */
        }

        /// <summary>
        /// IMOD
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        protected static double IMOD(double A, double B)
        {
            return (A) - (((A) / (B)) * (B)); /* Integer mod function */

        }

        ///<summary>
        ///Function to return the sign of an argument
        ///</summary>
        protected static double sign(double x)
        {
            if (x < 0.0)
                return (-1);
            else return (1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected static double adjust_lon(double x)
        {
            long count = 0;
            for (;;)
            {
                if (Math.Abs(x) <= PI)
                    break;
                else
                    if (((long)Math.Abs(x / Math.PI)) < 2)
                    x = x - (sign(x) * TWO_PI);
                else
                    if (((long)Math.Abs(x / TWO_PI)) < prjMAXLONG)
                {
                    x = x - (((long)(x / TWO_PI)) * TWO_PI);
                }
                else
                    if (((long)Math.Abs(x / (prjMAXLONG * TWO_PI))) < prjMAXLONG)
                {
                    x = x - (((long)(x / (prjMAXLONG * TWO_PI))) * (TWO_PI * prjMAXLONG));
                }
                else
                    if (((long)Math.Abs(x / (DBLLONG * TWO_PI))) < prjMAXLONG)
                {
                    x = x - (((long)(x / (DBLLONG * TWO_PI))) * (TWO_PI * DBLLONG));
                }
                else
                    x = x - (sign(x) * TWO_PI);
                count++;
                if (count > MAX_VAL)
                    break;
            }
            return (x);
        }

        /// <summary>
        /// Function to compute the constant small m which is the radius of
        /// a parallel of latitude, phi, divided by the semimajor axis.
        /// </summary>
        protected static double msfnz(double eccent, double sinphi, double cosphi)
        {
            double con;

            con = eccent * sinphi;
            return ((cosphi / (Math.Sqrt(1.0 - con * con))));
        }

        /// <summary>
        /// Function to compute constant small q which is the radius of a 
        /// parallel of latitude, phi, divided by the semimajor axis. 
        /// </summary>
        protected static double qsfnz(double eccent, double sinphi)
        {
            double con;

            if (eccent > 1.0e-7)
            {
                con = eccent * sinphi;
                return ((1.0 - eccent * eccent) * (sinphi / (1.0 - con * con) - (.5 / eccent) *
                    Math.Log((1.0 - con) / (1.0 + con))));
            }
            else
                return 2.0 * sinphi;
        }


        /// <summary>
        /// Function to compute the constant small t for use in the forward
        /// computations in the Lambert Conformal Conic and the Polar
        /// Stereographic projections.
        /// </summary>
        protected static double tsfnz(double eccent, double phi, double sinphi)
        {
            double con;
            double com;
            con = eccent * sinphi;
            com = .5 * eccent;
            con = Math.Pow(((1.0 - con) / (1.0 + con)), com);
            return (Math.Tan(.5 * (HALF_PI - phi)) / con);
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="eccent"></param>
        /// <param name="qs"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        protected static double phi1z(double eccent, double qs, out long flag)
        {
            double eccnts;
            double dphi;
            double con;
            double com;
            double sinpi;
            double cospi;
            double phi;
            flag = 0;
            //double asinz();
            long i;

            phi = asinz(.5 * qs);
            if (eccent < EPSLN)
                return (phi);
            eccnts = eccent * eccent;
            for (i = 1; i <= 25; i++)
            {
                sincos(phi, out sinpi, out cospi);
                con = eccent * sinpi;
                com = 1.0 - con * con;
                dphi = .5 * com * com / cospi * (qs / (1.0 - eccnts) - sinpi / com +
                    .5 / eccent * Math.Log((1.0 - con) / (1.0 + con)));
                phi = phi + dphi;
                if (Math.Abs(dphi) <= 1e-7)
                    return (phi);
            }
            //p_error ("Convergence error","phi1z-conv");
            //ASSERT(FALSE);
            throw new ArgumentException("Convergence error.");
        }

        ///<summary>
        ///Function to eliminate roundoff errors in asin
        ///</summary>
        protected static double asinz(double con)
        {
            if (Math.Abs(con) > 1.0)
            {
                if (con > 1.0)
                    con = 1.0;
                else
                    con = -1.0;
            }
            return (Math.Asin(con));
        }

        /// <summary>
        /// Function to compute the latitude angle, phi2, for the inverse of the
        /// Lambert Conformal Conic and Polar Stereographic projections.
        /// </summary>
        /// <param name="eccent">Spheroid eccentricity</param>
        /// <param name="ts">Constant value t</param>
        /// <param name="flag">Error flag number</param>
        protected static double phi2z(double eccent, double ts, out long flag)
        {
            double con;
            double dphi;
            double sinpi;
            long i;

            flag = 0;
            double eccnth = .5 * eccent;
            double chi = HALF_PI - 2 * Math.Atan(ts);
            for (i = 0; i <= 15; i++)
            {
                sinpi = Math.Sin(chi);
                con = eccent * sinpi;
                dphi = HALF_PI - 2 * Math.Atan(ts * (Math.Pow(((1.0 - con) / (1.0 + con)), eccnth))) - chi;
                chi += dphi;
                if (Math.Abs(dphi) <= .0000000001)
                    return (chi);
            }
            throw new ArgumentException("Convergence error - phi2z-conv");
        }

        ///<summary>
        ///Functions to compute the constants e0, e1, e2, and e3 which are used
        ///in a series for calculating the distance along a meridian.  The
        ///input x represents the eccentricity squared.
        ///</summary>
        protected static double e0fn(double x)
        {
            return (1.0 - 0.25 * x * (1.0 + x / 16.0 * (3.0 + 1.25 * x)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected static double e1fn(double x)
        {
            return (0.375 * x * (1.0 + 0.25 * x * (1.0 + 0.46875 * x)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected static double e2fn(double x)
        {
            return (0.05859375 * x * x * (1.0 + 0.75 * x));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected static double e3fn(double x)
        {
            return (x * x * x * (35.0 / 3072.0));
        }

        /// <summary>
        /// Function to compute the constant e4 from the input of the eccentricity
        /// of the spheroid, x.  This constant is used in the Polar Stereographic
        /// projection.
        /// </summary>
        protected static double e4fn(double x)
        {
            double con;
            double com;
            con = 1.0 + x;
            com = 1.0 - x;
            return (Math.Sqrt((Math.Pow(con, con)) * (Math.Pow(com, com))));
        }

        /// <summary>
        /// Function computes the value of M which is the distance along a meridian
        /// from the Equator to latitude phi.
        /// </summary>
        protected static double mlfn(double e0, double e1, double e2, double e3, double phi)
        {
            return (e0 * phi - e1 * Math.Sin(2.0 * phi) + e2 * Math.Sin(4.0 * phi) - e3 * Math.Sin(6.0 * phi));
        }

        /// <summary>
        /// Function to calculate UTM zone number--NOTE Longitude entered in DEGREES!!!
        /// </summary>
        protected static long calc_utm_zone(double lon)
        {
            return ((long)(((lon + 180.0) / 6.0) + 1.0));
        }

        #endregion

        /// <summary>
        /// Compute grid scale 
        /// </summary>
        /// <param name="e2">Eccentricity</param>
        /// <param name="latitude">Latitude of the point</param>
        /// <param name="longitude">Longitude of the point</param>
        /// <param name="LongOfOrigin">Longitude of origin from the grid</param>
        /// <param name="scale">Scale from the grid</param>
        /// <returns>Returns the grid scale</returns>
        public virtual double GridScale(double e2, double latitude, double longitude, double LongOfOrigin, double scale)
        {
            //Compute Grid scale a point
            //k = ko[1+(1+C)A2/2+(5-4T+42C+13C2-28e'2)A4/24+(61-148T+16T2)A6/720)
            //C=e'2Cos2ϕ
            //T=tan2ϕ
            //A = cos ϕ ( λ -  λo)
            var ep2 = e2 / (1 - e2); // e'2
            var T = Math.Pow(Math.Tan(ToRadians(latitude)), 2);
            var C = Math.Pow(Math.Cos(ToRadians(latitude)), 2) * ep2;
            var A = Math.Cos(ToRadians(latitude)) * (ToRadians(longitude) - ToRadians(LongOfOrigin));
            return scale * (1 + ((1 + C) * Math.Pow(A, 2)) / 2 + ((5 - 4 * T + 42 * C + 13 * Math.Pow(C, 2) - 28 * ep2) * Math.Pow(A, 4)) / 24 + ((61 - 148 * T + 16 * Math.Pow(T, 2)) * Math.Pow(A, 6)) / 720);

        }

        /// <summary>
        /// Convergence 
        /// </summary>
        /// <param name="latitude">Latitude of the point</param>
        /// <param name="longitude">Longitude of the point</param>
        /// <param name="LongOfOrigin">Longitude of origin from the grid</param>
        /// <returns>Returns the convergence</returns>
        public virtual double Convergence(double latitude, double longitude, double LongOfOrigin)
        {
            //Compute convergence(γ)                
            /*
                γ = arctan [tan (λ - λ0) × sin φ]

                where

                γ is grid convergence,
                λ0 is longitude of UTM zone's central meridian,
                φ, λ are latitude, longitude of point in question
             */

            var γ = ToDegrees(Math.Atan(Math.Tan(ToRadians(longitude - LongOfOrigin)) * Math.Sin(ToRadians(latitude))));
            return γ;
        }

        /// <summary>
        /// Get the zone from the longitude
        /// </summary>
        /// <param name="longitude">The longitude to extract the zone from</param>
        /// <returns></returns>
        protected internal virtual double getLongZone(double longitude)
        {
            double longZone = 0;
            if (longitude < 0.0)
            {
                longZone = ((180.0 + longitude) / 6) + 1;
            }
            else
            {
                longZone = (longitude / 6) + 31;
            }

            return (int)longZone;
        }

        /// <summary>
        /// Get the DMS reading in the decimal angle to a string presentation
        /// </summary>
        /// <param name="DegDEC">The angle in decimal degree</param>
        /// <param name="lat">Indicate if it is the latitude value provided</param>
        /// <param name="IsLatLon">Is the reading provided part of LAT-LONG </param>
        /// <returns>Returns the a string representation of the reading</returns>
        public virtual string ConvertToDMSString(double DegDEC, bool lat, bool IsLatLon)
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

        /// <summary>
        /// Get eccentricity square
        /// </summary>
        /// <param name="invF">Inverse flattening</param>
        /// <returns>Eccentricity square</returns>
        public virtual double Esq(double invF)
        {
            return (2 * (1 / invF)) - ((1 / invF) * (1 / invF));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a">Semi major axis</param>
        /// <param name="lat">Latitude of the point</param>
        /// <param name="e_sq">Eccentricity</param>
        /// <returns></returns>
        public virtual double V(double a, double lat, double e_sq)
        {
            // Convert to radians
            lat = ToRadians(lat);

            return a / Math.Pow(1 - (e_sq * Math.Pow(Math.Sin(lat), 2)), 0.5);
        }

        /// <summary>
        /// Convert degree value to radians
        /// </summary>
        /// <param name="degree">The value in degree to be converted</param>
        /// <returns>Radians of a degree value</returns>
        public virtual double ToRadians(double degree)
        {
            return degree * Math.PI / 180;
        }


        /// <summary>
        /// Convert radian value to degrees
        /// </summary>
        /// <param name="radian">The value in radians to convert in degree</param>
        /// <returns>Degrees of the radians value</returns>
        public virtual double ToDegrees(double radian)
        {
            return radian * 180 / Math.PI;
        }

        private static void validate(double latitude, double longitude)
        {
            if (latitude < -90.0 || latitude > 90.0 || longitude < -180.0 || longitude >= 180.0)
            {
                throw new System.ArgumentException("Legal ranges: latitude [-90,90], longitude [-180,180).");
            }
        }

        #region CONVERT DMS TO DECIMAL DEGREE
        public virtual double DMS2DecDeg(string Deg, double Min, double Sec)
        {
            double DegDec = 0;

            DegDec = (Deg == "-0" || Deg == "-00" || Deg == "-000" || Convert.ToDouble(Deg) < 0) ? (Math.Abs(Convert.ToDouble(Deg)) + Min / 60 + Sec / 3600) * -1 : Convert.ToDouble(Deg) + Min / 60 + Sec / 3600;

            return DegDec;
        }

        #endregion

        #region CONVERT DMM TO DECIMAL DEGREE
        public virtual double DMM2DecDeg(double Deg, double Min)
        {
            double DegDec = 0;

            DegDec = (Deg < 0) ? (Math.Abs(Deg) + Min / 60) * -1 : Deg + Min / 60;

            return DegDec;
        }

        #endregion

        #region CONVERT DECIMAL DEGREE TO DD MM.MMM
        public virtual Array DegDEC2DMM(double DegDEC)
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
        public virtual double[] DegDec2DMS(double DegDEC)
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


    }


}
