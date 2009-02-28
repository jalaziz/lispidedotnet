using System;
using System.Windows.Forms;
using SingleInstancing;

namespace SingleInstancingTest
{
    public partial class MainForm : SingleInstance
    {
        public MainForm()
        {
            InitializeComponent();
        }

        #region ISingleInstanceEnforcer Members

        protected override void OnMessageReceived(MessageEventArgs e)
        {
            base.OnMessageReceived(e);

            object[] msg = (object[])e.Message;

            MessageBox.Show(msg[0] + ", " + String.Join(", ", (string[])msg[1]), "Message From New Instance", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        private void cmdNewInstance_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Application.ExecutablePath);
            }
            catch
            {
                MessageBox.Show("Failed to start a new instance of the application.");
            }
        }
    }
}