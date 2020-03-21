using System;
using System.Collections.Generic;

namespace TSensor.Web.ViewModels.Chart
{
    public class ChartViewModel : ViewModelBase
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string MainParam { get; set; }
        public string AdditionalParam { get; set; }
        public IEnumerable<Guid> TankGuidList { get; set; }
        public IEnumerable<dynamic> Values { get; set; }

        public static readonly Dictionary<string, string> ParamList = new Dictionary<string, string>
        {
            { "WEIGHT", "Масса" },
            { "VOLUME", "Объём" },
            { "DENSITY", "Плотность" },
            { "TEMPERATURE", "Температура" },
            { "LEVEL", "Уровень" }
        };
    }
}