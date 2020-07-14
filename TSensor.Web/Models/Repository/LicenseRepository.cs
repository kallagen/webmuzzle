namespace TSensor.Web.Models.Repository
{
    public class LicenseRepository : RepositoryBase, ILicenseRepository
    {
        public LicenseRepository(string connectionString) : base(connectionString) { }

        public int GetTankCount()
        {
            return QueryFirst<int>("SELECT COUNT(*) FROM Tank");
        }
    }
}
