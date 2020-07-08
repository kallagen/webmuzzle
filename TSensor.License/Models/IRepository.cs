using System;
using System.Collections.Generic;

namespace TSensor.License.Models
{
    public interface IRepository
    {
        public IEnumerable<License> List();
        public License GetByGuid(Guid licenseGuid);
        public bool Create(License license);
        public bool Remove(Guid licenseGuid);
        public void Activate(Guid licenseGuid, string ip);
    }
}
