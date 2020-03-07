using System;
using System.Collections.Generic;
using System.Linq;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public class FavoriteRepository : RepositoryBase, IFavoriteRepository
    {
        public FavoriteRepository(string connectionString) : base(connectionString) { }

        public Guid? Create(Guid userGuid, string name, IEnumerable<Guid> itemList)
        {
            //todo transaction

            var favoriteGuid = QueryFirst<Guid?>(@"
                DECLARE @guid UNIQUEIDENTIFIER = NEWID()

                INSERT Favorite(
                    FavoriteGuid, [Name], UserGuid)
                VALUES(
                    @guid, @name, @userGuid)

                SELECT FavoriteGuid FROM Favorite WHERE FavoriteGuid = @guid",
                new { userGuid, name });

            if (favoriteGuid.HasValue)
            {
                foreach (var itemGuid in itemList)
                {
                    QueryFirst<int?>(@"
                        INSERT FavoriteItem(FavoriteGuid, ItemGuid)
                        VALUES(@favoriteGuid, @itemGuid)

                        SELECT @@ROWCOUNT",
                        new { favoriteGuid, itemGuid });
                }                

                return favoriteGuid;
            }
            else
            {
                return null;
            }
        }

        public Favorite GetByGuid(Guid favoriteGuid)
        {
            var items = Query<dynamic>(@"
                SELECT f.FavoriteGuid, f.[Name], fi.ItemGuid
                FROM Favorite f
                    JOIN FavoriteItem fi ON f.FavoriteGuid = fi.FavoriteGuid
                WHERE f.FavoriteGuid = @favoriteGuid", new { favoriteGuid });

            if (items.Any())
            {
                var el = items.First();

                return new Favorite
                {
                    FavoriteGuid = el.FavoriteGuid,
                    Name = el.Name,
                    ItemList = items.Select(p => (Guid)p.ItemGuid)
                };
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<Favorite> ListByUser(Guid userGuid)
        {
            return Query<Favorite>(@"
                SELECT f.FavoriteGuid, f.[Name]
                FROM Favorite f
                WHERE f.UserGuid = @userGuid AND 
                    EXISTS (SELECT 1 
                        FROM FavoriteItem fi 
                        WHERE fi.FavoriteGuid = f.FavoriteGuid)", new { userGuid });
        }

        public bool Remove(Guid userGuid, Guid favoriteGuid)
        {
            return QueryFirst<int?>(@"
                DELETE Favorite
                WHERE FavoriteGuid = @favoriteGuid AND UserGuid = @userGuid
                    
                SELECT @@ROWCOUNT",
                new { favoriteGuid, userGuid }) > 0;
        }
    }
}
