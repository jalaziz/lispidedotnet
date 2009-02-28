using System;
using System.Windows.Forms;
using System.Collections.Generic;
using SingleInstancing;

namespace SingleInstancingTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                using(MainForm form = new MainForm())
                {
                    // If this is the first instance of the application, run the main form
                    if (form.IsFirstInstance)
                        Application.Run(form);
                    else // This is not the first instance of the application, so do nothing but send a message to the first instance
                    {
                        form.SendMessageToFirstInstance(new object[] { Environment.CurrentDirectory, Environment.GetCommandLineArgs() });
                    }
                }
            }
            catch (SingleInstancingException ex)
            {
                MessageBox.Show("Could not create a SingleInstance object:\n" + ex.Message + "\nApplication will now terminate.");

                return;
            }
        }
    }
}