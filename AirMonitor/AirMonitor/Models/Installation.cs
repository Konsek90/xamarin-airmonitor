using System;
using AirMonitor.Models.Tables;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace AirMonitor.Models
{
    public class Installation
    {
        public Installation()
        {
        }

        public Installation(InstallationEntity entity)
        {
            Id = entity.Id;
            Location = JsonConvert.DeserializeObject<Location>(entity.Location);
            Address = JsonConvert.DeserializeObject<Address>(entity.Address);
            Elevation = entity.Elevation;
            IsAirlyInstallation = entity.IsAirlyInstallation;
        }

        public string Id { get; set; }
        public Location Location { get; set; }
        public Address Address { get; set; }
        public double Elevation { get; set; }

        [JsonProperty(PropertyName = "airly")]
        public bool IsAirlyInstallation { get; set; }
    }
}