using System;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IRepository
    {
        public bool PushValue(string ip, SensorValue value, DateTime eventDate);
    }
}
