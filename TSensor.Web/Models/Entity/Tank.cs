using System;

namespace TSensor.Web.Models.Entity
{
    public class Tank
    {
        public Guid TankGuid { get; set; }
        public string Name { get; set; }
        public Guid? ProductGuid { get; set; }
        public string ProductName { get; set; }
        public bool DualMode { get; set; }
        public string MainDeviceGuid { get; set; }
        public int? MainIZKId { get; set; }
        public int? MainSensorId { get; set; }
        public string SecondDeviceGuid { get; set; }
        public int? SecondIZKId { get; set; }
        public int? SecondSensorId { get; set; }
        public string Description { get; set; }
        public decimal? WeightChangeDelta { get; set; }
        public int? WeightChangeTimeout { get; set; }
    }
}
