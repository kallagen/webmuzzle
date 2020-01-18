using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IPointRepository
    {
        public IEnumerable<Point> List();
        public Point GetPointByGuid(Guid pointGuid);
        public Guid? Create(string name);
        public bool Edit(Guid pointGuid, string name);
        public bool Remove(Guid pointGuid);
    }
}