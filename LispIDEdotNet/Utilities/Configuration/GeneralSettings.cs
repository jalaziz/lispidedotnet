using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Rectangle=System.Drawing.Rectangle;

namespace LispIDEdotNet.Utilities.Configuration
{
    class GeneralSettings : ConfigurationElement
    {
        #region Static Fields

        private static ConfigurationProperty windowStateProperty;
        private static ConfigurationProperty windowBoundsProperty;
        private static ConfigurationProperty showToolbarProperty;
        private static ConfigurationProperty showStatusbarProperty;

        private static ConfigurationPropertyCollection properties;

        #endregion Static Fields

        #region Properties

        [ConfigurationProperty("WindowState", DefaultValue = FormWindowState.Normal)]
        public FormWindowState WindowState
        {
            get { return (FormWindowState)base[windowStateProperty]; }
            set { base[windowStateProperty] = value; }
        }

        [ConfigurationProperty("WindowBounds")]
        public Rectangle WindowBounds
        {
            get { return (Rectangle)base[windowBoundsProperty]; }
            set { base[windowBoundsProperty] = value; }
        }

        [ConfigurationProperty("ShowToolbar", DefaultValue = true)]
        public bool ShowToolbar
        {
            get { return (bool)base[showToolbarProperty]; }
            set { base[showToolbarProperty] = value; }
        }

        [ConfigurationProperty("ShowStatusbar", DefaultValue = true)]
        public bool ShowStatusbar
        {
            get { return (bool)base[showStatusbarProperty]; }
            set { base[showStatusbarProperty] = value; }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return properties; }
        }

        #endregion Properties

        #region Constructors

        static GeneralSettings()
        {
            windowStateProperty = new ConfigurationProperty("WindowState", typeof(FormWindowState),
                                                            FormWindowState.Normal);
            windowBoundsProperty = new ConfigurationProperty("WindowBounds", typeof(Rectangle),
                                                             Rectangle.Empty);
            showToolbarProperty = new ConfigurationProperty("ShowToolbar", typeof(bool), true);
            showStatusbarProperty = new ConfigurationProperty("ShowStatusbar", typeof(bool), true);

            properties = new ConfigurationPropertyCollection();
            properties.Add(windowStateProperty);
            properties.Add(windowBoundsProperty);
            properties.Add(showToolbarProperty);
            properties.Add(showStatusbarProperty);
        }

        #endregion Constructors
    }
}
