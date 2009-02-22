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
        private ConfigurationUserLevel configLevel;
        private object _lock = new object();

        #endregion Fields

        #region Static Fields

        private static ConfigurationProperty recentFilesProperty;
        private static ConfigurationProperty openDocumentsProperty;
        private static ConfigurationProperty generalProperty;
        private static ConfigurationProperty scintillaProperty;
        private static ConfigurationProperty pipeProperty;

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
            generalProperty = new ConfigurationProperty("General", typeof(GeneralSettings), null,
                                                                ConfigurationPropertyOptions.IsRequired);
            scintillaProperty = new ConfigurationProperty("Scintilla", typeof(ScintillaSettings), null,
                                                                ConfigurationPropertyOptions.IsRequired);
            pipeProperty = new ConfigurationProperty("Pipe", typeof(PipeSettings), null,
                                                                ConfigurationPropertyOptions.IsRequired);

            properties = new ConfigurationPropertyCollection();
            properties.Add(recentFilesProperty);
            properties.Add(openDocumentsProperty);
            properties.Add(generalProperty);
            properties.Add(scintillaProperty);
            properties.Add(pipeProperty);
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

        [ConfigurationProperty("General", IsRequired = true)]
        public GeneralSettings General
        {
            get { return (GeneralSettings)base[generalProperty]; }
        }

        [ConfigurationProperty("Scintilla", IsRequired = true)]
        public ScintillaSettings Scintilla
        {
            get { return (ScintillaSettings)base[scintillaProperty]; }
        }

        [ConfigurationProperty("Pipe", IsRequired = true)]
        public PipeSettings Pipe
        {
            get { return (PipeSettings)base[pipeProperty]; }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return properties;
            }
        }

        public ConfigurationUserLevel ConfigLevel
        {
            get { return configLevel; }
        }

        #endregion Properties

        #region Methods

        public void Save()
        {
            InternalSave(true);
        }

        private void InternalSave(bool external)
        {
            lock (_lock)
            {
                if (config != null)
                {
                    try
                    {
                        config.Save();
                    } catch (Exception)
                    {
                        /* If the config file was externally modified, we have to load the configuration file
                         * and merge the changes. Instead of merging, we'll just concern ourselves with the last 
                         * closed instance of the application. This is not the most elegant solution
                         * but it seems to work. Unfortunately, the save method throws the same exception for 
                         * an externally modified file and an unavailable or locked file. Therefore, we must
                         * make sure we don't end up in an infinite loop. So, we use the external flag to determine
                         * if the call to save was internal (and therefore the second iteration) or external.
                         */

                        try
                        {
                            if (!external)
                                throw;

                            LispIDEConfigSection configSection = GetSection(this.ConfigLevel);

                            foreach (ConfigurationProperty property in this.Properties)
                            {
                                configSection[property.Name] = this[property];
                            }

                            configSection.InternalSave(false);

                            this.ResetModified();
                        } catch (Exception)
                        {
                            MessageBox.Show("There was an error saving the configuration file.", "Configuration Save Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
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
            configSection.configLevel = configLevel;

            return configSection;
        }

        #endregion Static Methods
    }
}
