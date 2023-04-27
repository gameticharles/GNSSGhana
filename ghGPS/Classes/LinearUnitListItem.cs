using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public class LinearUnitListItem
    {
        /// <summary>
        /// Unit name
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// Meters Per Unit conversion factor
        /// </summary>
        public string MetersPerUnit { get; set; }

        /// <summary>
        /// Authority
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Authority Code of the unit
        /// </summary>
        public string AuthorityCode { get; set; }

        /// <summary>
        /// Alias of the unit
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Abbreviation of th unit
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// Remarks of the unit
        /// </summary>
        public string Remarks { get; set; }

        public LinearUnitListItem(string UnitName, string MetersPerUnit, string Authority, string AuthorityCode, string Alias, string Abbreviation, string Remarks)
        {
            this.UnitName = UnitName;
            this.MetersPerUnit = MetersPerUnit;
            this.Authority = Authority;
            this.AuthorityCode = AuthorityCode;
            this.Alias = Alias;
            this.Abbreviation = Abbreviation;
            this.Remarks = Remarks;

            //Rovers = new List<Rover>();
        }
    }
}
