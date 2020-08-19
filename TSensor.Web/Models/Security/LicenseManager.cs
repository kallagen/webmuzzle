using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TSensor.Web.Models.Repository;

namespace TSensor.Web.Models.Security
{
    public class LicenseManager
    {
        private readonly string FileName = "TSensor.Web.pdb";

        private string sKey { get; }
        private string sIV { get; }
        private string aPublicKey { get; }

        private string licenseServiceUrl { get; }

        private readonly ILicenseRepository _repository;

        public LicenseManager(IConfiguration configuration, ILicenseRepository repository)
        {
            sKey = configuration["sKey"];
            sIV = configuration["sIV"];
            aPublicKey = configuration["aPublicKey"];

            licenseServiceUrl = configuration["licenseServiceUrl"];
            licenseServiceUrl +=
                (licenseServiceUrl.EndsWith("/") ? string.Empty : "/") +
                "api/activate";

            _repository = repository;
        }

        public bool IsValid(out InvalidLicenseReason reason)
        {
            if (Current == null)
            {
                reason = InvalidLicenseReason.NotFound;
            }
            else if (Current.WrongLicense)
            {
                reason = InvalidLicenseReason.Corrupted;
            }
            else if (DateTime.Now > Current.ExpireDate)
            {
                reason = InvalidLicenseReason.Expired;
            }
            else if (Current.SensorCount != 0 && _repository.GetTankCount() > Current.SensorCount)
            {
                reason = InvalidLicenseReason.MaxSensorCount;
            }
            else
            {
                reason = InvalidLicenseReason.AllFine;
                return true;
            }

            return false;
        }

        private License current;
        public License Current
        {
            get
            {
                if (current == null)
                {
                    if (File.Exists(FileName))
                    {
                        try
                        {
                            var content = File.ReadAllText(FileName);
                            var info = JsonSerializer.Deserialize<LicenseInfo>(content);

                            if (Verify(info.Data, info.Sign))
                            {
                                current = DecryptLicense(info.Data);
                            }
                        }
                        catch 
                        {
                            current = new License
                            {
                                WrongLicense = true
                            };
                        }
                    }
                }

                return current;
            }
        }

        private bool Verify(string data, string signature)
        {
            using var rsa = new RSACryptoServiceProvider()
            {
                PersistKeyInCsp = false
            };
            rsa.FromXmlString(aPublicKey);

            return rsa.VerifyData(
                Convert.FromBase64String(data),
                new SHA256CryptoServiceProvider(),
                Convert.FromBase64String(signature));
        }

        private License DecryptLicense(string data)
        {
            try
            {
                using var aes = Aes.Create();
                aes.Key = Convert.FromBase64String(sKey);
                aes.IV = Convert.FromBase64String(sIV);

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using var memoryStream = new MemoryStream(Convert.FromBase64String(data));
                using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                using var reader = new StreamReader(cryptoStream);
                return JsonSerializer.Deserialize<License>(reader.ReadToEnd());
            }
            catch { return null; }
        }

        public bool Activate(string data)
        {
            try
            {
                var licenseInfo = JsonSerializer.Deserialize<LicenseInfo>(data);

                if (Verify(licenseInfo.Data, licenseInfo.Sign))
                {
                    var license = DecryptLicense(licenseInfo.Data);
                    if (license != null)
                    {
                        if (RemoteActivate(data))
                        {
                            File.WriteAllText(FileName, licenseInfo.ToString());
                            current = license;

                            return true;
                        }                        
                    }
                }
            }
            catch { }

            return false;
        }

        private bool RemoteActivate(string data)
        {
            try
            {
                var request = WebRequest.Create(licenseServiceUrl) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                request.Timeout = 10000;
                request.MaximumAutomaticRedirections = 5;
                request.AllowAutoRedirect = true;
                request.Headers.Add("Upgrade-Insecure-Requests", "1");

                var postBytes = Encoding.UTF8.GetBytes($"data={WebUtility.UrlEncode(data)}");
                request.ContentLength = postBytes.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(postBytes, 0, postBytes.Length);
                    stream.Close();
                }

                var response = request.GetResponse() as HttpWebResponse;
                using var reader = new StreamReader(response.GetResponseStream());

                var content = reader.ReadToEnd();
                return data == content;
            }
            catch
            {
                return false;
            }
        }
    }
}
