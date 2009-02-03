namespace LispIDEdotNet.Forms
{
    partial class LispPipe
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LispPipe));
            this.SuspendLayout();
            // 
            // LispPipe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 268);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                                                                          | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                                                                         | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                                                                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LispPipe";
            this.TabText = "LispPipe";
            this.Text = "LispPipe";
            this.Load += new System.EventHandler(this.LispPipe_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LispPipe_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

    }
}