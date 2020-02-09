using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Repository;

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
            var currentUserGuid = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value);
            return _pointGroupRepository.GetPointGroupStructure(currentUserGuid);
        }

        public static readonly Guid NotAssignedSensorGroupGuid = new Guid("00000000-0000-0000-0000-000000000001");

        public IViewComponentResult Invoke()
        {
            var pointStructure = GetPointGroupStructure();

            if (HttpContext.User.IsInRole("admin"))
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
