﻿using System;
using System.Linq;

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
            MainSensorLastDate < DateTime.Now.AddHours(-1);

        public bool IsWarningSecond =>
            DualMode == true && SecondSensorLastDate < DateTime.Now.AddHours(-1);

        public bool IsWarning =>
            IsWarningMain || IsWarningSecond;

        public string MainSensorGuid =>
            GetSensorGuid(MainDeviceGuid, MainIZKId, MainSensorId);

        public string SecondSensorGuid =>
            GetSensorGuid(SecondDeviceGuid, SecondIZKId, SecondSensorId);
    }
}
