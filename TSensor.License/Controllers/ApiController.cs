using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TSensor.License.Models;

namespace TSensor.License.Controllers
{
    public class ApiController : Controller
    {
        private readonly IRepository _repository;
        private readonly EncodeService _encodeService;

        public ApiController(IRepository repository, EncodeService encodeService)
        {
            _repository = repository;
            _encodeService = encodeService;
        }

        [Route("api/activate")]
        [HttpPost]
        public IActionResult Activate(string data)
        {
            try
            {
                var licenseInfo = JsonSerializer.Deserialize<LicenseInfo>(data);
                var licenseGuid = _encodeService.DecryptLicenseGuid(licenseInfo.Data);

                _repository.Activate(licenseGuid, Request.HttpContext.Connection.RemoteIpAddress.ToString());
            }
            catch { }

            return Content(data, "application/json");
        }
    }
}
