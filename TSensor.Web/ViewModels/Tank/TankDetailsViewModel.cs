using System;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.ViewModels.Tank
{
    public class TankDetailsViewModel
    {
        public Guid TankGuid { get; set; }
        public string TankName { get; set; }
        public bool DualMode { get; set; }
        public string ProductName { get; set; }
        public ActualSensorValue MainValue { get; set; }
        public ActualSensorValue SecondValue { get; set; }
    }
}
