using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IFavoriteRepository
    {
        public IEnumerable<Favorite> ListByUser(Guid userGuid);
        public Favorite GetByGuid(Guid favoriteGuid);
        public Guid? Create(Guid userGuid, string name, IEnumerable<Guid> itemList);
        public bool Remove(Guid userGuid, Guid favoriteGuid);
    }
}
