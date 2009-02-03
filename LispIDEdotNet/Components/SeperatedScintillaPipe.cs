using System;
using ScintillaNet;

namespace LispIDEdotNet.Components
{
    class SeperatedScintillaPipe : BufferedScintillaPipe
    {
        #region Properties

        public override string BufferedText
        {
            get
            {
                return this.Text;
            }
        }

        #endregion Properties

        #region Methods

        protected override void ClearBuffer()
        {
            this.Text = String.Empty;
        }

        protected override void SetCommand(string command)
        {
            this.Selection.Range =
                new Range(0, this.TextLength, this);
            this.Selection.Text = command;
            // Force an OnSelectionChanged Event
            OnSelectionChanged(EventArgs.Empty);
        }

        #endregion Methods
    }
}