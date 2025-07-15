using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;

namespace RegistryHelper.UserRegistries
{
    public interface IAppRegistryKey
    {
        RegistryKey OpenOrCreateKey(string path);
        void Save(string valueName, string value, string path);
        string Load(string valueName, string defaultValue, string path);
        bool IsExist(string path);
    }
}
