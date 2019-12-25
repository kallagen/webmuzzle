using System;

namespace TSensor.Web.Models.Repository
{
    public interface IRepository
    {
        public bool PushValue(string ip, string value, DateTime eventDate, string deviceGuid);
    }
}
