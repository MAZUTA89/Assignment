using System;
using RegistryHelper.UserRegistries;
using RegistryHelper.RegistryEncryption;

namespace RegistryHelper.JSONDataSerializing
{
    public interface IRegistryDataDSliser : IRegistryDataDeserializer, IRegistryDataSerializer
    {
        IAppRegistryKey UserRegistryKey { get; }
        IRegistryEncryption RegistryEncryption { get; }
    }
}
