using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace RegistryHelper.UserRegistries
{

    public class AppRegistryKey : IAppRegistryKey
    {
        public RegistryKey OpenOrCreateKey(string path)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(path))
            {
                return key;
            }
        }
        public void Save(string valueName, string value, string path)
        {
            using(var key = Registry.CurrentUser.OpenSubKey(path, true))
            {
                key.SetValue(valueName, value, RegistryValueKind.String);
            }
        }
        public string Load(string valueName, string path, string defaultValue = null)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(path))
            {
                return (string)key.GetValue(valueName, defaultValue,
                    RegistryValueOptions.DoNotExpandEnvironmentNames);
            }
        }

        public bool IsExist(string path)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(path))
            {
                return key != null;
            }
        }
    }
}
