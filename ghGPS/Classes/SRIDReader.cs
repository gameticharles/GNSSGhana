using System.Collections.Generic;
using System.IO;


namespace ghGPS.Classes
{
    internal class SRIDReader
    {
        private const string filename = @"..\..\Classes\SRID.csv";

        public class WKTstring
        {
            /// <summary>
            /// Well-known ID
            /// </summary>
            public string WKID;

            /// <summary>
            /// Well-Known Name
            /// </summary>
            public string WKName;

            /// <summary>
            /// Type of CRS
            /// </summary>
            public string WKType;
            /// <summary>
            /// Well-known Text
            /// </summary>
            public string WKT;

            /// <summary>
            /// 
            /// </summary>
            public string WKProjType;
        }
                

        /// <summary>
        /// Enumerates all SRID's in the SRID.csv file.
        /// </summary>
        /// <returns>Enumerator</returns>
        public static IEnumerable<WKTstring> GetSRIDs()
        {
            using (StreamReader sr = File.OpenText(filename))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    int split = line.IndexOf(';');
                    if (split > -1)
                    {
                        WKTstring wkt = new WKTstring();
                        wkt.WKID = "EPSG:"+ line.Substring(0, split);                         
                        wkt.WKT = line.Substring(split + 1);
                        wkt.WKType = wkt.WKT.Substring(0, 6);
                        
                        if (wkt.WKType.Contains("PROJCS"))
                        {
                            wkt.WKName = GNSS_Functions.GetUntilOrEmpty(wkt.WKT.Substring(8), "\",GEOGCS").Trim();
                            wkt.WKProjType = GNSS_Functions.GetUntilOrEmpty(wkt.WKT.Substring(wkt.WKT.IndexOf("PROJECTION[") + 11), "\"],PARAMETER").Trim(); ;
                        }
                        else
                        {
                            wkt.WKName = GNSS_Functions.GetUntilOrEmpty(wkt.WKT.Substring(8), "\",DATUM").Trim();
                            wkt.WKProjType = "";
                        }

                        yield return wkt;
                    }
                }
                sr.Close();
            }
        }
        /// <summary>
        /// Gets a coordinate system from the SRID.csv file
        /// </summary>
        /// <param name="id">EPSG ID</param>
        /// <returns>Coordinate system, or null if SRID was not found.</returns>
        public static string GetCSbyID(IEnumerable<WKTstring> SRIDS, int id)
        {
            
            foreach (WKTstring wkt in SRIDS)
            {
                
                if (int.Parse(wkt.WKID.Substring(5)) == id)
                {
                    
                    return wkt.WKT;
                }
            }
            return null;
        }
    }
}
