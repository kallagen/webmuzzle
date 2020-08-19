using System;
using System.Globalization;

namespace TSensor.FakeSensor
{
    public class ActualSensorValue
    {
        public string DeviceGuid { get; set; }
        public DateTime EventUTCDate { get; set; }
        public string Raw { get; set; }
        public Guid? TankGuid { get; set; }

        public bool? IsSecond { get; set; }

        public DateTime InsertDate { get; set; }
        public string SensorGuid =>
            string.Join("_", new[] { DeviceGuid, izkNumber.ToString(), sensorSerial.ToString() });
        public string InsertDateStr =>
            InsertDate.ToString("dd.MM.yyyy HH:mm:ss");
        public decimal AvgT =>
            decimal.Round((t1 + t2 + t3 + t4 + t5 + t6) / 6, 1);
        public int PercentLevel =>
            (int)decimal.Round(levelInPercent, 0);

        public int izkNumber { get; set; }
        public int banderolType { get; set; }
        public int sensorSerial { get; set; }
        public int sensorChannel { get; set; }
        public string pressureAndTempSensorState { get; set; }
        public string sensorFirmwareVersionAndReserv { get; set; }
        public string alarma { get; set; }
        public decimal environmentLevel { get; set; }
        public int pressureFilter { get; set; }
        public decimal pressureMeasuring { get; set; }
        public decimal levelInPercent { get; set; }
        public decimal environmentVolume { get; set; }
        public decimal liquidEnvironmentLevel { get; set; }
        public decimal steamMass { get; set; }
        public decimal liquidDensity { get; set; }
        public decimal steamDensity { get; set; }
        public decimal dielectricPermeability { get; set; }
        public string dielectricPermeability2 { get; set; }
        public decimal t1 { get; set; }
        public decimal t2 { get; set; }
        public decimal t3 { get; set; }
        public decimal t4 { get; set; }
        public decimal t5 { get; set; }
        public decimal t6 { get; set; }
        public decimal plateTemp { get; set; }
        public int period { get; set; }
        public string plateServiceParam { get; set; }
        public int environmentComposition { get; set; }
        public decimal cs1 { get; set; }
        public decimal plateServiceParam2 { get; set; }
        public decimal plateServiceParam3 { get; set; }
        public decimal sensorWorkMode { get; set; }
        public decimal plateServiceParam4 { get; set; }
        public int plateServiceParam5 { get; set; }
        public string crc { get; set; }

        public decimal gasMass =>
            liquidEnvironmentLevel + steamMass * 10;

        public static ActualSensorValue TryParse(string raw, bool storeRaw = false)
        {
            try
            {
                var entity = new ActualSensorValue
                {
                    izkNumber = int.Parse(raw.Substring(1, 2), NumberStyles.HexNumber),
                    banderolType = int.Parse(raw.Substring(3, 2), NumberStyles.HexNumber),
                    sensorSerial = int.Parse(raw.Substring(5, 2), NumberStyles.HexNumber),
                    sensorChannel = int.Parse(raw.Substring(9, 2), NumberStyles.HexNumber),
                    pressureAndTempSensorState = raw.Substring(11, 2),
                    sensorFirmwareVersionAndReserv = raw.Substring(13, 2),
                    alarma = raw.Substring(15, 2),
                    environmentLevel = (decimal)int.Parse(raw.Substring(17, 4), NumberStyles.HexNumber) / 10, // уровень среды
                    pressureFilter = int.Parse(raw.Substring(21, 4), NumberStyles.HexNumber),
                    pressureMeasuring = (decimal)int.Parse(raw.Substring(25, 4), NumberStyles.HexNumber) / 100,
                    levelInPercent = (decimal)int.Parse(raw.Substring(29, 4), NumberStyles.HexNumber) / 10, // объем %
                    environmentVolume = (decimal)int.Parse(raw.Substring(33, 6), NumberStyles.HexNumber) / 1000, // объем
                    liquidEnvironmentLevel = (decimal)int.Parse(raw.Substring(39, 6), NumberStyles.HexNumber) / 1000, // масса жидкой среды
                    steamMass = (decimal)int.Parse(raw.Substring(45, 4), NumberStyles.HexNumber) / 1000, // масса пара
                    liquidDensity = (decimal)int.Parse(raw.Substring(49, 4), NumberStyles.HexNumber) / 10, // плотность жидкости
                    steamDensity = (decimal)int.Parse(raw.Substring(53, 4), NumberStyles.HexNumber) / 10, // плотность пара
                    dielectricPermeability = (decimal)int.Parse(raw.Substring(57, 4), NumberStyles.HexNumber) / 1000,
                    dielectricPermeability2 = raw.Substring(61, 4),
                    t1 = (decimal)short.Parse(raw.Substring(65, 4), NumberStyles.HexNumber) / 10,
                    t2 = (decimal)short.Parse(raw.Substring(69, 4), NumberStyles.HexNumber) / 10,
                    t3 = (decimal)short.Parse(raw.Substring(73, 4), NumberStyles.HexNumber) / 10,
                    t4 = (decimal)short.Parse(raw.Substring(77, 4), NumberStyles.HexNumber) / 10,
                    t5 = (decimal)short.Parse(raw.Substring(81, 4), NumberStyles.HexNumber) / 10,
                    t6 = (decimal)short.Parse(raw.Substring(85, 4), NumberStyles.HexNumber) / 10,
                    plateTemp = (decimal)short.Parse(raw.Substring(89, 4), NumberStyles.HexNumber) / 10,
                    period = int.Parse(raw.Substring(93, 4), NumberStyles.HexNumber),
                    plateServiceParam = raw.Substring(97, 4),
                    environmentComposition = int.Parse(raw.Substring(103, 2), NumberStyles.HexNumber),
                    cs1 = (decimal)int.Parse(raw.Substring(105, 4), NumberStyles.HexNumber) / 100,
                    plateServiceParam2 = (decimal)int.Parse(raw.Substring(109, 4), NumberStyles.HexNumber) / 10,
                    plateServiceParam3 = (decimal)(int.Parse(raw.Substring(113, 4), NumberStyles.HexNumber) - 65536) / 100,
                    sensorWorkMode = (decimal)int.Parse(raw.Substring(117, 2), NumberStyles.HexNumber) / 10,
                    plateServiceParam4 = (decimal)int.Parse(raw.Substring(119, 2), NumberStyles.HexNumber) / 10,
                    plateServiceParam5 = int.Parse(raw.Substring(121, 4), NumberStyles.HexNumber),
                    crc = raw.Substring(125, 2)
                };

                if (storeRaw)
                {
                    entity.Raw = raw;
                }

                return entity;
            }
            catch
            {
                return null;
            }
        }
    }
}