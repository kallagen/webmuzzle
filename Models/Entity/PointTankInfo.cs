using System;

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

        public bool IsError => 
            TankGuid.HasValue && 
            (!MainSensorLastDate.HasValue || (DualMode == true && !SecondSensorLastDate.HasValue));

        public bool IsWarning
        {
            get
            {
                var hourAgo = DateTime.Now.AddHours(-1);
                return MainSensorLastDate < hourAgo || (DualMode == true && SecondSensorLastDate < hourAgo);
            }
        }
    }
}
