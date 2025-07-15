using System;
using System.Collections.Generic;

namespace RegistryHelper.RegistryEncryption
{
    public interface IRegistryEncryption
    {
        string Encrypt(string data);

        string Decrypt(string data);
    }
}
