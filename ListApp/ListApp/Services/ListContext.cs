using ListApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using Xamarin.Essentials;

namespace ListApp.Services
{
    public class ListContext : DbContext
    {
        public DbSet<List> Lists { get; set; }
        public DbSet<ListItem> ListItems { get; set; }

        public ListContext()
        {
            SQLitePCL.Batteries_V2.Init();

            try
            {
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "listApp.db3");
                if (!File.Exists(dbPath))
                    File.Create(dbPath);

                this.Database.Migrate();
            }
            catch (Exception) { }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath;
            if (DeviceInfo.Platform == DevicePlatform.Unknown)
                dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            else
                dbPath = Path.Combine(FileSystem.AppDataDirectory, "listApp.db3");

            optionsBuilder
                .UseSqlite($"Filename={dbPath}");
        }
    }
}
