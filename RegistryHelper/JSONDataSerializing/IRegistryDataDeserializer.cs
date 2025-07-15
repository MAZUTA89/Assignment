namespace RegistryHelper.JSONDataSerializing
{
    public interface IRegistryDataDeserializer
    {
        IRegistryData Deserialize(string subKeyPath, string valueName);
    }
}
