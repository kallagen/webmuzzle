using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TSensor.Web.Models.Services.Sms.Provider
{
    public class SmsgorodProvider : ISmsServiceProvider
    {
        private const string URL = "https://new.smsgorod.ru/apiSms/create";

        private readonly string apiKey;
        private readonly string phone;

        public SmsgorodProvider(IConfiguration configuration)
        {
            apiKey = configuration["smsApiKey"];

            phone = configuration["smsPhoneNumber"];
            phone = Regex.Replace(phone, "[^\\d]", string.Empty);
            if (phone.StartsWith("8"))
            {
                phone = $"7{phone.Substring(1)}";
            }
        }

        private string HttpSend(string json)
        {

            var request = WebRequest.Create(URL) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Timeout = 10000;
            request.MaximumAutomaticRedirections = 5;
            request.AllowAutoRedirect = true;
            request.Headers.Add("Upgrade-Insecure-Requests", "1");

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(json);
                writer.Flush();
                writer.Close();
            }

            var response = request.GetResponse() as HttpWebResponse;
            using var reader = new StreamReader(response.GetResponseStream());

            return reader.ReadToEnd();
        }

        public void Send(string message, out string request, out string response)
        {
            var requestData = new
            {
                apiKey,
                sms = new[]
                {
                    new
                    {
                        channel = "char",
                        phone,
                        text = message
                    }
                }
            };

            request = JsonSerializer.Serialize(requestData);
            response = HttpSend(request);
        }
    }
}
