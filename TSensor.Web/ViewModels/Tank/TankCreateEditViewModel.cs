using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TSensor.Web.ViewModels.Tank
{
    public class TankCreateEditViewModel : ViewModelBase
    {
        public Guid? TankGuid { get; set; }
        public Guid? PointGuid { get; set; }
        public string PointName { get; set; }
        [Required(ErrorMessage = "Укажите название")]
        [StringLength(128, ErrorMessage = "Слишком длинное название")]
        public string Name { get; set; }
        public bool DualMode { get; set; }
        [StringLength(5, ErrorMessage = "Значение должно быть не больше 5 символов")]
        public string MainDeviceGuid { get; set; }
        [StringLength(2, ErrorMessage = "Значение должно быть не больше 2 символов")]
        public string MainIZKId { get; set; }
        [StringLength(2, ErrorMessage = "Значение должно быть не больше 2 символов")]
        public string MainSensorId { get; set; }
        [StringLength(5, ErrorMessage = "Значение должно быть не больше 5 символов")]
        public string SecondDeviceGuid { get; set; }
        [StringLength(2, ErrorMessage = "Значение должно быть не больше 2 символов")]
        public string SecondIZKId { get; set; }
        [StringLength(2, ErrorMessage = "Значение должно быть не больше 2 символов")]
        public string SecondSensorId { get; set; }
        public string Description { get; set; }
        public Guid? ProductGuid { get; set; }
        public string WeightChangeDelta { get; set; }
        public string WeightChangeTimeout { get; set; }

        public IEnumerable<Models.Entity.Product> ProductList { get; set; }

        public decimal? ParsedWeightChangeDelta { get; set; }
        public int? ParsedWeightChangeTimeout { get; set; }

        public void Validate(ModelStateDictionary modelState)
        {
            if (!string.IsNullOrEmpty(WeightChangeDelta))
            {
                if (!decimal.TryParse(WeightChangeDelta, out var weightChangeDelta))
                {
                    modelState?.AddModelError("WeightChangeDelta", "Неверный формат значения");
                }
                else
                {
                    if (weightChangeDelta < 0 || weightChangeDelta > 1000)
                    {
                        modelState?.AddModelError("WeightChangeDelta", "Значение должно быть в диапазоне от 0 до 1000 тонн");
                    }
                    else
                    {
                        ParsedWeightChangeDelta = weightChangeDelta;
                    }
                }
            }
            if (!string.IsNullOrEmpty(WeightChangeTimeout))
            {
                if (!int.TryParse(WeightChangeTimeout, out var weightChangeTimeout))
                {
                    modelState?.AddModelError("WeightChangeTimeout", "Неверный формат значения");
                }
                else
                {
                    if (weightChangeTimeout < 0 || weightChangeTimeout > 86400)
                    {
                        modelState?.AddModelError("WeightChangeTimeout", "Значение должно быть в диапазоне от 0 до 86400 секунд");
                    }
                    else
                    {
                        ParsedWeightChangeTimeout = weightChangeTimeout;
                    }
                }
            }
        }
    }
}
