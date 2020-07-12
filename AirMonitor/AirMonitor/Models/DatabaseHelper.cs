﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AirMonitor.Models;
using AirMonitor.Models.Tables;
using Newtonsoft.Json;
using SQLite;

namespace AirMonitor.Helpers
{
    public class DatabaseHelper : IDisposable
    {
        private SQLiteConnection db;

        public void Initialize()
        {
            string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AirMonitorDatabase.db");

            db = new SQLiteConnection(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);

            db.CreateTable<InstallationEntity>();
            db.CreateTable<MeasurementEntity>();
            db.CreateTable<MeasurementItemEntity>();
            db.CreateTable<MeasurementValue>();
            db.CreateTable<AirQualityIndex>();
            db.CreateTable<AirQualityStandard>();
        }

        public void SaveInstallations(IEnumerable<Installation> installations)
        {
            IEnumerable<InstallationEntity> entities = installations.Select(installation => new InstallationEntity(installation));

            db.DeleteAll<InstallationEntity>();
            db.InsertAll(entities);
        }

        public IEnumerable<Installation> GetInstallations()
        {
            IEnumerable<Installation> installations = db.Table<InstallationEntity>().Select(entity => new Installation(entity));

            return installations;
        }

        public void SaveMeasurements(IEnumerable<Measurement> measurements)
        {
            db.DeleteAll<MeasurementValue>();
            db.DeleteAll<AirQualityIndex>();
            db.DeleteAll<AirQualityStandard>();
            db.DeleteAll<MeasurementItemEntity>();
            db.DeleteAll<MeasurementEntity>();

            foreach (var measurement in measurements)
            {
                db.InsertAll(measurement.Current.Values);
                db.InsertAll(measurement.Current.Indexes);
                db.InsertAll(measurement.Current.Standards);

                MeasurementItemEntity measurementItemEntity = new MeasurementItemEntity(measurement.Current);
                db.Insert(measurementItemEntity);

                MeasurementEntity measurementEntity = new MeasurementEntity(measurementItemEntity.Id, measurement.Installation.Id);
                db.Insert(measurementEntity);
            }
        }

        public IEnumerable<Measurement> GetMeasurements()
        {
            return db.Table<MeasurementEntity>().Select(s =>
            {
                var measurementItem = GetMeasurementItem(s.Current);
                var installation = GetInstallation(s.Installation);
                return new Measurement(measurementItem, installation);
            });
        }

        private MeasurementItem GetMeasurementItem(int id)
        {
            var entity = db.Get<MeasurementItemEntity>(id);
            var valueIds = JsonConvert.DeserializeObject<int[]>(entity.MeasurementValueIds);
            var indexIds = JsonConvert.DeserializeObject<int[]>(entity.AirQualityIndexIds);
            var standardIds = JsonConvert.DeserializeObject<int[]>(entity.AirQualityStandardIds);
            var values = db.Table<MeasurementValue>().Where(s => valueIds.Contains(s.Id)).ToArray();
            var indexes = db.Table<AirQualityIndex>().Where(s => indexIds.Contains(s.Id)).ToArray();
            var standards = db.Table<AirQualityStandard>().Where(s => standardIds.Contains(s.Id)).ToArray();
            return new MeasurementItem(entity, values, indexes, standards);
        }

        private Installation GetInstallation(string id)
        {
            var entity = db.Get<InstallationEntity>(id);
            return new Installation(entity);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    db.Dispose();
                    db = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}