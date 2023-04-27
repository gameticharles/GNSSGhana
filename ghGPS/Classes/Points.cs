using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    /// <summary>
    /// Point details structure
    /// </summary>
    public class Points
    {
        /// <summary>
        /// Site ID of the stn
        /// </summary>
        public string SiteID { get; set; }

        /// <summary>
        /// Eastings of point
        /// </summary>
        public string Eastings { get; set; }

        /// <summary>
        /// Northings of the point
        /// </summary>
        public string Northings { get; set; }


        public Points(string SiteID, string Eastings, string Northings)
        {
            this.SiteID = SiteID;
            this.Eastings = Eastings;
            this.Northings = Northings;

            //Rovers = new List<Rover>();
        }
        //public List<Rover> Rovers { get; set; }
        public Points(string[] More)
        {
            this.SiteID = More[0].Trim();
            this.Eastings = More[1].Trim();
            this.Northings = More[2].Trim();

        }
        public Points(Points More)
        {
            this.SiteID = More.SiteID;
            this.Eastings = More.Eastings;
            this.Northings = More.Northings;

        }



        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
