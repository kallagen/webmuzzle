namespace TSensor.License.Models
{
    public class LicenseInfo
    {
        public string d { get; set; }
        public string s { get; set; }

        public string Data => d;
        public string Sign => s;
    }
}
