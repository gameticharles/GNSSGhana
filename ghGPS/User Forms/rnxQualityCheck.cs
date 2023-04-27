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
using System.IO;
using ghGPS.Forms;

namespace ghGPS.User_Forms
{
    public partial class rnxQualityCheck : UserControl
    {
        public rnxQualityCheck()
        {
            InitializeComponent();

            cbxReportType.SelectedIndex = 0;

            
            
        }

        
        public string ReportType = "";

        private async void btnExecuteQualityCheck_Click(object sender, EventArgs e)
        {
            using (var waiting = new WaitLoadingForm())
            {
                waiting.OpenedForm = "Quality Check";
                waiting.ShowDialog();                
            }                    

        }

        private void btnExportResult_Click(object sender, EventArgs e)
        {
            //rtbxQualityCheck.SaveDocumentAs();//
            using (var ofd = new openFileBrowser(true))
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show(ofd.shellView.CurrentFolder.FileSystemPath); 
                }
            }
        }
    }
}
