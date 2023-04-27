using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;

namespace GNSSUpdate
{
    /// <summary>
    /// Contains update informations
    /// </summary>
    internal class GNSSUpdateXml
    {
        private Version version;
        private Uri uri;
        private string fileName;
        private string md5;
        private string description;
        private string launchArgs;

        /// <summary>
        /// The update version number
        /// </summary>
        internal Version Version
        {
            get { return this.version; }
        }

        /// <summary>
        /// The location of the update binary
        /// </summary>
        internal Uri Uri
        {
            get { return this.uri; }
        }

        /// <summary>
        /// The file name of the binary
        /// for use on local computer
        /// </summary>
        internal string FileName
        {
            get { return this.fileName; }
        }

        /// <summary>
        /// The MD5 of the update's binary
        /// </summary>
        internal string MD5
        {
            get { return this.md5; }
        }

        /// <summary>
        /// The Updates description
        /// </summary>
        internal string Description
        {
            get { return this.description; }
        }

        /// <summary>
        /// The Arguments to pass to the updated application on startup
        /// </summary>
        internal string LaunchArgs
        {
            get { return this.launchArgs; }
        }



        /// <summary>
        /// Create a new ReClipUpdateXml object
        /// </summary>
        /// <param name="version"></param>
        /// <param name="uri"></param>
        /// <param name="fileName"></param>
        /// <param name="md5"></param>
        /// <param name="description"></param>
        /// <param name="launchArgs"></param>
        internal GNSSUpdateXml(Version version, Uri uri, string fileName, string md5, string description, string launchArgs)
        {
            this.version = version;
            this.uri = uri;
            this.fileName = fileName;
            this.md5 = md5;
            this.description = description;
            this.launchArgs = launchArgs;
        }

        /// <summary>
        /// Check if update's version is newer than the old version
        /// </summary>
        /// <param name="version">Application's current version</param>
        /// <returns></returns>
        internal bool IsNewerThan(Version version)
        {
            return this.version > version;
        }


        /// <summary>
        /// Check the Uri to make sure file exist
        /// </summary>
        /// <param name="location">The Uri of the update.xml</param>
        /// <returns></returns>
        internal static bool ExistOnServer(Uri location)
        {
            try
            {

                //Request the update.xml
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(location.AbsoluteUri);
                //Read the response
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                resp.Close();

                return resp.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception)
            {

                return false;
            }
        }

        /// <summary>
        /// Parses the update.xml into ReClipUpdateXml object
        /// </summary>
        /// <param name="location">Uri of the update.xml on the server></param>
        /// <param name="appID">The application's ID</param>
        /// <returns>The ReClipUpdateXml object with the data, or null of any errors</returns>
        internal static GNSSUpdateXml Parse(Uri location, string appID)
        {
            Version version = null;
            string url = "", fileName = "", md5 = "", description = "", launchArgs = "";

            try
            {
                //Load the document
                XmlDocument doc = new XmlDocument();
                doc.Load(location.AbsoluteUri);

                //Get the appID's node with the update info
                //This allows you to store all program's update node in one file
                XmlNode node = doc.DocumentElement.SelectSingleNode("//update[@appId='" + appID + "']");

                //If the node doesn't exist, there is no update
                if (node == null)
                {
                    return null;
                }

                //Parse data
                version = Version.Parse(node["version"].InnerText);
                url = node["url"].InnerText;
                fileName = node["fileName"].InnerText;
                md5 = node["md5"].InnerText;
                description = node["description"].InnerText;
                launchArgs = node["launchArgs"].InnerText;

                return new GNSSUpdateXml(version, new Uri(url), fileName, md5, description, launchArgs);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
