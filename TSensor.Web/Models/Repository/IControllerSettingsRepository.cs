﻿using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IControllerSettingsRepository
    {
        public ControllerSettings GetSettings();
        public bool SaveSettings(decimal testValue);
        public bool ControllerExist(string deviceGuid);

        public IList<string> GetAllDeviceGuid();
    }
}