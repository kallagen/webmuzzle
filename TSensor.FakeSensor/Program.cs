using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace TSensor.FakeSensor
{
    class Program
    {
        private static Config config = new Config();

        private static Sensor r92 = new Sensor(SensorType.Tank, ProductType.G92, "00001", 3240, 46.2M);
        private static Sensor r95 = new Sensor(SensorType.Tank, ProductType.G95, "00002", 2400, 10M);
        private static Sensor rdt = new Sensor(SensorType.Tank, ProductType.DT, "00003", 2790, 28.466M);

        private static Sensor rg1 = new Sensor(SensorType.Tank, ProductType.Gas, "00010", 1250, 9M);
        private static Sensor rg2 = new Sensor(SensorType.Tank, ProductType.Gas, "00011", 1250, 9M);
        private static Sensor rg3 = new Sensor(SensorType.Tank, ProductType.Gas, "00012", 1250, 9M);

        private static Sensor t92 = new Sensor(SensorType.Tanker, ProductType.G92, "00021", 2000, 8.3M);
        private static Sensor t95 = new Sensor(SensorType.Tanker, ProductType.G95, "00022", 2200, 11.1M);
        private static Sensor tdt = new Sensor(SensorType.Tanker, ProductType.DT, "00020", 2200, 10.6M);
        private static Sensor tg = new Sensor(SensorType.Tanker, ProductType.Gas, "00023", 2000, 36);

        private static Sensor s92 = new Sensor(SensorType.Storage, ProductType.G92, "00041", 5560, 104.7M);
        private static Sensor s95 = new Sensor(SensorType.Storage, ProductType.G95, "00042", 5560, 104.7M);
        private static Sensor sdt = new Sensor(SensorType.Storage, ProductType.DT, "00040", 5560, 104.7M);
        private static Sensor sg = new Sensor(SensorType.Storage, ProductType.Gas, "00043", 3000, 100.85M);

        private static BackgroundWorker worker = new BackgroundWorker();

        private static void Main(string[] args)
        {
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            worker.RunWorkerAsync();

            Thread.Sleep(Timeout.Infinite);
        }

        private static void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Thread.Sleep(13000);

            worker.RunWorkerAsync();
        }

        private static async void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            r92.Do();
            r95.Do();
            rdt.Do();
            rg1.Do();
            rg2.Do();
            rg3.Do();
            t92.Do();
            t95.Do();
            tdt.Do();
            tg.Do();
            s92.Do();
            s95.Do();
            sdt.Do();
            sg.Do();

            await r92.SendAsync(config.ApiPushValueUrl);
            await r95.SendAsync(config.ApiPushValueUrl);
            await rdt.SendAsync(config.ApiPushValueUrl);
            await rg1.SendAsync(config.ApiPushValueUrl);
            await rg2.SendAsync(config.ApiPushValueUrl);
            await rg3.SendAsync(config.ApiPushValueUrl);
            await t92.SendAsync(config.ApiPushValueUrl);
            await t95.SendAsync(config.ApiPushValueUrl);
            await tdt.SendAsync(config.ApiPushValueUrl);
            await tg.SendAsync(config.ApiPushValueUrl);
            await s92.SendAsync(config.ApiPushValueUrl);
            await s95.SendAsync(config.ApiPushValueUrl);
            await sdt.SendAsync(config.ApiPushValueUrl);
            await sg.SendAsync(config.ApiPushValueUrl);

            var coordinates = Coordinates.Next;
            var result = await Http.PostAsync(config.ApiPushCoordinatesUrl, new Dictionary<string, string>
            {
                { "d", "00023" },
                { "lon", Math.Round(coordinates.lon, 7).ToString() },
                { "lat", Math.Round(coordinates.lat, 7).ToString() }
            });
        }
    }
}