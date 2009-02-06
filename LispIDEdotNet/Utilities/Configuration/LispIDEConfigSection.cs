using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LispIDEdotNet.Utilities.Configuration
{
    class LispIDEConfigSection : ConfigurationSection
    {
        #region Fields

        private System.Configuration.Configuration config;

        #endregion Fields

        #region Static Fields

        private static ConfigurationProperty recentFilesProperty;
        private static ConfigurationProperty openDocumentsProperty;
        private static ConfigurationProperty settingsProperty;
        private static ConfigurationProperty scintillaProperty;

        private static ConfigurationPropertyCollection properties;

        #endregion Static Fields

        #region Constructors

        public LispIDEConfigSection()
        {
            this.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
        }

        static LispIDEConfigSection()
        {
            recentFilesProperty = new ConfigurationProperty("RecentFiles", typeof(RecentFiles), null,
                                                            ConfigurationPropertyOptions.IsRequired);
            openDocumentsProperty = new ConfigurationProperty("OpenDocuments", typeof(OpenDocumentsCollection), null,
                                                              ConfigurationPropertyOptions.IsDefaultCollection);

            properties = new ConfigurationPropertyCollection();
            properties.Add(recentFilesProperty);
            properties.Add(openDocumentsProperty);
        }

        #endregion Constructors

        #region Properties

        [ConfigurationProperty("RecentFiles", IsRequired = true)]
        public RecentFiles RecentFiles
        {
            get { return (RecentFiles)base[recentFilesProperty]; }
        }

        [ConfigurationProperty("OpenDocuments", IsDefaultCollection = true)]
        public OpenDocumentsCollection OpenDocuments
        {
            get { return (OpenDocumentsCollection)base[openDocumentsProperty]; }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return properties;
            }
        }

        #endregion Properties

        #region Methods

        public void Save()
        {
            if(config != null)
                config.Save();
        }

        #endregion Methods

        #region Static Methods

        public static LispIDEConfigSection GetSection(ConfigurationUserLevel configLevel)
        {
            string appData = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData);
            string localData = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);

            ExeConfigurationFileMap exeMap = new ExeConfigurationFileMap();
            exeMap.ExeConfigFilename = Application.ExecutablePath + ".config";
            exeMap.RoamingUserConfigFilename = 
            Path.Combine(appData, @"LispIDEdotNet\user.config");
            exeMap.LocalUserConfigFilename = 
            Path.Combine(localData, @"LispIDEdotNet\user.config");

            System.Configuration.Configuration config =
                System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(exeMap,
                                                                                     configLevel);

            LispIDEConfigSection configSection = (LispIDEConfigSection)config.GetSection("LispIDEConfig");

            if (configSection == null)
            {
                configSection = new LispIDEConfigSection();
                config.Sections.Add("LispIDEConfig", configSection);
            }
            configSection.config = config;

            return configSection;
        }

        #endregion Static Methods
    }
}
