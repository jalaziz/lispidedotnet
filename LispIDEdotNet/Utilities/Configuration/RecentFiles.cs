using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LispIDEdotNet.Utilities.Configuration
{
    class RecentFiles : ConfigurationElement
    {
        #region Static Fields

        private static ConfigurationProperty maxCountProperty;
        private static ConfigurationProperty fileListProperty;

        private static ConfigurationPropertyCollection properties;

        #endregion Static Fields

        #region Event Handlers

        public event EventHandler<RecentFileEventArgs> RecentFileChanged;
        public event EventHandler<RecentFileEventArgs> RecentFileClicked;

        #endregion Event Handlers

        #region Properties

        [ConfigurationProperty("maxCount", DefaultValue = 6, IsRequired = true)]
        [IntegerValidator(MinValue = 1, MaxValue = 50)]
        public int MaxCount
        {
            get { return (int)base[maxCountProperty]; }
            set { base[maxCountProperty] = value; }
        }

        [ConfigurationProperty("fileList", DefaultValue = "", IsRequired = true)]
        [TypeConverter(typeof(CommaDelimitedStringCollectionConverter))]
        public CommaDelimitedStringCollection FileList
        {
            get { return (CommaDelimitedStringCollection)base[fileListProperty]; }
            set { base[fileListProperty] = value; }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return properties; }
        }

        #endregion Properties

        #region Constructors

        static RecentFiles()
        {
            maxCountProperty = new ConfigurationProperty("maxCount", typeof(int), 6, null, 
                new IntegerValidator(1,50), ConfigurationPropertyOptions.IsRequired);

            fileListProperty = new ConfigurationProperty("fileList", typeof(CommaDelimitedStringCollection),
                                                         string.Empty, new CommaDelimitedStringCollectionConverter(),
                                                         null, ConfigurationPropertyOptions.IsRequired);

            properties = new ConfigurationPropertyCollection();
            properties.Add(maxCountProperty);
            properties.Add(fileListProperty);
        }

        #endregion Constructors

        #region Methods

        protected override bool IsModified()
        {
            if (FileList.IsModified)
                return true;

            return base.IsModified();
        }

        public void GenerateRecentFiles(ToolStripMenuItem recentFilesMenu)
        {
            recentFilesMenu.DropDownItems.Clear();
            recentFilesMenu.Enabled = this.FileList.Count > 0;

            foreach (string file in FileList)
            {
                AddRecentFileMenuItem(recentFilesMenu, file);
            }
        }

        private void AddRecentFileMenuItem(ToolStripDropDownItem recentFilesMenu, string filePath)
        {
            ToolStripMenuItem recentFile = new ToolStripMenuItem(filePath);
            recentFile.Click += new EventHandler(recentFile_Click);
            recentFilesMenu.DropDownItems.Add(recentFile);
        }

        public void AddRecentFile(string filePath)
        {
            FileList.Remove(filePath);

            if(FileList.Count >= MaxCount)
            {
                FileList.RemoveAt(FileList.Count - 1);
            }

            FileList.Insert(0, filePath);

            OnRecentFileChanged(filePath);
        }

        public void RemoveRecentFile(string filePath)
        {
            FileList.Remove(filePath);

            OnRecentFileChanged(filePath);
        }

        #endregion Methods

        #region Events

        private void recentFile_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;

            if (item != null)
                OnRecentFileClicked(item);
        }

        protected void OnRecentFileChanged(string file)
        {
            if(RecentFileChanged != null)
                RecentFileChanged(this, new RecentFileEventArgs(file, null));
        }

        protected void OnRecentFileClicked(ToolStripMenuItem item)
        {
            if(RecentFileClicked != null)
                RecentFileClicked(this, new RecentFileEventArgs(item.Text, item));
        }

        #endregion Events
    }

    public class RecentFileEventArgs : EventArgs
    {
        private string path;
        private ToolStripMenuItem menuItem;

        public RecentFileEventArgs(string file, ToolStripMenuItem item)
        {
            this.path = file;
            this.menuItem = item;
        }

        public string FilePath
        {
            get
            {
                return path;
            }
        }

        public ToolStripMenuItem MenuItem
        {
            get
            {
                return menuItem;
            }
        }
    }
}
