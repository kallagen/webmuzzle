using System;
using System.Collections.Generic;

namespace TSensor.License.Models
{
    public class Repository : RepositoryBase, IRepository
    {
        public Repository(string connectionString) : base(connectionString) { }

        public IEnumerable<License> List()
        {
            return Query<License>(@"
                SELECT 
                    l.LicenseGuid, l.Name, l.ExpireDate, l.SensorCount, l.CreationDateUTC,
                    l.IsActivated, a.ActivationDateUTC, a.ActivationIp
                FROM [License] l
                    OUTER APPLY (
                        SELECT TOP 1 ActivationDateUTC, ActivationIp
                        FROM LicenseActivation la
                        WHERE la.LicenseGuid = l.LicenseGuid
                        ORDER BY ActivationDateUTC) a");
        }

        public License GetByGuid(Guid licenseGuid)
        {
            return QueryFirst<License>(@"
                SELECT 
                    LicenseGuid, Name, Data, IsActivated
                FROM [License]
                WHERE LicenseGuid = @licenseGuid", new { licenseGuid });
        }

        public bool Create(License license)
        {
            return QueryFirst<int?>(@"
                INSERT [License](LicenseGuid, Data, [Name], ExpireDate, SensorCount)
                VALUES(@LicenseGuid, @Data, @Name, @ExpireDate, @SensorCount)
                
                SELECT 1 FROM [License] WHERE LicenseGuid = @LicenseGuid", license) == 1;
        }

        public void Activate(Guid licenseGuid, string ip)
        {
            QueryFirst<int>(@"
                UPDATE [License] SET IsActivated = 1
                WHERE LicenseGuid = @licenseGuid
                    
                INSERT LicenseActivation(LicenseGuid, ActivationIp)
                VALUES(@licenseGuid, @ip)", new { licenseGuid, ip });
        }

        public bool Remove(Guid licenseGuid)
        {
            return QueryFirst<int?>(@"
                DELETE [License] 
                WHERE LicenseGuid = @licenseGuid AND IsActivated = 0
                    
                SELECT @@ROWCOUNT",
                new { licenseGuid }) == 1;
        }
    }
}