namespace TSensor.Proxy.Logger
{
    public interface ILogger
    {
        public void Log(string message, string prefix = null, Elapsed elapsed = null, bool isError = false);
    }
}
