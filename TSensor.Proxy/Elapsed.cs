using System;

namespace TSensor.Proxy
{
    public class Elapsed : IDisposable
    {
        private DateTime? start = DateTime.Now;

        public static Elapsed Create =>
            new Elapsed();

        public override string ToString()
        {
            return Math.Round((DateTime.Now - start.Value).TotalMilliseconds, 0, MidpointRounding.ToPositiveInfinity)
                .ToString();
        }

        public void Dispose()
        {
            start = null;
        }
    }
}
