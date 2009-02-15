using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using LispIDEdotNet.Properties;
using LispIDEdotNet.Utilities;
using ScintillaNet;
using WeifenLuo.WinFormsUI.Docking;
using Clipboard=System.Windows.Forms.Clipboard;

namespace LispIDEdotNet.Forms
{
    public partial class LispEditor : DockContent
    {
        #region Fields

        private bool addSpace = false;
        private LispEditorContextMenu contextMenu;

        #endregion Fields

        #region Properties

        public string FilePath { get; set; }

        public Scintilla Scintilla
        { 
            get { return this.scintilla; }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                this.TabText = value;
            }
        }

        #endregion Properties

        public LispEditor()
        {
            InitializeComponent();
            this.contextMenu = new LispEditorContextMenu(this);
            this.TabPageContextMenuStrip = this.contextMenu;
            this.Scintilla.Folding.Flags = FoldFlag.LineAfterContracted;
            //this.Scintilla.Commands.RemoveBinding(Keys.Enter, Keys.Shift);
            //this.Scintilla.KeyPress += new KeyPressEventHandler(Scintilla_KeyPress);
        }

        #region Methods

        public string GetPipeString()
        {
            string text = this.Scintilla.Selection.Text;

            if (String.IsNullOrEmpty(text))
            {
                int startPos = this.Scintilla.CurrentPos - 1;
                char c = this.Scintilla.CharAt(startPos);

                if (c == ')')
                {
                    int endPos = this.Scintilla.NativeInterface.BraceMatch(startPos, 0);
                    if (endPos >= 0)
                    {
                        text = this.Scintilla.GetRange(endPos, startPos + 1).Text;
                    }
                }
            }

            return text;
        }

        #endregion Methods

        #region Events

        /*
        private void Scintilla_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && ModifierKeys == Keys.Shift)
            {
                e.Handled = true;

                string pipestring = GetPipeString();
            }
        }
         */

        private void scintilla_ModifiedChanged(object sender, EventArgs e)
        {
            if (this.scintilla.Modified)
            {
                if (!this.Text.EndsWith(" *"))
                    this.Text += " *";
            } else
            {
                if (this.Text.EndsWith(" *"))
                    this.Text = this.Text.Substring(0, this.Text.Length - 2);
            }
        }

        private void scintilla_CharAdded(object sender, CharAddedEventArgs e)
        {
            if (e.Ch == '(' && ConfigurationManager.EnableAutocomplete)
                this.Scintilla.AutoComplete.Show();
        }

        private void scintilla_AutoCompleteAccepted(object sender, AutoCompleteAcceptedEventArgs e)
        {
            this.addSpace = true;
        }

        private void scintilla_DocumentChange(object sender, NativeScintillaEventArgs e)
        {
            if (this.addSpace && ((e.SCNotification.modificationType & 4) != 0))
            {
                this.addSpace = false;

                if (this.Scintilla.CharAt(this.Scintilla.CurrentPos - 1) == ')')
                {
                    this.Scintilla.UndoRedo.Undo();
                }

                this.Scintilla.InsertText(" )");
                this.Scintilla.CurrentPos = this.Scintilla.CurrentPos - 1;
            }
        }

        private void LispEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if the document has been modified
            if (this.Scintilla.Modified)
            {
                string message = String.Format(
                    CultureInfo.CurrentCulture,
                    "The text in {0} has changed.{1}{2}Do you want to save the changes?",
                    this.Text.TrimEnd(' ', '*'),
                    Environment.NewLine,
                    Environment.NewLine);

                DialogResult dr = MessageBox.Show(this, message, Program.Title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                if (dr == DialogResult.Cancel)
                {
                    // Cancel closing
                    e.Cancel = true;
                    return;
                } else if (dr == DialogResult.Yes)
                {
                    // Try to save
                    e.Cancel = !FileCommands.SaveFile(this, this);
                    return;
                }
            }

            // Continue closing
        }

        #endregion Events

        #region ContextMenu

        protected class LispEditorContextMenu : ContextMenuStrip
        {
            private LispEditor parent;

            public LispEditorContextMenu(LispEditor parent, IContainer container) : base(container)
            {
                this.parent = parent;
                InitializeComponent();
            }

            public LispEditorContextMenu(LispEditor parent) : base()
            {
                this.parent = parent;
                InitializeComponent();
            }

            protected override void OnOpening(CancelEventArgs e)
            {
                if(this.parent.DockPanel.DocumentsCount > 1)
                {
                    this.saveAllContextMenuItem.Enabled = true;
                    this.closeAllContextMenuItem.Enabled = true;
                    this.closeAllButThisContextMenuItem.Enabled = true;
                }
                else
                {
                    this.saveAllContextMenuItem.Enabled = false;
                    this.closeAllContextMenuItem.Enabled = false;
                    this.closeAllButThisContextMenuItem.Enabled = false;
                }

                if(this.parent.Scintilla.Modified)
                {
                    this.saveContextMenuItem.Enabled = true;
                }
                else
                {
                    this.saveContextMenuItem.Enabled = false;
                }

                if(String.IsNullOrEmpty(this.parent.FilePath))
                {
                    this.copyFilePathContextMenuItem.Enabled = false;
                }
                else
                {
                    this.copyFilePathContextMenuItem.Enabled = true;
                }

                base.OnOpening(e);
            }

