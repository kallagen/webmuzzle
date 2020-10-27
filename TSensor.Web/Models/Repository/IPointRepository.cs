using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IPointRepository
    {
        public IEnumerable<Point> List();
        public Point GetByGuid(Guid pointGuid);
        public Guid? Create(string name, string address, string phone, string email, string description,
            decimal? longitude, decimal? latitude, Guid? pointTypeGuid);
        public bool Edit(Guid pointGuid, string name, string address, string phone, string email, string description,
            decimal? longitude, decimal? latitude, Guid? pointTypeGuid);
        public bool Remove(Guid pointGuid);
        public IEnumerable<SensorValue> GetNotAssignedSensorState();
        public IEnumerable<TankSensorValue> GetSensorActualState(IEnumerable<Guid> tankGuidList);

        public IEnumerable<Point> GetUserPointList(Guid? userGuid);
    }
}