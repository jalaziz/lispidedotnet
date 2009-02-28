namespace LispIDEdotNet.Forms
{
    partial class LispEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LispEditor));
            this.scintilla = new ScintillaNet.Scintilla();
            ((System.ComponentModel.ISupportInitialize)(this.scintilla)).BeginInit();
            this.SuspendLayout();
            // 
            // scintilla
            // 
            this.scintilla.AllowDrop = true;
            this.scintilla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scintilla.Location = new System.Drawing.Point(0, 0);
            this.scintilla.Name = "scintilla";
            this.scintilla.Size = new System.Drawing.Size(288, 268);
            this.scintilla.TabIndex = 0;
            this.scintilla.CharAdded += new System.EventHandler<ScintillaNet.CharAddedEventArgs>(this.scintilla_CharAdded);
            this.scintilla.ModifiedChanged += new System.EventHandler(this.scintilla_ModifiedChanged);
            this.scintilla.AutoCompleteAccepted += new System.EventHandler<ScintillaNet.AutoCompleteAcceptedEventArgs>(this.scintilla_AutoCompleteAccepted);
            this.scintilla.DocumentChange += new System.EventHandler<ScintillaNet.NativeScintillaEventArgs>(this.scintilla_DocumentChange);
            // 
            // LispEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 268);
            this.Controls.Add(this.scintilla);
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LispEditor";
            this.TabText = "LispEditor";
            this.Text = "LispEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LispEditor_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.scintilla)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ScintillaNet.Scintilla scintilla;
    }
}