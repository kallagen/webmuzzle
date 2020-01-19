using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface ITankRepository
    {
        public IEnumerable<Tank> GetTankListByPoint(Guid pointGuid);
        public Tank GetTankByGuid(Guid tankGuid);
        public Guid? Create(Guid pointGuid, string name, bool dualMode,
            string mainDeviceGuid, string mainIZKId, string mainSensorId,
            string secondDeviceGuid, string secondIZKId, string secondSensorId);
        public bool Edit(Guid tankGuid, string name, bool dualMode,
            string mainDeviceGuid, string mainIZKId, string mainSensorId,
            string secondDeviceGuid, string secondIZKId, string secondSensorId);
        public bool Remove(Guid tankGuid);
    }
}
