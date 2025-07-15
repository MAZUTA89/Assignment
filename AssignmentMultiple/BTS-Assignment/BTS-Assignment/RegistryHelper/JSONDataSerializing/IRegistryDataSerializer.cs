using System;

namespace RegistryHelper.JSONDataSerializing
{
    public interface IRegistryDataSerializer
    {
        void Serialize(IRegistryData registryData, string subKeyPath, string valueName);
    }
}
