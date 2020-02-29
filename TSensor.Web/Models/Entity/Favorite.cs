using System;
using System.Collections.Generic;

namespace TSensor.Web.Models.Entity
{
    public class Favorite
    {
        public Guid FavoriteGuid { get; set; }
        public string Name { get; set; }

        public IEnumerable<Guid> ItemList { get; set; }
    }
}
