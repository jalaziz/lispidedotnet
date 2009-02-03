using System;
using System.Windows.Forms;
using ScintillaNet;

namespace LispIDEdotNet.Components
{
    /// <summary>
    /// Represents a Scintilla text editor control with the ability for output and input with an integrated buffer.
    /// </summary>
    public class IntegratedScintillaPipe : BufferedScintillaPipe
    {
        #region Fields

        private int currPos = -1;
        private int startPos = 0;

        private BufferedTextBox bufferTextBox = new BufferedTextBox();

        private bool outputStreamMode = false;

        #endregion Fields

        #region Properties

        public override string BufferedText
        {
            get { return this.bufferTextBox.Text; }
        }

        public bool OutputStreamMode
        { 
            get
            {
                return this.outputStreamMode;
            }
            set
            {
                this.outputStreamMode = value;
                this.IsReadOnly = !value;
            }
        }

        #endregion Properties

        #region Constructor

        public IntegratedScintillaPipe()
        { }

        #endregion Constructor

        #region Methods

        protected override void ClearBuffer()
        {
            this.bufferTextBox.Text = String.Empty;
            this.currPos = -1;
            this.startPos = this.TextLength;
        }

        protected override void SetCommand(string command)
        {
            this.Selection.Range =
                new Range(this.currPos, this.TextLength, this);
            this.Selection.Text = command;
            this.bufferTextBox.Text = command;
            // Force an OnSelectionChanged Event
            OnSelectionChanged(EventArgs.Empty);
        }

        #endregion Methods

        #region Events

        protected override void OnBufferReady(EventArgs e)
        {
            this.CurrentPos = this.TextLength;
            base.OnBufferReady(e);
        }

        protected override void OnTextInserted(TextModifiedEventArgs e)
        {
            if (!this.OutputStreamMode)
            {
                if (e.Position >= this.startPos)
                {
                    int pos = e.Position - this.startPos;

                    if(pos >= 0)
                    {
                        try
                        {
                            this.bufferTextBox.Text = this.bufferTextBox.Text.Insert(pos, e.Text);
                        }
                        catch(Exception) {}
                    }
                }
            }
            else
            {
                this.startPos = this.TextLength;
                this.currPos = -1;
                this.bufferTextBox.Text = String.Empty;
                this.UndoRedo.EmptyUndoBuffer();
            }

            base.OnTextInserted(e);
        }

        protected override void OnTextDeleted(TextModifiedEventArgs e)
        {
            if (!this.OutputStreamMode)
            {
                if (e.Position >= this.startPos)
                {
                    int pos = e.Position - this.startPos;

                    if (pos >= 0)
                    {
                        try
                        {
                            this.bufferTextBox.Text = this.bufferTextBox.Text.Remove(pos, e.Length);
                        } catch (Exception) { }
                    }
                }
            }

            base.OnTextDeleted(e);
        }

        protected override void OnSelectionChanged(EventArgs e)
        {
            this.bufferTextBox.SelectionStart = this.bufferTextBox.TextLength;

            this.IsReadOnly = this.Selection.Start < this.startPos;

            base.OnSelectionChanged(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            bool suppress = true;
            int start, end;

            if (this.currPos == -1)
            {
                GetSelection(out start, out end);
                this.currPos = this.startPos;
            }

            // We need to enable history navigation and limit the availibility
            // of actions on already printed text
            switch (e.KeyCode)
            {
                case Keys.Back:
                    GetSelection(out start, out end);
                    if(start > this.currPos)
                    {
                        suppress = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                    break;
                case Keys.Prior:
                case Keys.Next:
                case Keys.Left:
                case Keys.Right:
                    break;
                case Keys.Home:
                    this.Selection.Range =
                        new Range(this.currPos, this.currPos, this);
                    this.bufferTextBox.Select(0,0);
                    e.Handled = true;
                    break;
                case Keys.Return:
                    this.CurrentPos = this.TextLength;
                    suppress = false;
                    break;
                default:
                    GetSelection(out start, out end);
                    if (start < this.currPos)
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    else
                    {
                        //e.Handled = false;
                        suppress = false;
                    }
                    break;
            }

            base.OnKeyDown(e);

            if(!suppress)
            {
                e.SuppressKeyPress = false; 
            }
        }

        #endregion Events

        #region Message Forwarding

        //private bool preprocessed = false;
        private Message bufferMsg;

        public override bool PreProcessMessage(ref Message msg)
        {
            CopyMessage(ref msg, out this.bufferMsg);
            this.bufferTextBox.PreProcessMessage(ref this.bufferMsg);
            //preprocessed = true;
            return base.PreProcessMessage(ref msg);
        }

        /*
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_KEYDOWN:
                case WM_CHAR:
                case WM_PASTE:
                    if(!preprocessed)
                    {
                        CopyMessage(ref m, out bufferMsg);
                    }
                    bufferTextBox.SendMessage(ref bufferMsg);
                    preprocessed = false;
                    break;
            }

            base.WndProc(ref m);
        }
         */

        protected void CopyMessage(ref Message m, out Message newMsg)
        {
            newMsg = new Message();
            newMsg.Msg = m.Msg;
            newMsg.WParam = m.WParam;
            newMsg.LParam = m.LParam;
            newMsg.Result = m.Result;
            newMsg.HWnd = this.bufferTextBox.Handle;
        }

        #endregion Message Forwarding

        #region InnerBuffer

        private class BufferedTextBox : TextBox
        {
            public BufferedTextBox() : base()
            {
                this.Multiline = true;
                //this.AcceptsReturn = false;
            }

            /*
            public void SendMessage(ref Message m)
            {
                this.WndProc(ref m);
            }
             */
        }

        #endregion InnerBuffer
    }
}