using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using LispIDEdotNet.Utilities.Configuration;

namespace LispIDEdotNet.Utilities
{
    class ConfigurationManager
    {
        private static readonly LispIDEConfigSection configSection;

        private ConfigurationManager()
        { }

        static ConfigurationManager()
        {
            configSection = LispIDEConfigSection.GetSection(ConfigurationUserLevel.PerUserRoamingAndLocal);
        }

        public static LispIDEConfigSection ConfigSection
        {
            get
            {
                return configSection;
            }
        }

        public static RecentFiles RecentFiles
        {
            get
            {
                return configSection.RecentFiles;
            }
        }

        public static OpenDocumentsCollection OpenDocuments
        {
            get
            {
                return configSection.OpenDocuments;
            }
        }

        public static void Save()
        {
            configSection.Save();
        }
    }
}
