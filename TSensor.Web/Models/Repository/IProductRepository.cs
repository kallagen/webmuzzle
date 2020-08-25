using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IProductRepository
    {
        public IEnumerable<Product> List();
        public Product GetByGuid(Guid productGuid);
        public Guid? Create(string name, bool isGas);
        public bool Edit(Guid productGuid, string name, bool isGas);
        public bool Remove(Guid productGuid);
    }
}