            private void InitializeComponent()
            {
                this.closeContextMenuItem = new ToolStripMenuItem();
                this.closeAllContextMenuItem = new ToolStripMenuItem();
                this.closeAllButThisContextMenuItem = new ToolStripMenuItem();
                this.closeSeperator = new ToolStripSeparator();
                this.saveContextMenuItem = new ToolStripMenuItem();
                this.saveAsContextMenuItem = new ToolStripMenuItem();
                this.saveAllContextMenuItem = new ToolStripMenuItem();
                this.saveSeperator = new ToolStripSeparator();
                this.copyFilePathContextMenuItem = new ToolStripMenuItem();
                this.SuspendLayout();

                //
                // Close Context Menu
                //
                this.closeContextMenuItem.Name = "closeContextMenuItem";
                this.closeContextMenuItem.Text = "Close";
                this.closeContextMenuItem.Click += new EventHandler(closeContextMenuItem_Click);

                this.closeAllContextMenuItem.Name = "closeAllContextMenuItem";
                this.closeAllContextMenuItem.Text = "Close All";
                this.closeAllContextMenuItem.Click += new EventHandler(closeAllContextMenuItem_Click);

                this.closeAllButThisContextMenuItem.Name = "closeAllButThisContextMenuItem";
                this.closeAllButThisContextMenuItem.Text = "Close All But This";
                this.closeAllButThisContextMenuItem.Click += new EventHandler(closeAllButThisContextMenuItem_Click);

                //
                // Save Context Menu
                //
                this.saveContextMenuItem.Name = "saveContextMenuItem";
                this.saveContextMenuItem.Text = "Save";
                this.saveContextMenuItem.Image = Resources.save;
                this.saveContextMenuItem.Click += new EventHandler(saveContextMenuItem_Click);

                this.saveAsContextMenuItem.Name = "saveAsContextMenuItem";
                this.saveAsContextMenuItem.Text = "Save As";
                this.saveAsContextMenuItem.Click += new EventHandler(saveAsContextMenuItem_Click);

                this.saveAllContextMenuItem.Name = "saveAllContextMenuItem";
                this.saveAllContextMenuItem.Text = "Save All";
                this.saveAllContextMenuItem.Image = Resources.saveAll;
                this.saveAllContextMenuItem.Click += new EventHandler(saveAllContextMenuItem_Click);
                
                //
                // Copy File Path Context Menu
                //
                this.copyFilePathContextMenuItem.Name = "copyFilePathContextMenuItem";
                this.copyFilePathContextMenuItem.Text = "Copy File Path";
                this.copyFilePathContextMenuItem.Click += new EventHandler(copyFilePathContextMenuItem_Click);

                this.Items.Add(this.closeContextMenuItem);
                this.Items.Add(this.closeAllContextMenuItem);
                this.Items.Add(this.closeAllButThisContextMenuItem);
                this.Items.Add(this.closeSeperator);
                this.Items.Add(this.saveContextMenuItem);
                this.Items.Add(this.saveAsContextMenuItem);
                this.Items.Add(this.saveAllContextMenuItem);
                this.Items.Add(this.saveSeperator);
                this.Items.Add(this.copyFilePathContextMenuItem);

                this.ResumeLayout(false);
            }

            void closeAllButThisContextMenuItem_Click(object sender, EventArgs e)
            {
                IDockContent[] editors = this.parent.DockPanel.DocumentsToArray();
                foreach (IDockContent editor in editors)
                {
                    LispEditor leditor = editor as LispEditor;
                    if(leditor != null && leditor != this.parent)
                    {
                        leditor.Close();
                    }
                }
            }

            void closeAllContextMenuItem_Click(object sender, EventArgs e)
            {
                FileCommands.CloseAll(this.parent.DockPanel.DocumentsToArray());
            }

            void closeContextMenuItem_Click(object sender, EventArgs e)
            {
                FileCommands.Close(this.parent);
            }

            private void copyFilePathContextMenuItem_Click(object sender, EventArgs e)
            {
                if(!String.IsNullOrEmpty(this.parent.FilePath))
                {
                    try
                    {
                        Clipboard.SetText(this.parent.FilePath);
                    }
                    catch (Exception)
                    { }
                }
            }

            private void saveAllContextMenuItem_Click(object sender, EventArgs e)
            {
                FileCommands.SaveAll(this.parent, this.parent.DockPanel.Documents);
            }

            private void saveContextMenuItem_Click(object sender, EventArgs e)
            {
                FileCommands.SaveFile(this.parent, this.parent);
            }

            private void saveAsContextMenuItem_Click(object sender, EventArgs e)
            {
                FileCommands.SaveFileAs(this.parent, this.parent);
            }

            private ToolStripMenuItem closeContextMenuItem;
            private ToolStripMenuItem closeAllContextMenuItem;
            private ToolStripMenuItem closeAllButThisContextMenuItem;
            private ToolStripSeparator closeSeperator;
            private ToolStripMenuItem saveContextMenuItem;
            private ToolStripMenuItem saveAllContextMenuItem;
            private ToolStripMenuItem saveAsContextMenuItem;
            private ToolStripSeparator saveSeperator;
            private ToolStripMenuItem copyFilePathContextMenuItem;
        }

        #endregion ContextMenu
    }
}