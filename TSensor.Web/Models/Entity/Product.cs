using System;

namespace TSensor.Web.Models.Entity
{
    public class Product
    {
        public Guid ProductGuid { get; set; }
        public string Name { get; set; }
        public bool IsGas { get; set; }
    }
}