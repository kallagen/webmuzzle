using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface ITankRepository
    {
        public IEnumerable<Tank> GetListByPoint(Guid pointGuid);
        public Tank GetByGuid(Guid tankGuid);
        public Guid? Create(Guid pointGuid, string name, Guid? productGuid, bool dualMode,
            string mainDeviceGuid, string mainIZKId, string mainSensorId,
            string secondDeviceGuid, string secondIZKId, string secondSensorId, string description);
        public bool Edit(Guid tankGuid, Guid pointGuid, string name, Guid? productGuid, bool dualMode,
            string mainDeviceGuid, string mainIZKId, string mainSensorId,
            string secondDeviceGuid, string secondIZKId, string secondSensorId, string description);
        public bool Remove(Guid tankGuid, Guid pointGuid);
    }
}
