using System.Linq;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.ViewModels
{
    public class SearchRemovableViewModel<T> : SearchViewModel<T> where T: IRemovableEntity
    {
        public bool HasRemovedRecord =>
            Data.Any(p => p.IsRemoved);

        public bool ShowRemoved { get; set; }
    }
}
