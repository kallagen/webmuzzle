using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace TSensor.Web.Models.Services.Email.Provider
{
    public class EmailProvider: IEmailServiceProvider
    {
        private readonly string emailPass;
        private readonly string emailLogin;
        private readonly string emailSMTPServer;
        private readonly string emailReceiver;

        public EmailProvider(IConfiguration configuration)
        {
            emailPass = configuration["emailPass"];
            emailLogin = configuration["emailLogin"];
            emailSMTPServer = configuration["emailSMTPServer"];
            emailReceiver = configuration["emailReceiver"];
            
        }
        
        public void Send(string subject, string body) 
        {
            string to = emailReceiver;
            string from = emailLogin;
            
            MailMessage message = new MailMessage(from, to) {Subject = subject, Body = body};
            SmtpClient client = new SmtpClient(emailSMTPServer)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailLogin, emailPass)
            };

            client.Send(message); // can throw exeption up
        }
    }
}