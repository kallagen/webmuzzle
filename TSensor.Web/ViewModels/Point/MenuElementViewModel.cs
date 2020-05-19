using System;

namespace TSensor.Web.ViewModels.Point
{
    public class MenuElementViewModel
    {
        public Models.Entity.Point Point { get; set; }
        public Guid? GroupGuid { get; set; }

        public int MaxLabelLength { get; set; }
    }
}
