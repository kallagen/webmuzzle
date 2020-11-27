namespace TSensor.Web.Models.Services.Sms
{
    public interface ISmsServiceProvider
    {
        public void Send(string message, string senderName, out string request, out string response);
    }
}
