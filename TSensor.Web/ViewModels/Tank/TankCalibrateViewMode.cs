using System;
using System.Collections.Generic;
using System.Linq;

namespace TSensor.Web.ViewModels.Tank
{
    public class TankCalibrateViewMode : ViewModelBase
    {
        public Guid TankGuid { get; set; }
        public string TankName { get; set; }
        
        public string PointName { get; set; }
        public string ProductName { get; set; }

        public Guid MassmeterGuid { get; set; }
        public string MassmeterName { get; set; }

        public IEnumerable<dynamic> CalibrationIntervalList { get; set; }

        public bool HasCalibrationInterval => CalibrationIntervalList?.Any() == true;
    }
}
