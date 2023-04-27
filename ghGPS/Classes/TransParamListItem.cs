using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public class TransParamListItem
    {
        /// <summary>
        /// Unit name
        /// </summary>
        public string TransParamName { get; set; }

        /// <summary>
		/// Shift in X - meters.
		/// </summary>
		public string Dx { get; set; }
        /// <summary>
        /// Shift in Y - meters.
        /// </summary>
        public string Dy { get; set; }
        /// <summary>
        /// Shift in Z - meters.
        /// </summary>
        public string Dz { get; set; }
        /// <summary>
        /// Rotation in arc seconds.
        /// </summary>
        public string Rx { get; set; }
        /// <summary>
        /// Rotation in arc seconds.
        /// </summary>
        public string Ry { get; set; }
        /// <summary>
        /// Rotation in arc seconds.
        /// </summary>
        public string Rz { get; set; }
        /// <summary>
        /// Scaling in parts per million.
        /// </summary>
        public string Ppm { get; set; }
        /// <summary>
		/// Center of rotation in X.
		/// </summary>
		public string Xm { get; set; }
        /// <summary>
        /// Center of rotation in Y.
        /// </summary>
        public string Ym { get; set; }
        /// <summary>
        /// Center of rotation in Z.
        /// </summary>
        public string Zm { get; set; }

        /// <summary>
		/// Initializes an instance of Wgs84ConversionInfo
		/// </summary>
		/// <param name="dx">Bursa Wolf shift in meters.</param>
		/// <param name="dy">Bursa Wolf shift in meters.</param>
		/// <param name="dz">Bursa Wolf shift in meters.</param>
		/// <param name="Rx">Bursa Wolf rotation in arc seconds.</param>
		/// <param name="Ry">Bursa Wolf rotation in arc seconds.</param>
		/// <param name="Rz">Bursa Wolf rotation in arc seconds.</param>
		/// <param name="ppm">Bursa Wolf scaling in parts per million.</param>
		/// <param name="areaOfUse">Area of use for this transformation</param>
		public TransParamListItem(string transParamName, string dx, string dy, string dz, string rx, string ry, string rz, string ppm, string xm, string ym, string zm)
        {
            TransParamName = transParamName;
            Dx = dx; Dy = dy; Dz = dz;
            Rx = rx; Ry = ry; Rz = rz;
            Ppm = ppm; Xm = xm; Ym = ym;
            Zm = zm;
        }
    }
}
