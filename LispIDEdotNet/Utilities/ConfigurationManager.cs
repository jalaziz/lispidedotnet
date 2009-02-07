using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LispIDEdotNet.Utilities.Configuration;

namespace LispIDEdotNet.Utilities
{
    class ConfigurationManager
    {
        #region Fields

        private static readonly LispIDEConfigSection configSection;

        #endregion Fields

        #region Constructors

        private ConfigurationManager()
        { }

        static ConfigurationManager()
        {
            configSection = LispIDEConfigSection.GetSection(ConfigurationUserLevel.PerUserRoamingAndLocal);
        }

        #endregion Constructors

        #region Properties

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

        #endregion Properties

        #region Methods

        public static void Save()
        {
            configSection.Save();
        }

        public static void SaveWindowState(Form window)
        {
            FormWindowState windowState = window.WindowState;

            // We want to save the normal window bounds
            window.WindowState = FormWindowState.Normal;

            Properties.Settings.Default.WindowState = windowState;
            Properties.Settings.Default.WindowBounds = window.DesktopBounds;

            window.WindowState = windowState;
            
            Properties.Settings.Default.Save();
        }

        public static void LoadWindowState(Form window)
        {
            FormWindowState windowState = Properties.Settings.Default.WindowState;
            Rectangle windowBounds = Properties.Settings.Default.WindowBounds;

            if(windowState != FormWindowState.Minimized && windowBounds.Width > 0 && windowBounds.Height > 0)
            {
                Rectangle screenBounds = Screen.GetBounds(windowBounds);
                if (windowBounds.X > screenBounds.Right)
                {
                    windowBounds.X = screenBounds.X;
                    windowBounds.Y = screenBounds.Y;
                }

                window.StartPosition = FormStartPosition.Manual;
                window.DesktopBounds = windowBounds;
                window.WindowState = windowState;
            }
        }

        #endregion Methods
    }
}
