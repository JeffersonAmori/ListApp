using ListApp.Api.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace ListApp.Api.Database
{
    public class ListContext : DbContext
    {
        public ListContext() : base() { }

        public ListContext(DbContextOptions<ListContext> options) : base(options) { }

        public DbSet<List> Lists { get; set; }
        public DbSet<ListItem> ListItems { get; set; }
    }
}
