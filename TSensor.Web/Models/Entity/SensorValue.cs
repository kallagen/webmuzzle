using System;

namespace TSensor.Web.Models.Entity
{
    public class SensorValue
    {
        public string DeviceGuid { get; set; }
        public int IzkNumber { get; set; }
        public int SensorSerial { get; set; }
        public DateTime InsertDate { get; set; }
        public decimal EnvironmentLevel { get; set; }
        public decimal LevelInPercent { get; set; }
        public decimal EnvironmentVolume { get; set; }
        public decimal LiquidEnvironmentLevel { get; set; }
        public decimal LiquidDensity { get; set; }
        public int PressureFilter { get; set; }
        public decimal T1 { get; set; }
        public decimal T2 { get; set; }
        public decimal T3 { get; set; }
        public decimal T4 { get; set; }
        public decimal T5 { get; set; }
        public decimal T6 { get; set; }

        public string SensorGuid =>
            string.Join("_", new[] { DeviceGuid, IzkNumber.ToString(), SensorSerial.ToString() });
        public string InsertDateStr =>
            InsertDate.ToString("dd.MM.yyyy HH:mm:ss");                
        public decimal AvgT =>
            decimal.Round((T1 + T2 + T3 + T4 + T5 + T6) / 6, 1);
        public int PercentLevel =>
            (int)decimal.Round(LevelInPercent, 0);
    }
}
