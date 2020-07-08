using System.Collections.Generic;
using System.Linq;

namespace TSensor.License.ViewModels
{
    public class SearchViewModel<T> : ViewModelBase
    {
        public IEnumerable<T> Data { get; set; }

        public bool HasData =>
            Data?.Any() == true;
    }
}
