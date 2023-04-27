using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    /// <summary>
    /// Grid Systems
    /// </summary>
    public class GridSystem
    {
        public GridSystem()
        {

        }
        
        /// <summary>
        /// World Geograhpic System 1984
        /// </summary>
        /// <returns></returns>
        public virtual GridSystem WGS84()
        {
            //Create a new Gird system
            var _WGS84 = new GridSystem();
            _WGS84.setValues(6378137, 298.257223563, 0.9996, 0, 0, 500000, 0, 1, "Transverse_Mercator");
            var wgs84Trans = new TransParams();
            wgs84Trans.setTransParams(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            _WGS84.TransParam = wgs84Trans;
            _WGS84.MREParam = null;

            return _WGS84;
        }

        /// <summary>
        /// Ghan National Grid 
        /// </summary>
        /// <returns></returns>
        public virtual GridSystem GhanaNationalGrid(TransParams TransParam)
        {
            var _GhanaNationalGrid = new GridSystem();

            _GhanaNationalGrid.setValues(6378299.996, 296, 0.99975, 4.666666667, -1, 900000, 0, 0.304799706846, "Transverse_Mercator");//274319.736
            _GhanaNationalGrid.TransParam = TransParam;
            _GhanaNationalGrid.MREParam = new MultipleRegressionEquationParams().GHMREParams(); 

            return _GhanaNationalGrid;
        }

        /// <summary>
        /// Meters and Degrees
        /// [a, b, f, invF, e_sq, ko, φ0, λ0, FE, FN, Mo]
        /// </summary>
        /// <returns></returns>
        public virtual void setValues(double a, double invF, double ko, double φ0, double λ0, double FE, double FN, double ToMetersFactor, string projectionType)
        {
            this.a = a;
            this.invF = invF;
            this.ko = ko;
            this.φ0 = φ0;
            this.λ0 = λ0;
            this.FE = FE;
            this.FN = FN;
            this.b = a * (1 - (1 / invF));
            this.f = 1 / invF;
            this.e_sq = (2 * this.f) - (this.f * this.f);
            this.Mo = GetM_Mo(φ0);
            this.ToMetersFactor = ToMetersFactor;
            this.ProjectionType = projectionType;
            
        }
                

        /// <summary>
        /// [a, b, f, invF, e_sq, ko, φ0, λ0, FE, FN, Mo, tometerFactor]
        /// </summary>
        public virtual double[] GetValues()
        {
            return new double[] { this.a, this.b, this.f, this.invF, this.e_sq, this.ko, this.φ0, this.λ0, this.FE, this.FN, this.Mo, this.ToMetersFactor};
        }


        /// <summary>
        /// Semi-major axis
        /// </summary>
        public virtual double a { get; set; }

        /// <summary>
        /// Inverse Flattening
        /// </summary>
        public virtual double invF { get; set; }

        /// <summary>
        /// Semi-minor axis
        /// </summary>
        public virtual double b { get; set; }

        /// <summary>
        /// Scale
        /// </summary>
        public virtual double ko { get; set; }

        /// <summary>
        /// Flattening
        /// </summary>
        public virtual double f { get; set; }

        /// <summary>
        /// Latitude of Origin
        /// </summary>
        public virtual double φ0 { get; set; }

        /// <summary>
        /// Longitude of Origin
        /// </summary>
        public virtual double λ0 { get; set; }

        /// <summary>
        /// False Easting
        /// </summary>
        public virtual double FE { get; set; }

        /// <summary>
        /// False Northing
        /// </summary>
        public virtual double FN { get; set; }

        /// <summary>
        /// Eccentricity Squared
        /// </summary>
        public virtual double e_sq { get; set; }

        /// <summary>
        /// Mo of the origin
        /// </summary>
        public virtual double Mo { get; set; }

        /// <summary>
        /// Multiplying Constant to Meters
        /// </summary>
        public virtual double ToMetersFactor { get; set; }

        /// <summary>
        /// Projection Type for the system
        /// </summary>
        public virtual string ProjectionType { get; set; }

        /// <summary>
        /// Get M-value with input of latitude;
        /// </summary>
        /// <param name="lat">Latitude</param>
        /// <returns></returns>
        public virtual double GetM_Mo(double lat)
        {
            // Convert to radians
            lat = lat * Math.PI / 180;

            double A0 = 1 - (e_sq / 4) - ((3 * Math.Pow(e_sq, 2)) / 64) - ((5 * Math.Pow(e_sq, 3)) / 256);
            double A2 = (0.375) * (e_sq + (Math.Pow(e_sq, 2) / 4) + ((15 * Math.Pow(e_sq, 3)) / 128));
            double A4 = (0.05859375) * (Math.Pow(e_sq, 2) + ((3 * Math.Pow(e_sq, 3)) / 4));
            double A6 = ((35 * Math.Pow(e_sq, 6)) / 3072);

            return a * ((A0 * lat) - (A2 * Math.Sin(2 * lat)) + (A4 * Math.Sin(4 * lat)) - (A6 * Math.Sin(6 * lat)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Northing">Northing or Y value of the point</param>
        /// <returns></returns>
        public virtual double GetM(double Northing)
        {           
            return Mo + Northing / (ko);
        }

        /// <summary>
        /// Transformation parameters to WGS84
        /// </summary>
        public virtual TransParams TransParam { get; set; } = null;

        /// <summary>
        /// MRE parameters to WGS84
        /// </summary>
        public virtual MultipleRegressionEquationParams MREParam { get; set; } = null;

    }
}
