namespace TSensor.Web.Models.Services.Email
{
    public interface IEmailServiceProvider
    {
        public void Send(string subject, string body);
    }
}