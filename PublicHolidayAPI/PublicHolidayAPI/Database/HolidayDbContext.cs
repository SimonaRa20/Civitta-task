using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using PublicHolidayAPI.Contracts;
using PublicHolidayAPI.Models;
using System.Collections.Generic;

namespace PublicHolidayAPI.Database
{
    public class HolidayDbContext : DbContext
    {
        public HolidayDbContext(DbContextOptions<HolidayDbContext> options)
        : base(options)
        {
            var dbCreater = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (dbCreater != null)
            {
                if (!dbCreater.CanConnect())
                {
                    dbCreater.Create();
                }

                if (!dbCreater.HasTables())
                {
                    dbCreater.CreateTables();
                }
            }
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<HolidayName> HolidayNames { get; set; }
        public DbSet<HolidayNote> HolidayNotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>()
                .OwnsOne(c => c.FromDate);

            modelBuilder.Entity<Country>()
                .OwnsOne(c => c.ToDate);

            modelBuilder.Entity<Country>()
                .Property(c => c.Regions)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            modelBuilder.Entity<Country>()
                .Property(c => c.HolidayTypes)
                .HasConversion(
                    v => string.Join(',', v.Select(h => h.ToString())),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(Enum.Parse<HolidayTypes>).ToList()
                );

            modelBuilder.Entity<Holiday>()
                .HasMany(h => h.Names)
                .WithOne(n => n.Holiday)
                .HasForeignKey(n => n.HolidayId);

            modelBuilder.Entity<Holiday>()
                .HasMany(h => h.Notes)
                .WithOne(n => n.Holiday)
                .HasForeignKey(n => n.HolidayId);
        }
    }
}
