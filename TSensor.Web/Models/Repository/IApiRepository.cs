using System.Threading.Tasks;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IApiRepository
    {
        public Task<bool> PushValueAsync(string ip, ActualSensorValue value, string raw);
    }
}
