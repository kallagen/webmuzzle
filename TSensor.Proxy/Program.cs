namespace TSensor.Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Config();
            var logger = new Logger();

            new SerialService(config, logger).Run();
        }
    }
}