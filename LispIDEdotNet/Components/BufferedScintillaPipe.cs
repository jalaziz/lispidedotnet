using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ScintillaNet;

namespace LispIDEdotNet.Components
{
    /// <summary>
    /// Represents a Scintilla text editor control with history and buffering capabilities.
    /// </summary>
    [DefaultBindingProperty("Text"), DefaultProperty("Text"), DefaultEvent("DocumentChanged")]
    public abstract class BufferedScintillaPipe : Scintilla
    {
        #region Notifiers

        private static readonly object _bufferReadyEventKey = new object();

        #endregion Notifiers

        #region Fields

        protected const int WM_KEYDOWN = 0x100;
        protected const int WM_CHAR = 0x102;
        protected const int WM_PASTE = 0x302;

        private int currCommand = 0;
        private readonly List<string> history = new List<string>();
        
        #endregion Fields

        #region Properties

        public virtual string BufferedText
        {
            get { throw new NotImplementedException(); }
        }

        protected virtual bool HandleEnter
        {
            get { return false; }
        }

        protected int NextCommand
        {
            get
            {
                this.currCommand++;
                if (this.currCommand >= this.history.Count)
                    this.currCommand = 0;

                return this.currCommand;
            }
        }

        protected int PrevCommand
        {
            get
            {
                this.currCommand--;
                if (this.currCommand < 0)
                    this.currCommand = this.history.Count - 1;
                return this.currCommand;
            }
        }
        
        protected List<string> History
        {
            get
            {
                return this.history;
            }
        }

        #endregion Properties

        #region Constructor

        protected BufferedScintillaPipe()
        {
            this.Commands.AddBinding(Keys.Enter, Keys.Control, BindableCommand.NewLine);
        }

        #endregion Constructor

        #region Methods

        protected virtual void GetSelection(out int start, out int end)
        {
            Range range = this.Selection.Range;
            start = range.Start;
            end = range.End;
        }

        protected virtual void ClearBuffer()
        {
            return;
        }

        protected virtual void SetCommand(string command)
        {
            return;
        }

        #endregion Methods

        #region Events

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // We need to enable history navigation and limit the availibility
            // of actions on already printed text
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (!(e.Control || e.Shift || e.Alt))
                    {
                        string text = this.BufferedText;
                        if (!String.IsNullOrEmpty(text))
                            this.history.Add(text);

                        this.currCommand = this.history.Count;
                        OnBufferReady(EventArgs.Empty);
                        ClearBuffer();
                        e.Handled = HandleEnter;
                    }
                    break;
                case Keys.Up:
                    if (this.history.Count > 0)
                    {
                        SetCommand(this.history[this.PrevCommand].Trim());
                        e.Handled = true;
                    }
                    break;
                case Keys.Down:
                    if (this.history.Count > 0)
                    {
                        SetCommand(this.history[this.NextCommand].Trim());
                        e.Handled = true;
                    }
                    break;
                default:
                    break;
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Occurs when the text user hits Enter, signifying the buffer should be cleared.
        /// </summary>
        [Category("Buffer"), Description("Occurs when the text or styling of the document changes or is about to change.")]
        public event EventHandler BufferReady
        {
            add { this.Events.AddHandler(_bufferReadyEventKey, value); }
            remove { this.Events.RemoveHandler(_bufferReadyEventKey, value); }
        }

        /// <summary>
        /// Raises the <see cref="BufferReady"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnBufferReady(EventArgs e)
        {
            EventHandler handler = this.Events[_bufferReadyEventKey] as EventHandler;
            if (handler != null)
                handler(this, e);
        }

        #endregion Events
    }
}