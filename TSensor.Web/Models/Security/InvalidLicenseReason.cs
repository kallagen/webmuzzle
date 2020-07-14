namespace TSensor.Web.Models.Security
{
    public enum InvalidLicenseReason : int
    {
        AllFine = 0,
        NotFound = 1,
        Corrupted = 2,
        Expired = 3,
        MaxSensorCount = 4
    }
}
