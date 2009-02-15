using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LispIDEdotNet.Utilities.Configuration
{
    class ScintillaSettings : ConfigurationElement
    {
        #region Static Fields

        private static ConfigurationProperty fontProperty;
        private static ConfigurationProperty showWhitespaceProperty;
        private static ConfigurationProperty showEOLProperty;
        private static ConfigurationProperty showLineNumbersProperty;
        private static ConfigurationProperty enableWordWrapProperty;
        private static ConfigurationProperty enableIndentGuidesProperty;
        private static ConfigurationProperty enableFoldingProperty;
        private static ConfigurationProperty enableAutocompleteProperty;

        private static ConfigurationPropertyCollection properties;

        #endregion Static Fields

        #region Properties

        [ConfigurationProperty("Font")]
        public Font Font
        {
            get { return (Font)base[fontProperty]; }
            set { base[fontProperty] = value; }
        }

        [ConfigurationProperty("ShowWhitespace", DefaultValue = false)]
        public bool ShowWhitespace
        {
            get { return (bool)base[showWhitespaceProperty]; }
            set { base[showWhitespaceProperty] = value; }
        }

        [ConfigurationProperty("ShowEOL", DefaultValue = false)]
        public bool ShowEOL
        {
            get { return (bool)base[showEOLProperty]; }
            set { base[showEOLProperty] = value; }
        }

        [ConfigurationProperty("ShowLineNumbers", DefaultValue = true)]
        public bool ShowLineNumbers
        {
            get { return (bool)base[showLineNumbersProperty]; }
            set { base[showWhitespaceProperty] = value; }
        }

        [ConfigurationProperty("EnableWordWrap", DefaultValue = false)]
        public bool EnableWordWrap
        {
            get { return (bool)base[enableWordWrapProperty]; }
            set { base[enableWordWrapProperty] = value; }
        }

        [ConfigurationProperty("EnableIndentGuides", DefaultValue = false)]
        public bool EnableIndentGuides
        {
            get { return (bool)base[enableIndentGuidesProperty]; }
            set { base[enableIndentGuidesProperty] = value; }
        }

        [ConfigurationProperty("EnableFolding", DefaultValue = true)]
        public bool EnableFolding
        {
            get { return (bool)base[enableFoldingProperty]; }
            set { base[enableFoldingProperty] = value; }
        }

        [ConfigurationProperty("EnableAutocomplete", DefaultValue = true)]
        public bool EnableAutocomplete
        {
            get { return (bool)base[enableAutocompleteProperty]; }
            set { base[enableAutocompleteProperty] = value; }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return properties; }
        }

        #endregion Properties

        #region Constructors

        static ScintillaSettings()
        {
            fontProperty = new ConfigurationProperty("Font", typeof(Font), null);
            showWhitespaceProperty = new ConfigurationProperty("ShowWhitespace", typeof(bool), false);
            showEOLProperty = new ConfigurationProperty("ShowEOL", typeof(bool), false);
            showLineNumbersProperty = new ConfigurationProperty("ShowLineNumbers", typeof(bool), true);
            enableWordWrapProperty = new ConfigurationProperty("EnableWordWrap", typeof(bool), false);
            enableIndentGuidesProperty = new ConfigurationProperty("EnableIndentGuides", typeof(bool), false);
            enableFoldingProperty = new ConfigurationProperty("EnableFolding", typeof(bool), true);
            enableAutocompleteProperty = new ConfigurationProperty("EnableAutocomplete", typeof(bool), true);

            properties = new ConfigurationPropertyCollection();
            properties.Add(fontProperty);
            properties.Add(showWhitespaceProperty);
            properties.Add(showEOLProperty);
            properties.Add(showLineNumbersProperty);
            properties.Add(enableWordWrapProperty);
            properties.Add(enableIndentGuidesProperty);
            properties.Add(enableFoldingProperty);
            properties.Add(enableAutocompleteProperty);
        }

        #endregion Constructors
    }
}
