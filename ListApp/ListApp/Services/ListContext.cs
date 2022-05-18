using ListApp.Models;
using ListApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using Xamarin.Essentials;

namespace ListApp.Services
{
    public class ListContext : DbContext
    {
        private ILogger _logger;
        public DbSet<List> Lists { get; set; }
        public DbSet<ListItem> ListItems { get; set; }

        public ListContext(ILogger logger)
        {
            _logger = logger;

            try
            {
                SQLitePCL.Batteries_V2.Init();

                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "listApp.db3");
                if (!File.Exists(dbPath))
                    File.Create(dbPath);

                this.Database.Migrate();
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                string dbPath;
                if (DeviceInfo.Platform == DevicePlatform.Unknown)
                    dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                else
                    dbPath = Path.Combine(FileSystem.AppDataDirectory, "listApp.db3");

                optionsBuilder
                    .UseSqlite($"Filename={dbPath}");
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }
    }
}
