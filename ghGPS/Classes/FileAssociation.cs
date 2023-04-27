using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public class FileAssociation
    {
        /// <summary>
        /// File Extension
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// Program ID
        /// </summary>
        public string ProgId { get; set; }
        /// <summary>
        /// File type description of the file
        /// </summary>
        public string FileTypeDescription { get; set; }
        /// <summary>
        /// Path to the excutable file of the program
        /// </summary>
        public string ExecutableFilePath { get; set; }
    }

    public class FileAssociations
    {
        // needed so that Explorer windows get refreshed after the registry is updated
        [System.Runtime.InteropServices.DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        private const int SHCNE_ASSOCCHANGED = 0x8000000;
        private const int SHCNF_FLUSH = 0x1000;

        /// <summary>
        /// Create and ensure association is made to the file is set
        /// </summary>
        public static void EnsureAssociationsSet()
        {
            var filePath = Process.GetCurrentProcess().MainModule.FileName;
            EnsureAssociationsSet(
                new FileAssociation
                {
                    Extension = ".ggp",
                    ProgId = "GNSS Ghana",
                    FileTypeDescription = "GNSS Ghana Processed Project File",
                    ExecutableFilePath = filePath
                }
                ,
                new FileAssociation
                {
                    Extension = ".ggc",
                    ProgId = "GNSS Ghana",
                    FileTypeDescription = "GNSS Ghana Cadastral Project File",
                    ExecutableFilePath = filePath
                }
                );
        }

        /// <summary>
        /// Parse all file associations to ensure associations are set
        /// </summary>
        /// <param name="associations"></param>
        public static void EnsureAssociationsSet(params FileAssociation[] associations)
        {
            bool madeChanges = false;
            foreach (var association in associations)
            {
                madeChanges |= SetAssociation(
                    association.Extension,
                    association.ProgId,
                    association.FileTypeDescription,
                    association.ExecutableFilePath);
            }

            if (madeChanges)
            {
                SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Set the file association
        /// </summary>
        /// <param name="extension">File extension</param>
        /// <param name="progId">Program ID</param>
        /// <param name="fileTypeDescription">Description of file type</param>
        /// <param name="applicationFilePath">Full path to application's excutable</param>
        /// <returns></returns>
        public static bool SetAssociation(string extension, string progId, string fileTypeDescription, string applicationFilePath)
        {
            bool madeChanges = false;
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + extension, progId);
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + progId, fileTypeDescription);
            madeChanges |= SetKeyDefaultValue($@"Software\Classes\{progId}\shell\open\command", "\"" + applicationFilePath + "\" \"%1\"");

            return madeChanges;
        }

        /// <summary>
        /// Set app default keys
        /// </summary>
        /// <param name="keyPath"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool SetKeyDefaultValue(string keyPath, string value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(keyPath))
            {
                if (key.GetValue(null) as string != value)
                {
                    key.SetValue(null, value);
                    return true;
                }
            }

            return false;
        }

    }
}
