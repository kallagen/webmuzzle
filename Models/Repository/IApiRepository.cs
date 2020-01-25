using System;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IApiRepository
    {
        public bool PushValue(string ip, ActualSensorValue value, DateTime eventDate);
    }
}
