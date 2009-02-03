using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LispIDEdotNet.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace LispIDEdotNet.Utilities
{
    static class FileCommands
    {
        #region Fields

        private const string NEW_DOCUMENT_TEXT = "Untitled";

        private static int newDocumentCount = 0;

        public static string filter = "Lisp files (*.lisp;*.lsp)|*.lisp;*.lsp|All files (*.*)|*.*";
        public static string defaultExt = "lsp";

        #endregion Fields

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
            LispEditor editor = new LispEditor();
            editor.Scintilla.Text = File.ReadAllText(filePath);
            editor.Scintilla.UndoRedo.EmptyUndoBuffer();
            editor.Scintilla.Modified = false;
            editor.Text = Path.GetFileName(filePath);
            editor.FilePath = filePath;

            return editor;
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
                return true;
            }
            catch (Exception) { }

            return false;
        }

        #endregion Methods
    }
}