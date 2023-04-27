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
    public partial class GlobalSettings : UserControl
    {
        public GlobalSettings()
        {
            InitializeComponent();
        }

        private void btnGeneral_Click(object sender, EventArgs e)
        {
            foreach (Button btn in pnlButtons.Controls)
            {
                btn.BackColor = Color.FromArgb(90, 139, 196);
                btn.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            }
           ((Button)sender).BackColor = Color.FromArgb(80, 139, 218);
           ((Button)sender).Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {

            //Show Settings button
            MainScreen.btnSettings.Show();
            this.SendToBack();

        }


        #region Button Effect
        /// <summary>
        /// Add check effect to the button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void buttonEffect(object sender, EventArgs e)
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                {
                    if (control is Button)
                    {
                        if ((control as Button) == (Button)sender)
                        {
                            if (!(control as Button).Name.StartsWith("btnSub") && (control as Button).Parent.Parent == pnlButtons)
                            {
                                (control as Button).BackColor = Color.FromArgb(100, 160, 199);
                            }
                            else if ((control as Button).Name.StartsWith("btnSub") && (control as Button).Parent == pnlCoordinateSystem)
                            {
                                btnCoordinateSystem.BackColor = Color.FromArgb(100, 160, 199);
                                (control as Button).BackColor = Color.FromArgb(83, 127, 183);
                            }

                        }
                        else
                        {
                            if (!(control as Button).Name.StartsWith("btnSub") && (control as Button).Parent.Parent == pnlButtons)
                            {
                                (control as Button).BackColor = Color.FromArgb(90, 139, 196);
                            }
                            else if ((control as Button).Name.StartsWith("btnSub") && (control as Button).Parent == pnlCoordinateSystem)
                            {

                                (control as Button).BackColor = Color.FromArgb(72, 115, 164);
                            }

                        }
                    }
                    else
                    {
                        func(control.Controls);
                    }

                }
            };

            func(Controls);

        }
        #endregion

        private void btnCoordinateSystem_Click(object sender, EventArgs e)
        {
            buttonEffect(sender, e);
            GNSS_Functions.expandEffect(pnlCoordinateSystem, 215);
        }

        private void btnGeneralSettings_Click(object sender, EventArgs e)
        {
            buttonEffect(sender, e);
        }       

        private void btnGNSS_Settings_Click(object sender, EventArgs e)
        {
            buttonEffect(sender, e);
        }

        private void btnCadastral_Click(object sender, EventArgs e)
        {
            buttonEffect(sender, e);
        }
    }
}
