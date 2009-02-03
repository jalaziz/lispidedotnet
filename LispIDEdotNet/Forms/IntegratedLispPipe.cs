using LispIDEdotNet.Components;
using ScintillaNet;

namespace LispIDEdotNet.Forms
{
    public partial class IntegratedLispPipe : LispPipe
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
                return this.scintilla;
            }
        }

        #endregion Properties

        public IntegratedLispPipe()
        {
            InitializeComponent();
        }

        #region Methods

        protected override void SetText(string text)
        {
            ((IntegratedScintillaPipe)this.Scintilla).OutputStreamMode = true;
            this.Scintilla.AppendText(text);
            this.Scintilla.EndOfLine.ConvertAllLines(this.Scintilla.EndOfLine.Mode);
            this.Scintilla.CurrentPos = this.Scintilla.TextLength;
            ((IntegratedScintillaPipe)this.Scintilla).OutputStreamMode = false;
        }

        #endregion Methods
    }
}