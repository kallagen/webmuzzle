using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.ViewModels.Dashboard
{
    public class ActualSensorValuesViewModel : ViewModelBase
    {
        public IEnumerable<ActualSensorValue> ActualSensorValueList { get; set; }
        public Favorite Favorite { get; set; }
    }
}
