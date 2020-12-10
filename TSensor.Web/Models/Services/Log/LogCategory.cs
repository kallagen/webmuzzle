using System.Runtime.InteropServices;

namespace TSensor.Web.Models.Services.Log
{
    public class LogCategory
    {
        private readonly string _name;

        private LogCategory(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }

        public override bool Equals(object obj)
        {
            var compare = obj as LogCategory;
            return compare != null && _name == compare._name;
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }

        public static readonly LogCategory InputError = new LogCategory("inputerror");
        public static readonly LogCategory RawInput = new LogCategory("rawinput");
        public static readonly LogCategory Exception = new LogCategory("exception");
        public static readonly LogCategory SystemException = new LogCategory("systemexception");
        public static readonly LogCategory SmsLog = new LogCategory("smslog");
        public static readonly LogCategory SmsException = new LogCategory("smsexception");
        public static readonly LogCategory LiquidLevel = new LogCategory("LiquidLevel");
        public static readonly LogCategory EmailLog = new LogCategory("EmailLog");
        public static readonly LogCategory EmailException = new LogCategory("EmailException");

    }
}
