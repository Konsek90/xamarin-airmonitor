using System;
using Newtonsoft.Json;
using SQLite;

namespace AirMonitor.Models.Tables
{
    public class MeasurementEntity
    {
        public MeasurementEntity()
        {
        }

        public MeasurementEntity(int current, string installation)
        {
            Current = current;
            Installation = installation;
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int Current { get; set; }
        public string Installation { get; set; }
    }
}