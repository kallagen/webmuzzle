using System;
using TSensor.Web.Models.Services;

namespace TSensor.Web.Models.Entity
{
    public class TankSensorValue : SensorValue
    {
        public string PointName { get; set; }
        public Guid TankGuid { get; set; }
        public string TankName { get; set; }
        public string ProductName { get; set; }
        public bool DualMode { get; set; }
        public string MainDeviceGuid { get; set; }
        public string MainIZKId { get; set; }
        public string MainSensorId { get; set; }

        public decimal? WeightChangeDelta { get; set; }
        public int? WeightChangeTimeout { get; set; }

        public DateTime? MainSensorInsertDate { get; set; }
        public string MainSensorInsertDateStr =>
            MainSensorInsertDate?.ToString("dd.MM.yyyy HH:mm:ss");

        private long? GetWarningDate(DateTime? date)
        {
            return date.HasValue ? (long?)date.Value.AddMinutes(10).TicksJs() : null;
        }
        public long? MainSensorWarningDateTicks =>
            GetWarningDate(MainSensorInsertDate);

        private string GetSensorGuid(string deviceGuid, string izkGuid, string sensorId)
        {
            return string.Join("_", new[] { deviceGuid, izkGuid, sensorId });
        }
        public string MainSensorGuid =>
            GetSensorGuid(MainDeviceGuid, MainIZKId, MainSensorId);

        public bool IsError =>
            !MainSensorInsertDate.HasValue;

        public bool IsWarning =>
            MainSensorInsertDate < DateTime.Now.AddMinutes(-10);
    }
}
