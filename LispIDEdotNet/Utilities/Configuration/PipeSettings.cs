using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LispIDEdotNet.Utilities.Configuration
{
    class PipeSettings : ConfigurationElement
    {
         #region Static Fields

        private static ConfigurationProperty lispPathProperty;
        private static ConfigurationProperty pipeTypeProperty;

        private static ConfigurationPropertyCollection properties;

        #endregion Static Fields

        #region Properties

        [ConfigurationProperty("LispPath", DefaultValue = "", IsRequired = true)]
        public string LispPath
        {
            get { return (string)base[lispPathProperty]; }
            set { base[lispPathProperty] = value; }
        }

        [ConfigurationProperty("PipeType", DefaultValue = PipeType.Integrated, IsRequired = true)]
        public PipeType PipeType
        {
            get { return (PipeType)base[pipeTypeProperty]; }
            set { base[pipeTypeProperty] = value; }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return properties; }
        }

        #endregion Properties

        #region Constructors

        static PipeSettings()
        {
            lispPathProperty = new ConfigurationProperty("LispPath", typeof(string),
                                                            String.Empty,
                                                            ConfigurationPropertyOptions.IsRequired);
            pipeTypeProperty = new ConfigurationProperty("PipeType", typeof(PipeType),
                                                             PipeType.Integrated,
                                                             ConfigurationPropertyOptions.IsRequired);

            properties = new ConfigurationPropertyCollection();
            properties.Add(lispPathProperty);
            properties.Add(pipeTypeProperty);
        }

        #endregion Constructors
    }
}
