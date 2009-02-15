using System;
using LispIDEdotNet.Components;
using ScintillaNet;

namespace LispIDEdotNet.Forms
{
    public partial class SeperatedLispPipe : LispPipe
    {
        #region Properties

        public override Scintilla Scintilla
        {
            get
            {
                return this.scintilla;
            }
        }

        public override BufferedScintillaPipe ScintillaBuffer
        {
            get
            {
                return this.scintillaPipe;
            }
        }

        #endregion Properties

        public SeperatedLispPipe()
        {
            InitializeComponent();
            scintillaPipe.SizeChanged += new EventHandler(scintillaPipe_SizeChanged);
            scintilla.IsBraceMatching = true;
            scintillaPipe.IsBraceMatching = true;
            scintillaPipe.LineWrap.Mode = WrapMode.None;
        }

        #region Methods

        protected override void SetText(string text)
        {
            this.Scintilla.IsReadOnly = false;
            this.Scintilla.AppendText(text);
            this.Scintilla.EndOfLine.ConvertAllLines(this.Scintilla.EndOfLine.Mode);
            this.Scintilla.CurrentPos = this.Scintilla.TextLength;
            this.Scintilla.IsReadOnly = true;
        }

        private void SetPanelSize()
        {
            int lines = this.scintillaPipe.Lines.Count;
            int height = this.splitContainer1.Height;
            height = height - (lines * this.scintillaPipe.Lines[0].Height
                               + this.splitContainer1.SplitterRectangle.Height
                               + (this.splitContainer1.Margin.Bottom << 2));

            if (height > this.splitContainer1.Panel1MinSize)
                this.splitContainer1.SplitterDistance = height;
        }

        public override void Configure(ScintillaNet.Configuration.Configuration config)
        {
            scintilla.ConfigurationManager.Configure(config);
            scintillaPipe.ConfigurationManager.Configure(config);
        }

        #endregion Methods

        #region Events

        private void SeperatedLispPipe_Load(object sender, EventArgs e)
        {
            scintillaPipe.Focus();
        }

        private void scintillaPipe_SizeChanged(object sender, EventArgs e)
        {
            this.scintillaPipe.Scrolling.HorizontalWidth = this.scintillaPipe.ClientRectangle.Width
                                                           - (this.scintillaPipe.Margin.Left << 2);
        }

        private void seperatedScintillaPipe1_TextInserted(object sender, TextModifiedEventArgs e)
        {
            SetPanelSize();
        }

        private void seperatedScintillaPipe1_TextDeleted(object sender, TextModifiedEventArgs e)
        {
            SetPanelSize();
        }

        #endregion Events
    }
}