using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.IO;

namespace RegistryHelper.RegistryEncryption
{
    public class AesEncryption : IRegistryEncryption
    {
        string _key;
         
        public AesEncryption(string key)
        {
            _key = key;
        }

        public string Decrypt(string data)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GenerateKey(_key);
                aes.IV = aes.Key.Take(16).ToArray();
                try
                {
                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream(Convert.FromBase64String(data)))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var reader = new StreamReader(cs))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception($"Ошибка расшифрования данных: {ex.Message}");
                }
            }
        }

        public string Encrypt(string data)
        {
            
            using (Aes aes = Aes.Create())
            {
                aes.Key = GenerateKey(_key); // Метод для генерации ключа
                aes.IV = aes.Key.Take(16).ToArray(); // Первые 16 байт ключа как IV
                try
                {
                    using (var ms = new MemoryStream())
                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(cs))
                    {
                        // Пишем данные в поток шифрования
                        writer.Write(data);

                        // Очень важно: сбрасываем writer, чтобы все данные записались
                        writer.Flush();

                        // Сбрасываем CryptoStream, чтобы зашифрованные данные записались в MemoryStream
                        cs.FlushFinalBlock();

                        // Конвертируем зашифрованные данные в Base64
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception($"Ошибка шифрования данных: {ex.Message}");
                }
                
            }
        }

        byte[] GenerateKey(string key)
        {
            using(var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            }
        }
    }
}
