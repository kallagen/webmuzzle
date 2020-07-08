using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace TSensor.License.ViewModels
{
    public class LicenseCreateViewModel : ViewModelBase
    {
        [Required(ErrorMessage = "Укажите название")]
        [StringLength(128, ErrorMessage = "Слишком длинное название")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Укажите кол-во датчиков")]
        [Range(0, 1000, ErrorMessage = "Значение должно быть от 0 до 1000")]
        public string SensorCount { get; set; }

        [Required(ErrorMessage = "Укажите дату окончания лицензии")]
        public string ExpireDate { get; set; }

        public int SensorCountParsed { get; set; }
        public DateTime ExpireDateParsed { get; set; }

        public void Validate(ModelStateDictionary modelState)
        {
            if (string.IsNullOrWhiteSpace(SensorCount))
            {
                modelState?.AddModelError("SensorCount", "Укажите кол-во датчиков");
            }
            else
            {
                if (int.TryParse(SensorCount, out var _sensorCount))
                {
                    if (_sensorCount < 0 || _sensorCount > 1000)
                    {
                        modelState?.AddModelError("SensorCount", "Значение должно быть от 0 до 1000");
                    }
                    else
                    {
                        SensorCountParsed = _sensorCount;
                    }
                }
                else
                {
                    modelState?.AddModelError("DaysAmount", "Значение должно быть от 1 до 1095");
                }
            }

            if (string.IsNullOrWhiteSpace(ExpireDate))
            {
                modelState?.AddModelError("ExpireDate", "Укажите дату окончания лицензии");
            }
            else
            {
                if (DateTime.TryParseExact(ExpireDate, new[] { "dd.MM.yyyy", "dd.MM.yyyy HH:mm", "dd.MM.yyyy H:mm" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var _expireDate))
                {
                    ExpireDateParsed = _expireDate;
                }
                else
                {
                    modelState?.AddModelError("ExpireDate", "Укажите корректную дату окончания лицензии");
                }
            }
        }
    }
}