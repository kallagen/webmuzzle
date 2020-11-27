using System;
using System.Net;
using System.Net.Mail;
using TSensor.Web.Models.Services.Log;

namespace TSensor.Web.Models.Services.Email
{
    public class EmailService
    {
        private readonly FileLogService _logService;
        private readonly IEmailServiceProvider _provider;
        public EmailService(IEmailServiceProvider provider, FileLogService logService)
        {
            _provider = provider;
            _logService = logService;
        }
        
        public void Send(string subject, string body)
        {
            try
            {
                _provider.Send(subject, body);
                _logService.Write(LogCategory.SmsLog, $@"Email message sended");
            }
            catch (Exception ex)
            {
                _logService.Write(LogCategory.SmsException, ex.ToString());
            }
           
        }
        
    }
}