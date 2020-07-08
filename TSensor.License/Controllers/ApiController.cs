using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Activate(string d, string s)
        {
            try
            {
                var licenseGuid = _encodeService.DecryptLicenseGuid(d);

                _repository.Activate(licenseGuid, Request.HttpContext.Connection.RemoteIpAddress.ToString());
            }
            catch { }

            return Json(new { d, s });
        }
    }
}
