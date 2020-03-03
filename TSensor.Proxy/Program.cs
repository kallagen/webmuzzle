namespace TSensor.Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Config();
            
            var serialService = new SerialService(config);
            serialService.Refresh();

            while (true) { }
        }
    }
}