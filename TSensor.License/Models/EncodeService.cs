using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace TSensor.License.Models
{
    public class EncodeService
    {
        private string sKey { get; }
        private string sIV { get; }
        private string aPrivateKey { get; }

        public EncodeService(IConfiguration configuration)
        {
            sKey = configuration["sKey"];
            sIV = configuration["sIV"];
            aPrivateKey = configuration["aPrivateKey"];
        }

        private string Encrypt(string data)
        {
            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(sKey);
            aes.IV = Convert.FromBase64String(sIV);

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            using (var writer = new StreamWriter(cryptoStream))
            {
                writer.Write(data);
            }

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public Guid DecryptLicenseGuid(string data)
        {
            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(sKey);
            aes.IV = Convert.FromBase64String(sIV);

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var memoryStream = new MemoryStream(Convert.FromBase64String(data));
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cryptoStream);

            return JsonSerializer.Deserialize<License>(reader.ReadToEnd()).LicenseGuid;
        }

        private string Sign(string data)
        {
            using var rsa = new RSACryptoServiceProvider()
            {
                PersistKeyInCsp = false
            };

            rsa.FromXmlString(aPrivateKey);

            return Convert.ToBase64String(rsa.SignData(Convert.FromBase64String(data), new SHA256CryptoServiceProvider()));
        }

        public string EncodeLicense(License license)
        {
            var data = JsonSerializer.Serialize(license);
            var encrypted = Encrypt(data);
           
            return JsonSerializer.Serialize(new
            {
                d = encrypted,
                s = Sign(encrypted)
            });
        }
    }
}