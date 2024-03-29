﻿using System;
using System.Collections.Generic;
using System.Linq;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Services.Security;

namespace TSensor.Web.Models.Repository
{
    public class PointRepository : RepositoryBase, IPointRepository
    {
        public PointRepository(string connectionString) : base(connectionString) { }

		public static readonly Guid MASSMETER_POINT_GUID = new Guid("00000000-0000-0000-0000-999999999999");

		public IEnumerable<Point> List()
        {
            return Query<Point>($@"
                SELECT PointGuid, p.[Name], Address, Phone, Email, Description,
					Longitude, Latitude,
					p.PointTypeGuid, pt.Name AS PointTypeName
                FROM [Point] p
					LEFT JOIN PointType pt ON p.PointTypeGuid = pt.PointTypeGuid
				WHERE PointGuid != '{MASSMETER_POINT_GUID}'
				ORDER BY [Name]");
        }

        public Point GetByGuid(Guid pointGuid)
        {
			if (pointGuid == MASSMETER_POINT_GUID)
            {
				return null;
            }

			var point = QueryFirst<Point>(@"
                SELECT PointGuid, [Name], Address, Phone, Email, Description,
					Longitude, Latitude, PointTypeGuid
                FROM [Point] WHERE PointGuid = @pointGuid", new { pointGuid });

			if (point != null)
			{
				var userList = Query<dynamic>($@"
                    SELECT u.UserGuid, u.FirstName, u.LastName, u.Patronymic,
						u.Login, u.Description, upr.PointGuid
                    FROM [User] u 
                        LEFT JOIN UserPointRights upr ON 
                            upr.UserGuid = u.UserGuid AND upr.PointGuid = @pointGuid
                    WHERE u.[Role] = '{RoleCollection.Operator}'",
					new { pointGuid });

				point.UserList = userList.Where(p => p.PointGuid != null)
					.Select(p => User.From(p) as User);
				point.AvailableUserList = userList.Where(p => p.PointGuid == null)
					.Select(p => User.From(p) as User);
			}

			return point;
		}

        public Guid? Create(string name, string address, string phone, string email, string description,
			decimal? longitude, decimal? latitude, Guid? pointTypeGuid)
        {
            return QueryFirst<Guid?>(@"
                DECLARE @guid UNIQUEIDENTIFIER = NEWID()

                INSERT [Point](PointGuid, [Name], [Address], Phone, Email, Description, Longitude, Latitude, PointTypeGuid)
                VALUES(@guid, @name, @address, @phone, @email, @description, @longitude, @latitude, @pointTypeGuid)
                
                SELECT PointGuid FROM [Point] WHERE PointGuid = @guid",
                new { name, address, phone, email, description, longitude, latitude, pointTypeGuid });
        }

        public bool Edit(Guid pointGuid, string name, string address, string phone, string email, string description,
			decimal? longitude, decimal? latitude, Guid? pointTypeGuid)
        {
			if (pointGuid == MASSMETER_POINT_GUID)
            {
				return false;
            }

			return QueryFirst<int?>(@"
                UPDATE [Point] SET 
                    [Name] = @name,
					[Address] = @address,
					Phone = @phone,
					Email = @email,
					Description = @description,
					Longitude = @longitude,
					Latitude = @latitude,
					PointTypeGuid = @pointTypeGuid
                WHERE PointGuid = @pointGuid

                SELECT @@ROWCOUNT",
				new { pointGuid, name, address, phone, email, description, longitude, latitude, pointTypeGuid }) == 1;
        }

        public bool Remove(Guid pointGuid)
        {
			if (pointGuid == MASSMETER_POINT_GUID)
			{
				return false;
			}

			return QueryFirst<int?>(@"
                DELETE [Point] 
                WHERE PointGuid = @pointGuid
                    
                SELECT @@ROWCOUNT",
                new { pointGuid }) == 1;
        }

        public IEnumerable<SensorValue> GetNotAssignedSensorState()
        {
			return Query<SensorValue>(@"
				SELECT 
					InsertDate,
					DeviceGuid,
					izkNumber AS IzkNumber,
					sensorSerial AS SensorSerial,
			        environmentLevel AS EnvironmentLevel,
					levelInPercent AS LevelInPercent,
					environmentVolume AS EnvironmentVolume,
					liquidEnvironmentLevel AS LiquidEnvironmentLevel,
					liquidDensity AS LiquidDensity,
					t1 AS T1,
					t2 AS T2,
					t3 AS T3,
					t4 AS T4,
					t5 AS T5,
					t6 AS T6
				FROM ActualSensorValue asv
				WHERE NOT EXISTS(
					SELECT 1 
					FROM Tank t
					WHERE 
						(t.MainDeviceGuid = asv.DeviceGuid AND t.MainIZKId = asv.izkNumber AND t.MainSensorId = asv.sensorSerial) OR
						(t.DualMode = 1 AND t.SecondDeviceGuid = asv.DeviceGuid AND t.SecondIZKId = asv.izkNumber AND t.SecondSensorId = asv.sensorSerial)) AND
						InsertDate >= DATEADD(MINUTE, -1, GETDATE())");
		}

		public IEnumerable<TankSensorValue> GetSensorActualState(IEnumerable<Guid> tankGuidList)
        {
            return Query<TankSensorValue>(@"
				SELECT
					p.Name AS PointName, t.TankGuid, t.Name AS TankName, t.DualMode AS DualMode, pr.Name AS ProductName,
					t.MainDeviceGuid, t.MainIZKId, t.MainSensorId,
					t.WeightChangeDelta, t.WeightChangeTimeout,
					m.InsertDate AS MainSensorInsertDate,
					m.environmentLevel AS EnvironmentLevel,
					m.levelInPercent AS LevelInPercent,
					m.environmentVolume AS EnvironmentVolume,
					m.liquidEnvironmentLevel AS LiquidEnvironmentLevel,
					m.liquidDensity AS LiquidDensity,
					m.pressureFilter AS PressureFilter,
					m.t1,
					m.t2,
					m.t3,
					m.t4,
					m.t5,
					m.t6,
					m.steamMass AS SteamMass,
					m.environmentComposition AS EnvironmentComposition,
					m.pressureMeasuring AS PressureMeasuring,
					pr.IsGas
				FROM Point p
					JOIN Tank t ON p.PointGuid = t.PointGuid
					LEFT JOIN Product pr ON t.ProductGuid = pr.ProductGuid
					LEFT JOIN ActualSensorValue m ON t.MainDeviceGuid = m.DeviceGuid AND t.MainIZKId = m.izkNumber AND t.MainSensorId = m.sensorSerial
				WHERE
					t.TankGuid in @tankGuidList", new { tankGuidList = tankGuidList.Where(p => p != null).Distinct() });
        }

        public IEnumerable<Point> GetUserPointList(Guid? userGuid)
        {
			var pointList = Query<Point>(@"
				SELECT DISTINCT p.PointGuid, p.Name, p.Latitude, p.Longitude, p.PointTypeGuid
				FROM Point p
					LEFT JOIN PointGroupPoint pgp ON p.PointGuid = pgp.PointGuid
					LEFT JOIN UserPointGroupRights upgp ON upgp.PointGroupGuid = pgp.PointGroupGuid AND upgp.UserGuid = @userGuid
					LEFT JOIN UserPointRights upr ON p.PointGuid = upr.PointGuid AND upr.UserGuid = @userGuid
				WHERE @userGuid IS NULL OR (upgp.UserGuid IS NOT NULL OR upr.UserGuid IS NOT NULL)", new { userGuid });

			var tankList = Query<Tank>(@"
				SELECT TankGuid, PointGuid, t.Name, p.Name AS ProductName
				FROM Tank t
					LEFT JOIN Product p ON t.ProductGuid = p.ProductGuid
				WHERE t.PointGuid IN @pointList", new { pointList = pointList.Select(p => p.PointGuid) });

			foreach (var tank in tankList)
            {
				pointList.FirstOrDefault(p => p.PointGuid == tank.PointGuid).TankList.Add(tank);				
            }

			return pointList;
        }
    }
}