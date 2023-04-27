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
    public partial class TheEllipsoids : MetroSuite.MetroForm
    {
       

        public TheEllipsoids()
        {
            InitializeComponent();

            SetupColumns();
        }

        //Make sure that on click you set the values
        private void SetupColumns()
        {
            
            this.clnEllipsoidName.AspectGetter = delegate (object x) { return ((EllipsoidListItem)x).EllipsoidName; ; };
            this.clnSemiMajorAxis.AspectGetter = delegate (object x) { return ((EllipsoidListItem)x).SemiMajorAxis; ; };
            this.clnInverseFlattening.AspectGetter = delegate (object x) { return ((EllipsoidListItem)x).InverseFlattening; ; };
            
        }

        private void olvPointListTree1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (olvPointListTree.SelectedIndex > -1)
            {                
                tbxEllipsoidName.Text = olvPointListTree.SelectedItem.GetSubItem(0).Text;        //Get Name
                tbxSemi_Major_Axis.Text = olvPointListTree.SelectedItem.GetSubItem(1).Text;      //Get Semi-major axis
                tbxInverseFlattening.Text = olvPointListTree.SelectedItem.GetSubItem(2).Text;    //Get Inverse flattening                
            }
            
        }
    }

   

}
