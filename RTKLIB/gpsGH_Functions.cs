using GongSolutions.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS
{
    public sealed class gpsGH_Functions
    {
        /// <summary>
        /// Filter Text, Video, or all files
        /// </summary>
        public static string NormalFilter = "Text Files(*.txt)|*.txt|Video Files|*.avi, *.wmv|All Files(*.*)|*.*";

        /// <summary>
        /// Filter RINEX OBSERVATION FILES .*O, .*D, .*OBS
        /// </summary>
        public static string FilterObservaton = "RINEX O-Files|*.*O|RINEX D-Files|*.*D|RINEX OBS Files(*.obs)|*.obs";
        
        /// <summary>
        /// Filter All Navigation message files
        /// </summary>
        public static string FilterNavigation = "RINEX NAV Files|*.nav|RINEX N-Files|*.*N|RINEX P-Files|*.*P|RINEX G-Files|*.*G|RINEX H-Files|*.*H|RINEX Q-Files|*.*Q";
        
        /// <summary>
        /// Filter Precise Ephemeris Files an
        /// </summary>
        public static string FilterPreciseEphemeris = "Precise Ephemeris Files|*.eph*|SP3 Files|*.sp3|RINEX Clock files|*.clk*";

        /// <summary>
        /// Filter Ionospheric Correction Files
        /// </summary>
        public static string FilterIonosphere = "I-Files|*.*i|IONEX Files|*.ionex";

        private static ShellItem[] selectedItems;

        public static ShellItem[] SelectedItems { get => selectedItems; set => selectedItems = value; }

        public static string OFDheaderText = "Open: ";

        /// <summary>
        /// Clear Create new project entries and go back to home page
        /// </summary>
        public static void ClearCreateNewProject()
        {
            // Go back to the recent or start menu
            MainScreen.recentStartScreen.BringToFront();

            //Reset global variables
            MainScreen.createProject.tbxProjectName.Text = "";
            MainScreen.createProject.tbxFolderPath.Text = "";
            MainScreen.createProject.cbxProjectType.SelectedIndex = -1;

            MainScreen.createProject.chbxIncludeReport.Checked = false;

            //Hide the title 
            MainScreen.lblHeaderTitle.Text = "GPS Ghana";
            MainScreen.lblHeaderTitle.Hide();


            //===============GNSS IMPORT CONTROLS===================
            //Clear Import Tables
            MainScreen.GNSSDataImport.dgvRovers.Rows.Clear();
            MainScreen.GNSSDataImport.lblRoverCounts.ResetText();
            MainScreen.GNSSDataImport.tbxBasePath.ResetText();

        }

        /// <summary>
        /// Function to capitalize Words
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FirstCharToUpper(string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": return null;
                default:
                    var arr = input.ToLower().ToCharArray();
                    arr[0] = Char.ToUpperInvariant(arr[0]);
                    return new String(arr);                    
            }
        }

        /// <summary>
        /// Function to capitalize Words
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CapitalizeSentence(string input)
        {
            string myCustomFolder = null;

            var split = input.Split(' ');
            int wordIndex = 0;
            foreach (var item in split)
            {
                if (wordIndex == 0)
                {
                    myCustomFolder = FirstCharToUpper(item);
                }
                else
                {
                    myCustomFolder += " " + FirstCharToUpper(item);
                }
                wordIndex += 1;
            }

            return myCustomFolder;
        }
              
        
    }

    public static class StringExtension
    {
        public static string CapitalizeFirst(this string s)
        {
            bool IsNewSentense = true;
            var result = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                if (IsNewSentense && char.IsLetter(s[i]))
                {
                    result.Append(char.ToUpper(s[i]));
                    IsNewSentense = false;
                }
                else
                    result.Append(s[i]);

                if (s[i] == '!' || s[i] == '?' || s[i] == '.' || s[i] == '.')
                {
                    IsNewSentense = true;
                }
            }

            return result.ToString();
        }
    }    

}
