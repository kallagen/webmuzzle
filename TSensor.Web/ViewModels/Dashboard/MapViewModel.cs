using System.Collections.Generic;

namespace TSensor.Web.ViewModels.Dashboard
{
    public class MapViewModel
    {
        public IEnumerable<Models.Entity.Point> Features { get; set; }

        public decimal? DefaultLon { get; set; }
        public decimal? DefaultLat { get; set; }

        public Models.Entity.MapSettings Settings { get; set; }
    }
}