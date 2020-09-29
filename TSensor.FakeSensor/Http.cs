using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TSensor.FakeSensor
{
    public static class Http
    {
        public static async Task<string> PostAsync(string url, Dictionary<string, string> param)
        {
            HttpWebRequest request;
            HttpWebResponse response;

            try
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 10000;
                request.MaximumAutomaticRedirections = 5;
                request.AllowAutoRedirect = true;
                request.Headers.Add("Upgrade-Insecure-Requests", "1");

                var body = string.Join("&", param.Select(p => $"{p.Key}={WebUtility.UrlEncode(p.Value)}"));
                var postBytes = Encoding.ASCII.GetBytes(body);
                request.ContentLength = postBytes.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(postBytes, 0, postBytes.Length);
                    stream.Close();
                }

                response = await request.GetResponseAsync() as HttpWebResponse;
                using var reader = new StreamReader(response.GetResponseStream());

                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                request = null;
                response = null;
            }
        }

        public static string Post(string url, Dictionary<string, string> param)
        {
            HttpWebRequest request;
            HttpWebResponse response;

            try
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 10000;
                request.MaximumAutomaticRedirections = 5;
                request.AllowAutoRedirect = true;
                request.Headers.Add("Upgrade-Insecure-Requests", "1");

                var body = string.Join("&", param.Select(p => $"{p.Key}={WebUtility.UrlEncode(p.Value)}"));
                var postBytes = Encoding.ASCII.GetBytes(body);
                request.ContentLength = postBytes.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(postBytes, 0, postBytes.Length);
                    stream.Close();
                }

                response = request.GetResponse() as HttpWebResponse;
                using var reader = new StreamReader(response.GetResponseStream());

                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                request = null;
                response = null;
            }
        }
    }
}