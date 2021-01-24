using System;

namespace TSensor.Web.ViewModels.Izk
{
    public class IzkModel : SearchViewModel<Models.Entity.Izk>
    {
        public int? IzkNumber { get; set; }
        public Guid DeviceGuid { get; set; }
    }
}