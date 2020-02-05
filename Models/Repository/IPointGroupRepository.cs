using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IPointGroupRepository
    {
        public IEnumerable<PointGroup> List();
        public PointGroup GetByGuid(Guid pointGroupGuid);
        public Guid? Create(string name);
        public bool Edit(Guid pointGroupGuid, string name);
        public bool Remove(Guid pointGroupGuid);
    }
}
