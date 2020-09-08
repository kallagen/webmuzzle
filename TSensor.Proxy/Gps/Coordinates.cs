using System;
using System.Text.RegularExpressions;

namespace TSensor.Proxy.Gps
{
    public class Coordinates
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        private const string REGEX_PATTERN = @"[$]GPGLL,([0-9]{2})([0-9]{2})[.]([0-9]+),N,([0-9]{3})([0-9]{2})[.]([0-9]+),E";

        public static Coordinates Parse(string raw)
        {
            try
            {
                var matches = Regex.Match(raw, REGEX_PATTERN);
                if (matches.Groups.Count == 7)
                {
                    var lat1 = double.Parse(matches.Groups[1].Value);
                    var lat2 = double.Parse(matches.Groups[2].Value);
                    var lat3 = double.Parse(matches.Groups[3].Value.Substring(0, 2));

                    var lon1 = double.Parse(matches.Groups[4].Value);
                    var lon2 = double.Parse(matches.Groups[5].Value);
                    var lon3 = double.Parse(matches.Groups[6].Value.Substring(0, 2));

                    return new Coordinates
                    {
                        Longitude = Math.Round(lon1 + lon2 / 60 + lon3 / 3600, 6),
                        Latitude = Math.Round(lat1 + lat2 / 60 + lat3 / 3600, 6)
                    };
                }
            }
            catch { }

            throw new Exception(@$"coordinates parse error: 
{raw}");
        }

        public override string ToString()
        {
            return $"lon: {Longitude}, lat: {Latitude}";
        }
    }
}
