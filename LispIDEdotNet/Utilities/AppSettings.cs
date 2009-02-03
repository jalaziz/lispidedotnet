using System;
using Microsoft.Win32;

namespace LispIDEdotNet.Utilities
{
    public static class AppSettings
    {
        // Must be called once to set the name of the key under HKLM\\Software
        // or HKCU\\Software.
        // Normally this will be of the form "[company name]\\[app name]"
        public static string RegKey { get; set; }

        public static string GetUserProfileString(string section, string entry)
        {
            return GetUserProfileString(section, entry, null);
        }

        public static string GetUserProfileString(string section, string entry, string defaultValue)
        {
            return (string)GetRegistryValue(Registry.CurrentUser, section, entry, defaultValue);
        }

        public static string GetMachineProfileString(string section, string entry)
        {
            return GetMachineProfileString(section, entry, null);
        }

        public static string GetMachineProfileString(string section, string entry, string defaultValue)
        {
            return (string)GetRegistryValue(Registry.LocalMachine, section, entry, defaultValue);
        }

        public static int GetUserProfileDword(string section, string entry)
        {
            return GetUserProfileDword(section, entry, 0);
        }

        public static int GetUserProfileDword(string section, string entry, int defaultValue)
        {
            return (int)GetRegistryValue(Registry.CurrentUser, section, entry, defaultValue);
        }

        public static int GetMachineProfileDword(string section, string entry)
        {
            return GetMachineProfileDword(section, entry, 0);
        }

        public static int GetMachineProfileDword(string section, string entry, int defaultValue)
        {
            return (int)GetRegistryValue(Registry.LocalMachine, section, entry, defaultValue);
        }

        private static object GetRegistryValue(RegistryKey root, string section, string entry, object defaulValue)
        {
            string subKey = CatKeyAndSubkeyNames(section);
            RegistryKey key = root.OpenSubKey(subKey);

            object val = defaulValue;

            if(key != null)
            {
                try
                {
                    val = key.GetValue(entry, defaulValue);
                }
                catch { }

                key.Close();
            }

            return val;
        }

        public static bool WriteMachineProfileString(string section, string entry, string val)
        {
            return WriteRegistryValue(Registry.LocalMachine, section, entry, val, RegistryValueKind.String);
        }

        public static bool WriteMachineProfileDword(string section, string entry, int val)
        {
            return WriteRegistryValue(Registry.LocalMachine, section, entry, val, RegistryValueKind.DWord);
        }

        public static bool WriteUserProfileString(string section, string entry, string val)
        {
            return WriteRegistryValue(Registry.CurrentUser, section, entry, val, RegistryValueKind.String);
        }

        public static bool WriteUserProfileDword(string section, string entry, int val)
        {
            return WriteRegistryValue(Registry.CurrentUser, section, entry, val, RegistryValueKind.DWord);
        }

        private static bool WriteRegistryValue(RegistryKey root, string section, string entry, object val, RegistryValueKind kind)
        {
            string subKey = CatKeyAndSubkeyNames(section);
            RegistryKey key = root.CreateSubKey(subKey);

            if(key != null)
            {
                try
                {
                    key.SetValue(entry, val, kind);
                    key.Close();
                    return true;
                }
                catch { }

                key.Close();
            }

            return false;
        }

        /// <exception cref="Exception">The RegKey property must be set.</exception>
        private static string CatKeyAndSubkeyNames(string section)
        {
            if(String.IsNullOrEmpty(RegKey))
            {
                throw new Exception("The RegKey property must be set.");
            }
            string key = RegKey;

            if(!String.IsNullOrEmpty(section))
            {
                key += "\\";
                key += section;
            }

            return key;
        }
    }
}