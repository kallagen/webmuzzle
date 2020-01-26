using System;

namespace TSensor.Web.Models.Services
{
    public static class Helpers
    {
        public static long TicksJs(this DateTime date)
        {
            return (date.Ticks - 621355968000000000) / 10000;
        }
    }
}
