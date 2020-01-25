using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IBroadcastRepository
    {
        public IEnumerable<ActualSensorValue> GetActualSensorValues();
        public IEnumerable<ActualSensorValue> GetSensorActualState(Guid? pointGuid = null);
    }
}
