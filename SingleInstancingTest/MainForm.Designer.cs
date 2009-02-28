namespace SingleInstancingTest
{
    partial class MainForm
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.cmdNewInstance = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdNewInstance
            // 
            this.cmdNewInstance.AutoSize = true;
            this.cmdNewInstance.Location = new System.Drawing.Point(27, 38);
            this.cmdNewInstance.Name = "cmdNewInstance";
            this.cmdNewInstance.Size = new System.Drawing.Size(249, 34);
            this.cmdNewInstance.TabIndex = 0;
            this.cmdNewInstance.Text = "Open a new instance of the application";
            this.cmdNewInstance.UseVisualStyleBackColor = true;
            this.cmdNewInstance.Click += new System.EventHandler(this.cmdNewInstance_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 111);
            this.Controls.Add(this.cmdNewInstance);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Single Instance Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdNewInstance;
    }
}

