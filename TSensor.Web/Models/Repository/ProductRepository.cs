using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public class ProductRepository : RepositoryBase, IProductRepository
    {
        public ProductRepository(string connectionString) : base(connectionString) { }

        public IEnumerable<Product> List()
        {
            return Query<Product>(@"
                SELECT ProductGuid, [Name], IsGas
                FROM [Product]
                ORDER BY [Name]");
        }

        public Product GetByGuid(Guid productGuid)
        {
            return QueryFirst<Product>(@"
                SELECT ProductGuid, [Name], IsGas
                FROM Product WHERE ProductGuid = @productGuid", new { productGuid });
        }

        public Guid? Create(string name, bool isGas)
        {
            return QueryFirst<Guid?>(@"
                DECLARE @guid UNIQUEIDENTIFIER = NEWID()

                INSERT Product(ProductGuid, [Name], IsGas)
                VALUES(@guid, @name, @isGas)
                
                SELECT ProductGuid FROM Product WHERE ProductGuid = @guid",
                new { name, isGas });
        }

        public bool Edit(Guid productGuid, string name, bool isGas)
        {
            return QueryFirst<int?>(@"
                UPDATE Product SET 
                    [Name] = @name,
                    IsGas = @isGas
                WHERE ProductGuid = @productGuid

                SELECT @@ROWCOUNT",
                new { productGuid, name, isGas }) == 1;
        }

        public bool Remove(Guid productGuid)
        {
            return QueryFirst<int?>(@"
                DELETE Product 
                WHERE ProductGuid = @productGuid
                    
                SELECT @@ROWCOUNT",
                new { productGuid }) == 1;
        }
	}
}