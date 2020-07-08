using System;

namespace TSensor.License.Models
{
    public class License
    {
        public Guid LicenseGuid { get; set; }
        public string Data { get; set; }
        public string Name { get; set; }
        public DateTime ExpireDate { get; set; }
        public int SensorCount { get; set; }
        public DateTime CreationDateUTC { get; set; }
        public bool IsActivated { get; set; }
        public DateTime? ActivationDateUTC { get; set; }
        public string ActivationIp { get; set; }

        public string CreationDate =>
            CreationDateUTC.ToLocalTime().ToString("dd.MM.yyyy HH:mm");

        public string ActivationDate => ActivationDateUTC.HasValue ?
            ActivationDateUTC.Value.ToLocalTime().ToString("dd.MM.yyyy HH:mm") : null;

        public string ExpireDateStr =>
            ExpireDate.ToString("dd.MM.yyyy");
    }
}