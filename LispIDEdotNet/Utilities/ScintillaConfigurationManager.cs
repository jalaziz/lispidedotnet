using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScintillaNet.Configuration;

namespace LispIDEdotNet.Utilities
{
    class ScintillaConfigurationManager
    {
        #region Fields

        private const string LISP_CONFIG = "lisp.xml";
        private const string PIPE_CONFIG = "lispPipe.xml";
        private const string CONFIG_DOCUMENT = "config.xml";

        private readonly string CONFIG_FOLDER;
        private readonly string LISP_CONFIG_PATH;
        private readonly string LISP_PIPE_CONFIG_PATH;
        private readonly string CONFIG_DOCUMENT_PATH;

        private string lispConfigPath;
        private string lispPipeConfigPath;
        private string configDocPath;

        private ScintillaNet.Configuration.Configuration scintillaConfiguration;
        private ScintillaNet.Configuration.Configuration pipeScintillaConfiguration;

        #endregion Fields

        #region Properties

        public string LispConfigPath
        {
            get
            {
                return this.lispConfigPath ?? this.LISP_CONFIG_PATH;
            }
            set
            {
                this.lispConfigPath = value;
                LoadScintillaConfiguration();
            }
        }

        public string LispPipeConfigPath
        {
            get
            {
                return this.lispPipeConfigPath ?? this.LISP_PIPE_CONFIG_PATH;
            }
            set
            {
                this.lispPipeConfigPath = value;
                LoadPipeScintillaConfiguration();
            }
        }

        public string ConfigDocumentPath
        {
            get
            {
                return this.configDocPath ?? this.CONFIG_DOCUMENT_PATH;
            }
            set
            {
                this.configDocPath = value;
            }
        }

        public ScintillaNet.Configuration.Configuration ScintillaConfiguration
        {
            get
            {
                return this.scintillaConfiguration;
            }
        }

        public ScintillaNet.Configuration.Configuration PipeScintillaConfiguration
        {
            get
            {
                return this.pipeScintillaConfiguration;
            }
        }

        #endregion Properties

        #region Constructors

        public ScintillaConfigurationManager()
        {
            CONFIG_FOLDER = Path.Combine(Application.StartupPath, "config");
            LISP_CONFIG_PATH = Path.Combine(CONFIG_FOLDER, LISP_CONFIG);
            LISP_PIPE_CONFIG_PATH = Path.Combine(CONFIG_FOLDER, PIPE_CONFIG);
            CONFIG_DOCUMENT_PATH = Path.Combine(CONFIG_FOLDER, CONFIG_DOCUMENT);

            LoadScintillaConfiguration();
            LoadPipeScintillaConfiguration();

            foreach (StyleConfig style in this.scintillaConfiguration.Styles)
            {
                if (style.FontName == null)
                {
                    style.FontName = FontFamily.GenericMonospace.Name;
                    style.Size = 11;
                }
            }

            foreach (StyleConfig style in this.pipeScintillaConfiguration.Styles)
            {
                if (style.FontName == null)
                {
                    style.FontName = FontFamily.GenericMonospace.Name;
                    style.Size = 11;
                }
            }
        }

        #endregion Constructors

        #region Methods

        private void LoadScintillaConfiguration()
        {
            this.scintillaConfiguration = new ScintillaNet.Configuration.Configuration(this.LispConfigPath, "lisp", true);
        }

        private void LoadPipeScintillaConfiguration()
        {
            this.pipeScintillaConfiguration = new ScintillaNet.Configuration.Configuration(this.LispPipeConfigPath, "lisp", true);
        }

        #endregion Methods
    }
}