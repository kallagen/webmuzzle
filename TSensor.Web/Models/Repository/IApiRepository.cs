using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IApiRepository
    {
        public Task<bool> PushValueAsync(string ip, ActualSensorValue value, string raw);
        public Task<ActualSensorValue> TakeLastValueAsync(ActualSensorValue value);
        public Task<string> TakePointTankNameFromGuidAsync(string tankGuid);
        public Task PushArchivedValuesAsync(string ip, IEnumerable<ActualSensorValue> valueList);

        public Task<IEnumerable<dynamic>> UploadPointCoordinatesAsync(string deviceGuid,
            decimal longitude, decimal latitude);
        public Task UpdatePointCoordinate(Guid pointGuid, decimal longitude, decimal latitude,
            bool? coordinatesChanged, bool? isMoving, DateTime? lastMovingDateUTC);
    }
}
