﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AbstractBaseClass;
using LispIDEdotNet.Components;
using LispIDEdotNet.Utilities;
using ScintillaNet;
using WeifenLuo.WinFormsUI.Docking;

namespace LispIDEdotNet.Forms
{
    [TypeDescriptionProvider(typeof(ConcreteClassProvider))]
    [ConcreteClass(typeof(ConcreteLispPipe))]
    public abstract partial class LispPipe : DockContent
    {
        #region Fields

        private Process lispProcess;
        private StreamWriter stdInWriter;
        private StreamReader stdOutReader;

        private AsyncStreamReader output;

        private bool started = false;
        //private bool carryNewLine = false;

        #endregion Fields

        #region Properties

        public virtual string LispPath { get; set; }

        public virtual Scintilla Scintilla
        {
            get
            {
                // Workaround to enable design-time support
                if(this.DesignMode)
                    return new Scintilla();

                throw new NotImplementedException();
            }
        }

        public virtual BufferedScintillaPipe ScintillaBuffer
        {
            get
            {
                // Workaround to enable design-time support
                if (this.DesignMode)
                    return new IntegratedScintillaPipe();

                throw new NotImplementedException();
            }
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

        public LispPipe()
        {
            InitializeComponent();

            this.lispProcess = new Process();
            this.Disposed += (sender, e) => CloseLisp();
            this.Closing += (sender, e) => CloseLisp();
        }

        #region Methods

        public void SendCommand(string text)
        {
            if (this.stdInWriter != null)
            {
                this.stdInWriter.Write(text + Environment.NewLine);
                this.stdInWriter.Flush();
                //this.carryNewLine = true;
            }
        }

        public void StartLisp()
        {
            OpenLisp();
        }

        protected void OpenLisp()
        {
            // Close the lisp process if one has already
            // been started.
            CloseLisp();

            if(String.IsNullOrEmpty(this.LispPath))
            {
                throw new NullReferenceException("The Lisp Path cannot be empty or null.");
            }

            this.Scintilla.Text = String.Empty;

            try
            {
                // Redirect the output stream of the child process.
                this.lispProcess.StartInfo.UseShellExecute = false;
                this.lispProcess.StartInfo.RedirectStandardOutput = true;
                this.lispProcess.StartInfo.RedirectStandardError = true;
                this.lispProcess.StartInfo.RedirectStandardInput = true;
                this.lispProcess.StartInfo.CreateNoWindow = true;
                this.lispProcess.EnableRaisingEvents = true;
                this.lispProcess.Exited += new EventHandler(lispProcess_Exited);
                this.lispProcess.ErrorDataReceived += new DataReceivedEventHandler(lispProcess_ErrorDataReceived);
                this.lispProcess.StartInfo.FileName = this.LispPath;
                
                // Start the child process.
                this.lispProcess.Start();

                this.stdInWriter = this.lispProcess.StandardInput;
                this.stdOutReader = this.lispProcess.StandardOutput;

                this.output = new AsyncStreamReader(this.lispProcess, this.stdOutReader.BaseStream, 
                                               new UserCallBack(this.SetTextCallback), this.stdOutReader.CurrentEncoding);

                this.output.BeginRead();

                this.lispProcess.BeginErrorReadLine();

                started = true;
            }
            catch (Exception e)
            {
                this.Scintilla.InsertText("\r\nError starting REPL process.\r\n");
                this.Scintilla.InsertText(e.Message);

                throw e;
            }
        }

        protected void CloseLisp()
        {
            if (started)
            {
                if (this.output != null)
                {
                    this.output.CancelOperation();
                    this.output.Close();
                }

                this.lispProcess.CancelErrorRead();
                this.lispProcess.Exited -= lispProcess_Exited;
                this.lispProcess.ErrorDataReceived -= lispProcess_ErrorDataReceived;

                // We need to close the process if it hasn't exited
                if (!this.lispProcess.HasExited)
                {
                    // Try and close the process cleanly
                    this.SendCommand("(quit)" + Environment.NewLine + "(bye)" + Environment.NewLine + "(exit)" +
                                     Environment.NewLine);

                    // If that didnt work, force kill the process
                    // Note: GCL (and possibly other REPLs) run off a .bat file. Killing the process
                    // only kills the cmd.exe process and not the actual REPL. However, exiting
                    // LispIDE.Net does kill the actual REPL process. I'm not sure how to force
                    // kill the child process.
                    if (!this.lispProcess.WaitForExit(100))
                        this.lispProcess.Kill();
                }

                this.lispProcess.Close();

                started = false;
            }
        }

        public virtual void Configure(ScintillaNet.Configuration.Configuration config)
        {
            return;
        }

        // This method is used for making thread-safe
        // calls on the Scintilla control. 
        //
        // If the calling thread is different from the thread that
        // created the Scintilla control, this method creates a
        // SetTextCallback and calls itself asynchronously using the
        // Invoke method.
        //
        // If the calling thread is the same as the thread that created
        // the Scintilla control, the Text property is set directly. 
        private void SetTextCallback(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.InvokeRequired)
            {
                UserCallBack d = new UserCallBack(SetTextCallback);
                this.Invoke(d, new object[] { text });
            } 
            else
            {
                StringBuilder sb = new StringBuilder(text);
                //if (carryNewLine && sb[0] == '\n')
                //{
                //    carryNewLine = false;
                //    sb.Remove(0, 1);
                //}

                SetText(sb.ToString());
            }
        }

        protected virtual void SetText(string text)
        {
            return;
        }

        #endregion Methods

        #region Events

        private void LispPipe_Load(object sender, EventArgs e)
        {
            this.Scintilla.IsReadOnly = true;
            this.Scintilla.SizeChanged += new EventHandler(Scintilla_SizeChanged);
            this.ScintillaBuffer.BufferReady += new EventHandler(ScintillaBuffer_BufferReady);
        }

        private void Scintilla_SizeChanged(object sender, EventArgs e)
        {
            // We want the caret to be visibale after a size change.
            // This solution simply scrolls to the caret position so that it's the last
            // line in the view (if it wasn't in view before). This may not be the 
            // desired behavior, so I'll have to test it out and see.
            this.Scintilla.Scrolling.ScrollToCaret();
        }

        private void LispPipe_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.ScintillaBuffer.BufferReady -= ScintillaBuffer_BufferReady;
            CloseLisp();
        }

        private void lispProcess_Exited(object sender, EventArgs e)
        {
            CloseLisp();
            SetTextCallback("Error: The process exited unexpectedly!\n");
        }

        private void lispProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if(e.Data != null)
                SetTextCallback(e.Data + "\n");
        }

        private void ScintillaBuffer_BufferReady(object sender, EventArgs e)
        {
            SendCommand(this.ScintillaBuffer.BufferedText);
        }

        #endregion Events
    }

    internal class ConcreteLispPipe : LispPipe
    {
        public override Scintilla Scintilla
        {
            get
            {
                return new Scintilla();
            }
        }

        public override BufferedScintillaPipe ScintillaBuffer
        {
            get
            {
                return new IntegratedScintillaPipe();
            }
        }
    }
}