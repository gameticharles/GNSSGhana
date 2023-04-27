using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ghGPS.Classes;
using ghGPS.Classes.CoordinateSystems;

namespace ghGPS.User_Forms
{
    public partial class WGS84_ConversionInfoItem : UserControl
    {

        public Wgs84ConversionInfo _WGS84_ConversionInfo = new Wgs84ConversionInfo();
        public WGS84_ConversionInfoItem()
        {
            InitializeComponent();

            this.Size = new Size(363, 24);
        }

        private void chbxAllZeroValues_CheckedChanged(object sender, bool isChecked)
        {
            if (chbxAllZeroValues.Checked)
            {
                this.Height = 24;
            }
            else
            {
                this.Height = 141;
            }
        }

        private void btnTransParam_Click(object sender, EventArgs e)
        {
            using (var myTransParam = new Forms.TransParamLists())
            {
                var AllTransParams = MainScreen._SQLiteHelper.GetEntries("TransParamsTable");
                var TransParamlist = new List<TransParamListItem>();
                foreach (var item in AllTransParams)
                {

                    TransParamlist.Add(new TransParamListItem(item.Text, item.SubItems[0].ToString(), item.SubItems[1].ToString(),
                        item.SubItems[2].ToString(), item.SubItems[3].ToString(), item.SubItems[4].ToString(), item.SubItems[5].ToString(),
                        item.SubItems[6].ToString(), item.SubItems[7].ToString(), item.SubItems[8].ToString(), item.SubItems[9].ToString()));
                }

                myTransParam.olvPointListTree.SetObjects(TransParamlist);

                if (myTransParam.ShowDialog() == DialogResult.OK)
                {

                    tbxDX.Text = myTransParam.tbxDX.Text;
                    tbxDY.Text = myTransParam.tbxDY.Text;
                    tbxDZ.Text = myTransParam.tbxDZ.Text;
                    tbxRX.Text = myTransParam.tbxRX.Text;
                    tbxRY.Text = myTransParam.tbxRY.Text;
                    tbxRZ.Text = myTransParam.tbxRZ.Text;
                    tbxScale.Text = myTransParam.tbxScale.Text;
                    tbxXm.Text = myTransParam.tbxXm.Text;
                    tbxYm.Text = myTransParam.tbxYm.Text;
                    tbxZm.Text = myTransParam.tbxZm.Text;                   

                }
            }
        }
    }
}
