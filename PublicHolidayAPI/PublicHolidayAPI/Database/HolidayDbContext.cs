using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
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
    }
}
