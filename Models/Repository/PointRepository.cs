using System;
using System.Collections.Generic;
using System.Linq;
using TSensor.Web.Models.Entity;
using TSensor.Web.Models.Services.Security;

namespace TSensor.Web.Models.Repository
{
    public class PointRepository : RepositoryBase, IPointRepository
    {
        public PointRepository(string connectionString) : base(connectionString) { }

        public IEnumerable<Point> List()
        {
            return Query<Point>(@"
                SELECT PointGuid, [Name], Address, Phone, Email, Description
                FROM [Point]
                ORDER BY [Name]");
        }

        public Point GetByGuid(Guid pointGuid)
        {
			var point = QueryFirst<Point>(@"
                SELECT PointGuid, [Name], Address, Phone, Email, Description
                FROM [Point] WHERE PointGuid = @pointGuid", new { pointGuid });

			if (point != null)
			{
				var userList = Query<dynamic>($@"
                    SELECT u.UserGuid, u.Name, u.Login, u.Description, upr.PointGuid
                    FROM [User] u 
                        LEFT JOIN UserPointRights upr ON 
                            upr.UserGuid = u.UserGuid AND upr.PointGuid = @pointGuid
                    WHERE u.[Role] = '{RoleCollection.Operator}'",
					new { pointGuid });

				point.UserList = userList.Where(p => p.PointGuid != null)
					.Select(p => new User { UserGuid = p.UserGuid, Name = p.Name, Login = p.Login, 
						Description = p.Description });
				point.AvailableUserList = userList.Where(p => p.PointGuid == null)
					.Select(p => new User { UserGuid = p.UserGuid, Name = p.Name, Login = p.Login,
						Description = p.Description });
			}

			return point;
		}

        public Guid? Create(string name, string address, string phone, string email, string description)
        {
            return QueryFirst<Guid?>(@"
                DECLARE @guid UNIQUEIDENTIFIER = NEWID()

                INSERT [Point](PointGuid, [Name], [Address], Phone, Email, Description)
                VALUES(@guid, @name, @address, @phone, @email, @description)
                
                SELECT PointGuid FROM [Point] WHERE PointGuid = @guid",
                new { name, address, phone, email, description });
        }

        public bool Edit(Guid pointGuid, string name, string address, string phone, string email, string description)
        {
            return QueryFirst<int?>(@"
                UPDATE [Point] SET 
                    [Name] = @name,
					[Address] = @address,
					Phone = @phone,
					Email = @email,
					Description = @description
                WHERE PointGuid = @pointGuid

                SELECT @@ROWCOUNT",
                new { pointGuid, name, address, phone, email, description }) == 1;
        }

        public bool Remove(Guid pointGuid)
        {
            return QueryFirst<int?>(@"
                DELETE [Point] 
                WHERE PointGuid = @pointGuid
                    
                SELECT @@ROWCOUNT",
                new { pointGuid }) == 1;
        }

        public IEnumerable<PointTankInfo> GetAllPointInfo()
        {
            return Query<PointTankInfo>(@"
                SELECT
                    p.PointGuid, p.Name AS PointName,
					t.TankGuid, t.Name AS TankName, t.DualMode AS DualMode,
					m.InsertDate AS MainSensorLastDate, s.InsertDate AS SecondSensorLastDate
                FROM Point p
                    LEFT JOIN Tank t ON p.PointGuid = t.PointGuid
                    LEFT JOIN ActualSensorValue m ON t.TankGuid = m.TankGuid AND m.IsSecond = 0
                    LEFT JOIN ActualSensorValue s ON t.TankGuid = s.TankGuid AND t.DualMode = 1 AND s.IsSecond = 1");
        }

        public IEnumerable<ActualSensorValue> GetNotAssignedSensorState()
        {
			return Query<ActualSensorValue>(@"
				SELECT 
					InsertDate
					DeviceGuid,
					izkNumber,
					banderolType,
					sensorSerial,
					sensorChannel,
					pressureAndTempSensorState,
					sensorFirmwareVersionAndReserv,
					alarma,
					environmentLevel,
					pressureFilter,
					pressureMeasuring,
					levelInPercent,
					environmentVolume,
					liquidEnvironmentLevel,
					steamMass,
					liquidDensity,
					steamDensity,
					dielectricPermeability,
					dielectricPermeability2,
					t1,
					t2,
					t3,
					t4,
					t5,
					t6,
					plateTemp,
					[period],
					plateServiceParam,
					environmentComposition,
					cs1,
					plateServiceParam2,
					plateServiceParam3,
					sensorWorkMode,
					plateServiceParam4,
					plateServiceParam5,
					crc
				FROM ActualSensorValue asv
				WHERE NOT EXISTS(
					SELECT 1 
					FROM Tank t
					WHERE 
						(t.MainDeviceGuid = asv.DeviceGuid AND t.MainIZKId = asv.izkNumber AND t.MainSensorId = asv.sensorSerial) OR
						(t.SecondDeviceGuid = asv.DeviceGuid AND t.SecondIZKId = asv.izkNumber AND t.SecondSensorId = asv.sensorSerial AND t.DualMode = 1))");
        }

        public IEnumerable<ActualSensorValue> GetSensorActualState(IEnumerable<Guid?> tankGuidList)
        {
            return Query<ActualSensorValue>(@"
				SELECT 
					p.PointGuid, p.Name AS PointName,
					t.TankGuid, t.Name AS TankName, t.DualMode AS DualMode,
					m.InsertDate AS MainSensorLastDate, s.InsertDate AS SecondSensorLastDate,
					t.MainDeviceGuid, t.MainIZKId, t.MainSensorId,
					t.SecondDeviceGuid, t.SecondIZKId, t.SecondSensorId,

					m.DeviceGuid,
					m.izkNumber,
					m.banderolType,
					m.sensorSerial,
					m.sensorChannel,
					m.pressureAndTempSensorState,
					m.sensorFirmwareVersionAndReserv,
					m.alarma,
					m.environmentLevel,
					m.pressureFilter,
					m.pressureMeasuring,
					m.levelInPercent,
					m.environmentVolume,
					m.liquidEnvironmentLevel,
					m.steamMass,
					m.liquidDensity,
					m.steamDensity,
					m.dielectricPermeability,
					m.dielectricPermeability2,
					ISNULL(s.t1, m.t1) AS t1,
					ISNULL(s.t2, m.t2) AS t2,
					ISNULL(s.t3, m.t3) AS t3,
					ISNULL(s.t4, m.t4) AS t4,
					ISNULL(s.t5, m.t5) AS t5,
					ISNULL(s.t6, m.t6) AS t6,
					m.plateTemp,
					m.[period],
					m.plateServiceParam,
					m.environmentComposition,
					m.cs1,
					m.plateServiceParam2,
					m.plateServiceParam3,
					m.sensorWorkMode,
					m.plateServiceParam4,
					m.plateServiceParam5,
					m.crc
				FROM Point p
					LEFT JOIN Tank t ON p.PointGuid = t.PointGuid
					FULL JOIN ActualSensorValue m ON t.MainDeviceGuid = m.DeviceGuid AND t.MainIZKId = m.izkNumber AND t.MainSensorId = m.sensorSerial
					LEFT JOIN ActualSensorValue s ON t.DualMode = 1 AND t.SecondDeviceGuid = s.DeviceGuid AND t.SecondIZKId = s.izkNumber AND t.SecondSensorId = s.sensorSerial
				WHERE
					t.TankGuid in @tankGuidList", new { tankGuidList = tankGuidList.Where(p => p != null).Distinct() });
        }
    }
}