using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IProductRepository
    {
        public IEnumerable<Product> List();
        public Product GetByGuid(Guid productGuid);
        public Guid? Create(string name);
        public bool Edit(Guid productGuid, string name);
        public bool Remove(Guid productGuid);
    }
}