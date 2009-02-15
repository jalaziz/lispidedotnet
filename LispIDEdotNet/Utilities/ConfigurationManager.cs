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

        public static GeneralSettings General
        {
            get
            {
                return configSection.General;
            }
        }

        public static ScintillaSettings Scintilla
        {
            get
            {
                return configSection.Scintilla;
            }
        }

        public static PipeSettings Pipe
        {
            get
            {
                return configSection.Pipe;
            }
        }

        public static string LispPath
        {
            get
            {
                return Pipe.LispPath;
            }
            set
            {
                Pipe.LispPath = value;
                Save();
            }
        }

        public static Font Font
        {
            get
            {
                return Scintilla.Font;
            }
            set
            {
                Scintilla.Font = value;
                Save();
            }
        }

        public static PipeType PipeType
        {
            get
            {
                return Pipe.PipeType;
            }
            set
            {
                Pipe.PipeType = value;
                Save();
            }
        }

        public static bool ShowToolbar
        {
            get
            {
                return General.ShowToolbar;
            }
            set
            {
                General.ShowToolbar = value;
                Save();
            }
        }

        public static bool ShowStatusbar
        {
            get
            {
                return General.ShowStatusbar;
            }
            set
            {
                General.ShowStatusbar = value;
                Save();
            }
        }

        public static bool ShowWhitespace
        {
            get
            {
                return Scintilla.ShowWhitespace;
            }
            set
            {
                Scintilla.ShowWhitespace = value;
                Save();
            }
        }

        public static bool ShowEOL
        {
            get
            {
                return Scintilla.ShowEOL;
            }
            set
            {
                Scintilla.ShowEOL = value;
                Save();
            }
        }

        public static bool ShowLineNumbers
        {
            get
            {
                return Scintilla.ShowLineNumbers;
            }
            set
            {
                Scintilla.ShowLineNumbers = value;
                Save();
            }
        }

        public static bool EnableWordWrap
        {
            get
            {
                return Scintilla.EnableWordWrap;
            }
            set
            {
                Scintilla.EnableWordWrap = value;
                Save();
            }
        }

        public static bool EnableIndentGuides
        {
            get
            {
                return Scintilla.EnableIndentGuides;
            }
            set
            {
                Scintilla.EnableIndentGuides = value;
                Save();
            }
        }

        public static bool EnableFolding
        {
            get
            {
                return Scintilla.EnableFolding;
            }
            set
            {
                Scintilla.EnableFolding = value;
                Save();
            }
        }

        public static bool EnableAutocomplete
        {
            get
            {
                return Scintilla.EnableAutocomplete;
            }
            set
            {
                Scintilla.EnableAutocomplete = value;
                Save();
            }
        }

        public static FormWindowState WindowState
        {
            get
            {
                return General.WindowState;
            }
            set
            {
                General.WindowState = value;
                Save();
            }
        }

        public static Rectangle WindowBounds
        {
            get
            {
                return General.WindowBounds;
            }
            set
            {
                General.WindowBounds = value;
                Save();
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

            General.WindowState = windowState;
            General.WindowBounds = window.DesktopBounds;

            window.WindowState = windowState;
            
            Save();
        }

        public static void LoadWindowState(Form window)
        {
            FormWindowState windowState = WindowState;
            Rectangle windowBounds = WindowBounds;

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
