using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IPointTypeRepository
    {
        public IEnumerable<PointType> List();
        public PointType GetByGuid(Guid pointTypeGuid);
        public Guid? Create(string name, string image);
        public bool Edit(Guid pointTypeGuid, string name, string image);
        public bool Remove(Guid pointTypeGuid);
    }
}