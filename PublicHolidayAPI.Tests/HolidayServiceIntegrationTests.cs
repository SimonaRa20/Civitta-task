using Microsoft.EntityFrameworkCore;
using Moq;
using PublicHolidayAPI.Contracts;
using PublicHolidayAPI.Database;
using PublicHolidayAPI.Interfaces;
using PublicHolidayAPI.Models;
using PublicHolidayAPI.Services;

namespace PublicHolidayAPI.Tests
{
    public class HolidayServiceIntegrationTests
    {
        private HolidayDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<HolidayDbContext>().UseInMemoryDatabase(databaseName: "PublicHolidayDB").Options;
            return new HolidayDbContext(options);
        }

        [Fact]
        public async Task GetCountries_ReturnsCountriesFromDatabase()
        {
            // setup
            var context = GetInMemoryDbContext();
            var kayaposoftApiServiceMock = new Mock<IKayaposoftApiService>();

            context.Countries.Add(new Country
            {
                CountryCode = "US",
                FullName = "United States",
                Regions = new List<string>(),
                HolidayTypes = new List<HolidayTypes> { HolidayTypes.public_holiday },
                FromDate = new DateDetails { Day = 1, Month = 1, Year = 2000 },
                ToDate = new DateDetails { Day = 31, Month = 12, Year = 2100 }
            });

            await context.SaveChangesAsync();

            var service = new HolidayService(kayaposoftApiServiceMock.Object, context);

            // action
            var countries = await service.GetCountriesAsync();

            // assert
            Assert.Single(countries);
            Assert.Equal("US", countries[0].CountryCode);
        }

        [Fact]
        public async Task GetHolidays_ReturnsHolidays_FromService_WhenDbEmpty()
        {
            // setup
            var context = GetInMemoryDbContext();
            var kayaposoftApiServiceMock = new Mock<IKayaposoftApiService>();
            kayaposoftApiServiceMock
                .Setup(service => service.GetHolidaysAsync("US", 2023))
                .ReturnsAsync(new List<Holiday>
                {
                    new Holiday
                    {
                        Name = "New Year's Day",
                        Date = new DateTime(2023, 1, 1),
                        Country = new Country { CountryCode = "US" }
                    }
                });

            var service = new HolidayService(kayaposoftApiServiceMock.Object, context);

            // action
            var holidays = await service.GetHolidaysAsync("US", 2023);

            // assert
            Assert.Single(holidays);
            Assert.Equal("New Year's Day", holidays[0].Holidays[0].Name);
        }

        [Fact]
        public async Task IsPublicHoliday_ReturnsTrue_WhenDateIsHoliday()
        {
            // setup
            var context = GetInMemoryDbContext();
            var kayaposoftApiServiceMock = new Mock<IKayaposoftApiService>();

            var country = new Country
            {
                CountryCode = "US",
                FullName = "United States",
                Regions = new List<string>(),
                HolidayTypes = new List<HolidayTypes> { HolidayTypes.public_holiday },
                FromDate = new DateDetails { Day = 1, Month = 1, Year = 2000 },
                ToDate = new DateDetails { Day = 31, Month = 12, Year = 2100 }
            };

            context.Countries.Add(country);
            await context.SaveChangesAsync();

            context.Holidays.Add(new Holiday
            {
                Name = "Christmas",
                Date = new DateTime(2023, 12, 25),
                Country = country 
            });
            await context.SaveChangesAsync();

            var service = new HolidayService(kayaposoftApiServiceMock.Object, context);

            // action
            var isPublicHoliday = await service.IsPublicHolidayAsync("US", new DateTime(2023, 12, 25));

            // assert
            Assert.True(isPublicHoliday);
        }

