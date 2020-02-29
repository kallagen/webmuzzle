using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.ViewModels.Dashboard
{
    public class ActualSensorValuesViewModel : ViewModelBase
    {
        public IEnumerable<TankSensorValue> ActualSensorValueList { get; set; }
        public Favorite Favorite { get; set; }
    }
}
