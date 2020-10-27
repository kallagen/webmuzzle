 using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.IO;

namespace TSensor.Web.Models.Services
{
    public static class Helpers
    {
        public static long TicksJs(this DateTime date)
        {
            return (date.Ticks - 621355968000000000) / 10000;
        }

        public static bool TryParseDecimal(this string str, out decimal result)
        {
            result = 0;

            switch (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
            {
                case ".": str = str?.Replace(",", "."); break;
                case ",": str = str?.Replace(".", ","); break;
                default: break;
            }

            var parseResult = decimal.TryParse(str, out var _result);
            if (parseResult)
            {
                result = _result;
            }

            return parseResult;
        }

        public static string Base64PngImage(this IFormFile file)
        {
            if (file != null)
            {
                using var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);

                return $"data:image/png;base64,{Convert.ToBase64String(memoryStream.ToArray())}";
            }
            else 
            {
                return null;
            }
        }
    }
}
