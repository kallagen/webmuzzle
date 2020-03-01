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
        public string MainIZKId { get; set; }
        public string MainSensorId { get; set; }
        public string SecondDeviceGuid { get; set; }
        public string SecondIZKId { get; set; }
        public string SecondSensorId { get; set; }
        public string Description { get; set; }
        public decimal? WeightChangeDelta { get; set; }
        public int? WeightChangeTimeout { get; set; }
    }
}
