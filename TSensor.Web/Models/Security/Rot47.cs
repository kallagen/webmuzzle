namespace TSensor.Web.Models.Security
{
    public static class Rot47
    {
        private const string ENCODE = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        private const string DECODE = "PQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNO";

        public static string Encode(string str)
        {
            if (str == null) { return null; }

            var result = string.Empty;
            for (var i = 0; i < str.Length; i++)
            {
                var idx = ENCODE.IndexOf(str[i]);
                if (idx == -1)
                {
                    result += str[i];
                }
                else
                {
                    result += DECODE[idx];
                }
            }
            return result;
        }

        public static string Decode(string str)
        {
            if (str == null) { return null; }

            var result = string.Empty;
            for (var i = 0; i < str.Length; i++)
            {
                var idx = DECODE.IndexOf(str[i]);
                if (idx == -1)
                {
                    result += str[i];
                }
                else
                {
                    result += ENCODE[idx];
                }
            }
            return result;
        }
    }
}
