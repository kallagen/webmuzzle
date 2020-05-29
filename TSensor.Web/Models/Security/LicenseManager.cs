using System;
using System.IO;

namespace TSensor.Web.Models.Security
{
    public class LicenseManager
    {
        private readonly string FileName = "TSensor.Web";

        public int Current
        {
            get
            {
                var current = Rot47.Decode(File.ReadAllText(FileName));
                if (!int.TryParse(current, out var _current))
                {
                    _current = 0;
                }

                return _current;
            }
        }

        public bool IsValid()
        {
            try
            {
                return Current <= 2635200;
            }
            catch { return true; }
        }

        public void Update(int second)
        {
            try
            {
                var current = Current + second;
                File.WriteAllText(FileName, Rot47.Encode(current.ToString()));
            }
            catch { }
        }
    }
}
