using ghGPS.Classes;
using ghGPS.Properties;
using GongSolutions.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ghGPS.Forms
{
    public partial class openFileBrowser : MetroSuite.MetroForm
    {
        private bool IsFolderBrowser = false;

        public string SelectedFolder = "";

        public openFileBrowser(bool IsFolderBrowser = false)
        {
            InitializeComponent();

            this.IsFolderBrowser = IsFolderBrowser;

            try
            {
                shellView.CurrentFolder = new ShellItem(Settings.Default.CurrentFolder);
            }
            catch (Exception)
            {
                shellView.CurrentFolder = new ShellItem("shell:///Desktop");
            }
            
            shellView.History.Clear();
        }

        ShellContextMenu m_ContextMenu; 
        protected override void WndProc(ref Message m)
        {
            if ((m_ContextMenu == null) || (!m_ContextMenu.HandleMenuMessage(ref m)))
            {
                base.WndProc(ref m);
            }
        }

        #region "Form Effects"
        bool drag;
        int posX;
        int posY;
        private void MainScreen_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    drag = true;
                    posX = Cursor.Position.X - this.Left;
                    posY = Cursor.Position.Y - this.Top;
                }
            }
        }

        private void MainScreen_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (drag == true)
                {
                    this.Opacity = 1;
                    this.Top = Cursor.Position.Y - posY;
                    this.Left = Cursor.Position.X - posX;

                }
            }

        }

        private void MainScreen_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                drag = false;
                this.Opacity = 1;
            }
        }


        #endregion

        private static string locationURL = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

        /// <summary>
        /// The selected item
        /// </summary>
        /// <param name="filename"></param>
        void OnFileSelected(ShellItem filename)
        {            
          
            if (!shellView.NavigateSelectedFolder())
            {
                ShellItem[] selected = shellView.SelectedItems;

                if (selected.Length > 0)
                {
                    GNSS_Functions.SelectedItems = selected;
                }
                else if (File.Exists(fileNameCombo.Text))
                {
                    OnFileSelected(new ShellItem(fileNameCombo.Text));
                }

                SelectedFolder = shellView.CurrentFolder.FileSystemPath;
            }
            else
            {
                SelectedFolder = shellView.CurrentFolder.FileSystemPath;
            }

            DialogResult = DialogResult.OK;
            
        }

        void UpdateOpenButtonState()
        {
            if (IsFolderBrowser && shellView.CurrentFolder.IsFolder)
            {               
                btnOpen.Enabled = shellView.CurrentFolder.IsFolder;
            }
            else
            {
                btnOpen.Enabled = (shellView.SelectedItems.Length > 0) ||
                                 (fileNameCombo.Text.Length > 0);
            }
            
        }

        /// <summary>
        /// Occurs when an item is double clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void shellView_DoubleClick(object sender, EventArgs e)
        {
            OnFileSelected(shellView.SelectedItems[0]);

            this.Close();
        }

        string itemsContained = null;
        void fileNameCombo_TextChanged(object sender, EventArgs e)
        {
            UpdateOpenButtonState();
        }

        void shellView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateOpenButtonState();

            string numSelectedItems = null;

            try
            {
                if (shellView.SelectedItems.Count() > 8) return;

                if (shellView.SelectedItems.Count() == 0)
                {
                    numSelectedItems = "";
                }
                else if (shellView.SelectedItems.Count() == 1)
                {
                    numSelectedItems = shellView.SelectedItems.Length.ToString() + " item selected";
                }
                else
                {
                    numSelectedItems = shellView.SelectedItems.Length.ToString() + " items selected";
                }

                lblItems.Text = itemsContained + numSelectedItems;

            }
            catch (Exception)
            {
                             
            }
        }

        void fileNameCombo_FilenameEntered(object sender, EventArgs e)
        {
            OnFileSelected(new ShellItem(fileNameCombo.Text));
        }

        void FileDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.CurrentFolder = shellView.CurrentFolder.ToString();
            Properties.Settings.Default.Save();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (shellView.CanNavigateBack)
            {
                shellView.NavigateBack();
            }
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            if (shellView.CanNavigateForward)
            {
                shellView.NavigateForward();
            }
        }

        bool IsSpecialFolder = true;
        private void btnDesktop_Click(object sender, EventArgs e)
        {
            IsSpecialFolder = true;
            try
            {
                shellView.View = ShellViewStyle.Details;

                ShellItem folder;

                //Get Location URL
                if (((Button)sender).Name == nameof(btnDesktop))
                {
                    shellView.View = ShellViewStyle.SmallIcon;

                    //Get Desktop Location
                    folder = new ShellItem(KnownFolders.GetPath(SpecialKnownFolder.Desktop));
                    
                }
                else if (((Button)sender).Name == nameof(btnComputer))
                {
                    shellView.View = ShellViewStyle.Tile;

                    //Get My Computer Location
                    folder = new ShellItem(Environment.SpecialFolder.MyComputer);
                }
                else if (((Button)sender).Name == nameof(btnMyDocument))
                {
                    //Get MyDocument Location
                    folder = new ShellItem(KnownFolders.GetPath(SpecialKnownFolder.Documents));                    
                }
                else if (((Button)sender).Name == nameof(btnDownloads))
                {
                    shellView.View = ShellViewStyle.SmallIcon;

                    //Get MyDocument Location
                    folder = new ShellItem(KnownFolders.GetPath(SpecialKnownFolder.Downloads));
                }
                else if (((Button)sender).Name == nameof(btnRecentFolder))
                {
                    shellView.View = ShellViewStyle.Tile;

                    //Get Recent folder Location
                    folder = new ShellItem(KnownFolders.GetPath(SpecialKnownFolder.Recent));
                }
                else if (((Button)sender).Name == nameof(btnNetwork))
                {
                    shellView.View = ShellViewStyle.Tile;

                    //Get Network Location                  
                    folder = new ShellItem(KnownFolders.GetPath(SpecialKnownFolder.NetHood));
                }
                else if (((Button)sender).Name == nameof(btnCustomFolder))
                {
                    shellView.View = ShellViewStyle.SmallIcon;

                    //Get User Defined Location
                    folder = new ShellItem(Settings.Default.customPath);
                }                
                else
                {
                    //Get My Computer Location
                    folder = new ShellItem(KnownFolders.GetPath(SpecialKnownFolder.Documents));

                }
                
                shellView.Navigate(folder);
                
            }
            catch ( Exception )
            {
                // set the default directory path
                //locationURL = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString();
                //webBrowser1.Url = new Uri(locationURL);
            }
                        

        }

        private void openFileBrowser_Load(object sender, EventArgs e)
        {            

            //Check if previously the user saved a path
            if (Settings.Default.customPath.Length == 0)
            {
                //Show the custom folder
                pnlCustomFolder.Visible = false;
                btnCustomFolder.Text = "    User Location      ";
                btnAddCustomFolder.Show();
            }
            else
            {
                string myNewString = "    " + Settings.Default.customFolderName;
                                                       

                //Show the custom folder 
                if (myNewString.Length > 17)
                {
                    myNewString = myNewString.Remove(17) + "...  ";
                }
                else
                {
                    myNewString = myNewString + new String(' ', 23 - myNewString.Length); ;
                }

                btnCustomFolder.Text = myNewString;
                pnlCustomFolder.Visible = true;

                btnAddCustomFolder.Hide();
            }
                       

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
       
        private void btnLocal_Click(object sender, EventArgs e)
        {            
            expandEffect(pnlLocal, 288);
        }

        private void expandEffect(Panel pnl, int StopAt)
        {

            if (pnl.Height > 48)
            {
                StopAt = 46;
            }

            pnl.Height = StopAt;

        }

        private void btnCloud_Click(object sender, EventArgs e)
        {
            expandEffect(pnlCloud, 86);
        }

        private void btnAddCustomFolder_Click(object sender, EventArgs e)
        {
            string myNewString = null;

            //Open the folder browser
            using (FolderBrowserDialog fbd = new FolderBrowserDialog() { Description = "Select your path"})
            {
                if (fbd.ShowDialog() ==  DialogResult.OK)
                {
                    
                    if (new ShellItem(fbd.SelectedPath).DisplayName.Split(' ').Length > 1 )
                    {
                        myNewString = "    " + GNSS_Functions.CapitalizeSentence(new ShellItem(fbd.SelectedPath).DisplayName);
                    }
                    else
                    {
                        myNewString = "    " + new ShellItem(fbd.SelectedPath).DisplayName;
                    }

                    
                    //Show the custom folder 
                    if (myNewString.Length > 17)
                    {
                        myNewString = myNewString.Remove(17) + "...  ";
                    }
                    else
                    {
                        myNewString = myNewString + new String(' ', 23 - myNewString.Length); ;
                    }

                    btnCustomFolder.Text = myNewString;

                    Settings.Default.customFolderName = new ShellItem(fbd.SelectedPath).DisplayName;
                    Settings.Default.customPath = new ShellItem(fbd.SelectedPath).FileSystemPath;
                    Settings.Default.Save();
                }
                else
                {
                    btnCustomFolder.Text = "    User Location      ";
                }

                MessageBox.Show(myNewString.Length.ToString());
            }

            //Show the custom folder
            pnlCustomFolder.Visible = true;

            // Set the name if the its OK

            btnAddCustomFolder.Hide();


        }
        
        private void btnMenuEdit_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(btnCustomFolder,180,25);
        }

        /// <summary>
        /// Count the number of Items in the current Folder
        /// </summary>
        private void CountItemsCurrent()
        {
            string numItems = null;

            if (shellView.CurrentFolder.Count() == 1)
            {
                numItems = " item ";
            }
            else
            {
                numItems = " items ";
            }

            itemsContained = shellView.CurrentFolder.Count().ToString() + numItems;
            lblItems.Text = itemsContained;
        }

        /// <summary>
        /// Events when the Uri is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void shellView_Navigated(object sender, EventArgs e)
        {
            // Set the URL of the browser
            lblHeaderText.Text = GNSS_Functions.OFDheaderText + shellView.CurrentFolder.DisplayName ;

            //Count the number of Items in the current Folder
            CountItemsCurrent();

            //Click button 
            if (shellView.CurrentFolder.DisplayName == (new ShellItem(KnownFolders.GetPath(SpecialKnownFolder.Desktop))).DisplayName)
            {
                IsSpecialFolder = true;
            }
            else if (shellView.CurrentFolder.DisplayName == (new ShellItem(Environment.SpecialFolder.MyComputer)).DisplayName)
            {                
                IsSpecialFolder = true;
            }
            else if (shellView.CurrentFolder.DisplayName == (new ShellItem(KnownFolders.GetPath(SpecialKnownFolder.NetHood))).DisplayName)
            {
                IsSpecialFolder = true;
            }
            else if (shellView.CurrentFolder.DisplayName == (new ShellItem(KnownFolders.GetPath(SpecialKnownFolder.Documents))).DisplayName)
            {
                IsSpecialFolder = true;
            }
            else if (shellView.CurrentFolder.DisplayName == (new ShellItem(KnownFolders.GetPath(SpecialKnownFolder.Recent))).DisplayName)
            {
                IsSpecialFolder = true;
            }
            else if (shellView.CurrentFolder.DisplayName == (new ShellItem(KnownFolders.GetPath(SpecialKnownFolder.Downloads))).DisplayName)
            {                
                IsSpecialFolder = true;
            }
            else 
            {
                IsSpecialFolder = false;
            }


            if (IsSpecialFolder)
            {
                tbxWebURL.Text = shellView.CurrentFolder.DisplayName;

                IsSpecialFolder = false;
            }
            else
            {
                tbxWebURL.Text = shellView.CurrentFolder.FileSystemPath;
                IsSpecialFolder = false;
            }


            // Check the navigation logicals
            if (shellView.CanNavigateBack)
            {
                btnBack.Enabled = true;
            }
            else
            {
                btnBack.Enabled = false;
            }

            if (shellView.CanNavigateForward)
            {
                btnForward.Enabled = true;
            }
            else
            {
                btnForward.Enabled = false;
            }

            if (shellView.CanNavigateParent)
            {
                btnUPFolder.Enabled = true;
            }
            else
            {
                btnUPFolder.Enabled = false;
            }

            if (shellView.CanCreateFolder)
            {
                btnNewFolder.Enabled = true;
            }
            else
            {
                btnNewFolder.Enabled = false;
            }

            // Set the shell view type
            switch (shellView.View)
            {
                case ShellViewStyle.LargeIcon:
                    btnViewStyle.Image = largeIconsToolStripMenuItem.Image;
                    break;
                case ShellViewStyle.SmallIcon:
                    btnViewStyle.Image = smallIconsToolStripMenuItem.Image;
                    break;
                case ShellViewStyle.List:
                    btnViewStyle.Image = ListIconsToolStripMenuItem.Image;
                    break;
                case ShellViewStyle.Details:
                    btnViewStyle.Image = DetailsIconsToolStripMenuItem.Image;
                    break;
                case ShellViewStyle.Thumbnail:
                    btnViewStyle.Image = thumnailToolStripMenuItem.Image;
                    break;
                case ShellViewStyle.Tile:
                    btnViewStyle.Image = tileToolStripMenuItem.Image;
                    break;
                case ShellViewStyle.Thumbstrip:
                    btnViewStyle.Image = thumbstripToolStripMenuItem.Image;
                    break;
                default:
                    btnViewStyle.Image = DetailsIconsToolStripMenuItem.Image;
                    break;
            }
        }
       
        private void btnUPFolder_Click(object sender, EventArgs e)
        {
            if (shellView.CanNavigateParent)
            {
                shellView.NavigateParent();
            }
        }

        private void btnNewFolder_Click(object sender, EventArgs e)
        {
            if (shellView.CanCreateFolder)
            {
                shellView.CreateNewFolder();
            }
        }

        private void btnViewStyle_Click(object sender, EventArgs e)
        {
            contextMenuStrip2.Show(btnViewStyle, 10, 23);
        }

        private void Open_Click(object sender, EventArgs e)
        {
            //if ((shellView.SelectedItems.Length > 0) && (!shellView.NavigateSelectedFolder()))
            //{
            //    MessageBox.Show(shellView.SelectedItems[0].FileSystemPath);
            //}

            if (!shellView.NavigateSelectedFolder())
            {
                ShellItem[] selected = shellView.SelectedItems;

                if (selected.Length > 0)
                {
                    GNSS_Functions.SelectedItems = selected;
                }
                else if (File.Exists(fileNameCombo.Text))
                {
                    OnFileSelected(new ShellItem(fileNameCombo.Text));
                }

                GC.Collect();
                this.Refresh();
            }

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            shellView.View = ShellViewStyle.Details;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            shellView.View = ShellViewStyle.List;
        }

        private void smallIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shellView.View = ShellViewStyle.SmallIcon;
        }

        private void largeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shellView.View = ShellViewStyle.LargeIcon;
        }

        private void tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shellView.View = ShellViewStyle.Tile;
        }

        private void thumnailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shellView.View = ShellViewStyle.Thumbnail;
        }

        private void thumbstripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shellView.View = ShellViewStyle.Thumbstrip;            
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Default.customFolderName = "";
            Settings.Default.customPath = "";
            Settings.Default.Save();

            //Show the custom folder
            pnlCustomFolder.Visible = false;
            btnCustomFolder.Text = "    User Location      ";
            btnAddCustomFolder.Show();
        }

        private void btnMenuEdit_MouseEnter(object sender, EventArgs e)
        {
            btnMenuEdit.BackColor = Color.FromArgb(100, 160, 199);
            btnCustomFolder.BackColor = Color.FromArgb(100, 160, 199);
        }

        private void btnMenuEdit_MouseLeave(object sender, EventArgs e)
        {
            btnMenuEdit.BackColor = Color.FromArgb(90, 139, 196);
            btnCustomFolder.BackColor = Color.FromArgb(90, 139, 196);
        }

        private void fileFilterComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Count the number of Items in the current Folder
            CountItemsCurrent();
        }
    }
}
