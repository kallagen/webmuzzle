using System;
using System.Linq;
using TSensor.Web.Models.Services;

namespace TSensor.Web.Models.Entity
{
    public class PointTankInfo
    {
        public Guid? PointGuid { get; set; }
        public string PointName { get; set; }
        public Guid? TankGuid { get; set; }
        public string TankName { get; set; }
        public bool? DualMode { get; set; }
        public DateTime? MainSensorLastDate { get; set; }
        public DateTime? SecondSensorLastDate { get; set; }

        public long? GetWarningDate(DateTime? date)
        {
            return date.HasValue ? (long?)date.Value.AddMinutes(10).TicksJs() : null;
        }
        public long? MainSensorWarningDateTicks =>
            GetWarningDate(MainSensorLastDate);
        public long? SecondSensorWarningDateTicks =>
            GetWarningDate(SecondSensorLastDate);

        public string MainDeviceGuid { get; set; }
        public string MainIZKId { get; set; }
        public string MainSensorId { get; set; }
        public string SecondDeviceGuid { get; set; }
        public string SecondIZKId { get; set; }
        public string SecondSensorId { get; set; }

        protected string GetSensorGuid(string deviceGuid, object izkGuid, object sensorId)
        {
            var items = new[] { deviceGuid, izkGuid?.ToString(), sensorId?.ToString() };
            return items.Any(p => string.IsNullOrWhiteSpace(p)) ? null : string.Join("_", items);
        }

        public bool IsGlobalError =>
            TankGuid.HasValue && IsErrorMain && (DualMode != true || IsErrorSecond);

        public bool IsError =>
            TankGuid.HasValue && (IsErrorMain || IsErrorSecond);

        public bool IsErrorMain =>
            !MainSensorLastDate.HasValue;

        public bool IsErrorSecond =>
            DualMode == true && !SecondSensorLastDate.HasValue;

        public bool IsWarningMain =>
            MainSensorLastDate < DateTime.Now.AddMinutes(-10);

        public bool IsWarningSecond =>
            DualMode == true && SecondSensorLastDate < DateTime.Now.AddMinutes(-10);

        public bool IsWarning =>
            IsWarningMain || IsWarningSecond;

        public string MainSensorGuid =>
            GetSensorGuid(MainDeviceGuid, MainIZKId, MainSensorId);

        public string SecondSensorGuid =>
            GetSensorGuid(SecondDeviceGuid, SecondIZKId, SecondSensorId);
    }
}
