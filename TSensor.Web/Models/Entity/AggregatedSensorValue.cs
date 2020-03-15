using System;

namespace TSensor.Web.Models.Entity
{
    public class AggregatedSensorValue
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public decimal AdditionalValue { get; set; }
    }
}