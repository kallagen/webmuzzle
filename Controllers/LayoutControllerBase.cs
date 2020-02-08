using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Repository;

namespace TSensor.Web.Controllers
{
    public class LayoutControllerBase : Controller
    {
        private readonly IPointGroupRepository __pointGroupRepository;
        public LayoutControllerBase(IPointGroupRepository pointGroupRepository)
        {
            __pointGroupRepository = pointGroupRepository;
        }

        private IEnumerable<PointGroup> GetPointGroupStructure()
        {
            var currentUserGuid = Guid.Parse(User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value);
            return __pointGroupRepository.GetPointGroupStructure(currentUserGuid);
        }
    }
}
