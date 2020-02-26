using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Repository;
using TSensor.Web.Models.Services.Security;

namespace TSensor.Web.ViewComponents
{
    public class PointGroupMenu : ViewComponent
    {
        private readonly IPointGroupRepository _pointGroupRepository;
        private readonly IPointRepository _pointRepository;

        public PointGroupMenu(IPointGroupRepository pointGroupRepository, IPointRepository pointRepository)
        {
            _pointGroupRepository = pointGroupRepository;
            _pointRepository = pointRepository;
        }

        private IEnumerable<PointGroup> GetPointGroupStructure()
        {
            return _pointGroupRepository.GetPointGroupStructure(
                !User.IsInRole(RoleCollection.Operator) ? null as Guid? :
                    Guid.Parse(HttpContext.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value));
        }

        public static readonly Guid NotAssignedSensorGroupGuid = new Guid("00000000-0000-0000-0000-000000000001");

        public IViewComponentResult Invoke()
        {
            var pointStructure = GetPointGroupStructure();

            if (HttpContext.User.IsInRole("ADMIN"))
            {
                var notAssignedPointInfo = _pointRepository.GetNotAssignedSensorState();
                if (notAssignedPointInfo.Any())
                {
                    return View(
                        pointStructure.Union(new[] {
                            new PointGroup { PointGroupGuid = NotAssignedSensorGroupGuid, Name = "Датчики без привязки" } }));
                }
            }

            return View(pointStructure);
        }
    }
}
