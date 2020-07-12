using System;
using Newtonsoft.Json;
using SQLite;

namespace AirMonitor.Models.Tables
{
    public class InstallationEntity
    {
        public InstallationEntity()
        {
        }

        public InstallationEntity(Installation installation)
        {
            if (installation == null) return;

            Id = installation.Id;
            Location = JsonConvert.SerializeObject(installation.Location);
            Address = JsonConvert.SerializeObject(installation.Address);
            Elevation = installation.Elevation;
            IsAirlyInstallation = installation.IsAirlyInstallation;
        }

        [PrimaryKey]
        public string Id { get; set; }

        public string Location { get; set; }
        public string Address { get; set; }
        public double Elevation { get; set; }
        public bool IsAirlyInstallation { get; set; }
    }
}