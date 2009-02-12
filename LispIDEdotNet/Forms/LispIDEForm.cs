using System;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using LispIDEdotNet.Utilities;
using LispIDEdotNet.Utilities.Configuration;
using ScintillaNet;
using WeifenLuo.WinFormsUI.Docking;
using ConfigurationManager=LispIDEdotNet.Utilities.ConfigurationManager;

namespace LispIDEdotNet.Forms
{
    public partial class LispIDEForm : Form
    {
        #region Constants

        private const string NEW_DOCUMENT_TEXT = "Untitled";
        private const int LINE_NUMBERS_MARGIN_WIDTH = 35; // TODO Don't hardcode this

        #endregion Constants

        #region Fields

        private bool toolstripButtonsEnabled = true;

        private LispPipe lispPipe;
        private ScintillaConfigurationManager scintillaConfig;

        private LispEditor activeDocument;

        #endregion Fields

        #region Properties

        public LispEditor ActiveDocument
        {
            get { return this.dockPanel1.ActiveDocument as LispEditor; }
        }

        #endregion Properties

        public LispIDEForm()
        {
            InitializeComponent();
            SetToolstripItemsEnabled(false);

            this.scintillaConfig = new ScintillaConfigurationManager();

            this.lispPipe = new IntegratedLispPipe();
            this.lispPipe.Scintilla.ConfigurationManager.Configure(this.scintillaConfig.PipeScintillaConfiguration);
            this.lispPipe.Scintilla.IsBraceMatching = true;
            this.lispPipe.Show(this.dockPanel1, DockState.DockBottom);
            this.lispPipe.LispPath = @"C:\GCL-ANSI\bin\gcl1.bat";

            SeperatedLispPipe slp = new SeperatedLispPipe();
            slp.Scintilla.ConfigurationManager.Configure(this.scintillaConfig.PipeScintillaConfiguration);
            slp.Scintilla.IsBraceMatching = true;
            slp.ScintillaBuffer.ConfigurationManager.Configure(this.scintillaConfig.PipeScintillaConfiguration);
            slp.ScintillaBuffer.IsBraceMatching = true;
            slp.ScintillaBuffer.LineWrap.Mode = WrapMode.None;
            slp.Show(this.dockPanel1, DockState.DockBottom);
            slp.LispPath = @"C:\GCL-ANSI\bin\gcl1.bat";

            this.statusStatusLabel.Text = "Ready";

            //For some reason, enter is not available in the form designer
            this.sendToLispToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Enter;
            this.macroexpandToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.Enter;

            ConfigurationManager.RecentFiles.RecentFileChanged +=
                new EventHandler<RecentFileEventArgs>(recentFiles_RecentFileChanged);
            ConfigurationManager.RecentFiles.RecentFileClicked +=
                new EventHandler<RecentFileEventArgs>(recentFiles_RecentFileClicked);
            ConfigurationManager.RecentFiles.GenerateRecentFiles(recentFilesToolStripMenuItem);

            bool showRecentFiles = (ConfigurationManager.RecentFiles.FileList.Count > 0);
            recentFilesToolStripMenuItem.Visible = showRecentFiles;
            recentFilesToolstripSeperator.Visible = showRecentFiles;
        }

        #region Recent File Events

        private void recentFiles_RecentFileClicked(object sender, RecentFileEventArgs e)
        {
            OpenFile(e.FilePath);
        }

        private void recentFiles_RecentFileChanged(object sender, RecentFileEventArgs e)
        {
            ConfigurationManager.RecentFiles.GenerateRecentFiles(recentFilesToolStripMenuItem);
            bool showRecentFiles = (ConfigurationManager.RecentFiles.FileList.Count > 0);
            recentFilesToolStripMenuItem.Visible = showRecentFiles;
            recentFilesToolstripSeperator.Visible = showRecentFiles;
        }

        #endregion Recent File Events

        #region File Menu Events

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAll();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool showPrintDialog = true;

            if (sender == this.printToolStripButton)
            {
                showPrintDialog = false;
            }

