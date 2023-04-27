using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.DLL
{
    static class LoadDLL
    {
        public static void LoadDLLFiles()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string resourceName = new AssemblyName(args.Name).Name + ".dll";
                string resource = Array.Find(sender.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    byte[] assemblData = new byte[stream.Length];
                    stream.Read(assemblData, 0, assemblData.Length);

                    return Assembly.Load(assemblData);
                }
            };

        }
    }
}
