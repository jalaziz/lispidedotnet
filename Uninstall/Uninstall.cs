using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Uninstall
{
    // Source: http://gaaton.blogspot.com/2006/07/add-uninstall-and-him-shortcut-to-net.html
    class Uninstall
    {
        static void Main(string[] args)
        {
            string[] arguments = Environment.GetCommandLineArgs();

            foreach (string argument in arguments)
            {
                string[] parameters = argument.Split('=');
                if (parameters[0].ToLower() == "/u")
                {
                    string productCode = parameters[1];
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.System);
                    Process proc = new Process();
                    proc.StartInfo.FileName = string.Concat(path, "\\msiexec.exe");
                    proc.StartInfo.Arguments = string.Concat(" /x ", productCode);
                    proc.Start();
                }
            }
        }
    }
}
