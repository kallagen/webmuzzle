﻿using System;

namespace TSensor.Web.Models.Entity
{
    public class ControllerCommand
    {
        public string Command { get; set; }
        public DateTime Date { get; set; }
        public int State { get; set; }
        public string? FailReason { get; set; }
    }
}