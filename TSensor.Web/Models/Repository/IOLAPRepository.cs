using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IOLAPRepository
    {
        public void AggregateAsync();
        public IEnumerable<AggregatedSensorValue> GetAggregatedSensorValues(IEnumerable<Guid> itemList,
            DateTime dateStart, DateTime dateEnd, string paramName, string additionalParamName);
        public Dictionary<dynamic, IEnumerable<dynamic>> GetSensorValues(IEnumerable<Guid> tankGuidList,
            DateTime dateStart, DateTime dateEnd, string paramName, string additionalParamName);
    }
}