using ghGPS.Classes;
using ghGPS.Classes.CoordinateSystems;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ghGPS.Forms
{
    public partial class LinearUnits : MetroSuite.MetroForm
    {
        public LinearUnit linearUnit = new LinearUnit(0, "Test", "EPSG", 10000, null, "tt", "Try");

        public LinearUnits()
        {
            InitializeComponent();
            SetupColumns();
        }


        private void SetupColumns()
        {

            this.clnUnitName.AspectGetter = delegate (object x) { return ((LinearUnitListItem)x).UnitName; ; };
            this.clnMeterPerUnit.AspectGetter = delegate (object x) { return ((LinearUnitListItem)x).MetersPerUnit; ; };
            this.clnAutority.AspectGetter = delegate (object x) { return ((LinearUnitListItem)x).Authority; ; };
            this.clnAuthorityCode.AspectGetter = delegate (object x) { return ((LinearUnitListItem)x).AuthorityCode; ; };
            this.clnAlias.AspectGetter = delegate (object x) { return ((LinearUnitListItem)x).Alias; ; };
            this.clnAbbreviation.AspectGetter = delegate (object x) { return ((LinearUnitListItem)x).Abbreviation; ; };
            this.clnRemark.AspectGetter = delegate (object x) { return ((LinearUnitListItem)x).Remarks; ; };

        }

        private void olvPointListTree_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                
                var linearUnitListItem = olvPointListTree.SelectedObject as LinearUnitListItem;

                tbxLinearUnitName.Text = linearUnitListItem.UnitName;
                tbxToMeterFactor.Text = linearUnitListItem.MetersPerUnit;
                tbxLinearUnitAbb.Text = linearUnitListItem.Abbreviation;
                tbxAlias.Text = linearUnitListItem.Alias;
                tbxRemarks.Text = linearUnitListItem.Remarks;

                linearUnit = new LinearUnit(double.Parse(linearUnitListItem.MetersPerUnit), linearUnitListItem.UnitName, linearUnitListItem.Authority, 
                    long.Parse(linearUnitListItem.AuthorityCode), linearUnitListItem.Alias, linearUnitListItem.Abbreviation, linearUnitListItem.Remarks);
            }
            catch (Exception)
            {
            }
           
        }
    }
}
