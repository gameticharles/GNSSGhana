using ghGPS.Classes;
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
    public partial class TransParamLists : MetroSuite.MetroForm
    {
        public TransParamLists()
        {
            InitializeComponent();

            SetupColumns();
        }

        private void SetupColumns()
        {

            this.clnTransName.AspectGetter = delegate (object x) { return ((TransParamListItem)x).TransParamName; ; };
            this.clnDx.AspectGetter = delegate (object x) { return ((TransParamListItem)x).Dx; ; };
            this.clnDy.AspectGetter = delegate (object x) { return ((TransParamListItem)x).Dy; ; };
            this.clnDz.AspectGetter = delegate (object x) { return ((TransParamListItem)x).Dz; ; };
            this.clnRx.AspectGetter = delegate (object x) { return ((TransParamListItem)x).Rx; ; };
            this.clnRy.AspectGetter = delegate (object x) { return ((TransParamListItem)x).Ry; ; };
            this.clnRz.AspectGetter = delegate (object x) { return ((TransParamListItem)x).Rz; ; };
            this.clnScale.AspectGetter = delegate (object x) { return ((TransParamListItem)x).Ppm; ; };
            this.clnXm.AspectGetter = delegate (object x) { return ((TransParamListItem)x).Xm; ; };
            this.clnYm.AspectGetter = delegate (object x) { return ((TransParamListItem)x).Ym; ; };
            this.clnZm.AspectGetter = delegate (object x) { return ((TransParamListItem)x).Zm; ; };

        }

        private void olvPointListTree_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                var transParmListItem = olvPointListTree.SelectedObject as TransParamListItem;

                tbxTransParamName.Text = transParmListItem.TransParamName;
                tbxDX.Text = transParmListItem.Dx;
                tbxDY.Text = transParmListItem.Dy;
                tbxDZ.Text = transParmListItem.Dz;
                tbxRX.Text = transParmListItem.Rx;
                tbxRY.Text = transParmListItem.Ry;
                tbxRZ.Text = transParmListItem.Rz;
                tbxScale.Text = transParmListItem.Ppm;
                tbxXm.Text = transParmListItem.Xm;
                tbxYm.Text = transParmListItem.Ym;
                tbxZm.Text = transParmListItem.Zm;

            }
            catch (Exception)
            {
            }
        }
    }
}
