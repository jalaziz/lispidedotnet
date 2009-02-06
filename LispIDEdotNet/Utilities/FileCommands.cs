using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LispIDEdotNet.Forms;
using LispIDEdotNet.Utilities.Configuration;
using WeifenLuo.WinFormsUI.Docking;

namespace LispIDEdotNet.Utilities
{
    class FileCommands
    {
        #region Fields

        private const string NEW_DOCUMENT_TEXT = "Untitled";

        private static int newDocumentCount = 0;

        public static string filter = "Lisp files (*.lisp;*.lsp)|*.lisp;*.lsp|All files (*.*)|*.*";
        public static string defaultExt = "lsp";

        #endregion Fields

        #region Event Handlers

        public static event EventHandler<FileCommandEventArgs> FileCreated;
        public static event EventHandler<FileCommandEventArgs> FileOpened;
        public static event EventHandler<FileCommandEventArgs> FileSaved;
        public static event EventHandler<FileCommandEventArgs> FileClosed;

        #endregion

        #region Properties

        public static string Filter
        {
            get
            {
                return filter;
            }
            set
            {
                filter = value;
            }
        }

        public static string DefaultExt
        {
            get
            {
                return defaultExt;
            }
            set
            {
                defaultExt = value;
            }
        }

        #endregion Properties

        #region Methods

        public static LispEditor NewFile()
        {
            LispEditor editor = new LispEditor();
            editor.Text = string.Format(CultureInfo.CurrentCulture, "{0}{1}", NEW_DOCUMENT_TEXT, ++newDocumentCount);

            OnFileCreated(null, editor.Text, editor);

            return editor;
        }

        public static string[] OpenFiles(IWin32Window owner)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = Filter;
                openFileDialog.DefaultExt = DefaultExt;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog(owner) == DialogResult.OK)
                    return openFileDialog.FileNames;
            }

            return new string[0];
        }

        public static LispEditor OpenFile(string filePath)
        {
            try
            {
                LispEditor editor = new LispEditor();
                editor.Scintilla.Text = File.ReadAllText(filePath);
                editor.Scintilla.UndoRedo.EmptyUndoBuffer();
                editor.Scintilla.Modified = false;
                editor.Text = Path.GetFileName(filePath);
                editor.FilePath = filePath;

                OnFileOpened(editor.FilePath, editor.Text, editor);

                return editor;
            }
            catch (FileNotFoundException fex)
            {
                ConfigurationManager.RecentFiles.RemoveRecentFile(filePath);
                MessageBox.Show("Cannot open file \"" + filePath + "\". The file was not found.", "File not found",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot open file \"" + filePath + "\". There was an error opening the file.",
                                "Error opening file",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        public static bool SaveAll(IWin32Window owner, IEnumerable<IDockContent> documents)
        {
            bool status = true;

            foreach(LispEditor editor in documents)
            {
                if (!SaveFile(owner, editor))
                    status = false;
            }

            return status;
        }

        public static bool SaveFile(IWin32Window owner, LispEditor editor)
        {
            if(String.IsNullOrEmpty(editor.FilePath))
            {
                return SaveFileAs(owner, editor);
            }

            return Save(editor.FilePath, editor);
        }

        public static bool SaveFileAs(IWin32Window owner, LispEditor editor)
        {
            using(SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = Filter;
                saveFileDialog.DefaultExt = DefaultExt;
                
                if(saveFileDialog.ShowDialog(owner) == DialogResult.OK)
                {
                    return Save(saveFileDialog.FileName, editor);
                }
            }

            return false;
        }

        public static bool Save(string filePath, LispEditor editor)
        {
            try
            {
                using (FileStream fs = File.Create(filePath))
                using (BinaryWriter bw = new BinaryWriter(fs))
                    bw.Write(editor.Scintilla.RawText, 0, editor.Scintilla.RawText.Length - 1); // Omit trailing NULL

                editor.FilePath = filePath;
                editor.Text = Path.GetFileName(filePath);
                editor.Scintilla.Modified = false;

                OnFileSaved(editor.FilePath, editor.Text, editor);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot save file \"" + filePath + "\".\nException: " + ex.Message,
                                "Error saving file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        public static void Close(LispEditor editor)
        {
            if(editor != null)
            {
                string path = editor.FilePath;
                string name = editor.Text;
                editor.Close();
                OnFileClosed(path, name, editor);
            }
        }

        public static void CloseAll(IDockContent[] editors)
        {
            foreach (IDockContent editor in editors)
            {
                LispEditor leditor = editor as LispEditor;
                if (leditor != null)
                {
                    string path = leditor.FilePath;
                    string name = leditor.Text;
                    leditor.Close();
                    OnFileClosed(path, name, leditor);
                }
            }
        }

        #endregion Methods

        #region Events

        protected static void OnFileCreated(string file, string name, LispEditor editor)
        {
            if (FileCreated != null)
                FileCreated(null, new FileCommandEventArgs(file, name, editor));
        }

        protected static void OnFileOpened(string file, string name, LispEditor editor)
        {
            ConfigurationManager.RecentFiles.AddRecentFile(file);
            ConfigurationManager.Save();

            if (FileOpened != null)
                FileOpened(null, new FileCommandEventArgs(file, name, editor));
        }

        protected static void OnFileSaved(string file, string name, LispEditor editor)
        {
            ConfigurationManager.RecentFiles.AddRecentFile(file);
            ConfigurationManager.Save();

            if (FileSaved != null)
                FileSaved(null, new FileCommandEventArgs(file, name, editor));
        }

        protected static void OnFileClosed(string file, string name, LispEditor editor)
        {
            if (FileClosed != null)
                FileClosed(null, new FileCommandEventArgs(file, name, editor));
        }

        #endregion Events
    }

    public class FileCommandEventArgs : EventArgs
    {
        private string path;
        private string name;
        private LispEditor editor;

        public FileCommandEventArgs(string file, string name, LispEditor editor)
        {
            this.path = file;
            this.name = name;
            this.editor = editor;
        }

        public string FilePath
        {
            get
            {
                return path;
            }
        }

        public string EditorTabText
        {
            get
            {
                return name;
            }
        }

        public LispEditor Editor
        {
            get
            {
                return editor;
            }
        }
    }
}