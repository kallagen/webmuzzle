using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TSensor.Web.ViewModels.Point
{
    public class PointCreateEditViewModel : SearchViewModel<Models.Entity.Tank>
    {
        public Guid PointGuid { get; set; }
        [Required(ErrorMessage = "Укажите название")]
        [StringLength(128, ErrorMessage = "Слишком длинное название")]
        public string Name { get; set; }
        [StringLength(128, ErrorMessage = "Слишком длинный адрес")]
        public string Address { get; set; }
        [StringLength(128, ErrorMessage = "Слишком длинный телефон")]
        public string Phone { get; set; }
        [StringLength(128, ErrorMessage = "Слишком длинный email")]
        public string Email { get; set; }
        public string Description { get; set; }

        public string Longitude { get; set; }
        public string Latitude { get; set; }        

        public decimal? LongitudeParsed =>
            decimal.TryParse(Longitude, out var _longitude) ? (decimal?)_longitude : null;
        public decimal? LatitudeParsed =>
            decimal.TryParse(Latitude, out var _latitude) ? (decimal?)_latitude : null;

        public decimal DefaultLongitude { get; set; }
        public decimal DefaultLatitude { get; set; }        

        public Models.Entity.MapSettings MapSettings { get; set; }

        public IEnumerable<Models.Entity.User> UserList { get; set; }
        public bool HasUser =>
            UserList?.Any() == true;

        public IEnumerable<Models.Entity.User> AvailableUserList { get; set; }
        public bool HasAvailableUser =>
            AvailableUserList?.Any() == true;

        public void Validate(ModelStateDictionary modelState)
        {
            if (!string.IsNullOrEmpty(Longitude))
            {
                if (!decimal.TryParse(Longitude, out var longitudeParsed))
                {
                    modelState?.AddModelError("Longitude", "Неверный формат значения");
                }
                else
                {
                    if (longitudeParsed < -180 || longitudeParsed > 180)
                    {
                        modelState?.AddModelError("Longitude", "Значение должно быть в диапазоне от -180 до 180 градусов");
                    }
                }
            }
            if (!string.IsNullOrEmpty(Latitude))
            {
                if (!decimal.TryParse(Latitude, out var latitudeParsed))
                {
                    modelState?.AddModelError("Latitude", "Неверный формат значения");
                }
                else
                {
                    if (latitudeParsed < -90 || latitudeParsed > 90)
                    {
                        modelState?.AddModelError("Latitude", "Значение должно быть в диапазоне от -90 до 90 градусов");
                    }
                }
            }            
        }
    }
}