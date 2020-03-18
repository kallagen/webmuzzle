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
        public string SecondDeviceGuid { get; set; }
        public string SecondIZKId { get; set; }
        public string SecondSensorId { get; set; }
        public decimal? WeightChangeDelta { get; set; }
        public int? WeightChangeTimeout { get; set; }

        public DateTime? MainSensorInsertDate { get; set; }
        public string MainSensorInsertDateStr =>
            MainSensorInsertDate?.ToString("dd.MM.yyyy HH:mm:ss");
        public DateTime? SecondSensorInsertDate { get; set; }
        public string SecondSensorInsertDateStr =>
            SecondSensorInsertDate?.ToString("dd.MM.yyyy HH:mm:ss");

        private long? GetWarningDate(DateTime? date)
        {
            return date.HasValue ? (long?)date.Value.AddMinutes(10).TicksJs() : null;
        }
        public long? MainSensorWarningDateTicks =>
            GetWarningDate(MainSensorInsertDate);
        public long? SecondSensorWarningDateTicks =>
            GetWarningDate(SecondSensorInsertDate);

        private string GetSensorGuid(string deviceGuid, string izkGuid, string sensorId)
        {
            return string.Join("_", new[] { deviceGuid, izkGuid, sensorId });
        }
        public string MainSensorGuid =>
            GetSensorGuid(MainDeviceGuid, MainIZKId, MainSensorId);
        public string SecondSensorGuid =>
            GetSensorGuid(SecondDeviceGuid, SecondIZKId, SecondSensorId);

        public bool IsErrorMain =>
            !MainSensorInsertDate.HasValue;
        public bool IsErrorSecond =>
            DualMode == true && !SecondSensorInsertDate.HasValue;
        public bool IsError =>
            IsErrorMain || IsErrorSecond;
        public bool IsGlobalError =>
            IsErrorMain && (DualMode != true || IsErrorSecond);
        public bool IsWarningMain =>
            MainSensorInsertDate < DateTime.Now.AddMinutes(-10);
        public bool IsWarningSecond =>
            DualMode == true && SecondSensorInsertDate < DateTime.Now.AddMinutes(-10);
        public bool IsWarning =>
            IsWarningMain || IsWarningSecond;
    }
}
