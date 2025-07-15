using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegistryHelper.UserRegistries;
using RegistryHelper.JSONDataSerializing;
using RegistryHelper.RegistryEncryption;
using RegistryHelper;

namespace BTS_Assignment.Settings
{
    public class AssignmentSettings
    {
        public IRegistryDataDSliser RegistryDataDSliser { get; private set; }
        public AssignmentSettingsData LoadedData { get; private set; }
        AppRegistryKey _appRegistryKey;
        public const string c_KeyPath = @"Software\BTS-Assignment\Multiple";
        public const string c_Name = "Settings";

        public AssignmentSettings()
        {
            LoadedData = new AssignmentSettingsData();

            _appRegistryKey = new AppRegistryKey();

            if (!_appRegistryKey.IsExist(c_KeyPath))
            {
                _appRegistryKey.OpenOrCreateKey(c_KeyPath);
            }

            RegistryDataDSliser = new RegistryDataDSlizer(new AppRegistryKey(), new AesEncryption(c_Name));
        }

        public bool Load()
        {
            LoadedData = (AssignmentSettingsData)RegistryDataDSliser.Deserialize(c_KeyPath, c_Name);

            if(LoadedData == null)
            {
                LoadedData = new AssignmentSettingsData();
                return false;
            }
            return true;
        }

        public void Save(AssignmentSettingsData assignmentSettingsData)
        {
            RegistryDataDSliser.Serialize(assignmentSettingsData, c_KeyPath, c_Name);
        }
    }
}
