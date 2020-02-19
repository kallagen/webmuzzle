using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IPointRepository
    {
        public IEnumerable<Point> List();
        public Point GetByGuid(Guid pointGuid);
        public Guid? Create(string name, string address, string phone, string email, string description);
        public bool Edit(Guid pointGuid, string name, string address, string phone, string email, string description);
        public bool Remove(Guid pointGuid);
        public IEnumerable<PointTankInfo> GetAllPointInfo();
        public IEnumerable<ActualSensorValue> GetSensorActualState(IEnumerable<Guid> tankGuidList);
        public IEnumerable<SensorValue> GetNotAssignedSensorState();
    }
}