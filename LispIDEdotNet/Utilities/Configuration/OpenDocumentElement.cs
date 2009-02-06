using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;

namespace LispIDEdotNet.Utilities.Configuration
{
    class OpenDocumentElement : ConfigurationElement
    {
        #region Static Fields

        private static ConfigurationProperty nameProperty;
        private static ConfigurationProperty filePathProperty;
        private static ConfigurationProperty dockStateProperty;

        private static ConfigurationPropertyCollection properties;

        #endregion Static Fields

        #region Properties

        [ConfigurationProperty("name", DefaultValue = "", IsKey = true)]
        public string Name
        {
            get { return (string)base[nameProperty]; }
            set { base[nameProperty] = value; }
        }

        [ConfigurationProperty("dockSate", DefaultValue = DockState.Document, IsRequired = true)]
        public DockState DockState
        {
            get { return (DockState)base[dockStateProperty]; }
            set { base[dockStateProperty] = value; }
        }

        [ConfigurationProperty("filePath", DefaultValue = "", IsRequired = true)]
        public string FilePath
        {
            get { return (string)base[filePathProperty]; }
            set { base[filePathProperty] = value; }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return properties; }
        }

        #endregion Properties

        #region Constructors

        static OpenDocumentElement()
        {
            nameProperty = new ConfigurationProperty("name", typeof(string), string.Empty, ConfigurationPropertyOptions.IsKey);
            filePathProperty = new ConfigurationProperty("filePath", typeof(string), string.Empty,
                                                         ConfigurationPropertyOptions.IsRequired);
            dockStateProperty = new ConfigurationProperty("dockState", typeof(DockState), DockState.Document,
                                                          ConfigurationPropertyOptions.IsRequired);

            properties = new ConfigurationPropertyCollection();
            properties.Add(nameProperty);
            properties.Add(dockStateProperty);
            properties.Add(filePathProperty);
        }

        #endregion Constructors
    }
}
