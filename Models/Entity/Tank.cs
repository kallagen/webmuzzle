﻿using System;

namespace TSensor.Web.Models.Entity
{
    public class Tank
    {
        public Guid TankGuid { get; set; }
        public string Name { get; set; }
        public bool DualMode { get; set; }
        public string MainDeviceGuid { get; set; }
        public string MainIZKId { get; set; }
        public string MainSensorId { get; set; }
        public string SecondDeviceGuid { get; set; }
        public string SecondIZKId { get; set; }
        public string SecondSensorId { get; set; }
    }
}