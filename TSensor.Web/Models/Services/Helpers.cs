using System;
using System.Globalization;

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
    }
}
