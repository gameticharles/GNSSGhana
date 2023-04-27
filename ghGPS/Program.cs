using BrendanGrant.Helpers.FileAssociation;
using ghGPS.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace ghGPS
{
   
    static class Program
    {        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            //Set file association
            FileAssociationInfo fai = new FileAssociationInfo(".ggp"); //Set the file extension
            
                       
            if (fai.Exists)
            {
                
                //Set Broad categories of system recognized file format types.
                fai.PerceivedType = PerceivedTypes.None;

                ProgramAssociationInfo pai = new ProgramAssociationInfo(fai.ProgID);

                //Description of the file in the explorer
                pai.Description = "GNSS Ghana Project File";

                //Show Extension
                pai.AlwaysShowExtension = false;

                pai.EditFlags = EditFlags.None;

                //Set the Icon for the file extension
                ProgramIcon icon = pai.DefaultIcon;
                
            }
            else
            {
                FileAssociations.EnsureAssociationsSet();
            }

            //Embed DLL into the program and load them automatically
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            //Parse in the arguments
            if (args.Length == 0)
            {
                Application.Run(new MainScreen());
            }
            else
            {
                Application.Run(new MainScreen(args[0]));
            }

        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string resourceName = new AssemblyName(args.Name).Name + ".dll";
            string resource = Array.Find(sender.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));
            
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
            {
                byte[] assemblData = new byte[stream.Length];
                stream.Read(assemblData, 0, assemblData.Length);

                return Assembly.Load(assemblData);
            }
        }
    }
}
