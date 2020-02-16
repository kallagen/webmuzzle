using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IPointGroupRepository
    {
        public IEnumerable<PointGroup> List();
        public PointGroup GetByGuid(Guid pointGroupGuid);
        public Guid? Create(string name, string description);
        public bool Edit(Guid pointGroupGuid, string name, string description);
        public bool Remove(Guid pointGroupGuid);
        public bool AddPoint(Guid pointGroupGuid, Guid pointGuid);
        public bool RemovePoint(Guid pointGroupGuid, Guid pointGuid);
        public IEnumerable<PointGroup> GetPointGroupStructure(Guid userGuid);
    }
}
