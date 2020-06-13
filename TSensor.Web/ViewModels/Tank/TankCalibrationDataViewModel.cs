using System;
using System.Collections.Generic;

namespace TSensor.Web.ViewModels.Tank
{
    public class TankCalibrationDataViewModel
    {
        public Guid TankGuid { get; set; }
        public string TankName { get; set; }
        public string PointName { get; set; }
        public string ProductName { get; set; }

        public Dictionary<int, decimal> CalibrationData { get; set; }
    }
}
