using System;

namespace TSensor.Web.Models.Security
{
    public class License
    {
        public Guid LicenseGuid { get; set; }
        public DateTime ExpireDate { get; set; }
        public int SensorCount { get; set; }

        public bool WrongLicense { get; set; }
    }
}
