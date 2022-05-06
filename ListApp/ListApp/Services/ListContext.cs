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
                this.Database.EnsureCreated();
            }
            catch (Exception) { }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "listApp.db3");

            optionsBuilder
                .UseSqlite($"Filename={dbPath}");
        }
    }
}