        [Fact]
        public async Task IsWorkDay_ReturnsFalse_WhenDateIsHoliday()
        {
            // setup
            var context = GetInMemoryDbContext();
            var kayaposoftApiServiceMock = new Mock<IKayaposoftApiService>();

            var country = new Country
            {
                CountryCode = "US",
                FullName = "United States",
                Regions = new List<string>(),
                HolidayTypes = new List<HolidayTypes> { HolidayTypes.public_holiday },
                FromDate = new DateDetails { Day = 1, Month = 1, Year = 2000 },
                ToDate = new DateDetails { Day = 31, Month = 12, Year = 2100 }
            };

            context.Countries.Add(country);
            await context.SaveChangesAsync();

            context.Holidays.Add(new Holiday
            {
                Name = "Christmas",
                Date = new DateTime(2023, 12, 25),
                Country = country
            });
            await context.SaveChangesAsync();

            var service = new HolidayService(kayaposoftApiServiceMock.Object, context);

            // action
            var isPublicHoliday = await service.IsWorkDayAsync("US", new DateTime(2023, 12, 25));

            // assert
            Assert.False(isPublicHoliday);
        }

        [Fact]
        public async Task GetSpecificDayStatusAsync_ReturnsHolidayStatus_WhenDateIsHoliday()
        {
            // setup
            var context = GetInMemoryDbContext();
            var kayaposoftApiServiceMock = new Mock<IKayaposoftApiService>();

            var country = new Country
            {
                CountryCode = "US",
                FullName = "United States",
                Regions = new List<string>(),
                HolidayTypes = new List<HolidayTypes> { HolidayTypes.public_holiday },
                FromDate = new DateDetails { Day = 1, Month = 1, Year = 2000 },
                ToDate = new DateDetails { Day = 31, Month = 12, Year = 2100 }
            };

            context.Countries.Add(country);
            await context.SaveChangesAsync();

            context.Holidays.Add(new Holiday
            {
                Name = "New Year's Day",
                Date = new DateTime(2023, 1, 1),
                Country = country
            });
            await context.SaveChangesAsync();

            var service = new HolidayService(kayaposoftApiServiceMock.Object, context);

            // action
            var status = await service.GetSpecificDayStatusAsync("US", new DateTime(2023, 1, 1));

            // assert
            Assert.Equal("Holiday", status.Status);
        }

        [Fact]
        public async Task GetSpecificDayStatusAsync_ReturnsWeekendStatus_WhenDateIsWeekend()
        {
            // setup
            var context = GetInMemoryDbContext();
            var kayaposoftApiServiceMock = new Mock<IKayaposoftApiService>();

            var country = new Country
            {
                CountryCode = "US",
                FullName = "United States",
                Regions = new List<string>(),
                HolidayTypes = new List<HolidayTypes> { HolidayTypes.public_holiday },
                FromDate = new DateDetails { Day = 1, Month = 1, Year = 2000 },
                ToDate = new DateDetails { Day = 31, Month = 12, Year = 2100 }
            };

            context.Countries.Add(country);
            await context.SaveChangesAsync();

            kayaposoftApiServiceMock
                .Setup(service => service.GetHolidaysAsync("US", It.IsAny<int>()))
                .ReturnsAsync(new List<Holiday>());

            var service = new HolidayService(kayaposoftApiServiceMock.Object, context);

            // action
            var status = await service.GetSpecificDayStatusAsync("US", new DateTime(2023, 9, 23));

            // assert
            Assert.Equal("Weekend", status.Status);
        }

        [Fact]
        public async Task GetSpecificDayStatusAsync_ReturnsWorkDayStatus_WhenDateIsWorkDay()
        {
            // setup
            var context = GetInMemoryDbContext();
            var kayaposoftApiServiceMock = new Mock<IKayaposoftApiService>();

            var country = new Country
            {
                CountryCode = "US",
                FullName = "United States",
                Regions = new List<string>(),
                HolidayTypes = new List<HolidayTypes> { HolidayTypes.public_holiday },
                FromDate = new DateDetails { Day = 1, Month = 1, Year = 2000 },
                ToDate = new DateDetails { Day = 31, Month = 12, Year = 2100 }
            };

            context.Countries.Add(country);
            await context.SaveChangesAsync();

            kayaposoftApiServiceMock
                .Setup(service => service.GetHolidaysAsync("US", 2023))
                .ReturnsAsync(new List<Holiday>());

            var service = new HolidayService(kayaposoftApiServiceMock.Object, context);

            // action
            var status = await service.GetSpecificDayStatusAsync("US", new DateTime(2023, 9, 25));

            // assert
            Assert.Equal("WorkDay", status.Status);
        }
    }
}
