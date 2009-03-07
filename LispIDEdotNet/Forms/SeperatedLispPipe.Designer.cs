using LispIDEdotNet.Components;

namespace LispIDEdotNet.Forms
{
    partial class SeperatedLispPipe
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeperatedLispPipe));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.scintilla = new ScintillaNet.Scintilla();
            this.scintillaBuffer = new LispIDEdotNet.Components.SeperatedScintillaPipe();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scintilla)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scintillaBuffer)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.scintilla);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.scintillaBuffer);
            this.splitContainer1.Panel2MinSize = 40;
            this.splitContainer1.Size = new System.Drawing.Size(533, 362);
            this.splitContainer1.SplitterDistance = 289;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 0;
            // 
            // scintilla
            // 
            this.scintilla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scintilla.Location = new System.Drawing.Point(0, 0);
            this.scintilla.Name = "scintilla";
            this.scintilla.Size = new System.Drawing.Size(529, 285);
            this.scintilla.TabIndex = 0;
            // 
            // scintillaBuffer
            // 
            this.scintillaBuffer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scintillaBuffer.Location = new System.Drawing.Point(0, 0);
            this.scintillaBuffer.Name = "scintillaBuffer";
            this.scintillaBuffer.Size = new System.Drawing.Size(529, 67);
            this.scintillaBuffer.TabIndex = 0;
            this.scintillaBuffer.TextDeleted += new System.EventHandler<ScintillaNet.TextModifiedEventArgs>(this.seperatedScintillaPipe1_TextDeleted);
            this.scintillaBuffer.TextInserted += new System.EventHandler<ScintillaNet.TextModifiedEventArgs>(this.seperatedScintillaPipe1_TextInserted);
            // 
            // SeperatedLispPipe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 362);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SeperatedLispPipe";
            this.Load += new System.EventHandler(this.SeperatedLispPipe_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scintilla)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scintillaBuffer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private SeperatedScintillaPipe scintillaBuffer;
        private ScintillaNet.Scintilla scintilla;
    }
}