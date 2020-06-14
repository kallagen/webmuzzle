using System;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.ViewModels.Tank
{
    public class TankDetailsViewModel : ViewModelBase
    {
        public Guid TankGuid { get; set; }
        public string TankName { get; set; }
        public bool DualMode { get; set; }
        public string PointName { get; set; }
        public string ProductName { get; set; }
        public ActualSensorValue Value { get; set; }
        public bool HasCalibrationRights { get; set; }
        public bool HasCalibrationData { get; set; }
        public bool IsMassmeter { get; set; }
    }
}
