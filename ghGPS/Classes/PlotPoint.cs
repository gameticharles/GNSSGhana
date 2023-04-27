using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    /// <summary>
    /// Structure of a Rover data
    /// </summary>
    public class PlotPoint
    {

        /// <summary>
        /// Site ID
        /// </summary>
        public string SiteID { get; set; }

        /// <summary>
        /// Eastings of the point
        /// </summary>
        public double Eastings { get; set; }

        /// <summary>
        /// Northings of the point
        /// </summary>
        public double Northings { get; set; }

        /// <summary>
        /// Height of the point
        /// </summary>
        public double Heights { get; set; }

        public PlotPoint(string SiteID, double Eastings, double Northings, double Heights)
        {
            
            this.SiteID = SiteID;
           
            this.Eastings = Eastings;
            this.Northings = Northings;
            this.Heights = Heights;

        }      

    }
}
