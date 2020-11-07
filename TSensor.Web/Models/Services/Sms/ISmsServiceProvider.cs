namespace TSensor.Web.Models.Services.Sms
{
    public interface ISmsServiceProvider
    {
        public void Send(string message, out string request, out string response);
    }
}
