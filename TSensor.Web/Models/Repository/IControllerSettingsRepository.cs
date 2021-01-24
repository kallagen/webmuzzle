using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IControllerSettingsRepository
    {
        public IEnumerable<ControllerSettings> List();
        public Izk GetByDeviceGuidAndIzkNum(Guid deviceGuid, int izkNum);
        
        
        public bool SaveSettings(decimal testValue);
        public bool ControllerExist(string deviceGuid);

        public IList<string> GetAllDeviceGuid();
    }
}