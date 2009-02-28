using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using LispIDEdotNet.Forms;
using SingleInstancing;

namespace LispIDEdotNet
{
    static class Program
    {
        public static string Title
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (!String.IsNullOrEmpty(titleAttribute.Title))
                        return titleAttribute.Title;
                }

                // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Debug.AutoFlush = true;
            Debug.WriteLine("Starting LispIDE.Net", "Info");

            try
            {
                using (LispIDEForm form = new LispIDEForm())
                {
                    // If this is the first instance of the application, run the main form
                    if (form.IsFirstInstance)
                    {
                        Debug.WriteLine("Fist Instance", "Info");
                        Application.Run(form);
                        Debug.WriteLine("First Instance Ended", "Info");
                    }
                    else // This is not the first instance of the application, so do nothing but send a message to the first instance
                    {
                        try
                        {
                            Debug.WriteLine("Second Instance", "Info");
                            form.SendMessageToFirstInstance(new object[] { Environment.CurrentDirectory, Environment.GetCommandLineArgs() });
                            Debug.WriteLine("Second Instance Message Sent", "Info");
                        } catch (ObjectDisposedException) { Debug.WriteLine("Second Instance: Object Disposed", "Debug"); }
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