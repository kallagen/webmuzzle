using System;
using TSensor.Web.Models.Services.Log;

namespace TSensor.Web.Models.Services.Sms
{
    public class SmsService
    {
        private readonly ISmsServiceProvider _provider;
        private readonly FileLogService _logService;

        public SmsService(ISmsServiceProvider provider, FileLogService logService)
        {
            _provider = provider;
            _logService = logService;
        }

        public void SendSms(string message)
        {
            try
            {
                _provider.Send(message, out var request, out var response);
                _logService.Write(LogCategory.SmsLog, $@"
request: {request}
response: {response}");
            }
            catch (Exception ex)
            {
                _logService.Write(LogCategory.SmsException, ex.ToString());
            }
        }
    }
}
