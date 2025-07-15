using Microsoft.Win32;
using System;
using System.Collections.Generic;
using RegistryHelper.UserRegistries;
using RegistryHelper.RegistryEncryption;
using Newtonsoft.Json;

namespace RegistryHelper.JSONDataSerializing
{
    public class RegistryDataDSlizer : IRegistryDataDSliser
    {
        IAppRegistryKey _userRegisrtyKey;
        IRegistryEncryption _registryEncryption;
        JsonSerializerSettings _serializerSettings;
        public RegistryDataDSlizer(IAppRegistryKey userRegistryKey,
            IRegistryEncryption registryEncription)
        {
            _userRegisrtyKey = userRegistryKey;
            _registryEncryption = registryEncription;
            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.TypeNameHandling = TypeNameHandling.All;
        }

        public IAppRegistryKey UserRegistryKey => _userRegisrtyKey;

        public IRegistryEncryption RegistryEncryption => _registryEncryption;

        public IRegistryData Deserialize(string subKeyPath, string valueName)
        {
            string loadedData = _userRegisrtyKey.Load(valueName, subKeyPath, null);

            if(loadedData == null)
                return null;

            if (_registryEncryption != null &&
                loadedData != String.Empty)
            {
                loadedData = _registryEncryption.Decrypt(loadedData);
            }


            var regisrtyData = JsonConvert.DeserializeObject<IRegistryData>(loadedData, _serializerSettings);

            return regisrtyData;
        }

        public void Serialize(IRegistryData registryData, string subKeyPath, string valueName)
        {
            string data = JsonConvert.SerializeObject(registryData, _serializerSettings);

            if (_registryEncryption != null)
            {
                data = _registryEncryption.Encrypt(data);
            }

            _userRegisrtyKey.Save(valueName, data, subKeyPath);
        }
    }
}
