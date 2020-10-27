using System.Collections.Generic;

namespace TSensor.Web.Models.Entity
{
    public class MapSettings
    {
        public int MaxZoom { get; set; }
        public string PushpinImage { get; set; }

        public decimal DefaultLongitude { get; set; }
        public decimal DefaultLatitude { get; set; }

        public Dictionary<string, string> PointTypeImageList { get; set; }
    }
}
