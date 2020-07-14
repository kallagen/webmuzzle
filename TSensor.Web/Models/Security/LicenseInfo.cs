using System.Text.Json;

namespace TSensor.Web.Models.Security
{
    public class LicenseInfo
    {
        public string d { get; set; }
        public string s { get; set; }

        public string Data => d;
        public string Sign => s;

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
