using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ghGPS.User_Forms
{
    public partial class ProjectionParameterItem : UserControl
    {
        public ProjectionParameterItem()
        {
            InitializeComponent();
        }

        private void CbxParamName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxParamName.SelectedIndex>-1)
            {
                tbxParamValue.ReadOnly = false;
            }
        }
    }
}
