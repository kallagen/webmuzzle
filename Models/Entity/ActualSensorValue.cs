using System;

namespace TSensor.Web.Models.Entity
{
    public class ActualSensorValue
    {
        public string SensorGuid { get; set; }
        public long SensorValueId { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime EventDate { get; set; }
        public string Value { get; set; }
    }
}
