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
    public class Rover
    {
        /// <summary>
        /// Directory path to the file
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Site ID
        /// </summary>
        public string SiteID { get; set; }

        /// <summary>
        /// Time for observation start
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Time observation ended
        /// </summary>
        public string StopTime { get; set; }

        /// <summary>
        /// Epoch count during observation
        /// </summary>
        public string Epoch { get; set; }

        /// <summary>
        /// Rate of observation
        /// </summary>
        public string DataRate { get; set; }

        /// <summary>
        /// Size of the file
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Eastings of the point
        /// </summary>
        public string Eastings { get; set; }

        /// <summary>
        /// Northings of the point
        /// </summary>
        public string Northings { get; set; }

        /// <summary>
        /// Height of the point
        /// </summary>
        public string Heights { get; set; }

        /// <summary>
        /// Antenna height of the receiver
        /// </summary>
        public string AntennaHeight { get; set; }

        /// <summary>
        /// Antenna Type of the receiver
        /// </summary>
        public string AntennaType { get; set; }

        public Rover(string FilePath, string SiteID, string StartTime, string StopTime, string Epoch, string DataRate, string Size, string Eastings, string Northings, string Heights, string AntennaHeight, string AntennaType)
        {
            this.FilePath = FilePath;
            this.SiteID = SiteID;
            this.StartTime = StartTime;
            this.StopTime = StopTime;
            this.Epoch = Epoch;
            this.DataRate = DataRate;
            this.Size = Size;
            this.Eastings = Eastings;
            this.Northings = Northings;
            this.Heights = Heights;
            this.AntennaHeight = AntennaHeight;
            this.AntennaType = AntennaType;

            //Rovers = new List<Rover>();
        }
        //public List<Rover> Rovers { get; set; }
        public Rover(Rover More)
        {
            this.FilePath = More.FilePath;
            this.SiteID = More.SiteID;
            this.StartTime = More.StartTime;
            this.StopTime = More.StopTime;
            this.Epoch = More.Epoch;
            this.DataRate = More.DataRate;
            this.Size = More.Size;
            this.Eastings = More.Eastings;
            this.Northings = More.Northings;
            this.Heights = More.Heights;
            this.AntennaHeight = More.AntennaHeight;
            this.AntennaType = More.AntennaType;
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
