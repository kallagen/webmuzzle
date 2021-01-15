using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TSensor.Proxy.Http
{
    public static class Http
    {
        public static async Task<HttpResult> PostAsync(string url, Dictionary<string, string> param)
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
                var sas = reader.ReadToEnd(); //TODO убрать
                return new HttpResult
                {
                    Content = reader.ReadToEnd()
                };
            }
            catch (Exception ex)
            {
                return new HttpResult
                {
                    Exception = ex
                };
            }
            finally
            {
                request = null;
                response = null;
            }
        }
        
        public static async Task<HttpResult> GetAsync(string url, Dictionary<string, string> param)
        {
            HttpWebRequest request;
            HttpWebResponse response;

            try
            {
                var args = string.Join("&", param.Select(p => $"{p.Key}={WebUtility.UrlEncode(p.Value)}"));

                request = WebRequest.Create(url + "?" +  args) as HttpWebRequest;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 10000;
                request.MaximumAutomaticRedirections = 5;
                request.AllowAutoRedirect = true;
                request.Headers.Add("Upgrade-Insecure-Requests", "1");

                response = await request.GetResponseAsync() as HttpWebResponse;
                using var reader = new StreamReader(response.GetResponseStream());
                var sas = reader.ReadToEnd(); //TODO убрать
                return new HttpResult
                {
                    Content = reader.ReadToEnd()
                };
            }
            catch (Exception ex)
            {
                return new HttpResult
                {
                    Exception = ex
                };
            }
            finally
            {
                request = null;
                response = null;
            }
        }

        public static async Task<HttpResult> PostFileAsync(string url, Dictionary<string, string> param, Stream fileStream = null)
        {
            try
            {
                using var client = new HttpClient
                {
                    Timeout = new TimeSpan(0, 5, 0)
                };

                var form = new MultipartFormDataContent();
                if (param?.Any() == true)
                {
                    foreach (var p in param)
                    {
                        form.Add(new StringContent(p.Value), p.Key);
                    }
                }
                if (fileStream != null)
                {
                    form.Add(new StringContent("file"), "file");

                    HttpContent fileContent = new StreamContent(fileStream);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "file",
                        FileName = "file"
                    };
                    form.Add(fileContent);
                }

                using var response = await client.PostAsync(url, form);
                return new HttpResult
                {
                    Content = await response.Content.ReadAsStringAsync()
                };
            }
            catch (Exception ex)
            {
                return new HttpResult
                {
                    Exception = ex
                };
            }          
        }
    }
}