using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TSensor.FakeSensor
{
    public class Sensor
    {
        public SensorType Type { get; }
        public ProductType Product { get; }

        public State State { get; set; } = State.Nothing;
        public decimal StateParam { get; set; }

        public Sensor(SensorType type, ProductType product,
            string devGuid, decimal maxLevel, decimal maxVol)
        {
            Type = type;
            Product = product;
            DevGuid = devGuid;
            MaxLevel = maxLevel;
            MaxVol = maxVol;

            switch (type)
            {
                case SensorType.Tank:
                    CurrentVol = MaxVol * 0.95M;
                    State = State.TankDec;
                    break;
                case SensorType.Tanker:
                    CurrentVol = MaxVol * 50 / MaxLevel;
                    break;
                case SensorType.Storage:
                    CurrentVol = MaxVol * 0.95M;
                    break;
            }
        }

        public string DevGuid { get; set; }

        public decimal MaxVol { get; set; }
        public decimal MaxLevel { get; set; }

        public decimal CurrentVol { get; set; }

        private decimal Density =>
            Product switch
            {
                ProductType.G92 => 740,
                ProductType.G95 => 760,
                ProductType.DT => 860,
                ProductType.Gas => 500,
                _ => 0,
            };

        public decimal CurrentLevel =>
            CurrentVol * MaxLevel / MaxVol;

        public decimal PercentLevel =>
            CurrentVol * 100 / MaxVol;

        public decimal Weight =>
            Density * CurrentVol / 1000;

        public void Do(DateTime? date = null)
        {
            var _date = date ?? DateTime.Now;

            var fuelCoef = 1M;
            switch (Product)
            {
                case ProductType.DT: fuelCoef = 0.15M; break;
                case ProductType.G95: fuelCoef = 0.92M; break;
                case ProductType.Gas: fuelCoef = 0.07M; break;
                default: break;
            }
            //7 - 9 slow
            //9 - 20 fast
            //20 - 23 slow
            //23 - 7 very slow

            var coef = 1.15M;
            if ((_date.Hour >= 7 && _date.Hour <= 9) || (_date.Hour >= 20 && _date.Hour <= 23))
            {
                coef = 0.85M;
            }
            else if (_date.Hour >= 23 || _date.Hour < 7)
            {
                coef = 0.055M;
            }

            if (State == State.TankDec)
            {
                var rand = new Random().Next(0, 30);
                if (CurrentVol < MaxVol * rand / 100)
                {
                    CurrentVol = MaxVol * new Random().Next(85, 95) / 100;
                }
                else
                {
                    var delta = new Random().Next(0, 45);
                    CurrentVol -= delta * 0.0003M * coef * fuelCoef;
                }
            }
        }

        public async Task<string> SendAsync(string url)
        {
            var pracel = GetPracel();

            //var debug = ActualSensorValue.TryParse(pracel);

            return await Http.PostAsync(url, new Dictionary<string, string>
            {
                { "v", GetPracel() },
                { "d", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff") },
                { "g", DevGuid }
            });
        }

        public string Send(string url, DateTime date)
        {
            var pracel = GetPracel();

            return Http.Post(url, new Dictionary<string, string>
            {
                { "v", GetPracel() },
                { "d", date.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff") },
                { "g", DevGuid }
            });
        }

        private string GetPracel()
        {
            var temp = 18.4M;
            var gasPercent = Product == ProductType.Gas ? 50 : 0;
            var pressure = Product == ProductType.Gas ? 5.5M : 0;

            var str = ":5000010000000000"
                + ((int)(CurrentLevel * 10)).ToString("X").PadLeft(4, '0')
                + "0000"
                + ((int)(pressure * 100)).ToString("X").PadLeft(4, '0')
                + ((int)(PercentLevel * 10)).ToString("X").PadLeft(4, '0')
                + ((int)(CurrentVol * 1000)).ToString("X").PadLeft(6, '0')
                + ((int)(Weight * 1000)).ToString("X").PadLeft(6, '0')
                + "0000"
                + ((int)(Density * 10)).ToString("X").PadLeft(4, '0')
                + "000000000000"
                + ((int)(temp * 10)).ToString("X").PadLeft(4, '0')
                + ((int)(temp * 10)).ToString("X").PadLeft(4, '0')
                + ((int)(temp * 10)).ToString("X").PadLeft(4, '0')
                + ((int)(temp * 10)).ToString("X").PadLeft(4, '0')
                + ((int)(temp * 10)).ToString("X").PadLeft(4, '0')
                + ((int)(temp * 10)).ToString("X").PadLeft(4, '0')
                + "00000000000000"
                + ((int)(gasPercent)).ToString("X").PadLeft(2, '0');

            return str.PadRight(127, '0');
        }
    }
}
