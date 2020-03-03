using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TSensor.Proxy
{
    public static class Http
    {
        public static async Task<string> PostAsync(string url, Dictionary<string, string> param)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 30000;
            request.MaximumAutomaticRedirections = 5;
            request.AllowAutoRedirect = true;
            request.Headers.Add("Upgrade-Insecure-Requests", "1");

            var body = string.Join("&", param.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
            var postBytes = Encoding.ASCII.GetBytes(body);
            request.ContentLength = postBytes.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();
            }

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
