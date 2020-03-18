using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace TSensor.Web.Controllers
{
    [Authorize]
    public class ChartController : Controller
    {
        [Route("chart")]
        [HttpPost]
        public IActionResult Index(IEnumerable<Guid> guidList)
        {
            return View();
        }
    }
}