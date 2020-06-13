using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TSensor.Web.Models.Middleware
{
    public class RequestModifyMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestModifyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method?.ToUpperInvariant() == "POST" &&
                !context.Request.Path.StartsWithSegments("/broadcast") &&
                !context.Request.Path.StartsWithSegments("/tank/calibration/upload"))
            {
                using (var reader = new StreamReader(context.Request.Body))
                {
                    var content = await reader.ReadToEndAsync();
                    var query = QueryHelpers.ParseNullableQuery(content);

                    if (query != null)
                    {
                        var modifiedQuery = new QueryBuilder();
                        foreach (var item in query.SelectMany(p => p.Value, (k, v) => new { key = k.Key, value = v }))
                        {
                            modifiedQuery.Add(item.key,
                                item.key == "__RequestVerificationToken" ? item.value : item.value.ToUpperInvariant());
                        }

                        var modifiedContent = new StringContent(modifiedQuery.ToQueryString().ToString().Substring(1));
                        context.Request.Body = await modifiedContent.ReadAsStreamAsync();
                    }
                }
            }

            await _next.Invoke(context);
        }
    }
}
