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
        private static readonly object Locker = new object();

        private static bool isConnectionError = false;
        public static bool IsConnectionError
        {
            get
            {
                return isConnectionError;
            }
            set
            {
                lock (Locker)
                {
                    isConnectionError = value;
                }
            }
        }

        public static async Task<HttpResult> PostAsync(string url, Dictionary<string, string> param)
        {
            try
            {
                var request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 10000;
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

                if (IsConnectionError)
                {
                    IsConnectionError = false;
                }

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return new HttpResult
                    {
                        Content = reader.ReadToEnd()
                    };
                }
            }
            catch (Exception ex)
            {
                if (!isConnectionError)
                {
                    IsConnectionError = true;
                }

                return new HttpResult
                {
                    Exception = ex
                };
            }
        }
    }
}
