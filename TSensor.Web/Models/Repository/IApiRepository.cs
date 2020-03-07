using System.Collections.Generic;
using System.Threading.Tasks;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IApiRepository
    {
        public Task<bool> PushValueAsync(string ip, ActualSensorValue value, string raw);
        public Task PushArchivedValuesAsync(string ip, string deviceGuid, IEnumerable<ActualSensorValue> valueList);
    }
}