            if (this.ActiveDocument != null)
            {
                this.ActiveDocument.Scintilla.Printing.PrintDocument.DocumentName = this.ActiveDocument.Text;
                this.ActiveDocument.Scintilla.Printing.Print(showPrintDialog);
            }
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveDocument != null)
            {
                this.ActiveDocument.Scintilla.Printing.PrintDocument.DocumentName = this.ActiveDocument.Text;
                this.ActiveDocument.Scintilla.Printing.PrintPreview(this.ActiveDocument);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion File Menu Events

        #region Edit Menu Events

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.ActiveDocument != null)
                this.ActiveDocument.Scintilla.UndoRedo.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveDocument != null)
                this.ActiveDocument.Scintilla.UndoRedo.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveDocument != null)
                this.ActiveDocument.Scintilla.Clipboard.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.ActiveDocument != null)
                this.ActiveDocument.Scintilla.Clipboard.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.ActiveDocument != null)
                this.ActiveDocument.Scintilla.Clipboard.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.ActiveDocument != null)
                this.ActiveDocument.Scintilla.Selection.SelectAll();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.ActiveDocument != null)
                this.ActiveDocument.Scintilla.FindReplace.ShowFind();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.ActiveDocument != null)
                this.ActiveDocument.Scintilla.FindReplace.ShowReplace();
        }

        private void goToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.ActiveDocument != null)
                this.ActiveDocument.Scintilla.GoTo.ShowGoToDialog();
        }

        #endregion Edit Menu Events

        #region Bookmark Events

        private void toggleBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.ActiveDocument == null)
                return;
            
            Line currentLine = this.ActiveDocument.Scintilla.Lines.Current;
            if (this.ActiveDocument.Scintilla.Markers.GetMarkerMask(currentLine) == 0)
            {
                currentLine.AddMarker(0);
            } else
            {
                currentLine.DeleteMarker(0);
            }
        }

        private void previousBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveDocument == null)
                return;

            Line l = this.ActiveDocument.Scintilla.Lines.Current.FindPreviousMarker(1);
            if (l != null)
                l.Goto();
        }

        private void nextBookmakrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveDocument == null)
                return;

            Line l = this.ActiveDocument.Scintilla.Lines.Current.FindNextMarker(1);
            if (l != null)
                l.Goto();
        }

        private void clearBookmarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.ActiveDocument != null)
                this.ActiveDocument.Scintilla.Markers.DeleteAll();
        }

        #endregion Bookmark Events

        #region Formatting Events

        private void makeUppercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveDocument != null)
                this.ActiveDocument.Scintilla.Commands.Execute(BindableCommand.UpperCase);
        }

        private void makeLowercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveDocument != null)
                this.ActiveDocument.Scintilla.Commands.Execute(BindableCommand.LowerCase);
        }

        private void commentStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveDocument != null)
                this.ActiveDocument.Scintilla.Commands.Execute(BindableCommand.StreamComment);
        }

        private void commentLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveDocument != null)
                this.ActiveDocument.Scintilla.Commands.Execute(BindableCommand.LineComment);
        }

        private void uncommentLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveDocument != null)
                this.ActiveDocument.Scintilla.Commands.Execute(BindableCommand.LineUncomment);
        }

        #endregion Formatting Events

        #region Window Events

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.ActiveDocument != null)
                FileCommands.Close(this.ActiveDocument);
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileCommands.CloseAll(this.dockPanel1.DocumentsToArray());
        }

        #endregion Window Events

        #region View Events

        private void toolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.toolbarToolStripMenuItem.Checked = !this.toolbarToolStripMenuItem.Checked;
            this.lispToolStrip.Visible = this.toolbarToolStripMenuItem.Checked;
            this.standardToolStrip.Visible = this.toolbarToolStripMenuItem.Checked;
        }

        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.statusBarToolStripMenuItem.Checked = !this.statusBarToolStripMenuItem.Checked;
            this.statusStrip1.Visible = this.statusBarToolStripMenuItem.Checked;
        }

        private void resetZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.Scintilla.Zoom = 0;
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveDocument != null)
                ++ActiveDocument.Scintilla.Zoom;
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveDocument != null)
                --ActiveDocument.Scintilla.Zoom;
        }

        private void whitespaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WhitespaceMode whitespaceMode = whitespaceToolStripMenuItem.Checked
                                                ? WhitespaceMode.VisibleAlways
                                                : WhitespaceMode.Invisible;

            foreach (LispEditor editor in this.dockPanel1.Documents)
            {
                editor.Scintilla.Whitespace.Mode = whitespaceMode;
            }
        }

        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WrapMode wrapMode = wordWrapToolStripMenuItem.Checked ? WrapMode.Word : WrapMode.None;

            foreach (LispEditor editor in this.dockPanel1.Documents)
            {
                editor.Scintilla.LineWrap.Mode = wrapMode;
            }
        }

        private void endOfLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool showEOL = endOfLineToolStripMenuItem.Checked;

            foreach (LispEditor editor in this.dockPanel1.Documents)
            {
                editor.Scintilla.EndOfLine.IsVisible = showEOL;
            }
        }

        private void lineNumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int lineNumbers = lineNumbersToolStripMenuItem.Checked ? 35 : 0;

            foreach (LispEditor editor in this.dockPanel1.Documents)
            {
                editor.Scintilla.Margins[0].Width = lineNumbers;
            }
        }

        private void indentGuideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IndentGuideView guideView = indentGuideToolStripMenuItem.Checked
                                            ? IndentGuideView.LookForward
                                            : IndentGuideView.None;

            foreach (LispEditor editor in this.dockPanel1.Documents)
            {
                editor.Scintilla.Indentation.Guides = guideView;
            }
        }

        private void navigateForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.Scintilla.DocumentNavigation.NavigateForward();
        }

        private void navigateBackwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.Scintilla.DocumentNavigation.NavigateBackward();
        }

        #endregion View Events

        #region Help Menu Events

        private void clhHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "CLHS.chm");
        }

        private void cltlHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "CLtL2.chm");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(AboutDialog about = new AboutDialog())
                about.ShowDialog(this);
        }

        #endregion Help Menu Events

        #region Command Events

        private void resetLispToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.lispPipe.StartLisp();
        }

        private void macroexpandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveDocument == null)
                return;

            string pipetext = this.ActiveDocument.GetPipeString();

            if (!String.IsNullOrEmpty(pipetext))
                this.lispPipe.SendCommand("(pprint (macroexpand-1 '" + pipetext + "))");
        }

        private void sendToLispToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveDocument == null)
                return;

            string pipetext = this.ActiveDocument.GetPipeString();

            if (!String.IsNullOrEmpty(pipetext))
                this.lispPipe.SendCommand(pipetext);
        }

        #endregion Command Events

        #region StatusBar Events

        private void macEOLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveDocument != null)
            {
                ActiveDocument.Scintilla.EndOfLine.Mode = EndOfLineMode.CR;
                ActiveDocument.Scintilla.EndOfLine.ConvertAllLines(EndOfLineMode.CR);
                SetStatusLabels();
            }
        }

        private void linuxEOLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveDocument != null)
            {
                ActiveDocument.Scintilla.EndOfLine.Mode = EndOfLineMode.LF;
                ActiveDocument.Scintilla.EndOfLine.ConvertAllLines(EndOfLineMode.LF);
                SetStatusLabels();
            }
        }

        private void dosEOLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveDocument != null)
            {
                ActiveDocument.Scintilla.EndOfLine.Mode = EndOfLineMode.Crlf;
                ActiveDocument.Scintilla.EndOfLine.ConvertAllLines(EndOfLineMode.Crlf);
                SetStatusLabels();
            }
        }

        #endregion StatusBar Events

        #region Main Form Events

        private void LispIDEForm_Load(object sender, EventArgs e)
        {
            this.Text = Program.Title;
            this.aboutToolStripMenuItem.Text = String.Format(CultureInfo.CurrentCulture, "&About {0}...", Program.Title);
            LoadOpenDocuments();
            ConfigurationManager.LoadWindowState(this);
        }

        private void LispIDEForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConfigurationManager.SaveWindowState(this);
            SaveOpenDocuments();
        }

        private void dockPanel1_ActiveDocumentChanged(object sender, EventArgs e)
        {
            // Update the main form text to show the current document
            if (this.ActiveDocument != null)
                this.Text = String.Format(CultureInfo.CurrentCulture, "{0} - {1}", this.ActiveDocument.Text, Program.Title);
            else
                this.Text = Program.Title;

            if(this.activeDocument != null)
            {
                this.activeDocument.Scintilla.SelectionChanged -= Scintilla_SelectionChanged;
                this.activeDocument.Scintilla.DocumentChange -= Scintilla_DocumentChange;
            }

            this.activeDocument = this.ActiveDocument;

            bool editItems = true;

            if (this.activeDocument == null)
            {
                editItems = false;
                this.eolFormatStatusLabel.Text = String.Empty;
                this.eolFormatStatusLabel.Enabled = false;

                this.lineInfoStatusLabel.Text = String.Empty;
                this.lineInfoStatusLabel.Enabled = false;

                this.lengthStatusLabel.Text = String.Empty;
                this.lengthStatusLabel.Enabled = false;

                SetStatusLabels();
            }
            else
            {
                this.eolFormatStatusLabel.Enabled = true;
                this.lineInfoStatusLabel.Enabled = true;
                this.lengthStatusLabel.Enabled = true;

                this.activeDocument.Scintilla.SelectionChanged += new EventHandler(Scintilla_SelectionChanged);
                this.activeDocument.Scintilla.DocumentChange += new EventHandler<NativeScintillaEventArgs>(Scintilla_DocumentChange);
            }

            SetToolstripItemsEnabled(editItems);
        }

        #endregion Main Form Events

        #region Scintilla Events

        private void Scintilla_DocumentChange(object sender, NativeScintillaEventArgs e)
        {
            SetStatusLabels();
        }

        private void Scintilla_SelectionChanged(object sender, EventArgs e)
        {
            SetStatusLabels();
        }

        #endregion Scintilla Events

        #region Methods

        private void SaveOpenDocuments()
        {
            ConfigurationManager.OpenDocuments.Clear();

            foreach (LispEditor editor in dockPanel1.Documents)
            {
                if(editor != null && !String.IsNullOrEmpty(editor.FilePath))
                {
                    OpenDocumentElement doc = new OpenDocumentElement();
                    doc.Name = editor.Text;
                    doc.FilePath = editor.FilePath;
                    doc.DockState = editor.DockState;
                    ConfigurationManager.OpenDocuments.Add(doc);
                }
            }

            ConfigurationManager.Save();
        }

        private void LoadOpenDocuments()
        {
            OpenDocumentsCollection docs = ConfigurationManager.OpenDocuments;

            foreach (OpenDocumentElement doc in docs)
            {
                OpenFile(doc.FilePath, doc.DockState);
            }
        }

        private void ShowEditor(LispEditor editor, DockState dockState)
        {
            if(editor == null)
                return;

            editor.Scintilla.ConfigurationManager.Configure(this.scintillaConfig.ScintillaConfiguration);
            editor.Scintilla.IsBraceMatching = true;
            editor.Show(this.dockPanel1, dockState);
            SetStatusLabels();
        }

        private void NewFile()
        {
            LispEditor editor = FileCommands.NewFile();
            ShowEditor(editor, DockState.Document);
        }

        private void OpenFile()
        {
            string[] files = FileCommands.OpenFiles(this);

            foreach (string filePath in files)
            {
                OpenFile(filePath);
            }
        }

        private void OpenFile(string filePath)
        {
            OpenFile(filePath, DockState.Document);
        }

        private void OpenFile(string filePath, DockState dockState)
        {
            // Ensure this file isn't already open
            if (!IsOpen(filePath))
            {
                LispEditor editor = FileCommands.OpenFile(filePath); //Open the file

                if (editor != null)
                {
                    ShowEditor(editor, dockState);
                }
            }
        }

        private bool IsOpen(string filePath)
        {
            bool isOpen = false;
            foreach (LispEditor documentForm in this.dockPanel1.Documents)
            {
                if (filePath.Equals(documentForm.FilePath, StringComparison.OrdinalIgnoreCase))
                {
                    documentForm.Select();
                    isOpen = true;
                    break;
                }
            }

            return isOpen;
        }

        private void SaveAll()
        {
            FileCommands.SaveAll(this, this.dockPanel1.Documents);
        }

        private void SaveFile()
        {
            SaveFile(this.ActiveDocument);
        }

        private void SaveFile(LispEditor editor)
        {
            FileCommands.SaveFile(this, editor);
        }

        private void SaveFileAs()
        {
            FileCommands.SaveFileAs(this, this.ActiveDocument);
        }

        private void SetStatusLabels()
        {
            string length;
            string info;
            string eol = String.Empty;

            if(this.ActiveDocument != null)
            {
                Range selRange = this.ActiveDocument.Scintilla.Selection.Range;

                length = "Len: " + this.ActiveDocument.Scintilla.TextLength;
                info = String.Format("Ln: {0}  Col: {1}  Sel: {2}",
                                     selRange.EndingLine,
                                     selRange.End,
                                     selRange.Length);

                this.dosEOLToolStripMenuItem.Checked = false;
                this.linuxEOLToolStripMenuItem.Checked = false;
                this.macEOLToolStripMenuItem.Checked = false;

                switch (this.ActiveDocument.Scintilla.EndOfLine.Mode)
                {
                    case EndOfLineMode.CR:
                        eol = "Mac";
                        this.macEOLToolStripMenuItem.Checked = true;
                        break;
                    case EndOfLineMode.LF:
                        eol = "Linux\\Unix";
                        this.linuxEOLToolStripMenuItem.Checked = true;
                        break;
                    case EndOfLineMode.Crlf:
                        eol = "Dos\\Windows";
                        this.dosEOLToolStripMenuItem.Checked = true;
                        break;
                }
            }
            else
            {
                length = "Len: 0";
                info = "Ln: 0  Col: 0  Sel: 0";
                eol = "EOL";
            }

            this.lengthStatusLabel.Text = length;
            this.lineInfoStatusLabel.Text = info;
            this.eolFormatStatusLabel.Text = eol;
        }

        private void SetToolstripItemsEnabled(bool enabled)
        {
            if (this.toolstripButtonsEnabled == enabled)
                return;

            this.saveToolStripButton.Enabled = enabled;
            this.saveToolStripMenuItem.Enabled = enabled;
            this.saveAsToolStripMenuItem.Enabled = enabled;
            this.saveAllToolStripButton.Enabled = enabled;
            this.saveAllToolStripMenuItem.Enabled = enabled;
            
            this.cutToolStripButton.Enabled = enabled;
            this.cutToolStripMenuItem.Enabled = enabled;
            this.copyToolStripButton.Enabled = enabled;
            this.copyToolStripMenuItem.Enabled = enabled;
            this.pasteToolStripButton.Enabled = enabled;
            this.pasteToolStripMenuItem.Enabled = enabled;
            this.undoToolStripButton.Enabled = enabled;
            this.undoToolStripMenuItem.Enabled = enabled;
            this.redoToolStripButton.Enabled = enabled;
            this.redoToolStripMenuItem.Enabled = enabled;

            this.selectAllToolStripMenuItem.Enabled = enabled;

            this.printToolStripButton.Enabled = enabled;
            this.printToolStripMenuItem.Enabled = enabled;
            this.printPreviewToolStripMenuItem.Enabled = enabled;

            this.sendToLispToolStripButton.Enabled = enabled;
            this.sendToLispToolStripMenuItem.Enabled = enabled;
            this.macroexpandToolStripMenuItem.Enabled = enabled;

            this.findAndReplaceToolStripMenuItem.Enabled = enabled;
            this.goToToolStripMenuItem.Enabled = enabled;
            this.bookmarksToolStripMenuItem.Enabled = enabled;
            this.advancedToolStripMenuItem.Enabled = enabled;

            this.closeToolStripMenuItem.Enabled = enabled;
            this.closeAllToolStripMenuItem.Enabled = enabled;

            this.toolstripButtonsEnabled = enabled;
        }

        #endregion Methods
    }
}