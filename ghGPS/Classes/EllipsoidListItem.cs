using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public class EllipsoidListItem
    {
        /// <summary>
        /// Name of the ellipsoid
        /// </summary>
        public string EllipsoidName { get; set; }

        /// <summary>
        /// Semi-major axis of the Ellipsoid
        /// </summary>
        public string SemiMajorAxis { get; set; }

        /// <summary>
        /// Inverse Flattening of the Ellipsoid
        /// </summary>
        public string InverseFlattening { get; set; }


        public EllipsoidListItem(string EllipsoidName, string SemiMajorAxis, string InverseFlattening)
        {
            this.EllipsoidName = EllipsoidName;
            this.SemiMajorAxis = SemiMajorAxis;
            this.InverseFlattening = InverseFlattening;

            //Rovers = new List<Rover>();
        }
    }
}
