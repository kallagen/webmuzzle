using System;
using System.Globalization;
using System.Text;

namespace TSensor.Web.Models.Entity
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
        public string pressureMeasuring { get; set; }
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

        public static ActualSensorValue Parse(string raw, bool storeRaw = false)
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
                environmentLevel = (decimal)int.Parse(raw.Substring(17, 4), NumberStyles.HexNumber) / 10,
                pressureFilter = int.Parse(raw.Substring(21, 4), NumberStyles.HexNumber),
                pressureMeasuring = raw.Substring(25, 4),
                levelInPercent = (decimal)int.Parse(raw.Substring(29, 4), NumberStyles.HexNumber) / 10,
                environmentVolume = (decimal)int.Parse(raw.Substring(33, 6), NumberStyles.HexNumber) / 1000,            //
                liquidEnvironmentLevel = (decimal)int.Parse(raw.Substring(39, 6), NumberStyles.HexNumber) / 1000,       //
                steamMass = (decimal)int.Parse(raw.Substring(45, 4), NumberStyles.HexNumber) / 1000,                    //
                liquidDensity = (decimal)int.Parse(raw.Substring(49, 4), NumberStyles.HexNumber) / 10,                  //
                steamDensity = (decimal)int.Parse(raw.Substring(53, 4), NumberStyles.HexNumber) / 10,
                dielectricPermeability = (decimal)int.Parse(raw.Substring(57, 4), NumberStyles.HexNumber) / 1000,
                dielectricPermeability2 = raw.Substring(61, 4),
                t1 = (decimal)short.Parse(raw.Substring(65, 4), NumberStyles.HexNumber) / 10,                           //
                t2 = (decimal)short.Parse(raw.Substring(69, 4), NumberStyles.HexNumber) / 10,
                t3 = (decimal)short.Parse(raw.Substring(73, 4), NumberStyles.HexNumber) / 10,
                t4 = (decimal)short.Parse(raw.Substring(77, 4), NumberStyles.HexNumber) / 10,
                t5 = (decimal)short.Parse(raw.Substring(81, 4), NumberStyles.HexNumber) / 10,
                t6 = (decimal)short.Parse(raw.Substring(85, 4), NumberStyles.HexNumber) / 10,
                plateTemp = (decimal)short.Parse(raw.Substring(89, 4), NumberStyles.HexNumber) / 10,
                period = int.Parse(raw.Substring(93, 4), NumberStyles.HexNumber),
                plateServiceParam = raw.Substring(97, 4),
                environmentComposition = int.Parse(raw.Substring(103, 2), NumberStyles.HexNumber),                       //
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

        public override string ToString()
        {
            var builder = new StringBuilder();
                
            builder.AppendLine($"izkNumber:{izkNumber}");
            builder.AppendLine($"banderolType:{banderolType}");
            builder.AppendLine($"sensorSerial:{sensorSerial}");
            builder.AppendLine($"sensorChannel:{sensorChannel}");
            builder.AppendLine($"pressureAndTempSensorState:{pressureAndTempSensorState}");
            builder.AppendLine($"sensorFirmwareVersionAndReserv:{sensorFirmwareVersionAndReserv}");
            builder.AppendLine($"alarma:{alarma}");
            builder.AppendLine($"environmentLevel:{environmentLevel}");
            builder.AppendLine($"pressureFilter:{pressureFilter}");
            builder.AppendLine($"pressureMeasuring:{pressureMeasuring}");
            builder.AppendLine($"levelInPercent:{levelInPercent}");
            builder.AppendLine($"environmentVolume:{environmentVolume}");
            builder.AppendLine($"liquidEnvironmentLevel:{liquidEnvironmentLevel}");
            builder.AppendLine($"steamMass:{steamMass}");
            builder.AppendLine($"liquidDensity:{liquidDensity}");
            builder.AppendLine($"steamDensity:{steamDensity}");
            builder.AppendLine($"dielectricPermeability:{dielectricPermeability}");
            builder.AppendLine($"dielectricPermeability2:{dielectricPermeability2}");
            builder.AppendLine($"t1:{t1}");
            builder.AppendLine($"t2:{t2}");
            builder.AppendLine($"t3:{t3}");
            builder.AppendLine($"t4:{t4}");
            builder.AppendLine($"t5:{t5}");
            builder.AppendLine($"t6:{t6}");
            builder.AppendLine($"plateTemp:{plateTemp}");
            builder.AppendLine($"period:{period}");
            builder.AppendLine($"plateServiceParam:{plateServiceParam}");
            builder.AppendLine($"environmentComposition:{environmentComposition}");
            builder.AppendLine($"cs1:{cs1}");
            builder.AppendLine($"plateServiceParam2:{plateServiceParam2}");
            builder.AppendLine($"plateServiceParam3:{plateServiceParam3}");
            builder.AppendLine($"sensorWorkMode:{sensorWorkMode}");
            builder.AppendLine($"plateServiceParam4:{plateServiceParam4}");
            builder.AppendLine($"plateServiceParam5:{plateServiceParam5}");
            builder.AppendLine($"crc:{crc}");

            return builder.ToString();
        }

        public static ActualSensorValue Parse(dynamic entity)
        {
            return new ActualSensorValue
            {
                InsertDate = entity.InsertDate,
                DeviceGuid = entity.DeviceGuid,
                IsSecond = entity.IsSecond,
                izkNumber = entity.izkNumber,
                banderolType = entity.banderolType,
                sensorSerial = entity.sensorSerial,
                sensorChannel = entity.sensorChannel,
                pressureAndTempSensorState = entity.pressureAndTempSensorState,
                sensorFirmwareVersionAndReserv = entity.sensorFirmwareVersionAndReserv,
                alarma = entity.alarma,
                environmentLevel = entity.environmentLevel,
                pressureFilter = entity.pressureFilter,
                pressureMeasuring = entity.pressureMeasuring,
                levelInPercent = entity.levelInPercent,
                environmentVolume = entity.environmentVolume,
                liquidEnvironmentLevel = entity.liquidEnvironmentLevel,
                steamMass = entity.steamMass,
                liquidDensity = entity.liquidDensity,
                steamDensity = entity.steamDensity,
                dielectricPermeability = entity.dielectricPermeability,
                dielectricPermeability2 = entity.dielectricPermeability2,
                t1 = entity.t1,
                t2 = entity.t2,
                t3 = entity.t3,
                t4 = entity.t4,
                t5 = entity.t5,
                t6 = entity.t6,
                plateTemp = entity.plateTemp,
                period = entity.period,
                plateServiceParam = entity.plateServiceParam,
                environmentComposition = entity.environmentComposition,
                cs1 = entity.cs1,
                plateServiceParam2 = entity.plateServiceParam2,
                plateServiceParam3 = entity.plateServiceParam3,
                sensorWorkMode = entity.sensorWorkMode,
                plateServiceParam4 = entity.plateServiceParam4,
                plateServiceParam5 = entity.plateServiceParam5,
                crc = entity.crc
            };
        }
    }
}