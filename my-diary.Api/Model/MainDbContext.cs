using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my_diary.Api.Model
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //auto increment identity columns
            modelBuilder.UseSerialColumns();
        }

        public DbSet<Entry> Entries { get; set; }
        public DbSet<User>  Users { get; set; }
    }
}
