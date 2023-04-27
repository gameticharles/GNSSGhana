using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GNSSUpdate
{
    public interface IGNSSUpdatable
    {
        /// <summary>
        /// The name of your application as you want it to displayed on the update form
        /// </summary>
        string ApplicationName { get; }

        /// <summary>
        /// An identifier string to use to identify your application in the update.xml
        /// </summary>
        string ApplicationID { get; }

        /// <summary>
        /// Get the current assembly
        /// </summary>
        Assembly ApplicationAssembly { get; }

        /// <summary>
        /// The application's icon to be displayed on the form(Top Left)
        /// </summary>
        Icon ApplicationIcon { get; }

        /// <summary>
        /// The location of the update.xml on the server
        /// </summary>
        Uri UpdateXmlLocation { get; }

        /// <summary>
        /// The Context of the program
        /// For windows form application, use 'this'
        /// For console Apps, reference System.Windows.Forms and return null
        /// </summary>
        Form Context { get; }
    }
}
